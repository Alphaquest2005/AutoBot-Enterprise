using System;
using System.IO;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming FileTypes, CoreEntitiesContext, TODO_C71ToXML are here
using EntryDataDS.Business.Entities; // Assuming EntryDataDSContext, Suppliers are here
using ValuationDS.Business.Entities; // Assuming ValuationDSContext, TODO_C71ToCreate, xC71_Value_declaration_form are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using WaterNut.DataSpace.Asycuda; // Assuming C71ToDataBase is here

namespace AutoBot
{
    public partial class C71Utils
    {
        internal static void CreateC71(FileTypes ft)
        {
            try
            {
                Console.WriteLine("Create C71 Files");

                using (var ctx = new ValuationDSContext())
                {
                    // This calls GetConsigneeData, which needs to be in its own partial class
                    var consigneeCode = GetConsigneeData( out var consigneeName, out var consigneeAddress, ft.AsycudaDocumentSetId);

                    ctx.Database.CommandTimeout = 10;
                    var pOs = ctx.TODO_C71ToCreate
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)//ft.AsycudaDocumentSetId == 0 ||
                        .ToList();
                    foreach (var pO in pOs)
                    {
                        //if (pO.Item1.Declarant_Reference_Number != "30-15936") continue;
                        var directoryName = BaseDataModel.GetDocSetDirectoryName(pO.Declarant_Reference_Number); // Assuming GetDocSetDirectoryName exists
                        var fileName = Path.Combine(directoryName, "C71.xml");
                        if (File.Exists(fileName) && (pO.TotalCIF.HasValue && Math.Abs(pO.TotalCIF.GetValueOrDefault() - pO.C71Total) <= 0.01)) continue;
                        var c71results = Path.Combine(directoryName, "C71-InstructionResults.txt");
                        if (File.Exists(c71results))
                        {
                            var c71instructions = Path.Combine(directoryName, "C71-Instructions.txt");
                            //if (AssessC71Complete(c71instructions, c71results, out int lcont) == true) continue; // Calls AssessC71Complete

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
                        if (supplierCode != null)
                        {
                             supplier = new EntryDataDSContext().Suppliers.FirstOrDefault(x =>
                                x.SupplierCode == supplierCode &&
                                x.ApplicationSettingsId == pO.ApplicationSettingsId);
                        }

                        // Pass consigneeName and consigneeAddress to CreateC71
                        var c71 = C71ToDataBase.Instance.CreateC71(supplier, lst.SelectMany(x => x.Select(z => z)).ToList(), pO.Declarant_Reference_Number, consigneeCode, consigneeName, consigneeAddress ?? ""); // Pass fetched values, ensure address is not null
                        ctx.xC71_Value_declaration_form.Add(c71);
                        ctx.SaveChanges();
                        C71ToDataBase.Instance.ExportC71(pO.AsycudaDocumentSetId, c71, fileName); // Assuming ExportC71 exists
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