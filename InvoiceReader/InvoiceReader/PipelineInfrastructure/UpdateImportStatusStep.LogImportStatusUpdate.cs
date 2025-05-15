namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class UpdateImportStatusStep
    {
        private static bool LogImportStatusUpdate(ILogger logger, ImportStatus importStatus, string filePath, int? templateId) // Add logger parameter
        {
            logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(LogImportStatusUpdate), "Logging", "Import status processed.", $"ImportStatus: {importStatus}, FilePath: {filePath}, TemplateId: {templateId}", "");
            // This step's success depends only on reaching this point after processing, not the status itself.
            return true;
        }
    }
}