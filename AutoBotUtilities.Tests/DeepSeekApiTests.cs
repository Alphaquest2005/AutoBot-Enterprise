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
using Serilog; // Added
using System.Reflection; // Added for reflection
using Core.Common.Extensions; // For LogCategory and TypedLoggerExtensions
using Serilog.Events;
using Serilog.Core;
using Serilog.Sinks.NUnit;

namespace AutoBotUtilities.Tests
{
    // Mock HTTP message handler for testing
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _responseContent;

        public MockHttpMessageHandler(string responseContent)
        {
            _responseContent = responseContent;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_responseContent, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }
    }

    [TestFixture]
    public class DeepSeekApiTests : IDisposable
    {
        private static ILogger _log;
        private string invocationId;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            invocationId = Guid.NewGuid().ToString();

            // Configure LogFilterState for test logging
            LogFilterState.EnabledCategoryLevels = new Dictionary<LogCategory, LogEventLevel>
            {
                { LogCategory.MethodBoundary, LogEventLevel.Information },
                { LogCategory.InternalStep, LogEventLevel.Information },
                { LogCategory.DiagnosticDetail, LogEventLevel.Information },
                { LogCategory.Performance, LogEventLevel.Warning },
                { LogCategory.Undefined, LogEventLevel.Information }
            };

            _log = new LoggerConfiguration()
                .MinimumLevel.Verbose() // Allow all logs to pass to the filter
                .Enrich.FromLogContext().Enrich.WithProperty("TestFixture", nameof(DeepSeekApiTests))
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{LogCategory}] [{SourceContext}] [{MemberName}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.NUnitOutput(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{LogCategory}] [{SourceContext}] [{MemberName}] {Message:lj}{NewLine}")
                .WriteTo.File("DeepSeekApiTests.log", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{LogCategory}] [{SourceContext}] [{MemberName}] {Message:lj}{NewLine}{Properties:j}{NewLine}{Exception}")
                .Filter.ByIncludingOnly(evt =>
                {
                    var category = evt.Properties.TryGetValue("LogCategory", out var categoryValue) && categoryValue is ScalarValue sv && sv.Value is LogCategory lc ? lc : LogCategory.Undefined;
                    if (!string.IsNullOrEmpty(LogFilterState.TargetSourceContextForDetails))
                    {
                        var sourceContext = evt.Properties.TryGetValue("SourceContext", out var sourceContextValue) && sourceContextValue is ScalarValue scv ? scv.Value?.ToString() : "";
                        var memberName = evt.Properties.TryGetValue("MemberName", out var memberNameValue) && memberNameValue is ScalarValue mnv ? mnv.Value?.ToString() : "";
                        var contextMatch = sourceContext?.Contains(LogFilterState.TargetSourceContextForDetails) == true;
                        var methodMatch = string.IsNullOrEmpty(LogFilterState.TargetMethodNameForDetails) || memberName?.Contains(LogFilterState.TargetMethodNameForDetails) == true;
                        if (contextMatch && methodMatch) { return evt.Level >= LogFilterState.DetailTargetMinimumLevel; }
                    }
                    if (LogFilterState.EnabledCategoryLevels.TryGetValue(category, out var enabledMinLevelForCategory)) { return evt.Level >= enabledMinLevelForCategory; }
                    return false;
                })
                .CreateLogger();
            Log.Logger = _log;

            _log.LogInfoCategorized(LogCategory.Undefined, "=== DeepSeekApiTests OneTimeSetup Starting ===", invocationId: invocationId);
        }

        public void Dispose()
        {
            // Cleanup if needed
            _log?.LogInfoCategorized(LogCategory.Undefined, "=== DeepSeekApiTests Disposed ===", invocationId: invocationId);
        }

        [TestFixture]
        public class ShipmentInvoiceTests
        {


            [Test]
            public async Task ExtractShipmentInvoice_ProcessesSampleText_ReturnsValidDocuments()
            {
                var testInvocationId = Guid.NewGuid().ToString();

                // Arrange - Use proper logging directives
                using (LogLevelOverride.Begin(LogEventLevel.Information))
                {
                    _log.LogMethodEntry(testInvocationId);
                    _log.LogInfoCategorized(LogCategory.DiagnosticDetail, "=== Starting ExtractShipmentInvoice Test ===", invocationId: testInvocationId);

                    // Create mock HTTP response with expected JSON structure
                    var mockJsonResponse = @"{
                        ""Invoices"": [
                            {
                                ""InvoiceNo"": ""138845514"",
                                ""InvoiceDate"": ""2024-07-15"",
                                ""Total"": 83.17,
                                ""Currency"": ""USD"",
                                ""SubTotal"": 77.92,
                                ""SupplierCode"": ""TEMU"",
                                ""SupplierName"": ""TEMU"",
                                ""SupplierAddress"": ""Online Store"",
                                ""SupplierCountryCode"": ""US"",
                                ""InvoiceDetails"": [
                                    {
                                        ""ItemDescription"": ""Going Viral Handbag - Silver"",
                                        ""Quantity"": 1,
                                        ""Cost"": 10.00,
                                        ""TotalCost"": 10.00,
                                        ""ItemNumber"": ""ITEM001""
                                    },
                                    {
                                        ""ItemDescription"": ""Ready To Slay Jumpsuit - Charcoal"",
                                        ""Quantity"": 1,
                                        ""Cost"": 11.98,
                                        ""TotalCost"": 11.98,
                                        ""ItemNumber"": ""ITEM002""
                                    },
                                    {
                                        ""ItemDescription"": ""Lorena Slinky Max: Dress - Hunter"",
                                        ""Quantity"": 1,
                                        ""Cost"": 7.00,
                                        ""TotalCost"": 7.00,
                                        ""ItemNumber"": ""ITEM003""
                                    },
                                    {
                                        ""ItemDescription"": ""Worth It Ribbed Top - Ivory"",
                                        ""Quantity"": 1,
                                        ""Cost"": 7.00,
                                        ""TotalCost"": 7.00,
                                        ""ItemNumber"": ""ITEM004""
                                    },
                                    {
                                        ""ItemDescription"": ""Always Adored Ribbed Jumpsuit - Rust"",
                                        ""Quantity"": 1,
                                        ""Cost"": 11.98,
                                        ""TotalCost"": 11.98,
                                        ""ItemNumber"": ""ITEM005""
                                    },
                                    {
                                        ""ItemDescription"": ""Out Of Your League Sunglasses - Silver"",
                                        ""Quantity"": 1,
                                        ""Cost"": 2.98,
                                        ""TotalCost"": 2.98,
                                        ""ItemNumber"": ""ITEM006""
                                    },
                                    {
                                        ""ItemDescription"": ""Feeling Brand New Thong 2 Pack Panties - Grey/combo"",
                                        ""Quantity"": 1,
                                        ""Cost"": 6.99,
                                        ""TotalCost"": 6.99,
                                        ""ItemNumber"": ""ITEM007""
                                    },
                                    {
                                        ""ItemDescription"": ""Keep On Slashing Romper - Black"",
                                        ""Quantity"": 1,
                                        ""Cost"": 0.00,
                                        ""TotalCost"": 0.00,
                                        ""ItemNumber"": ""ITEM008""
                                    },
                                    {
                                        ""ItemDescription"": ""Nova Season Long Sleeve One Shoulder Jumpsuit - Black"",
                                        ""Quantity"": 1,
                                        ""Cost"": 19.99,
                                        ""TotalCost"": 19.99,
                                        ""ItemNumber"": ""ITEM009""
                                    }
                                ]
                            }
                        ],
                        ""CustomsDeclarations"": [
                            {
                                ""CustomsOffice"": ""GDWBS"",
                                ""ManifestYear"": 2024,
                                ""ManifestNumber"": 28,
                                ""Consignee"": ""ARTISHA CHARLES"",
                                ""BLNumber"": ""HAWB9592028"",
                                ""PackageType"": ""Package"",
                                ""Packages"": 1,
                                ""GrossWeightKG"": 6.0,
                                ""FreightCurrency"": ""USD"",
                                ""Freight"": 13.00,
                                ""Goods"": [
                                    {
                                        ""Description"": ""Personal Effects"",
                                        ""TariffCode"": """"
                                    }
                                ]
                            }
                        ]
                    }";

                    _log.LogInfoCategorized(LogCategory.DiagnosticDetail, "Mock JSON Response prepared with {InvoiceCount} invoices and {CustomsCount} customs declarations", testInvocationId, propertyValues: new object[] { 1, 1 });

                    // Create mock HTTP handler
                    var mockHttpHandler = new MockHttpMessageHandler(mockJsonResponse);
                    var mockHttpClient = new HttpClient(mockHttpHandler);

                    // Create API instance with proper logger
                    var api = new DeepSeekInvoiceApi(_log);

                    // Use reflection to inject the mock HttpClient
                    var httpClientField = typeof(DeepSeekInvoiceApi).GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (httpClientField == null)
                    {
                        _log.LogErrorCategorized(LogCategory.DiagnosticDetail, "Could not find private field '_httpClient' via reflection", testInvocationId);
                        Assert.Fail("Could not find private field '_httpClient' via reflection.");
                    }

                    // Dispose original HttpClient and inject mock
                    var originalHttpClient = httpClientField.GetValue(api) as HttpClient;
                    originalHttpClient?.Dispose();
                    httpClientField.SetValue(api, mockHttpClient);

                    _log.LogInfoCategorized(LogCategory.DiagnosticDetail, "Mock HttpClient injected successfully", testInvocationId);

                    var textVariants = new List<string> { SampleText };
                    _log.LogInfoCategorized(LogCategory.DiagnosticDetail, "Processing {VariantCount} text variants", testInvocationId, propertyValues: new object[] { textVariants.Count });

                    // Act
                    var results = await api.ExtractShipmentInvoice(textVariants).ConfigureAwait(false);
                    _log.LogInfoCategorized(LogCategory.DiagnosticDetail, "ExtractShipmentInvoice returned {ResultCount} results", testInvocationId, propertyValues: new object[] { results?.Count ?? 0 });

                    // Log all results for debugging
                    if (results != null)
                    {
                        for (int i = 0; i < results.Count; i++)
                        {
                            var result = results[i];
                            if (result is IDictionary<string, object> dict)
                            {
                                _log.LogInfoCategorized(LogCategory.DiagnosticDetail, "Result {Index}: DocumentType = {DocumentType}", testInvocationId, propertyValues: new object[] { i, dict.TryGetValue("DocumentType", out var docType) ? docType : "NULL" });
                                foreach (var kvp in dict)
                                {
                                    _log.LogDebugCategorized(LogCategory.DiagnosticDetail, "  {Key} = {Value}", testInvocationId, propertyValues: new object[] { kvp.Key, kvp.Value?.ToString() ?? "NULL" });
                                }
                            }
                        }
                    }

                    // Assert - Invoices
                    var invoice = results?.FirstOrDefault(d => d is IDictionary<string, object> dict && dict.TryGetValue("DocumentType", out var docType) && docType as string == FileTypeManager.EntryTypes.ShipmentInvoice) as IDictionary<string, object>;
                    _log.LogInfoCategorized(LogCategory.DiagnosticDetail, "Invoice found: {Found}, Expected DocumentType: {ExpectedType}", testInvocationId, propertyValues: new object[] { invoice != null, FileTypeManager.EntryTypes.ShipmentInvoice });

                    Assert.That(invoice, Is.Not.Null, "No invoice found");
                    Assert.That(invoice["InvoiceNo"], Is.EqualTo("138845514"));
                    Assert.That(invoice["InvoiceDate"], Is.EqualTo(DateTime.Parse("2024-07-15")));
                    Assert.That(invoice["Total"], Is.EqualTo(83.17m).Within(0.01m));
                    Assert.That(invoice["Currency"], Is.EqualTo("USD"));

                    // Assert Line Items
                    var lineItems = invoice["InvoiceDetails"] as List<IDictionary<string, object>>;
                    _log.LogInfoCategorized(LogCategory.DiagnosticDetail, "Line items found: {Count}", testInvocationId, propertyValues: new object[] { lineItems?.Count ?? 0 });
                    Assert.That(lineItems.Count, Is.EqualTo(9));
                    Assert.That(lineItems[0]["ItemDescription"], Is.EqualTo("Going Viral Handbag - Silver"));
                    Assert.That(lineItems[0]["Quantity"], Is.EqualTo(1m));

                    // Assert - Customs Declaration
                    var customs = results?.FirstOrDefault(d => d is IDictionary<string, object> dict && dict.TryGetValue("DocumentType", out var docType) && docType as string == FileTypeManager.EntryTypes.SimplifiedDeclaration) as IDictionary<string, object>;
                    _log.LogInfoCategorized(LogCategory.DiagnosticDetail, "Customs declaration found: {Found}, Expected DocumentType: {ExpectedType}", testInvocationId, propertyValues: new object[] { customs != null, FileTypeManager.EntryTypes.SimplifiedDeclaration });

                    Assert.That(customs, Is.Not.Null, "No customs declaration found");
                    Assert.That(customs["Consignee"], Is.EqualTo("ARTISHA CHARLES"));
                    Assert.That(customs["BLNumber"], Is.EqualTo("HAWB9592028"));

                    // Note: PackageInfo is not being parsed in the current implementation, so we'll check the direct fields
                    Assert.That(customs["Packages"], Is.EqualTo(1));
                    Assert.That(customs["GrossWeightKG"], Is.EqualTo(6.0m));

                    _log.LogInfoCategorized(LogCategory.DiagnosticDetail, "=== Test completed successfully ===", testInvocationId);
                    _log.LogMethodExitSuccess(testInvocationId, 0); // 0 duration since we're not tracking time in this test

                    // Cleanup
                    mockHttpClient.Dispose();
                }
            }

            [Test]
            public void ValidateTariffCode_CleansHsCodesCorrectly()
            {
                var mockLogger = new Mock<Serilog.ILogger>();
                var api = new DeepSeekInvoiceApi(mockLogger.Object);

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
            var mockLogger = new Mock<Serilog.ILogger>(); // Changed to Mock<Serilog.ILogger>
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
            var api = new DeepSeekApi(mockLogger.Object, dummyApiKey); // Pass the mock Serilog logger

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
            var mockLogger = new Mock<Serilog.ILogger>(); // Changed to Mock<Serilog.ILogger>
            // Use the parameterized constructor to inject the mock logger
            _deepSeekApi = new DeepSeekApi(mockLogger.Object, "dummy-api-key"); // Pass the mock Serilog logger
            // The mock HttpClient is injected later in the ClassifyItemsAsync_WithCategory_ReturnsCorrectData test
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

shopify_pay Ws

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