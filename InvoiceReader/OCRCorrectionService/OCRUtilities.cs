// File: OCRCorrectionService/OCRUtilities.cs
using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using EntryDataDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Helper Methods

        /// <summary>
        /// Cleans text for analysis
        /// </summary>
        private string CleanTextForAnalysis(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            var cleaned = Regex.Replace(text, @"-{30,}[^-]*-{30,}", "", RegexOptions.Multiline);

            if (cleaned.Length > 8000)
                cleaned = cleaned.Substring(0, 8000) + "...[truncated]";

            return cleaned;
        }

        /// <summary>
        /// Cleans JSON response
        /// </summary>
        private string CleanJsonResponse(string jsonResponse)
        {
            if (string.IsNullOrWhiteSpace(jsonResponse)) return string.Empty;

            var cleaned = Regex.Replace(jsonResponse, @"```json|```|'''|\uFEFF", string.Empty, RegexOptions.IgnoreCase);

            var startIndex = cleaned.IndexOf('{');
            var endIndex = cleaned.LastIndexOf('}');

            if (startIndex == -1 || endIndex == -1 || startIndex >= endIndex)
            {
                _logger.Warning("No valid JSON boundaries detected in response");
                return string.Empty;
            }

            return cleaned.Substring(startIndex, endIndex - startIndex + 1);
        }

        /// <summary>
        /// Parses corrected value to appropriate type
        /// </summary>
        private object ParseCorrectedValue(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            var cleanValue = value.Replace("$", "").Replace(",", "").Trim();

            if (IsNumericField(fieldName))
            {
                if (decimal.TryParse(cleanValue, out var decimalValue))
                    return decimalValue;
            }

            return value;
        }

        /// <summary>
        /// Determines if a field should contain numeric values
        /// </summary>
        private bool IsNumericField(string fieldName)
        {
            var numericFields = new[] {
                "invoicetotal", "subtotal", "totalinternalfreight",
                "totalothercost", "totalinsurance", "totaldeduction",
                "quantity", "cost", "totalcost", "discount"
            };
            return Array.Exists(numericFields, f => f.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets current field value for logging
        /// </summary>
        private object GetCurrentFieldValue(ShipmentInvoice invoice, string fieldName)
        {
            return fieldName.ToLower() switch
            {
                "invoicetotal" => invoice.InvoiceTotal,
                "subtotal" => invoice.SubTotal,
                "totalinternalfreight" => invoice.TotalInternalFreight,
                "totalothercost" => invoice.TotalOtherCost,
                "totalinsurance" => invoice.TotalInsurance,
                "totaldeduction" => invoice.TotalDeduction,
                "invoiceno" => invoice.InvoiceNo,
                "suppliername" => invoice.SupplierName,
                "currency" => invoice.Currency,
                _ => null
            };
        }

        /// <summary>
        /// Recalculates invoice detail total cost
        /// </summary>
        private void RecalculateDetailTotal(InvoiceDetails detail)
        {
            detail.TotalCost = (detail.Quantity * detail.Cost) - (detail.Discount ?? 0);
        }

        /// <summary>
        /// Gets string value from JSON element
        /// </summary>
        private string GetStringValue(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
                return prop.GetString();
            return null;
        }

        /// <summary>
        /// Gets double value from JSON element
        /// </summary>
        private double GetDoubleValue(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
            {
                if (prop.TryGetDouble(out var value)) return value;
                if (prop.TryGetDecimal(out var decimalValue)) return (double)decimalValue;
                if (prop.TryGetInt32(out var intValue)) return intValue;
            }
            return 0.0;
        }

        /// <summary>
        /// Truncates text for logging
        /// </summary>
        private string TruncateForLog(string text, int maxLength = 500)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
        }

        #endregion
    }
}