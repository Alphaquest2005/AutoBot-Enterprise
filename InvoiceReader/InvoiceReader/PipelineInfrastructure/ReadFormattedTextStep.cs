// Assuming this is needed for _template.Read

using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System;
using System.Runtime.InteropServices; // Added
using OCR.Business.Entities; // Added for Invoice
using Core.Common.Extensions; // Added for BetterExpando

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class ReadFormattedTextStep : IPipelineStep<InvoiceProcessingContext>
    {
        private static readonly ILogger _logger = Log.ForContext<ReadFormattedTextStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "Unknown";
            var res = false;
            foreach (var template in context.Templates)
            {


                try
                {
                    if (!ExecutionValidation(template, filePath))
                    {
                        res = false;
                        continue;
                    }

                    var textLines = GetTextLinesFromFormattedPdfText(template, filePath);

                    ////////////////////////////////////////////////////////////

                    template.CsvLines = template.Read(textLines);

                    ////////////////////////////////////////////////////////////

                    res =  ExecutionSuccess(template, filePath);
                    
                }
                catch (Exception ex)
                {
                    LogExecutionError(ex, filePath, template.OcrInvoices.Id);
                    if (context != null) template.CsvLines = null;
                    
                }
            }
            return res; // Default return if no templates processed
        }

        private bool ExecutionValidation(Invoice template, string filePath)
        {
            LogExecutionStart(filePath, template.OcrInvoices.Id, template.OcrInvoices.Name);

            
           
            if (string.IsNullOrEmpty(template.FormattedPdfText))
            {
                LogEmptyFormattedPdfTextWarning(filePath, template.OcrInvoices.Id);
                return false;
            }

            return true;
        }

        private bool ExecutionSuccess(Invoice template, string filePath)
        {
            LogTemplateReadFinished(filePath, template.OcrInvoices.Id, template.CsvLines?.Count ?? 0);

            if (template.CsvLines == null || !template.CsvLines.Any())
            {
                LogEmptyCsvLinesWarning(filePath, template.OcrInvoices.Id);
                return false;
            }

            LogExecutionSuccess(filePath, template.OcrInvoices.Id);
            return true;
        }

        private List<string> GetTextLinesFromFormattedPdfText(Invoice template, string filePath)
        {
            LogDataExtractionStart(filePath, template.OcrInvoices.Id);
            LogFormattedPdfText(template.FormattedPdfText);

            if (template?.OcrInvoices?.Parts != null)
            {
                LogTemplateRegexPatterns(template.OcrInvoices.Parts);
            }

            var textLines = template.FormattedPdfText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
            LogSplitTextLines(textLines.Count);

            var topLevelParts = template.OcrInvoices.Parts
                .Where(p => (p.ParentParts.Any() && !p.ChildParts.Any()) ||
                            (!p.ParentParts.Any() && !p.ChildParts.Any()))
                .ToList();

            LogTopLevelPartsIdentified(topLevelParts.Count);
            LogCallingTemplateRead(textLines.Count, filePath, template.OcrInvoices.Id);
            return textLines;
        }

        private void LogExecutionStart(string filePath, int? templateId, string templateName)
        {
            _logger.Debug("Executing ReadFormattedTextStep for File: {FilePath}, TemplateId: {TemplateId}, TemplateName: '{TemplateName}'", filePath, templateId, templateName);
        }

        private void LogNullContextError()
        {
            _logger.Error("ReadFormattedTextStep executed with null context.");
        }

        private void LogNullTemplateWarning(string filePath)
        {
            _logger.Warning("Skipping ReadFormattedTextStep: Template is null for File: {FilePath}", filePath);
        }

        private void LogEmptyFormattedPdfTextWarning(string filePath, int? templateId)
        {
            _logger.Warning("Skipping ReadFormattedTextStep: FormattedPdfText is null or empty for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
        }

        private void LogDataExtractionStart(string filePath, int? templateId)
        {
            _logger.Debug("Starting data extraction using template for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
        }

        private void LogFormattedPdfText(string formattedPdfText)
        {
            _logger.Verbose("FormattedPdfText:\n{FormattedPdfText}", formattedPdfText);
        }

        private void LogTemplateRegexPatterns(List<Parts> parts)
        {
            _logger.Verbose("Template Regex Patterns:");
            foreach (var part in parts)
            {
                if (part.Lines != null)
                {
                    foreach (var line in part.Lines)
                    {
                        if (line.RegularExpressions != null)
                        {
                            _logger.Verbose("  PartId: {PartId}, Line: {LineName}, Regex: {RegexPattern}",
                                part.Id, line.Name ?? "Unknown", line.RegularExpressions.RegEx);
                        }
                    }
                }
            }
        }

        private void LogSplitTextLines(int lineCount)
        {
            _logger.Verbose("Split FormattedPdfText into {LineCount} lines.", lineCount);
        }

        private void LogTopLevelPartsIdentified(int topLevelPartCount)
        {
            _logger.Verbose("Identified {TopLevelPartCount} top-level parts from template.", topLevelPartCount);
        }

        private void LogCallingTemplateRead(int lineCount, string filePath, int? templateId)
        {
            _logger.Debug("Calling context.Template.Read with {LineCount} lines for File: {FilePath}, TemplateId: {TemplateId}", lineCount, filePath, templateId);
        }

        private void LogTemplateReadFinished(string filePath, int? templateId, int resultCount)
        {
            _logger.Debug("context.Template.Read finished for File: {FilePath}, TemplateId: {TemplateId}. Result count: {ResultCount}", filePath, templateId, resultCount);
        }

        private void LogEmptyCsvLinesWarning(string filePath, int? templateId)
        {
            _logger.Warning("CsvLines is null or empty after extraction attempt for File: {FilePath}, TemplateId: {TemplateId}. Step fails.", filePath, templateId);
        }

        private void LogExecutionSuccess(string filePath, int? templateId)
        {
            _logger.Information("ReadFormattedTextStep finished for File: {FilePath}, TemplateId: {TemplateId}. Step Success: true.", filePath, templateId);
        }

        private void LogExecutionError(Exception ex, string filePath, int? templateId)
        {
            _logger.Error(ex, "Error during ReadFormattedTextStep for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
        }
    }
}