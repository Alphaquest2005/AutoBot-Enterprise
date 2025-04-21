namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class UpdateImportStatusStep
    {
        private static bool LogImportStatusUpdate(ImportStatus importStatus, string filePath, int? templateId)
        {
            _logger.Information(
                "Import status processed as {ImportStatus} for File: {FilePath}, TemplateId: {TemplateId}",
                importStatus, filePath, templateId);
            // This step's success depends only on reaching this point after processing, not the status itself.
            return true;
        }
    }
}