using System;
using System.Collections.Generic;
using System.Linq;

namespace WaterNut.DataSpace
{
    public partial class Invoice
    {
        public List<dynamic> Read(string text)
        {
            int? invoiceId = this.OcrInvoices?.Id;
            string methodName = nameof(Read) + "(string)";
            _logger.Verbose("Entering {MethodName} for InvoiceId: {InvoiceId}. Input text length: {Length}", methodName,
                invoiceId, text?.Length ?? 0);

            if (text == null)
            {
                _logger.Warning(
                    "{MethodName}: Called with null text for InvoiceId: {InvoiceId}. Returning empty list structure.",
                    methodName, invoiceId);
                var emptyResult = new List<dynamic> { new List<IDictionary<string, object>>() };
                _logger.Verbose("Exiting {MethodName} for InvoiceId: {InvoiceId} due to null input.", methodName,
                    invoiceId);
                return emptyResult;
            }

            List<string> lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
            _logger.Verbose("{MethodName}: Split input text into {LineCount} lines for InvoiceId: {InvoiceId}.",
                methodName,
                lines.Count, invoiceId);

            // Call the primary Read method
            var result = Read(lines); // Primary Read method handles its own entry/exit logging

            _logger.Verbose("Exiting {MethodName} for InvoiceId: {InvoiceId}.", methodName, invoiceId);
            return result;
        }
    }

}