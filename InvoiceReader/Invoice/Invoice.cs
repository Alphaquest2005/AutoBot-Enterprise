using Core.Common.Extensions;
using EntryDataDS.Business.Entities;
using Invoices = OCR.Business.Entities.Invoices;
using MoreEnumerable = MoreLinq.MoreEnumerable;
using OCR.Business.Entities; // Added for Part, Line
using System.Collections.Generic; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added
using Core.Common;
using DocumentDS.Business.Entities;
using FileTypes = CoreEntities.Business.Entities.FileTypes; // Added for BetterExpando

namespace WaterNut.DataSpace
{
    public partial class Invoice
    {
        // Define logger instance here
        private static readonly ILogger _logger = Log.ForContext<Invoice>();

        private EntryData EntryData { get; } = new EntryData(); // Simple initialization, no logging needed
        public Invoices OcrInvoices { get; }
        public List<Part> Parts { get; set; } // Set in constructor

        public List<AsycudaDocumentSet> DocSet { get; set; }
        public string EmailId { get; set; }
        public bool OverWriteExisting { get; set; }
        public string FilePath { get; set; }


      //  private ImportStatus _importStatus;
      public ImportStatus ImportStatus // Added ImportStatus property
      {
          get
          {
              if (this.Parts?.All(x => x != null && x.Success) ?? true) return ImportStatus.Success;
              if (this.Parts.All(x => x != null && !x.Success)) return ImportStatus.Failed;
              return ImportStatus.HasErrors;

          }
      }

      private string _formattedPdfText = string.Empty;
        public string FormattedPdfText // Added FormattedPdfText property
        {
            get => _formattedPdfText;
            set
            {
                if (_formattedPdfText != value)
                {
                    _formattedPdfText = value;
                    _logger.Debug("Template Property Changed: FormattedPdfText. Length = {NewValueLength}", value?.Length ?? 0);
                }
            }
        }

        private List<dynamic> _csvLines;
        public List<dynamic> CsvLines // Added CsvLines property
        {
            get => _csvLines;
            set
            {
                if (_csvLines != value)
                {
                    _csvLines = value;
                    _logger.Debug("Template Property Changed: CsvLines. Count = {NewValueCount}", value?.Count ?? 0);
                }
            }
        }

        private List<Line> _failedLines;
        public List<Line> FailedLines // Added FailedLines property
        {
            get => _failedLines;
            set
            {
                if (_failedLines != value)
                {
                    _failedLines = value;
                    _logger.Debug("Template Property Changed: FailedLines. Count = {NewValueCount}", value?.Count ?? 0);
                }
            }
        }

        // Added logging to property getters as they contain logic
        public bool Success
        {
            get
            {
                 int? invoiceId = this.OcrInvoices?.Id;
                 _logger.Verbose("Evaluating Invoice Success for InvoiceId: {InvoiceId}", invoiceId);
                 // Added null check for Parts collection itself and individual parts
                 bool success = this.Parts?.All(x => x != null && x.Success) ?? true; // Default to true if Parts is null? Or false? Assuming true.
                 _logger.Verbose("Invoice Success evaluation result: {Success}. Part Count: {PartCount}", success, this.Parts?.Count ?? 0);
                 return success;
            }
        }
        public List<Line> Lines
        {
             get
             {
                  int? invoiceId = this.OcrInvoices?.Id;
                  _logger.Verbose("Evaluating Invoice Lines property for InvoiceId: {InvoiceId}", invoiceId);
                  List<Line> lines = null;
                  try
                  {
                       // Added comprehensive null checks
                       lines = MoreEnumerable.DistinctBy(
                                    this.Parts?.Where(p => p != null) // Filter null parts
                                           .SelectMany(x => x.AllLines ?? Enumerable.Empty<Line>()) // Safe navigation for AllLines
                                           ?? Enumerable.Empty<Line>(), // Handle null Parts collection
                                    x => x?.OCR_Lines?.Id // Use safe navigation for key selector
                                )
                                .Where(l => l?.OCR_Lines != null) // Filter out lines where key selector resulted in null
                                .ToList();
                        _logger.Verbose("Generated {LineCount} distinct lines for InvoiceId: {InvoiceId}", lines.Count, invoiceId);
                  }
                  catch (Exception ex)
                  {
                       _logger.Error(ex, "Error evaluating Lines property for InvoiceId: {InvoiceId}", invoiceId);
                       lines = new List<Line>(); // Return empty list on error
                  }
                  return lines;
             }
        }


        public Invoice(Invoices ocrInvoices)
        {
             int? invoiceId = ocrInvoices?.Id;
             _logger.Debug("Constructing Invoice object for OcrInvoices Id: {InvoiceId}", invoiceId);

             // Null check input
             if (ocrInvoices == null)
             {
                  _logger.Error("Invoice constructor called with null OcrInvoices object. Cannot initialize.");
                  throw new ArgumentNullException(nameof(ocrInvoices), "OcrInvoices object cannot be null.");
             }

            OcrInvoices = ocrInvoices;
             _logger.Verbose("Assigned OcrInvoices (Id: {InvoiceId}) to Invoice property.", invoiceId);

            // Initialize Parts collection safely
            try
            {
                 _logger.Debug("Filtering and selecting Parts for InvoiceId: {InvoiceId}", invoiceId);
                 if (ocrInvoices.Parts == null)
                 {
                      _logger.Warning("OcrInvoices Id: {InvoiceId} has a null Parts collection. Invoice will have no parts.", invoiceId);
                      this.Parts = new List<Part>(); // Initialize to empty list
                 }
                 else
                 {
                     _logger.Verbose("Filtering criteria: Part is not null AND ((Has ParentParts AND No ChildParts) OR (No ParentParts AND No ChildParts))");
                     // Added null checks within LINQ for safety
                     this.Parts = ocrInvoices.Parts
                         .Where(x => x != null && // Check part is not null
                                     ( (x.ParentParts != null && x.ParentParts.Any() && (x.ChildParts == null || !x.ChildParts.Any())) || // Condition 1 (safe checks)
                                       ((x.ParentParts == null || !x.ParentParts.Any()) && (x.ChildParts == null || !x.ChildParts.Any())) ) // Condition 2 (safe checks)
                               )
                         .Select(z => {
                              _logger.Verbose("Creating Part object for OCR_Part Id: {PartId}", z.Id);
                              // Assuming Part constructor handles potential errors/logging
                              return new Part(z);
                              })
                         .ToList();
                 }
                  _logger.Information("Initialized InvoiceId: {InvoiceId} with {PartCount} selected Parts.", invoiceId, this.Parts?.Count ?? 0);
            }
            catch (Exception ex)
            {
                 _logger.Error(ex, "Error during Parts initialization in Invoice constructor for OcrInvoices Id: {InvoiceId}", invoiceId);
                 this.Parts = new List<Part>(); // Ensure Parts is not null even on error
                 // throw; // Optionally re-throw
            }
        }

        // Static members - logging might be less relevant unless initialization is complex
        private static readonly Dictionary<string, string> Sections = new Dictionary<string, string>()
        {
            { "Single", "---Single Column---" },
            { "Sparse", "---SparseText---" },
            { "Ripped", "---Ripped Text---" }
        };

        // MaxLinesCheckedToStart might be logged when it's used, rather than here
        public double MaxLinesCheckedToStart { get; set; } = 0.5;

        private FileTypes _fileType;
        public FileTypes FileType
        {
            get => _fileType;
            set
            {
                if (_fileType != value)
                {
                    _fileType = value;
                    _logger.Debug("Template Property Changed: FileType = {NewValue}", value?.Description ?? "null");
                }
            }
        }


        // 'table' is used by CreateOrGetDitm - logging added there. Initialization here is simple.
        private static readonly Dictionary<string, List<BetterExpando>> table = new Dictionary<string, List<BetterExpando>>();


    }
}