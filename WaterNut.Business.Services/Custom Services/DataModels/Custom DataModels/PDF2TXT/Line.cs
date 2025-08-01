﻿using System;
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

        

        // Modified to accept instance from the calling Part
        public bool Read(string line, int lineNumber, string section, int instance)
        {
            try
            {
                //Instance += 1;
                var matches = Regex.Matches(line, OCR_Lines.RegularExpressions.RegEx,
                    (OCR_Lines.RegularExpressions.MultiLine == true
                        ? RegexOptions.Multiline
                        : RegexOptions.Singleline) | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture,TimeSpan.FromSeconds(2));
                if (matches.Count == 0) return false;
                var values = new Dictionary<(Fields Fields, int Instance), string>();
                //var instance = 0;

                

                FormatValues(instance, matches, values);

                SaveLineValues(lineNumber, section, instance, values);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void SaveLineValues(int lineNumber, string section, int instance, Dictionary<(Fields Fields, int Instance), string> values)
        {
            // Check if key exists before adding/updating
            if (Values.ContainsKey((lineNumber, section))) {
                // Merge new values with existing ones for the same line/section, potentially handling duplicates if necessary
                foreach(var kvp in values) {
                    // Ensure the key uses the correct instance number passed from the parent
                    // Ensure the key uses the correct instance number passed from the parent and correct field access
                    var correctInstanceKey = (kvp.Key.Fields, instance);
                    if (!Values[(lineNumber, section)].ContainsKey(correctInstanceKey)) {
                        Values[(lineNumber, section)].Add(correctInstanceKey, kvp.Value);
                    } else {
                        // Handle potential duplicate key scenario if needed (e.g., log, overwrite, append)
                        // Current logic seems to append strings or add numbers based on field.DataType in lines 63-74
                        // For simplicity, let's assume overwrite or the existing append logic handles it.
                        // Ensure update uses the correct instance key
                        Values[(lineNumber, section)][correctInstanceKey] = kvp.Value;
                    }
                }
            } 
            else 
            {
                Values[(lineNumber, section)] = values;
            }
        }

        private void FormatValues(int instance, MatchCollection matches, Dictionary<(Fields Fields, int Instance), string> values)
        {
            foreach (Match match in matches)
            {
                //  instance += 1;

                    


                foreach (var field in OCR_Lines.Fields.Where(x => x.ParentField == null))
                {
                    var value = field.FieldValue?.Value.Trim() ?? match.Groups[field.Key].Value.Trim();
                    foreach (var reg in field.FormatRegEx.OrderBy(x => x.Id))
                    {
                        value = (Regex.Replace(value, reg.RegEx.RegEx, reg.ReplacementRegEx.RegEx,
                            (OCR_Lines.RegularExpressions.MultiLine == true
                                ? RegexOptions.Multiline
                                : RegexOptions.Singleline) | RegexOptions.IgnoreCase |
                            RegexOptions.ExplicitCapture)).Trim();
                    }

                    if(string.IsNullOrEmpty(value)) continue;

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
                        // Use the passed instance number (from the parent) in the key
                        values.Add((field, instance), value);
                    }
                        

                    foreach (var childField in field.ChildFields)
                    {
                        ReadChildField(childField, values,
                            values.Where(x => x.Key.Fields.Field == field.Field).Select(x => x.Value).DefaultIfEmpty("")
                                .Aggregate((o, n) => o + " " + n));
                    }

                }
                    

            }
        }

        private void ReadChildField(Fields childField, Dictionary<(Fields fields, int instance), string> values, string strValue)
        {
         
            var match = Regex.Match(strValue.Trim(), childField.Lines.RegularExpressions.RegEx,
                (childField.Lines.RegularExpressions.MultiLine == true
                    ? RegexOptions.Multiline
                    : RegexOptions.Singleline) | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture, TimeSpan.FromSeconds(2));
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