// DELETED - This file has been merged into OCRCorrectionService.cs

        public OCRCorrectionService()
        {
            _deepSeekApi = new DeepSeekInvoiceApi();
        }

        /// <summary>
        /// Updates OCR regex patterns based on field errors identified by DeepSeek
        /// </summary>
        public async Task UpdateRegexPatternsAsync(
            List<(string Field, string Error, string Value)> errors,
            string fileTxt,
            OCR.Business.Entities.Invoices ocrInvoice)
        {
            if (!errors?.Any() == true || string.IsNullOrEmpty(fileTxt) || ocrInvoice?.Parts == null)
                return;

            var fileLines = SplitIntoLines(fileTxt);

            var correctionTasks = errors.Select(async error =>
                await ProcessFieldErrorAsync(error, fileTxt, fileLines, ocrInvoice));

            var corrections = await Task.WhenAll(correctionTasks);

            await ApplyCorrectionsAsync(corrections.Where(c => c != null));
        }

        /// <summary>
        /// Processes a single field error and determines the appropriate correction
        /// </summary>
        private async Task<OCRCorrection> ProcessFieldErrorAsync(
            (string Field, string Error, string Value) error,
            string fileTxt,
            string[] fileLines,
            OCR.Business.Entities.Invoices ocrInvoice)
        {
            try
            {
                // Get line information from DeepSeek
                var lineInfo = await GetErrorLineInfoAsync(error, fileTxt);
                if (lineInfo == null) return null;

                // Create 10-line window (5 before + target + 5 after)
                var windowLines = GetLineWindow(fileLines, lineInfo.LineNumber, 5);

                // Find matching OCR field in template
                var matchingField = FindMatchingOCRField(error.Field, windowLines, ocrInvoice);
                if (matchingField == null) return null;

                // Ask DeepSeek to determine the best correction approach
                var correctionStrategy = await GetCorrectionStrategyAsync(error, lineInfo, windowLines, matchingField);

                return new OCRCorrection
                {
                    Error = error,
                    LineInfo = lineInfo,
                    Field = matchingField,
                    Strategy = correctionStrategy,
                    WindowLines = windowLines
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing field {error.Field}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets line information where DeepSeek found the error
        /// </summary>
        private async Task<LineInfo> GetErrorLineInfoAsync(
            (string Field, string Error, string Value) error,
            string fileTxt)
        {
            var prompt = CreateLineDetectionPrompt(error, fileTxt);
            var response = await _deepSeekApi.GetResponseAsync(prompt);

            var lineInfo = ParseLineInfoResponse(response);

            // Store prompt and response for debugging
            if (lineInfo != null)
            {
                lineInfo.DeepSeekPrompt = prompt;
                lineInfo.DeepSeekResponse = response;
            }

            return lineInfo;
        }

        /// <summary>
        /// Creates a window of lines around the target line
        /// </summary>
        private static string[] GetLineWindow(string[] fileLines, int targetLine, int windowSize)
        {
            var startLine = Math.Max(0, targetLine - windowSize);
            var endLine = Math.Min(fileLines.Length - 1, targetLine + windowSize);
            var windowLength = endLine - startLine + 1;

            var window = new string[windowLength];
            Array.Copy(fileLines, startLine, window, 0, windowLength);

            return window;
        }

        /// <summary>
        /// Finds the matching OCR field by testing line regex patterns
        /// </summary>
        private static Fields FindMatchingOCRField(
            string deepSeekFieldName,
            string[] windowLines,
            OCR.Business.Entities.Invoices ocrInvoice)
        {
            return ocrInvoice.Parts
                .SelectMany(part => part.Lines)
                .SelectMany(line => line.Fields)
                .Where(field => string.Equals(field.Key, deepSeekFieldName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault(field => TestFieldRegexInWindow(field, windowLines));
        }

        /// <summary>
        /// Tests if a field's regex matches any line in the window
        /// </summary>
        private static bool TestFieldRegexInWindow(Fields field, string[] windowLines)
        {
            if (field.Lines?.RegularExpressions?.RegEx == null) return false;

            var regex = new Regex(field.Lines.RegularExpressions.RegEx,
                RegexOptions.IgnoreCase | (field.Lines.RegularExpressions.MultiLine ? RegexOptions.Multiline : RegexOptions.None));

            return windowLines.Any(line => regex.IsMatch(line ?? string.Empty));
        }

        /// <summary>
        /// Asks DeepSeek to determine the best correction strategy
        /// </summary>
        private async Task<CorrectionStrategy> GetCorrectionStrategyAsync(
            (string Field, string Error, string Value) error,
            LineInfo lineInfo,
            string[] windowLines,
            Fields field)
        {
            var prompt = CreateCorrectionStrategyPrompt(error, lineInfo, windowLines, field);
            var response = await _deepSeekApi.GetResponseAsync(prompt);

            return ParseCorrectionStrategyResponse(response);
        }

        /// <summary>
        /// Splits text into lines, removing empty entries
        /// </summary>
        private static string[] SplitIntoLines(string text) =>
            text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        /// <summary>
        /// Applies the determined corrections to the database
        /// </summary>
        private async Task ApplyCorrectionsAsync(IEnumerable<OCRCorrection> corrections)
        {
            using var ctx = new OCRContext();

            foreach (var correction in corrections)
            {
                try
                {
                    await ApplySingleCorrectionAsync(ctx, correction);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error applying correction for field {correction.Error.Field}: {ex.Message}");
                }
            }

            await ctx.SaveChangesAsync();
        }

        /// <summary>
        /// Applies a single correction based on the determined strategy
        /// </summary>
        private async Task ApplySingleCorrectionAsync(OCRContext ctx, OCRCorrection correction)
        {
            var startTime = DateTime.UtcNow;
            bool success = true;
            string errorMessage = null;

            try
            {
                switch (correction.Strategy.Type)
                {
                    case CorrectionType.UpdateLineRegex:
                        await UpdateLineRegexAsync(ctx, correction);
                        break;
                    case CorrectionType.AddFieldFormatRegex:
                        await AddFieldFormatRegexAsync(ctx, correction);
                        break;
                    case CorrectionType.CreateNewRegex:
                        await CreateNewRegexAsync(ctx, correction);
                        break;
                }
            }
            catch (Exception ex)
            {
                success = false;
                errorMessage = ex.Message;
                Console.WriteLine($"Error applying correction for field {correction.Field.Key}: {ex.Message}");
            }

            // Log the correction for learning (including success/failure info)
            await LogCorrectionAsync(ctx, correction, success, errorMessage, startTime);
        }

        /// <summary>
        /// Updates the line regex pattern for better field detection
        /// </summary>
        private async Task UpdateLineRegexAsync(OCRContext ctx, OCRCorrection correction)
        {
            var regex = correction.Field.Lines.RegularExpressions;
            if (regex != null)
            {
                regex.RegEx = correction.Strategy.NewRegexPattern;
                regex.TrackingState = TrackingState.Modified;
                Console.WriteLine($"Updated line regex for field {correction.Field.Key}: {correction.Strategy.NewRegexPattern}");
            }
        }

        /// <summary>
        /// Adds a FieldFormatRegEx for post-processing corrections
        /// </summary>
        private async Task AddFieldFormatRegexAsync(OCRContext ctx, OCRCorrection correction)
        {
            // Get or create the correction regex
            var correctionRegex = await GetOrCreateRegularExpressionAsync(ctx, correction.Strategy.NewRegexPattern);
            var replacementRegex = await GetOrCreateRegularExpressionAsync(ctx, correction.Strategy.ReplacementPattern);

            // Check if this FieldFormatRegEx already exists
            var existingFormatRegex = ctx.OCR_FieldFormatRegEx.FirstOrDefault(x =>
                x.FieldId == correction.Field.Id &&
                x.RegExId == correctionRegex.Id &&
                x.ReplacementRegExId == replacementRegex.Id);

            if (existingFormatRegex == null)
            {
                var newFormatRegex = new FieldFormatRegEx()
                {
                    Fields = correction.Field,
                    RegEx = correctionRegex,
                    ReplacementRegEx = replacementRegex,
                    TrackingState = TrackingState.Added
                };
                ctx.OCR_FieldFormatRegEx.Add(newFormatRegex);
                Console.WriteLine($"Added FieldFormatRegEx for field {correction.Field.Key}: {correction.Strategy.NewRegexPattern} -> {correction.Strategy.ReplacementPattern}");
            }
        }

        /// <summary>
        /// Creates a new combined regex pattern
        /// </summary>
        private async Task CreateNewRegexAsync(OCRContext ctx, OCRCorrection correction)
        {
            var existingPattern = correction.Field.Lines.RegularExpressions?.RegEx ?? "";
            var newCombinedPattern = $"({existingPattern})|({correction.Strategy.NewRegexPattern})";

            var regex = correction.Field.Lines.RegularExpressions;
            if (regex != null)
            {
                regex.RegEx = newCombinedPattern;
                regex.TrackingState = TrackingState.Modified;
                Console.WriteLine($"Created new combined regex for field {correction.Field.Key}: {newCombinedPattern}");
            }
        }

        /// <summary>
        /// Gets an existing regex or creates a new one
        /// </summary>
        private async Task<RegularExpressions> GetOrCreateRegularExpressionAsync(OCRContext ctx, string pattern)
        {
            var existing = ctx.RegularExpressions.FirstOrDefault(x => x.RegEx == pattern);
            if (existing != null) return existing;

            var newRegex = new RegularExpressions(true)
            {
                RegEx = pattern,
                MultiLine = false,
                TrackingState = TrackingState.Added
            };
            ctx.RegularExpressions.Add(newRegex);
            return newRegex;
        }

        /// <summary>
        /// Logs the correction for learning and analysis
        /// </summary>
        private async Task LogCorrectionAsync(OCRContext ctx, OCRCorrection correction, bool success = true, string errorMessage = null, DateTime? startTime = null)
        {
            try
            {
                var processingTime = startTime.HasValue ? (int)(DateTime.UtcNow - startTime.Value).TotalMilliseconds : (int?)null;

                var learningEntry = new OCRCorrectionLearning
                {
                    FieldName = correction.Error.Field,
                    OriginalError = correction.Error.Error,
                    CorrectValue = correction.Error.Value,
                    LineNumber = correction.LineInfo.LineNumber,
                    LineText = correction.LineInfo.LineText,
                    WindowText = string.Join("\n", correction.WindowLines),
                    ExistingRegex = correction.Field.Lines?.RegularExpressions?.RegEx,
                    CorrectionType = correction.Strategy.Type.ToString(),
                    NewRegexPattern = correction.Strategy.NewRegexPattern,
                    ReplacementPattern = correction.Strategy.ReplacementPattern,
                    DeepSeekReasoning = correction.Strategy.Reasoning,
                    Confidence = (decimal?)correction.Strategy.Confidence,
                    FieldId = correction.Field.Id,
                    Success = success,
                    ErrorMessage = errorMessage,
                    ProcessingTimeMs = processingTime,
                    DeepSeekPrompt = correction.LineInfo.DeepSeekPrompt,
                    DeepSeekResponse = correction.LineInfo.DeepSeekResponse,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "OCRCorrectionService",
                    TrackingState = TrackingState.Added
                };

                ctx.OCRCorrectionLearning.Add(learningEntry);

                var status = success ? "SUCCESS" : "FAILED";
                Console.WriteLine($"Logged {status} correction for field {correction.Field.Key}: {correction.Strategy.Reasoning}");

                if (!success && !string.IsNullOrEmpty(errorMessage))
                {
                    Console.WriteLine($"Error details: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging correction: {ex.Message}");
                // Don't throw - logging failure shouldn't stop the correction process
            }
        }

        #region Prompt Creation Methods

        /// <summary>
        /// Creates prompt for DeepSeek to identify the line where the error was found
        /// </summary>
        private string CreateLineDetectionPrompt((string Field, string Error, string Value) error, string fileTxt)
        {
            return $@"
You are an OCR error detection specialist. Analyze the following invoice text and find the line where the field '{error.Field}' contains the incorrect value '{error.Error}' instead of the correct value '{error.Value}'.

Common OCR errors to watch for:
- Commas instead of periods in numbers (10,00 should be 10.00)
- Character confusion: 1↔l, 1↔I, 0↔O, 5↔S, 6↔G, 8↔B

Invoice Text:
{fileTxt}

Return ONLY a JSON response with:
{{
  ""lineNumber"": <1-based line number>,
  ""lineText"": ""<exact line text where the error was found>""
}}

Field: {error.Field}
Incorrect Value: {error.Error}
Correct Value: {error.Value}";
        }

        /// <summary>
        /// Creates prompt for DeepSeek to determine the best correction strategy
        /// </summary>
        private string CreateCorrectionStrategyPrompt(
            (string Field, string Error, string Value) error,
            LineInfo lineInfo,
            string[] windowLines,
            Fields field)
        {
            var existingRegex = field.Lines?.RegularExpressions?.RegEx ?? "No existing regex";
            var windowText = string.Join("\n", windowLines.Select((line, i) => $"{i + 1}: {line}"));

            return $@"
You are an OCR regex correction specialist. Analyze the following situation and determine the best correction approach.

FIELD INFORMATION:
- Field Name: {error.Field}
- Current Regex: {existingRegex}
- Error Found: '{error.Error}' should be '{error.Value}'
- Line Number: {lineInfo.LineNumber}
- Line Text: {lineInfo.LineText}

TEXT WINDOW (10 lines around error):
{windowText}

CORRECTION OPTIONS:
1. UpdateLineRegex: Update existing line regex if it failed to detect the value
2. AddFieldFormatRegex: Add post-processing regex if OCR captured wrong format (e.g., '10,00' → '10.00')
3. CreateNewRegex: Create new combined regex if no reasonable pattern exists that won't reduce existing matches

RULES:
- Use option 1 if the problem is regex identification failure
- Use option 2 if text contains incorrect value format (comma/period confusion, character misrecognition)
- Use option 3 if no easy or reasonable identifying regex can be found

Common OCR issues: comma/period confusion (10,00 vs 10.00), character misrecognition (1/l, 1/I, 0/O, 5/S, 6/G, 8/B)

Return ONLY a JSON response:
{{
  ""type"": ""UpdateLineRegex|AddFieldFormatRegex|CreateNewRegex"",
  ""newRegexPattern"": ""<regex pattern>"",
  ""replacementPattern"": ""<replacement pattern if applicable>"",
  ""reasoning"": ""<detailed explanation of decision>"",
  ""confidence"": <0.0-1.0>
}}";
        }

        #endregion

        #region Response Parsing Methods

        /// <summary>
        /// Parses DeepSeek response for line information
        /// </summary>
        private LineInfo ParseLineInfoResponse(string response)
        {
            try
            {
                // TODO: Implement JSON parsing for DeepSeek response
                // For now, return a placeholder
                return new LineInfo { LineNumber = 1, LineText = "Placeholder" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing line info response: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Parses DeepSeek response for correction strategy
        /// </summary>
        private CorrectionStrategy ParseCorrectionStrategyResponse(string response)
        {
            try
            {
                // TODO: Implement JSON parsing for DeepSeek response
                // For now, return a placeholder
                return new CorrectionStrategy
                {
                    Type = CorrectionType.AddFieldFormatRegex,
                    NewRegexPattern = @"\d+[\,\.]+\d+",
                    ReplacementPattern = "$1.$2",
                    Reasoning = "Placeholder reasoning",
                    Confidence = 0.8
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing correction strategy response: {ex.Message}");
                return null;
            }
        }

        #endregion
    }

    #region Data Models

    public class OCRCorrection
    {
        public (string Field, string Error, string Value) Error { get; set; }
        public LineInfo LineInfo { get; set; }
        public Fields Field { get; set; }
        public CorrectionStrategy Strategy { get; set; }
        public string[] WindowLines { get; set; }
    }

    public class LineInfo
    {
        public int LineNumber { get; set; }
        public string LineText { get; set; }
        public string DeepSeekPrompt { get; set; }
        public string DeepSeekResponse { get; set; }
    }

    public class CorrectionStrategy
    {
        public CorrectionType Type { get; set; }
        public string NewRegexPattern { get; set; }
        public string ReplacementPattern { get; set; }
        public string Reasoning { get; set; }
        public double Confidence { get; set; }
    }

    public enum CorrectionType
    {
        UpdateLineRegex,      // Option 1: Update existing line regex
        AddFieldFormatRegex,  // Option 2: Add FieldFormatRegEx for post-processing
        CreateNewRegex        // Option 3: Create new combined regex
    }

    #endregion
}
