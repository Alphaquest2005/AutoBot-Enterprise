using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Common.Extensions;
using EntryDataDS.Business.Entities;
using MoreLinq;
using OCR.Business.Entities;
using Invoices = OCR.Business.Entities.Invoices;
using MoreEnumerable = MoreLinq.MoreEnumerable;

namespace WaterNut.DataSpace
{
    public class Invoice
    {
        private EntryData EntryData { get; } = new EntryData();
        public Invoices OcrInvoices { get; }
        public List<Part> Parts { get; set; }
        public bool Success => Parts.All(x => x.Success);
        public List<Line> Lines => MoreEnumerable.DistinctBy(Parts.SelectMany(x => x.AllLines), x => x.OCR_Lines.Id).ToList();

        public Invoice(Invoices ocrInvoices)
        {
            OcrInvoices = ocrInvoices;
            Parts = ocrInvoices.Parts
                .Where(x => (x.ParentParts.Any() && !x.ChildParts.Any()) ||
                            (!x.ParentParts.Any() && !x.ChildParts.Any()))
                //.Where(x => x.Id == 7)
                .Select(z => new Part(z)).ToList();
        }

        public List<dynamic> Read(string text)
        {
            return Read(text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList());
        }

        private static readonly Dictionary<string, string> Sections = new Dictionary<string, string>()
        {
            { "Single", "---Single Column---" },
            { "Sparse", "---SparseText---" },
            { "Ripped", "---Ripped Text---" }
        };

        public List<dynamic> Read(List<string> text)
        {
            try
            {


                var lineCount = 0;
                var section = "";
                foreach (var line in text)
                {
                    
                    Sections.ForEach(s =>
                    {
                        if (line.Contains(s.Value)) section = s.Key;
                    });

                    lineCount += 1;
                    var iLine = new List<InvoiceLine>(){ new InvoiceLine(line, lineCount) };
                    Parts.ForEach(x => x.Read(iLine, section));
                }

                if (!this.Lines.SelectMany(x => x.Values.Values).Any()) return new List<dynamic>();//!Success

                var ores = Parts.Select(x =>
                    {

                        var lst = SetPartLineValues(x);
                        return lst;
                    }
                ).ToList();

                
                return new List<dynamic> {ores.SelectMany(x => x.ToList()).ToList()};
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public double MaxLinesCheckedToStart { get; set; } = 0.5;
        private static readonly Dictionary<string, List<BetterExpando>> table = new Dictionary<string, List<BetterExpando>>();

        private List<IDictionary<string, object>> SetPartLineValues(Part part)
        {
            try
            {


                var lst = new List<IDictionary<string, object>>();
                var itm = new BetterExpando();
                var ditm = ((IDictionary<string, object>)itm);



                foreach (var line in part.Lines)
                {
                    if (!line.OCR_Lines.Fields.Any()) continue;
                    var values = (line.OCR_Lines.DistinctValues == true
                        ? DistinctValues(line.Values)
                        : line.Values).ToList();

                    if (!table.ContainsKey(line.OCR_Lines.Fields.First().EntityType) && line.OCR_Lines.IsColumn == true)
                        table.Add(line.OCR_Lines.Fields.First().EntityType, new List<BetterExpando>() { itm });


                    var instances = values.SelectMany(z => z.Value).GroupBy(x => x.Key.instance).ToList();

                    for (int i = 0; i <= values.Count() - 1; i++)
                    {
                        var value = values[i];

                        //foreach (var instance in instances)
                        //{
                            itm = CreateOrGetDitm(part, line, i, itm, ref ditm, lst);

                            ditm["FileLineNumber"] = value.Key.lineNumber + 1;
                            ditm["Instance"] = i;//instance.Key;
                            ditm["Section"] = value.Key.section;




                            foreach (var field in value.Value)
                            {
                                if (ditm.ContainsKey(field.Key.fields.Field) &&
                                    (field.Key.fields.AppendValues == true || line.OCR_Lines.Fields.Select(z => z.Field)
                                        .Count(f => f == field.Key.fields.Field) > 1))
                                {
                                    ImportByDataType(field, ditm, value);
                                }
                                else
                                {
                                    ditm[field.Key.fields.Field] = GetValue(value, field.Key);
                                    //ImportByDataType(field, ditm, value);
                                }

                            }

                            if (ditm.Count == 1) continue;
                            if (part.OCR_Part.RecuringPart != null && part.OCR_Part.RecuringPart.IsComposite == false)
                                if(lst.ElementAtOrDefault(i) == null) lst.Add(itm);
                        //}
                    }
                }

                foreach (var childPart in part.ChildParts.Where(x => x.AllLines.Any()))
                {
                    // if (!childPart.Lines.Any()) continue;
                    var childItms = SetPartLineValues(childPart);
                    var fieldname = childPart.AllLines.First().OCR_Lines.Fields.First().EntityType;
                    if (childPart.OCR_Part.RecuringPart != null || !childPart.Lines.Any())
                    {
                        if (!part.Lines.Any())
                        {
                            lst.AddRange(childItms);
                        }
                        else
                        {
                            if (ditm.ContainsKey(fieldname))
                            {
                                ((List<IDictionary<string, object>>)ditm[fieldname]).AddRange(childItms);
                            }
                            else
                            {
                                ditm[fieldname] = childItms;
                            }

                        }

                    }
                    else
                    {
                        if (!part.Lines.Any())
                        {
                            lst.Add(childItms.FirstOrDefault());
                        }
                        else
                        {
                            ditm[fieldname] = childItms.FirstOrDefault();
                        }

                    }

                }


                if ((part.OCR_Part.RecuringPart == null || part.OCR_Part.RecuringPart.IsComposite) && ditm.Any())
                    lst.Add(itm);
                return lst;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static BetterExpando CreateOrGetDitm(Part part, Line line, int i, BetterExpando itm,
            ref IDictionary<string, object> ditm, List<IDictionary<string, object>> lst)
        {
            if (part.OCR_Part.RecuringPart != null && part.OCR_Part.RecuringPart.IsComposite == false)
            {
                if (line.OCR_Lines?.IsColumn == true)
                {
                    if (i > table[line.OCR_Lines.Fields.First().EntityType]
                            .Count - 1)
                    {
                        itm = new BetterExpando();
                        if (line.OCR_Lines.IsColumn == true)
                        {
                            table[line.OCR_Lines.Fields.First().EntityType].Add(itm);
                        }
                    }
                    else
                    {
                        itm = table[line.OCR_Lines.Fields.First().EntityType][i];
                    }
                }
                else
                {
                    itm = (BetterExpando)lst.ElementAtOrDefault(i) ?? new BetterExpando();
                }
                

                ditm = ((IDictionary<string, object>)itm);
            }

            return itm;
        }

        private Dictionary<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>> DistinctValues(Dictionary<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>> lineValues)
        {
            var res = new Dictionary<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>>();
            foreach (var val in lineValues.Where(val => !res.Values.Any(z => z.Values.Any(q => val.Value.ContainsValue(q)))))
            {
                res.Add((val.Key.lineNumber, val.Key.section), val.Value);
            }
            return res;
        }

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

        private dynamic GetValue(KeyValuePair<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>> z, (Fields fields, int instance) field)
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
        private dynamic GetValueByKey(KeyValuePair<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>> z, string key)
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
                            "dd/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy", "d/M/yyyy", "M/yyyy", "MMMM d, yyyy", "dd.MM.yyyy"
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

        public string Format(string pdftxt)
        {
            try
            {
                foreach (var reg in OcrInvoices.RegEx.OrderBy(x => x.Id))
                {
                    pdftxt = Regex.Replace(pdftxt, reg.RegEx.RegEx, reg.ReplacementRegEx.RegEx,
                        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
                }

                return pdftxt;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}