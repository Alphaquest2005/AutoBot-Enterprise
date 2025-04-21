using System.Text.RegularExpressions;
using OCR.Business.Entities;
using System.Collections.Generic; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added
using System.Globalization; // Added for TryParse

namespace WaterNut.DataSpace
{
    public partial class Line
    {
        // Logger instance is defined in the main Line.cs partial class file.
        // RegexTimeout constant is defined in the main Line.cs partial class file.

        private void FormatValues(int instance, Match match, // Changed from MatchCollection to Match
            Dictionary<(Fields Fields, int Instance), string> values)
        {
            string methodName = nameof(FormatValues);
            if (GetValueValidation(instance, match, values, methodName, out var lineId)) return;

            _logger.Verbose("{MethodName}: Input validation passed for LineId: {LineId}.", methodName, lineId);


            // --- Process Single Match --- // Updated comment
            // int matchIndex = -1; // Removed loop variable
            // foreach (Match match in matches) // Removed loop
            // { // Removed loop
            // matchIndex++; // Removed loop variable update
            _logger.Verbose(
                "{MethodName}: Processing single Match for LineId: {LineId}, Instance: {Instance}", // Updated log
                methodName, lineId, instance);
            // if (match == null || !match.Success) // Removed check as already done above
            // { // Removed check
            // _logger.Warning( // Removed log
            // "{MethodName}: Skipping null or unsuccessful match at index {MatchIndex} for LineId: {LineId}", // Removed log
            // methodName, matchIndex, lineId); // Removed log
            // continue; // Removed continue
            // } // Removed check

            // --- Process Top-Level Fields for this Match ---
            int fieldIndex = -1;
            var topLevelFields = OCR_Lines.Fields.Where(x => x != null && x.ParentField == null).ToList();
            _logger.Verbose("{MethodName}: Processing {FieldCount} top-level fields for the match.", // Updated log
                methodName, topLevelFields.Count);

            foreach (var field in topLevelFields)
            {
                fieldIndex++;
                int? fieldId = field.Id;
                string fieldName = field.Field ?? "UnknownField";
                _logger.Verbose(
                    "{MethodName}: Processing Field {FieldIndex}/{TotalFields} (Id: {FieldId}, Name: '{FieldName}') for Instance: {Instance}",
                    methodName, fieldIndex + 1, topLevelFields.Count, fieldId, fieldName, instance);

                var initialValue = GetInitialValue(match, field, methodName, fieldId);

                var formattedValue = ApplyFormating(initialValue, field, methodName, fieldId);


                // --- Store Value (Add/Update/Append) ---
                if (string.IsNullOrEmpty(formattedValue))
                {
                    _logger.Verbose(
                        "{MethodName}: Skipping FieldId: {FieldId} because formatted value is null or empty.",
                        methodName, fieldId);
                    continue; // Skip to next field
                }

                if (GetValue(instance, values, field, methodName, fieldId, formattedValue, out var valueKey)) continue; // Skip update


                // --- Process Child Fields Recursively ---
                ProcessChildFields(instance, values, field, methodName, fieldId, valueKey);

                // Removed logging statement using matchIndex
                // _logger.Verbose(
                //     "{MethodName}: Finished processing FieldId: {FieldId} for Match {MatchIndex}, Instance: {Instance}",
                //     methodName, fieldId, matchIndex + 1, instance);
            } // End foreach field

            // Removed logging statement using matchIndex
            // _logger.Verbose(
            //     "{MethodName}: Finished processing Match {MatchIndex} for LineId: {LineId}, Instance: {Instance}",
            //     methodName, matchIndex + 1, lineId, instance);
            // Removed foreach match closing brace

            _logger.Verbose(
                "Exiting {MethodName} for LineId: {LineId}, Instance: {Instance}. Final values count: {ValuesCount}",
                methodName, lineId, instance, values.Count);
        }

        // Assuming ReadChildField exists in another partial class part
        // private void ReadChildField(Fields childField, Dictionary<(Fields Fields, int Instance), string> values, string value) { ... }
    }
} // Added missing closing brace for the class