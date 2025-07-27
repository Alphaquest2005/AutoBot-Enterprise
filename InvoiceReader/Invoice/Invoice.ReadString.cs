using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WaterNut.DataSpace
{
    public partial class Template
    {
        public List<dynamic> Read(string text)
        {
            var methodStopwatch = Stopwatch.StartNew();
            int? invoiceId = this.OcrTemplates?.Id;
            string methodName = nameof(Read) + "(string)";
            _logger.Information("ACTION_START: {ActionName}. Context: [InvoiceId: {InvoiceId}, InputTextLength: {Length}]",
                methodName, invoiceId, text?.Length ?? 0);

            if (text == null)
            {
                methodStopwatch.Stop();
                _logger.Warning("ACTION_END_FAILURE: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. Reason: Called with null text.",
                    methodName, methodStopwatch.ElapsedMilliseconds);
                var emptyResult = new List<dynamic> { new List<IDictionary<string, object>>() };
                return emptyResult;
            }

            List<string> lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
            _logger.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): Split input text into {LineCount} lines for InvoiceId: {InvoiceId}.",
                methodName, "SplitText", lines.Count, invoiceId);

            // Call the primary Read method
            var result = Read(lines); // Primary Read method handles its own entry/exit logging

           
            methodStopwatch.Stop();
            _logger.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. ResultLineCount: {ResultLineCount}",
                methodName, methodStopwatch.ElapsedMilliseconds, result?.Count ?? 0);
            return result;
        }
    }

}