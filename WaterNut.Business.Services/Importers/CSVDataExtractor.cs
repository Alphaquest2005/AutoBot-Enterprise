using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using Core.Common.CSV;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using WaterNut.Business.Services.Utils;

namespace WaterNut.Business.Services.Importers
{
    public class CSVDataExtractor : IDataExtractor
    {
        private readonly FileTypes _fileType;
        private readonly IEnumerable<string> _lines;
        private readonly IEnumerable<string> _header;
        private readonly string _emailId;

        public CSVDataExtractor(FileTypes fileType, IEnumerable<string> lines, IEnumerable<string> header, string emailId)
        {
            _fileType = fileType;
            _lines = lines;
            _header = header;
            _emailId = emailId;
        }
        public List<dynamic> Execute()
        {
           

            var mapping = GetMappings(_header.ToArray(), _fileType) ;
                


            var eslst = GetCSVDataSummayList(_lines.ToArray(), mapping, _header.ToArray(), _fileType);
            return eslst;
        }

        public List<dynamic> Execute(List<dynamic> list)
        {
            return Execute();
        }


        private List<dynamic> GetCSVDataSummayList(string[] lines, Dictionary<FileTypeMappings, int> mapping,
            string[] headings, FileTypes fileType)
        {



            int i = 0;
            try
            {
                var eslst = new List<dynamic>();

                for (i = 1; i < lines.Count(); i++)
                {

                    var d = GetCSVDataFromLine(lines[i], mapping, headings, fileType);
                    if (d != null)
                    {
                        d.LineNumber = i;
                        eslst.Add(d);
                    }
                }

                return eslst;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private Dictionary<FileTypeMappings, int> GetMappings(string[] headings, FileTypes fileType)
        {
            var mapping = new Dictionary<FileTypeMappings, int>();
            try
            {
                CheckMappingsForCarriageReturn(fileType);

                for (var i = 0; i < headings.Count(); i++)
                {
                    var h = headings[i].Trim().ToUpper();

                    if (h == "") continue;

                    var ftms = fileType.FileTypeMappings.Where(x =>
                        x.OriginalName.ToUpper().Trim() == h.ToUpper().Trim() /*|| x.DestinationName.ToUpper().Trim() == h.ToUpper().Trim()*/).ToList(); // added destination name to reduce redundancy
                    // took out destination because do duplicate "total cost & cost"
                    foreach (var ftm in ftms)
                    {
                        mapping.Add(ftm, i);
                    }
                }

                return mapping;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void CheckMappingsForCarriageReturn(FileTypes fileType)
        {
            foreach (var m in fileType.FileTypeMappings)
            {
                if (m.DestinationName.EndsWith("\r\n"))
                    throw new ApplicationException(
                        $"Mapping contain New Line: {m.DestinationName}");
                if (m.OriginalName.EndsWith("\r\n"))
                    throw new ApplicationException(
                        $"Mapping contain New Line: {m.OriginalName}");
            }
        }


        private dynamic GetCSVDataFromLine(string line, Dictionary<FileTypeMappings, int> map, string[] headings,
            FileTypes fileType)
        {

            try
            {
                if (string.IsNullOrEmpty(line)) return null;
                var splits = line.CsvSplit().Select(x => x.Trim()).ToArray();
                if (splits.Length < headings.Length) return null;

                //var requiredcnt = fileType.FileTypeMappings.Count(x => x.Required == true);


                dynamic res = new BetterExpando();
                res.SourceRow = line;
                foreach (var key in map.Keys)
                {
                    try
                    {
                        if (key.Required == true && string.IsNullOrEmpty(splits[map[key]])) return null;

                        if (string.IsNullOrEmpty(splits[map[key]]))
                        {
                            if (((IDictionary<string, object>)res).ContainsKey(key.DestinationName) &&
                                string.IsNullOrEmpty(
                                    ((IDictionary<string, object>)res)[key.DestinationName] as string))
                                ((IDictionary<string, object>)res)[key.DestinationName] = "";
                            continue;
                        }

                        if (ImportChecks.ContainsKey(key.DestinationName))
                        {

                            var err = ImportChecks[key.DestinationName].Invoke(res,
                                map
                                    //////// turn on for Filetype 125 itemnumber  --
                                    /// Turn off because- the mapping should only contains items in heading. see mapping code
                                    //.Where(x => headings.Contains(x.Key.OriginalName) )//|| headings.Contains(x.Key.DestinationName)
                                    .ToDictionary(x => x.Key.DestinationName, x => x.Value), splits);
                            if (err.Item1) throw new ApplicationException(err.Item2);
                        }

                        if (ImportActions.ContainsKey(key.DestinationName))
                        {// come up with a better solution cuz of duplicate keys
                            ImportActions[key.DestinationName].Invoke(res,
                                map
                                    //////// turn on for Filetype 125 itemnumber
                                    /// /// Turn off because- the mapping should only contains items in heading. see mapping code
                                    //.Where(x => headings.Contains(x.Key.OriginalName) )//|| headings.Contains(x.Key.DestinationName)
                                    .ToDictionary(x => x.Key.DestinationName, x => x.Value), splits);
                        }
                        else
                        {
                            ((IDictionary<string, object>)res)[key.DestinationName] =
                                GetMappingValue(key, splits, map[key]);
                        }


                    }
                    catch (Exception e)
                    {
                        var message =
                            $"Could not Import '{headings[map[key]]}' from Line:'{line}'. Error:{e.Message}";
                        Console.WriteLine(e);
                        throw new ApplicationException(message);
                    }

                }

                foreach (var action in fileType.ImportActions)
                {
                    try
                    {
                        ((IDictionary<string, object>)res)[action.Name] = DynamicQueryable.ReplaceMacro(
                            action.Action,
                            new ImportData()
                            {
                                res = (IDictionary<string, object>)res,
                                mapping = map
                                    .Where(x => headings.Contains(x.Key.OriginalName))
                                    .ToDictionary(x => x.Key.DestinationName, x => x.Value),
                                splits = splits
                            });
                    }
                    catch (Exception e)
                    {
                        // can't figure out how to test for missing columns so just skip if failed
                    }

                }

                return res;
                // }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        private object GetMappingValue(FileTypeMappings key, string[] splits, int index)
        {
            try
            {


                var val = splits[index];
                foreach (var regEx in key.FileTypeMappingRegExs)
                {

                    val = Regex.Replace(val, regEx.ReplacementRegex, regEx.ReplacementValue ?? "",
                        RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Multiline);
                }

                if (key.DataType == "Date")
                {
                    DateTime rdate;
                    if (DateTime.TryParse(val, out rdate))
                    {
                        return rdate;
                    }

                    var formatStrings = new List<string>() { "M/y", "M/d/y", "M-d-y", "dd/MM/yyyy", "dd/M/yyyy", "dddd dd MMMM yyyy" };
                    foreach (String formatString in formatStrings)
                    {
                        if (DateTime.TryParseExact(val, formatString, CultureInfo.InvariantCulture, DateTimeStyles.None,
                                out rdate))
                            return rdate;
                    }

                    return rdate; //DateTime.Parse(val);
                }

                if (key.DataType == "Number") return Convert.ToSingle(string.IsNullOrEmpty(val) ? "0" : val);
                return val;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public static Dictionary<string, Action<dynamic, Dictionary<string, int>, string[]>> ImportActions = new Dictionary<string, Action<dynamic, Dictionary<string, int>, string[]>>()
        {
            {
                "EntryDataId",
                (c, mapping, splits) => c.EntryDataId = splits[mapping["EntryDataId"]].Trim().Replace("PO/GD/", "")
                    .Replace("SHOP/GR_", "")
            },
            {
                "EntryDataDate",
                (c, mapping, splits) =>
                {
                    DateTime date = DateTime.MinValue;
                    var strDate = string.IsNullOrEmpty(splits[mapping["EntryDataDate"]])
                        ? DateTime.MinValue.ToShortDateString()
                        : splits[mapping["EntryDataDate"]].Replace("�", "");
                    if (DateTime.TryParse(strDate, out date) == false)
                        DateTime.TryParseExact(strDate, "dd'/'MM'/'yyyy",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out date);
                    c.EntryDataDate = date;
                }
            },
            {"ItemNumber", (c, mapping, splits) => c.ItemNumber = splits[mapping["ItemNumber"]].Replace("[", "")},
            //{
            //    "ItemAlias",
            //    (c, mapping, splits) =>
            //        c.ItemAlias = mapping.ContainsKey("ItemAlias") ? splits[mapping["ItemAlias"]] : ""
            //},
            {"ItemDescription", (c, mapping, splits) => c.ItemDescription = splits[mapping["ItemDescription"]]},
            {
                "Cost",
                (c, mapping, splits) => c.Cost = !mapping.ContainsKey("Cost")
                    ? 0
                    : Convert.ToSingle(string.IsNullOrEmpty(splits[mapping["Cost"]]) || splits[mapping["Cost"]] == "{NULL}"
                        ? "0"
                        : splits[mapping["Cost"]].Replace("$", "").Replace("�", "").Replace("USD", "").Trim())
            },
            {
                "Quantity",
                (c, mapping, splits) => c.Quantity = Convert.ToSingle(splits[mapping["Quantity"]].Replace("�", ""))
            },

            {
                "TotalCost",
                (c, mapping, splits) =>
                {
                    c.Cost = !string.IsNullOrEmpty(splits[mapping["TotalCost"]]) &&
                             Convert.ToSingle(splits[mapping["TotalCost"]].Replace("$", "")) !=
                             0.0
                        ? Convert.ToSingle(splits[mapping["TotalCost"]].Replace("$", "")) /
                          (c.Quantity ?? Convert.ToSingle(splits[mapping["Quantity"]].Replace("�", "")))
                        : c.Cost;

                    c.TotalCost = !string.IsNullOrEmpty(splits[mapping["TotalCost"]])
                        ? Convert.ToSingle(splits[mapping["TotalCost"]].Replace("$", ""))
                        : 0;

                }
            },
            {
                "SupplierItemNumber",
                (c, mapping, splits) =>
                {
                    c.ItemNumber = mapping.ContainsKey("POItemNumber") &&!string.IsNullOrEmpty(splits[mapping["POItemNumber"]])
                        ? splits[mapping["POItemNumber"]]
                        : splits[mapping["SupplierItemNumber"]];

                    c.SupplierItemNumber = splits[mapping["SupplierItemNumber"]];
                }
            },
            {
                "SupplierItemDescription",
                (c, mapping, splits) =>
                {
                    c.ItemDescription =
                        mapping.ContainsKey("POItemDescription") && !string.IsNullOrEmpty(splits[mapping["POItemDescription"]])
                            ? splits[mapping["POItemDescription"]]
                            : splits[mapping["SupplierItemDescription"]];
                    c.SupplierItemDescription = splits[mapping["SupplierItemDescription"]];
                }
            },
            {
                "SupplierInvoiceNo",
                (c, mapping, splits) =>
                {
                    if (string.IsNullOrEmpty(c.EntryDataId))// do this incase entrydataid empty.. might nee to check order?
                    {
                        c.EntryDataId = mapping.ContainsKey("EntryDataId") &&!string.IsNullOrEmpty(splits[mapping["EntryDataId"]])
                            ? splits[mapping["EntryDataId"]].Trim().Replace("PO/GD/", "").Replace("SHOP/GR_", "")
                            : splits[mapping["SupplierInvoiceNo"]];
                    }
                    c.SupplierInvoiceNo = splits[mapping["SupplierInvoiceNo"]];
                }
            },
            {
                "POItemNumber",
                (c, mapping, splits) => c.ItemNumber = !string.IsNullOrEmpty(splits[mapping["POItemNumber"]])
                    ? splits[mapping["POItemNumber"]]
                    : splits[mapping["SupplierItemNumber"]]
            },
            {
                "POItemDescription",
                (c, mapping, splits) => c.ItemDescription =
                    !string.IsNullOrEmpty(splits[mapping["POItemDescription"]])
                        ? splits[mapping["POItemDescription"]]
                        : splits[mapping["SupplierItemDescription"]]
            },
            {
                "Total",
                (c, mapping, splits) => { }
            },

        };


        public Dictionary<string, Func<dynamic, Dictionary<string, int>, string[], Tuple<bool, string>>> ImportChecks =
            new Dictionary<string, Func<dynamic, Dictionary<string, int>, string[], Tuple<bool, string>>>()
            {
                {
                    "EntryDataId",
                    (c, mapping, splits) =>
                        new Tuple<bool, string>(Regex.IsMatch(splits[mapping["EntryDataId"]], @"\d+E\+\d+"),
                            "Invoice # contains Excel E+ Error")
                },
                {
                    "ItemNumber",
                    (c, mapping, splits) =>
                        new Tuple<bool, string>(Regex.IsMatch(splits[mapping["ItemNumber"]], @"\d+E\+\d+"),
                            "ItemNumber contains Excel E+ Error")
                },

            };


    }
}