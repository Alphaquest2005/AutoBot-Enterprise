using System.Text;
using OCR.Business.Entities;
using System.Collections.Generic; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added
using Core.Common; // Added for BetterExpando if needed elsewhere
using MoreLinq; // Added for DistinctBy

namespace WaterNut.DataSpace
{
    public partial class Part
    {
        // Define logger and timeout here
        private static readonly ILogger _logger = Log.ForContext<Part>();
        private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(5); // Consistent timeout

        private readonly List<InvoiceLine> _startlines = new List<InvoiceLine>();
        private readonly List<InvoiceLine> _endlines = new List<InvoiceLine>();
        private readonly List<InvoiceLine> _lines = new List<InvoiceLine>(); // Holds ALL lines passed to this part within its current parent context
        private readonly StringBuilder _instanceLinesTxt = new StringBuilder(); // Text buffer specific to the current instance being processed

        public Parts OCR_Part { get; } // Make readonly after constructor

        public Part(Parts part)
        {
            int? partId = part?.Id;
            _logger.Debug("Constructing Part object for OCR_Part Id: {PartId}", partId);

            if (part == null)
            {
                 _logger.Error("Part constructor called with null OCR_Part object. Cannot initialize.");
                 throw new ArgumentNullException(nameof(part), "OCR_Part object cannot be null.");
            }

            OCR_Part = part;
             _logger.Verbose("Assigned OCR_Part (Id: {PartId}) to Part property.", partId);

            // Safely calculate counts
            StartCount = part.Start?.Count() ?? 0;
            EndCount = part.End?.Count() ?? 0;
             _logger.Verbose("PartId: {PartId} - StartCondition Count: {StartCount}, EndCondition Count: {EndCount}", partId, StartCount, EndCount);

            // Initialize ChildParts safely
            try
            {
                 _logger.Debug("Initializing ChildParts for PartId: {PartId}", partId);
                 // Use null-conditional operator and null-coalescing operator for concise safe initialization
                 ChildParts = part.ParentParts?
                                 .Where(pp => pp?.ChildPart != null) // Safe check
                                 .Select(x => {
                                      _logger.Verbose("Creating child Part for OCR_Part Id: {ChildPartId}", x.ChildPart.Id);
                                      return new Part(x.ChildPart); // Recursive constructor call
                                      })
                                 .ToList() ?? new List<Part>(); // Default to empty list if ParentParts is null
                  _logger.Information("Initialized PartId: {PartId} with {ChildCount} ChildParts.", partId, ChildParts.Count);
            }
            catch (Exception ex)
            {
                 _logger.Error(ex, "Error initializing ChildParts for PartId: {PartId}", partId);
                 ChildParts = new List<Part>(); // Ensure collection is not null
            }

            // Initialize Lines safely
            try
            {
                 _logger.Debug("Initializing Lines for PartId: {PartId}", partId);
                 // Use null-conditional and null-coalescing operators
                 Lines = part.Lines?
                             .Where(x => x != null && (x.IsActive ?? true)) // Safe check for line and IsActive
                             .Select(x => {
                                  _logger.Verbose("Creating Line object for OCR_Lines Id: {LineId}", x.Id);
                                  return new Line(x); // Line constructor handles logging
                                  })
                             .ToList() ?? new List<Line>(); // Default to empty list if Lines is null
                  _logger.Information("Initialized PartId: {PartId} with {LineCount} active Lines.", partId, Lines.Count);
            }
            catch (Exception ex)
            {
                 _logger.Error(ex, "Error initializing Lines for PartId: {PartId}", partId);
                 Lines = new List<Line>(); // Ensure collection is not null
            }


            lastLineRead = 0; // Initial state
             _logger.Debug("Finished constructing Part object for PartId: {PartId}", partId);
        }

        public List<Part> ChildParts { get; } // Changed to readonly after constructor assignment
        public List<Line> Lines { get; } // Changed to readonly after constructor assignment
        private int EndCount { get; }
        private int StartCount { get; }

        // Added logging to property getters
        public bool Success
        {
             get {
                  int? partId = this.OCR_Part?.Id;
                  _logger.Verbose("Evaluating Success property for PartId: {PartId}", partId);
                  // Call helper methods which now contain logging
                  bool result = AllRequiredFieldsFilled() && NoFailedLines() && AllChildPartsSucceded();
                  _logger.Verbose("Success evaluation result for PartId: {PartId}: {Result}", partId, result);
                  return result;
             }
        }

        private bool AllRequiredFieldsFilled()
        {
             // This property relies on FailedFields logic. Let's log the check here.
             bool hasNoFailedRequiredFields = !this.FailedFields.Any(); // Access FailedFields property
             _logger.Verbose("Evaluating AllRequiredFieldsFilled for PartId: {PartId}. Result (based on FailedFields): {Result}", this.OCR_Part?.Id, hasNoFailedRequiredFields);
             return hasNoFailedRequiredFields;
        }
        private bool NoFailedLines()
        {
             bool noFailed = !this.FailedLines.Any(); // Access FailedLines property which has logging
             _logger.Verbose("Evaluating NoFailedLines for PartId: {PartId}. Result: {Result}", this.OCR_Part?.Id, noFailed);
             return noFailed;
        }
        private bool AllChildPartsSucceded()
        {
             // Added null check for ChildParts collection
             bool allSucceeded = this.ChildParts?.All(x => x != null && x.Success) ?? true; // Default to true if ChildParts is null
             _logger.Verbose("Evaluating AllChildPartsSucceded for PartId: {PartId}. Child Count: {Count}, Result: {Result}", this.OCR_Part?.Id, this.ChildParts?.Count ?? 0, allSucceeded);
             return allSucceeded;
        }

        public List<Line> FailedLines
        {
             get {
                  int? partId = this.OCR_Part?.Id;
                  _logger.Verbose("Evaluating FailedLines property for PartId: {PartId}", partId);
                  List<Line> failed = null;
                  try
                  {
                       // Added null checks and safe navigation
                       var directFailed = this.Lines?
                                            .Where(x => x?.OCR_Lines?.Fields != null && // Safe checks
                                                        x.OCR_Lines.Fields.Any(z => z != null && z.IsRequired && z.FieldValue?.Value == null) &&
                                                        (x.Values == null || !x.Values.Any())) // Check Values null/empty
                                            .ToList() ?? new List<Line>();

                       var childFailed = this.ChildParts?
                                            .Where(cp => cp != null) // Safe check
                                            .SelectMany(x => x.FailedLines ?? Enumerable.Empty<Line>()) // Access property recursively, handle null
                                            .ToList() ?? new List<Line>();

                       failed = directFailed.Union(childFailed).ToList(); // Combine direct and child failures
                        _logger.Verbose("Found {Count} total failed lines (direct + child) for PartId: {PartId}", failed.Count, partId);
                  }
                  catch (Exception ex)
                  {
                       _logger.Error(ex, "Error evaluating FailedLines property for PartId: {PartId}", partId);
                       failed = new List<Line>(); // Return empty list on error
                  }
                  return failed;
             }
        }

        public List<Dictionary<string, List<KeyValuePair<(Fields fields, int instance), string>>>> FailedFields
        {
             get {
                  int? partId = this.OCR_Part?.Id;
                  _logger.Verbose("Evaluating FailedFields property for PartId: {PartId}", partId);
                  List<Dictionary<string, List<KeyValuePair<(Fields fields, int instance), string>>>> failed = null;
                  try
                  {
                       // Added null checks and safe navigation
                       failed = this.Lines?
                                    .Where(l => l != null) // Safe check
                                    .SelectMany(x => x.FailedFields ?? Enumerable.Empty<Dictionary<string, List<KeyValuePair<(Fields fields, int instance), string>>>>()) // Access property, handle null
                                    .ToList()
                                    ?? new List<Dictionary<string, List<KeyValuePair<(Fields fields, int instance), string>>>>(); // Default if Lines is null
                        _logger.Verbose("Found {Count} groups of failed fields from direct lines for PartId: {PartId}", failed.Count, partId);
                  }
                  catch (Exception ex)
                  {
                       _logger.Error(ex, "Error evaluating FailedFields property for PartId: {PartId}", partId);
                       failed = new List<Dictionary<string, List<KeyValuePair<(Fields fields, int instance), string>>>>(); // Return empty list on error
                  }
                  return failed;
             }
        }
        public List<Line> AllLines
        {
             get {
                  int? partId = this.OCR_Part?.Id;
                  _logger.Verbose("Evaluating AllLines property for PartId: {PartId}", partId);
                  List<Line> all = null;
                  try
                  {
                       // Added null checks and safe navigation
                       var directLines = this.Lines ?? new List<Line>();
                       var childLines = this.ChildParts?
                                          .Where(cp => cp != null) // Safe check
                                          .SelectMany(x => x.AllLines ?? Enumerable.Empty<Line>()) // Access property recursively, handle null
                                          .ToList() ?? new List<Line>();

                       all = directLines.Union(childLines)
                                        .DistinctBy(x => x?.OCR_Lines?.Id) // Safe DistinctBy
                                        .Where(l => l != null) // Filter nulls post-distinct
                                        .ToList();
                        _logger.Verbose("Found {Count} total distinct lines (direct + child) for PartId: {PartId}", all.Count, partId);
                  }
                  catch (Exception ex)
                  {
                       _logger.Error(ex, "Error evaluating AllLines property for PartId: {PartId}", partId);
                       all = new List<Line>(); // Return empty list on error
                  }
                  return all;
             }
        }
        public bool WasStarted => _startlines?.Any() ?? false; // Safe check

        private int lastLineRead = 0;
        private int _instance = 1; // Internal instance counter
        private int _lastProcessedParentInstance = 0; // Track the last parent instance processed by this child
        private int _currentInstanceStartLineNumber = -1; // Track the line number where the current instance started

        // FindStart now returns the triggering InvoiceLine or null
        // Takes the lines relevant to the current instance buffer as input
    }
}