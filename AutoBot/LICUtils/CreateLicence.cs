using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext, TODO_LicenseToXML, AsycudaDocumentSet, Consignees, Customs_Procedure, Contacts, ExportTemplates are here
using EntryDataDS.Business.Entities; // Assuming EntryDataDSContext, Suppliers, EntryData are here
using LicenseDS.Business.Entities; // Assuming LicenseDSContext, TODO_LICToCreate, xLIC_License are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using WaterNut.DataSpace.Asycuda; // Assuming LicenseToDataBase is here

namespace AutoBot
{
    public partial class LICUtils
    {
        public static void CreateLicence(FileTypes ft)
        {
            try
            {
                Console.WriteLine("Create License Files");

                using (var ctx = new LicenseDSContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var pOs = ctx.TODO_LICToCreate
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)//ft.AsycudaDocumentSetId == 0 ||
                        .ToList();
                    foreach (var pO in pOs)
                    {
                        //if (pO.Item1.Declarant_Reference_Number != "30-15936") continue;
                        var directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                            pO.Declarant_Reference_Number);
                        if (!Directory.Exists(directoryName)) continue;

                        var instructions = Path.Combine(directoryName, "LIC-Instructions.txt");
                            if (File.Exists(instructions)) File.Delete(instructions);

                        // --- Fetch Consignee Info --- Moved inside the outer loop
                        string consigneeCode = null;
                        string consigneeName = null;
                            string consigneeAddress = null;
                            int? docSetAppId = pO.ApplicationSettingsId;
                            int? asycudaDocumentSetId = pO.AsycudaDocumentSetId;

                            if (asycudaDocumentSetId.HasValue && docSetAppId.HasValue)
                            {
                                using (var coreCtx = new CoreEntitiesContext())
                                {
                                    var docSetData = coreCtx.AsycudaDocumentSet
                                                        .Where(ds => ds.AsycudaDocumentSetId == asycudaDocumentSetId.Value)
                                                        .Select(ds => new { ds.ConsigneeName, ds.Customs_ProcedureId })
                                                        .FirstOrDefault();

                                    if (docSetData != null && !string.IsNullOrEmpty(docSetData.ConsigneeName))
                                    {
                                        consigneeName = docSetData.ConsigneeName;
                                        int? docSetCustomsProcId = docSetData.Customs_ProcedureId;

                                        // Fetch primary address
                                        var consignee = coreCtx.Consignees
                                                               .FirstOrDefault(c => c.ConsigneeName == consigneeName && c.ApplicationSettingsId == docSetAppId.Value);

                                        consigneeCode = consignee?.ConsigneeCode;
                                        consigneeAddress = consignee?.Address;

                                        // Fallback Logic
                                        if (string.IsNullOrWhiteSpace(consigneeAddress) && docSetCustomsProcId.HasValue)
                                        {
                                            var customsProcedureStr = coreCtx.Customs_Procedure
                                                                           .Where(cp => cp.Customs_ProcedureId == docSetCustomsProcId.Value)
                                                                           .Select(cp => cp.CustomsProcedure)
                                                                           .FirstOrDefault();

                                            if (!string.IsNullOrEmpty(customsProcedureStr))
                                            {
                                                var exportTemplate = BaseDataModel.Instance.ExportTemplates // Assuming ExportTemplates exists
                                                                            .FirstOrDefault(et => et.ApplicationSettingsId == docSetAppId.Value
                                                                                                 && et.Customs_Procedure == customsProcedureStr);

                                                consigneeAddress = exportTemplate?.Consignee_Address;
                                            }
                                        }
                                    }
                                }
                            }
                            // --- End Fetch Consignee Info ---

                            var llst = new CoreEntitiesContext().Database
                            .SqlQuery<TODO_LicenseToXML>(
                                $"select * from [TODO-LicenseToXML]  where asycudadocumentsetid = {pO.AsycudaDocumentSetId} and LicenseDescription is not null").ToList();

                        var lst = llst
                            .Where(x => !string.IsNullOrEmpty(x.LicenseDescription))
                            .GroupBy(x => x.EntryDataId)
                            .SelectMany(x => x.OrderByDescending(z => z.SourceFile))//.FirstOrDefault()
                            .GroupBy(x => new { x.EntryDataId, x.TariffCategoryCode, x.SourceFile })
                            .ToList();

                        foreach (var itm in lst)
                        {
                            var fileName = Path.Combine(directoryName, $"{itm.Key.EntryDataId}-{itm.Key.TariffCategoryCode}-LIC.xml");

                            if (File.Exists(fileName))
                            {
                                var instrFile = Path.Combine(
                                    BaseDataModel.GetDocSetDirectoryName(pO.Declarant_Reference_Number), "LIC-Instructions.txt"); // Assuming GetDocSetDirectoryName exists
                                if (File.Exists(instrFile))
                                {
                                    var resultsFile = Path.Combine(
                                        BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                                        pO.Declarant_Reference_Number, "LIC-InstructionResults.txt");
                                    var lcont = 0;
                                    // This calls AssessLICComplete, which needs to be in its own partial class
                                    if (AssessLICComplete(instrFile, resultsFile, out lcont) == true)
                                        continue;
                                }
                            }

                            var contact = new CoreEntitiesContext().Contacts.FirstOrDefault(x =>
                                x.Role == "Broker" && x.ApplicationSettingsId == BaseDataModel.Instance
                                    .CurrentApplicationSettings.ApplicationSettingsId);
                            Suppliers supplier = null; // Initialize to null

                            using (var ectx = new EntryDataDSContext())
                            {
                                //var elst = lst.Select(s => s.EntryDataId).ToList();
                                supplier = ectx.Suppliers.FirstOrDefault(x =>
                                    ectx.EntryData.Where(z => z.EntryDataId == itm.Key.EntryDataId).Select(z => z.SupplierCode)
                                        .Any(z => z == x.SupplierCode) &&
                                    x.ApplicationSettingsId == pO.ApplicationSettingsId);
                            }
                            // Pass consigneeName and consigneeAddress to CreateLicense
                            // IMPORTANT: LicenseToDataBase.CreateLicense signature must be manually updated elsewhere
                            var lic = LicenseToDataBase.Instance.CreateLicense(new List<TODO_LicenseToXML>(itm), contact, supplier, // Assuming CreateLicense exists
                                itm.Key.EntryDataId, consigneeCode ,consigneeName, consigneeAddress ?? ""); // Pass fetched values
                            var invoices = itm.Select(x => new Tuple<string, string>(x.EntryDataId, Path.Combine(new FileInfo(x.SourceFile).DirectoryName, $"{x.EntryDataId}.pdf"))).Where(x => File.Exists(x.Item2)).Distinct().ToList();
                            if (!invoices.Any()) continue;
                            ctx.xLIC_License.Add(lic);
                            ctx.SaveChanges();
                            LicenseToDataBase.Instance.ExportLicense(pO.AsycudaDocumentSetId, lic, fileName, // Assuming ExportLicense exists
                                invoices);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}