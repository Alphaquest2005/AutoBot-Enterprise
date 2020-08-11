using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.UI;
using CoreEntities.Business.Entities;
using CoreEntities.Business.Enums;
using DocumentDS.Business.Entities;
using WaterNut.Business.Entities;
using WaterNut.Interfaces;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;
using Customs_Procedure = DocumentDS.Business.Entities.Customs_Procedure;
using Document_Type = DocumentDS.Business.Entities.Document_Type;

namespace WaterNut.DataSpace
{
    public class CreateOPSClass
    {
        
        private static readonly CreateOPSClass _instance;
        static CreateOPSClass()
        {
            _instance = new CreateOPSClass();
        }

        public static CreateOPSClass Instance
        {
            get { return _instance; }
        }

        public void OPSIntializeCdoc(DocumentCT cdoc,  AsycudaDocumentSet ads)
        {
            BaseDataModel.Instance.IntCdoc(cdoc, ads);
            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = false;
            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Description = "Existing Opening Stock Entries";
            cdoc.Document.xcuda_Declarant.Number = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Declarant_Reference_Number + "-OPS" + "-F" + cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.FileNumber.ToString();
        }

        public  async Task CreateOPS(string filterExpression, AsycudaDocumentSet docSet)
        {

            if (docSet == null)
            {
               throw new ApplicationException("Please Select a Asycuda Document Set before proceding");
                
            }

            StatusModel.Timer("Getting Data...");

            var slstSource = await AllocationsModel.Instance.GetAsycudaSalesAllocations(filterExpression).ConfigureAwait(false);
            


            var slst = from s in slstSource//.Where(p => p.PreviousDocumentItem != null && p.PreviousDocumentItem.AsycudaDocument.IsManuallyAssessed == true)
                .Select(x => x.EntryDataDetails)
                group s by new
                {
                    s.EntryDataDetailsId
                }
                into g
                select new
                {
                   // Allocations = g.ToList(),
                    EntlnData = new AllocationsModel.AlloEntryLineData
                    {
                        ItemNumber = g.First().ItemNumber,
                        ItemDescription = g.First().ItemDescription,
                        InventoryItem =(IInventoryItem) g.First().EntryDataDetailsEx.InventoryItemsEx,
                        TariffCode = g.First().EntryDataDetailsEx.InventoryItemsEx.TariffCode,
                        Cost = g.First().Cost,
                        Quantity = g.Sum(x =>x.Quantity - x.QtyAllocated),//First().Quantity
                        EntryDataDetails = new List<EntryDataDetailSummary>() { new EntryDataDetailSummary()
                        {
                            EntryDataDetailsId = g.First().EntryDataDetailsId,
                            EntryDataId = g.First().EntryDataId,
                            EntryData_Id = g.First().EntryData_Id,
                            QtyAllocated = g.First().QtyAllocated
                        }}
                    }

                };

            var res = slst.Select(x => x.EntlnData).GroupBy(x => new {x.ItemNumber, x.Cost})
                .Select(g => new AllocationsModel.AlloEntryLineData
                {
                    ItemNumber = g.Key.ItemNumber,
                    ItemDescription = g.First().ItemDescription,
                    InventoryItem = g.First().InventoryItem,
                    TariffCode = g.First().InventoryItem.TariffCode,
                    Cost = g.Key.Cost,
                    Quantity = g.Sum(z => z.Quantity),
                    EntryDataDetails = g.SelectMany(z => z.EntryDataDetails).ToList()
                });


            if (!res.Any())
            {
                throw new ApplicationException(
                    "No OPS Allocations found! If you just deleted Entries, Please Allocate Sales then continue Else Contact your Network Administrator");
                StatusModel.StopStatusUpdate();
                return;
            }

            var cdoc = new DocumentCT();
            cdoc.Document = BaseDataModel.Instance.CreateNewAsycudaDocument(docSet);



            var itmcount = cdoc.DocumentItems.Count();

            
            var cp =
                BaseDataModel.Instance.Customs_Procedures
                    .Single(x => x.CustomsOperationId == (int) CustomsOperations.Warehouse && x.Stock == true);

            docSet.Customs_Procedure = cp;
           

            OPSIntializeCdoc(cdoc, docSet);


            StatusModel.StartStatusUpdate("Creating Opening Stock Entries", slst.Count());


            foreach (var pod in res.ToList())
            {

                StatusModel.StatusUpdate();

                var itm = BaseDataModel.Instance.CreateItemFromEntryDataDetail(pod, cdoc);

               

                itmcount += 1;
                if (itmcount % BaseDataModel.Instance.CurrentApplicationSettings.MaxEntryLines == 0)
                {
                    await BaseDataModel.Instance.SaveDocumentCT(cdoc).ConfigureAwait(false);
                    //dup new file
                    cdoc = new DocumentCT();
                    cdoc.Document = BaseDataModel.Instance.CreateNewAsycudaDocument(docSet);

                    OPSIntializeCdoc(cdoc, docSet);
                }

            }
            if (cdoc.DocumentItems.Count == 0) cdoc = null;

            StatusModel.Timer("Saving to Database...");

            await BaseDataModel.Instance.SaveDocumentCT(cdoc).ConfigureAwait(false);

            StatusModel.StopStatusUpdate();



        }
    }
}