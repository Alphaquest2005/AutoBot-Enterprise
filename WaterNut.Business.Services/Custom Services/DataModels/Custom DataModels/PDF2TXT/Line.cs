using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MoreLinq;
using OCR.Business.Entities;

namespace WaterNut.DataSpace
{
    public class Line
    {
        public Lines OCR_Lines { get; }

        public Line(Lines lines)
        {
            OCR_Lines = lines;
        }

        //private int Instance { get; set; } = 0;

        public bool Read(string line, int lineNumber, string section)
        {
            try
            {
                //Instance += 1;
                var matches = Regex.Matches(line, OCR_Lines.RegularExpressions.RegEx,
                    (OCR_Lines.RegularExpressions.MultiLine == true
                        ? RegexOptions.Multiline
                        : RegexOptions.Singleline) | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (matches.Count == 0) return false;
                var values = new Dictionary<(Fields Fields, int Instance), string>();
                var instance = 0;

                

                foreach (Match match in matches)
                {
                    instance += 1;

                    


                    foreach (var field in OCR_Lines.Fields.Where(x => x.ParentField == null))
                    {
                        var value = field.FieldValue?.Value ?? match.Groups[field.Key].Value.Trim();
                        foreach (var reg in field.FormatRegEx)
                        {
                            value = Regex.Replace(value, reg.RegEx.RegEx, reg.ReplacementRegEx.RegEx,
                                (OCR_Lines.RegularExpressions.MultiLine == true
                                    ? RegexOptions.Multiline
                                    : RegexOptions.Singleline) | RegexOptions.IgnoreCase |
                                RegexOptions.ExplicitCapture);
                        }


                        if (values.ContainsKey((field, instance)))
                        {
                            if (/*OCR_Lines.Parts.RecuringPart != null && OCR_Lines.Parts.RecuringPart.IsComposite == true && 
                                 Took this out becasue marineco has two details which combining
                                 */ OCR_Lines.DistinctValues.GetValueOrDefault() == true) continue;
                            
                            switch (field.DataType)
                            {
                                case "String":
                                    values[(field, instance)] = values[(field, instance)] + " " + value.Trim();
                                    break;
                                case "Number":
                                    values[(field, instance)] = (double.Parse(values[(field, instance)]) + double.Parse(value)).ToString();
                                    break;
                                case "Date":
                                    values[(field, instance)] = value;
                                    break;
                            }
                        }
                        else
                        {
                            values.Add((field, instance), value.Trim()); 
                        }
                        

                        foreach (var childField in field.ChildFields)
                        {
                            ReadChildField(childField, values,
                                values.Where(x => x.Key.Fields.Field == field.Field).Select(x => x.Value).DefaultIfEmpty("")
                                    .Aggregate((o, n) => o + " " + n));
                        }

                    }
                    

                }

                Values[(lineNumber, section)] = values;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void ReadChildField(Fields childField, Dictionary<(Fields fields, int instance), string> values, string strValue)
        {
         
            var match = Regex.Match(strValue.Trim(), childField.Lines.RegularExpressions.RegEx,
                (childField.Lines.RegularExpressions.MultiLine == true
                    ? RegexOptions.Multiline
                    : RegexOptions.Singleline) | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            if (!match.Success) return;
            foreach (var field in childField.Lines.Fields)
            {


                var value = field.FieldValue?.Value ?? match.Groups[field.Key].Value.Trim();
                foreach (var reg in field.FormatRegEx)
                {
                    value = Regex.Replace(value, reg.RegEx.RegEx, reg.ReplacementRegEx.RegEx,
                        (OCR_Lines.RegularExpressions.MultiLine == true
                            ? RegexOptions.Multiline
                            : RegexOptions.Singleline) | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                }
                values.Add((field, 1), value.Trim());
            }
        }

        public Dictionary<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>> Values { get; } = new Dictionary<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>>();
        //public bool MultiLine => OCR_Lines.MultiLine;

        public List<Dictionary<string, List<KeyValuePair<(Fields fields, int instance), string>>>> FailedFields => this.Values
            .Where(x => x.Value.Any(z => z.Key.fields.IsRequired && string.IsNullOrEmpty(z.Value.ToString())))
            .SelectMany(x => x.Value.ToList())
            .DistinctBy(x => x.Key.fields.Id)
            .GroupBy(x => $"{x.Key.fields.Field}-{x.Key.fields.Key}")
            .DistinctBy(x => x.Key)
            .Select(x => x.ToDictionary(k => x.Key, v => x.ToList()))
            .ToList();

    }
}