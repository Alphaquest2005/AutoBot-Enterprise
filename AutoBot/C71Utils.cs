using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using EntryDataDS.Business.Entities;
using ValuationDS.Business.Entities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using WaterNut.DataSpace.Asycuda;
using FileTypes = CoreEntities.Business.Entities.FileTypes;
using TODO_C71ToCreate = ValuationDS.Business.Entities.TODO_C71ToCreate;

namespace AutoBot
{
    public class C71Utils
    {
        public static void ImportC71(FileTypes ft)
        {
            
                
            Console.WriteLine("Import C71");
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 10;
                var docSets = ctx.TODO_C71ToCreate
                    //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)//ft.AsycudaDocumentSetId == 0 || 
                    .ToList();
                foreach (var poInfo in docSets)
                {
                    ImportC71(poInfo.Declarant_Reference_Number, poInfo.AsycudaDocumentSetId);

                }
            }
        }

        public static bool ImportC71(string declarant_Reference_Number, int asycudaDocumentSetId)
        {
            try
            {

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    //var reference = declarant_Reference_Number;
                    //var directory = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                    //    reference);
                    //if (!Directory.Exists(directory)) return false;

                    var lastdbfile =
                        ctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachments).OrderByDescending(x => x.AttachmentId).FirstOrDefault(x => x.AsycudaDocumentSetId == asycudaDocumentSetId && x.FileTypes.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.C71);
                    var lastfiledate = lastdbfile != null ? File.GetCreationTime(lastdbfile.Attachments.FilePath) : DateTime.Today.AddDays(-1);


                    var ft = ctx.FileTypes.FirstOrDefault(x =>
                        x.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.C71 && x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                    if (ft == null) return true;
                    var desFolder = Path.Combine(BaseDataModel.GetDocSetDirectoryName("Imports"), FileTypeManager.EntryTypes.C71);

                    if (!Directory.Exists(desFolder)) Directory.CreateDirectory(desFolder);

                    var csvFiles = new DirectoryInfo(desFolder).GetFiles()
                        .Where(x => x.LastWriteTime >= lastfiledate)
                        .Where(x => Regex.IsMatch(x.FullName, ft.FilePattern, RegexOptions.IgnoreCase) && x.Name != "C71.xml")
                        .ToList();
                    //if (lastC71 == null) return false;
                    //var csvFiles = new FileInfo[] {lastC71};

                    if (csvFiles.Any())
                    {
                        BaseDataModel.Instance.ImportC71(asycudaDocumentSetId,
                            csvFiles.Select(x => x.FullName).ToList());
                        ft.AsycudaDocumentSetId = asycudaDocumentSetId;
                        //BaseDataModel.Instance.SaveAttachedDocuments(csvFiles, ft).Wait();
                    }

                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        internal static void CreateC71(FileTypes ft)
        {
            try

            {
                Console.WriteLine("Create C71 Files");

                using (var ctx = new ValuationDSContext())
                {





                    ctx.Database.CommandTimeout = 10;
                    var pOs = ctx.TODO_C71ToCreate
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)//ft.AsycudaDocumentSetId == 0 ||
                        .ToList();
                    foreach (var pO in pOs)
                    {
                        //if (pO.Item1.Declarant_Reference_Number != "30-15936") continue;
                        var directoryName = BaseDataModel.GetDocSetDirectoryName(pO.Declarant_Reference_Number);
                        var fileName = Path.Combine(directoryName, "C71.xml");
                        if (File.Exists(fileName) && (pO.TotalCIF.HasValue && Math.Abs(pO.TotalCIF.GetValueOrDefault() - pO.C71Total) <= 0.01)) continue;
                        var c71results = Path.Combine(directoryName, "C71-InstructionResults.txt");
                        if (File.Exists(c71results))
                        {
                            var c71instructions = Path.Combine(directoryName, "C71-Instructions.txt");
                            //if (AssessC71Complete(c71instructions, c71results, out int lcont) == true) continue;


                            File.Delete(c71results);
                            if (File.Exists(Path.Combine(directoryName, "C71OverView-PDF.txt"))) File.Delete(Path.Combine(directoryName, "C71OverView-PDF.txt"));
                        }
                        var lst = new CoreEntitiesContext().TODO_C71ToXML.Where(x =>
                                x.ApplicationSettingsId == pO.ApplicationSettingsId &&
                                x.AsycudaDocumentSetId == pO.AsycudaDocumentSetId)
                            .GroupBy(x => x.AsycudaDocumentSetId)
                            .Where(x => x.Sum(z => z.InvoiceTotal * z.CurrencyRate) > 1).ToList();
                        if (!lst.Any()) continue;
                        var supplierCode = lst.SelectMany(x => x.Select(z => z.SupplierCode)).FirstOrDefault(x => !string.IsNullOrEmpty(x));
                        Suppliers supplier = new Suppliers();
                        if (supplierCode == null)
                        {

                        }
                        else
                        {
                            supplier = new EntryDataDSContext().Suppliers.FirstOrDefault(x =>
                                x.SupplierCode == supplierCode &&
                                x.ApplicationSettingsId == pO.ApplicationSettingsId);
                        }

                        var c71 = C71ToDataBase.Instance.CreateC71(supplier, lst.SelectMany(x => x.Select(z => z)).ToList(), pO.Declarant_Reference_Number);
                        ctx.xC71_Value_declaration_form.Add(c71);
                        ctx.SaveChanges();
                        C71ToDataBase.Instance.ExportC71(pO.AsycudaDocumentSetId, c71, fileName);


                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

      


        public static void DownLoadC71(FileTypes ft)
        {
            try

            {
                Console.WriteLine("Attempting Download C71 Files");
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;

                    var lst = ctx.TODO_C71ToCreate
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId) 
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)//ft.AsycudaDocumentSetId == 0 ||
                        .OrderByDescending(x => x.AsycudaDocumentSetId)
                        .Take(1)
                        .ToList();


                    if (!lst.Any()) return;
                    var directoryName = StringExtensions.UpdateToCurrentUser(Path.Combine(BaseDataModel.GetDocSetDirectoryName("Imports"), "C71"));

                    Console.WriteLine("Download C71 Files");
                    var notries = 2;
                    var tries = 0;
                    var lcont = 0;
                    while (ImportC71Complete(directoryName, out lcont) == false)
                    {
                        Utils.RunSiKuLi(directoryName, "C71", lcont.ToString());
                        tries += 1;
                        if (tries >= notries) break;
                    }


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public static bool ImportC71Complete(string directoryName, out int lcont)
        {
            lcont = 0;

            var desFolder = directoryName + "\\";
            var overviewFile = Path.Combine(desFolder, "C71OverView-PDF.txt");
            if (File.Exists(overviewFile))
            {


                if (File.GetLastWriteTime(overviewFile) <= DateTime.Now.AddMinutes(-10)) return false;
                var lines = File.ReadAllText(Path.Combine(directoryName, "C71OverView-PDF.txt"))
                    .Split(new[] { $"\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                {

                    return false;
                }

                if (lines.FirstOrDefault() == "No Data" && File.GetLastWriteTime(overviewFile) <= DateTime.Now.AddHours(-0.25)) return false;
                if (lines.FirstOrDefault() == "No Data") return true;



                var existingfiles = 0;

                foreach (var line in lines)
                {
                    lcont += 1;

                    var p = line.Split('\t');
                    if (p.Length < 3) continue;
                    if (File.Exists(Path.Combine(desFolder, $"{p[1]}-C71.xml")))
                    {
                        existingfiles += 1;
                        continue;
                    }
                    return false;
                }

                return existingfiles != 0;
            }
            else
            {

                return false;
            }
        }


        public static bool AssessC71Complete(string instrFile, string resultsFile, out int lcont)
        {
            lcont = 0;


            if (File.Exists(instrFile))
            {
                if (!File.Exists(resultsFile)) return false;
                var lines = File.ReadAllLines(instrFile);
                var res = File.ReadAllLines(resultsFile);
                if (res.Length == 0)
                {

                    return false;
                }


                foreach (var line in lines)
                {
                    var p = line.Split('\t');
                    if (lcont >= res.Length) return false;
                    if (string.IsNullOrEmpty(res[lcont])) return false;
                    var r = res[lcont].Split('\t');
                    lcont += 1;
                    if (p[1] == r[1] && r.Length == 5 && r[4] == "Success")
                    {
                        continue;
                    }
                    return false;
                }

                return true;
            }
            else
            {

                return true;
            }
        }

        public static void ReImportC71()
        {
            Console.WriteLine("Export Latest PO Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 10;
                var docset =
                    ctx.TODO_PODocSet.Where(x =>
                            x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .OrderByDescending(x => x.AsycudaDocumentSetId)
                        .FirstOrDefault();
                if (docset != null)
                {
                    C71Utils.ImportC71(docset.Declarant_Reference_Number, docset.AsycudaDocumentSetId);
                }
            }
        }


        public static void AssessC71(FileTypes ft)
        {


            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 10;
                var res = ctx.TODO_C71ToCreate
                    .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)//ft.AsycudaDocumentSetId == 0 ||
                    .OrderByDescending(x => x.AsycudaDocumentSetId)
                    .Take(1)
                    .ToList();

                foreach (var doc in res)
                {

                    var directoryName = BaseDataModel.GetDocSetDirectoryName(doc.Declarant_Reference_Number);
                    var instrFile = Path.Combine(directoryName, "C71-Instructions.txt");
                    if (!File.Exists(instrFile)) continue;
                    var resultsFile = Path.Combine(directoryName, "C71-InstructionResults.txt");
                    var lcont = 0;
                    while (C71Utils.AssessC71Complete(instrFile, resultsFile, out lcont) == false)
                    {
                        Utils.RunSiKuLi(directoryName, "AssessC71", lcont.ToString());
                    }
                }
            }
        }
    }
}