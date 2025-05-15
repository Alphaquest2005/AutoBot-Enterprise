using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Serilog;
using OCR.Business.Entities;
using Core.Common;
using WaterNut.DataSpace;
using WaterNut.DataSpace.PipelineInfrastructure;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using Microsoft.Office.Interop.Excel;

namespace InvoiceReader.PipelineInfrastructure
{
    using System.Diagnostics;

    public class GetTemplatesStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Remove static logger
        // private static readonly ILogger _logger = Log.ForContext<GetTemplatesStep>();
        private static IEnumerable<Invoice> _allTemplates = null;

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
            string filePath = context?.FilePath ?? "Unknown";
            context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(Execute), "Get invoice templates for processing", $"FilePath: {filePath}");

            context.Logger?.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                nameof(GetTemplatesStep), $"Getting invoice templates for file: {filePath}");

            try
            {
                if (_allTemplates == null)
                {
                    context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                        nameof(Execute), "TemplateCheck", "Templates not loaded, calling GetTemplates.", "", "");
                    bool getTemplatesSuccess = await GetTemplates(context).ConfigureAwait(false); // Pass context
                    if (!getTemplatesSuccess)
                    {
                        methodStopwatch.Stop(); // Stop stopwatch on failure
                        context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                            nameof(Execute), "Get invoice templates for processing", methodStopwatch.ElapsedMilliseconds, "Failed to load templates.");
                        context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                            nameof(GetTemplatesStep), "Template loading", methodStopwatch.ElapsedMilliseconds, "Failed to load templates.");
                        return false; // Indicate step failure
                    }
                }
                else
                {
                    context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                        nameof(Execute), "TemplateCheck", "Templates already loaded, using cached templates.", "", "");
                }

                context.Templates = _allTemplates;

                methodStopwatch.Stop(); // Stop stopwatch on success
                context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(Execute), "Successfully retrieved invoice templates", $"TemplateCount: {context.Templates?.Count() ?? 0}", methodStopwatch.ElapsedMilliseconds);
                context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                    nameof(GetTemplatesStep), $"Successfully retrieved {context.Templates?.Count() ?? 0} invoice templates for file: {filePath}", methodStopwatch.ElapsedMilliseconds);
                return true; // Indicate step success
            }
            catch (Exception ex)
            {
                methodStopwatch.Stop(); // Stop stopwatch on exception
                string errorMessage = $"Unexpected error during GetTemplatesStep for file: {filePath}: {ex.Message}";
                context.Logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(Execute), "Get invoice templates for processing", methodStopwatch.ElapsedMilliseconds, errorMessage);
                context.Logger?.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(GetTemplatesStep), "Unexpected error during execution", methodStopwatch.ElapsedMilliseconds, errorMessage);
                context.AddError(errorMessage); // Add error to context
                return false; // Indicate step failure
            }
        }

        public static async Task<List<Invoice>> GetTemplates(InvoiceProcessingContext context,
            Expression<Func<Invoices, bool>> templateExpression)
        {
            var activeTemplatesQuery = GetActiveTemplatesQuery(new OCRContext(), templateExpression);
            var templates = await activeTemplatesQuery.ToListAsync().ConfigureAwait(false);
            return await GetContextTemplates(context, templates).ConfigureAwait(false);
        }


        private static async Task<bool> GetTemplates(InvoiceProcessingContext context)
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
            string filePath = context?.FilePath ?? "unknown";
            context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(GetTemplates), "Load invoice templates from database", $"FilePath: {filePath}");

            context.Logger?.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                nameof(GetTemplates), $"Loading invoice templates from database for file: {filePath}");

            try
            {
                // Always attempt to load templates, with special handling for Amazon
                context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(GetTemplates), "DatabaseAccess", "Attempting to access database for templates.", "", "");
                try
                {
                    using (var ctx = new OCRContext())
                    {
                        context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                            nameof(GetTemplates), "DatabaseConnection", "Database connection established.", $"Database: {ctx.Database.Connection.Database}, DataSource: {ctx.Database.Connection.DataSource}", "");

                        try
                        {
                            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                nameof(GetTemplates), "Querying", "Loading templates from database.", $"Database: {ctx.Database.Connection.Database}, Server: {ctx.Database.Connection.DataSource}, FilePath: {filePath}", "");

                            
                            // Load all active templates
                            context.Logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                                "GetActiveTemplatesQuery", "SYNC_EXPECTED"); // Log before query execution
                            var queryStopwatch = Stopwatch.StartNew(); // Start stopwatch for query
                            var activeTemplatesQuery = GetActiveTemplatesQuery(ctx, x => true);
                            var templates = activeTemplatesQuery.ToList(); // Execute query
                            queryStopwatch.Stop(); // Stop stopwatch after query
                            context.Logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                                "GetActiveTemplatesQuery", queryStopwatch.ElapsedMilliseconds, "Sync call returned"); // Log after query execution


                            if (templates.Any())
                            {
                                context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                    nameof(GetTemplates), "QueryResults", "Found active templates.", $"Count: {templates.Count}", "");
                                foreach (var t in templates)
                                {
                                    context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                        nameof(GetTemplates), "TemplateDetails", "Template details.", $"TemplateId: {t.Id}, TemplateName: {t.Name ?? "null"}, PartCount: {t.Parts?.Count ?? 0}, IsActive: {t.IsActive}", "");
                                }
                                context.Logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                                    "GetContextTemplates", "ASYNC_EXPECTED"); // Log before GetContextTemplates
                                var contextTemplatesStopwatch = Stopwatch.StartNew(); // Start stopwatch
                                context.Templates = await GetContextTemplates(context, templates).ConfigureAwait(false);
                                contextTemplatesStopwatch.Stop(); // Stop stopwatch
                                context.Logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                                    "GetContextTemplates", contextTemplatesStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return"); // Log after GetContextTemplates

                                _allTemplates = context.Templates;

                                methodStopwatch.Stop(); // Stop stopwatch on success
                                context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                                    nameof(GetTemplates), "Successfully loaded templates from database", $"TemplateCount: {_allTemplates?.Count() ?? 0}", methodStopwatch.ElapsedMilliseconds);
                                context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                                    nameof(GetTemplates), $"Successfully loaded {_allTemplates?.Count() ?? 0} templates from database for file: {filePath}", methodStopwatch.ElapsedMilliseconds);
                                return true;
                            }

                            context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                nameof(GetTemplates), "QueryResults", "No active templates found in database.", "", "");
                            
                            // Diagnostic check - see if there are any templates at all
                            context.Logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                                "Check for any templates", "ASYNC_EXPECTED"); // Log before AnyAsync
                            var anyTemplatesStopwatch = Stopwatch.StartNew(); // Start stopwatch
                            var anyTemplates = await ctx.Invoices.AnyAsync().ConfigureAwait(false);
                            anyTemplatesStopwatch.Stop(); // Stop stopwatch
                            context.Logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                                "Check for any templates", anyTemplatesStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return"); // Log after AnyAsync

                            if (!anyTemplates)
                            {
                                context.Logger?.Error("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                    nameof(GetTemplates), "DiagnosticCheck", "No templates exist in the database at all.", "", "");
                            }
                            else
                            {
                                context.Logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                                    "Count inactive templates", "ASYNC_EXPECTED"); // Log before CountAsync
                                var inactiveCountStopwatch = Stopwatch.StartNew(); // Start stopwatch
                                var inactiveCount = await ctx.Invoices.CountAsync(x => !x.IsActive).ConfigureAwait(false);
                                inactiveCountStopwatch.Stop(); // Stop stopwatch
                                context.Logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                                    "Count inactive templates", inactiveCountStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return"); // Log after CountAsync
                                context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                    nameof(GetTemplates), "DiagnosticCheck", "Found inactive templates in database.", $"Count: {inactiveCount}", "");
                            }

                            context.Templates = new List<Invoice>();

                            methodStopwatch.Stop(); // Stop stopwatch on failure
                            context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                                nameof(GetTemplates), "Load invoice templates from database", methodStopwatch.ElapsedMilliseconds, "No active templates found.");
                            context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                                nameof(GetTemplates), "Query results", methodStopwatch.ElapsedMilliseconds, "No active templates found in database.");
                            return false;
                        }
                        catch (Exception ex)
                        {
                            methodStopwatch.Stop(); // Stop stopwatch on exception
                            string errorMessage = $"Failed to load templates from database. Connection string: {ctx.Database.Connection.ConnectionString}. Error: {ex.Message}";
                            context.Logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                                nameof(GetTemplates), "Load invoice templates from database", methodStopwatch.ElapsedMilliseconds, errorMessage);
                            context.Logger?.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                                nameof(GetTemplates), "Database query", methodStopwatch.ElapsedMilliseconds, errorMessage);
                            context.Templates = new List<Invoice>();
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    methodStopwatch.Stop(); // Stop stopwatch on exception
                    string errorMessage = $"Error loading templates from database: {ex.Message}";
                    context.Logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                        nameof(GetTemplates), "Load invoice templates from database", methodStopwatch.ElapsedMilliseconds, errorMessage);
                    context.Logger?.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                        nameof(GetTemplates), "Database access", methodStopwatch.ElapsedMilliseconds, errorMessage);
                    return false;
                }

                // No templates found in primary loading method - this part of the code is unreachable
                // as the inner try-catch blocks always return.
            }
            catch (Exception ex)
            {
                methodStopwatch.Stop(); // Stop stopwatch on exception
                string errorMessage = $"Failed to load templates for file: {filePath}: {ex.Message}";
                context.Logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(GetTemplates), "Load invoice templates from database", methodStopwatch.ElapsedMilliseconds, errorMessage);
                context.Logger?.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(GetTemplates), "Unexpected error during execution", methodStopwatch.ElapsedMilliseconds, errorMessage);
                context.Templates = new List<Invoice>();
                return false;
            }
        }

        private static async Task<List<Invoice>> GetContextTemplates(InvoiceProcessingContext context, List<Invoices> templates)
        {
            var docSet = context.DocSet ?? await WaterNut.DataSpace.Utils.GetDocSets(context.FileType).ConfigureAwait(false);
            return templates.Select(x => new Invoice(x){
                FileType = context.FileType,
                DocSet = docSet,
                FilePath = context.FilePath,
                EmailId = context.EmailId,

            }).ToList();
        }

        public static IQueryable<Invoices> GetActiveTemplatesQuery(OCRContext ctx, Expression<Func<Invoices, bool>> exp)
        {
            var activeTemplatesQuery = ctx.Invoices
                .Include(x => x.Parts)
                .Include("InvoiceIdentificatonRegEx.OCR_RegularExpressions")
                .Include("RegEx.RegEx")
                .Include("RegEx.ReplacementRegEx")
                .Include("Parts.RecuringPart")
                .Include("Parts.Start.RegularExpressions")
                .Include("Parts.End.RegularExpressions")
                .Include("Parts.PartTypes")
                .Include("Parts.ChildParts.ChildPart.Start.RegularExpressions")
                .Include("Parts.ParentParts.ParentPart.Start.RegularExpressions")
                .Include("Parts.Lines.RegularExpressions")
                .Include("Parts.Lines.Fields.FieldValue")
                .Include("Parts.Lines.Fields.FormatRegEx.RegEx")
                .Include("Parts.Lines.Fields.FormatRegEx.ReplacementRegEx")
                .Include("Parts.Lines.Fields.ChildFields.FieldValue")
                .Include("Parts.Lines.Fields.ChildFields.FormatRegEx.RegEx")
                .Include("Parts.Lines.Fields.ChildFields.FormatRegEx.ReplacementRegEx")
                .Where(x => x.IsActive)
                .Where(x => x.ApplicationSettingsId ==
                                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                .Where(exp);
            return activeTemplatesQuery;
        }

        /*
        private static async Task<List<Template>> GetInvoiceTemplatesAsync()
        {
            _logger.Debug("Loading invoice templates from database");

            try
            {
                using (var ctx = new OCRContext())
                {
                    // Log database connection info
                    _logger.Information($"Database: {ctx.Database.Connection.Database}");
                    _logger.Information($"DataSource: {ctx.Database.Connection.DataSource}");

                    // Get and log all available templates
                    var allTemplates = await ctx.Invoices
                        .Include(x => x.Parts)
                        .ToListAsync()
                        .ConfigureAwait(false);

                    _logger.Information("Available invoice templates:");
                    foreach (var template in allTemplates)
                    {
                        _logger.Information($"- ID: {template.Id}, Name: {template.Name}, Active: {template.IsActive}, Parts: {template.Parts?.Count ?? 0}");
                    }

                    // Return only active templates
                    return allTemplates
                        .Where(x => x.IsActive)
                        .Select(x => new Template(x))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error loading templates from database");
                return new List<Template>();
            }
        }
*/
    }
}