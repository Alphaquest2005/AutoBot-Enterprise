using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoBot;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using ExcelDataReader;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using Utils = AutoBot.Utils;

namespace AutoBotUtilities.CSV
{
    public static class XLSXImporter
    {
        public static void Xlsx2csv(FileInfo[] files, FileTypes fileType, bool? overwrite = null )
        {
            try
            {

                var adjReference = $"ADJ-{new DocumentDSContext().AsycudaDocumentSets.First(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId).Declarant_Reference_Number}";
                var disReference = $"DIS-{new DocumentDSContext().AsycudaDocumentSets.First(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId).Declarant_Reference_Number}";
                var dic = new Dictionary<string, Func<Dictionary<string, string>,DataRow,DataRow, string>>()
                {
                    {"CurrentDate", (dt, drow, header)=> DateTime.Now.Date.ToShortDateString() },
                    { "DIS-Reference", (dt, drow, header) => disReference},
                    { "ADJ-Reference", (dt, drow, header) => $"ADJ-{DateTime.Parse(drow[Array.LastIndexOf(header.ItemArray,"Date".ToUpper())].ToString()):MMM-yy}"},//adjReference
                    {"Quantity", (dt, drow, header) => dt.ContainsKey("Received Quantity") && dt.ContainsKey("Invoice Quantity")? Convert.ToString(Math.Abs(Convert.ToDouble(dt["Received Quantity"].Replace("\"","")) - Convert.ToDouble(dt["Invoice Quantity"].Replace("\"",""))), CultureInfo.CurrentCulture) : Convert.ToDouble(dt["Quantity"].Replace("\"","")).ToString(CultureInfo.CurrentCulture) },
                    {"ZeroCost", (x, drow, header) => "0" },
                    {"ABS-Added", (dt, drow, header) =>  Math.Abs(Convert.ToDouble(dt["{Added}"].Replace("\"",""))).ToString(CultureInfo.CurrentCulture) },
                    {"ABS-Removed", (dt, drow, header) => Math.Abs(Convert.ToDouble(dt["{Removed}"].Replace("\"",""))).ToString(CultureInfo.CurrentCulture) },
                    {"ADJ-Quantity", (dt, drow, header) => Convert.ToString(Math.Abs((Math.Abs(Convert.ToDouble(dt["{Added}"].Replace("\"",""))) - Math.Abs(Convert.ToDouble(dt["{Removed}"].Replace("\"",""))))), CultureInfo.CurrentCulture) },
                    {"Cost2USD", (dt, drow, header) => dt.ContainsKey("{XCDCost}") && Convert.ToDouble(dt["{XCDCost}"].Replace("\"","")) > 0 ? (Convert.ToDouble(dt["{XCDCost}"].Replace("\"",""))/2.7169).ToString(CultureInfo.CurrentCulture) : "{NULL}" },
                };

                foreach (var file in files)
                {
                   
                    var result = ExtractTables(file);


                    if (result.Tables.Contains("MisMatches") && result.Tables.Contains("POTemplate")) ShipmentUtils.ReadMISMatches(result.Tables["MisMatches"], result.Tables["POTemplate"]);

                    var mainTable = result.Tables[0];
                    var rows = FixupDataSet(mainTable);
                    var fileText = GetText(fileType, rows, result);

                    if (ProcessUnknownFileType(fileType, file, rows)) continue;



                    


                    string output = Path.ChangeExtension(file.FullName, ".csv");
                    StreamWriter csv = new StreamWriter(output, false);
                    
                    csv.Write(fileText);
                    csv.Close();

                    CSVUtils.FixCsv(new FileInfo(output), fileType, dic, overwrite);

                }

            }
            catch (Exception e)
            {

                throw;
            }
        }

        private static string GetText(FileTypes fileType, List<DataRow> rows, DataSet result)
        {
            var table = new ConcurrentDictionary<int, string>();
            Parallel.ForEach(rows, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount * 1 },
                row =>
                {
                    StringBuilder a = new StringBuilder();

                    if (fileType.FileTypeMappings.Any() && fileType.FileTypeMappings.Select(x => x.OriginalName)
                            .All(x => row.ItemArray.Contains(x)))
                    {
                        //if(dic.ContainsKey())
                        a.Append("");
                    }

                    for (int i = 0; i < result.Tables[0].Columns.Count - 1; i++)
                    {
                        a.Append(CSVUtils.StringToCSVCell(row[i].ToString()) + ",");
                    }


                    a.Append("\n");
                    table.GetOrAdd(Convert.ToInt32(row["LineNumber"]), a.ToString());
                });

            var aggregate = table.OrderBy(x => x.Key).Select(x => x.Value).Aggregate((a, x) => a + x);
            return aggregate;
        }

        private static DataSet ExtractTables(FileInfo file) => ReadFile(file);

        private static List<DataRow> FixupDataSet( DataTable Table)
        {
            int row_no = 0;


            Table.Columns.Add("LineNumber", typeof(int));

            var rows = new List<DataRow>();
            ///insert linenumber
            while (row_no < Table.Rows.Count)
            {
                var dataRow = Table.Rows[row_no];
                dataRow["LineNumber"] = row_no;
                rows.Add(dataRow);
                row_no++;
            }

            return rows;
        }

        private static DataSet ReadFile(FileInfo file)
        {
            FileStream stream = File.Open(file.FullName, FileMode.Open, FileAccess.Read);
            var excelReader = ExcelReaderFactory.CreateReader(stream);
            var result = excelReader.AsDataSet();
            excelReader.Close();
            return result;
        }

        private static bool ProcessUnknownFileType(FileTypes fileType, FileInfo file, List<DataRow> rows)
        {
            if (fileType.ChildFileTypes.FirstOrDefault()?.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Unknown)
            {
                Utils.SendBackTooBigEmail(file, fileType);

                DetectFileType(fileType, file, rows);
                return true;
            }

            return false;
        }

        private static void DetectFileType(FileTypes fileType, FileInfo file, List<DataRow> dataRows)
        {
            try
            {

                FileTypes rfileType = null;
                var potentialsFileTypes = new List<FileTypes>();
                var lastHeaderRow = dataRows[0].ItemArray.ToList();
                int drow_no = 0;
                List<object> headerRow;
                var filetypes = FileTypeManager.FileTypes();
                while (drow_no < dataRows.Take(Utils.maxRowsToFindHeader).ToList().Count)
                {
                    headerRow = dataRows[drow_no].ItemArray.ToList();

                    foreach (var f in filetypes.Where(x => x.IsImportable != false && x.FileTypeMappings.Any()))
                    {
                        if (//headerRow.Any(x => f.FileTypeMappings.All(z => z.Required == false) && f.FileTypeMappings.All(z => z.OriginalName == x.ToString())) || // All False && all in header or all required in header
                            headerRow.Any(x => f.FileTypeMappings.Where(z => z.Required == true).Any(z => z.OriginalName.ToUpper().Trim() == x.ToString().ToUpper().Trim() || z.DestinationName.ToUpper().Trim() == x.ToString().ToUpper().Trim())))
                        {
                            potentialsFileTypes.Add(f);
                            lastHeaderRow = headerRow;
                        }
                    }

                    drow_no++;
                }

                if (!potentialsFileTypes.Any()) return;
                rfileType = potentialsFileTypes
                    .OrderByDescending(x => x.FileTypeMappings.Where(z =>
                        lastHeaderRow.Select(h => h.ToString().ToUpper().Trim())
                            .Contains(z.OriginalName.ToUpper().Trim())).Count())
                    .ThenByDescending(x => x.FileTypeActions.Count())
                    .FirstOrDefault();
                rfileType.AsycudaDocumentSetId = fileType.AsycudaDocumentSetId;
                rfileType.Data = fileType.Data;
                rfileType.EmailId = fileType.EmailId;
                headerRow = lastHeaderRow;
                drow_no = 0;

                ImportUtils.ExecuteDataSpecificFileActions(rfileType, new FileInfo[] { file },
                    BaseDataModel.Instance.CurrentApplicationSettings);
                ImportUtils.ExecuteNonSpecificFileActions(rfileType, new FileInfo[] { file },
                    BaseDataModel.Instance.CurrentApplicationSettings);
                return;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
