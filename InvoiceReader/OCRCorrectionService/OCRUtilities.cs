// File: OCRCorrectionService/OCRUtilities.cs
using EntryDataDS.Business.Entities; // For ShipmentInvoice in GetCurrentFieldValue
using Serilog; // Assuming ILogger is from Serilog, available as this._logger
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace WaterNut.DataSpace
{
    using System.Globalization;
    using System.IO;

    public partial class OCRCorrectionService
    {
        #region Text Manipulation and Cleaning Utilities

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Text cleaning with LLM diagnostic workflow and business success criteria
        /// 
        /// **MANDATORY LLM BEHAVIOR RULES**: LOG PRESERVATION + LOG-FIRST ANALYSIS + CONTINUOUS LOG ENHANCEMENT + SUCCESS CRITERIA VALIDATION
        /// **LLM DIAGNOSTIC WORKFLOW**: Phase 1 Analysis → Phase 2 Enhancement → Phase 3 Evidence-Based Implementation → Phase 4 Success Criteria Validation
        /// **METHOD PURPOSE**: Cleans raw OCR text by removing artifacts, normalizing formatting, and truncating for LLM processing with gift card preservation
        /// **BUSINESS OBJECTIVE**: Ensure optimal OCR text quality through systematic cleaning while preserving critical business content like gift cards
        /// **SUCCESS CRITERIA**: Must validate input, clean systematically, preserve critical content, truncate appropriately, and return optimized text
        /// 
        /// Cleans raw OCR text by removing common artifacts and truncating if too long for LLM processing.
        /// </summary>
        public string CleanTextForAnalysis(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            // Log entry with version marker
            _logger?.Debug("CleanTextForAnalysis | v2.1 | Start: {InitialText}", TruncateForLog(text, 200));

            // **CRITICAL CHECK**: Track gift card content through cleaning process
            bool initialHasGiftCard = text.Contains("Gift Card") || text.Contains("-$6.99");
            _logger?.Information("🔍 **TEXT_CLEANING_GIFT_CHECK_INITIAL**: Original text contains gift card? Expected=TRUE, Actual={HasGiftCard}", initialHasGiftCard);
            if (initialHasGiftCard)
            {
                var giftLines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(l => l.Contains("Gift") || l.Contains("Card") || l.Contains("-$6.99")).ToList();
                _logger?.Information("🔍 **TEXT_CLEANING_GIFT_LINES_INITIAL**: Found {Count} gift card lines: {Lines}",
                    giftLines.Count, string.Join(" | ", giftLines));
            }
            else
            {
                _logger?.Error("❌ **TEXT_CLEANING_ASSERTION_FAILED**: Original text does not contain gift card - cleaning cannot preserve what doesn't exist");
            }

            // 1. Normalize all line endings to a single '\n' for consistent processing.
            string cleaned = text.Replace("\r\n", "\n").Replace("\r", "\n");
            _logger?.Debug("CleanTextForAnalysis | v2.1 | Step 1 (Normalize Newlines): {Text}", TruncateForLog(cleaned, 200));

            // 2. Remove long separator lines (e.g., ------------------)
            cleaned = Regex.Replace(cleaned, @"(?m)^\s*(?:-{20,}|_{20,}|={20,})\s*$", "", RegexOptions.Multiline);
            _logger?.Debug("CleanTextForAnalysis | v2.1 | Step 2 (Remove Separators): {Text}", TruncateForLog(cleaned, 200));

            // 3. Trim whitespace from the start and end of every line.
            cleaned = Regex.Replace(cleaned, @"(?m)^[ \t]+|[ \t]+$", "");
            _logger?.Debug("CleanTextForAnalysis | v2.1 | Step 3 (Trim Lines): {Text}", TruncateForLog(cleaned, 200));

            // 4. Collapse multiple spaces/tabs within a line to a single space.
            cleaned = Regex.Replace(cleaned, @"[ \t]{2,}", " ");
            _logger?.Debug("CleanTextForAnalysis | v2.1 | Step 4 (Collapse Spaces): {Text}", TruncateForLog(cleaned, 200));

            // 5. Collapse 3 or more consecutive newlines into exactly two (a single blank line).
            cleaned = Regex.Replace(cleaned, @"\n{3,}", "\n\n");
            _logger?.Debug("CleanTextForAnalysis | v2.1 | Step 5 (Collapse Newlines): {Text}", TruncateForLog(cleaned, 200));

            // 6. Trim the entire block of text to remove any leading/trailing blank lines created by the process.
            cleaned = cleaned.Trim();
            _logger?.Debug("CleanTextForAnalysis | v2.1 | Step 6 (Final Trim): {Text}", TruncateForLog(cleaned, 200));

            // 7. Truncate if excessively long
            int maxLength = 15000;
            if (cleaned.Length > maxLength)
            {
                cleaned = cleaned.Substring(0, maxLength) + "...[text truncated]";
                _logger?.Debug("CleanTextForAnalysis | v2.1 | Step 7 (Truncate): Text was truncated to {MaxLength} characters.", maxLength);
            }

            _logger?.Debug("CleanTextForAnalysis | v2.1 | Final Result: {FinalText}", TruncateForLog(cleaned, 200));

            // **FINAL CHECK**: Verify gift card content survived cleaning process
            bool finalHasGiftCard = cleaned.Contains("Gift Card") || cleaned.Contains("-$6.99");
            _logger?.Information("🔍 **TEXT_CLEANING_GIFT_CHECK_FINAL**: Cleaned text contains gift card? Expected=TRUE, Actual={HasGiftCard}", finalHasGiftCard);

            if (initialHasGiftCard && !finalHasGiftCard)
            {
                _logger?.Error("❌ **TEXT_CLEANING_DATA_LOSS**: Gift card content was LOST during cleaning process - DeepSeek will not detect missing fields");
            }
            else if (initialHasGiftCard && finalHasGiftCard)
            {
                _logger?.Information("✅ **TEXT_CLEANING_PRESERVED**: Gift card content successfully preserved through cleaning");
            }
            else if (finalHasGiftCard)
            {
                var finalGiftLines = cleaned.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(l => l.Contains("Gift") || l.Contains("Card") || l.Contains("-$6.99")).ToList();
                _logger?.Information("🔍 **TEXT_CLEANING_GIFT_LINES_FINAL**: Final {Count} gift card lines: {Lines}",
                    finalGiftLines.Count, string.Join(" | ", finalGiftLines));
            }

            return cleaned;
        }

        /// <summary>
        /// Cleans a raw JSON response string, typically from an LLM, by removing common non-JSON artifacts
        /// like markdown code fences (```json ... ```) and Byte Order Marks (BOM).
        /// It attempts to extract the main JSON object or array.
        /// </summary>
        public string CleanJsonResponse(string jsonResponse)
        {
            if (string.IsNullOrWhiteSpace(jsonResponse)) return string.Empty;

            _logger?.Information("CleanJsonResponse input: Length={Length}, FirstChar={FirstChar}, StartsWithBrace={StartsWithBrace}, JSON={@JSON}",
                jsonResponse.Length,
                jsonResponse.Length > 0 ? jsonResponse[0].ToString() : "EMPTY",
                jsonResponse.StartsWith("{"),
                jsonResponse);

            string cleaned = jsonResponse.Trim();

            _logger?.Information("After trim: Length={Length}, FirstChar={FirstChar}, StartsWithBrace={StartsWithBrace}, JSON={@JSON}",
                cleaned.Length,
                cleaned.Length > 0 ? cleaned[0].ToString() : "EMPTY",
                cleaned.StartsWith("{"),
                cleaned);

            bool hasBom = cleaned.Length > 0 && cleaned[0] == '\uFEFF';
            _logger?.Information("BOM check: HasBOM={HasBOM}, FirstCharCode={FirstCharCode}, Length={Length}, BOMCharCode=65279",
                hasBom,
                cleaned.Length > 0 ? ((int)cleaned[0]).ToString() : "EMPTY",
                cleaned.Length);

            if (hasBom)
            {
                cleaned = cleaned.Substring(1);
                _logger?.Information("Removed BOM: Length={Length}, FirstChar={FirstChar}, JSON={@JSON}",
                    cleaned.Length,
                    cleaned.Length > 0 ? cleaned[0].ToString() : "EMPTY",
                    cleaned);
            }

            // Remove markdown code block fences - but only if they actually exist
            if (cleaned.StartsWith("```"))
            {
                cleaned = Regex.Replace(cleaned, @"^```(?:json)?\s*[\r\n]*", "", RegexOptions.IgnoreCase);
                _logger?.Information("Applied backtick removal: Length={Length}, FirstChar={FirstChar}, JSON={@JSON}",
                    cleaned.Length,
                    cleaned.Length > 0 ? cleaned[0].ToString() : "EMPTY",
                    cleaned);
            }

            // Only apply the ending regex if the string ends with backticks
            if (cleaned.EndsWith("```"))
            {
                cleaned = Regex.Replace(cleaned, @"[\r\n]*```\s*$", "", RegexOptions.IgnoreCase);
                _logger?.Information("Applied ending backtick removal: Length={Length}, FirstChar={FirstChar}, JSON={@JSON}",
                    cleaned.Length,
                    cleaned.Length > 0 ? cleaned[0].ToString() : "EMPTY",
                    cleaned);
            }

            cleaned = cleaned.Trim();

            _logger?.Information("Before JSON extraction: Length={Length}, FirstChar={FirstChar}, JSON={@JSON}",
                cleaned.Length,
                cleaned.Length > 0 ? cleaned[0].ToString() : "EMPTY",
                cleaned);

            // Find the first '{' or '[' and the last '}' or ']' to extract the core JSON part
            int firstBrace = cleaned.IndexOf('{');
            int firstBracket = cleaned.IndexOf('[');
            int startIndex = -1;

            if (firstBrace == -1 && firstBracket == -1)
            {
                _logger?.Warning("CleanJsonResponse: No JSON object ('{{') or array ('[') start found in response: {ResponseSnippet}", TruncateForLog(cleaned, 100));
                return string.Empty;
            }

            if (firstBrace != -1 && firstBracket != -1) startIndex = Math.Min(firstBrace, firstBracket);
            else if (firstBrace != -1) startIndex = firstBrace;
            else startIndex = firstBracket; // firstBracket must be != -1 here

            char expectedEndChar = (cleaned[startIndex] == '{') ? '}' : ']';
            int endIndex = cleaned.LastIndexOf(expectedEndChar);

            if (startIndex == -1 || endIndex == -1 || endIndex < startIndex)
            {
                _logger?.Warning("CleanJsonResponse: Could not find valid JSON start/end boundaries. StartIndex: {StartIndex}, EndIndex: {EndIndex}. Response snippet: {ResponseSnippet}", startIndex, endIndex, TruncateForLog(cleaned, 100));
                return string.Empty;
            }

            var extractedJson = cleaned.Substring(startIndex, endIndex - startIndex + 1);

            // **🔧 JSON_ESCAPING_FIX**: Fix common JSON escaping issues that cause parsing failures
            _logger?.Information("🔧 **JSON_ESCAPING_FIX**: Applying JSON string escape fixes to prevent parsing errors");
            var fixedJson = FixJsonStringEscaping(extractedJson);
            
            _logger?.Information("🔍 **JSON_ESCAPING_RESULT**: Original length={OriginalLength}, Fixed length={FixedLength}", 
                extractedJson?.Length ?? 0, fixedJson?.Length ?? 0);

            return fixedJson;
        }

        /// <summary>
        /// Fixes common JSON string escaping issues that cause parsing failures.
        /// Specifically addresses: invalid escape sequences like \', unescaped quotes, etc.
        /// </summary>
        private string FixJsonStringEscaping(string json)
        {
            if (string.IsNullOrEmpty(json)) return json;

            try
            {
                var result = new StringBuilder(json.Length);
                bool inString = false;
                bool inEscape = false;

                for (int i = 0; i < json.Length; i++)
                {
                    char current = json[i];
                    char next = (i + 1 < json.Length) ? json[i + 1] : '\0';

                    if (!inString)
                    {
                        // Outside strings - copy as-is and detect string starts
                        result.Append(current);
                        if (current == '"') inString = true;
                    }
                    else
                    {
                        // Inside strings - handle escaping
                        if (inEscape)
                        {
                            // Previous char was backslash - validate escape sequence
                            switch (current)
                            {
                                case '"':
                                case '\\':
                                case '/':
                                case 'b':
                                case 'f':
                                case 'n':
                                case 'r':
                                case 't':
                                    // Valid escape sequences
                                    result.Append(current);
                                    break;
                                case 'u':
                                    // Unicode escape - need to validate 4 hex digits follow
                                    result.Append(current);
                                    break;
                                case '\'':
                                    // **CRITICAL FIX**: Single quote doesn't need escaping in JSON
                                    _logger?.Warning("🔧 **FIXED_INVALID_ESCAPE**: Removed invalid escape sequence \\' -> '");
                                    result.Length--; // Remove the backslash we just added
                                    result.Append('\'');
                                    break;
                                default:
                                    // Invalid escape sequence - remove the backslash
                                    _logger?.Warning("🔧 **FIXED_INVALID_ESCAPE**: Removed invalid escape sequence \\{Character} -> {Character}", current, current);
                                    result.Length--; // Remove the backslash we just added
                                    result.Append(current);
                                    break;
                            }
                            inEscape = false;
                        }
                        else if (current == '\\')
                        {
                            // Start of escape sequence
                            result.Append(current);
                            inEscape = true;
                        }
                        else if (current == '"')
                        {
                            // End of string
                            result.Append(current);
                            inString = false;
                        }
                        else
                        {
                            // Regular character in string
                            result.Append(current);
                        }
                    }
                }

                var fixedJson = result.ToString();
                
                // Log if we made any fixes
                if (fixedJson != json)
                {
                    _logger?.Information("✅ **JSON_ESCAPING_APPLIED**: Fixed {Changes} JSON escaping issues", 
                        json.Length - fixedJson.Length);
                }

                return fixedJson;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "❌ **JSON_ESCAPING_FAILED**: Error fixing JSON escaping, returning original");
                return json;
            }
        }

        #endregion

        #region JSON Element Parsing Utilities (with Logging)

        public string GetStringValueWithLogging(JsonElement element, string propertyName, int itemIndex, bool isOptional = false)
        {
            try
            {
                if (!element.TryGetProperty(propertyName, out var prop))
                {
                    if (!isOptional) _logger?.Debug("Item {ItemIndex}: Property '{PropertyName}' not found.", itemIndex, propertyName);
                    return null;
                }
                // _logger?.Verbose("Item {ItemIndex}.{PropertyName}: Type={ValueKind}, RawValue={RawValue}", itemIndex, propertyName, prop.ValueKind, TruncateForLog(prop.GetRawText(), 50));
                return prop.ValueKind switch
                {
                    JsonValueKind.String => prop.GetString(),
                    JsonValueKind.Number => prop.GetRawText(), // Convert number to its string representation
                    JsonValueKind.True => "true",
                    JsonValueKind.False => "false",
                    JsonValueKind.Null => null,
                    _ => prop.GetRawText() // Fallback for arrays/objects if GetString is called on them
                };
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Item {ItemIndex}: Error extracting string for property '{PropertyName}'.", itemIndex, propertyName);
                return null;
            }
        }

        internal double GetDoubleValueWithLogging(JsonElement element, string propertyName, int itemIndex, double defaultValue = 0.0, bool isOptional = false)
        {
            try
            {
                if (!element.TryGetProperty(propertyName, out var prop))
                {
                    if (!isOptional) _logger?.Debug("Item {ItemIndex}: Property '{PropertyName}' not found, using default {Default}.", itemIndex, propertyName, defaultValue);
                    return defaultValue;
                }
                // _logger?.Verbose("Item {ItemIndex}.{PropertyName}: Type={ValueKind}, RawValue={RawValue}", itemIndex, propertyName, prop.ValueKind, TruncateForLog(prop.GetRawText(), 50));
                if (prop.ValueKind == JsonValueKind.Number && prop.TryGetDouble(out var val)) return val;
                if (prop.ValueKind == JsonValueKind.String && double.TryParse(prop.GetString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var parsedVal)) return parsedVal;
                if (prop.ValueKind != JsonValueKind.Null && !isOptional) _logger?.Warning("Item {ItemIndex}: Cannot parse '{PropertyName}' as double (Raw: '{RawVal}'). Using default {Default}.", itemIndex, propertyName, prop.GetRawText(), defaultValue);
                return defaultValue;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Item {ItemIndex}: Error extracting double for property '{PropertyName}'.", itemIndex, propertyName);
                return defaultValue;
            }
        }

        internal int GetIntValueWithLogging(JsonElement element, string propertyName, int itemIndex, int defaultValue = 0, bool isOptional = false)
        {
            try
            {
                if (!element.TryGetProperty(propertyName, out var prop))
                {
                    if (!isOptional) _logger?.Debug("Item {ItemIndex}: Property '{PropertyName}' not found, using default {Default}.", itemIndex, propertyName, defaultValue);
                    return defaultValue;
                }
                // _logger?.Verbose("Item {ItemIndex}.{PropertyName}: Type={ValueKind}, RawValue={RawValue}", itemIndex, propertyName, prop.ValueKind, TruncateForLog(prop.GetRawText(), 50));
                if (prop.ValueKind == JsonValueKind.Number && prop.TryGetInt32(out var val)) return val;
                if (prop.ValueKind == JsonValueKind.String && int.TryParse(prop.GetString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var parsedVal)) return parsedVal;
                if (prop.ValueKind != JsonValueKind.Null && !isOptional) _logger?.Warning("Item {ItemIndex}: Cannot parse '{PropertyName}' as int (Raw: '{RawVal}'). Using default {Default}.", itemIndex, propertyName, prop.GetRawText(), defaultValue);
                return defaultValue;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Item {ItemIndex}: Error extracting int for property '{PropertyName}'.", itemIndex, propertyName);
                return defaultValue;
            }
        }

        internal bool GetBooleanValueWithLogging(JsonElement element, string propertyName, int itemIndex, bool defaultValue = false, bool isOptional = false)
        {
            try
            {
                if (!element.TryGetProperty(propertyName, out var prop))
                {
                    if (!isOptional) _logger?.Debug("Item {ItemIndex}: Property '{PropertyName}' not found, using default {Default}.", itemIndex, propertyName, defaultValue);
                    return defaultValue;
                }
                // _logger?.Verbose("Item {ItemIndex}.{PropertyName}: Type={ValueKind}, RawValue={RawValue}", itemIndex, propertyName, prop.ValueKind, TruncateForLog(prop.GetRawText(), 50));
                if (prop.ValueKind == JsonValueKind.True) return true;
                if (prop.ValueKind == JsonValueKind.False) return false;
                if (prop.ValueKind == JsonValueKind.String && bool.TryParse(prop.GetString(), out var parsedBool)) return parsedBool;
                if (prop.ValueKind == JsonValueKind.Number && prop.TryGetInt32(out var intVal)) return intVal != 0; // Treat 0 as false, others as true
                if (prop.ValueKind != JsonValueKind.Null && !isOptional) _logger?.Warning("Item {ItemIndex}: Cannot parse '{PropertyName}' as bool (Raw: '{RawVal}'). Using default {Default}.", itemIndex, propertyName, prop.GetRawText(), defaultValue);
                return defaultValue;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Item {ItemIndex}: Error extracting boolean for property '{PropertyName}'.", itemIndex, propertyName);
                return defaultValue;
            }
        }

        public List<string> ParseContextLinesArray(JsonElement element, string propertyName, int itemIndex)
        {
            var contextLines = new List<string>();
            try
            {
                if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in prop.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.String) contextLines.Add(item.GetString());
                        else if (item.ValueKind != JsonValueKind.Null) contextLines.Add(item.GetRawText()); // Add non-string array elements as raw text
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Item {ItemIndex}: Error parsing context lines for property '{PropertyName}'.", itemIndex, propertyName);
            }
            return contextLines;
        }

        #endregion

        #region Document Text Utilities

        /// <summary>
        /// Extracts the text of a specific line number (1-based) from a multi-line string.
        /// </summary>
        public string GetOriginalLineText(string fullText, int lineNumber)
        {
            if (string.IsNullOrEmpty(fullText) || lineNumber <= 0)
                return ""; // Or null, depending on desired behavior for invalid input

            // Split by common newline sequences. Using StringSplitOptions.None to preserve empty lines if line numbers are absolute.
            var lines = fullText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            if (lineNumber <= lines.Length)
            {
                return lines[lineNumber - 1]; // 0-indexed access for 1-based lineNumber
            }
            _logger?.Debug("GetOriginalLineText: LineNumber {LineNum} is out of bounds for text with {TotalLines} lines.", lineNumber, lines.Length);
            return ""; // Line number out of bounds
        }

        /// <summary>
        /// Extracts a window of text lines around a specified center line number.
        /// </summary>
        /// <param name="fullText">The complete document text.</param>
        /// <param name="centerLineNumber">The 1-based line number for the center of the window.</param>
        /// <param name="windowHalfSize">Number of lines to include before and after the center line.</param>
        /// <returns>A string containing the window of text, with lines separated by newlines.</returns>
        public string ExtractWindowText(string fullText, int centerLineNumber, int windowHalfSize)
        {
            if (string.IsNullOrEmpty(fullText) || centerLineNumber <= 0 || windowHalfSize < 0)
                return "";

            var lines = fullText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var centerIndex = centerLineNumber - 1; // 0-based

            if (centerIndex < 0 || centerIndex >= lines.Length)
            {
                _logger?.Debug("ExtractWindowText: CenterLineNumber {CenterLineNum} is out of bounds.", centerLineNumber);
                return "";
            }

            var startIndex = Math.Max(0, centerIndex - windowHalfSize);
            var endIndex = Math.Min(lines.Length - 1, centerIndex + windowHalfSize);

            if (startIndex > endIndex) return ""; // Should not happen with valid inputs

            return string.Join("\n", lines.Skip(startIndex).Take(endIndex - startIndex + 1));
        }

        /// <summary>
        /// Extracts all named capture groups from a given regex pattern string.
        /// </summary>
        public List<string> ExtractNamedGroupsFromRegex(string regexPattern)
        {
            if (string.IsNullOrEmpty(regexPattern)) return new List<string>();
            try
            {
                // Regex.GetGroupNames() includes "0" for the full match, and then named/numbered groups.
                // We filter out the "0" and any other purely numeric group names.
                var regex = new Regex(regexPattern); // Throws ArgumentException on invalid pattern
                return regex.GetGroupNames().Where(name => name != "0" && !int.TryParse(name, out _)).ToList();
            }
            catch (ArgumentException ex)
            {
                _logger?.Error(ex, "ExtractNamedGroupsFromRegex: Invalid regex pattern syntax: {Pattern}", regexPattern);
                return new List<string>(); // Return empty on error
            }
        }

        #endregion

        #region Field Type and Property Mapping Utilities

        /// <summary>
        /// Parses a string value into an appropriate object type based on the target field's characteristics.
        /// CRITICAL FIX: Now correctly returns the appropriate C# type for all supported data types.
        /// Logging is enhanced to meet the Assertive Self-Documenting Logging Mandate v4.3.
        /// </summary>
        public object ParseCorrectedValue(string valueToParse, string targetFieldName)
        {
            _logger.Error("➡️ **Enter ParseCorrectedValue** for Field '{Field}' with input value '{Value}'.", targetFieldName, valueToParse);

            if (valueToParse == null)
            {
                _logger.Error("   - ✅ **Exit ParseCorrectedValue**: Input was null, returning null.");
                return null;
            }

            var fieldInfo = this.MapDeepSeekFieldToDatabase(targetFieldName);
            // Use a case-insensitive match for the data type.
            string dataType = fieldInfo?.DataType?.ToLowerInvariant() ?? "string";
            _logger.Error("   - **ANALYSIS**: Mapped Field '{Field}' to DataType '{DataType}'.", targetFieldName, dataType);

            var numberStyles = NumberStyles.Any;
            var cultureInfo = CultureInfo.InvariantCulture;

            // --- Handle Numeric Types ---
            if (dataType == "number" || dataType == "decimal" || dataType == "double" || dataType == "currency" || dataType == "int" || dataType == "integer")
            {
                string cleanedValue = Regex.Replace(valueToParse, @"[^\d.,-]", "").Trim();
                _logger.Error("   - **CLEANING_NUMERIC**: Initial cleaning of '{Original}' resulted in '{Cleaned}'.", valueToParse, cleanedValue);

                if (cleanedValue.Contains(',') && cleanedValue.Contains('.'))
                {
                    cleanedValue = cleanedValue.LastIndexOf(',') < cleanedValue.LastIndexOf('.')
                        ? cleanedValue.Replace(",", "")
                        : cleanedValue.Replace(".", "").Replace(",", ".");
                    _logger.Error("   - **CLEANING_CULTURE**: Handled mixed separators. Value is now '{Cleaned}'.", cleanedValue);
                }
                else if (cleanedValue.Contains(','))
                {
                    cleanedValue = cleanedValue.Replace(',', '.');
                    _logger.Error("   - **CLEANING_CULTURE**: Replaced comma decimal separator. Value is now '{Cleaned}'.", cleanedValue);
                }

                switch (dataType)
                {
                    case "number": // This is our primary pseudo-type
                    case "decimal":
                    case "double":
                    case "currency":
                        if (double.TryParse(cleanedValue, numberStyles, cultureInfo, out var doubleResult))
                        {
                            _logger.Error("   - ✅ **Exit ParseCorrectedValue**: Successfully parsed '{Cleaned}' as Double: {Result}.", cleanedValue, doubleResult);
                            return doubleResult;
                        }
                        break;
                    case "int":
                    case "integer":
                        if (int.TryParse(cleanedValue, numberStyles, cultureInfo, out var intResult))
                        {
                            _logger.Error("   - ✅ **Exit ParseCorrectedValue**: Successfully parsed '{Cleaned}' as Int32: {Result}.", cleanedValue, intResult);
                            return intResult;
                        }
                        break;
                }
            }

            // --- Handle Date/Time Types ---
            if (dataType.Contains("date")) // Catches "date", "datetime", "english date"
            {
                if (DateTime.TryParse(valueToParse, cultureInfo, DateTimeStyles.AssumeUniversal, out var dateResult))
                {
                    _logger.Error("   - ✅ **Exit ParseCorrectedValue**: Successfully parsed '{Original}' as DateTime: {Result}.", valueToParse, dateResult);
                    return dateResult;
                }
            }

            // --- Handle Boolean Types ---
            if (dataType == "bool" || dataType == "boolean")
            {
                if (bool.TryParse(valueToParse, out var boolResult))
                {
                    _logger.Error("   - ✅ **Exit ParseCorrectedValue**: Successfully parsed '{Original}' as Boolean: {Result}.", valueToParse, boolResult);
                    return boolResult;
                }
                if (valueToParse.Trim().Equals("1")) return true;
                if (valueToParse.Trim().Equals("0")) return false;
            }

            // --- Fallback for String and unhandled types ---
            _logger.Error("   - ✅ **Exit ParseCorrectedValue**: No specific parsing rule matched for DataType '{DataType}'. Returning original string: '{Original}'.", dataType, valueToParse);
            return valueToParse;
        }

        /// <summary>
        /// Determines if a field (by its canonical/DB name) is expected to hold a numeric value.
        /// </summary>
        public bool IsNumericField(string canonicalOrMappedFieldName)
        {
            var fieldInfo = this.MapDeepSeekFieldToDatabase(canonicalOrMappedFieldName);
            if (fieldInfo != null)
            {
                string dataType = fieldInfo.DataType?.ToLowerInvariant();
                return dataType == "decimal" || dataType == "double" || dataType == "int" || dataType == "integer" || dataType == "currency";
            }
            _logger?.Verbose("IsNumericField: No mapping info for '{FieldName}', cannot determine if numeric from type.", canonicalOrMappedFieldName);
            return false; // Default if type unknown
        }

        /// <summary>
        /// Retrieves the current value of a field from a ShipmentInvoice object using reflection.
        /// </summary>
        public object GetCurrentFieldValue(ShipmentInvoice invoice, string fieldNameFromError)
        {
            if (invoice == null || string.IsNullOrEmpty(fieldNameFromError)) return null;

            var fieldInfo = this.MapDeepSeekFieldToDatabase(fieldNameFromError);
            var targetPropertyName = fieldInfo?.DatabaseFieldName ?? fieldNameFromError;

            try
            {
                var propInfo = typeof(ShipmentInvoice).GetProperty(targetPropertyName,
                    System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (propInfo != null)
                {
                    return propInfo.GetValue(invoice);
                }

                if (targetPropertyName.StartsWith("invoicedetail", StringComparison.OrdinalIgnoreCase) ||
                    (fieldInfo != null && fieldInfo.EntityType == "InvoiceDetails"))
                {
                    var parts = fieldNameFromError.Split('_');
                    if (parts.Length >= 3 && parts[0].Equals("InvoiceDetail", StringComparison.OrdinalIgnoreCase))
                    {
                        if (int.TryParse(Regex.Match(parts[1], @"\d+").Value, out int lineNum))
                        {
                            var detailItem = invoice.InvoiceDetails?.FirstOrDefault(d => d.LineNumber == lineNum);
                            if (detailItem != null)
                            {
                                string detailFieldName = parts[2];
                                var detailPropInfo = typeof(InvoiceDetails).GetProperty(detailFieldName,
                                    System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                if (detailPropInfo != null)
                                {
                                    return detailPropInfo.GetValue(detailItem);
                                }
                            }
                        }
                    }
                }
                _logger?.Debug("GetCurrentFieldValue: Property '{TargetProp}' (from error field '{ErrorField}') not found on ShipmentInvoice or its details.", targetPropertyName, fieldNameFromError);
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Exception trying to get current value for field '{ErrorField}' (target: {TargetProp}).", fieldNameFromError, targetPropertyName);
            }
            return null;
        }

        /// <summary>
        /// Determines the invoice type (e.g., "Amazon", "Generic") based on file path heuristics.
        /// </summary>
        internal string DetermineInvoiceType(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return "Unknown";
            var fileName = Path.GetFileName(filePath).ToLowerInvariant();
            if (fileName.Contains("amazon")) return "Amazon";
            if (fileName.Contains("temu")) return "Temu";
            if (fileName.Contains("shein")) return "Shein";
            if (fileName.Contains("alibaba")) return "Alibaba";
            return "Generic";
        }

        /// <summary>
        /// Maps OCR template field names (from OCR.Business.Entities.Fields.Field) to canonical C# property names.
        /// </summary>
        public static string MapTemplateFieldToPropertyName(string templateDbFieldName)
        {
            if (string.IsNullOrWhiteSpace(templateDbFieldName)) return templateDbFieldName;

            return templateDbFieldName.ToLowerInvariant() switch
            {
                "invoicetotal" or "total" or "invoice_total" or "grandtotal" or "amountdue" => "InvoiceTotal",
                "subtotal" or "sub_total" => "SubTotal",
                "totalinternalfreight" or "freight" or "internal_freight" or "shipping" or "shippingandhandling" => "TotalInternalFreight",
                "totalothercost" or "other_cost" or "othercost" or "tax" or "vat" or "othercharges" => "TotalOtherCost",
                "totalinsurance" or "insurance" => "TotalInsurance",
                "totaldeduction" or "deduction" or "discount" or "giftcard" or "promotion" => "TotalDeduction",
                "invoiceno" or "invoice_no" or "invoice_number" or "invoice" or "invoiceid" or "ordernumber" or "orderno" => "InvoiceNo",
                "invoicedate" or "invoice_date" or "date" or "issuedate" => "InvoiceDate",
                "currency" => "Currency",
                "suppliername" or "supplier_name" or "supplier" or "vendor" or "soldby" or "from" => "SupplierName",
                "supplieraddress" or "supplier_address" or "address" => "SupplierAddress",
                "itemdescription" or "description" or "productdescription" or "item" or "productname" => "ItemDescription",
                "quantity" or "qty" => "Quantity",
                "cost" or "price" or "unitprice" => "Cost",
                "totalcost" or "linetotal" or "amount" => "TotalCost",
                "units" => "Units",
                _ => templateDbFieldName
            };
        }

        public static bool IsMetadataField(string fieldName)
        {
            return new[] { "LineNumber", "FileLineNumber", "Section", "Instance" }.Contains(fieldName);
        }

        #endregion
    }
}