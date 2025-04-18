using System;
using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming TODO_SubmitDiscrepanciesToCustoms, TODO_SubmitAllXMLToCustoms, Customs_Procedures are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class DISUtils
    {
        public static IEnumerable<IGrouping<string, TODO_SubmitDiscrepanciesToCustoms>> GetSubmitEntryData()
        {
            try
            {
                var saleInfo = BaseDataModel.CurrentSalesInfo(0); // Assuming CurrentSalesInfo exists

                var cplst = BaseDataModel.Instance.Customs_Procedures
                    .Where(x => x.CustomsOperation.Name == "Exwarehouse" || (x.CustomsOperation.Name == "Warehouse" && x.Stock == true)).Select(x => x.CustomsProcedure).ToList();

                // This calls a private method which needs to be in its own partial class
                var res =  GetSubmitEntryDataPerDocSet(saleInfo.DocSet.AsycudaDocumentSetId, cplst); // Potential NullReferenceException if saleInfo or DocSet is null

                // This calls a private method which needs to be in its own partial class
                var lst = CreateDISEntryDataFromSubmitData(res);
                return lst;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}