using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OCR.Business.Entities;
using Serilog;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Main database update methods for OCR correction service
    /// </summary>
    public partial class OCRCorrectionService
    {
        #region Private Fields

        private DatabaseUpdateStrategyFactory _strategyFactory;

        #endregion

        #region Main Database Update Methods

        /// <summary>
        /// Updates regex patterns in database based on correction results
        /// </summary>
        /// <param name="corrections">List of corrections to process</param>
        /// <param name="fileText">Original file text for context</param>
        /// <param name="filePath">Path to the file being processed</param>
        /// <returns>Task representing the async operation</returns>
        public async Task UpdateRegexPatternsAsync(IEnumerable<CorrectionResult> corrections, string fileText, string filePath = null)
        {
            if (corrections == null || !corrections.Any())
            {
                _logger?.Information("No corrections provided for regex pattern updates");
                return;
            }

            _logger?.Information("Starting regex pattern updates for {CorrectionCount} corrections", corrections.Count());

            // Initialize strategy factory if not already done
            _strategyFactory ??= new DatabaseUpdateStrategyFactory(_logger);

            var successCount = 0;
            var failureCount = 0;

            using var context = new OCRContext();

            foreach (var correction in corrections.Where(c => c.Success))
            {
                try
                {
                    var request = CreateUpdateRequest(correction, fileText, filePath);
                    var result = await ProcessSingleCorrectionAsync(context, request);

                    if (result.IsSuccess)
                    {
                        successCount++;
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

            _logger?.Information("Completed regex pattern updates: {SuccessCount} successful, {FailureCount} failed",
                successCount, failureCount);
        }

        /// <summary>
        /// Processes a single correction and updates the database
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="request">Update request</param>
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

                // Get appropriate strategy
                var correction = new CorrectionResult
                {
                    FieldName = request.FieldName,
                    OldValue = request.OldValue,
                    NewValue = request.NewValue,
                    CorrectionType = request.CorrectionType,
                    Success = true,
                    Confidence = request.Confidence
                };

                var strategy = _strategyFactory.GetStrategy(correction);

                // Execute the strategy
                var result = await strategy.ExecuteAsync(context, request);

                _logger?.Debug("Strategy {StrategyType} executed for field {FieldName}: {Success}",
                    strategy.StrategyType, request.FieldName, result.Success);

                return result;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error processing correction for field {FieldName}", request.FieldName);
                return DatabaseUpdateResult.Failed($"Processing error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates an update request from a correction result
        /// </summary>
        /// <param name="correction">Correction result</param>
        /// <param name="fileText">Original file text</param>
        /// <param name="filePath">File path</param>
        /// <returns>Update request</returns>
        private RegexUpdateRequest CreateUpdateRequest(CorrectionResult correction, string fileText, string filePath)
        {
            return new RegexUpdateRequest
            {
                FieldName = correction.FieldName,
                OldValue = correction.OldValue,
                NewValue = correction.NewValue,
                LineNumber = correction.LineNumber,
                LineText = ExtractLineText(fileText, correction.LineNumber),
                WindowText = ExtractWindowText(fileText, correction.LineNumber, 5),
                CorrectionType = correction.CorrectionType,
                Confidence = correction.Confidence,
                DeepSeekReasoning = correction.Reasoning,
                FilePath = filePath,
                InvoiceType = DetermineInvoiceType(filePath)
            };
        }

        /// <summary>
        /// Validates an update request
        /// </summary>
        /// <param name="request">Request to validate</param>
        /// <returns>Validation result</returns>
        private FieldValidationInfo ValidateUpdateRequest(RegexUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FieldName))
            {
                return new FieldValidationInfo { IsValid = false, ErrorMessage = "Field name is required" };
            }

            if (string.IsNullOrWhiteSpace(request.OldValue) || string.IsNullOrWhiteSpace(request.NewValue))
            {
                return new FieldValidationInfo { IsValid = false, ErrorMessage = "Both old and new values are required" };
            }

            if (request.OldValue == request.NewValue)
            {
                return new FieldValidationInfo { IsValid = false, ErrorMessage = "Old and new values are identical" };
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
        /// Logs correction details to the learning table
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="request">Update request</param>
        /// <param name="result">Update result</param>
        /// <returns>Task representing the async operation</returns>
        private async Task LogCorrectionLearningAsync(OCRContext context, RegexUpdateRequest request, DatabaseUpdateResult result)
        {
            try
            {
                var learning = new OCRCorrectionLearning
                {
                    FieldName = request.FieldName,
                    OriginalError = request.OldValue,
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
                    CreatedBy = "OCRCorrectionService"
                };

                context.OCRCorrectionLearning.Add(learning);
                await context.SaveChangesAsync();

                _logger?.Debug("Logged correction learning entry for field {FieldName}", request.FieldName);
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Failed to log correction learning for field {FieldName}", request.FieldName);
                // Don't throw - logging failure shouldn't stop the main process
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

        #endregion
    }
}
