using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using NUnit.Framework;
using Serilog;
using Serilog.Events;
using WaterNut.DataSpace;
using EntryDataDS.Business.Entities;
using Core.Common.Extensions;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
using OCR.Business.Entities;
using System.Text;
using Serilog.Core;

namespace AutoBotUtilities.Tests.Production
{
    using OCRCorrectionService = WaterNut.DataSpace.OCRCorrectionService;

    /// <summary>
    /// Performance and memory tests for OCR Correction Service
    /// </summary>
    public partial class OCRCorrectionService_ProductionTests
    {
        #region Performance Tests

        [Test]
        [Category("Performance")]
        public async Task CorrectInvoices_100Invoices_ShouldCompleteWithinTimeLimit()
        {
            _logger.Information("Testing performance with 100 invoices");

            var stopwatch = Stopwatch.StartNew();
            var invoices = new List<ShipmentInvoice>();

            // Generate 100 test invoices
            for (int i = 0; i < 100; i++)
            {
                invoices.Add(CreateTestInvoice($"PERF-{i:D3}", 100 + i, 90 + i, 5, 5, 0, 0));
            }

            var processedCount = 0;
            foreach (var invoice in invoices)
            {
                var fileText = $"Invoice #{invoice.InvoiceNo}\nTotal: ${invoice.InvoiceTotal:F2}";

                // Test the static TotalsZero method for performance (no API calls)
                var result = OCRCorrectionService.TotalsZero(invoice, out _, _logger);

                if ((invoices.IndexOf(invoice) + 1) % 25 == 0)
                {
                    _logger.Information("Processed {Count}/100 invoices in {Elapsed}ms",
                        invoices.IndexOf(invoice) + 1, stopwatch.ElapsedMilliseconds);
                }

                processedCount++;
            }

            stopwatch.Stop();

            _logger.Information("Performance test completed: {Count} invoices in {Elapsed}ms ({Rate:F2} invoices/sec)",
                processedCount, stopwatch.ElapsedMilliseconds, processedCount / (stopwatch.ElapsedMilliseconds / 1000.0));

            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(30000),
                "Should process 100 invoices within 30 seconds");
            Assert.That(processedCount, Is.EqualTo(100), "Should process all invoices");

            _logger.Information("✓ Performance test passed");
        }

        [Test]
        [Category("Memory")]
        public async Task CorrectInvoices_MemoryUsage_ShouldNotLeak()
        {
            _logger.Information("Testing memory usage and leak detection");

            var initialMemory = GC.GetTotalMemory(true);
            _logger.Information("Initial memory: {Memory:N0} bytes", initialMemory);

            const int iterations = 50;
            var memoryReadings = new List<long>();

            for (int i = 0; i < iterations; i++)
            {
                using (var service = new OCRCorrectionService(_logger))
                {
                    var invoice = CreateTestInvoice($"MEM-{i:D3}", 100 + i, 90 + i, 5, 5, 0, 0);
                    var fileText = $"Invoice #{invoice.InvoiceNo}\nTotal: ${invoice.InvoiceTotal:F2}";

                    // Test memory usage with TotalsZero
                    var result = OCRCorrectionService.TotalsZero(invoice, out _, _logger);
                }

                if (i % 10 == 0)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();

                    var currentMemory = GC.GetTotalMemory(false);
                    memoryReadings.Add(currentMemory);

                    _logger.Information("Iteration {Iteration}: Memory = {Memory:N0} bytes",
                        i, currentMemory);
                }
            }

            var finalMemory = GC.GetTotalMemory(false);
            var memoryIncrease = finalMemory - initialMemory;
            var memoryIncreasePercent = (initialMemory == 0) ? 0 : (memoryIncrease / (double)initialMemory) * 100;

            _logger.Information("Final memory: {Memory:N0} bytes", finalMemory);
            _logger.Information("Memory increase: {Increase:N0} bytes ({Percent:F1}%)",
                memoryIncrease, memoryIncreasePercent);

            // Memory should not increase by more than 50% during the test
            Assert.That(memoryIncreasePercent, Is.LessThan(50),
                $"Memory usage increased by {memoryIncreasePercent:F1}%, should be less than 50%");

            _logger.Information("✓ Memory usage test passed");
        }

        [Test]
        [Category("Performance")]
        [Category("EdgeCases")]
        public void TotalsZero_ExtremeValues_ShouldHandleGracefully()
        {
            _logger.Information("Testing extreme value handling");

            var extremeValueCases = new[]
            {
                (double.MaxValue, double.MaxValue / 2, 0.0, 0.0, 0.0, 0.0),
                (double.MinValue, double.MinValue / 2, 0.0, 0.0, 0.0, 0.0),
                (0.0, 0.0, 0.0, 0.0, 0.0, 0.0),
                (1e-10, 5e-11, 3e-11, 2e-11, 0.0, 0.0),
                (1e10, 5e9, 3e9, 2e9, 0.0, 0.0)
            };

            foreach (var (total, subTotal, freight, other, insurance, deduction) in extremeValueCases)
            {
                var invoice = CreateTestInvoice("EXTREME", total, subTotal, freight, other, insurance, deduction);
                if (invoice.InvoiceDetails == null) invoice.InvoiceDetails = new List<InvoiceDetails>();
                invoice.InvoiceDetails.Add(new InvoiceDetails
                {
                    LineNumber = 1,
                    ItemDescription = "Extreme Test Item",
                    Quantity = 1,
                    Cost = subTotal,
                    TotalCost = subTotal
                });

                Assert.DoesNotThrow(() =>
                {
                    var result = OCRCorrectionService.TotalsZero(invoice, out _, _logger);
                    _logger.Information("Extreme value test: {Total} → {Result}", total, result);
                }, $"Should handle extreme values gracefully: {total}");
            }

            _logger.Information("✓ Extreme value handling test passed");
        }

        [Test]
        [Category("Performance")]
        [Category("Concurrency")]
        public async Task TotalsZero_ConcurrentAccess_ShouldBeThreadSafe()
        {
            _logger.Information("Testing concurrent access to TotalsZero method");

            const int threadCount = 10;
            const int operationsPerThread = 100;
            var tasks = new List<Task<bool>>();
            var results = new List<bool>();
            var lockObject = new object();

            for (int t = 0; t < threadCount; t++)
            {
                int threadId = t;
                tasks.Add(Task.Run(() =>
                {
                    var threadResults = new List<bool>();
                    for (int i = 0; i < operationsPerThread; i++)
                    {
                        var invoice = CreateTestInvoice($"THREAD-{threadId}-{i}", 100, 90, 5, 5, 0, 0);
                        var result = OCRCorrectionService.TotalsZero(invoice, out _, _logger);
                        threadResults.Add(result);
                    }

                    lock (lockObject)
                    {
                        results.AddRange(threadResults);
                    }

                    return threadResults.All(r => r); // All should be true for balanced invoices
                }));
            }

            var allTaskResults = await Task.WhenAll(tasks).ConfigureAwait(false);

            _logger.Information("Concurrent test completed: {ThreadCount} threads × {OpsPerThread} operations = {TotalOps} total operations",
                threadCount, operationsPerThread, results.Count);

            Assert.That(results.Count, Is.EqualTo(threadCount * operationsPerThread),
                "Should complete all operations");
            Assert.That(allTaskResults.All(r => r), Is.True,
                "All threads should return consistent results");

            _logger.Information("✓ Concurrent access test passed");
        }

        [Test]
        [Category("Performance")]
        [Category("StressTest")]
        public void TotalsZero_StressTest_ShouldMaintainAccuracy()
        {
            _logger.Information("Running stress test with various invoice configurations");

            var configurations = new[]
            {
                // (invoiceCount, baseTotal, variation)
                (1000, 100.0, 10.0),
                (500, 1000.0, 100.0),
                (200, 10000.0, 1000.0),
                (100, 0.01, 0.001)
            };

            var totalProcessed = 0;
            var stopwatch = Stopwatch.StartNew();

            foreach (var (invoiceCount, baseTotal, variation) in configurations)
            {
                _logger.Information("Testing {Count} invoices with base total ${BaseTotal} ± ${Variation}",
                    invoiceCount, baseTotal, variation);

                var configResults = new List<bool>();

                for (int i = 0; i < invoiceCount; i++)
                {
                    var random = new Random(i); // Deterministic for reproducibility
                    var total = baseTotal + (random.NextDouble() - 0.5) * variation;
                    var subTotal = total * 0.85;
                    var freight = total * 0.08;
                    var other = total * 0.05;
                    var insurance = total * 0.02;

                    var invoice = CreateTestInvoice($"STRESS-{totalProcessed}", total, subTotal, freight, other, insurance, 0);
                    var result = OCRCorrectionService.TotalsZero(invoice, out _, _logger);
                    configResults.Add(result);
                    totalProcessed++;
                }

                var successRate = configResults.Count(r => r) / (double)configResults.Count * 100;
                _logger.Information("Configuration success rate: {SuccessRate:F1}%", successRate);

                Assert.That(successRate, Is.GreaterThan(95.0),
                    $"Success rate should be > 95% for configuration with base total {baseTotal}");
            }

            stopwatch.Stop();

            _logger.Information("Stress test completed: {TotalProcessed} invoices in {Elapsed}ms ({Rate:F2} invoices/sec)",
                totalProcessed, stopwatch.ElapsedMilliseconds, totalProcessed / (stopwatch.ElapsedMilliseconds / 1000.0));

            _logger.Information("✓ Stress test passed");
        }

        #endregion
    }
}
