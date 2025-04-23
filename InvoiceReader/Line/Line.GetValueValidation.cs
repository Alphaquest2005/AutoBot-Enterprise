using System.Collections.Generic;
using System.Text.RegularExpressions;
using OCR.Business.Entities;

namespace WaterNut.DataSpace
{
    public partial class Line
    {
        private bool GetValueValidation(string instance, Match match,
            Dictionary<(Fields field, string instance), string> values, string methodName, out int? lineId)
        {
            lineId = this.OCR_Lines?.Id;
            _logger.Verbose(
                "Entering {MethodName} for LineId: {LineId}, Instance: {Instance}. Input Match Success: {MatchSuccess}", // Updated log
                methodName, lineId, instance, match?.Success ?? false); // Updated log

            // --- Input Validation ---
            if (match == null || !match.Success) // Updated validation
            {
                _logger.Warning(
                    "{MethodName}: Called with null or unsuccessful match for LineId: {LineId}. Exiting.", // Updated log
                    methodName, lineId);
                _logger.Verbose(
                    "Exiting {MethodName} for LineId: {LineId} due to null/unsuccessful match.", // Updated log
                    methodName, lineId);
                return true;
            }

            if (values == null)
            {
                _logger.Error(
                    "{MethodName}: Called with null values dictionary for LineId: {LineId}. Cannot store results. Exiting.",
                    methodName, lineId);
                _logger.Verbose("Exiting {MethodName} for LineId: {LineId} due to null values dictionary.", methodName,
                    lineId);
                return true;
            }

            if (this.OCR_Lines?.Fields == null)
            {
                _logger.Warning(
                    "{MethodName}: Called with null OCR_Lines or Fields for LineId: {LineId}. Cannot process fields. Exiting.",
                    methodName, lineId);
                _logger.Verbose("Exiting {MethodName} for LineId: {LineId} due to null OCR_Lines/Fields.", methodName,
                    lineId);
                return true;
            }

            return false;
        }
    }
}