using System;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using OCR.Business.Entities;
using Serilog;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Utility to delete the existing MANGO template (ID: 1329) with poor patterns
    /// and force recreation with enhanced AITemplateService patterns.
    /// </summary>
    public class FixMangoTemplate
    {
        public static async Task DeleteExistingMangoTemplate()
        {
            var logger = Log.ForContext<FixMangoTemplate>();
            
            try
            {
                using (var context = new OCRContext())
                {
                    // Find existing MANGO template
                    var mangoTemplate = await context.Templates
                        .Where(i => i.Name == "MANGO" || i.Id == 1329)
                        .FirstOrDefaultAsync();
                    
                    if (mangoTemplate != null)
                    {
                        logger.Information("🗑️ **DELETING_BAD_TEMPLATE**: Found MANGO template ID {TemplateId}, deleting to force recreation", mangoTemplate.Id);
                        
                        // Delete related records first (cascade delete might not be configured)
                        var parts = context.Parts.Where(p => p.TemplateId == mangoTemplate.Id);
                        var lines = context.Lines.Where(l => parts.Any(p => p.Id == l.PartId));
                        var fields = context.Fields.Where(f => lines.Any(l => l.Id == f.LineId));
                        var regex = context.RegularExpressions.Where(r => lines.Any(l => l.RegExId == r.Id));
                        
                        // Delete in order: regex -> fields -> lines -> parts -> invoice
                        context.RegularExpressions.RemoveRange(regex);
                        context.Fields.RemoveRange(fields);
                        context.Lines.RemoveRange(lines);
                        context.Parts.RemoveRange(parts);
                        context.Templates.Remove(mangoTemplate);
                        
                        await context.SaveChangesAsync();
                        
                        logger.Information("✅ **TEMPLATE_DELETED**: MANGO template and all related records deleted successfully");
                        logger.Information("🔄 **NEXT_TEST_WILL**: Create fresh template with enhanced AITemplateService patterns");
                    }
                    else
                    {
                        logger.Information("ℹ️ **NO_TEMPLATE_FOUND**: No existing MANGO template found to delete");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "❌ **DELETE_FAILED**: Failed to delete MANGO template");
                throw;
            }
        }
    }
}