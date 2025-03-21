using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using WaterNut.Business.Services.Utils;
using System.Net.Http;
using NUnit.Framework.Legacy;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

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
                ILogger<DeepSeekInvoiceApi> logger = LoggingConfig.CreateLogger();
                var api = new DeepSeekInvoiceApi(logger);
                var textVariants = new List<string> { SampleText };

                // Act
                var results = await api.ExtractShipmentInvoice(textVariants).ConfigureAwait(false);

                // Assert - Invoices
                var invoice = results.FirstOrDefault(d => d["DocumentType"] as string == "Invoice");
                Assert.That(invoice, Is.Not.Null, "No invoice found");
                Assert.That(invoice["InvoiceNo"], Is.EqualTo("138845514"));
                Assert.That(invoice["InvoiceDate"], Is.EqualTo(DateTime.Parse("2024-07-15")));
                Assert.That(invoice["Total"], Is.EqualTo(83.17m).Within(0.01m));
                Assert.That(invoice["Currency"], Is.EqualTo("USD"));

                // Assert Line Items
                var lineItems = invoice["LineItems"] as List<IDictionary<string, object>>;
                Assert.That(lineItems.Count, Is.EqualTo(9));
                Assert.That(lineItems[0]["Description"], Is.EqualTo("Going Viral Handbag - Silver"));
                Assert.That(lineItems[0]["Quantity"], Is.EqualTo(1m));

                // Assert - Customs Declaration
                var customs = results.FirstOrDefault(d => d["DocumentType"] as string == "CustomsDeclaration");
                Assert.That(customs, Is.Not.Null, "No customs declaration found");
                Assert.That(customs["Consignee"], Is.EqualTo("ARTISHA CHARLES (FREIGHT 13.00 US)"));
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
                var api = new DeepSeekInvoiceApi(Mock.Of<ILogger<DeepSeekInvoiceApi>>());

                Assert.Multiple(() =>
                {
                    Assert.That(api.ValidateTariffCode("8481.80.0000"), Is.EqualTo("8481.80"));
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
        public void ClassifyItems_ProcessesMixedItems_CorrectlyPopulatesMissingFields()
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
            var results = api.ClassifyItems(testItems);

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
        public void ClassifyItems_SanitizesAllInputFields()
        {
            var api = new DeepSeekApi();
            var testItems = new List<(string, string, string)>
            {
                ("Invalid|Item#", "Dirty\nDescription", "Invalid/HS-Code"),
                ("", "Wireless Mouse", "8517.60.00")
            };

            var results = api.ClassifyItems(testItems);

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
        [Category("Integration")]
        public async Task GetTariffCode_RealApi_WomensDress()
        {
            using var realApi = new DeepSeekApi();
            var result =   await realApi.GetTariffCode("Women's cotton dress with polyester lining").ConfigureAwait(false);

            Assert.That(result, Does.Match(_deepSeekApi.HsCodePattern));
            Console.WriteLine($"Women's Dress HS Code: {result}");
        }

        [Test]
        [Category("Integration")]
        public async Task GetTariffCode_RealApi_WoodenChairs()
        {
            using var realApi = new DeepSeekApi();
            var result = await realApi.GetTariffCode("Oak wooden dining chairs with upholstered seats").ConfigureAwait(false);

            Assert.That(result, Does.Match(_deepSeekApi.HsCodePattern));
            Console.WriteLine($"Wooden Chairs HS Code: {result}");
        }

        [Test]
        [Category("Integration")]
        public async Task GetTariffCode_RealApi_ComplexProduct()
        {
            using var realApi = new DeepSeekApi();
            var description = "MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial";

            var result = await realApi.GetTariffCode(description).ConfigureAwait(false);

            Assert.That(result, Does.Match(_deepSeekApi.HsCodePattern));
            Console.WriteLine($"LED Display HS Code: {result}");
        }

        [Test]
        [Category("Integration")]
        public async Task GetTariffCode_RealApi_InvalidProduct()
        {
            using var realApi = new DeepSeekApi();

            Assert.ThrowsAsync<HSCodeRequestException>(() =>
                realApi.GetTariffCode("Non-existent imaginary product"));
        }

        // Add this to existing mock setup section
        private DeepSeekApi _deepSeekApi;
        private Mock<HttpMessageHandler> _mockHttpHandler;
        private HttpClient _mockHttpClient;

        public DeepSeekApiTests()
        {
            _mockHttpHandler = new Mock<HttpMessageHandler>();
            _mockHttpClient = new HttpClient(_mockHttpHandler.Object);
            _deepSeekApi = new DeepSeekApi()
            {
                //_httpClient = _mockHttpClient
            };
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