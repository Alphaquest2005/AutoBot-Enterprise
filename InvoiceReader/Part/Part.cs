using System.Text;
using OCR.Business.Entities;


namespace WaterNut.DataSpace
{
    public partial class Part
    {
        private readonly List<InvoiceLine> _startlines = new List<InvoiceLine>();
        private readonly List<InvoiceLine> _endlines = new List<InvoiceLine>();
        private readonly List<InvoiceLine> _lines = new List<InvoiceLine>(); // Holds ALL lines passed to this part within its current parent context
        private readonly StringBuilder _instanceLinesTxt = new StringBuilder(); // Text buffer specific to the current instance being processed

        public Parts OCR_Part;

        public Part(Parts part)
        {
            StartCount = part.Start.Select(x => x.RegularExpressions.RegEx).Count();
            EndCount = part.End.Select(x => x.RegularExpressions.RegEx).Count();
            OCR_Part = part;
            ChildParts = part.ParentParts.Select(x => new Part(x.ChildPart)).ToList();
            Lines = part.Lines.Where(x => x.IsActive ?? true).Select(x => new Line(x)).ToList();
            lastLineRead = 0;
        }

        public List<Part> ChildParts { get; }
        public List<Line> Lines { get; }
        private int EndCount { get; }
        private int StartCount { get; }

        public bool Success => AllRequiredFieldsFilled() && NoFailedLines() && AllChildPartsSucceded();

        private bool AllRequiredFieldsFilled() => Lines.All(x => !x.Values.SelectMany(z => z.Value).Any(z => z.Key.fields.IsRequired && string.IsNullOrEmpty(z.Value?.ToString())));
        private bool NoFailedLines() => !FailedLines.Any();
        private bool AllChildPartsSucceded() => ChildParts.All(x => x.Success);

        public List<Line> FailedLines => Lines.Where(x => x.OCR_Lines.Fields.Any(z => z.IsRequired && z.FieldValue?.Value == null) && x.Values.Count == 0)
                                            .Union(ChildParts.SelectMany(x => x.FailedLines)).ToList();

        public List<Dictionary<string, List<KeyValuePair<(Fields fields, int instance), string>>>> FailedFields => Lines.SelectMany(x => x.FailedFields).ToList();
        public List<Line> AllLines => Lines.Union(ChildParts.SelectMany(x => x.AllLines)).DistinctBy(x => x.OCR_Lines.Id).ToList();
        public bool WasStarted => _startlines.Any();

        private int lastLineRead = 0;
        private int _instance = 1; // Internal instance counter
        private int _lastProcessedParentInstance = 0; // Track the last parent instance processed by this child
        private int _currentInstanceStartLineNumber = -1; // Track the line number where the current instance started

        // FindStart now returns the triggering InvoiceLine or null
        // Takes the lines relevant to the current instance buffer as input
    }
}