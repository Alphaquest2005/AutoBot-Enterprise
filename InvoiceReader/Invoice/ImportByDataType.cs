using OCR.Business.Entities;

namespace WaterNut.DataSpace;

public partial class Invoice
{
    private void ImportByDataType(KeyValuePair<(Fields fields, int instance), string> field,
        IDictionary<string, object> ditm,
        KeyValuePair<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>> value)
    {
        try
        {
            switch (field.Key.fields.DataType)
            {
                case "String":
                    ditm[field.Key.fields.Field] =
                        (ditm[field.Key.fields.Field] + " " + GetValueByKey(value, field.Key.fields.Key)).Trim();
                    break;
                case "Number":
                case "Numeric":

                    if (field.Key.fields.AppendValues == true)
                    {
                        var val = GetValueByKey(value, field.Key.fields.Key);
                        if (ditm[field.Key.fields.Field].ToString() != val.ToString())
                            ditm[field.Key.fields.Field] =
                                Convert.ToDouble(ditm[field.Key.fields.Field] ?? "0") +
                                Convert.ToDouble(GetValueByKey(value, field.Key.fields.Key));
                    }
                    else
                    {
                        ditm[field.Key.fields.Field] =
                            Convert.ToDouble(ditm[field.Key.fields.Field] ?? "0") +
                            Convert.ToDouble(GetValueByKey(value, field.Key.fields.Key));
                    }

                    break;
                default:
                    ditm[field.Key.fields.Field] = GetValueByKey(value, field.Key.fields.Key);
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}