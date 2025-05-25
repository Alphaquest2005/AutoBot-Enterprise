using System.Diagnostics;
﻿using Core.Common.Extensions;
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
                    _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): Template Property Changed: FormattedPdfText. Length = {NewValueLength}", nameof(Invoice), "PropertySet", value?.Length ?? 0);
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
                    _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): Template Property Changed: CsvLines. Count = {NewValueCount}", nameof(Invoice), "PropertySet", value?.Count ?? 0);
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
                    _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): Template Property Changed: FailedLines. Count = {NewValueCount}", nameof(Invoice), "PropertySet", value?.Count ?? 0);
                }
            }
        }

        // Added logging to property getters as they contain logic
        public bool Success
        {
            get
            {
                 int? invoiceId = this.OcrInvoices?.Id;
                 _logger.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): Evaluating Template Success for InvoiceId: {InvoiceId}", nameof(Invoice), "PropertyGet", invoiceId);
                 // Added null check for Parts collection itself and individual parts
                 bool success = this.Parts?.All(x => x != null && x.Success) ?? true; // Default to true if Parts is null? Or false? Assuming true.
                 _logger.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): Template Success evaluation result: {Success}. Part Count: {PartCount}", nameof(Invoice), "PropertyGet", success, this.Parts?.Count ?? 0);
                 return success;
            }
        }
        public List<Line> Lines
        {
             get
             {
                  var propertyStopwatch = Stopwatch.StartNew();
                  int? invoiceId = this.OcrInvoices?.Id;
                  _logger.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): Evaluating Template Lines property for InvoiceId: {InvoiceId}", nameof(Invoice), "PropertyGet", invoiceId);
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
                        _logger.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): Generated {LineCount} distinct lines for InvoiceId: {InvoiceId}", nameof(Invoice), "PropertyGet", lines.Count, invoiceId);
                        propertyStopwatch.Stop();
                        _logger.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): Lines property evaluation completed. Duration: {DurationMs}ms.", nameof(Invoice), "PropertyGet", propertyStopwatch.ElapsedMilliseconds);
                  }
                  catch (Exception ex)
                  {
                       propertyStopwatch.Stop();
                       _logger.Error(ex, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms. Error evaluating Lines property for InvoiceId: {InvoiceId}",
                           nameof(Invoice) + " - LinesProperty", propertyStopwatch.ElapsedMilliseconds, invoiceId);
                       lines = new List<Line>(); // Return empty list on error
                  }
                  return lines;
             }
        }


        public Invoice(Invoices ocrInvoices)
        {
            var methodStopwatch = Stopwatch.StartNew();
            int? invoiceId = ocrInvoices?.Id;
            _logger.Information("ACTION_START: {ActionName}. Context: [OcrInvoicesId: {InvoiceId}]",
                nameof(Invoice), invoiceId);

             // Null check input
             if (ocrInvoices == null)
             {
                  methodStopwatch.Stop();
                  _logger.Error("ACTION_END_FAILURE: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. Reason: Null OcrInvoices object.",
                      nameof(Invoice), methodStopwatch.ElapsedMilliseconds);
                  throw new ArgumentNullException(nameof(ocrInvoices), "OcrInvoices object cannot be null.");
             }

            OcrInvoices = ocrInvoices;
             _logger.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): Assigned OcrInvoices (Id: {InvoiceId}) to Template property.", nameof(Invoice), "Assignment", invoiceId);

            // Initialize Parts collection safely
            try
            {
                 _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): Filtering and selecting Parts for InvoiceId: {InvoiceId}", nameof(Invoice), "PartsInitialization", invoiceId);
                 if (ocrInvoices.Parts == null)
                 {
                      _logger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): OcrInvoices Id: {InvoiceId} has a null Parts collection. Template will have no parts.", nameof(Invoice), "PartsInitialization", invoiceId);
                      this.Parts = new List<Part>(); // Initialize to empty list
                 }
                 else
                 {
                     _logger.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): Filtering criteria: Part is not null AND ((Has ParentParts AND No ChildParts) OR (No ParentParts AND No ChildParts))", nameof(Invoice), "PartsInitialization");
                     // Added null checks within LINQ for safety
                     this.Parts = ocrInvoices.Parts
                         //.Where(x => x != null && // Check part is not null
                         //            ( (x.ParentParts != null && x.ParentParts.Any() && (x.ChildParts == null || !x.ChildParts.Any())) || // Condition 1 (safe checks)
                         //              ((x.ParentParts == null || !x.ParentParts.Any()) && (x.ChildParts == null || !x.ChildParts.Any())) ) // Condition 2 (safe checks)
                         //       )
                         .Select(z =>
                         {
                             _logger.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): Creating Part object for OCR_Part Id: {PartId}", nameof(Invoice), "PartCreation", z.Id);
                             // Assuming Part constructor handles potential errors/logging
                             return new Part(z);
                         })
                         .ToList();
                }
                  _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): Initialized InvoiceId: {InvoiceId} with {PartCount} selected Parts.", nameof(Invoice), "PartsInitialization", invoiceId, this.Parts?.Count ?? 0);
            }
            catch (Exception ex)
            {
                 methodStopwatch.Stop();
                 _logger.Error(ex, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms. Error during Parts initialization in Template constructor for OcrInvoices Id: {InvoiceId}",
                     nameof(Invoice) + " - PartsInitialization", methodStopwatch.ElapsedMilliseconds, invoiceId);
                 this.Parts = new List<Part>(); // Ensure Parts is not null even on error
                 throw; // Re-throw after logging
            }

            methodStopwatch.Stop();
            _logger.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms.",
                nameof(Invoice), methodStopwatch.ElapsedMilliseconds);
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