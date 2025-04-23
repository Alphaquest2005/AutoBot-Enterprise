using CoreEntities.Business.Entities; // Assuming FileTypes is here
using WaterNut.Business.Services.Utils; // Assuming FileTypeManager and DataFileProcessor are here
using WaterNut.Business.Services; // Added for DataFile
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added
using System.Linq; // Added for Any() checks if needed
using System.Collections.Generic; // Added for List checks if needed

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class HandleImportSuccessStateStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        private static readonly ILogger _logger = Log.ForContext<HandleImportSuccessStateStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "Unknown";
            bool stepResult = false; // Initialize step result
            foreach (var template in context.Templates)
            {


                int? templateId = template?.OcrInvoices?.Id; // Safe access
                _logger.Information(
                    "Executing HandleImportSuccessStateStep for File: {FilePath}, TemplateId: {TemplateId}", filePath,
                    templateId);
                _logger.Verbose("Context details at start of HandleImportSuccessStateStep: {@Context}",
                    context); // Log context details

                // Null check context first
                if (context == null)
                {
                    _logger.Error("HandleImportSuccessStateStep executed with null context.");
                    return false;
                }

                bool requiredDataMissing = IsRequiredDataMissing(template); // Handles its own logging
                _logger.Debug("IsRequiredDataMissing check result: {Result}", requiredDataMissing);
                if (requiredDataMissing)
                {
                    _logger.Warning(
                        "HandleImportSuccessStateStep cannot proceed due to missing required data in context for File: {FilePath}",
                        filePath);
                    return false; // Step fails if required data is missing
                }

                
                try
                {
                    _logger.Debug("Resolving file type for File: {FilePath}, TemplateId: {TemplateId}", filePath,
                        templateId);
                    FileTypes fileType = ResolveFileType(template); // Handles its own logging
                    if (fileType == null)
                    {
                        _logger.Error(
                            "ResolveFileType returned null. Cannot proceed with data file processing for File: {FilePath}",
                            filePath);
                        return false;
                    }

                    // Removed .Name as it doesn't exist on FileTypes
                    _logger.Information("Resolved FileType to Id: {FileTypeId} for File: {FilePath}", fileType.Id,
                        filePath);

                    _logger.Debug("Creating DataFile object for File: {FilePath}", filePath);
                    DataFile dataFile = CreateDataFile(template, fileType); // Handles its own logging
                    if (dataFile == null)
                    {
                        _logger.Error(
                            "CreateDataFile returned null. Cannot proceed with data file processing for File: {FilePath}",
                            filePath);
                        return false;
                    }

                    _logger.Verbose("Created DataFile details: {@DataFile}", dataFile); // Log DataFile details

                    // Add logging for CsvLines content before processing
                    if (template.CsvLines != null && template.CsvLines.Any())
                    {
                        _logger.Verbose("CsvLines content before processing: {@CsvLines}", template.CsvLines);
                        // Optionally log details of the first few items in CsvLines if it's a large list
                        if (template.CsvLines.First() is List<IDictionary<string, object>> firstDocList)
                        {
                            _logger.Verbose("First document list in CsvLines (first 5 items): {@FirstDocList}",
                                firstDocList.Take(5).ToList());
                        }
                    }
                    else
                    {
                        _logger.Warning("CsvLines is null or empty before DataFileProcessor for File: {FilePath}",
                            filePath);
                    }

                    // Add logging for DataFile content before processing
                    if (dataFile != null)
                    {
                        _logger.Verbose(
                            "DataFile content before processing (relevant properties): FileType={FileType}, DocSet Count={DocSetCount}, OverWriteExisting={OverWriteExisting}, EmailId={EmailId}",
                            dataFile.FileType?.Id, dataFile.DocSet?.Count, dataFile.OverWriteExisting,
                            dataFile.EmailId);
                        // Note: FilePath and CsvLines properties are not available on this DataFile object for logging.
                    }
                    else
                    {
                        _logger.Error("DataFile is null before DataFileProcessor for File: {FilePath}", filePath);
                    }

                    _logger.Information("Starting DataFileProcessor for File: {FilePath}, FileTypeId: {FileTypeId}",
                        filePath, fileType.Id);
                    // Assuming DataFileProcessor().Process is synchronous or its async nature is handled internally
                    // The original code uses GetAwaiter().GetResult() which blocks.
                    // Running potentially blocking code in background thread to avoid blocking pipeline thread.
                    var processor = new DataFileProcessor();
                    bool processResult = await Task.Run(() => processor.Process(dataFile)).ConfigureAwait(false);
                    _logger.Information("DataFileProcessor finished for File: {FilePath}. Success: {ProcessResult}",
                        filePath, processResult);

                    // LogImportProcessingOutcome handles its own logging
                    stepResult = LogImportProcessingOutcome(processResult, filePath); // Pass context

                }
                catch (Exception ex)
                {
                    _logger.Error(ex,
                        "Error during HandleImportSuccessStateStep for File: {FilePath}, TemplateId: {TemplateId}",
                        filePath, templateId);
                    stepResult = false; // Indicate failure
                }
                finally
                {
                    _logger.Information(
                        "Finished executing HandleImportSuccessStateStep for File: {FilePath}, TemplateId: {TemplateId}. Result: {Result}",
                        filePath, templateId, stepResult);
                }
            }

            return stepResult;
        }

        private static bool IsRequiredDataMissing(Invoice Invoice)
        {
             _logger.Debug("Entering IsRequiredDataMissing check.");
             _logger.Verbose("Checking for missing required data in Invoice for success state.");
             // Check each property and log which one is missing if any
             // Context null check happens in Execute
             if (Invoice.FileType == null) { _logger.Warning("Missing required data for success state: FileType is null."); return true; }
             if (Invoice.DocSet == null) { _logger.Warning("Missing required data for success state: DocSet is null."); return true; }
             
             // Check Template.OcrInvoices as it's used in ResolveFileType
             if (Invoice.OcrInvoices == null) { _logger.Warning("Missing required data for success state: Template.OcrInvoices is null."); return true; }
             if (Invoice.CsvLines == null) { _logger.Warning("Missing required data for success state: CsvLines is null."); return true; }
             if (string.IsNullOrEmpty(Invoice.FilePath)) { _logger.Warning("Missing required data for success state: FilePath is null or empty."); return true; }
             if (string.IsNullOrEmpty(Invoice.EmailId)) { _logger.Warning("Missing required data for success state: EmailId is null or empty."); return true; }

             _logger.Debug("Exiting IsRequiredDataMissing check. Result: False (No missing data).");
             return false; // All required data is present
        }

        private static FileTypes ResolveFileType(Invoice template)
        {
             _logger.Debug("Entering ResolveFileType.");
             // Context and Template null checks happen in caller
             int? templateId = template.OcrInvoices?.Id; // Null check done by caller
             int originalFileTypeId = template.FileType.Id; // Null check done by caller
             int templateFileTypeId = template.OcrInvoices.FileTypeId; // Null check done by caller

             _logger.Debug("Resolving FileType. Original Context FileTypeId: {OriginalFileTypeId}, Template FileTypeId: {TemplateFileTypeId}", originalFileTypeId, templateFileTypeId);

             FileTypes fileType = template.FileType; // Start with the template's file type

             // Check if the template specifies a different FileTypeId
             if (fileType.Id != templateFileTypeId)
             {
                  _logger.Information("Template FileTypeId ({TemplateFileTypeId}) differs from template FileTypeId ({OriginalFileTypeId}). Attempting to get FileType from FileTypeManager.",
                     templateFileTypeId, originalFileTypeId);
                  try
                  {
                      // Assuming FileTypeManager has a static Instance property or similar access method
                      // Corrected: Call static GetFileType method directly
                      fileType = FileTypeManager.GetFileType(templateFileTypeId);
                      if (fileType == null)
                      {
                           _logger.Error("FileTypeManager.GetFileType returned null for FileTypeId: {FileTypeId}. Using original template FileType.", templateFileTypeId);
                           fileType = template.FileType; // Fallback to original if manager fails
                      } else {
                           // Removed .Name as it doesn't exist on FileTypes
                           _logger.Debug("Successfully retrieved FileType Id: {FileTypeId} from FileTypeManager.", fileType.Id);
                      }
                  }
                  catch (Exception ex)
                  {
                       _logger.Error(ex, "Error calling FileTypeManager.GetFileType for FileTypeId: {FileTypeId}. Using original template FileType.", templateFileTypeId);
                       fileType = template.FileType; // Fallback on error
                  }
             } else {
                  _logger.Debug("Using original FileType from template (Id: {FileTypeId}).", originalFileTypeId);
             }
             _logger.Debug("Exiting ResolveFileType. Resolved FileType Id: {FileTypeId}", fileType?.Id);
             return fileType;
        }

        // Added filePath for context
        private static bool LogImportProcessingOutcome(bool processResult, string filePath)
        {
             _logger.Debug("Entering LogImportProcessingOutcome for File: {FilePath}. ProcessResult: {ProcessResult}", filePath, processResult);
             // Replace Console.WriteLine
             if (processResult)
             {
                  _logger.Information("DataFileProcessor completed successfully for File: {FilePath}.", filePath);
             }
             else
             {
                  _logger.Error("DataFileProcessor failed for File: {FilePath}.", filePath);
             }
             _logger.Debug("Exiting LogImportProcessingOutcome for File: {FilePath}. Returning: {ProcessResult}", filePath, processResult);
            return processResult; // Indicate success based on the result of DataFileProcessor().Process
        }

        private static DataFile CreateDataFile(Invoice template, FileTypes fileType)
        {
             string filePath = template?.FilePath ?? "Unknown";
             _logger.Debug("Entering CreateDataFile for File: {FilePath}, FileTypeId: {FileTypeId}", filePath, fileType?.Id ?? -1);
             // Null checks for template properties used here happen in IsRequiredDataMissing
             DataFile dataFile = null;
             try
             {
                 // Ensure DocSet is not null before accessing its properties if needed by DataFile constructor
                 if (template.DocSet == null || !template.DocSet.Any()) {
                     _logger.Error("Cannot create DataFile: DocSet is null or empty for File: {FilePath}", filePath);
                     return null;
                 }

                 dataFile = new DataFile(fileType, template.DocSet, template.OverWriteExisting,
                                                   template.EmailId,
                                                   template.FilePath, template.CsvLines);
                  _logger.Verbose("DataFile object created successfully for File: {FilePath}", filePath);
             }
             catch (Exception ex)
             {
                  _logger.Error(ex, "Error creating DataFile object for File: {FilePath}", filePath);
                  dataFile = null;
             }
             _logger.Debug("Exiting CreateDataFile for File: {FilePath}. DataFile is null: {IsDataFileNull}", filePath, dataFile == null);
             return dataFile;
        }
    }
}