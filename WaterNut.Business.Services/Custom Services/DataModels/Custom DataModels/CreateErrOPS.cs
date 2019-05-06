using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using Core.Common.UI;
using DocumentDS.Business.Entities;
using WaterNut.Business.Entities;
using WaterNut.Interfaces;


namespace WaterNut.DataSpace
{
    public class CreateErrOPS
    {
        private static readonly CreateErrOPS _instance;
        static CreateErrOPS()
        {
            _instance = new CreateErrOPS();
        }

        public static CreateErrOPS Instance
        {
            get { return _instance; }
        }


        public async Task CreateErrorOPS(string filterExpression, AsycudaDocumentSet docSet)
        {


            var cdoc = new DocumentCT();
            cdoc.Document = BaseDataModel.Instance.CreateNewAsycudaDocument(docSet);

            StatusModel.Timer("Getting Data ...");

            var itmcount = 0;
            var slst = await GetErrOPSData(filterExpression).ConfigureAwait(false);


            Document_Type dt;
            if (GetOPSDocumentType(out dt)) return;
            
            ErrOpsIntializeCdoc(cdoc, dt, docSet);

            StatusModel.StartStatusUpdate("Creating Error OPS entries", slst.Count());


            var cslst = AllocationEntryLine(slst);


            foreach (var pod in cslst)
            {
                StatusModel.StatusUpdate();

                BaseDataModel.Instance.CreateItemFromEntryDataDetail(pod, cdoc);


                itmcount += 1;
                if (itmcount % BaseDataModel.Instance.CurrentApplicationSettings.MaxEntryLines == 0)
                {
                    await BaseDataModel.Instance.SaveDocumentCT(cdoc).ConfigureAwait(false);
                    //dup new file
                    cdoc = new DocumentCT();
                    cdoc.Document = BaseDataModel.Instance.CreateNewAsycudaDocument(docSet);

                    ErrOpsIntializeCdoc(cdoc, dt, docSet);
                }

            }
            if (cdoc.DocumentItems.Count == 0) cdoc = null;

            await BaseDataModel.Instance.SaveDocumentCT(cdoc).ConfigureAwait(false);
            StatusModel.StopStatusUpdate();


        }

        private IEnumerable<AllocationsModel.AlloEntryLineData> AllocationEntryLine(List<AsycudaSalesAllocations> slst)
        {
            var cslst = from s in slst
                        where
                            s.EntryDataDetails.EntryDataId != null 
                            //||
                            //s.EntryDataDetails.Quantity != s.EntryDataDetails.QtyAllocated
                        group s by new
                        {
                            s.EntryDataDetails.ItemNumber,
                            s.EntryDataDetails.ItemDescription,
                            s.EntryDataDetails.Cost,
                            s.EntryDataDetails.EntryDataDetailsEx.InventoryItemsEx.TariffCode
                            //,
                          //  s.EntryDataDetails.InventoryItem
                        }
                into g
                        select new AllocationsModel.AlloEntryLineData
                        {
                            ItemNumber = g.Key.ItemNumber,
                            ItemDescription = g.Key.ItemDescription,
                            Cost = g.Key.Cost,
                            TariffCode = g.Key.TariffCode,
                            InventoryItem = g.First().EntryDataDetails.EntryDataDetailsEx.InventoryItemsEx as IInventoryItem,
                            Quantity = g.Sum(x => x.QtyAllocated),
                            EntryDataDetails = g.Select(x => new EntryDataDetailSummary()
                            {
                                EntryDataDetailsId = (int) x.EntryDataDetailsId,
                                EntryDataId = x.EntryDataDetails.EntryDataId,
                                QtyAllocated = x.QtyAllocated
                            }).ToList()
                        };
            return cslst.Where(x => x.Quantity > 0);
        }


        private bool GetOPSDocumentType(out Document_Type dt)
        {
            dt = BaseDataModel.Instance.Document_Types.AsEnumerable().FirstOrDefault(x => /*x.DisplayName == "IM7" ||*/ x.DisplayName == "OS7");

            if (dt == null)
            {
                throw new ApplicationException($"Null Document Type for '{"IM7"}' Contact your Network Administrator");

            }
            var docTypeId = dt.Document_TypeId;
            dt.DefaultCustoms_Procedure =
                BaseDataModel.Instance.Customs_Procedures.AsEnumerable()
                    .FirstOrDefault(x => x.DisplayName.Contains("OPS") && x.Document_TypeId == docTypeId);

            if (dt.DefaultCustoms_Procedure == null)
            {
                throw new ApplicationException(
                    $"Null Customs Procedure for '{"OPS"}' Contact your Network Administrator");

            }
            return false;
        }

        private async Task<List<AsycudaSalesAllocations>> GetErrOPSData(string filterExpression)
        {
            var lst = await AllocationsModel.Instance.GetAsycudaSalesAllocations(filterExpression).ConfigureAwait(false);
            return lst.Where(x => !string.IsNullOrEmpty(x.Status) && x.Status != "Net Weight < 0.01").ToList();
            //return lst.Where(x => x.PreviousDocumentItem == null 
            //                      && x.QtyAllocated == 0
            //                      && x.EntryDataDetails.Quantity > 0
            //                      && x.EntryDataDetails.Cost > 0).ToList() ;
            //res.Append("PreviousItem_Id == 0" +
            //              "&& SalesQtyAllocated == 0" +
            //              "&& SalesQuantity > 0 " +
            //              "&& Cost > 0");
        }

        public void ErrOpsIntializeCdoc(DocumentCT cdoc, Document_Type dt, AsycudaDocumentSet ads)
        {
            BaseDataModel.Instance.IntCdoc(cdoc, ads);
            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = false;
            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Description = "Error Opening Stock Entries";
            cdoc.Document.xcuda_Declarant.Number = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Declarant_Reference_Number + "-ERROPS" + "-F" + cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.FileNumber.ToString();
        }


    }
}