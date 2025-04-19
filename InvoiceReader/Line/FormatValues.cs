using System.Text.RegularExpressions;
using OCR.Business.Entities;

namespace WaterNut.DataSpace;

public partial class Line
{
    private void FormatValues(int instance, MatchCollection matches,
        Dictionary<(Fields Fields, int Instance), string> values)
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

                if (string.IsNullOrEmpty(value)) continue;

                if (values.ContainsKey((field, instance)))
                {
                    if ( /*OCR_Lines.Parts.RecuringPart != null && OCR_Lines.Parts.RecuringPart.IsComposite == true &&
                                 Took this out becasue marineco has two details which combining
                                 */ OCR_Lines.DistinctValues.GetValueOrDefault() == true) continue;

                    switch (field.DataType)
                    {
                        case "String":
                            values[(field, instance)] = values[(field, instance)] + " " + value.Trim();
                            break;
                        case "Number":
                            values[(field, instance)] =
                                (double.Parse(values[(field, instance)]) + double.Parse(value)).ToString();
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
}