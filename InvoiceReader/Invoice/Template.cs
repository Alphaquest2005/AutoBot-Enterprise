using System.Diagnostics;
﻿using Core.Common.Extensions;
using EntryDataDS.Business.Entities;

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
    public partial class Template
    {
        // Logger instance passed from caller
        private readonly ILogger _logger;

        private EntryData EntryData { get; } = new EntryData(); // Simple initialization, no logging needed
        public Templates OcrTemplates { get; }
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
                    _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): Template Property Changed: FormattedPdfText. Length = {NewValueLength}", nameof(Template), "PropertySet", value?.Length ?? 0);
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
                    _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): Template Property Changed: CsvLines. Count = {NewValueCount}", nameof(Template), "PropertySet", value?.Count ?? 0);
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
                    _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): Template Property Changed: FailedLines. Count = {NewValueCount}", nameof(Template), "PropertySet", value?.Count ?? 0);
                }
            }
        }

        // Added logging to property getters as they contain logic
        public bool Success
        {
            get
            {
                 int? invoiceId = this.OcrTemplates?.Id;
                 _logger.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): Evaluating Template Success for InvoiceId: {InvoiceId}", nameof(Template), "PropertyGet", invoiceId);
                 // Added null check for Parts collection itself and individual parts
                 bool success = this.Parts?.All(x => x != null && x.Success) ?? true; // Default to true if Parts is null? Or false? Assuming true.
                 _logger.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): Template Success evaluation result: {Success}. Part Count: {PartCount}", nameof(Template), "PropertyGet", success, this.Parts?.Count ?? 0);
                 return success;
            }
        }
        public List<Line> Lines
        {
             get
             {
                  var propertyStopwatch = Stopwatch.StartNew();
                  int? invoiceId = this.OcrTemplates?.Id;
                  _logger.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): Evaluating Template Lines property for InvoiceId: {InvoiceId}", nameof(Template), "PropertyGet", invoiceId);
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
                        _logger.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): Generated {LineCount} distinct lines for InvoiceId: {InvoiceId}", nameof(Template), "PropertyGet", lines.Count, invoiceId);
                        propertyStopwatch.Stop();
                        _logger.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): Lines property evaluation completed. Duration: {DurationMs}ms.", nameof(Template), "PropertyGet", propertyStopwatch.ElapsedMilliseconds);
                  }
                  catch (Exception ex)
                  {
                       propertyStopwatch.Stop();
                       _logger.Error(ex, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms. Error evaluating Lines property for InvoiceId: {InvoiceId}",
                           nameof(Template) + " - LinesProperty", propertyStopwatch.ElapsedMilliseconds, invoiceId);
                       lines = new List<Line>(); // Return empty list on error
                  }
                  return lines;
             }
        }


        public Template(Templates ocrTemplates, ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var methodStopwatch = Stopwatch.StartNew();
            int? invoiceId = ocrTemplates?.Id;
            _logger.Debug("ACTION_START: {ActionName}. Context: [OcrInvoicesId: {InvoiceId}]",
                nameof(Template), invoiceId);

             // Null check input
             if (ocrTemplates == null)
             {
                  methodStopwatch.Stop();
                  _logger.Error("ACTION_END_FAILURE: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. Reason: Null OcrInvoices object.",
                      nameof(Template), methodStopwatch.ElapsedMilliseconds);
                  throw new ArgumentNullException(nameof(ocrTemplates), "OcrInvoices object cannot be null.");
             }

            this.OcrTemplates = ocrTemplates;
             _logger.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): Assigned OcrInvoices (Id: {InvoiceId}) to Template property.", nameof(Template), "Assignment", invoiceId);

            // Initialize Parts collection safely
            try
            {
                 _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): Filtering and selecting Parts for InvoiceId: {InvoiceId}", nameof(Template), "PartsInitialization", invoiceId);
                 if (ocrTemplates.Parts == null)
                 {
                      _logger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): OcrInvoices Id: {InvoiceId} has a null Parts collection. Template will have no parts.", nameof(Template), "PartsInitialization", invoiceId);
                      this.Parts = new List<Part>(); // Initialize to empty list
                 }
                 else
                 {
                     _logger.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): Filtering criteria: Part is not null AND ((Has ParentParts AND No ChildParts) OR (No ParentParts AND No ChildParts))", nameof(Template), "PartsInitialization");
                     // Added null checks within LINQ for safety
                     this.Parts = ocrTemplates.Parts
                         //.Where(x => x != null && // Check part is not null
                         //            ( (x.ParentParts != null && x.ParentParts.Any() && (x.ChildParts == null || !x.ChildParts.Any())) || // Condition 1 (safe checks)
                         //              ((x.ParentParts == null || !x.ParentParts.Any()) && (x.ChildParts == null || !x.ChildParts.Any())) ) // Condition 2 (safe checks)
                         //       )
                         .Select(z =>
                         {
                             _logger.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): Creating Part object for OCR_Part Id: {PartId}", nameof(Template), "PartCreation", z.Id);
                             // Assuming Part constructor handles potential errors/logging
                             return new Part(z, _logger);
                         })
                         .ToList();
                }
                  _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): Initialized InvoiceId: {InvoiceId} with {PartCount} selected Parts.", nameof(Template), "PartsInitialization", invoiceId, this.Parts?.Count ?? 0);
            }
            catch (Exception ex)
            {
                 methodStopwatch.Stop();
                 _logger.Error(ex, "ACTION_END_FAILURE: {ActionName}. Duration: {TotalObservedDurationMs}ms. Error during Parts initialization in Template constructor for OcrInvoices Id: {InvoiceId}",
                     nameof(Template) + " - PartsInitialization", methodStopwatch.ElapsedMilliseconds, invoiceId);
                 this.Parts = new List<Part>(); // Ensure Parts is not null even on error
                 throw; // Re-throw after logging
            }

            methodStopwatch.Stop();
            _logger.Debug("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms.",
                nameof(Template), methodStopwatch.ElapsedMilliseconds);
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

        /// <summary>
        /// Clears all mutable state that gets populated during the Read process to prepare for re-import.
        /// This includes Line.Values, CsvLines, and all Part-level accumulated state.
        /// </summary>
        public void ClearInvoiceForReimport()
        {
            var methodStopwatch = Stopwatch.StartNew();
            int? invoiceId = this.OcrTemplates?.Id;
            string methodName = nameof(ClearInvoiceForReimport);

            _logger.Information("ACTION_START: {ActionName}. Context: [InvoiceId: {InvoiceId}]", methodName, invoiceId);

            try
            {
                // 1. Clear CsvLines - final processed results
                _logger.Debug("{MethodName}: Clearing CsvLines (Count: {Count})", methodName, this.CsvLines?.Count ?? 0);
                this.CsvLines = null;

                // 2. Clear FormattedPdfText if it was set
                if (!string.IsNullOrEmpty(this.FormattedPdfText))
                {
                    _logger.Debug("{MethodName}: Clearing FormattedPdfText (Length: {Length})", methodName, this.FormattedPdfText.Length);
                    this.FormattedPdfText = string.Empty;
                }

                // 3. Clear FailedLines if it was set
                if (this.FailedLines != null)
                {
                    _logger.Debug("{MethodName}: Clearing FailedLines (Count: {Count})", methodName, this.FailedLines.Count);
                    this.FailedLines = null;
                }

                // 4. Clear Line.Values for all lines
                _logger.Debug("{MethodName}: Clearing Line.Values for {LineCount} lines", methodName, this.Lines?.Count ?? 0);
                this.Lines?.ForEach(line =>
                {
                    if (line?.Values != null)
                    {
                        var valueCount = line.Values.Count;
                        line.Values.Clear();
                        _logger.Verbose("{MethodName}: Cleared {ValueCount} values from Line {LineId}",
                            methodName, valueCount, line.OCR_Lines?.Id);
                    }
                });

                // 5. Clear Part-level mutable state that accumulates during Read process
                _logger.Debug("{MethodName}: Clearing Part-level mutable state for {PartCount} parts", methodName, this.Parts?.Count ?? 0);
                this.Parts?.ForEach(part =>
                {
                    if (part != null)
                    {
                        part.ClearPartForReimport();
                    }
                });

                methodStopwatch.Stop();
                _logger.Information("ACTION_END_SUCCESS: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms.",
                    methodName, methodStopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                methodStopwatch.Stop();
                _logger.Error(ex, "ACTION_END_FAILURE: {ActionName}. Total observed duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    methodName, methodStopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }

    }
}