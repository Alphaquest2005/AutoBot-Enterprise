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
using System.Data.Entity.Core.Objects; // For MergeOption
using System.Data.Entity.Infrastructure; // For IObjectContextAdapter

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

        public static async Task<List<Invoice>> GetTemplates(InvoiceProcessingContext context, Func<Invoice, bool> templateExpression)
        {
            var templates = _allTemplates.Where(templateExpression).ToList();
            return await GetContextTemplates(context, templates).ConfigureAwait(false);//GetActiveTemplatesQuery(new OCRContext(), templateExpression);
        }

        private static async Task<List<Invoice>> GetContextTemplates(InvoiceProcessingContext context, List<Invoice> templates)
        {
            var docSet = context.DocSet ?? await WaterNut.DataSpace.Utils.GetDocSets(context.FileType, context.Logger).ConfigureAwait(false);
            return templates.Select(x =>
                {
                    x.FileType = context.FileType;
                    x.DocSet = docSet;
                    x.FilePath = context.FilePath;
                    x.EmailId = context.EmailId;

                    return x;
                }).ToList();
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

                            
                            var templates = GetAllTemplates(context, ctx);

                            if (templates.Any())
                            {
                                context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                    nameof(GetTemplates), "QueryResults", "Found active templates.", $"Count: {templates.Count()}", "");
                                foreach (var t in templates)
                                {
                                    context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                        nameof(GetTemplates), "TemplateDetails", "Template details.", $"TemplateId: {t.OcrInvoices.Id}, TemplateName: {t.OcrInvoices.Name ?? "null"}, PartCount: {t.Parts?.Count ?? 0}, IsActive: {t.OcrInvoices.IsActive}", "");
                                }

                                /////////////////////////////////// Move this step to possible invoices because we only getting the invoices once!
                                //context.Logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                                //    "GetContextTemplates", "ASYNC_EXPECTED"); // Log before GetContextTemplates
                                //var contextTemplatesStopwatch = Stopwatch.StartNew(); // Start stopwatch

                                //context.Templates = await GetContextTemplates(context, templates).ConfigureAwait(false);
                                
                                //contextTemplatesStopwatch.Stop(); // Stop stopwatch
                                //context.Logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                                //    "GetContextTemplates", contextTemplatesStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return"); // Log after GetContextTemplates

                                context.Templates = templates;
                                _allTemplates = templates;

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

        public static IEnumerable<Invoice> GetAllTemplates(InvoiceProcessingContext context, OCRContext ctx)
        {
            // Load all active templates
            context.Logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                "GetActiveTemplatesQuery", "SYNC_EXPECTED"); // Log before query execution
            var queryStopwatch = Stopwatch.StartNew(); // Start stopwatch for query

            // Disable lazy loading and proxy creation to prevent "already been loaded" errors with complex object graphs
            //var initialLazyLoadingEnabled = ctx.Configuration.LazyLoadingEnabled;
            //var initialProxyCreationEnabled = ctx.Configuration.ProxyCreationEnabled;
            //ctx.Configuration.LazyLoadingEnabled = false;
            //ctx.Configuration.ProxyCreationEnabled = false;

            var activeTemplatesQuery = GetActiveTemplatesQuery(ctx, x => true);
            var templates = activeTemplatesQuery.Select(x => new Invoice(x)).ToList(); // activeTemplatesQuery is already a List

           


            //// Re-enable lazy loading and proxy creation
            //ctx.Configuration.LazyLoadingEnabled = initialLazyLoadingEnabled;
            //ctx.Configuration.ProxyCreationEnabled = initialProxyCreationEnabled;
            queryStopwatch.Stop(); // Stop stopwatch after query
            context.Logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                "GetActiveTemplatesQuery", queryStopwatch.ElapsedMilliseconds, "Sync call returned"); // Log after query execution
            return templates;
        }



        public static List<Invoices> GetActiveTemplatesQuery(OCRContext ctx, Expression<Func<Invoices, bool>> exp)
        {

            var activeTemplates = ctx.Invoices
                .AsNoTracking()
                .Include(x => x.Parts)
                .Include("InvoiceIdentificatonRegEx.OCR_RegularExpressions")
                .Include("RegEx.RegEx")
                .Include("RegEx.ReplacementRegEx")
                .Include("Parts.RecuringPart")
                .Include("Parts.Start.RegularExpressions")
                .Include("Parts.End.RegularExpressions")
                .Include("Parts.PartTypes")
                //.Include("Parts.ChildParts.ChildPart.Start.RegularExpressions") // Commented out as per user instruction
                //.Include("Parts.ParentParts.ParentPart.Start.RegularExpressions") // Commented out as per user instruction
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
                .Where(exp)
                .ToList(); // Materialize the query to a List
                           // 2. Create a dictionary of ALL UNIQUE Part instances that were initially loaded.
                           // These are our canonical Part objects.
            if (!activeTemplates.Any()) return activeTemplates;

            // --- Store original top-level part IDs for each invoice ---
            // This is crucial to know which parts should be directly under each invoice later.
            var invoiceOriginalPartIds = activeTemplates.ToDictionary(
                inv => inv.Id, // Assuming Invoice has an Id
                inv => inv.Parts?.Select(p => p.Id).ToList() ?? new List<int>()
            );

            // 2. Populate canonicalParts dictionary (Your existing Step 2 code)
            var canonicalParts = new Dictionary<int, Parts>();
            foreach (var invoice in activeTemplates)
            {
                if (invoice.Parts != null)
                {
                    foreach (var part in invoice.Parts)
                    {
                        if (part != null && !canonicalParts.ContainsKey(part.Id))
                        {
                            part.ChildParts = new List<ChildParts>(); // Initialize collections
                            part.ParentParts = new List<ChildParts>();
                            canonicalParts.Add(part.Id, part);
                        }
                        // If a part instance was already added from another invoice (unlikely if parts are distinct per invoice initially),
                        // the existing instance in canonicalParts is fine.
                    }
                }
            }

            if (!canonicalParts.Any())
            {
                // If no parts were loaded at all, clear any potentially empty Parts collections on invoices
                // (though they should be from the .Include if parts existed)
                foreach (var invoice in activeTemplates) { invoice.Parts = new List<Parts>(); }
                return activeTemplates;
            }

            var allInitiallyLoadedPartIds = canonicalParts.Keys.ToList();

            // 3. Load ALL ChildParts linking entities (Your existing Step 3 code)
            // We need to ensure we get all links that could form the hierarchy,
            // even if some parts in the hierarchy were not directly in `allInitiallyLoadedPartIds`.
            // This might require iteratively finding all related part IDs.

            var allRelevantPartIdsInHierarchy = new HashSet<int>(allInitiallyLoadedPartIds);
            bool newIdsAddedInIteration;
            int maxIterations = 10; // Safety break for very deep or circular structures
            int currentIteration = 0;

            do
            {
                newIdsAddedInIteration = false;
                currentIteration++;
                // Find IDs of parts that are linked to current 'allRelevantPartIdsInHierarchy' but not yet in it.
                var newlyDiscoveredRelatedPartIds = ctx.ChildParts
                    .AsNoTracking()
                    .Where(link => (allRelevantPartIdsInHierarchy.Contains(link.ParentPartId) && !allRelevantPartIdsInHierarchy.Contains(link.ChildPartId)) ||
                                   (allRelevantPartIdsInHierarchy.Contains(link.ChildPartId) && !allRelevantPartIdsInHierarchy.Contains(link.ParentPartId)))
                    .SelectMany(link => new[] { link.ParentPartId, link.ChildPartId })
                    .Distinct()
                    .ToList();

                foreach (var id in newlyDiscoveredRelatedPartIds)
                {
                    if (allRelevantPartIdsInHierarchy.Add(id)) // .Add returns true if the item was new
                    {
                        newIdsAddedInIteration = true;
                    }
                }
            } while (newIdsAddedInIteration && currentIteration < maxIterations);


            List<ChildParts> allLinks = ctx.ChildParts
                .AsNoTracking()
                // Use the fully discovered set of relevant part IDs for fetching links
                .Where(link => allRelevantPartIdsInHierarchy.Contains(link.ParentPartId) && allRelevantPartIdsInHierarchy.Contains(link.ChildPartId))
                .Include(link => link.ChildPart.Start.Select(s => s.RegularExpressions))
                .Include(link => link.ChildPart.End.Select(s => s.RegularExpressions))
                .Include(link => link.ParentPart.Start.Select(s => s.RegularExpressions))
                .Include(link => link.ParentPart.End.Select(s => s.RegularExpressions))
                .ToList();

            // 4. Process links to build hierarchy in canonicalParts (Your existing Step 4 code)
            foreach (var linkFromServer in allLinks)
            {
                Parts canonicalParent = null;
                Parts canonicalChild = null;

                if (!canonicalParts.TryGetValue(linkFromServer.ParentPartId, out canonicalParent))
                {
                    if (linkFromServer.ParentPart != null)
                    {
                        canonicalParent = linkFromServer.ParentPart;
                        canonicalParent.ChildParts = new List<ChildParts>();
                        canonicalParent.ParentParts = new List<ChildParts>();
                        canonicalParts.Add(canonicalParent.Id, canonicalParent);
                    }
                }

                if (!canonicalParts.TryGetValue(linkFromServer.ChildPartId, out canonicalChild))
                {
                    if (linkFromServer.ChildPart != null)
                    {
                        canonicalChild = linkFromServer.ChildPart;
                        canonicalChild.ChildParts = new List<ChildParts>();
                        canonicalChild.ParentParts = new List<ChildParts>();
                        canonicalParts.Add(canonicalChild.Id, canonicalChild);
                    }
                }

                if (canonicalParent != null && canonicalChild != null)
                {
                    ChildParts currentLink = linkFromServer;
                    currentLink.ParentPart = canonicalParent;
                    currentLink.ChildPart = canonicalChild;

                    if (canonicalParent.ChildParts.All(l => l.Id != currentLink.Id))
                    {
                        canonicalParent.ChildParts.Add(currentLink);
                    }
                    if (canonicalChild.ParentParts.All(l => l.Id != currentLink.Id))
                    {
                        canonicalChild.ParentParts.Add(currentLink);
                    }
                }
            }

            // 5. Rebuild Invoice.Parts collections using canonical, fully-stitched Part objects.
            //    Only include parts that were originally direct children of the invoice AND
            //    are now considered "top-level" within that invoice's context (i.e., their ParentPart,
            //    if it exists, was NOT one of the other original parts of THIS invoice).

            foreach (var invoice in activeTemplates)
            {
                var newTopLevelInvoiceParts = new List<Parts>();
                if (invoiceOriginalPartIds.TryGetValue(invoice.Id, out var originalPartIdsForThisInvoiceSet))
                {
                    // Convert to HashSet for efficient lookups of original parts of this invoice
                    var originalPartIdsLookup = new HashSet<int>(originalPartIdsForThisInvoiceSet);

                    foreach (var partId in originalPartIdsForThisInvoiceSet)
                    {
                        if (canonicalParts.TryGetValue(partId, out var canonicalPartInstance))
                        {
                            // Check if this part is truly top-level for THIS invoice.
                            // A part is top-level if none of its ParentParts (linking entities)
                            // point to a ParentPart (actual Part entity) whose ID is also in
                            // originalPartIdsForThisInvoiceSet (excluding itself, though self-parenting is unlikely here).
                            bool isTopLevelForThisInvoice = true;
                            if (canonicalPartInstance.ParentParts != null)
                            {
                                foreach (var parentLink in canonicalPartInstance.ParentParts)
                                {
                                    // parentLink is a ChildParts (linking) entity
                                    // parentLink.ParentPart is the actual parent Part entity
                                    if (parentLink.ParentPart != null &&
                                        parentLink.ParentPart.Id != canonicalPartInstance.Id && // Not a self-reference check
                                        originalPartIdsLookup.Contains(parentLink.ParentPart.Id))
                                    {
                                        // This part has a parent that was ALSO an original part of this invoice.
                                        // Therefore, this part is not top-level for this invoice.
                                        isTopLevelForThisInvoice = false;
                                        break;
                                    }
                                }
                            }

                            if (isTopLevelForThisInvoice)
                            {
                                newTopLevelInvoiceParts.Add(canonicalPartInstance);
                            }
                        }
                    }
                }
                invoice.Parts = newTopLevelInvoiceParts; // Replace the old list with the new one.
            }

            return activeTemplates;
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