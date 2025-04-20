using OCR.Business.Entities; // Added
using System.Collections.Generic; // Added
using System.IO; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added
using MoreLinq; // Added for DistinctBy

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class HandleErrorStateStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        private static readonly ILogger _logger = Log.ForContext<HandleErrorStateStep>();

        private readonly bool _isLastTemplate; // Need to get this from somewhere, maybe context or constructor

        // Assuming _isLastTemplate is passed in the constructor
        public HandleErrorStateStep(bool isLastTemplate)
        {
            _isLastTemplate = isLastTemplate;
             _logger.Debug("HandleErrorStateStep initialized with IsLastTemplate: {IsLastTemplate}", _isLastTemplate);
             // Consider logging a warning if _isLastTemplate is intended to be used but is commented out in IsValidErrorState
             _logger.Warning("Note: _isLastTemplate field is initialized but currently commented out in IsValidErrorState check.");
        }

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "Unknown";
            int? templateId = context?.Template?.OcrInvoices?.Id; // Safe access
            _logger.Debug("Executing HandleErrorStateStep for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);

            // Null check context first
            if (context == null)
            {
                 _logger.Error("HandleErrorStateStep executed with null context.");
                 return false;
            }

            if (HasMissingRequiredData(context)) // Handles its own logging
            {
                 _logger.Warning("HandleErrorStateStep cannot proceed due to missing required data in context for File: {FilePath}", filePath);
                 return false; // Step fails if required data is missing
            }

            _logger.Debug("Getting failed lines for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
            List<Line> failedlines = GetFailedLines(context); // Handles its own logging
            _logger.Debug("Initial failed lines count: {Count}", failedlines.Count);

            _logger.Debug("Adding existing failed lines from template parts for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
            AddExistingFailedLines(context, failedlines); // Handles its own logging
            _logger.Debug("Total failed lines count after adding existing: {Count}", failedlines.Count);


            _logger.Debug("Getting distinct required lines for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
            List<Line> allRequired = GetDistinctRequiredLines(context); // Handles its own logging
            _logger.Debug("Distinct required lines count: {Count}", allRequired.Count);

            // Check if all required lines failed (and there are required lines)
            // Use safe count access
            if (allRequired.Any() && failedlines.Count >= allRequired.Count)
            {
                 _logger.Warning("All {RequiredCount} required lines appear to have failed for File: {FilePath}, TemplateId: {TemplateId}. Returning false from HandleErrorStateStep.", allRequired.Count, filePath, templateId);
                 return false; // Indicate failure if all required lines failed
            }
             _logger.Debug("Not all required lines failed (or no required lines found). Proceeding with error state check.");


            _logger.Debug("Checking if current state is a valid error state for email notification for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
            if (IsValidErrorState(context, failedlines)) // Handles its own logging
            {
                 _logger.Information("Valid error state detected. Handling error email pipeline for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                 // HandleErrorEmailPipeline handles its own logging
                 // The return value indicates if the email pipeline was *attempted*, not necessarily if it succeeded.
                 bool emailAttempted = await HandleErrorEmailPipeline(context).ConfigureAwait(false);
                 _logger.Information("HandleErrorEmailPipeline finished for File: {FilePath}. Email Attempted: {EmailAttempted}", filePath, emailAttempted);
                 // Decide what Execute should return. If email was attempted, maybe it's 'true' for this step?
                 // Original code returned true here. Let's assume attempting email means the step 'succeeded' in its error handling task.
                 return true;
            }
            else
            {
                 _logger.Information("Not a valid error state for email notification for File: {FilePath}, TemplateId: {TemplateId}. Handling as unsuccessful import.", filePath, templateId);
                 // HandleUnsuccessfulImport handles its own logging
                 return HandleUnsuccessfulImport(filePath); // Pass context for logging
            }
        }

        private static bool HasMissingRequiredData(InvoiceProcessingContext context)
        {
             _logger.Verbose("Checking for missing required data in context.");
             // Check each property and log which one is missing if any
             // Context null check happens in Execute
             if (context.Template == null) { _logger.Warning("Missing required data: Template is null."); return true; }
             if (context.CsvLines == null) { _logger.Warning("Missing required data: CsvLines is null."); return true; }
             if (context.DocSet == null) { _logger.Warning("Missing required data: DocSet is null."); return true; }
             if (context.Client == null) { _logger.Warning("Missing required data: Client is null."); return true; }
             if (string.IsNullOrEmpty(context.FilePath)) { _logger.Warning("Missing required data: FilePath is null or empty."); return true; }
             if (string.IsNullOrEmpty(context.EmailId)) { _logger.Warning("Missing required data: EmailId is null or empty."); return true; }
             if (string.IsNullOrEmpty(context.FormattedPdfText)) { _logger.Warning("Missing required data: FormattedPdfText is null or empty."); return true; }

             _logger.Verbose("No missing required data found in context.");
             return false; // All required data is present
        }

        // Added filePath for context
        private static bool HandleUnsuccessfulImport(string filePath)
        {
             _logger.Information("Handling unsuccessful import state for File: {FilePath}. No error email will be sent based on current criteria.", filePath);
            // Replace Console.WriteLine
            // Console.WriteLine($"[OCR DEBUG] Pipeline Step: Handled error state.");

            return false; // Indicate that the error state did not lead to a successful import or email action
        }

        private static async Task<bool> HandleErrorEmailPipeline(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "Unknown";
             _logger.Information("Starting HandleErrorEmailPipeline for File: {FilePath}", filePath);

             // Populate FileInfo and TxtFile in context for email pipeline
             try
             {
                 _logger.Debug("Creating FileInfo for File: {FilePath}", filePath);
                 context.FileInfo = new FileInfo(filePath); // Can throw if path is invalid

                 // Assuming TxtFile was set in a previous step (e.g., WriteFormattedTextFileStep)
                 if (string.IsNullOrEmpty(context.TxtFile))
                 {
                      _logger.Warning("TxtFile is missing in context for File: {FilePath}. Email attachment might be incomplete.", filePath);
                      // Decide if this is fatal for the email process
                 } else {
                      _logger.Debug("Using TxtFile from context: {TxtFile}", context.TxtFile);
                 }

                 // Add FailedLines to context if not already there (needed by CreateEmailPipeline/ConstructEmailBodyStep)
                 if (context.FailedLines == null)
                 {
                     _logger.Warning("Context.FailedLines is null before calling CreateEmailPipeline. Attempting to populate for File: {FilePath}", filePath);
                     // Re-calculate failed lines specifically for the email body generation
                     context.FailedLines = GetFailedLines(context); // Use the same logic
                     AddExistingFailedLines(context, context.FailedLines); // Add existing ones too
                     _logger.Information("Populated Context.FailedLines with {Count} lines for email generation.", context.FailedLines.Count);
                 }


                 // Create and run the CreateEmailPipeline
                 _logger.Debug("Creating CreateEmailPipeline instance for File: {FilePath}", filePath);
                 var createEmailPipeline = new CreateEmailPipeline(context); // Handles its own logging

                 _logger.Information("Running CreateEmailPipeline for File: {FilePath}", filePath);
                 bool emailPipelineSuccess = await createEmailPipeline.RunPipeline().ConfigureAwait(false); // Handles its own logging

                 _logger.Information("CreateEmailPipeline finished for File: {FilePath}. Success: {Success}", filePath, emailPipelineSuccess);
                 return true; // Indicate that error handling (including email) was ATTEMPTED
             }
             catch (Exception ex)
             {
                  _logger.Error(ex, "Error during HandleErrorEmailPipeline for File: {FilePath}", filePath);
                  return false; // Indicate failure in setting up or running the email pipeline
             }
        }

        private static bool IsValidErrorState(InvoiceProcessingContext context, List<Line> failedlines)
        {
             int? templateId = context?.Template?.OcrInvoices?.Id;
             string filePath = context?.FilePath ?? "Unknown";
             _logger.Debug("Checking IsValidErrorState for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);

             // Perform null checks early
             if (context?.Template?.Lines == null || context?.Template?.Parts == null || failedlines == null)
             {
                 _logger.Warning("IsValidErrorState check cannot proceed due to null Template.Lines, Template.Parts, or failedlines list for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                 return false;
             }

             // Evaluate conditions step-by-step and log
             bool hasFailedLines = failedlines.Any();
             _logger.Verbose("Condition [IsValidErrorState]: Has Failed Lines? {Result} (Count: {Count})", hasFailedLines, failedlines.Count);
             if (!hasFailedLines) return false; // Short-circuit if no failed lines

             bool lessThanTotalLines = failedlines.Count < context.Template.Lines.Count;
              _logger.Verbose("Condition [IsValidErrorState]: Failed Lines Count ({FailedCount}) < Template Lines Count ({TemplateCount})? {Result}", failedlines.Count, context.Template.Lines.Count, lessThanTotalLines);
              if (!lessThanTotalLines) return false; // Short-circuit

             // Safely check Parts conditions
             var firstPart = context.Template.Parts.FirstOrDefault();
             bool firstPartStartedOrNoStart = false;
             if (firstPart != null)
             {
                 bool wasStarted = firstPart.WasStarted;
                 // Ensure OCR_Part and Start are not null before checking Any()
                 bool hasNoStartConditions = !(firstPart.OCR_Part?.Start?.Any() ?? false);
                 firstPartStartedOrNoStart = wasStarted || hasNoStartConditions;
                  _logger.Verbose("Condition [IsValidErrorState]: First Part Started ({WasStarted}) OR First Part Has No Start Conditions ({HasNoStart})? {Result}", wasStarted, hasNoStartConditions, firstPartStartedOrNoStart);
             } else {
                  _logger.Warning("Template has no Parts. Condition 'firstPartStartedOrNoStart' evaluated as false for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                  // If no parts, this condition fails unless logic requires otherwise
                  return false;
             }
              if (!firstPartStartedOrNoStart) return false; // Short-circuit


             // Ensure Lines and Values are not null before checking Any()
             bool hasAnyValues = context.Template.Lines
                                     .Where(l => l?.Values != null) // Check line and Values not null
                                     .SelectMany(x => x.Values.Values) // Select the values from the dictionary
                                     .Any(); // Check if any values exist across all lines
              _logger.Verbose("Condition [IsValidErrorState]: Any Template Line has any Values? {Result}", hasAnyValues);
              if (!hasAnyValues) return false; // Short-circuit

             // Original code commented out _isLastTemplate check
             // bool isLastTemplateCheck = _isLastTemplate;
             // _logger.Verbose("Condition [IsValidErrorState]: Is Last Template? {Result} (Currently not used in logic)", isLastTemplateCheck);

             // If all checks passed
             _logger.Information("IsValidErrorState evaluation result: TRUE for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
             return true;
        }

        private static List<Line> GetDistinctRequiredLines(InvoiceProcessingContext context)
        {
             int? templateId = context?.Template?.OcrInvoices?.Id;
             _logger.Verbose("Getting distinct required lines for TemplateId: {TemplateId}", templateId);
             // Null check
             if (context?.Template?.Lines == null)
             {
                 _logger.Warning("Cannot get distinct required lines: Template.Lines is null for TemplateId: {TemplateId}", templateId);
                 return new List<Line>();
             }

             try
             {
                 var requiredLines = context.Template.Lines
                     .Where(line => line?.OCR_Lines?.Fields != null) // Ensure line, OCR_Lines, and Fields are not null
                     .DistinctBy(x => x.OCR_Lines.Id) // Requires MoreLinq or equivalent implementation
                     .Where(z => z.OCR_Lines.Fields.Any(f => f != null && f.IsRequired && (f.Field != "SupplierCode" && f.Field != "Name"))) // Ensure field is not null
                     .ToList();
                  _logger.Verbose("Found {Count} distinct required lines (excluding Name/SupplierCode) for TemplateId: {TemplateId}", requiredLines.Count, templateId);
                 return requiredLines;
             }
             catch (Exception ex)
             {
                  _logger.Error(ex, "Error getting distinct required lines for TemplateId: {TemplateId}", templateId);
                  return new List<Line>();
             }
        }

        private static void AddExistingFailedLines(InvoiceProcessingContext context, List<Line> failedlines)
        {
             int? templateId = context?.Template?.OcrInvoices?.Id;
             _logger.Verbose("Adding existing failed lines from template parts for TemplateId: {TemplateId}", templateId);
             // Null checks
             if (context?.Template?.Parts == null || failedlines == null)
             {
                 _logger.Warning("Cannot add existing failed lines: Template.Parts or target failedlines list is null for TemplateId: {TemplateId}", templateId);
                 return;
             }

             try
             {
                 var existingFailed = context.Template.Parts
                     .Where(part => part?.FailedLines != null) // Check part and FailedLines are not null
                     .SelectMany(z => z.FailedLines)
                     .Where(line => line != null) // Ensure individual lines are not null
                     .ToList();

                 int countAdded = existingFailed.Count;
                 _logger.Verbose("Found {Count} existing failed lines in Template Parts to add for TemplateId: {TemplateId}", countAdded, templateId);
                 if (countAdded > 0)
                 {
                    failedlines.AddRange(existingFailed);
                 }
             }
             catch (Exception ex)
             {
                  _logger.Error(ex, "Error adding existing failed lines for TemplateId: {TemplateId}", templateId);
             }
        }

        private static List<Line> GetFailedLines(InvoiceProcessingContext context)
        {
             int? templateId = context?.Template?.OcrInvoices?.Id;
             _logger.Verbose("Getting initially failed lines (FailedFields or Missing Required Values) for TemplateId: {TemplateId}", templateId);
             // Null check
             if (context?.Template?.Lines == null)
             {
                 _logger.Warning("Cannot get failed lines: Template.Lines is null for TemplateId: {TemplateId}", templateId);
                 return new List<Line>();
             }

             try
             {
                 var failed = context.Template.Lines
                     .Where(line => line?.OCR_Lines != null) // Ensure line and OCR_Lines not null
                     .DistinctBy(x => x.OCR_Lines.Id) // Requires MoreLinq or equivalent implementation
                     .Where(z =>
                         (z.FailedFields != null && z.FailedFields.Any()) || // Has any explicitly marked failed fields
                         (
                             (z.OCR_Lines.Fields != null && z.OCR_Lines.Fields.Any(f => f != null && f.IsRequired && f.FieldValue?.Value == null)) && // Has a required field with null value
                             (z.Values == null || !z.Values.Any()) // And has no successfully extracted values for the line
                         )
                      )
                     .ToList();
                  _logger.Verbose("Found {Count} initially failed lines for TemplateId: {TemplateId}", failed.Count, templateId);
                 return failed;
             }
             catch (Exception ex)
             {
                  _logger.Error(ex, "Error getting failed lines for TemplateId: {TemplateId}", templateId);
                  return new List<Line>();
             }
        }
    }
}