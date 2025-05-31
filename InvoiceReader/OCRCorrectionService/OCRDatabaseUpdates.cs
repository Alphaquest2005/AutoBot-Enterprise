using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using OCR.Business.Entities;
using Serilog;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Main database update methods for OCR correction service
    /// Enhanced with omission handling support
    /// </summary>
    public partial class OCRCorrectionService
    {
        #region Private Fields

        private DatabaseUpdateStrategyFactory _strategyFactory;

        #endregion

        #region Main Database Update Methods

        /// <summary>
        /// Updates regex patterns in database based on correction results
        /// Enhanced to handle both format corrections and omissions
        /// </summary>
        /// <param name="corrections">List of corrections to process</param>
        /// <param name="fileText">Original file text for context</param>
        /// <param name="filePath">Path to the file being processed</param>
        /// <param name="invoiceMetadata">Enhanced metadata with line context</param>
        /// <returns>Task representing the async operation</returns>
        public async Task UpdateRegexPatternsAsync(IEnumerable<CorrectionResult> corrections, string fileText, 
            string filePath = null, Dictionary<string, OCRFieldMetadata> invoiceMetadata = null)
        {
            if (corrections == null || !corrections.Any())
            {
                _logger?.Information("No corrections provided for regex pattern updates");
                return;
            }

            _logger?.Information("Starting enhanced regex pattern updates for {CorrectionCount} corrections", corrections.Count());

            // Initialize strategy factory if not already done
            _strategyFactory ??= new DatabaseUpdateStrategyFactory(_logger, this);

            var successCount = 0;
            var failureCount = 0;
            var omissionCount = 0;
            var formatCount = 0;

            using var context = new OCRContext();

            foreach (var correction in corrections.Where(c => c.Success))
            {
                try
                {
                    var request = CreateEnhancedUpdateRequest(correction, fileText, filePath, invoiceMetadata);
                    var result = await ProcessSingleCorrectionAsync(context, request);

                    if (result.IsSuccess)
                    {
                        successCount++;
                        if (correction.CorrectionType == "omission")
                            omissionCount++;
                        else
                            formatCount++;
                        
                        _logger?.Debug("Successfully processed correction for field {FieldName}: {Operation}",
                            correction.FieldName, result.Operation);
                    }
                    else
                    {
                        failureCount++;
                        _logger?.Warning("Failed to process correction for field {FieldName}: {Message}",
                            correction.FieldName, result.Message);
                    }

                    // Log to learning table regardless of database update success
                    await LogCorrectionLearningAsync(context, request, result);
                }
                catch (Exception ex)
                {
                    failureCount++;
                    _logger?.Error(ex, "Exception processing correction for field {FieldName}", correction.FieldName);
                }
            }

            _logger?.Information("Completed enhanced regex pattern updates: {SuccessCount} successful ({OmissionCount} omissions, {FormatCount} format), {FailureCount} failed",
                successCount, omissionCount, formatCount, failureCount);
        }

        /// <summary>
        /// Processes a single correction and updates the database using enhanced strategy pattern
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="request">Enhanced update request</param>
        /// <returns>Database update result</returns>
        private async Task<DatabaseUpdateResult> ProcessSingleCorrectionAsync(OCRContext context, RegexUpdateRequest request)
        {
            try
            {
                // Validate the request
                var validationResult = ValidateUpdateRequest(request);
                if (!validationResult.IsValid)
                {
                    return DatabaseUpdateResult.Failed($"Validation failed: {validationResult.ErrorMessage}");
                }

                // Create correction result for strategy selection
                var correction = new CorrectionResult
                {
                    FieldName = request.FieldName,
                    OldValue = request.OldValue,
                    NewValue = request.NewValue,
                    CorrectionType = request.CorrectionType,
                    Success = true,
                    Confidence = request.Confidence,
                    LineText = request.LineText,
                    ContextLinesBefore = request.ContextLinesBefore,
                    ContextLinesAfter = request.ContextLinesAfter,
                    RequiresMultilineRegex = request.RequiresMultilineRegex
                };

                // Get appropriate strategy using enhanced factory
                var strategy = _strategyFactory.GetStrategy(correction);

                // Execute the strategy
                var result = await strategy.ExecuteAsync(context, request);

                _logger?.Debug("Strategy {StrategyType} executed for field {FieldName}: {Success}",
                    strategy.StrategyType, request.FieldName, result.IsSuccess);

                return result;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error processing correction for field {FieldName}", request.FieldName);
                return DatabaseUpdateResult.Failed($"Processing error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates an enhanced update request from a correction result with metadata context
        /// </summary>
        /// <param name="correction">Correction result</param>
        /// <param name="fileText">Original file text</param>
        /// <param name="filePath">File path</param>
        /// <param name="invoiceMetadata">Enhanced metadata dictionary</param>
        /// <returns>Enhanced update request</returns>
        private RegexUpdateRequest CreateEnhancedUpdateRequest(CorrectionResult correction, string fileText, 
            string filePath, Dictionary<string, OCRFieldMetadata> invoiceMetadata)
        {
            var request = new RegexUpdateRequest
            {
                FieldName = correction.FieldName,
                OldValue = correction.OldValue,
                NewValue = correction.NewValue,
                LineNumber = correction.LineNumber,
                LineText = correction.LineText,
                WindowText = ExtractWindowText(fileText, correction.LineNumber, 5),
                CorrectionType = correction.CorrectionType,
                Confidence = correction.Confidence,
                DeepSeekReasoning = correction.Reasoning,
                FilePath = filePath,
                InvoiceType = DetermineInvoiceType(filePath),
                ContextLinesBefore = correction.ContextLinesBefore ?? new List<string>(),
                ContextLinesAfter = correction.ContextLinesAfter ?? new List<string>(),
                RequiresMultilineRegex = correction.RequiresMultilineRegex
            };

            // Enhance with metadata if available
            if (invoiceMetadata != null && invoiceMetadata.TryGetValue(correction.FieldName, out var metadata))
            {
                request.LineId = metadata.LineId;
                request.PartId = metadata.PartId;
                
                _logger?.Debug("Enhanced request for field {FieldName} with LineId {LineId} and PartId {PartId}",
                    correction.FieldName, request.LineId, request.PartId);
            }
            else
            {
                _logger?.Debug("No metadata available for field {FieldName}, will use fallback resolution",
                    correction.FieldName);
            }

            return request;
        }

        /// <summary>
        /// Validates an enhanced update request
        /// </summary>
        /// <param name="request">Request to validate</param>
        /// <returns>Validation result</returns>
        private FieldValidationInfo ValidateUpdateRequest(RegexUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FieldName))
            {
                return new FieldValidationInfo { IsValid = false, ErrorMessage = "Field name is required" };
            }

            if (string.IsNullOrWhiteSpace(request.NewValue))
            {
                return new FieldValidationInfo { IsValid = false, ErrorMessage = "New value is required" };
            }

            // For omissions, old value can be empty
            if (request.CorrectionType != "omission" && 
                string.IsNullOrWhiteSpace(request.OldValue) && 
                request.OldValue == request.NewValue)
            {
                return new FieldValidationInfo { IsValid = false, ErrorMessage = "Old and new values are identical for non-omission correction" };
            }

            // Check if field is supported
            if (!IsFieldSupported(request.FieldName))
            {
                return new FieldValidationInfo { IsValid = false, ErrorMessage = $"Field {request.FieldName} is not supported" };
            }

            // Get field-specific validation
            var fieldValidation = GetFieldValidationInfo(request.FieldName);
            if (!fieldValidation.IsValid)
            {
                return fieldValidation;
            }

            // Validate new value format
            if (!string.IsNullOrEmpty(fieldValidation.ValidationPattern))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(request.NewValue, fieldValidation.ValidationPattern))
                {
                    return new FieldValidationInfo
                    {
                        IsValid = false,
                        ErrorMessage = $"New value '{request.NewValue}' does not match expected format for field {request.FieldName}"
                    };
                }
            }

            return new FieldValidationInfo { IsValid = true };
        }

        /// <summary>
        /// Logs correction details to the enhanced learning table
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="request">Enhanced update request</param>
        /// <param name="result">Update result</param>
        /// <returns>Task representing the async operation</returns>
        private async Task LogCorrectionLearningAsync(OCRContext context, RegexUpdateRequest request, DatabaseUpdateResult result)
        {
            try
            {
                var learning = new OCRCorrectionLearning
                {
                    FieldName = request.FieldName,
                    OriginalError = request.OldValue ?? "",
                    CorrectValue = request.NewValue,
                    LineNumber = request.LineNumber,
                    LineText = request.LineText ?? "",
                    WindowText = request.WindowText,
                    CorrectionType = request.CorrectionType,
                    DeepSeekReasoning = request.DeepSeekReasoning,
                    Confidence = (decimal?)request.Confidence,
                    InvoiceType = request.InvoiceType,
                    FilePath = request.FilePath,
                    Success = result.IsSuccess,
                    ErrorMessage = result.IsSuccess ? null : result.Message,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "OCRCorrectionService",
                    
                    // Enhanced fields for omission tracking
                    RequiresMultilineRegex = request.RequiresMultilineRegex,
                    ContextLinesBefore = string.Join("\n", request.ContextLinesBefore),
                    ContextLinesAfter = string.Join("\n", request.ContextLinesAfter),
                    LineId = request.LineId,
                    PartId = request.PartId
                };

                context.OCRCorrectionLearning.Add(learning);
                await context.SaveChangesAsync();

                _logger?.Debug("Logged enhanced correction learning entry for field {FieldName}", request.FieldName);
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Failed to log enhanced correction learning for field {FieldName}", request.FieldName);
                // Don't throw - logging failure shouldn't stop the main process
            }
        }

        /// <summary>
        /// Creates a new regex pattern from DeepSeek for omission corrections
        /// </summary>
        /// <param name="request">Enhanced regex update request</param>
        /// <returns>Regex creation response from DeepSeek</returns>
        public async Task<RegexCreationResponse> RequestNewRegexFromDeepSeekAsync(RegexUpdateRequest request)
        {
            try
            {
                // Get current line regex if LineId is available
                var currentLineRegex = "";
                var existingNamedGroups = new List<string>();

                if (request.LineId.HasValue)
                {
                    using var context = new OCRContext();
                    var line = await context.Lines
                        .Include(l => l.RegularExpressions)
                        .Include(l => l.Fields)
                        .FirstOrDefaultAsync(l => l.Id == request.LineId.Value);

                    if (line?.RegularExpressions != null)
                    {
                        currentLineRegex = line.RegularExpressions.RegEx;
                        existingNamedGroups = ExtractNamedGroupsFromRegex(currentLineRegex);
                    }
                }

                var prompt = CreateRegexCreationPrompt(request, currentLineRegex, existingNamedGroups);
                var response = await _deepSeekApi.GetResponseAsync(prompt);
                
                return ParseRegexCreationResponse(response);
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error requesting new regex from DeepSeek for field {FieldName}", request.FieldName);
                return null;
            }
        }

        /// <summary>
        /// Creates the prompt for DeepSeek regex creation
        /// </summary>
        private string CreateRegexCreationPrompt(RegexUpdateRequest request, string currentLineRegex, List<string> existingNamedGroups)
        {
            var contextLines = string.Join("\n", 
                request.ContextLinesBefore
                    .Concat(new[] { $">>> LINE {request.LineNumber}: {request.LineText} <<<" })
                    .Concat(request.ContextLinesAfter));

            return $@"CREATE REGEX PATTERN FOR OCR FIELD EXTRACTION:

A field '{request.FieldName}' with value '{request.NewValue}' was found but not extracted by current OCR processing.

CURRENT LINE REGEX: {currentLineRegex ?? "None"}
EXISTING NAMED GROUPS: {string.Join(", ", existingNamedGroups)}

TARGET LINE:
{request.LineText}

FULL CONTEXT:
{contextLines}

FIELD DETAILS:
- Field Name: {request.FieldName}
- Expected Value: {request.NewValue}
- Requires Multiline: {request.RequiresMultilineRegex}
- Error Type: {request.CorrectionType}

REQUIREMENTS:
1. Create a regex pattern that extracts the value '{request.NewValue}' using named group (?<{request.FieldName}>pattern)
2. If updating existing regex, ensure you don't break existing named groups: {string.Join(", ", existingNamedGroups)}
3. Pattern should work with the provided context
4. Decide if this should modify existing line or create new line

RESPONSE FORMAT:
{{
  ""strategy"": ""modify_existing_line"" OR ""create_new_line"",
  ""regex_pattern"": ""(?<{request.FieldName}>your_pattern_here)"",
  ""complete_line_regex"": ""full regex if modifying existing line"",
  ""is_multiline"": true/false,
  ""max_lines"": your_determined_number,
  ""test_match"": ""exact text from context that should be matched"",
  ""confidence"": 0.95,
  ""reasoning"": ""why you chose this approach and pattern""
}}

Choose the safest approach that won't break existing extractions.";
        }

        /// <summary>
        /// Parses the DeepSeek response for regex creation
        /// </summary>
        private RegexCreationResponse ParseRegexCreationResponse(string response)
        {
            try
            {
                var cleanJson = CleanJsonResponse(response);
                if (string.IsNullOrWhiteSpace(cleanJson)) return null;

                using var doc = System.Text.Json.JsonDocument.Parse(cleanJson);
                var root = doc.RootElement;

                return new RegexCreationResponse
                {
                    Strategy = GetStringValue(root, "strategy") ?? "create_new_line",
                    RegexPattern = GetStringValue(root, "regex_pattern") ?? "",
                    CompleteLineRegex = GetStringValue(root, "complete_line_regex") ?? "",
                    IsMultiline = root.TryGetProperty("is_multiline", out var multiProp) ? multiProp.GetBoolean() : false,
                    MaxLines = root.TryGetProperty("max_lines", out var maxProp) ? maxProp.GetInt32() : 1,
                    Confidence = GetDoubleValue(root, "confidence"),
                    Reasoning = GetStringValue(root, "reasoning") ?? "",
                    TestMatch = GetStringValue(root, "test_match") ?? ""
                };
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error parsing regex creation response");
                return null;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Extracts a specific line from text
        /// </summary>
        /// <param name="text">Full text</param>
        /// <param name="lineNumber">Line number (1-based)</param>
        /// <returns>Line text or empty string if not found</returns>
        private string ExtractLineText(string text, int lineNumber)
        {
            if (string.IsNullOrEmpty(text) || lineNumber <= 0)
                return "";

            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return lineNumber <= lines.Length ? lines[lineNumber - 1] : "";
        }

        /// <summary>
        /// Extracts a window of text around a specific line
        /// </summary>
        /// <param name="text">Full text</param>
        /// <param name="lineNumber">Center line number (1-based)</param>
        /// <param name="windowSize">Number of lines before and after</param>
        /// <returns>Window text</returns>
        private string ExtractWindowText(string text, int lineNumber, int windowSize)
        {
            if (string.IsNullOrEmpty(text) || lineNumber <= 0)
                return "";

            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var startLine = Math.Max(0, lineNumber - windowSize - 1);
            var endLine = Math.Min(lines.Length - 1, lineNumber + windowSize - 1);

            var windowLines = new List<string>();
            for (int i = startLine; i <= endLine; i++)
            {
                windowLines.Add($"{i + 1}: {lines[i]}");
            }

            return string.Join("\n", windowLines);
        }

        /// <summary>
        /// Determines invoice type from file path
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>Invoice type string</returns>
        private string DetermineInvoiceType(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return "Unknown";

            var fileName = System.IO.Path.GetFileName(filePath).ToLowerInvariant();

            if (fileName.Contains("amazon"))
                return "Amazon";
            if (fileName.Contains("temu"))
                return "Temu";
            if (fileName.Contains("shein"))
                return "Shein";
            if (fileName.Contains("alibaba"))
                return "Alibaba";

            return "Generic";
        }

        /// <summary>
        /// Gets string value from JSON element safely
        /// </summary>
        private string GetStringValue(System.Text.Json.JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && 
                prop.ValueKind != System.Text.Json.JsonValueKind.Null)
                return prop.GetString();
            return null;
        }

        /// <summary>
        /// Gets double value from JSON element safely
        /// </summary>
        private double GetDoubleValue(System.Text.Json.JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && 
                prop.ValueKind != System.Text.Json.JsonValueKind.Null)
            {
                if (prop.TryGetDouble(out var value)) return value;
                if (prop.TryGetDecimal(out var decimalValue)) return (double)decimalValue;
                if (prop.TryGetInt32(out var intValue)) return intValue;
            }
            return 0.0;
        }

        /// <summary>
        /// Cleans JSON response from DeepSeek
        /// </summary>
        private string CleanJsonResponse(string jsonResponse)
        {
            if (string.IsNullOrWhiteSpace(jsonResponse)) return string.Empty;

            var cleaned = System.Text.RegularExpressions.Regex.Replace(jsonResponse, @"```json|```|'''|\uFEFF", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            var startIndex = cleaned.IndexOf('{');
            var endIndex = cleaned.LastIndexOf('}');

            if (startIndex == -1 || endIndex == -1 || startIndex >= endIndex)
            {
                _logger?.Warning("No valid JSON boundaries detected in response");
                return string.Empty;
            }

            return cleaned.Substring(startIndex, endIndex - startIndex + 1);
        }

        #endregion
    }
}