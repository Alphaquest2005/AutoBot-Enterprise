// Assuming PipelineRunner is here
// Assuming ImportStatus is here

using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class InvoiceProcessingPipeline
    {
        // Add a static logger instance for this class
        private static readonly ILogger _logger = Log.ForContext<InvoiceProcessingPipeline>();

        private readonly InvoiceProcessingContext _context;
        private readonly bool _isLastTemplate;

        public InvoiceProcessingPipeline(InvoiceProcessingContext context, bool isLastTemplate)
        {
            _context = context;
            _isLastTemplate = isLastTemplate;
             // Log initialization with context details
             string filePath = _context?.FilePath ?? "Unknown";
             int? templateId = _context?.Template?.OcrInvoices?.Id; // Template might be null initially
             _logger.Debug("InvoiceProcessingPipeline initialized for File: {FilePath}, IsLastTemplate: {IsLastTemplate}, Initial TemplateId: {TemplateId}",
                filePath, _isLastTemplate, templateId);
        }

        public async Task<bool> RunPipeline()
        {
             string filePath = _context?.FilePath ?? "Unknown";
             _logger.Information("Starting main InvoiceProcessingPipeline execution for File: {FilePath}", filePath);

             // Null check context early
             if (_context == null)
             {
                  _logger.Error("InvoiceProcessingPipeline cannot run with a null context.");
                  return false;
             }

            try
            {
                _logger.Debug("Initializing initial pipeline steps for File: {FilePath}", filePath);
                List<IPipelineStep<InvoiceProcessingContext>> initialSteps = InitializePipelineSteps(); // Handles its own logging
                _logger.Debug("Initial pipeline steps created. Count: {Count}", initialSteps.Count);

                _logger.Information("Running initial pipeline steps (Format, Read) for File: {FilePath}", filePath);
                bool initialRunSuccess = await RunInitialPipelineSteps(initialSteps).ConfigureAwait(false); // Handles its own logging
                _logger.Information("Initial pipeline steps completed for File: {FilePath}. Success: {Success}", filePath, initialRunSuccess);

                _logger.Debug("Checking if initial run was unsuccessful for File: {FilePath}", filePath);
                if (IsInitialRunUnsuccessful(initialRunSuccess)) // Handles its own logging
                {
                    _logger.Warning("Initial run deemed unsuccessful for File: {FilePath}. Processing error pipeline.", filePath);
                    // ProcessErrorPipeline handles its own logging
                    bool errorPipelineResult = await ProcessErrorPipeline().ConfigureAwait(false);
                    _logger.Information("Error pipeline processing finished for File: {FilePath}. Result (Continue?): {Result}", filePath, errorPipelineResult);
                    return errorPipelineResult; // Return result from error pipeline
                }
                else
                {
                    _logger.Information("Initial run deemed successful for File: {FilePath}. Processing success steps.", filePath);
                    // ProcessSuccessfulSteps handles its own logging
                    bool successPipelineResult = await ProcessSuccessfulSteps().ConfigureAwait(false);
                    _logger.Information("Success pipeline processing finished for File: {FilePath}. Result (Overall Success?): {Result}", filePath, successPipelineResult);
                    return successPipelineResult; // Return result from success pipeline
                }
            }
            catch (Exception ex)
            {
                 _logger.Fatal(ex, "Fatal error during main InvoiceProcessingPipeline execution for File: {FilePath}", filePath);
                 // Ensure status reflects failure before returning
                 if (_context != null) _context.ImportStatus = ImportStatus.Failed;
                 return false; // Indicate overall failure
            }
        }

        private async Task<bool> ProcessSuccessfulSteps()
        {
             string filePath = _context?.FilePath ?? "Unknown";
             _logger.Information("Starting ProcessSuccessfulSteps for File: {FilePath}", filePath);
            // Replace Console.WriteLine

            try
            {
                // Success handling pipeline
                _logger.Debug("Creating success pipeline steps for File: {FilePath}", filePath);
                var successSteps = new List<IPipelineStep<InvoiceProcessingContext>>
                    {
                        new AddNameSupplierStep(),
                        new AddMissingRequiredFieldValuesStep(),
                        new WriteFormattedTextFileStep(), // Assuming this step exists
                        new HandleImportSuccessStateStep(),
                        new UpdateImportStatusStep() // Update status after success handling
                    };
                 _logger.Debug("Success pipeline steps created. Count: {Count}", successSteps.Count);

                // Pass pipeline name to runner constructor
                var successRunner = new PipelineRunner<InvoiceProcessingContext>(successSteps, "Success Pipeline");
                 _logger.Information("Running success pipeline steps for File: {FilePath}", filePath);
                 // Assuming PipelineRunner handles internal step logging/errors and context updates
                await successRunner.Run(_context).ConfigureAwait(false);
                 _logger.Information("Success pipeline runner finished for File: {FilePath}", filePath);

                // Determine overall success based on final import status
                bool overallSuccess = _context.ImportStatus == ImportStatus.Success || _context.ImportStatus == ImportStatus.HasErrors;
                 _logger.Information("Final ImportStatus after success steps: {ImportStatus}. Overall Success considered: {OverallSuccess}", _context.ImportStatus, overallSuccess);
                return overallSuccess;
            }
            catch (Exception ex)
            {
                 _logger.Error(ex, "Error during ProcessSuccessfulSteps for File: {FilePath}", filePath);
                 if (_context != null) _context.ImportStatus = ImportStatus.Failed; // Ensure status reflects failure
                 return false;
            }
        }

        private async Task<bool> ProcessErrorPipeline()
        {
             string filePath = _context?.FilePath ?? "Unknown";
             _logger.Information("Starting ProcessErrorPipeline for File: {FilePath}", filePath);
            // Replace Console.WriteLine

            try
            {
                // Error handling pipeline
                 _logger.Debug("Creating error pipeline steps for File: {FilePath}", filePath);
                var errorSteps = new List<IPipelineStep<InvoiceProcessingContext>>
                    {
                        new HandleErrorStateStep(_isLastTemplate), // Handles email logic
                        new UpdateImportStatusStep() // Update status after error handling
                    };
                 _logger.Debug("Error pipeline steps created. Count: {Count}", errorSteps.Count);

                // Pass pipeline name to runner constructor
                var errorRunner = new PipelineRunner<InvoiceProcessingContext>(errorSteps, "Error Pipeline");
                 _logger.Information("Running error pipeline steps for File: {FilePath}", filePath);
                 // Assuming PipelineRunner handles internal step logging/errors and context updates
                await errorRunner.Run(_context).ConfigureAwait(false);
                 _logger.Information("Error pipeline runner finished for File: {FilePath}", filePath);

                // Determine if processing should continue based on final status
                bool continueProcessing = _context.ImportStatus != ImportStatus.Failed;
                 _logger.Information("Final ImportStatus after error steps: {ImportStatus}. Continue Processing considered: {ContinueProcessing}", _context.ImportStatus, continueProcessing);
                return continueProcessing;
            }
            catch (Exception ex)
            {
                 _logger.Error(ex, "Error during ProcessErrorPipeline for File: {FilePath}", filePath);
                 if (_context != null) _context.ImportStatus = ImportStatus.Failed; // Ensure status reflects failure
                 return false; // Indicate failure to continue
            }
        }

        private bool IsInitialRunUnsuccessful(bool initialRunSuccess)
        {
             string filePath = _context?.FilePath ?? "Unknown";
             _logger.Debug("Checking IsInitialRunUnsuccessful for File: {FilePath}. InitialRunSuccess: {InitialRunSuccess}", filePath, initialRunSuccess);

             // Check each condition and log
             if (!initialRunSuccess)
             {
                  _logger.Warning("Initial run was unsuccessful (initialRunSuccess is false).");
                  return true;
             }
             // Safe check for CsvLines
             if (_context.CsvLines == null)
             {
                  _logger.Warning("Initial run considered unsuccessful: CsvLines is null.");
                  return true;
             }
             // Safe check for CsvLines count using Any()
             if (!_context.CsvLines.Any())
             {
                  _logger.Warning("Initial run considered unsuccessful: CsvLines count is 0.");
                  return true;
             }
             // Check Template and Success property safely
             bool templateSuccess = _context.Template?.Success ?? false; // Default to false if Template is null
             if (!templateSuccess)
             {
                  _logger.Warning("Initial run considered unsuccessful: Template.Success is false (or Template is null). TemplateId: {TemplateId}", _context.Template?.OcrInvoices?.Id);
                  return true;
             }

             _logger.Debug("Initial run conditions met for success path for File: {FilePath}", filePath);
             return false; // All conditions passed, initial run was successful enough
        }

        private async Task<bool> RunInitialPipelineSteps(List<IPipelineStep<InvoiceProcessingContext>> initialSteps)
        {
             string filePath = _context?.FilePath ?? "Unknown";
             _logger.Debug("Starting RunInitialPipelineSteps for File: {FilePath}", filePath);
             try
             {
                 // Pass pipeline name to runner constructor
                 var initialRunner = new PipelineRunner<InvoiceProcessingContext>(initialSteps, "Initial Pipeline");
                 _logger.Verbose("Initial PipelineRunner created.");
                 // Assuming PipelineRunner internally logs start/end of each step and handles context updates
                 bool initialRunSuccess = await initialRunner.Run(_context).ConfigureAwait(false);
                 _logger.Debug("Initial PipelineRunner finished. Overall Success: {Success}", initialRunSuccess);
                 return initialRunSuccess;
             }
             catch (Exception ex)
             {
                  _logger.Error(ex, "Error during RunInitialPipelineSteps for File: {FilePath}", filePath);
                  return false; // Indicate failure of initial steps
             }
        }

        private static List<IPipelineStep<InvoiceProcessingContext>> InitializePipelineSteps()
        {
             _logger.Debug("Initializing initial pipeline steps (FormatPdfTextStep, ReadFormattedTextStep).");
             // Initial steps: Format and Read
             var steps = new List<IPipelineStep<InvoiceProcessingContext>>
             {
                 new FormatPdfTextStep(),
                 new ReadFormattedTextStep() // Assuming this step exists
             };
             _logger.Verbose("Initial pipeline steps created. Count: {Count}", steps.Count);
             return steps;
        }
    }
}