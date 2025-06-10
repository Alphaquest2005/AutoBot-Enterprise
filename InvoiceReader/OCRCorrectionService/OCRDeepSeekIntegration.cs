// File: OCRCorrectionService/OCRDeepSeekIntegration.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Serilog; // ILogger is available as this._logger

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Handles direct interactions with the DeepSeek LLM API for OCR correction tasks,
    /// including general response parsing and specific regex creation requests.
    /// </summary>
    public partial class OCRCorrectionService
    {
        #region General DeepSeek Response Processing for Corrections

        /// <summary>
        /// Processes a generic JSON response from DeepSeek (expected to contain errors/corrections)
        /// and transforms it into a list of CorrectionResult objects.
        /// </summary>
        /// <param name="deepSeekResponseJson">The raw JSON string from DeepSeek.</param>
        /// <param name="originalDocumentText">The full original text of the document for context.</param>
        /// <returns>A list of CorrectionResult objects derived from the DeepSeek response.</returns>
        public List<CorrectionResult> ProcessDeepSeekCorrectionResponse(string deepSeekResponseJson, string originalDocumentText)
        {
            var corrections = new List<CorrectionResult>();
            
            // **PARSING_ENTRY_STATE**: Log entry state with complete input parameters
            _logger.Error("🔍 **PARSING_ENTRY_STATE**: ProcessDeepSeekCorrectionResponse called");
            _logger.Error("🔍 **PARSING_INPUT_PARAMS**: Response length={ResponseLength}, OriginalText length={OriginalTextLength}", 
                deepSeekResponseJson?.Length ?? 0, originalDocumentText?.Length ?? 0);
            
            if (string.IsNullOrWhiteSpace(deepSeekResponseJson))
            {
                _logger.Error("❌ **PARSING_ASSERTION_FAILED**: Received null or empty response from DeepSeek - cannot parse corrections");
                return corrections;
            }

            // **RAW_RESPONSE_LOGGING**: Log complete raw response for debugging
            _logger.Error("🔍 **PARSING_RAW_RESPONSE**: Complete DeepSeek JSON response: {RawResponse}", deepSeekResponseJson);
            
            // **EXPECTATION**: Raw response should contain valid JSON with 'errors' array
            bool responseContainsErrors = deepSeekResponseJson.Contains("\"errors\"");
            _logger.Error("🔍 **PARSING_EXPECTATION_1**: Raw response contains 'errors' key? Expected=TRUE, Actual={ContainsErrors}", responseContainsErrors);
            
            if (!responseContainsErrors)
            {
                _logger.Error("❌ **PARSING_ASSERTION_FAILED_1**: Raw response does not contain 'errors' key - DeepSeek response format unexpected");
            }

            try
            {
                _logger.Error("🔍 **PARSING_STATE_TRANSITION**: RAW_RESPONSE → JSON_ELEMENT_PARSING");
                
                // **STATE TRANSITION 1**: Raw string → JsonElement
                JsonElement? responseDataRoot = ParseDeepSeekResponseToElement(deepSeekResponseJson);
                
                // **PARSING_RESULT**: Check if JSON parsing succeeded
                bool jsonParsingSucceeded = responseDataRoot.HasValue;
                _logger.Error("🔍 **PARSING_RESULT_1**: JSON parsing succeeded? Expected=TRUE, Actual={Succeeded}", jsonParsingSucceeded);
                
                if (responseDataRoot == null)
                {
                    _logger.Error("❌ **PARSING_ASSERTION_FAILED_2**: Failed to parse DeepSeek response into valid JSON structure - data loss occurred");
                    return corrections;
                }

                // **JSON_STRUCTURE_LOGGING**: Log the parsed JSON structure
                _logger.Error("🔍 **PARSING_JSON_STRUCTURE**: Parsed JSON root type={ValueKind}", responseDataRoot.Value.ValueKind);
                
                _logger.Error("🔍 **PARSING_STATE_TRANSITION**: JSON_ELEMENT_PARSING → CORRECTION_EXTRACTION");
                
                // **STATE TRANSITION 2**: JsonElement → CorrectionResult list
                corrections = ExtractCorrectionsFromResponseElement(responseDataRoot.Value, originalDocumentText);
                
                // **PARSING_RESULT**: Check if correction extraction succeeded
                bool correctionExtractionSucceeded = corrections != null && corrections.Count >= 0;
                _logger.Error("🔍 **PARSING_RESULT_2**: Correction extraction succeeded? Expected=TRUE, Actual={Succeeded}, ExtractedCount={Count}", 
                    correctionExtractionSucceeded, corrections?.Count ?? 0);
                
                _logger.Error("🔍 **PARSING_FINAL_STATE**: Extracted {CorrectionCount} corrections from DeepSeek response", corrections.Count);
                
                // **EXPECTATION**: Should have found at least 1 correction for TotalInsurance
                bool foundExpectedCorrections = corrections.Any(c => c.FieldName == "TotalInsurance" || c.FieldName == "TotalDeduction");
                _logger.Error("🔍 **PARSING_EXPECTATION_2**: Found TotalInsurance/TotalDeduction corrections? Expected=TRUE, Actual={Found}", foundExpectedCorrections);
                
                if (!foundExpectedCorrections)
                {
                    _logger.Error("❌ **PARSING_ASSERTION_FAILED_3**: No TotalInsurance/TotalDeduction corrections found - Amazon gift card amount not detected");
                }
                
                // **DETAILED_CORRECTION_LOGGING**: Log each extracted correction with full details
                for (int i = 0; i < corrections.Count; i++)
                {
                    var correction = corrections[i];
                    _logger.Error("🔍 **PARSING_CORRECTION_{Index}**: Field={FieldName}, OldValue='{OldValue}', NewValue='{NewValue}', LineText='{LineText}', Confidence={Confidence}", 
                        i + 1, correction.FieldName, correction.OldValue, correction.NewValue, correction.LineText, correction.Confidence);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **PARSING_CRITICAL_ERROR**: Exception during DeepSeek response processing - potential data corruption. Response snippet: {Snippet}", 
                    this.TruncateForLog(deepSeekResponseJson, 200));
                
                // **ERROR_RECOVERY_LOGGING**: Log the exact point of failure
                _logger.Error("🔍 **PARSING_ERROR_RECOVERY**: Exception type={ExceptionType}, Message='{Message}'", ex.GetType().Name, ex.Message);
            }
            
            _logger.Error("🔍 **PARSING_EXIT_STATE**: ProcessDeepSeekCorrectionResponse returning {Count} corrections", corrections.Count);
            return corrections;
        }

        /// <summary>
        /// Parses a raw DeepSeek JSON response string into a JsonElement.
        /// </summary>
        private JsonElement? ParseDeepSeekResponseToElement(string rawResponse)
        {
            // **PARSE_ENTRY_STATE**: Log JSON parsing entry with input parameters
            _logger.Error("🔍 **PARSE_ENTRY_STATE**: ParseDeepSeekResponseToElement called with response length={Length}", rawResponse?.Length ?? 0);
            _logger.Error("🔍 **PARSE_RAW_INPUT**: Raw response content: {RawResponse}", rawResponse);
            
            try
            {
                // **STATE TRANSITION**: Raw response → Cleaned JSON
                _logger.Error("🔍 **PARSE_STATE_TRANSITION**: RAW_RESPONSE → JSON_CLEANING");
                
                var cleanJson = this.CleanJsonResponse(rawResponse);
                
                // **CLEANING_RESULT**: Check if cleaning succeeded and what changed
                bool cleaningSucceeded = !string.IsNullOrEmpty(cleanJson);
                bool contentChanged = rawResponse != cleanJson;
                _logger.Error("🔍 **PARSE_CLEANING_RESULT**: Cleaning succeeded? {Succeeded}, Content changed? {Changed}, CleanedLength={CleanedLength}", 
                    cleaningSucceeded, contentChanged, cleanJson?.Length ?? 0);
                
                if (contentChanged)
                {
                    _logger.Error("⚠️ **PARSE_CONTENT_MODIFIED**: JSON cleaning modified content - potential data loss");
                    _logger.Error("🔍 **PARSE_CLEANED_CONTENT**: Cleaned JSON: {CleanedJson}", cleanJson);
                }
                
                if (string.IsNullOrEmpty(cleanJson))
                {
                    _logger.Error("❌ **PARSE_ASSERTION_FAILED**: CleanJsonResponse returned empty - data completely lost during cleaning");
                    return null;
                }
                
                // **STATE TRANSITION**: Cleaned JSON → JsonDocument
                _logger.Error("🔍 **PARSE_STATE_TRANSITION**: JSON_CLEANING → JSON_DOCUMENT_PARSING");
                
                using var document = JsonDocument.Parse(cleanJson);
                var rootElement = document.RootElement.Clone();
                
                // **PARSING_SUCCESS**: Log successful parsing with structure details
                _logger.Error("✅ **PARSE_SUCCESS**: JSON parsed successfully to {ElementType}", rootElement.ValueKind);
                
                // **STRUCTURE_VERIFICATION**: Check if required 'errors' property exists
                if (rootElement.ValueKind == JsonValueKind.Object && rootElement.TryGetProperty("errors", out var errorsProperty))
                {
                    _logger.Error("🔍 **PARSE_STRUCTURE_VALID**: Found 'errors' property of type {ErrorsType} with {ArrayLength} elements", 
                        errorsProperty.ValueKind, 
                        errorsProperty.ValueKind == JsonValueKind.Array ? errorsProperty.GetArrayLength() : -1);
                }
                else
                {
                    _logger.Error("❌ **PARSE_STRUCTURE_INVALID**: Parsed JSON missing 'errors' property - unexpected DeepSeek response format");
                    _logger.Error("🔍 **PARSE_AVAILABLE_PROPERTIES**: Available properties: {Properties}", 
                        rootElement.ValueKind == JsonValueKind.Object ? 
                            string.Join(", ", rootElement.EnumerateObject().Select(p => p.Name)) : "N/A");
                }
                
                return rootElement;
            }
            catch (JsonException jsonEx)
            {
                _logger.Error(jsonEx, "🚨 **PARSE_JSON_EXCEPTION**: Failed to parse cleaned response as JSON - data corruption detected");
                _logger.Error("🔍 **PARSE_ERROR_DETAILS**: Exception message='{Message}', Path='{Path}', Position={Position}", 
                    jsonEx.Message, jsonEx.Path, jsonEx.BytePositionInLine);
                
                // **ERROR_CONTEXT**: Show the problematic cleaned JSON that failed to parse
                var cleanedForError = this.CleanJsonResponse(rawResponse);
                _logger.Error("🔍 **PARSE_ERROR_CONTEXT**: Cleaned JSON that failed to parse: {FailedJson}", cleanedForError);
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **PARSE_CRITICAL_EXCEPTION**: Unexpected exception during JSON parsing");
                return null;
            }
        }

        /// <summary>
        /// Extracts CorrectionResult objects from a parsed JsonElement, trying various common structures.
        /// </summary>
        private List<CorrectionResult> ExtractCorrectionsFromResponseElement(JsonElement responseDataRoot, string originalDocumentText)
        {
            var corrections = new List<CorrectionResult>();
            int errorIndexCounter = 0;

            // **EXTRACTION_ENTRY_STATE**: Log extraction entry with input parameters
            _logger.Error("🔍 **EXTRACTION_ENTRY_STATE**: ExtractCorrectionsFromResponseElement called with rootType={ValueKind}", responseDataRoot.ValueKind);
            _logger.Error("🔍 **EXTRACTION_INPUT_PARAMS**: OriginalText length={TextLength}", originalDocumentText?.Length ?? 0);
            
            // **STRUCTURE_ANALYSIS**: Analyze the JSON structure to understand DeepSeek response format
            _logger.Error("🔍 **EXTRACTION_STRUCTURE_ANALYSIS**: JSON root element type={ValueKind}", responseDataRoot.ValueKind);
            
            // **EXPECTATION**: Root should be Object containing 'errors' array
            bool isExpectedObjectType = responseDataRoot.ValueKind == JsonValueKind.Object;
            _logger.Error("🔍 **EXTRACTION_EXPECTATION_1**: Root is Object type? Expected=TRUE, Actual={IsObject}", isExpectedObjectType);
            
            if (!isExpectedObjectType)
            {
                _logger.Error("❌ **EXTRACTION_ASSERTION_FAILED_1**: Root is not Object type - cannot extract 'errors' property");
                return corrections;
            }

            // **STATE TRANSITION**: Object root → 'errors' property extraction
            if (responseDataRoot.ValueKind == JsonValueKind.Object)
            {
                _logger.Error("🔍 **EXTRACTION_STATE_TRANSITION**: OBJECT_ROOT → ERRORS_PROPERTY_EXTRACTION");
                
                // **PROPERTY_SEARCH**: Look for 'errors' property
                bool hasErrorsProperty = responseDataRoot.TryGetProperty("errors", out var errorsArray);
                _logger.Error("🔍 **EXTRACTION_PROPERTY_SEARCH**: 'errors' property found? Expected=TRUE, Actual={Found}", hasErrorsProperty);
                
                if (!hasErrorsProperty)
                {
                    _logger.Error("❌ **EXTRACTION_ASSERTION_FAILED_2**: 'errors' property not found in JSON object");
                    
                    // **DIAGNOSTIC_INFO**: Show available properties for debugging
                    var availableProperties = responseDataRoot.EnumerateObject().Select(p => p.Name).ToList();
                    _logger.Error("🔍 **EXTRACTION_AVAILABLE_PROPERTIES**: Available properties: {Properties}", string.Join(", ", availableProperties));
                    return corrections;
                }
                
                // **ARRAY_VALIDATION**: Check if 'errors' property is array type
                bool errorsIsArray = errorsArray.ValueKind == JsonValueKind.Array;
                _logger.Error("🔍 **EXTRACTION_ARRAY_VALIDATION**: 'errors' is Array type? Expected=TRUE, Actual={IsArray}, ActualType={ActualType}", 
                    errorsIsArray, errorsArray.ValueKind);
                
                if (errorsArray.ValueKind == JsonValueKind.Array)
                {
                    int arrayLength = errorsArray.GetArrayLength();
                    _logger.Error("🔍 **EXTRACTION_ARRAY_INFO**: Processing 'errors' array with {Count} elements", arrayLength);
                    
                    // **EXPECTATION**: Should have at least 1 error for TotalInsurance
                    bool hasExpectedErrors = arrayLength > 0;
                    _logger.Error("🔍 **EXTRACTION_EXPECTATION_2**: Array has errors? Expected=TRUE, Actual={HasErrors}, Count={Count}", hasExpectedErrors, arrayLength);
                    
                    if (!hasExpectedErrors)
                    {
                        _logger.Error("❌ **EXTRACTION_ASSERTION_FAILED_3**: 'errors' array is empty - DeepSeek found no missing fields (should find TotalInsurance)");
                        return corrections;
                    }
                    
                    // **STATE TRANSITION**: Array processing → Element extraction
                    _logger.Error("🔍 **EXTRACTION_STATE_TRANSITION**: ERRORS_ARRAY → ELEMENT_PROCESSING");
                    
                    foreach (var element in errorsArray.EnumerateArray())
                    {
                        errorIndexCounter++;
                        
                        // **ELEMENT_PROCESSING**: Log each error element processing
                        _logger.Error("🔍 **EXTRACTION_ELEMENT_{Index}_START**: Processing error element {Index} of type {ElementType}", 
                            errorIndexCounter, errorIndexCounter, element.ValueKind);
                        var cr = CreateCorrectionFromElement(element, originalDocumentText, errorIndexCounter);
                        if (cr != null) corrections.Add(cr);
                    }
                }
                else if (responseDataRoot.TryGetProperty("corrections", out var correctionsArray) && correctionsArray.ValueKind == JsonValueKind.Array)
            {
                _logger.Debug("Found 'corrections' array in DeepSeek response. Processing {Count} elements.", correctionsArray.GetArrayLength());
                 foreach (var element in correctionsArray.EnumerateArray())
                {
                    errorIndexCounter++;
                    var cr = CreateCorrectionFromElement(element, originalDocumentText, errorIndexCounter);
                    if (cr != null) corrections.Add(cr);
                }
            }
            else if (responseDataRoot.TryGetProperty("fields", out var fieldsObject) && fieldsObject.ValueKind == JsonValueKind.Object)
            {
                 _logger.Debug("Found 'fields' object in DeepSeek response. Processing properties.");
                foreach (var property in fieldsObject.EnumerateObject())
                {
                    errorIndexCounter++;
                    var cr = CreateCorrectionFromNamedFieldProperty(property, originalDocumentText, errorIndexCounter);
                    if (cr != null) corrections.Add(cr);
                }
            }
            else if (responseDataRoot.ValueKind == JsonValueKind.Object) // Fallback: try to parse root object properties directly
            {
                _logger.Debug("No 'errors'/'corrections'/'fields' found. Attempting to parse root object properties directly.");
                 foreach (var property in responseDataRoot.EnumerateObject())
                {
                    errorIndexCounter++;
                    var cr = CreateCorrectionFromNamedFieldProperty(property, originalDocumentText, errorIndexCounter);
                    if (cr != null) corrections.Add(cr);
                }
            }
            }
            else if (responseDataRoot.ValueKind == JsonValueKind.Array)
            {
                // Handle case where root element is directly an Array of corrections
                _logger.Debug("Root element is an Array. Processing {Count} elements directly.", responseDataRoot.GetArrayLength());
                foreach (var element in responseDataRoot.EnumerateArray())
                {
                    errorIndexCounter++;
                    var cr = CreateCorrectionFromElement(element, originalDocumentText, errorIndexCounter);
                    if (cr != null) corrections.Add(cr);
                }
            }
            else
            {
                _logger.Warning("DeepSeek response JSON structure not recognized for correction extraction. Root type: {RootType}", responseDataRoot.ValueKind);
            }
            
            // Final validation and enrichment pass on all collected corrections
            return ValidateAndEnrichParsedCorrections(corrections, originalDocumentText);
        }

        /// <summary>
        /// Creates a CorrectionResult from a JsonElement representing a single error/correction object.
        /// </summary>
        private CorrectionResult CreateCorrectionFromElement(JsonElement element, string originalText, int itemIndex)
        {
            // **CORRECTION_CREATION_ENTRY**: Log correction creation entry state
            _logger.Error("🔍 **CORRECTION_CREATION_ENTRY_{Index}**: Creating CorrectionResult from element {Index} of type {ElementType}", itemIndex, itemIndex, element.ValueKind);
            
            // **ELEMENT_STRUCTURE_LOGGING**: Log the complete element structure for debugging
            if (element.ValueKind == JsonValueKind.Object)
            {
                var properties = element.EnumerateObject().Select(p => $"{p.Name}={p.Value}").ToList();
                _logger.Error("🔍 **CORRECTION_ELEMENT_{Index}_STRUCTURE**: Element properties: {Properties}", itemIndex, string.Join(", ", properties));
            }
            
            // **FIELD_EXTRACTION**: Extract critical fields with validation
            _logger.Error("🔍 **CORRECTION_FIELD_EXTRACTION_{Index}**: Extracting 'field' and 'correct_value' from element", itemIndex);
            
            var fieldName = this.GetStringValueWithLogging(element, "field", itemIndex);
            var newValue = this.GetStringValueWithLogging(element, "correct_value", itemIndex);
            
            // **CRITICAL_FIELD_VALIDATION**: Validate required fields
            bool hasRequiredFields = !string.IsNullOrEmpty(fieldName) && newValue != null;
            _logger.Error("🔍 **CORRECTION_VALIDATION_{Index}**: Required fields present? Expected=TRUE, Actual={HasFields}, FieldName='{FieldName}', NewValue='{NewValue}'", 
                itemIndex, hasRequiredFields, fieldName, newValue);
            
            // **CARIBBEAN_CUSTOMS_CHECK**: Check if this is the expected TotalInsurance/TotalDeduction correction
            bool isExpectedField = fieldName == "TotalInsurance" || fieldName == "TotalDeduction";
            bool isExpectedGiftCardValue = newValue == "-6.99" || newValue == "6.99";
            _logger.Error("🔍 **CORRECTION_CARIBBEAN_CHECK_{Index}**: Is expected field? {IsExpectedField} | Is gift card value? {IsGiftCardValue}", 
                itemIndex, isExpectedField, isExpectedGiftCardValue);
            
            if (isExpectedField && isExpectedGiftCardValue)
            {
                _logger.Error("✅ **CORRECTION_SUCCESS_{Index}**: Found expected Caribbean customs correction: {FieldName} = {Value}", itemIndex, fieldName, newValue);
            }
            else if (isExpectedField)
            {
                _logger.Error("⚠️ **CORRECTION_PARTIAL_{Index}**: Found expected field {FieldName} but value {Value} may not be gift card amount", itemIndex, fieldName, newValue);
            }

            if (string.IsNullOrEmpty(fieldName) || newValue == null)
            {
                _logger.Error("❌ **CORRECTION_ASSERTION_FAILED_{Index}**: 'field' or 'correct_value' is missing - skipping correction", itemIndex);
                return null;
            }
            
            // **OPTIONAL_FIELD_EXTRACTION**: Extract optional fields with detailed logging
            _logger.Error("🔍 **CORRECTION_OPTIONAL_EXTRACTION_{Index}**: Extracting optional fields", itemIndex);

            var oldValue = this.GetStringValueWithLogging(element, "extracted_value", itemIndex, isOptional: true);
            var lineText = this.GetStringValueWithLogging(element, "line_text", itemIndex, isOptional: true);
            var lineNumber = this.GetIntValueWithLogging(element, "line_number", itemIndex, 0, isOptional: true);
            var confidence = this.GetDoubleValueWithLogging(element, "confidence", itemIndex, 0.8, isOptional: true);
            var reasoning = this.GetStringValueWithLogging(element, "reasoning", itemIndex, isOptional: true);
            var errorType = this.GetStringValueWithLogging(element, "error_type", itemIndex, isOptional: true);
            var requiresMultiline = this.GetBooleanValueWithLogging(element, "requires_multiline_regex", itemIndex, false, isOptional: true);
            
            // **FIELD_VALIDATION_LOGGING**: Log extracted field values for verification
            _logger.Error("🔍 **CORRECTION_EXTRACTED_FIELDS_{Index}**: OldValue='{OldValue}', LineText='{LineText}', LineNumber={LineNumber}, Confidence={Confidence}, ErrorType='{ErrorType}'", 
                itemIndex, oldValue, lineText, lineNumber, confidence, errorType);

            var contextBefore = this.ParseContextLinesArray(element, "context_lines_before", itemIndex);
            var contextAfter = this.ParseContextLinesArray(element, "context_lines_after", itemIndex);
            
            // **CONTEXT_LOGGING**: Log context lines for verification
            _logger.Error("🔍 **CORRECTION_CONTEXT_{Index}**: ContextBefore={BeforeCount} lines, ContextAfter={AfterCount} lines", 
                itemIndex, contextBefore?.Count ?? 0, contextAfter?.Count ?? 0);
            
            // If oldValue is not provided by DeepSeek and it's not an omission, try to find it.
            if (string.IsNullOrEmpty(oldValue) && errorType != "omission" && DetermineCorrectionType(null, newValue, errorType) != "omission")
            {
                oldValue = FindOriginalValueInText(fieldName, originalText);
            }
            
            return new CorrectionResult
            {
                FieldName = fieldName, // Raw from DeepSeek, mapping happens later
                OldValue = oldValue,
                NewValue = newValue,
                LineText = lineText,
                LineNumber = lineNumber,
                ContextLinesBefore = contextBefore,
                ContextLinesAfter = contextAfter,
                RequiresMultilineRegex = requiresMultiline,
                Success = true, // Assume valid structure from DeepSeek means potential success
                Confidence = confidence,
                CorrectionType = DetermineCorrectionType(oldValue, newValue, errorType),
                Reasoning = reasoning
            };
        }

        /// <summary>
        /// Creates a CorrectionResult from a JsonProperty where property name is field name.
        /// Handles cases where DeepSeek returns { "FieldName": "CorrectedValue" } or { "FieldName": { "corrected": "val" } }
        /// </summary>
        private CorrectionResult CreateCorrectionFromNamedFieldProperty(JsonProperty fieldProperty, string originalText, int itemIndex)
        {
            string fieldName = fieldProperty.Name;
            string newValue = null;
            string oldValue = null; // Not typically available in this simpler structure
            double confidence = 0.75; // Default for direct field value
            string errorType = "value_correction"; // Assume value correction

            if (fieldProperty.Value.ValueKind == JsonValueKind.String)
            {
                newValue = fieldProperty.Value.GetString();
            }
            else if (fieldProperty.Value.ValueKind == JsonValueKind.Object)
            {
                // Check for nested structure like { "corrected": "value", "original": "old", "confidence": 0.9 }
                var fieldObject = fieldProperty.Value;
                newValue = this.GetStringValueWithLogging(fieldObject, "corrected", itemIndex, isOptional: true) ??
                           this.GetStringValueWithLogging(fieldObject, "correct_value", itemIndex, isOptional: true);
                oldValue = this.GetStringValueWithLogging(fieldObject, "original", itemIndex, isOptional: true) ??
                           this.GetStringValueWithLogging(fieldObject, "extracted_value", itemIndex, isOptional: true);
                confidence = this.GetDoubleValueWithLogging(fieldObject, "confidence", itemIndex, confidence, isOptional: true);
                errorType = this.GetStringValueWithLogging(fieldObject, "error_type", itemIndex, isOptional: true) ?? errorType;
            }

            if (newValue == null) {
                 _logger.Verbose("Skipping field property {FieldName} in item {ItemIndex} as no correctable value found.", fieldName, itemIndex);
                return null;
            }

            if (string.IsNullOrEmpty(oldValue)) oldValue = FindOriginalValueInText(fieldName, originalText);
            
            return new CorrectionResult
            {
                FieldName = fieldName, OldValue = oldValue, NewValue = newValue,
                Success = true, Confidence = confidence, CorrectionType = DetermineCorrectionType(oldValue, newValue, errorType),
                LineNumber = FindLineNumberInTextByFieldName(fieldName, originalText) // Best guess
            };
        }

        #endregion

        #region DeepSeek Regex Creation API Call & Parsing
        
        /// <summary>
        /// Requests a new regex pattern from DeepSeek, typically for handling omissions.
        /// </summary>
        /// <param name="correction">The CorrectionResult detailing the omission.</param>
        /// <param name="lineContext">The LineContext providing surrounding information.</param>
        /// <returns>A RegexCreationResponse from DeepSeek, or null on failure.</returns>
        public async Task<RegexCreationResponse> RequestNewRegexFromDeepSeek(CorrectionResult correction, LineContext lineContext)
        {
            if (correction == null || lineContext == null)
            {
                _logger.Error("RequestNewRegexFromDeepSeek: Correction or LineContext is null.");
                return null;
            }
            try
            {
                // Prompt creation is delegated to OCRPromptCreation.cs
                var prompt = this.CreateRegexCreationPrompt(correction, lineContext); 
                _logger.Debug("Requesting new regex from DeepSeek for field {FieldName}. Prompt snippet: {PromptSnippet}", 
                    correction.FieldName, this.TruncateForLog(prompt, 300));
                
                var responseJson = await this._deepSeekApi.GetCompletionAsync(prompt, this.DefaultTemperature, this.DefaultMaxTokens).ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(responseJson))
                {
                    _logger.Warning("Received empty or whitespace response from DeepSeek for regex creation of field {FieldName}.", correction.FieldName);
                    return null;
                }
                _logger.Debug("DeepSeek response for regex creation ({FieldName}): {ResponseSnippet}", 
                    correction.FieldName, this.TruncateForLog(responseJson, 300));
                
                return ParseRegexCreationResponseJson(responseJson); // Uses helper in this file
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error requesting new regex from DeepSeek for field {FieldName}", correction.FieldName);
                return null;
            }
        }
        
        /// <summary>
        /// Parses the JSON response from DeepSeek (for regex creation) into a RegexCreationResponse object.
        /// </summary>
        private RegexCreationResponse ParseRegexCreationResponseJson(string responseJson)
        {
            try
            {
                var cleanJson = this.CleanJsonResponse(responseJson); // From OCRUtilities.cs
                if (string.IsNullOrWhiteSpace(cleanJson))
                {
                    _logger.Warning("ParseRegexCreationResponseJson: Cleaned JSON is empty. Cannot parse.");
                    return null;
                }

                using var doc = JsonDocument.Parse(cleanJson);
                var root = doc.RootElement;
                int dummyErrorIndex = -1; // For logging helpers, not a list of errors here

                return new RegexCreationResponse
                {
                    Strategy = this.GetStringValueWithLogging(root, "strategy", dummyErrorIndex) ?? "create_new_line",
                    RegexPattern = this.GetStringValueWithLogging(root, "regex_pattern", dummyErrorIndex) ?? "",
                    CompleteLineRegex = this.GetStringValueWithLogging(root, "complete_line_regex", dummyErrorIndex, isOptional: true) ?? "", // Optional
                    IsMultiline = this.GetBooleanValueWithLogging(root, "is_multiline", dummyErrorIndex, false),
                    MaxLines = this.GetIntValueWithLogging(root, "max_lines", dummyErrorIndex, 1), // Default 1 line
                    TestMatch = this.GetStringValueWithLogging(root, "test_match", dummyErrorIndex, isOptional: true) ?? "",
                    Confidence = this.GetDoubleValueWithLogging(root, "confidence", dummyErrorIndex, 0.75), // Default confidence
                    Reasoning = this.GetStringValueWithLogging(root, "reasoning", dummyErrorIndex, isOptional: true) ?? "",
                    PreservesExistingGroups = this.GetBooleanValueWithLogging(root, "preserves_existing_groups", dummyErrorIndex, true), // Default true
                    ContextLinesUsed = this.GetStringValueWithLogging(root, "context_lines_used", dummyErrorIndex, isOptional: true) ?? "" // For test compatibility
                };
            }
            catch (JsonException jsonEx)
            {
                _logger.Error(jsonEx, "Error parsing DeepSeek regex creation JSON response. Cleaned snippet: {Snippet}", this.TruncateForLog(this.CleanJsonResponse(responseJson), 200));
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Generic error parsing regex creation response.");
                return null;
            }
        }

        #endregion

        #region Internal Helper Methods for Parsing & Contextualization
        
        /// <summary>
        /// Finds the original value of a field in the provided text using known patterns.
        /// This is a simplified local lookup.
        /// </summary>
        private string FindOriginalValueInText(string fieldName, string text)
        {
            if (string.IsNullOrEmpty(fieldName) || string.IsNullOrEmpty(text)) return null;

            var mappedName = this.MapDeepSeekFieldToDatabase(fieldName)?.DatabaseFieldName ?? fieldName; // From OCRFieldMapping.cs
            // CreateFieldExtractionPatterns is from OCRPatternCreation.cs
            var patterns = this.CreateFieldExtractionPatterns(mappedName, Enumerable.Empty<string>()); 

            foreach (var patternString in patterns)
            {
                try
                {
                    var match = Regex.Match(text, patternString, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    if (match.Success)
                    {
                        // Prefer named group if it matches the mappedName, otherwise first capturing group.
                        if (match.Groups[mappedName].Success) return match.Groups[mappedName].Value.Trim();
                        if (match.Groups.Count > 1 && match.Groups[1].Success) return match.Groups[1].Value.Trim();
                    }
                }
                catch (ArgumentException regexEx) {
                     _logger.Verbose("Regex pattern error during FindOriginalValueInText for field '{FieldName}', pattern '{Pattern}': {Message}", fieldName, patternString, regexEx.Message);
                }
            }
            return null; 
        }

        /// <summary>
        /// Finds the line number in text where a field name or its typical pattern might appear.
        /// </summary>
        private int FindLineNumberInTextByFieldName(string fieldName, string text)
        {
            if (string.IsNullOrEmpty(fieldName) || string.IsNullOrEmpty(text)) return 0;

            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.None);
            var mappedName = this.MapDeepSeekFieldToDatabase(fieldName)?.DisplayName ?? fieldName; // Prefer DisplayName for text search
            var patterns = this.CreateFieldExtractionPatterns(mappedName, Enumerable.Empty<string>()); // From OCRPatternCreation

            for (int i = 0; i < lines.Length; i++)
            {
                // First, check for original field name as a keyword on the line
                if (lines[i].IndexOf(fieldName, StringComparison.OrdinalIgnoreCase) >= 0) return i + 1;

                // Then, check for mapped field name if different from original
                if (!string.Equals(fieldName, mappedName, StringComparison.OrdinalIgnoreCase) &&
                    lines[i].IndexOf(mappedName, StringComparison.OrdinalIgnoreCase) >= 0) return i + 1;

                // Finally, check regex patterns
                foreach (var patternString in patterns)
                {
                    try
                    {
                        if (Regex.IsMatch(lines[i], patternString, RegexOptions.IgnoreCase)) return i + 1;
                    }
                    catch (ArgumentException regexEx) {
                         _logger.Verbose("Regex pattern error during FindLineNumberInTextByFieldName for field '{FieldName}', pattern '{Pattern}': {Message}", fieldName, patternString, regexEx.Message);
                    }
                }
            }
            return 0; // Not found
        }
        
        /// <summary>
        /// Finds the line number in text that exactly matches the given lineText.
        /// </summary>
        private int FindLineNumberInTextByExactLine(string lineToFind, string text)
        {
            if (string.IsNullOrEmpty(lineToFind) || string.IsNullOrEmpty(text)) return 0;
            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim().Equals(lineToFind.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return i + 1; // 1-based
                }
            }
            return 0;
        }

        /// <summary>
        /// Final validation pass on a list of parsed CorrectionResult objects.
        /// Enriches them with line numbers and context if missing and possible.
        /// </summary>
        private List<CorrectionResult> ValidateAndEnrichParsedCorrections(List<CorrectionResult> corrections, string originalText)
        {
            var validatedAndEnriched = new List<CorrectionResult>();
            foreach (var cr in corrections)
            {
                // Map field name now, so subsequent logic uses canonical names
                var mappedInfo = this.MapDeepSeekFieldToDatabase(cr.FieldName); // From OCRFieldMapping.cs
                var canonicalFieldName = mappedInfo?.DatabaseFieldName ?? cr.FieldName;

                if (!this.IsFieldSupported(canonicalFieldName)) // From OCRFieldMapping.cs
                {
                    _logger.Warning("Unsupported field name '{CanonicalFieldName}' (from DeepSeek '{RawField}') in correction. Skipping.", canonicalFieldName, cr.FieldName);
                    continue;
                }
                cr.FieldName = canonicalFieldName; // Update to canonical name

                // Enrich LineNumber if missing
                if (cr.LineNumber <= 0)
                {
                    if (!string.IsNullOrEmpty(cr.LineText)) 
                        cr.LineNumber = FindLineNumberInTextByExactLine(cr.LineText, originalText);
                    if (cr.LineNumber <= 0) // Fallback
                        cr.LineNumber = FindLineNumberInTextByFieldName(cr.FieldName, originalText);
                }
                
                // Enrich LineText if missing but LineNumber is known
                if (string.IsNullOrEmpty(cr.LineText) && cr.LineNumber > 0)
                {
                    cr.LineText = this.GetOriginalLineText(originalText, cr.LineNumber); // From OCRUtilities.cs
                }

                // Enrich ContextLines if missing and LineNumber/LineText are known
                if (cr.LineNumber > 0 && (!cr.ContextLinesBefore.Any() || !cr.ContextLinesAfter.Any()))
                {
                    EnrichContextLinesFromText(cr, originalText); // Uses helper in this file
                }
                validatedAndEnriched.Add(cr);
            }
            return validatedAndEnriched;
        }

        /// <summary>
        /// Populates ContextLinesBefore and ContextLinesAfter on a CorrectionResult
        /// if they are empty, using the LineNumber and original document text.
        /// </summary>
        private void EnrichContextLinesFromText(CorrectionResult correction, string originalText)
        {
            if (correction.LineNumber <= 0 || string.IsNullOrEmpty(originalText) || 
                (correction.ContextLinesBefore.Any() && correction.ContextLinesAfter.Any())) // Don't overwrite if already populated
            {
                return;
            }

            var lines = originalText.Split(new[] { '\r', '\n' }, StringSplitOptions.None);
            var targetLineIndex = correction.LineNumber - 1; // 0-based
            int contextWindowSize = 5;

            if (targetLineIndex >= 0 && targetLineIndex < lines.Length)
            {
                if (!correction.ContextLinesBefore.Any())
                {
                    for (int i = Math.Max(0, targetLineIndex - contextWindowSize); i < targetLineIndex; i++)
                    {
                        correction.ContextLinesBefore.Add(lines[i]);
                    }
                }
                if (!correction.ContextLinesAfter.Any())
                {
                     for (int i = targetLineIndex + 1; i <= Math.Min(lines.Length - 1, targetLineIndex + contextWindowSize); i++)
                    {
                        correction.ContextLinesAfter.Add(lines[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Determines the type of correction (omission, format, value) based on old/new values and optional DeepSeek type.
        /// </summary>
        private string DetermineCorrectionType(string oldValue, string newValue, string errorTypeFromDeepSeek = null)
        {
            // Prefer explicit type from DeepSeek if reliable
            if (!string.IsNullOrEmpty(errorTypeFromDeepSeek)) return errorTypeFromDeepSeek;

            if (string.IsNullOrEmpty(oldValue) && newValue != null) return "omission"; // NewValue can be ""
            if (oldValue != null && newValue == null) return "removal"; // Explicit removal (newValue is null)
            if (oldValue != null && newValue == string.Empty) return "cleared"; // Value explicitly set to empty string

            if (oldValue != null && newValue != null) {
                var oldNormalized = Regex.Replace(oldValue, @"[\s\$,€£\-()]", "");
                var newNormalized = Regex.Replace(newValue, @"[\s\$,€£\-()]", "");
                if (string.Equals(oldNormalized, newNormalized, StringComparison.OrdinalIgnoreCase) && oldValue != newValue)
                {
                    return "format_error";
                }
                return "value_correction";
            }
            return "unknown"; // Should not happen if oldValue/newValue logic is sound
        }

        #endregion
    }
}