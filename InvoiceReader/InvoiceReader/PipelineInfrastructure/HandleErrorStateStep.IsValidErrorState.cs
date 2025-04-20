namespace WaterNut.DataSpace.PipelineInfrastructure;

public partial class HandleErrorStateStep
{
    private static bool IsValidErrorState(InvoiceProcessingContext context, List<Line> failedlines)
    {
        int? templateId = context?.Template?.OcrInvoices?.Id;
        string filePath = context?.FilePath ?? "Unknown";
        _logger.Debug("Checking IsValidErrorState for File: {FilePath}, TemplateId: {TemplateId}", filePath,
            templateId);

        // Perform null checks early
        if (context?.Template?.Lines == null || context?.Template?.Parts == null || failedlines == null)
        {
            _logger.Warning(
                "IsValidErrorState check cannot proceed due to null Template.Lines, Template.Parts, or failedlines list for File: {FilePath}, TemplateId: {TemplateId}",
                filePath, templateId);
            return false;
        }

        // Evaluate conditions step-by-step and log
        bool hasFailedLines = failedlines.Any();
        _logger.Verbose("Condition [IsValidErrorState]: Has Failed Lines? {Result} (Count: {Count})", hasFailedLines,
            failedlines.Count);
        if (!hasFailedLines) return false; // Short-circuit if no failed lines

        bool lessThanTotalLines = failedlines.Count < context.Template.Lines.Count;
        _logger.Verbose(
            "Condition [IsValidErrorState]: Failed Lines Count ({FailedCount}) < Template Lines Count ({TemplateCount})? {Result}",
            failedlines.Count, context.Template.Lines.Count, lessThanTotalLines);
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
            _logger.Verbose(
                "Condition [IsValidErrorState]: First Part Started ({WasStarted}) OR First Part Has No Start Conditions ({HasNoStart})? {Result}",
                wasStarted, hasNoStartConditions, firstPartStartedOrNoStart);
        }
        else
        {
            _logger.Warning(
                "Template has no Parts. Condition 'firstPartStartedOrNoStart' evaluated as false for File: {FilePath}, TemplateId: {TemplateId}",
                filePath, templateId);
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
        _logger.Information("IsValidErrorState evaluation result: TRUE for File: {FilePath}, TemplateId: {TemplateId}",
            filePath, templateId);
        return true;
    }
}