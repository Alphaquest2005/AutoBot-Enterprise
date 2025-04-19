//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Core.Common.UI;
//using DocumentDS.Business.Entities;
//using SimpleMvvmToolkit;
//using WaterNut.Business.Entities;
//using WaterNut.Interfaces;

//namespace WaterNut.DataSpace
//{
//    public class CreateIncompOPSClass
//    {
//        private static readonly CreateIncompOPSClass _instance;
//        static CreateIncompOPSClass()
//        {
//            _instance = new CreateIncompOPSClass();
//        }

//        public static CreateIncompOPSClass Instance
//        {
//            get { return _instance; }
//        }

//        private  void IncompOpsIntializeCdoc(DocumentCT cdoc, Document_Type dt, AsycudaDocumentSet ads)
//        {
//            BaseDataModel.Instance.IntCdoc(cdoc, dt, ads);
//            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = false;
//            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Description = "Incomplete Allocations Opening Stock Entries";
//            cdoc.Document.xcuda_Declarant.Number = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Declarant_Reference_Number + "-INCOPS" + "-F" + cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.FileNumber.ToString();
//        }

//        public async Task CreateIncompOPS(string filterExpression, AsycudaDocumentSet docSet)
//        {

//            if (docSet == null)
//            {
//                throw new ApplicationException("Please Select a Asycuda Document Set before proceding");
//            }

//            var cdoc = new DocumentCT();
//            cdoc.Document = BaseDataModel.Instance.CreateNewAsycudaDocument(docSet);

//            StatusModel.Timer("Getting Data ...");

//            var itmcount = cdoc.DocumentItems.Count();
//            var slst = await AllocationsModel.Instance.GetAsycudaSalesAllocations(filterExpression).ConfigureAwait(false);


//            var dt = BaseDataModel.Instance.Document_Types.AsEnumerable().FirstOrDefault(x => x.DisplayName == "IM7");

//            if (dt == null)
//            {
//                throw new ApplicationException(string.Format("Null Document Type for '{0}' Contact your Network Administrator", "IM7"));
               
//            }

//            dt.DefaultCustoms_Procedure = BaseDataModel.Instance.Customs_Procedures.AsEnumerable().FirstOrDefault(x => x.DisplayName.Contains("OPS") && x.Document_TypeId == dt.Document_TypeId);

//            if (dt.DefaultCustoms_Procedure == null)
//            {
//                throw new ApplicationException(string.Format("Null Customs Procedure for '{0}' Contact your Network Administrator", "OPS"));
               
//            }

//            IncompOpsIntializeCdoc(cdoc, dt, docSet);

//            var cslst = slst.Where(x => x.EntryDataDetails.Quantity > 0)
//                .Select(x => x.EntryDataDetails)
//                .GroupBy(x => new {x.EntryDataDetailsId
//                })
//                .Select(x => new AllocationsModel.AlloEntryLineData()
//                {
//                    ItemNumber = x.First().ItemNumber,
//                    ItemDescription = x.First().ItemDescription,
//                    InventoryItem = x.First().InventoryItem,
//                    Cost = x.First().Cost,
//                    TariffCode = x.First().InventoryItem.TariffCode,
//                    Quantity = x.First().Quantity - x.First().QtyAllocated,
//                    EntryDataDetails = new List<IEntryDataDetail>() {x.FirstOrDefault()}
//                });

//            var res = cslst.GroupBy(x => new { x.ItemNumber, x.Cost })
//                .Select(g => new AllocationsModel.AlloEntryLineData
//                {
//                    ItemNumber = g.Key.ItemNumber,
//                    ItemDescription = g.First().ItemDescription,
//                    InventoryItem = g.First().InventoryItem,
//                    TariffCode = g.First().InventoryItem.TariffCode,
//                    Cost = g.Key.Cost,
//                    Quantity = g.Sum(z => z.Quantity),
//                    EntryDataDetails = g.SelectMany(z => z.EntryDataDetails).ToList()
//                });

//            StatusModel.StartStatusUpdate("Creating Incomplete Allocation OPS entries", cslst.Count());
//            foreach (var pod in res)
//            {
//                StatusModel.StatusUpdate();
               

//                var itm = BaseDataModel.Instance.CreateItemFromEntryDataDetail(pod, cdoc);
                

//                itmcount += 1;
//                if (itmcount % BaseDataModel.Instance.CurrentApplicationSettings.MaxEntryLines == 0)
//                {
//                    await BaseDataModel.Instance.SaveDocumentCT(cdoc).ConfigureAwait(false);
//                    //dup new file
//                    cdoc = new DocumentCT();
//                    cdoc.Document = BaseDataModel.Instance.CreateNewAsycudaDocument(docSet);

//                    IncompOpsIntializeCdoc(cdoc, dt, docSet);
//                }

//            }
//            if (cdoc.DocumentItems.Count == 0) cdoc = null;

//            await BaseDataModel.Instance.SaveDocumentCT(cdoc).ConfigureAwait(false);
            
//            StatusModel.StopStatusUpdate();



//        }
//    }
//}