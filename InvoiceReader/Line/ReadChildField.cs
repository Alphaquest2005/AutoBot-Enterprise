using System.Text.RegularExpressions;
using OCR.Business.Entities;

namespace WaterNut.DataSpace;

public partial class Line
{
    private void ReadChildField(Fields childField, Dictionary<(Fields fields, int instance), string> values,
        string strValue)
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
}