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
            string methodName = nameof(Part) + " Constructor";
            int? partId = part?.Id;
            _logger.Verbose("Entering {MethodName} for OCR_Part Id: {PartId}", methodName, partId);

            // --- Input Validation ---
            if (part == null)
            {
                _logger.Error("{MethodName}: Called with null OCR_Part object. Cannot initialize.", methodName);
                _logger.Verbose("Exiting {MethodName} due to null input.", methodName);
                throw new ArgumentNullException(nameof(part), "OCR_Part object cannot be null.");
            }

            OCR_Part = part;
            _logger.Verbose("{MethodName}: Assigned OCR_Part (Id: {PartId}) to Part property.", methodName, partId);

            // --- Calculate Counts ---
            StartCount = part.Start?.Count() ?? 0;
            EndCount = part.End?.Count() ?? 0;
            _logger.Verbose("{MethodName}: PartId: {PartId} - StartCondition Count: {StartCount}, EndCondition Count: {EndCount}", methodName, partId, StartCount, EndCount);

            // --- Initialize ChildParts ---
            _logger.Verbose("{MethodName}: Initializing ChildParts for PartId: {PartId}...", methodName, partId);
            try
            {
                ChildParts = part.ParentParts?
                                .Where(pp => pp?.ChildPart != null) // Safe check
                                .Select(x => {
                                    _logger.Verbose("{MethodName}: Creating child Part for OCR_Part Id: {ChildPartId} (Parent: {ParentPartId})", methodName, x.ChildPart.Id, partId);
                                    return new Part(x.ChildPart); // Recursive constructor call
                                    })
                                .ToList() ?? new List<Part>();
                _logger.Information("{MethodName}: Initialized PartId: {PartId} with {ChildCount} ChildParts.", methodName, partId, ChildParts.Count);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "{MethodName}: Error initializing ChildParts for PartId: {PartId}", methodName, partId);
                ChildParts = new List<Part>(); // Ensure collection is not null
                _logger.Warning("{MethodName}: ChildParts collection set to empty list due to initialization error for PartId: {PartId}.", methodName, partId);
            }
            _logger.Verbose("{MethodName}: Finished initializing ChildParts for PartId: {PartId}.", methodName, partId);


            // --- Initialize Lines ---
            _logger.Verbose("{MethodName}: Initializing Lines for PartId: {PartId}...", methodName, partId);
            try
            {
                Lines = part.Lines?
                            .Where(x => x != null && (x.IsActive ?? true)) // Safe check for line and IsActive
                            .Select(x => {
                                _logger.Verbose("{MethodName}: Creating Line object for OCR_Lines Id: {LineId} (Parent: {ParentPartId})", methodName, x.Id, partId);
                                return new Line(x); // Line constructor handles logging
                                })
                            .ToList() ?? new List<Line>();
                _logger.Information("{MethodName}: Initialized PartId: {PartId} with {LineCount} active Lines.", methodName, partId, Lines.Count);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "{MethodName}: Error initializing Lines for PartId: {PartId}", methodName, partId);
                Lines = new List<Line>(); // Ensure collection is not null
                _logger.Warning("{MethodName}: Lines collection set to empty list due to initialization error for PartId: {PartId}.", methodName, partId);
            }
            _logger.Verbose("{MethodName}: Finished initializing Lines for PartId: {PartId}.", methodName, partId);


            // --- Set Initial State ---
            lastLineRead = 0;
            _logger.Verbose("{MethodName}: Initialized lastLineRead to 0 for PartId: {PartId}.", methodName, partId);

            _logger.Information("Exiting {MethodName} successfully for PartId: {PartId}", methodName, partId);
        }

        public List<Part> ChildParts { get; } // Changed to readonly after constructor assignment
        public List<Line> Lines { get; } // Changed to readonly after constructor assignment
        private int EndCount { get; }
        private int StartCount { get; }

        // Added logging to property getters

        public bool WasStarted => _startlines?.Any() ?? false; // Safe check - Simple property, extensive logging likely overkill

        private int lastLineRead = 0;
        private int _instance = 1; // Internal instance counter
        private int _lastProcessedParentInstance = 0; // Track the last parent instance processed by this child
        private int _currentInstanceStartLineNumber = -1; // Track the line number where the current instance started

        // FindStart now returns the triggering InvoiceLine or null
        // Takes the lines relevant to the current instance buffer as input
    }
}