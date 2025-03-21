using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MoreLinq;
using OCR.Business.Entities;
using sun.security.jca;

namespace WaterNut.DataSpace
{
    public class Part
    {
        private readonly List<InvoiceLine> _startlines = new List<InvoiceLine>();
        private readonly List<InvoiceLine> _endlines = new List<InvoiceLine>();
        private readonly List<InvoiceLine> _bodylines = new List<InvoiceLine>();
        private readonly List<InvoiceLine> _lines = new List<InvoiceLine>();

        private readonly StringBuilder _linesTxt = new StringBuilder();
       

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

        public bool Success
        {
            get
            {
                return AllRequiredFieldsFilled()
                       //&& this.Lines.Any()
                       && NoFailedLines()
                       && AllChildPartsSucceded();
            }
        }

        private bool AllRequiredFieldsFilled()
        {
            return Lines.All(x =>
                !x.Values.SelectMany(z => z.Value).Any(z =>
                    z.Key.fields.IsRequired && string.IsNullOrEmpty(z.Value.ToString())));
        }

        private bool NoFailedLines()
        {
            return !this.FailedLines.Any();
        }

        private bool AllChildPartsSucceded()
        {
            return ChildParts.All(x => x.Success == true);
        }

        public List<Line> FailedLines
        {
            get
            {
                var maxCount = Lines.Max(x => x.Values.Count(z => z.Key.section == "Single"));


                return Lines.Where(x =>
                        x.OCR_Lines.Fields.Any(z => z.IsRequired && z.FieldValue?.Value == null) &&  x.Values.Count < maxCount)
                    .ToList()
                    .Union(ChildParts.SelectMany(x => x.FailedLines)).ToList();
            }
        }

        public List<Dictionary<string, List<KeyValuePair<(Fields fields, int instance), string>>>> FailedFields
        {
            get { return Lines.SelectMany(x => x.FailedFields).ToList(); }
        }
        //public List<Dictionary<string, List<KeyValuePair<Fields, string>>>> FailedFields => Lines
        //                                                  .Where(x => x.Values.SelectMany(z => z.Value).Any(z => z.Key.IsRequired && string.IsNullOrEmpty(z.Value.ToString())))
        //                                                  .Select(x => x.Values.SelectMany(z => z.Value.ToList())
        //                                                                        .GroupBy(g => g.Key.Field)
        //                                                                        .ToDictionary(k => k.Key, v => v.ToList())
        //).ToList();

        public List<Line> AllLines
        {
            get { return Lines.Union(ChildParts.SelectMany(x => x.AllLines)).DistinctBy(x => x.OCR_Lines.Id).ToList(); }
        }

        public bool WasStarted
        {
            get
            {
                return this._startlines.Any();
                //{ get; set; } //;
            }
        }

        private int lastLineRead = 0;
        private int _instance = 1;


        //public string Section { get; set; }
        public void Read(List<InvoiceLine> newlines, string Section)
        {
            try
            {

                

                if ((_endlines.Count == EndCount && EndCount > 0)
                    || (EndCount == 0 && OCR_Part.Start.Any(x => x.RegularExpressions?.MultiLine == true)
                        ? _startlines.Count >= StartCount
                        : _startlines.Count > StartCount && StartCount > 0)
                   ) //attempting to do start to start
                {
                    if (OCR_Part.RecuringPart != null)
                    {
                        if (!OCR_Part.RecuringPart.IsComposite)
                        {
                            _startlines.Clear();
                            _endlines.Clear();
                            _bodylines.Clear();
                            lastLineRead = _lines.LastOrDefault()?.LineNumber??0;
                            _lines.Clear();
                            _linesTxt.Clear();
                            _instance += 1;
                        }

                    }
                    else
                    {

                        return;
                    }


                }


                //if (_lines.FirstOrDefault(x => x.LineNumber == newlines.First().LineNumber) == null)
                //{
                //    _lines.AddRange(newlines);
                //    newlines.ForEach(x => _linesTxt.AppendLine(x.Line));
                //}

                var newInvoiceLines = newlines.Where(x => (_lines.LastOrDefault()?.LineNumber?? lastLineRead) < x.LineNumber).ToList();
                _lines.AddRange(newInvoiceLines);
                newInvoiceLines.ForEach(x => _linesTxt.AppendLine(x.Line));


                var startFound = FindStart();
                if (startFound)
                {
                    // WasStarted = true;
                    if (!WasStarted || (WasStarted && OCR_Part.RecuringPart != null))
                    {
                        if (_startlines.Count() < StartCount)
                            _startlines.Add(_lines.First());
                        else if (_startlines.Count() == StartCount) // treat as start tot start
                        {
                            _startlines.Clear();
                            _endlines.Clear();
                            _bodylines.Clear();
                            if (StartCount != 0) _startlines.Add(_lines.First());
                           
                        }
                        _instance += 1;

                    } 
                }

                if (_startlines.Count() == StartCount &&
                    ((_endlines.Count < EndCount && EndCount > 0) || EndCount == 0))
                {


                    _bodylines.AddRange(_lines);
                    
                    
                    ChildParts.ForEach(x => x.Read(_lines, Section));
                    Lines.ForEach(x =>
                    {
                        if (x.OCR_Lines.RegularExpressions.MultiLine == true)
                            x.Read(_bodylines.TakeLast(x.OCR_Lines.RegularExpressions.MaxLines??10).Select(z => z.Line).DefaultIfEmpty("").Aggregate((o,n) => $"{o}\r\n{n}").ToString(), _bodylines.First().LineNumber, Section, _instance);
                        else x.Read(_bodylines.Last().Line, _bodylines.Last().LineNumber, Section, _instance);
                    });

                }

                if (_startlines.Count() == StartCount
                    && _startlines.All(x => _lines.All(l => l.LineNumber != x.LineNumber))
                    && _endlines.Count() < EndCount && OCR_Part.End.Any(z => Regex
                        .Match(_linesTxt.ToString(),
                            z.RegularExpressions.RegEx,
                            (z.RegularExpressions.MultiLine == true
                                ? RegexOptions.Multiline
                                : RegexOptions.Singleline) | RegexOptions.IgnoreCase).Success))
                {
                    _endlines.Add(_lines.LastOrDefault());
                }


                if (OCR_Part.Start.All(z => z.RegularExpressions?.MultiLine != true ))
                {
                    _lines.Clear();
                    _linesTxt.Clear();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private bool FindStart()
        {
            try
            {


                if (!OCR_Part.Start.Any()) return true;

                if (WasStarted && OCR_Part.RecuringPart?.IsComposite == true) return false;

                foreach (var z in OCR_Part.Start)
                {
                    var val = _linesTxt.ToString().TrimEnd("\r\n".ToArray());
                    var match = Regex
                        .Match(val,
                            z.RegularExpressions.RegEx,
                            (z.RegularExpressions.MultiLine == true
                                ? RegexOptions.Multiline
                                : RegexOptions.Singleline) | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture, TimeSpan.FromSeconds(5));
                    if (match.Success)
                    {

                        if (z.RegularExpressions?.MultiLine == true)
                        {
                            var sline = new List<InvoiceLine>();
                            var lines = _lines.OrderByDescending(x => x.LineNumber).ToList();
                            var matchLines = match.Value.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
                            for (var index = 0; index < lines.Count; index++)
                            {
                                if (sline.Count(x => !string.IsNullOrEmpty(x.Line)) > matchLines.Count(x => !string.IsNullOrEmpty(x)) + 1) return false;
                                var line = lines[index];
                                if(match.Value.Contains(line.Line) || matchLines.Any(x => line.Line.Contains(x))) sline.Add(line);
                                if (!sline.OrderBy(x => x.LineNumber).Select(x => x.Line).DefaultIfEmpty("")
                                        .Aggregate((c, n) => c + "\r\n" + n).Contains(match.Value.Trim())) continue;

                                _lines.Clear();
                                _lines.AddRange(sline.OrderBy(x => x.LineNumber));
                                return true;

                            }
                        }

                        return true;
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

    }
}