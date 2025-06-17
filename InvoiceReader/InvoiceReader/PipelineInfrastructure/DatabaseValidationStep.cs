// File: WaterNut.DataSpace.PipelineInfrastructure/DatabaseValidationStep.cs
using InvoiceReader.OCRCorrectionService;
using InvoiceReader.PipelineInfrastructure;
using Serilog;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    /// <summary>
    /// A new pipeline step dedicated to validating and healing the OCR template database.
    /// This step ensures that all subsequent steps work with a clean and consistent set of rules.
    /// </summary>
    public class DatabaseValidationStep : IPipelineStep<InvoiceProcessingContext>
    {
        private readonly DatabaseValidator _validator;

        public DatabaseValidationStep(ILogger logger)
        {
            // This step creates its own context to ensure it's working directly
            // with the database without any prior in-memory state.
            _validator = new DatabaseValidator(new OCR.Business.Entities.OCRContext(), logger);
        }

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            var methodStopwatch = Stopwatch.StartNew();
            context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}.",
                nameof(DatabaseValidationStep), "Validate and heal the entire OCR template database.");

            try
            {
                context.Logger?.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                    nameof(DatabaseValidationStep), "Running global database validation and healing process.");

                _validator.ValidateAndHealTemplate();

                // After healing, it's crucial to invalidate any static caches to force a full reload.
                GetTemplatesStep.InvalidateTemplateCache();
                context.Logger?.Information("INTERNAL_STEP: Template cache invalidated after database healing to force reload.");

                methodStopwatch.Stop();
                context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(DatabaseValidationStep), "Successfully validated and healed database.", methodStopwatch.ElapsedMilliseconds);

                return await Task.FromResult(true).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                methodStopwatch.Stop();
                string errorMessage = $"Critical error during DatabaseValidationStep: {ex.Message}";
                context.Logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(DatabaseValidationStep), methodStopwatch.ElapsedMilliseconds, errorMessage);
                context.AddError(errorMessage);
                return await Task.FromResult(false).ConfigureAwait(false);
            }
        }
    }
}