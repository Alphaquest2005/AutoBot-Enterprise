using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
    public class GetTemplatesStep : IPipelineStep<InvoiceProcessingContext>
    {
        private static readonly ILogger _logger = Log.ForContext<GetTemplatesStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "unknown";
            _logger.Debug("Starting GetTemplatesStep for file: {FilePath}", filePath);

            try
            {
                // Always attempt to load templates, with special handling for Amazon
                _logger.Information("Loading invoice templates from database");
                try
                {
                    using (var ctx = new OCRContext())
                    {
                        _logger.Information($"Database: {ctx.Database.Connection.Database}");
                        _logger.Information($"DataSource: {ctx.Database.Connection.DataSource}");

                        try
                        {
                            _logger.Information("Loading templates from database: {Database} on server: {Server} for file: {FilePath}",
                                ctx.Database.Connection.Database, ctx.Database.Connection.DataSource, filePath);

                            
                            // Load all active templates
                            _logger.Information("Querying for all active templates");
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
                                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                                // .Where(filter) //BaseDataModel.Instance.CurrentApplicationSettings.TestMode != true ||
                                

                            _logger.Debug("Active templates query: {Query}", activeTemplatesQuery.ToString());
                            
                            var templates = await activeTemplatesQuery.ToListAsync().ConfigureAwait(false);

                            if (templates.Any())
                            {
                                _logger.Information("Found {Count} active templates:", templates.Count);
                                foreach (var t in templates)
                                {
                                    _logger.Information("- ID: {Id}, Name: {Name}, Parts: {PartCount}, IsActive: {IsActive}",
                                        t.Id, t.Name ?? "null", t.Parts?.Count ?? 0, t.IsActive);
                                }
                                context.Templates = templates.Select(x => new Invoice(x){FileType = context.FileType,
                                    DocSet = WaterNut.DataSpace.Utils.GetDocSets(context.FileType),
                                    FilePath = context.FilePath,
                                    EmailId = context.EmailId,

                                }).ToList();
                                return true;
                            }

                            _logger.Error("No active templates found in database. Checking if any inactive templates exist...");
                            
                            // Diagnostic check - see if there are any templates at all
                            var anyTemplates = await ctx.Invoices.AnyAsync().ConfigureAwait(false);
                            if (!anyTemplates)
                            {
                                _logger.Error("No templates exist in the database at all");
                            }
                            else
                            {
                                var inactiveCount = await ctx.Invoices.CountAsync(x => !x.IsActive).ConfigureAwait(false);
                                _logger.Warning("Found {Count} inactive templates in database", inactiveCount);
                            }

                            context.Templates = new List<Invoice>();
                            return false;
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, "Failed to load templates from database. Connection string: {ConnectionString}",
                                ctx.Database.Connection.ConnectionString);
                            context.Templates = new List<Invoice>();
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error loading templates from database");
                    return false;
                }

                // No templates found in primary loading method
                _logger.Warning("No templates found in primary loading method");
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load templates for file: {FilePath}", filePath);
                context.Templates = new List<Invoice>();
                return false;
            }
        }

        private static async Task<List<Invoice>> GetInvoiceTemplatesAsync()
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
                        .Select(x => new Invoice(x))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error loading templates from database");
                return new List<Invoice>();
            }
        }
    }
}