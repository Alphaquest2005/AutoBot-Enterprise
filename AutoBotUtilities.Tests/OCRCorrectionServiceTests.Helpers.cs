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
    /// <summary>
    /// Helper methods and utilities for OCR Correction Service tests
    /// </summary>
    public partial class OCRCorrectionService_ProductionTests
    {
        #region Helper Methods

        private ShipmentInvoice CreateTestInvoice(string invoiceNo, double total, double subTotal, 
            double freight, double other, double insurance, double deduction)
        {
            return new ShipmentInvoice
            {
                InvoiceNo = invoiceNo,
                InvoiceTotal = total,
                SubTotal = subTotal,
                TotalInternalFreight = freight,
                TotalOtherCost = other,
                TotalInsurance = insurance,
                TotalDeduction = deduction,
                InvoiceDate = DateTime.Now,
                Currency = "USD",
                SupplierName = "Test Supplier",
                SupplierAddress = "123 Test St",
                InvoiceDetails = new List<InvoiceDetails>()
            };
        }

        private ShipmentInvoice CreateTestInvoiceWithErrors()
        {
            return new ShipmentInvoice
            {
                InvoiceNo = "ERR-001",
                InvoiceTotal = 100.0,
                SubTotal = 80.00,
                SupplierName = "Suplier Test Inc.",
                InvoiceDate = new DateTime(2023, 1, 15),
                InvoiceDetails = new List<InvoiceDetails>()
            };
        }

        private string CreateTestFileTextWithKnownErrors()
        {
            return @"
                INVOICE #INV-CORRECT-001
                Date: 2023-01-15
                Supplier: Suplier Test Inc.
                
                Subtotal: $80,00
                Shipping: $10,00
                Tax: $10,00
                Total: $100,00
            ";
        }

        private ShipmentInvoice CreateTestInvoiceWithProductErrors()
        {
            var invoice = CreateTestInvoice("PROD-ERR-01", 150.75, 120.50, 10.25, 20.00, 0, 0);
            invoice.InvoiceDetails = new List<InvoiceDetails>
            {
                new InvoiceDetails { LineNumber = 1, ItemDescription = "Widget A old", Quantity = 2, Cost = 25.25, TotalCost = 50.50 },
                new InvoiceDetails { LineNumber = 2, ItemDescription = "Gadget B Typo", Quantity = 1, Cost = 70.00, TotalCost = 70.00 }
            };
            return invoice;
        }

        private string CreateTestFileTextWithProductErrors()
        {
            return @"
                Invoice: PROD-ERR-01
                Item 1: Widget A old - Qty: 2 - Price: $25.25 - Total: $50.50
                Item 2: Gadget B Typo - Qty: 1 - Price: $70.00 - Total: $70.00
                Subtotal: $120.50
                Shipping: $10.25
                Other: $20.00
                Total: $150.75
            ";
        }

        private ShipmentInvoice CreateComplexTestInvoice()
        {
            var invoice = CreateTestInvoice("COMPLEX-001", 10000, 8000, 500, 500, 200, 800);
            invoice.InvoiceDetails = new List<InvoiceDetails>();
            for (int i = 0; i < 50; i++)
            {
                invoice.InvoiceDetails.Add(new InvoiceDetails
                {
                    LineNumber = i + 1,
                    ItemDescription = $"Complex Item {i + 1}",
                    Quantity = i + 1,
                    Cost = Math.Round(100.0 / (i + 1), 2),
                    TotalCost = 100.0,
                    Discount = (i % 5)
                });
            }
            return invoice;
        }

        private string CreateVeryLargeTestFileText()
        {
            var sb = new StringBuilder();
            sb.AppendLine("VERY LARGE INVOICE DOCUMENT START");
            sb.AppendLine("Invoice #LARGE-001");
            sb.AppendLine("Date: 2023-01-15");
            sb.AppendLine();

            for (int i = 0; i < 1000; i++)
            {
                sb.AppendLine($"Line Item {i + 1}: Product {i + 1} - Qty: {i % 10 + 1} - Price: ${(i % 100) + 1}.{i % 100:D2}");
            }

            sb.AppendLine();
            sb.AppendLine("VERY LARGE INVOICE DOCUMENT END");
            return sb.ToString();
        }

        private List<dynamic> GenerateLargeInvoiceList(int count)
        {
            var list = new List<dynamic>();
            for (int i = 0; i < count; i++)
            {
                var invoice = new Dictionary<string, object>
                {
                    ["InvoiceNo"] = $"LARGE-{i:D4}",
                    ["InvoiceTotal"] = 100.0 + i,
                    ["SubTotal"] = 90.0 + i,
                    ["TotalInternalFreight"] = 5.0,
                    ["TotalOtherCost"] = 5.0,
                    ["InvoiceDetails"] = new List<Dictionary<string, object>>()
                };
                list.Add(invoice);
            }
            return list;
        }

        private string GenerateLargeInvoiceText(int numLineItems)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Invoice Test Document");
            sb.AppendLine($"Total Line Items: {numLineItems}");
            sb.AppendLine();

            for (int i = 0; i < numLineItems; i++)
            {
                sb.AppendLine($"Item {i + 1}: Test Product {i + 1} - ${i + 1}.00");
            }

            sb.AppendLine();
            sb.AppendLine("End of Invoice");
            return sb.ToString();
        }

        private ShipmentInvoice CreateTestInvoiceWithInconsistentDetails()
        {
            var invoice = CreateTestInvoice("INCONSISTENT-001", 100, 50, 10, 5, 0, 0);
            invoice.InvoiceDetails = new List<InvoiceDetails>
            {
                new InvoiceDetails { LineNumber = 1, ItemDescription = "Item A", Quantity = 1, Cost = 30, TotalCost = 30 },
                new InvoiceDetails { LineNumber = 2, ItemDescription = "Item B", Quantity = 1, Cost = 30, TotalCost = 30 }
            };
            return invoice;
        }

        private List<string> ValidateDataQuality(ShipmentInvoice invoice)
        {
            var issues = new List<string>();
            if (string.IsNullOrEmpty(invoice.InvoiceNo)) issues.Add("Invoice number is missing.");
            if (invoice.InvoiceTotal <= 0) issues.Add("Invoice total is invalid.");
            if (invoice.SubTotal <= 0) issues.Add("Subtotal is invalid.");
            if (string.IsNullOrEmpty(invoice.SupplierName)) issues.Add("Supplier name is missing.");
            return issues;
        }

        private T InvokePrivateMethod<T>(object obj, string methodName, params object[] parameters)
        {
            var type = obj.GetType();
            var method = type.GetMethod(methodName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (method == null)
            {
                throw new ArgumentException($"Method {methodName} not found on type {type.Name}");
            }

            var result = method.Invoke(obj, parameters);

            if (result is T typedResult)
            {
                return typedResult;
            }
            else if (result is Task<T> taskResult)
            {
                return taskResult.Result;
            }
            else if (result is Task task)
            {
                task.Wait();
                return default(T);
            }
            else
            {
                return (T)result;
            }
        }

        private async Task<T> InvokePrivateMethodAsync<T>(object obj, string methodName, params object[] parameters)
        {
            var type = obj.GetType();
            var method = type.GetMethod(methodName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (method == null)
            {
                throw new ArgumentException($"Method {methodName} not found on type {type.Name}");
            }

            var result = method.Invoke(obj, parameters);

            if (result is Task<T> taskResult)
            {
                return await taskResult;
            }
            else if (result is Task task)
            {
                await task;
                return default(T);
            }
            else if (result is T directResult)
            {
                return directResult;
            }
            else
            {
                return (T)result;
            }
        }

        #endregion

        #region Test Infrastructure

        /// <summary>
        /// Custom log sink for capturing log events during tests
        /// </summary>
        public class TestLogSink : ILogEventSink
        {
            private readonly List<(LogEventLevel Level, string Message)> _events;
            public TestLogSink(List<(LogEventLevel Level, string Message)> events) => _events = events;
            public void Emit(LogEvent logEvent) => _events.Add((logEvent.Level, logEvent.RenderMessage()));
        }

        #endregion
    }
}
