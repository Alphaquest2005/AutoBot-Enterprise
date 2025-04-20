using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace.PipelineInfrastructure;

public partial class UpdateImportStatusStep
{
    private static ImportStatus ProcessImportFile(InvoiceProcessingContext context)
    {
        // Required data checks happen in caller (IsImportDataPresent)
        string filePath = context.FilePath;
        int templateId = context.Template.OcrInvoices.Id; // Already checked not null
        string templateName = context.Template.OcrInvoices.Name ?? "UnknownName";
        string fileDescription = "Unknown Description"; // Default

        _logger.Debug("Processing import file status for File: {FilePath}, TemplateId: {TemplateId}", filePath,
            templateId);

        try
        {
            // Safely get FileType description
            int fileTypeId = context.Template.OcrInvoices.FileTypeId;
            _logger.Verbose("Attempting to get FileType description for FileTypeId: {FileTypeId}", fileTypeId);
            // Corrected: Use static GetFileType method
            var fileType = FileTypeManager.GetFileType(fileTypeId);
            if (fileType != null)
            {
                // Corrected: Use Description property if it exists, otherwise log ID
                fileDescription = fileType.Description ?? $"FileTypeID_{fileTypeId}";
                _logger.Verbose("Retrieved FileType Description: '{FileDescription}' for FileTypeId: {FileTypeId}",
                    fileDescription, fileTypeId);
            }
            else
            {
                _logger.Warning(
                    "FileTypeManager returned null for FileTypeId: {FileTypeId}. Using default description.",
                    fileTypeId);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting FileType description from FileTypeManager for FileTypeId: {FileTypeId}",
                context.Template.OcrInvoices.FileTypeId);
            // Continue with default description
        }


        // Assuming ImportStatus is available in the context based on previous steps
        // If not set, it defaults to 0 (likely 'Failed')
        ImportStatus importStatus = context.ImportStatus;
        _logger.Information(
            "Current ImportStatus from context: {ImportStatus} for File: {FilePath}, TemplateId: {TemplateId}",
            importStatus, filePath, templateId);

        string importKey = $"{filePath}-{templateName}-{templateId}";
        _logger.Debug("Generated Imports dictionary key: {ImportKey}", importKey);

        try
        {
            // Use TryAdd for safety, although key should be unique per template run
            // If TryAdd fails, overwrite the existing entry.
            if (!context.Imports.TryAdd(importKey, (filePath, fileDescription, importStatus)))
            {
                _logger.Warning(
                    "Failed to add entry to Imports dictionary (Key already exists?): Key: {ImportKey}, Status: {ImportStatus}. Overwriting existing entry.",
                    importKey, importStatus);
                // Overwrite the existing entry if TryAdd fails
                context.Imports[importKey] = (filePath, fileDescription, importStatus);
            }
            else
            {
                _logger.Information("Added entry to Imports dictionary with Key: {ImportKey}, Status: {ImportStatus}",
                    importKey, importStatus);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error adding/updating entry in Imports dictionary for Key: {ImportKey}", importKey);
            // Depending on requirements, might need to handle this failure more explicitly
        }


        // The method returns the status that was processed and added to the dictionary
        return importStatus;
    }
}