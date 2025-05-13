using System.Text;
﻿using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected; // Added for mocking protected members like SendAsync
using NUnit.Framework;
using WaterNut.Business.Services.Utils;
using System.Net.Http;
using NUnit.Framework.Legacy;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Reflection; // Added for reflection

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class DeepSeekApiTests : IDisposable
    {

        [TestFixture]
        public class ShipmentInvoiceTests
        {


            [Test]
            public async Task ExtractShipmentInvoice_ProcessesSampleText_ReturnsValidDocuments()
            {
                // Arrange
                
                // Use a mock logger to prevent FileLoadException during test setup
                var mockLogger = new Mock<ILogger<DeepSeekInvoiceApi>>();
                // Use the constructor that allows injecting a logger and HttpClient
                // Pass null for HttpClient to use the default, or a mock if needed for other tests
                var api = new DeepSeekInvoiceApi(null); // Pass null for HttpClient, will use default
                // Use reflection to inject the mock logger into the private field
                var loggerField = typeof(DeepSeekInvoiceApi).GetField("_logger", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (loggerField == null)
                {
                    Assert.Fail("Could not find private field '_logger' via reflection.");
                }
                loggerField.SetValue(api, mockLogger.Object);

                var textVariants = new List<string> { SampleText };

                // Act
                var results = await api.ExtractShipmentInvoice(textVariants).ConfigureAwait(false);

                // Assert - Invoices
                var invoice = results.FirstOrDefault(d => d["DocumentType"] as string == "ShipmentInvoice");
                Assert.That(invoice, Is.Not.Null, "No invoice found");
                Assert.That(invoice["InvoiceNo"], Is.EqualTo("138845514"));
                Assert.That(invoice["InvoiceDate"], Is.EqualTo(DateTime.Parse("2024-07-15")));
                Assert.That(invoice["Total"], Is.EqualTo(83.17m).Within(0.01m));
                Assert.That(invoice["Currency"], Is.EqualTo("USD"));

                // Assert Line Items
                var lineItems = invoice["InvoiceDetails"] as List<IDictionary<string, object>>;
                Assert.That(lineItems.Count, Is.EqualTo(9));
                Assert.That(lineItems[0]["ItemDescription"], Is.EqualTo("Going Viral Handbag - Silver"));
                Assert.That(lineItems[0]["Quantity"], Is.EqualTo(1m));

                // Assert - Customs Declaration
                var customs = results.FirstOrDefault(d => d["DocumentType"] as string == "SimplifiedDeclaration");
                Assert.That(customs, Is.Not.Null, "No customs declaration found");
                Assert.That(customs["Consignee"], Is.EqualTo("ARTISHA CHARLES"));
                Assert.That(customs["BLNumber"], Is.EqualTo("HAWB9592028"));

                // Assert Package Info
                var packageInfo = customs["PackageInfo"] as IDictionary<string, object>;
                Assert.That(packageInfo["Count"], Is.EqualTo(1));
                Assert.That(packageInfo["WeightKG"], Is.EqualTo(6.0m)); // Fixed property name

                //// Assert - Goods Classifications
                //var goods = customs["Goods"] as List<IDictionary<string, object>>;
                //Assert.That(goods, Is.Not.Null);
                //Assert.That(goods.Count, Is.GreaterThan(0));

                //// Updated assertion for empty tariff code from sample response
                //Assert.That(goods[0]["TariffCode"], Is.Empty.Or.Null);
            }

            [Test]
            public void ValidateTariffCode_CleansHsCodesCorrectly()
            {
                var api = new DeepSeekInvoiceApi();

                Assert.Multiple(() =>
                {
                    Assert.That(api.ValidateTariffCode("8481.80.0000"), Is.EqualTo("84818000"));
                    Assert.That(api.ValidateTariffCode("1234-56"), Is.EqualTo("1234-56"));
                    Assert.That(api.ValidateTariffCode("invalid"), Is.Empty);
                    Assert.That(api.ValidateTariffCode(""), Is.Empty);
                });
            }
        }

        private class MockHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _response;
            public MockHttpMessageHandler(string response) => _response = response;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(_response)
                });
            }
        }


        [Test]
        public async Task ClassifyItems_ProcessesMixedItems_CorrectlyPopulatesMissingFields()
        {
            // Arrange
            var api = new DeepSeekApi();
            var testItems = new List<(string, string, string)>
            {
                // Format: (ItemNumber, Description, TariffCode)
                ("", "Stainless steel kitchen knife", ""),      // Missing both
                ("", "Wireless Bluetooth headphones", "8518"),  // Missing item number
                ("PC-123", "Leather laptop bag", ""),            // Missing tariff code
                ("MON-456", "LCD Computer Monitor", "8528.52")   // Complete
            };

            // Act
            var results = await api.ClassifyItemsAsync(testItems).ConfigureAwait(false);

            // Assert
            foreach (var kvp in results)
            {
                var description = kvp.Key;
                var item = kvp.Value;

                // Should never be empty after processing
                Assert.That(item.ItemNumber, Is.Not.Null.And.Not.Empty,
                    $"Item number missing for {description}");
                Assert.That(item.TariffCode, Is.Not.Null.And.Not.Empty,
                    $"Tariff code missing for {description}");

                // Item number validation
                Assert.That(item.ItemNumber.Length, Is.AtMost(20),
                    $"Item number too long for {description}");
                StringAssert.IsMatch(@"^[\w-]+$", item.ItemNumber,
                    $"Invalid characters in item number for {description}");

                // Tariff code validation
                StringAssert.IsMatch(@"^\d{4}[-]?(\d{2,4}[-]?)*$", item.TariffCode,
                    $"Invalid HS code format for {description}");
            }

            // Specific case validations
            var knifeEntry = results["Stainless steel kitchen knife"];
            Assert.That(knifeEntry.ItemNumber.Length, Is.LessThanOrEqualTo(20));

            var headphonesEntry = results["Wireless Bluetooth headphones"];
            Assert.That(headphonesEntry.TariffCode, Does.StartWith("8518"));

            var laptopBagEntry = results["Leather laptop bag"];
            Assert.That(laptopBagEntry.TariffCode, Is.Not.EqualTo(""));
        }

        [Test]
        public async Task ClassifyItems_SanitizesAllInputFields()
        {
            var api = new DeepSeekApi();
            var testItems = new List<(string, string, string)>
            {
                ("Invalid|Item#", "Dirty\nDescription", "Invalid/HS-Code"),
                ("", "Wireless Mouse", "8517.60.00")
            };

            var results = await api.ClassifyItemsAsync(testItems).ConfigureAwait(false);

            // Verify item number sanitization
            var mouseEntry = results["Wireless Mouse"];
            Assert.That(mouseEntry.ItemNumber, Does.Match(api.ItemNumberPattern));

            // Verify tariff code cleaning
            var invalidEntry = results["Dirty Description"];
            Assert.That(invalidEntry.TariffCode, Is.Not.EqualTo("Invalid/HS-Code"));

            // Verify existing valid HS code preservation
            Assert.That(results["Wireless Mouse"].TariffCode, Is.EqualTo("85176000"));
        }

        [Test]
        public async Task ClassifyItemsAsync_WithCategory_ReturnsCorrectData()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DeepSeekApi>>();
            var dummyApiKey = "test-api-key"; // Not used due to mocked HTTP handler

            // 1. Define Mock HTTP Response (including category fields)
            var mockJsonResponse = @"{
                ""items"": [
                    {
                        ""original_description"": ""Blue Cotton T-Shirt"",
                        ""product_code"": ""TS-BLU-COT"",
                        ""category"": ""Apparel"",
                        ""category_hs_code"": ""61000000"",
                        ""hs_code"": ""61091000""
                    },
                    {
                        ""original_description"": ""Wireless Mouse"",
                        ""product_code"": ""MOUSE-WL"",
                        ""category"": ""Electronics"",
                        ""category_hs_code"": ""85000000"",
                        ""hs_code"": ""85176000""
                    }
                ]
            }";

            var mockResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(mockJsonResponse, Encoding.UTF8, "application/json")
            };

            // Use Moq for HttpMessageHandler
            var mockHttpHandler = new Mock<HttpMessageHandler>();
            mockHttpHandler
                .Protected() // Needed to mock SendAsync
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(mockResponse);

            var mockHttpClient = new HttpClient(mockHttpHandler.Object);

            // 2. Instantiate DeepSeekApi (using constructor that takes logger/key)
            // We need to provide a valid base URL even though it won't be hit
            var api = new DeepSeekApi(mockLogger.Object, dummyApiKey);

            // 3. Use Reflection to inject the mock HttpClient
            var httpClientField = typeof(DeepSeekApi).GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (httpClientField == null)
            {
                Assert.Fail("Could not find private field '_httpClient' via reflection.");
            }
            // Dispose the original HttpClient created by the constructor before replacing it
            var originalHttpClient = httpClientField.GetValue(api) as HttpClient;
            originalHttpClient?.Dispose();
            // Set the mock client
            httpClientField.SetValue(api, mockHttpClient);


            // 4. Define Input Items
            var inputItems = new List<(string ItemNumber, string ItemDescription, string TariffCode)>
            {
                ("TS-BLU-COT", "Blue Cotton T-Shirt", ""), // Needs HS, Category, CatHS
                ("MOUSE-WL", "Wireless Mouse", "85176000") // Needs Category, CatHS (HS provided)
            };

            // 5. Define Expected Output
            var expectedResult = new Dictionary<string, (string ItemNumber, string ItemDescription, string TariffCode, string Category, string CategoryTariffCode)>
            {
                { "Blue Cotton T-Shirt", ("TS-BLU-COT", "Blue Cotton T-Shirt", "61091000", "Apparel", "61000000") },
                { "Wireless Mouse", ("MOUSE-WL", "Wireless Mouse", "85176000", "Electronics", "85000000") } // Expects original TariffCode to be kept
            };

            // Act
            var actualResult = await api.ClassifyItemsAsync(inputItems, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.That(actualResult, Is.Not.Null);
            Assert.That(actualResult.Count, Is.EqualTo(expectedResult.Count), "Result dictionary count mismatch.");

            foreach (var expectedPair in expectedResult)
            {
                Assert.That(actualResult.ContainsKey(expectedPair.Key), Is.True, $"Actual result missing key: {expectedPair.Key}");
                var actualValue = actualResult[expectedPair.Key];
                var expectedValue = expectedPair.Value;

                Assert.Multiple(() =>
                {
                    Assert.That(actualValue.ItemNumber, Is.EqualTo(expectedValue.ItemNumber), $"ItemNumber mismatch for '{expectedPair.Key}'");
                    Assert.That(actualValue.ItemDescription, Is.EqualTo(expectedValue.ItemDescription), $"ItemDescription mismatch for '{expectedPair.Key}'");
                    Assert.That(actualValue.TariffCode, Is.EqualTo(expectedValue.TariffCode), $"TariffCode mismatch for '{expectedPair.Key}'");
                    Assert.That(actualValue.Category, Is.EqualTo(expectedValue.Category), $"Category mismatch for '{expectedPair.Key}'");
                    Assert.That(actualValue.CategoryTariffCode, Is.EqualTo(expectedValue.CategoryTariffCode), $"CategoryTariffCode mismatch for '{expectedPair.Key}'");
                });
            }
             // Dispose the API instance which should dispose the HttpClient we injected
            api.Dispose();
        }




        [Test]
        [Category("Integration")]
        public async Task GetTariffCode_RealApi_WomensDress()
        {
            using var realApi = new DeepSeekApi();
            var result =   await realApi.GetClassificationInfoAsync("Women's cotton dress with polyester lining").ConfigureAwait(false);

            Assert.That(result, Does.Match(_deepSeekApi.HsCodePattern));
            Console.WriteLine($"Women's Dress HS Code: {result}");
        }

        [Test]
        [Category("Integration")]
        public async Task GetTariffCode_RealApi_WoodenChairs()
        {
            using var realApi = new DeepSeekApi();
            var result = await realApi.GetClassificationInfoAsync("Oak wooden dining chairs with upholstered seats").ConfigureAwait(false);

            Assert.That(result, Does.Match(_deepSeekApi.HsCodePattern));
            Console.WriteLine($"Wooden Chairs HS Code: {result}");
        }

        [Test]
        [Category("Integration")]
        public async Task GetTariffCode_RealApi_ComplexProduct()
        {
            using var realApi = new DeepSeekApi();
            var description = "MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial";

            var result = await realApi.GetClassificationInfoAsync(description).ConfigureAwait(false);

            Assert.That(result, Does.Match(_deepSeekApi.HsCodePattern));
            Console.WriteLine($"LED Display HS Code: {result}");
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators - NUnit's Assert.ThrowsAsync handles the await internally
        [Test]
        [Category("Integration")]
        public async Task GetTariffCode_RealApi_InvalidProduct()
        {
            using var realApi = new DeepSeekApi();

            // Assert.ThrowsAsync handles the awaiting of the async delegate.
            // The CS1998 warning is likely a compiler limitation here.
            Assert.ThrowsAsync<DeepSeekApi.HSCodeRequestException>(async () =>
                await realApi.GetClassificationInfoAsync("Non-existent imaginary product").ConfigureAwait(false));
        }
#pragma warning restore CS1998

        // Add this to existing mock setup section
        private DeepSeekApi _deepSeekApi;
        private Mock<HttpMessageHandler> _mockHttpHandler;
        private HttpClient _mockHttpClient;

        public DeepSeekApiTests()
        {
            _mockHttpHandler = new Mock<HttpMessageHandler>();
            _mockHttpClient = new HttpClient(_mockHttpHandler.Object);
            // Use a mock logger to avoid FileLoadException during test setup
            var mockLogger = new Mock<ILogger<DeepSeekApi>>();
            // Use the parameterized constructor to inject the mock logger
            _deepSeekApi = new DeepSeekApi(mockLogger.Object, "dummy-api-key");
            // The mock HttpClient is injected later in the ClassifyItemsAsync_WithCategory_ReturnsCorrectData test
        }

        public void Dispose()
        {
            _mockHttpClient?.Dispose();
            _deepSeekApi?.Dispose();
        }

        private const string SampleText = @"
------------------------------------------Single Column-------------------------
~s

NEED HELP?

Shipping options, prices, and times
Our return policy and precess

| want to edit or cancel my order

Visit help center

SHOP FASTER WITH THE APP

£, Qowatoad on thy
& App Store

GET HELP

GET HELP

sae yy Tor

COMPANY

COMPANY

$5.25

$83.17 USD

Payment method: Visa

hoc!

 

Y¥

shapify_pay Ws

QUICK LINKS

QUICK LINKS

 

 

 

 

 

 

Going Viral Handbag - Silver $10.00
Size: OS ay: 1
Ready To Slay Jumpsuit - Charcoal $11.98
Size: XL Qty:1
Lorena Slinky Max: Dress - Hunter $7.00
Size: L Oty:
Worth It Ribbed Top - Ivory $7.00
Size: L aty 1
ee
hi
yy
Always Adored Ribbed Jumpsuit - Rust $11.98
Size: 1X Oty: 1
Out Of Your League Sunglasses - Silver $2.98
Size: OS aye
Sub $9791
Discount $19.99

Shipping FREE

   

FREE
FASHIONNOWVAwSHes PlustcurVe MEN KIDS BEAUTY  Q ao 828 QAD @Oqgs

<— All Orders

Order #138845514
Date Placed: Jul 15/2024
Total $83.17

Delivery Address
Artisha charles

10813 NW 20th Street
BLDG 11S GRE 9223
Miami FL 33172
United States

Shipping Methods
3-7 Business Days

Billing Address
Artisha charles

10813 NW 30th Street
BLDG 115 GRE 9223
Miami FL 33172
United States

 

Feeling Brand New Thong 2 Pack Panties - Grey/combo $6.99
Size: XL Oty
Keep On Slashing Romper - Black $0.00
Size: XL $1999

Oty: 1
Nova Season Long Sleeve One Shoulder Jumpsuit - Black $19.99

Size: 1X at.

 

 

Grenada
Simplified Declaration Form

 

To be Used Only For a Maximum of Five (5) Personal Effects and Non Commercial items

WARNING: You can be prosecuted for a false declaration and your goods may be liable to forfeiture

 

 

 

Consignee: ARTISHA CHARLES (FREIGHT 13.00 US)
Cc Customs Office GDWBS
NS
Man Reg Number: 2024/28
WayBill Number: HAWB9592028
No and Type of package: 1 Package
Gross Mass: 6.0 Freight 0.0
Insuranc 9.0
Particulars of declaration by Importer
Description of Goods Customs Value $EC Tariff No. Weight (kg) Supplementary

 

 

 

 

 

 

|, the undersigned, ARTISHA CHARLES (FREIGHT 13.00 US) do hereby declare that the above particulars are true and correct.

 

 

 

 

Dated this day of 20
. Examination Required: YES / NO
Signed:
tmporter/Exporter or declarant For Comptroller of Customs
For Official Use Only
Description of Goods Customs Value $EC Tariff No. Weight (Kg) Supplementary

 

 

 

 

 

 

 

Examination Officer

 

For Comptroller of Customs

 


------------------------------------------SparseText-------------------------
~s

‘Taxes

$5.25

Total

$83.17 USD

Payment method: Visa

Y¥

hoc!

shopify_pay

Ws

NEED HELP?

Shipping options, prices, and times

Our return policy and precess

| want to edit or cancel my order

Visit help center

SHOP FASTER WITH THE APP

GET HELP

COMPANY

QUICK LINKS

GET HELP

COMPANY

QUICK LINKS

fy

Ve

$10.00

Going Viral Handbag - Silver

Size: OS

aty:1

ee

t

Ready To Slay Jumpsuit - Charcoal

$11.98

>t

1

Size: XL

Lorena Slinky Max: Dress - Hunter

$7.00

Size: L

Oty:1

Worth It Risbed Top - Ivory

$7.00

Size: L

att

2

$11.98

Always Adored Ribbed Jumpsuit - Rust

Size: 1X

Qty: 1

bt}

Out Of Your League Sunglasses - Silver

$2.98

Size: OS

aty

a. a

j, _ Pe

Wis A

$9791

$19.99

Discount

Shipping

FREE

IpPPI

ON

RS ©

ER S75

4 FREE

FASHIONNOWVAWSHe4 PLus+cuRVE MEN KIDS BEAUTY

eavpe®erv°o 8

<— All Orders

Order #138845514

Date Placed: Jul 15/2024

Total $83.17

Delivery Address

Artisha charles

10813 NW 20th Street

BLDG 11S GRE 9223

Miami FL 33172

United States

Shipping Methods

3-7 Business Days

Billing Address

Artisha charles

10813 NW 30th Street

BLDG 115 GRE 9223

Miami FL 33172

United States

Feeling Brand New Thong 2 Pack Panties - Grey/combo

$6.99

Size: XL

Oty1

$0.00

Keep On Slashing Romper - Black

Size: XL

Oty: 1

Nova Season Long Sleeve One Shoulder Jumpsuit - Black

$19.99

Size: 1X

Qty. 1

ae

hy

[a

Bod

gs

7

I

yy

Grenada

Simplified Declaration Form

To be Used Only For a Maximum of Five (5) Personal Effects and Non Commercial items

WARNING: You can be prosecuted for a false declaration and your goods may be liable to forfeiture

Consignee:

ARTISHA CHARLES (FREIGHT 13.00 US)

Customs Office

GDWBS

Man Reg Number:

2024/28

WayBill Number:

HAWB9592028

1

Package

No and Type of package:

Gross Mass:

6.0

Freight

0.0

Insuranc

0.0

Particulars of declaration by Importer

Description of Goods

Customs Value $EC

Tariff No.

Weight (kg)

Supplementary

|, the undersigned, ARTISHA CHARLES (FREIGHT 13.00 US) do hereby declare that the above particulars are true and correct.

Dated this

day of

20

Examination Required: YES / NO

Signed:

tmporter/Exporter or declarant

For Comptroller of Customs

For Official Use Only

Description of Goods

Customs Value $EC

Tariff No.

Weight (Kg)

Supplementary

Examination Officer

For Comptroller of Customs


------------------------------------------Ripped Text-------------------------

";
    }
}