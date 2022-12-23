using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Core.Common.Converters;
using Core.Common.Data.Contracts;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using SimpleMvvmToolkit.ModelExtensions;
using TrackableEntities;
using TrackableEntities.Client;
using WaterNut.DataSpace;
using FileInfo = System.IO.FileInfo;
using MoreEnumerable = MoreLinq.MoreEnumerable;

namespace WaterNut.Business.Services.Utils
{
    public class CSVUtils
    {
        

        public static void ProcessUnknownCSVFileType(FileTypes ft, FileInfo[] fs)
        {
            throw new NotImplementedException();
        }

        public static void SaveCSVReport<T>(List<T> errors, string errorfile) where T : class, IIdentifiableEntity, ITrackable, INotifyPropertyChanged
        {
            var res = new ExportToCSV<T, List<T>>();
            res.IgnoreFields.AddRange(typeof(IIdentifiableEntity).GetProperties());
            res.IgnoreFields.AddRange(typeof(IEntityWithKey).GetProperties());
            res.IgnoreFields.AddRange(typeof(ITrackable).GetProperties());
            res.IgnoreFields.AddRange(typeof(Core.Common.Business.Entities.BaseEntity<T>).GetProperties());
            res.IgnoreFields.AddRange(typeof(Core.Common.Business.Entities.BaseEntity<T>).GetGenericTypeDefinition().GetProperties());
            res.IgnoreFields.AddRange(typeof(ITrackingCollection<T>).GetProperties());
            res.dataToPrint = errors;
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
                Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None, TaskCreationOptions.None, sta);
            }
        }

        public static void SaveCsv(IEnumerable<FileInfo> csvFiles, FileTypes fileType)
        {
            Console.WriteLine($"Importing CSV {fileType.FileImporterInfos.EntryType}");
            foreach (var file in csvFiles) TryImportFile(fileType, file);
        }

        private static void TryImportFile(FileTypes fileType, FileInfo file)
        {
            try
            {
                SaveCSVModel.Instance.ProcessDroppedFile(file.FullName, fileType, fileType.OverwriteFiles ?? true)
                    .Wait(); //set to false to merge
            }
            catch (Exception e)
            {
                EmailCSVImportError(file, e);
            }
        }

        private static void EmailCSVImportError(FileInfo file, Exception e)
        {
           
                if (IsErrorLogSent(file)) return;
                var att = GetCSVOriginalFileAttachments(file);
                var body = CreateCSVErrorEmailBody(file, e);
                EmailDownloader.EmailDownloader.SendBackMsg(att?.EmailId, BaseDataModel.GetClient(), body);
                if(att != null) SaveErrorLog(att);
            
        }

        private static bool IsErrorLogSent(FileInfo file)
        {
            return GetSentErrorLog(file) != null;
        }

        private static void SaveErrorLog(AsycudaDocumentSet_Attachments att)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.AttachmentLog.Add(new AttachmentLog(true)
                {
                    DocSetAttachment = att.Id,
                    Status = "Sender Informed of Error",
                    TrackingState = TrackingState.Added
                });
                ctx.SaveChanges();
            }
        }

        private static string CreateCSVErrorEmailBody(FileInfo file, Exception e)
        {
            var body = "Error While Importing: \r\n" +
                       $"File: {file}\r\n" +
                       $"Error: {(e.InnerException ?? e).Message.Replace(file.FullName, file.Name)} \r\n" +
                       $"Please Check the spreadsheet or inform Joseph Bartholomew if this is an Error.\r\n" +
                       $"Regards,\r\n" +
                       $"AutoBot";
            return body;
        }

        private static AsycudaDocumentSet_Attachments GetCSVOriginalFileAttachments(FileInfo file)
        {
            
            var att = new CoreEntitiesContext().AsycudaDocumentSet_Attachments.FirstOrDefault(x =>
                x.Attachments.FilePath.Contains(file.FullName.Replace(file.Extension, "").Replace("-Fixed", "")));
            return att;
        }

        private static AttachmentLog GetSentErrorLog(FileInfo file)
        {
            return new CoreEntitiesContext().AttachmentLog.FirstOrDefault(x =>
                x.AsycudaDocumentSet_Attachments.Attachments.FilePath == file.FullName
                && x.Status == "Sender Informed of Error");
        }

        public static void ReplaceCSV(FileInfo[] csvFiles, FileTypes fileType)
        {
            Console.WriteLine($"Importing CSV {fileType.FileImporterInfos.EntryType}");
            foreach (var file in csvFiles)
            {

                try
                {
                    SaveCSVModel.Instance.ProcessDroppedFile(file.FullName, fileType, true)//set to false to merge
                        .Wait();


                    //if (VerifyCSVImport(file))
                    //    return;
                    //else
                    //    continue;

                }
                catch (Exception e)
                {

                    using (var ctx = new CoreEntitiesContext())
                    {
                        if (ctx.AttachmentLog.FirstOrDefault(x =>
                                x.AsycudaDocumentSet_Attachments.Attachments.FilePath == file.FullName
                                && x.Status == "Sender Informed of Error") == null)
                        {
                            var att = ctx.AsycudaDocumentSet_Attachments.FirstOrDefault(x => x.Attachments.FilePath.Contains(file.FullName.Replace(file.Extension, "").Replace("-Fixed", "")));
                            var body = "Error While Importing: \r\n" +
                                       $"File: {file}\r\n" +
                                       $"Error: {(e.InnerException ?? e).Message.Replace(file.FullName, file.Name)} \r\n" +
                                       $"Please Check the spreadsheet or inform Joseph Bartholomew if this is an Error.\r\n" +
                                       $"Regards,\r\n" +
                                       $"AutoBot";
                            var emailId = ctx.AsycudaDocumentSet_Attachments
                                .FirstOrDefault(x => x.Attachments.FilePath.Contains(file.FullName.Replace(file.Extension, "").Replace("-Fixed", "")))?.EmailId;
                            EmailDownloader.EmailDownloader.SendBackMsg(emailId, BaseDataModel.GetClient(), body);
                            ctx.AttachmentLog.Add(new AttachmentLog(true)
                            {
                                DocSetAttachment = att.Id,
                                Status = "Sender Informed of Error",
                                TrackingState = TrackingState.Added
                            });
                            ctx.SaveChanges();
                        }
                    }


                }
            }
        }



        public static string StringToCSVCell(string str)
        {
            //bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") 
            //                  || str.Contains("\n") || str.Contains(".") || str.Contains("'") || str.Contains("#"));
            //if (mustQuote)
            //{
            var data = str.Replace("\n", "");

            StringBuilder sb = new StringBuilder();
            sb.Append("\"");
            foreach (char nextChar in data)
            {
                sb.Append(nextChar);
                if (nextChar == '"')
                    sb.Append("\"");
            }
            sb.Append("\"");
            
            return sb.ToString().Trim(' ',',');
            //}

            //return str;
        }

        public static void FixCsv(FileInfo file, FileTypes fileType, bool? overwrite)
        {

            var dic = FileTypeManager.PreCalculatedFileTypeMappings(fileType);
            try
            {


                if (GetDataRows(file, fileType, out var table, out var dRows, out var header)) return; // not right file type

                ImportRows(file, fileType, dic, dRows, header, table);


                ImportFile(file, fileType, overwrite, table);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }


        }

        private static bool GetDataRows(FileInfo file, FileTypes fileType, out ConcurrentDictionary<int, string> table, out List<DataRow> dRows,
            out DataRow header)
        {
            table = new ConcurrentDictionary<int, string>();
            dRows = new List<DataRow>();

            if (fileType.FileTypeMappings.Count == 0 && fileType.ReplicateHeaderRow == false)
            {
                throw new ApplicationException($"Missing File Type Mappings for {fileType.FilePattern}");
            }

            var dfile = new FileInfo(
                $@"{file.DirectoryName}\{file.Name.Replace(".csv", "")}-Fixed{file.Extension}");
            //if (dfile.Exists && dfile.LastWriteTime >= file.LastWriteTime) return;
            if (File.Exists(dfile.FullName)) File.Delete(dfile.FullName);
            // Reading from a binary Excel file (format; *.xlsx)
            var dt = CSV2DataTable(file, "NO");

            dt.Columns.Add("LineNumber", typeof(string));

            LoadDataRows(fileType, dRows, dt);

            RemoveDuplicateHeaders(fileType, dRows);

            header = dRows[0];
            header["LineNumber"] = "LineNumber".ToUpper();

            RemoveUnmappedColumns(header, dt);

            AddLineNumbers(dRows);

            
            return CheckMissingMappings(file, fileType, header);
        }

        private static bool CheckMissingMappings(FileInfo file, FileTypes fileType, DataRow header)
        {
            var headerlst = header.ItemArray.ToList();

            var missingMaps = fileType.FileTypeMappings.Where(x => x.Required && !x.OriginalName.Contains("{"))
                .GroupBy(x => x.DestinationName)
                .Where(item => item.All(z =>
                    !headerlst.Any(q =>
                        String.Equals(q.ToString(), z.OriginalName, StringComparison.CurrentCultureIgnoreCase))))
                .ToList(); // !headerlst  (item.OriginalName.ToUpper())
            if (missingMaps.Any())
            {
                EmailDownloader.EmailDownloader.SendEmail(BaseDataModel.GetClient(), null, $"Bug Found",
                    new[] {"Joseph@auto-brokerage.com"},
                    $"Required Field - '{missingMaps.Select(x => x.Key).Aggregate((o, n) => o + "," + n)}' in File: {file.Name} dose not exists.",
                    Array.Empty<string>());
                return true;
            }

            return false;
        }

        private static void AddLineNumbers(List<DataRow> dRows)
        {
            for (int i = 0; i < dRows.Count; i++)
            {
                dRows[i]["LineNumber"] = i;
            }
        }

        private static void RemoveUnmappedColumns(DataRow header, DataTable dt)
        {
            List<DataColumn> deleteColumns = new List<DataColumn>();
            for (int i = 0; i < header.ItemArray.Length - 1; i++)
            {
                if (string.IsNullOrEmpty(header[i].ToString()))
                {
                    deleteColumns.Add(dt.Columns[i]);
                    continue;
                }

                header[i] = header[i].ToString().ToUpper().Trim();
            }

            deleteColumns.ForEach(x => dt.Columns.Remove(x));
        }

        private static void RemoveDuplicateHeaders(FileTypes fileType, List<DataRow> dRows)
        {
            if (fileType.FileTypeMappings.Any())
            {
                var dupheaders = dRows.Where(x =>
                        x.ItemArray.Contains(fileType.FileTypeMappings.OrderBy(z => z.Id)
                            .First(z => !z.OriginalName.Contains("{")).OriginalName)).Skip(1)
                    .ToList();
                foreach (var row in dupheaders)
                {
                    dRows.Remove(row);
                }
            }
        }

        private static void LoadDataRows(FileTypes fileType, List<DataRow> dRows, DataTable dt)
        {
            var headerRow = Enumerable.ToList<object>(dt.Rows[0].ItemArray);
            int drow_no = 0;
            DataRow currentReplicatedHeading = null;
            while (drow_no < dt.Rows.Count)
            {
                if (fileType.ReplicateHeaderRow == true)
                    currentReplicatedHeading = ProcessReplicatedHeaderRows(fileType, dRows, dt, drow_no, headerRow, currentReplicatedHeading);
                else
                    ProcessHeaderRows(fileType, dRows, dt, drow_no);

                drow_no++;
            }
        }

        private static void ProcessHeaderRows(FileTypes fileType, List<DataRow> dRows, DataTable dt, int drow_no)
        {
            if (Enumerable.Count<object>(dt.Rows[drow_no].ItemArray, x => !string.IsNullOrEmpty(x.ToString())) >=
                fileType.FileTypeMappings.Count(x => x.Required))
            {
                //dt.Rows[drow_no]["LineNumber"] = drow_no;// give value to prevent upper from bugging later
                dRows.Add(dt.Rows[drow_no]);
            }
        }

        private static DataRow ProcessReplicatedHeaderRows(FileTypes fileType, List<DataRow> dRows, DataTable dt,
            int drow_no,
            List<object> headerRow, DataRow currentReplicatedHeading)
        {
            
            if (fileType.FileTypeMappings.Where(x => x.Required).All(x =>
                    !string.IsNullOrEmpty(dt.Rows[drow_no][headerRow.IndexOf(x.OriginalName)].ToString())))
            {
                if (Enumerable.Count<object>(dt.Rows[drow_no].ItemArray, x => !string.IsNullOrEmpty(x.ToString())) >=
                    (fileType.FileTypeMappings.Any()
                        ? fileType.FileTypeMappings.Count(x => x.Required)
                        : 1))
                {
                    currentReplicatedHeading = dt.Rows[drow_no];
                    dRows.Add(dt.Rows[drow_no]);
                }
            }
            else
            {
                if (Enumerable.Count<object>(dt.Rows[drow_no].ItemArray, x => !string.IsNullOrEmpty(x.ToString())) >=
                    (fileType.FileTypeMappings.Any() ? fileType.FileTypeMappings.Count(x => x.Required) : 1))
                {
                    var row = dt.NewRow();
                    foreach (DataColumn col in dt.Columns)
                    {
                        var val = dt.Rows[drow_no][col.ColumnName].ToString();
                        if (!string.IsNullOrEmpty(val) &&
                            string.IsNullOrEmpty(row[col.ColumnName].ToString()))
                            row[col.ColumnName] = val;
                        if (string.IsNullOrEmpty(val) &&
                            !string.IsNullOrEmpty(currentReplicatedHeading[col.ColumnName].ToString()))
                            row[col.ColumnName] = currentReplicatedHeading[col.ColumnName].ToString();
                    }

                    dRows.Add(row);
                }
            }

            return currentReplicatedHeading;

        }

        private static void ImportFile(FileInfo file, FileTypes fileType, bool? overwrite, ConcurrentDictionary<int, string> table)
        {
            var output = CreateFile(file, table);
            if (fileType.ChildFileTypes.Any())
            {
                ImportChildFileTypes(fileType, overwrite, output);
            }
            else
            {
                SaveCsv(new FileInfo[] { new FileInfo(output) }, fileType);
            }
        }

        private static void ImportRows(FileInfo file, FileTypes fileType, Dictionary<string, Func<IDictionary<string, object>, IDictionary<string, object>, IDictionary<string, object>, string>> dic, List<DataRow> dRows, DataRow header,
            ConcurrentDictionary<int, string> table)
        {
            var mappingMailSent = false;
            Parallel.ForEach(dRows, new ParallelOptions() { MaxDegreeOfParallelism = 1 }, //Environment.ProcessorCount *
                drow =>
                {
                    var row = new Dictionary<string, string>();
                    var row_no = drow["LineNumber"].ToString() == $"LineNumber".ToUpper()
                        ? 0
                        : Convert.ToInt32(drow["LineNumber"]);
                    if (fileType.FileTypeMappings.Any())
                    {
                        mappingMailSent =
                            UpdateRowWithFileMapping(file, fileType, dic, mappingMailSent, header, row_no, drow, ref row);
                    }
                    else
                    {
                        UpdateRowFromFile(header, drow, row);
                    }


                    AddRowToTable(fileType, row, table, row_no);
                });
        }

        private static void ImportChildFileTypes(FileTypes fileType, bool? overwrite, string output)
        {
            foreach (var cfileType in fileType.ChildFileTypes.Where(x => x.FileTypeMappings.Any() && x.FileImporterInfos.Format == FileTypeManager.FileFormats.Csv))
            {
                var cfileTypes = FileTypeManager.GetFileType(cfileType);
                cfileTypes.AsycudaDocumentSetId = fileType.AsycudaDocumentSetId;
                cfileTypes.EmailId = fileType.EmailId;
                cfileTypes.OverwriteFiles = overwrite;
                var fileInfos = new FileInfo[] { new FileInfo(output) };
                SaveCsv(fileInfos, cfileTypes);
            }
        }

        private static string CreateFile(FileInfo file, ConcurrentDictionary<int, string> table)
        {
            string output = $@"{file.DirectoryName}\{file.Name.Replace(".csv", "")}-Fixed{file.Extension}";
            StreamWriter csv = new StreamWriter(output, false);
            csv.Write(table.OrderBy(x => x.Key).Select(x => x.Value).Aggregate((a, x) => a + x));
            csv.Close();
            return output;
        }

        private static void AddRowToTable(FileTypes fileType, Dictionary<string, string> row, ConcurrentDictionary<int, string> table, int row_no)
        {
            if (row.Count > 0 
                && row.Count(x => !string.IsNullOrEmpty(x.Value) && fileType.FileTypeMappings.Any(z => z.Required == true && z.DestinationName == x.Key)) 
                >= MoreEnumerable.DistinctBy(fileType.FileTypeMappings, x => x.DestinationName).Count(x => x.Required == true))
            {
                var value = fileType.FileTypeMappings.Any()
                    ? fileType.FileTypeMappings.OrderBy(x => x.Id).Select(x => x.DestinationName).Where(x => !x.StartsWith("{"))
                        .Distinct()
                        .Select(x => row.ContainsKey(x) ? row[x] : "")
                        .Aggregate((a, x) => a + "," + x) + "\n"
                    : row.Values.Aggregate((a, x) => a + "," + x) + "\n";
                table.GetOrAdd(row_no, value);
            }
        }

        private static void UpdateRowFromFile(DataRow header, DataRow drow, Dictionary<string, string> row)
        {
            foreach (var h in header.ItemArray)
            {
                var index = Array.LastIndexOf(header.ItemArray,
                    h); //last index of because of Cost USD file has two columns
                var val = drow[index].ToString();
                if (!row.ContainsKey(h.ToString())) row.Add(h.ToString(), StringToCSVCell(val));
            }
        }

        private static bool UpdateRowWithFileMapping(FileInfo file, FileTypes fileType, Dictionary<string, Func<IDictionary<string, object>, IDictionary<string, object>, IDictionary<string, object>, string>> dic, bool mappingMailSent,
            DataRow header, int row_no, DataRow drow, ref Dictionary<string, string> row)
        {
            foreach (var mapping in OrderFileTypeMappingsByOriginalName(fileType).Where(x => header.ItemArray.Contains(x.OriginalName) || x.OriginalName.StartsWith("{")))
            {
                var maps = mapping.OriginalName.Split('+');
                string val = null;
                foreach (var map in maps)
                {
                    mappingMailSent =
                        CheckingRequiredFields(file, fileType, dic, header, map, mapping, mappingMailSent, row_no);

                    if (GetRawValue(dic, row_no, mapping, maps, map, row, drow, header, ref val))
                        continue;
                    else
                        break;
                }
                if (val == null || string.IsNullOrEmpty(val)) continue;
                foreach (var regEx in mapping.FileTypeMappingRegExs)
                {
                    val = Regex.Replace(val, regEx.ReplacementRegex, regEx.ReplacementValue ?? "",
                        RegexOptions.IgnoreCase | RegexOptions.Multiline);
                }

                val = ProcessValue(file, fileType, dic, val, row_no, header, mapping, row, drow);

                row = UpdateRow(row, mapping, row_no, val);
            }

            return mappingMailSent;
        }

        private static IOrderedEnumerable<FileTypeMappings> OrderFileTypeMappingsByOriginalName(FileTypes fileType) => fileType.FileTypeMappings.OrderByDescending(x => x.OriginalName);

        private static Dictionary<string, string> UpdateRow(Dictionary<string, string> row, FileTypeMappings mapping, int row_no, string val)
        {
            if (row.ContainsKey(mapping.DestinationName))
            {
                if (row_no == 0)
                {
                    row.Remove(mapping.DestinationName);
                    var nrow = new Dictionary<string, string>();
                    nrow = row.Clone();
                    nrow.Add(mapping.DestinationName, StringToCSVCell(val));
                    row = nrow;
                    // row.Add(mapping.DestinationName, StringToCSVCell(val));
                }
                else
                {
                    if (val == "{NULL}" && !string.IsNullOrEmpty(row[mapping.DestinationName])) return row; //don't overwrite good value
                    row[mapping.DestinationName] = StringToCSVCell(val);
                }
            }
            else
            {
                row.Add(mapping.DestinationName, StringToCSVCell(val));
            }

            return row;
        }

        private static string ProcessValue(FileInfo file, FileTypes fileType, Dictionary<string, Func<IDictionary<string, object>, IDictionary<string, object>, IDictionary<string, object>, string>> dic, string val, int row_no,
            DataRow header, FileTypeMappings mapping, Dictionary<string, string> row, DataRow drow)
        {
            var errLst = new List<string>();
            if (val == "{NULL}") return val;
            if (val == "" && row_no == 0) return val;
            if (val == "" && row_no > 0 &&
                (!header.ItemArray.Contains(mapping.OriginalName.ToUpper()) &&
                 !dic.ContainsKey(mapping.OriginalName.Replace("{", "").Replace("}", "")))) return val;
            if (row_no > 0)
                if (string.IsNullOrEmpty(val) &&
                    (mapping.Required == true || mapping.DestinationName == "Invoice #")
                   ) // took out because it will replace invoice no regardless
                {
                    if (mapping.DestinationName == "Invoice #")
                    {
                        val += dic["DIS-Reference"].Invoke(row.ToDynamic(), drow.ToDynamic(), header.ToDynamic());
                    }
                    else
                    {
                        //EmailDownloader.EmailDownloader.SendEmail(Utils.Client, null, $"Bug Found",
                        //    new[] { "Joseph@auto-brokerage.com" }, $"Required Field - '{mapping.OriginalName}' on Line:{ row_no} in File: { file.Name} has no Value.", Array.Empty<string>());
                        errLst.Add($"Required Field - '{mapping.OriginalName}' on Line:{row_no} in File: {file.Name} has no Value.");
                        return val;
                    }
                }

                else if (mapping.DataType == "Number")
                {
                    if (string.IsNullOrEmpty(val)) val = "0";
                    if (val.ToCharArray().All(x => !char.IsDigit(x)))
                    {
                        //EmailDownloader.EmailDownloader.SendEmail(Utils.Client, null, $"Bug Found",
                        //    new[] { "Joseph@auto-brokerage.com" }, $"Required Field - '{mapping.OriginalName}' on Line:{ row_no} in File: { file.Name} has no Value.", Array.Empty<string>());
                        errLst.Add($"Required Field - '{mapping.OriginalName}' on Line:{row_no} in File: {file.Name} has Value ='{val}' cannot be converted to Number.");
                        return val;
                        //val = "";
                    }
                }
                else if (mapping.DataType == "Date")
                {
                    if (string.IsNullOrEmpty(val)) return val;
                    if (DateTime.TryParse(val, out var tmp) == false)
                    {
                        //EmailDownloader.EmailDownloader.SendEmail(Utils.Client, null, $"Bug Found",
                        //    new[] { "Joseph@auto-brokerage.com" }, $"Required Field - '{mapping.OriginalName}' on Line:{ row_no} in File: { file.Name} has no Value.", Array.Empty<string>());
                        errLst.Add($"Required Field - '{mapping.OriginalName}' on Line:{row_no} in File: {file.Name} has Value ='{val}' cannot be converted to date.");
                        return val;
                        //  val = "";
                    }
                }
            if(errLst.Any())
                EmailDownloader.EmailDownloader.ForwardMsg(fileType.EmailId,
                    BaseDataModel.GetClient(), $"Bug Found",
                    errLst.Aggregate((o,n) => o + "\r\n" + n),
                    new[] { "Joseph@auto-brokerage.com" }, Array.Empty<string>()
                );

            return val;
        }

        private static bool GetRawValue(Dictionary<string, Func<IDictionary<string, object>, IDictionary<string, object>, IDictionary<string, object>, string>> dic, int row_no, FileTypeMappings mapping, string[] maps, string map,
            Dictionary<string, string> row, DataRow drow, DataRow header, ref string val)
        {
            if (row_no == 0)
            {
                val += mapping.DestinationName;
                if (maps.Length > 1) return false;
            }
            else
            {
                //if (string.IsNullOrEmpty(dt.Rows[row_no][map].ToString())) continue;
                if (map.Contains("{") && dic.ContainsKey(map.Replace("{", "").Replace("}", "")))
                {
                    val += dic[map.Replace("{", "").Replace("}", "")].Invoke(ToBetterExpando(row), ToBetterExpando(drow), ToBetterExpando(header));
                }
                else
                {
                    var index = Array.LastIndexOf(header.ItemArray,
                        map.ToUpper().Trim()); //last index of because of Cost USD file has two columns
                    if (index == -1) return true;
                    val += drow[index];
                }

                if (maps.Length > 1 && map != maps.Last()) val += " - ";
            }

            return false;
        }

        private static IDictionary<string, object> ToBetterExpando(DataRow drow)
        {
            IDictionary<string, object> res = new BetterExpando();
            for (int i = 0; i < drow.Table.Columns.Count - 1; i++)
            {
                res[drow.Table.Columns[i].ColumnName] = drow.ItemArray[i];
            }
            
            return res;
        }

        private static IDictionary<string, object> ToBetterExpando(Dictionary<string, string> row)
        {
            IDictionary<string, object> res = new BetterExpando();
            foreach (var itm in row)
            {
                res[itm.Key] = itm.Value;
            }
            return res;
        }

        private static bool CheckingRequiredFields(FileInfo file, FileTypes fileType, Dictionary<string, Func<IDictionary<string, object>, IDictionary<string, object>, IDictionary<string, object>, string>> dic, DataRow header,
            string map, FileTypeMappings mapping, bool mappingMailSent, int row_no)
        {
            if (!header.ItemArray.Contains(map.ToUpper()) &&
                !dic.ContainsKey(map.Replace("{", "").Replace("}", "")))
            {
                if (mapping.Required)
                {
                    if (mappingMailSent) return mappingMailSent;
                    EmailDownloader.EmailDownloader.ForwardMsg(fileType.EmailId,
                        BaseDataModel.GetClient(), $"Bug Found",
                        $"Required Field - '{mapping.OriginalName}' on Line:{row_no} in File: {file.Name} dose not exists.",
                        new[] { "Joseph@auto-brokerage.com" }, Array.Empty<string>()
                    );
                    mappingMailSent = true;
                    return mappingMailSent;
                }

                //TODO: log error
                return mappingMailSent;
            }

            return mappingMailSent;
        }

        public static DataTable CSV2DataTable(FileInfo file, string headers)
        {
            var conn = Environment.Is64BitOperatingSystem == false
                ? new OleDbConnection(string.Format(
                    @"Provider=Microsoft.Jet.OleDb.4.0; Data Source={0};" +
                    $"Extended Properties=\"Text;HDR={headers};FMT=Delimited;CharacterSet=65001\"",
                    file.DirectoryName))
                : new OleDbConnection(string.Format(
                    @"Provider=Microsoft.ACE.OLEDB.16.0; Data Source={0};" +
                    $"Extended Properties=\"Text;HDR={headers};FMT=Delimited;CharacterSet=65001\"",
                    file.DirectoryName));


            conn.Open();

            string sql = string.Format("select * from [{0}]", Path.GetFileName(file.Name));
            OleDbCommand cmd = new OleDbCommand(sql, conn);
            OleDbDataReader reader = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();

            return dt;
        }

        
    }
}