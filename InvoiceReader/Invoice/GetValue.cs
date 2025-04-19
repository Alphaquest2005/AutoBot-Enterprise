using System.Globalization;
using OCR.Business.Entities;

namespace WaterNut.DataSpace;

public partial class Invoice
{
    private dynamic GetValue(
        KeyValuePair<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>> z,
        (Fields fields, int instance) field)
    {
        try
        {
            var f = z.Value.FirstOrDefault(q => q.Key == field);
            return f.Key.fields == null ? null : GetValue(f);
        }
        catch (Exception e)
        {
            var ex = new ApplicationException($"Error Importing Line:{z.Key} --- {e.Message}", e);
            Console.WriteLine(ex);
            throw ex;
        }
    }

    private dynamic GetValueByKey(
        KeyValuePair<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>> z, string key)
    {
        try
        {
            var f = z.Value.FirstOrDefault(q => q.Key.fields.Key == key);
            return f.Key.fields == null ? null : GetValue(f);
        }
        catch (Exception e)
        {
            var ex = new ApplicationException($"Error Importing Line:{z.Key} --- {e.Message}", e);
            Console.WriteLine(ex);
            throw ex;
        }
    }

    private dynamic GetValue(string field)
    {
        var f = Lines.Where(x => x.OCR_Lines.Parts.RecuringPart == null).SelectMany(x => x.Values.Values)
            .SelectMany(x => x).FirstOrDefault(x => x.Key.fields.Field == field);
        return f.Key.fields == null ? null : GetValue(f);
    }

    private static dynamic GetValue(KeyValuePair<(Fields fields, int instance), string> f)
    {
        try
        {
            if (f.Key.fields == null) return null;
            switch (f.Key.fields.DataType)
            {
                case "String":
                    return f.Value;
                case "Numeric":
                case "Number":
                    var val = f.Value.Replace("$", "");
                    if (val == "") val = "0";
                    if (double.TryParse(val, out double num))
                        return num;
                    else
                        throw new ApplicationException(
                            $"{f.Key.fields.Field} can not convert to {f.Key.fields.DataType} for Value:{f.Value}");

                case "Date":
                    if (DateTime.TryParse(f.Value, out DateTime date))
                        return date;
                    else
                        //throw new ApplicationException(
                        //    $"{f.Key.Field} can not convert to {f.Key.DataType} for Value:{f.Value}");
                        return DateTime.MinValue;
                case "English Date":
                    var formatStrings = new List<string>()
                    {
                        "dd/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy", "d/M/yyyy", "M/yyyy", "MMMM d, yyyy", "dd.MM.yyyy",
                        "yyyy-mm-dd"
                    };
                    foreach (String formatString in formatStrings)
                    {
                        if (DateTime.TryParseExact(f.Value, formatString, CultureInfo.InvariantCulture,
                                DateTimeStyles.None,
                                out DateTime edate))
                            return edate;
                    }

                    throw new ApplicationException(
                        $"{f.Key.fields.Field} can not convert to {f.Key.fields.DataType} for Value:{f.Value}");
                default:
                    return f.Value;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}