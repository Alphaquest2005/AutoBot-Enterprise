using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using MoreLinq;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Utils
{
    public static class FileTypeManager
    {
        private static List<FileTypes> _fileTypes;
        public static FileTypes GetFileType(FileTypes fileTypes) => Enumerable.First<FileTypes>(FileTypes(), x => x.Id == fileTypes.Id);

        private static List<FileTypes> FileTypes()
        {
            try
            {
                if (_fileTypes == null || BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId !=
                    _fileTypes.First().ApplicationSettingsId)
                    using (var ctx = new CoreEntitiesContext())
                    {
                        _fileTypes = ctx.FileTypes
                            .Include("FileTypeContacts.Contacts")
                            .Include("FileTypeActions.Actions")
                           // .Include("AsycudaDocumentSetEx")
                            .Include("ChildFileTypes")
                            .Include("FileTypeMappings.FileTypeMappingRegExs")
                            .Include(x => x.FileImporterInfos)
                            .Include(x => x.ImportActions)
                            .Include(x => x.FileTypeReplaceRegex)
                            .Where(x => x.FileImporterInfos != null)
                            .Where(x => x.ApplicationSettingsId ==
                                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                            .ToList();

                    }

                return _fileTypes;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static FileTypes GetFileType(int fileTypeId)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var fileType = ctx.FileTypes.FirstOrDefault(x => x.Id == fileTypeId);
                return fileType != null ? GetFileType(fileType) : null;
            }
        }

        public static FileTypes GetHeadingFileType(IEnumerable<string> heading, FileTypes suggestedfileType)
        {

            using (var ctx = new CoreEntitiesContext())
            {
                var mappingFileType =
                    ctx.FileTypes
                        .Include(x => x.FileTypeMappings)
                        .Include(x => x.ImportActions)
                        .Where(x => x.ApplicationSettingsId ==
                                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                                    x.FileTypeMappings.Any())
                        .SelectMany(x => x.FileTypeMappings.Where(z =>
                            heading.Select(h => h.ToUpper().Trim()).Contains(z.OriginalName.ToUpper().Trim())))
                        .GroupBy(x => x.FileTypes)
                        .OrderByDescending(x => x.Count())
                        .Where(x => x.Key.IsImportable == null || x.Key.IsImportable == true)
                        .Where(x => x.Key.FileImporterInfos.Format == suggestedfileType.FileImporterInfos.Format)
                        .FirstOrDefault(x =>
                            suggestedfileType.FileImporterInfos.EntryType == EntryTypes.Unknown
                                ?( x.Key.FileImporterInfos.EntryType != null )
                                : x.Key.FileImporterInfos.EntryType == suggestedfileType.FileImporterInfos.EntryType)?.Key;


                FileTypes fileType;
                if (mappingFileType != null
                    && mappingFileType.Id != suggestedfileType.Id
                    && mappingFileType.Id != suggestedfileType.ParentFileTypeId)
                {
                    fileType = GetFileType(mappingFileType);
                }
                else
                {
                    fileType = GetFileType(suggestedfileType);
                }

                return fileType;

            }
        }


     

        public static class EntryTypes
        {
            public const string Unknown = "Unknown";
            public const string Po = "PO";
            public const string Sales = "Sales";
            public const string Inv = "INV";
            public const string ShipmentInvoice = "Shipment Invoice";
            public const string Ops = "OPS";
            public const string Adj = "ADJ";
            public const string Dis = "DIS";
            public const string Rcon = "RCON";
            public const string CancelledEntries = "CancelledEntries";
            public const string ExpiredEntries = "ExpiredEntries";
            public const string Freight = "Freight";
            public const string BL = "BL";
            public const string Manifest = "Manifest";
            public const string Rider = "Rider";
            public const string SubItems = "SubItems";
            public const string C71 = "C71";
            public const string Lic = "LIC";
            public const string POTemplate = "POTemplate";
            public const string Info = "Info";
            public const string xSales = "xSales";
        }


        public static class FileFormats
        {
            public const string Csv = "CSV";
            public const string Xlsx = "XLSX";
            public const string PDF = "PDF";
            public const string XML = "XML";
        }

        public static List<FileTypes> GetImportableFileType(string entryType, string fileFormat, string fileName) =>
            FileTypes()
                .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                .Where(x => (x.FileImporterInfos?.EntryType == entryType || entryType == EntryTypes.Unknown) && x.FileImporterInfos?.Format == fileFormat)
                .Where(x => x.FileTypeMappings.Any() || entryType == EntryTypes.Unknown || x.FileImporterInfos?.Format == FileTypeManager.FileFormats.PDF)
                .Where(x => x.ParentFileTypeId == null)
                .Where(x => Regex.IsMatch(fileName, x.FilePattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture))
                .Select(x =>
                {
                    x.AsycudaDocumentSetId = EntryDocSetUtils.GetAsycudaDocumentSet(x.DocSetRefernece, true)
                            .AsycudaDocumentSetId;
                    return x;
                })
                .ToList();

        public static void SendBackTooBigEmail(FileInfo file, FileTypes fileType)
        {
            if (fileType.MaxFileSizeInMB != null && (file.Length / WaterNut.DataSpace.Utils._oneMegaByte) > fileType.MaxFileSizeInMB)
            {
                var errTxt =
                    "Hey,\r\n\r\n" +
                    $@"Attachment: '{file.Name}' is too large to upload into Asycuda ({Math.Round((double)(file.Length / WaterNut.DataSpace.Utils._oneMegaByte), 2)}). Please remove Formatting or Cut into Smaller chuncks and Resend!" +
                    "Thanks\r\n" +
                    "AutoBot";
                EmailDownloader.EmailDownloader.SendBackMsg(fileType.EmailId, BaseDataModel.GetClient(), errTxt);
            }
        }

        public static object MappingValueToType(FileTypeMappings key, string val)
        {
            if(val == "{NULL}") return null;
            if (key.DataType == "Date")
            {
                DateTime rdate;
                if (DateTime.TryParse(val, out rdate))
                {
                    return rdate;
                }

                var formatStrings = new List<string>()
                    { "M/y", "M/d/y", "M-d-y", "dd/MM/yyyy", "dd/M/yyyy", "dddd dd MMMM yyyy" };
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


        public static Dictionary<string, Func<IDictionary<string, object>, IDictionary<string, object>, IDictionary<string, object>, string>> PreCalculatedFileTypeMappings(FileTypes fileType)
        {
            var dic = new Dictionary<string, Func<IDictionary<string, object>, IDictionary<string, object>, IDictionary<string, object>, string>>()
            {
                { "CurrentDate", (dt, drow, header) => DateTime.Now.Date.ToShortDateString() },
                {
                    "DIS-Reference",
                    (dt, drow, header) =>
                        $"DIS-{new DocumentDSContext().AsycudaDocumentSets.First(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId).Declarant_Reference_Number}"
                },
                {
                    "ADJ-Reference",
                    (dt, drow, header) =>
                        $"ADJ-{DateTime.Parse(drow["Date".ToUpper()].ToString()):MMM-yy}"// can't think how to replicate and don't think its a problem //  $"ADJ-{DateTime.Parse(drow[Array.LastIndexOf(header.ItemArray, "Date".ToUpper())].ToString()):MMM-yy}"
                }, //adjReference
                {
                    "Quantity",
                    (dt, drow, header) => dt.ContainsKey("Received Quantity") && dt.ContainsKey("Invoice Quantity")
                        ? Convert.ToString(
                            Math.Abs(Convert.ToDouble(dt["Received Quantity"].ToString().Replace("\"", "")) -
                                     Convert.ToDouble(dt["Invoice Quantity"].ToString().Replace("\"", ""))), CultureInfo.CurrentCulture)
                        : Convert.ToDouble(dt["Quantity"].ToString().Replace("\"", "")).ToString(CultureInfo.CurrentCulture)
                },
                { "ZeroCost", (x, drow, header) => "0" },
                {
                    "ABS-Added",
                    (dt, drow, header) => Math.Abs(Convert.ToDouble(dt["{Added}"].ToString().Replace("\"", "")))
                        .ToString(CultureInfo.CurrentCulture)
                },
                {
                    "ABS-Removed",
                    (dt, drow, header) => Math.Abs(Convert.ToDouble(dt["{Removed}"].ToString().Replace("\"", "")))
                        .ToString(CultureInfo.CurrentCulture)
                },
                {
                    "ADJ-Quantity",
                    (dt, drow, header) =>
                        Convert.ToString(
                            Math.Abs((Math.Abs(Convert.ToDouble(dt["{Added}"].ToString().Replace("\"", ""))) -
                                      Math.Abs(Convert.ToDouble(dt["{Removed}"].ToString().Replace("\"", ""))))),
                            CultureInfo.CurrentCulture)
                },
                {
                    "Cost2USD",
                    (dt, drow, header) => dt.ContainsKey("{XCDCost}") && Convert.ToDouble(dt["{XCDCost}"].ToString().Replace("\"", "")) > 0
                        ? (Convert.ToDouble(dt["{XCDCost}"].ToString().Replace("\"", "")) / 2.7169).ToString(CultureInfo.CurrentCulture)
                        : "{NULL}"
                },
            };
            return dic;
        }

        public static Dictionary<string, Func<IDictionary<string, object>, IDictionary<string, object>, IDictionary<string, object>, string>> PostCalculatedFileTypeMappings(FileTypes fileType)
        {
            var dic = new Dictionary<string, Func<IDictionary<string, object>, IDictionary<string, object>, IDictionary<string, object>, string>>()
            {
                { "CurrentDate", (dt, drow, header) => DateTime.Now.Date.ToShortDateString() },
                {
                    "DIS-Reference",
                    (dt, drow, header) =>
                        $"DIS-{new DocumentDSContext().AsycudaDocumentSets.First(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId).Declarant_Reference_Number}"
                },
                {
                    "ADJ-Reference",
                    (dt, drow, header) =>
                        $"ADJ-{DateTime.Parse(drow["Date".ToUpper()].ToString()):MMM-yy}"// can't think how to replicate and don't think its a problem //  $"ADJ-{DateTime.Parse(drow[Array.LastIndexOf(header.ItemArray, "Date".ToUpper())].ToString()):MMM-yy}"
                }, //adjReference
                {
                    "Quantity",
                    (dt, drow, header) => dt.ContainsKey("ReceivedQuantity") && dt.ContainsKey("InvoiceQuantity")
                        ? Convert.ToString(
                            Math.Abs(Convert.ToDouble(dt["ReceivedQuantity"].ToString().Replace("\"", "")) -
                                     Convert.ToDouble(dt["InvoiceQuantity"].ToString().Replace("\"", ""))), CultureInfo.CurrentCulture)
                        : Convert.ToDouble(dt["Quantity"].ToString().Replace("\"", "")).ToString(CultureInfo.CurrentCulture)
                },
                { "ZeroCost", (x, drow, header) => "0" },
                {
                    "ABS-Added",
                    (dt, drow, header) => Math.Abs(Convert.ToDouble(dt["{Added}"].ToString().Replace("\"", "")))
                        .ToString(CultureInfo.CurrentCulture)
                },
                {
                    "ABS-Removed",
                    (dt, drow, header) => Math.Abs(Convert.ToDouble(dt["{Removed}"].ToString().Replace("\"", "")))
                        .ToString(CultureInfo.CurrentCulture)
                },
                {
                    "ADJ-Quantity",
                    (dt, drow, header) =>
                        Convert.ToString(
                            Math.Abs((Math.Abs(Convert.ToDouble(dt["{Added}"].ToString().Replace("\"", ""))) -
                                      Math.Abs(Convert.ToDouble(dt["{Removed}"].ToString().Replace("\"", ""))))),
                            CultureInfo.CurrentCulture)
                },
                {
                    "Cost2USD",
                    (dt, drow, header) => dt.ContainsKey("{XCDCost}") && Convert.ToDouble(dt["{XCDCost}"].ToString().Replace("\"", "")) > 0
                        ? (Convert.ToDouble(dt["{XCDCost}"].ToString().Replace("\"", "")) / 2.7169).ToString(CultureInfo.CurrentCulture)
                        : "{NULL}"
                },
            };
            return dic;
        }


        public static string ApplyFileMapRegEx(FileTypeMappings key, string val)
        {
            foreach (var regEx in key.FileTypeMappingRegExs)
            {
                val = Regex.Replace(val, regEx.ReplacementRegex, regEx.ReplacementValue ?? "",
                    RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Multiline);
            }

            return val;
        }

        public static FileTypes GetHeadingFileType(List<DataRow> rows, FileTypes suggestedfileType)
        {
            var fileType = suggestedfileType;
            foreach (var row in rows)
            {
                var res = GetHeadingFileType(row.ItemArray.Select(x => x.ToString()), suggestedfileType);
                if (res != null) return res;
            }

            return fileType;
        }
    }
}
