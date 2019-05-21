using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using CoreEntities.Business.Services;
using EntryDataQS.Business.Entities;
using ExcelDataReader;
using WaterNut.DataSpace;

namespace AutoBot
{
    partial class Program
    {
        public class FileAction
        {

            public string Filetype { get; set; }

            public List<Action<FileTypes, FileInfo[]>> Actions { get; set; } =
                new List<Action<FileTypes, FileInfo[]>>();
        }

        private static List<FileAction> fileActions => new List<FileAction>
        {
            new FileAction
            {
                Filetype = "XML",
                Actions = new List<Action<FileTypes, FileInfo[]>>
                {
                    (ft, fs) => BaseDataModel.Instance.ImportDocuments(ft.AsycudaDocumentSetId,
                        fs.Select(x => x.FullName).ToList(), true, true, false, false, true).Wait()
                }
            },
            new FileAction
            {
                Filetype = "PO",
                Actions = new List<Action<FileTypes, FileInfo[]>>
                {
                    (ft, fs) => SaveCsv(fs, ft.Type, ft.AsycudaDocumentSetId),
                    (ft, fs) => CreatePOEntries().Wait(),
                     (ft, fs) => ExportPOEntries().Wait(),
                    (x,y) => RunSiKuLi(x.AsycudaDocumentSetId,"AssessIM7"),
                }
            },
            new FileAction
            {
                Filetype = "Sales",
                Actions = new List<Action<FileTypes, FileInfo[]>>
                {

                    (ft, fs) => SaveCsv(fs, ft.Type, ft.AsycudaDocumentSetId),
                    (x,y) => RunSiKuLi(x.AsycudaDocumentSetId,"IM7"),
                }
            },
            new FileAction
            {
                Filetype = "OPS",
                Actions = new List<Action<FileTypes, FileInfo[]>>
                {
                    (ft, fs) => SaveCsv(fs, ft.Type, ft.AsycudaDocumentSetId)
                }
            },
            new FileAction
            {
                Filetype = "ADJ",
                Actions = new List<Action<FileTypes, FileInfo[]>>
                {
                    (ft, fs) => SaveCsv(fs, ft.Type, ft.AsycudaDocumentSetId)
                }
            },
            new FileAction
            {
                Filetype = "ADJ",
                Actions = new List<Action<FileTypes, FileInfo[]>>
                {
                    (ft, fs) => SaveCsv(fs, ft.Type, ft.AsycudaDocumentSetId)
                }
            },
            new FileAction
            {
                Filetype = "ADJ",
                Actions = new List<Action<FileTypes, FileInfo[]>>
                {
                    (ft, fs) => SaveCsv(fs, ft.Type, ft.AsycudaDocumentSetId)
                }
            },
            new FileAction
            {
                Filetype = "XLSX",
                Actions = new List<Action<FileTypes, FileInfo[]>>
                {
                    (ft, fs) => Xlsx2csv(fs, ft.FileTypeMappings)
                }
            },
            new FileAction
            {
                Filetype = "FIX",
                Actions = new List<Action<FileTypes, FileInfo[]>>
                {
                    (ft, fs) => FixCsv(fs, ft.FileTypeMappings)
                }
            },
            new FileAction
            {
                Filetype = "Info",
                Actions = new List<Action<FileTypes, FileInfo[]>>
                {
                     (ft, fs) =>  CreatePOEntries().Wait(),
                     (ft, fs) =>  ExportPOEntries().Wait(),
                    (x,y) => RunSiKuLi(x.AsycudaDocumentSetId,"AssessIM7"),
                }
            },
            new FileAction
            {
                Filetype = "PDF",
                Actions = new List<Action<FileTypes, FileInfo[]>>
                {
                     (ft, fs) =>  CreatePOEntries().Wait(),
                     (ft, fs) =>  ExportPOEntries().Wait(),
                    (x,y) => RunSiKuLi(x.AsycudaDocumentSetId,"AssessIM7"),
                }

            }
        };

        private static async Task CreatePOEntries()
        {
            try
            {
                using (var ctx = new EntryDataQSContext())
                {
                    var res = ctx.ToDo_POToXML.Where(x =>
                            x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .Select(x => new
                        {
                            DocSetId = x.Key,
                            Entrylst = x.Select(z => new {z.EntryDataDetailsId, z.IsClassified}).ToList()
                        })
                        .ToList();
                    foreach (var docSetId in res)
                    {
                        BaseDataModel.Instance.AddToEntry(
                                docSetId.Entrylst.Where(x => x.IsClassified == true).Select(x => x.EntryDataDetailsId),
                                docSetId.DocSetId,
                                BaseDataModel.Instance.CurrentApplicationSettings.InvoicePerEntry ?? true).Wait();
                        BaseDataModel.Instance.AddToEntry(
                                docSetId.Entrylst.Where(x => x.IsClassified == false).Select(x => x.EntryDataDetailsId),
                                docSetId.DocSetId,
                                BaseDataModel.Instance.CurrentApplicationSettings.InvoicePerEntry ?? true)
                            .Wait();
                        
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static async Task ExportPOEntries()
        {
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    var res = ctx.AsycudaDocuments
                        .Where(x =>x.ApplicationSettingsId ==BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                   && x.ImportComplete == false 
                                   && x.DocumentType == "IM7")
                        .GroupBy(x => new {x.AsycudaDocumentSetId, ReferenceNumber = x.AsycudaDocumentSetEx.Declarant_Reference_Number})
                        .Select(x => new
                        {
                            DocSet =  x.Key,
                            Entrylst = x.Select(z => new { z.ASYCUDA_Id}).ToList()
                        })
                        .ToList();
                    foreach (var docSetId in res)
                    {
                        BaseDataModel.Instance.ExportDocSet(docSetId.DocSet.AsycudaDocumentSetId.Value,
                            Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                                docSetId.DocSet.ReferenceNumber), false).Wait();
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        static void Main(string[] args)
        {

            using (var ctx = new CoreEntitiesContext(){StartTracking = true})
            {
                foreach (var appSetting in ctx.ApplicationSettings.Include(x => x.FileTypes)
                    
                    .Include("FileTypes.FileTypeMappings").ToList())
                {
                    // set BaseDataModel CurrentAppSettings
                    BaseDataModel.Instance.CurrentApplicationSettings = appSetting;
                    //check emails
                    var msgLst = EmailDownloader.EmailDownloader.CheckEmails(new EmailDownloader.Client
                    {
                        DataFolder = appSetting.DataFolder,
                        Password = appSetting.EmailPassword,
                        Email = appSetting.Email
                    });
                    // get downloads
                    foreach (var msg in msgLst)
                    {
                        var desFolder = Path.Combine(appSetting.DataFolder, msg.Key);
                        foreach (var fileType in appSetting.FileTypes)
                        {
                            var csvFiles = new DirectoryInfo(desFolder).GetFiles().Where(x => Regex.IsMatch(x.FullName, fileType.FilePattern)).ToArray();

                            if (csvFiles.Length == 0) continue;

                            var oldDocSet =
                                ctx.AsycudaDocumentSetExs
                                    .Include(x => x.AsycudaDocumentSet_Attachments)
                                    .Include("AsycudaDocumentSet_Attachments.Attachments")
                                    .FirstOrDefault(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId);
                            if (fileType.CreateDocumentSet)
                            {
                                var docSet =
                                    ctx.AsycudaDocumentSetExs
                                        .Include(x => x.AsycudaDocumentSet_Attachments)
                                        .Include("AsycudaDocumentSet_Attachments.Attachments")
                                        .FirstOrDefault(x => x.Declarant_Reference_Number == msg.Key);
                                if (docSet == null)
                                {
                                    ctx.Database.ExecuteSqlCommand($@"INSERT INTO AsycudaDocumentSet
                                        (ApplicationSettingsId, Declarant_Reference_Number, Document_TypeId, Customs_ProcedureId, Exchange_Rate)
                                    VALUES({oldDocSet.ApplicationSettingsId},'{msg.Key}',{oldDocSet.Document_TypeId},{oldDocSet.Customs_ProcedureId},0)");
                                    
                                }
                                oldDocSet =
                                    ctx.AsycudaDocumentSetExs.FirstOrDefault(x =>
                                        x.Declarant_Reference_Number == msg.Key);
                            }

                            fileType.AsycudaDocumentSetId = oldDocSet.AsycudaDocumentSetId;

                            if (fileType.Type == "Info")
                                {
                                    var dbStatement = "";
                                    foreach (var file in csvFiles)
                                    {
                                        var fileTxt = File.ReadAllLines(file.FullName);
                                        var res = ctx.InfoMapping.Where(x => x.EntityType == "AsycudaDocumentSet")
                                            .ToList();
                                        foreach (var line in fileTxt)
                                        {
                                            var match = Regex.Match(line, @"((?<Key>.[a-zA-Z\s]*):(?<Value>.[a-zA-Z0-9\- :$.]*))");
                                            if(match.Success)
                                            foreach (var infoMapping in res.Where(x => x.Key == match.Groups["Key"].Value.Trim()))
                                            {
                                                dbStatement += $@" Update AsycudaDocumentSet Set {infoMapping.Field} = '{ReplaceSpecialChar(match.Groups["Value"].Value.Trim(),"")}' Where AsycudaDocumentSetId = '{oldDocSet.AsycudaDocumentSetId}';";
                                            }
                                        }

                                    }
                                    if(!string.IsNullOrEmpty(dbStatement)) ctx.Database.ExecuteSqlCommand(dbStatement);
                                }
                                else
                                {
                                    foreach (var file in csvFiles)
                                    {
                                        if (!msg.Value.Contains(file.Name)) continue;
                                        var attachment =
                                            oldDocSet.AsycudaDocumentSet_Attachments.FirstOrDefault(x => x.Attachments.FilePath == file.FullName);
                                        if (attachment == null)
                                        {
                                            oldDocSet.AsycudaDocumentSet_Attachments.Add(
                                                new AsycudaDocumentSet_Attachments(true)
                                                {
                                                    Attachments = new Attachments(true) {FilePath = file.FullName, DocumentCode = fileType.DocumentCode},
                                                    DocumentSpecific = fileType.DocumentSpecific
                                                });
                                        }
                                    }

                                    ctx.SaveChanges();
                                }

                            
                            
                            
                                fileActions.Where(x => x.Filetype == fileType.Type).SelectMany(x => x.Actions).ToList()
                                    .ForEach(x => x.Invoke(fileType, csvFiles.Where(z => msg.Value.Contains(z.Name)).ToArray()));

                        }


                    }



                }
            }
        }

        private static string ReplaceSpecialChar(string msgSubject, string rstring)
        {
            return Regex.Replace(msgSubject, @"[^0-9a-zA-Z.\s]+", rstring);
        }

        private static void GetAsycudaEntries()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "java.exe";
            startInfo.Arguments =
                $@"-jar C:\jython2.7.0\bin\sikulix.jar -r C:\Users\josep\OneDrive\Clients\AutoBot\Scripts\IM7.sikuli --args {
                        BaseDataModel.Instance.CurrentApplicationSettings.AsycudaLogin
                    } {BaseDataModel.Instance.CurrentApplicationSettings.AsycudaPassword} ""{
                        BaseDataModel.Instance.CurrentApplicationSettings.DataFolder
                    }";
            startInfo.UseShellExecute = false;
            process.StartInfo = startInfo;
            process.Start();
            var timeoutCycles = 0;
            while (!process.HasExited && process.Responding)
            {
                if (timeoutCycles > 2) break;
                Thread.Sleep(1000 * 60);
                timeoutCycles += 1;
            }

            if (!process.HasExited) process.Kill();

            foreach (var process1 in Process.GetProcesses().Where(x => x.MainWindowTitle.Contains("ASYCUDA")).ToList())
            {
                process1.Kill();
            }
        }

        private static void RunSiKuLi(int docSetId, string scriptName)
        {
            var docRef = new AsycudaDocumentSetExService().GetAsycudaDocumentSetExByKey(docSetId.ToString()).Result.Declarant_Reference_Number;

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "java.exe";
            startInfo.Arguments =
                $@"-jar C:\jython2.7.0\bin\sikulix.jar -r C:\Users\josep\OneDrive\Clients\AutoBot\Scripts\{scriptName}.sikuli --args {
                        BaseDataModel.Instance.CurrentApplicationSettings.AsycudaLogin
                    } {BaseDataModel.Instance.CurrentApplicationSettings.AsycudaPassword} ""{
                        Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, docRef) + "\\"
                    }";
            startInfo.UseShellExecute = false;
            process.StartInfo = startInfo;
            process.Start();
            var timeoutCycles = 0;
            while (!process.HasExited && process.Responding)
            {
                if (timeoutCycles > 60) break;
                Thread.Sleep(1000 * 60);
                timeoutCycles += 1;
            }

            if (!process.HasExited) process.Kill();

            foreach (var process1 in Process.GetProcesses().Where(x => x.MainWindowTitle.Contains("ASYCUDA")).ToList())
            {
                process1.Kill();
            }
        }

        private static void SaveCsv(FileInfo[] csvFiles, string fileType, int asycudaDocumentSetId)
        {

            foreach (var file in csvFiles)
            {
                SaveCSVModel.Instance.ProcessDroppedFile(file.FullName, fileType, asycudaDocumentSetId, true)
                    .Wait();

                var dt = CSV2DataTable(file);
                var fileRes = dt.AsEnumerable()
                    .Select(x => new
                    {
                        Invoice = x["Invoice #"].ToString(),
                        Total = x.Field<Int32>("Quantity") * x.Field<double>("Cost")
                    })
                    .GroupBy(x => x.Invoice)
                    .Select(x => new
                    {
                        Invoice = x.Key,
                        Total = Math.Round(x.Sum(z => z.Total),2)
                    }).ToList();

                using (var ctx = new EntryDataQSContext())
                {

                    var dbres = ctx.EntryDataDetailsExes
                        .Select(x => new
                        {
                            Invoice = x.EntryDataId,
                            Total = x.Quantity * x.Cost
                        })
                        .GroupBy(x => x.Invoice)
                        .Select(x => new
                        {
                            Invoice = x.Key,
                            Total = Math.Round(x.Sum(z => z.Total),2)
                        }).ToList();
                    var res = fileRes.GroupJoin(dbres, x => x.Invoice, y => y.Invoice,
                            (x, y) => new {file = x, db = y.SingleOrDefault()})
                        .Where(x => x.file.Total != x.db.Total)
                        .Select(x => new {x.file.Invoice, FileTotal = x.file.Total, dbTotal = x.db.Total})
                        .ToList();

                    if (res.Any())
                    {
                        //TODO: Log Message
                        return;
                    }
                    

                }


            }
        }
    

    private static void Xlsx2csv(FileInfo[] files, List<FileTypeMappings> mappings)
        {
            foreach (var file in files)
            {
                // Reading from a binary Excel file (format; *.xlsx)
                    FileStream stream = File.Open(file.FullName, FileMode.Open, FileAccess.Read);
                    var excelReader = ExcelReaderFactory.CreateReader(stream);
                    var result = excelReader.AsDataSet();
                    excelReader.Close();

                string a = "";
                int row_no = 0;

                while (row_no < result.Tables[0].Rows.Count)
                {
                    if(mappings.Any() && mappings.Select(x => x.OriginalName).All(x => result.Tables[0].Rows[row_no].ItemArray.Contains(x)))
                        a = "";
                    for (int i = 0; i < result.Tables[0].Columns.Count; i++)
                    {

                        a += StringToCSVCell(result.Tables[0].Rows[row_no][i].ToString()) + ",";
                    }
                    row_no++;
                    a += "\n";

                    
                }
                string output = Path.ChangeExtension(file.FullName, ".csv");
                StreamWriter csv = new StreamWriter(output, false);
                csv.Write(a);
                csv.Close();

                
            }
        }

        private static void FixCsv(FileInfo[] files, List<FileTypeMappings> mappings)
        {
            foreach (var file in files)
            {
                if(File.Exists($@"{file.DirectoryName}\{file.Name.Replace(".csv", "")}-Fixed{file.Extension}")) continue;
                // Reading from a binary Excel file (format; *.xlsx)
                var dt = CSV2DataTable(file);

                
                string table = "";
                int row_no = 0;

                while (row_no < dt.Rows.Count)
                {
                    var row = new Dictionary<string,string>();
                    foreach (var mapping in mappings)
                    {
                        var maps = mapping.OriginalName.Split('+');
                        var val = "";
                        foreach (var map in maps)
                        {
                            if (!dt.Columns.Contains(map))
                            {
                                //TODO: log error
                                return;
                            }

                            if (row_no == 0)
                            {
                                val += mapping.DestinationName;
                                if (maps.Length > 1) break;
                            }
                            else
                            {
                                //if (string.IsNullOrEmpty(dt.Rows[row_no][map].ToString())) continue;

                                val += dt.Rows[row_no][map];
                                if (maps.Length > 1 && map != maps.Last()) val += " - ";
                            }


                        }
                        if(row_no > 0)
                            if (string.IsNullOrEmpty(val) && mapping.Required == true)
                                break;
                            else if (mapping.DataType == "Number")
                            {
                                if (val.ToCharArray().All(x => !char.IsDigit(x)))
                                {
                                //Log Error
                                    break;
                                //val = "";
                                }
                            }
                            else if (mapping.DataType == "Date")
                            {
                                DateTime tmp;
                                if (DateTime.TryParse(val, out tmp) == false)
                                {
                                //Log Error
                                    break;
                                //  val = "";
                            }
                            }

                        row.Add(mapping.DestinationName, StringToCSVCell(val));
                        
                    }
                    row_no++;
                   
                    if(row.Count == mappings.Count) table += row.Select(x => x.Value).Aggregate((a, x) => a + "," + x) + "\n";
                }
                string output = $@"{file.DirectoryName}\{file.Name.Replace(".csv","")}-Fixed{file.Extension}";
                StreamWriter csv = new StreamWriter(output, false);
                csv.Write(table);
                csv.Close();

                
            }
        }

        private static DataTable CSV2DataTable(FileInfo file)
        {
            OleDbConnection conn = new OleDbConnection(string.Format(
                @"Provider=Microsoft.Jet.OleDb.4.0; Data Source={0};" +
                "Extended Properties=\"Text;HDR=YES;FMT=Delimited\"",
                file.DirectoryName
            ));
            conn.Open();

            string sql = string.Format("select * from [{0}]", Path.GetFileName(file.Name));
            OleDbCommand cmd = new OleDbCommand(sql, conn);
            OleDbDataReader reader = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            return dt;
        }

        /// <summary>
        /// Turn a string into a CSV cell output
        /// </summary>
        /// <param name="str">String to output</param>
        /// <returns>The CSV cell formatted string</returns>
        public static string StringToCSVCell(string str)
        {
            bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n") );
            if (mustQuote)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\"");
                foreach (char nextChar in str)
                {
                    sb.Append(nextChar);
                    if (nextChar == '"')
                        sb.Append("\"");
                }
                sb.Append("\"");
                return sb.ToString();
            }

            return str;
        }
    }
}
