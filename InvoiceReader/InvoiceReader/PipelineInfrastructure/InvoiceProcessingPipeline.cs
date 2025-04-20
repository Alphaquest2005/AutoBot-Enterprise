// Assuming PipelineRunner is here
// Assuming ImportStatus is here

using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class InvoiceProcessingPipeline
    {
        // Add a static logger instance for this class
        private static readonly ILogger _logger = Log.ForContext<InvoiceProcessingPipeline>();

        private readonly InvoiceProcessingContext _context;
        private readonly bool _isLastTemplate;

        public InvoiceProcessingPipeline(InvoiceProcessingContext context, bool isLastTemplate)
        {
            _context = context;
            _isLastTemplate = isLastTemplate;
             // Log initialization with context details
             string filePath = _context?.FilePath ?? "Unknown";
             int? templateId = _context?.Template?.OcrInvoices?.Id; // Template might be null initially
             _logger.Debug("InvoiceProcessingPipeline initialized for File: {FilePath}, IsLastTemplate: {IsLastTemplate}, Initial TemplateId: {TemplateId}",
                filePath, _isLastTemplate, templateId);
        }
    }
}