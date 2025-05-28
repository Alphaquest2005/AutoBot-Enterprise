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
        private readonly ILogger _logger; // Use instance logger

        private readonly InvoiceProcessingContext _context;
        private readonly bool _isLastTemplate;

        public InvoiceProcessingPipeline(InvoiceProcessingContext context, bool isLastTemplate, ILogger logger)
        {
            _context = context;
            _isLastTemplate = isLastTemplate;
            _logger = logger; // Assign the passed logger
             // Log initialization with context details
             string filePath = _context?.FilePath ?? "Unknown";
             _logger.Debug("InvoiceProcessingPipeline initialized for File: {FilePath}, IsLastTemplate: {IsLastTemplate}",
                filePath, _isLastTemplate);
        }
    }
}