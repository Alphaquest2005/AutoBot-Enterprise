using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.UI;
using DocumentDS.Business.Entities;
using SimpleMvvmToolkit;
using WaterNut.Business.Entities;
using WaterNut.Interfaces;

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

        public void OPSIntializeCdoc(DocumentCT cdoc, Document_Type dt, AsycudaDocumentSet ads)
        {
            BaseDataModel.Instance.IntCdoc(cdoc, dt, ads);
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
            


            var slst = from s in slstSource.Where(p => p.PreviousDocumentItem != null
                                                       && p.PreviousDocumentItem.AsycudaDocument.IsManuallyAssessed == true)
                group s by new
                {
                    s.EntryDataDetails.ItemNumber,
                    s.EntryDataDetails.ItemDescription,
                    s.EntryDataDetails.EntryDataDetailsEx.TariffCode,
                    s.EntryDataDetails.Cost,
                }
                into g
                select new
                {
                    Allocations = g.ToList(),
                    EntlnData = new AllocationsModel.AlloEntryLineData
                    {
                        ItemNumber = g.Key.ItemNumber,
                        ItemDescription = g.Key.ItemDescription,
                        TariffCode = g.Key.TariffCode,
                        Cost = g.Key.Cost,
                        Quantity = g.Sum(x => x.EntryDataDetails.Quantity),
                        EntryDataDetails = g.Select(x => x.EntryDataDetails as IEntryDataDetail).ToList()
                    }

                };

            if (!slst.Any())
            {
                throw new ApplicationException(
                    "No OPS Allocations found! If you just deleted Entries, Please Allocate Sales then continue Else Contact your Network Administrator");
                StatusModel.StopStatusUpdate();
                return;
            }

            var cdoc = new DocumentCT();
            cdoc.Document = BaseDataModel.Instance.CreateNewAsycudaDocument(docSet);



            var itmcount = cdoc.DocumentItems.Count();

            var dt = BaseDataModel.Instance.Document_Types.AsEnumerable().FirstOrDefault(x => x.DisplayName == "IM7");

            if (dt == null)
            {
                throw new ApplicationException(string.Format("Null Document Type for '{0}' Contact your Network Administrator", "IM7"));
                return;
            }

            dt.DefaultCustoms_Procedure =
                BaseDataModel.Instance.Customs_Procedures.AsEnumerable()
                    .FirstOrDefault(x => x.DisplayName.Contains("OPS") && x.Document_TypeId == dt.Document_TypeId);

            if (dt.DefaultCustoms_Procedure == null)
            {
                throw new ApplicationException(string.Format("Null Customs Procedure for '{0}' Contact your Network Administrator", "OPS"));
                return;
            }

            OPSIntializeCdoc(cdoc, dt, docSet);

            StatusModel.StartStatusUpdate("Creating Opening Stock Entries", slst.Count());


            foreach (var pod in slst)
            {

                StatusModel.StatusUpdate();

                var itm = BaseDataModel.Instance.CreateItemFromEntryDataDetail(pod.EntlnData, cdoc);

                cdoc.DocumentItems.Add(itm);
                foreach (var allo in pod.Allocations)
                {
                    allo.PreviousItem_Id  = itm.Item_Id;
                }



                itmcount += 1;
                if (itmcount % BaseDataModel.Instance.CurrentApplicationSettings.MaxEntryLines == 0)
                {
                    //dup new file
                    cdoc = new DocumentCT();
                    cdoc.Document = BaseDataModel.Instance.CreateNewAsycudaDocument(docSet);

                    OPSIntializeCdoc(cdoc, dt, docSet);
                }

            }
            if (cdoc.DocumentItems.Count == 0) cdoc = null;

            StatusModel.Timer("Saving to Database...");

            await BaseDataModel.Instance.SaveDocumentCT(cdoc).ConfigureAwait(false);

            StatusModel.StopStatusUpdate();



        }
    }
}