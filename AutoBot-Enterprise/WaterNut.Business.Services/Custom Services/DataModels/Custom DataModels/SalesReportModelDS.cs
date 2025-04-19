using CoreEntities.Business.Entities;
using CoreEntities.Business.Enums;
using CoreEntities.Business.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;


namespace WaterNut.DataSpace
{
	public class SalesReportModel
    {
		
        private static readonly SalesReportModel instance;
        static SalesReportModel()
        {
            instance = new SalesReportModel();
        }

        public static SalesReportModel Instance
        {
            get { return instance; }
        }



        //public static void Send2Excel(IEnumerable<SaleReportLine> slst)
        //{
        //    if (slst == null) return;
        //    var s = new ExportToExcel<SaleReportLine, List<SaleReportLine>>();
        //    s.dataToPrint = slst.ToList();
        //    s.GenerateReport();
        //}



	    public async Task<IEnumerable<AsycudaDocument>> GetSalesDocuments(int docSetId)
	    {
	        List<int> doclst;
	        using (var ctx = new DocumentDSContext())
	        {
                doclst =
	                ctx.xcuda_ASYCUDA_ExtendedProperties.Where(x => x.AsycudaDocumentSetId == docSetId)
	                    .Select(x => x.ASYCUDA_Id).ToList();
	        }
	        foreach (var d in doclst)
	        {
	            await AsycudaEntrySummaryListModel.ReorderDocumentItems(d).ConfigureAwait(false);
	        }
                

            using (var ctx = new AsycudaDocumentService())
	        {
	            var lst =

	                (await
	                    ctx.GetAsycudaDocumentsByExpression(
	                        $"AsycudaDocumentSetId == {docSetId} && DoNotAllocate != true && (CustomsOperationId == {(int) CustomsOperations.Exwarehouse})",
                            null
	                        ).ConfigureAwait(false));

	            


	            return lst;
	        }
	    }

		public async Task<AsycudaDocument> GetSalesDocument(int docId)
		{
			try
			{




				List<int> doclst;
				using (var ctx = new DocumentDSContext())
				{
					doclst =
						ctx.xcuda_ASYCUDA_ExtendedProperties.Where(x => x.ASYCUDA_Id == docId)
							.Select(x => x.ASYCUDA_Id).ToList();
				}
				foreach (var d in doclst)
				{
					await AsycudaEntrySummaryListModel.ReorderDocumentItems(d).ConfigureAwait(false);
				}


				using (var ctx = new AsycudaDocumentService())
				{
					var lst =

						(await
							ctx.GetAsycudaDocumentsByExpression(
								$"ASYCUDA_Id == {docId} && DoNotAllocate != true && (CustomsOperationId == {(int)CustomsOperations.Exwarehouse})",
								null
								).ConfigureAwait(false));




					return lst.FirstOrDefault();
				}
			}
			catch (System.Exception)
			{

				throw;
			}
		}
	}
}