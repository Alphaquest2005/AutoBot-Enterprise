using System;
using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming FileTypes, TODO_SubmitDiscrepanciesToCustoms, TODO_SubmitAllXMLToCustoms, Customs_Procedures are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class DISUtils
    {
        public static IEnumerable<IGrouping<string, TODO_SubmitDiscrepanciesToCustoms>> GetSubmitEntryData(FileTypes ft)
        {
            try
            {
                var cnumberList = ft.Data.Where(z => z.Key == "CNumber").Select(x => x.Value).ToList();

                var cplst = BaseDataModel.Instance.Customs_Procedures
                    .Where(x => x.CustomsOperation.Name == "Exwarehouse" || (x.CustomsOperation.Name == "Warehouse" && x.Stock == true)).Select(x => x.CustomsProcedure).ToList();

                // These call private methods which need to be in their own partial classes
                var res = cnumberList.Any()
                    ? GetSubmitEntryDataPerCNumber(cplst, cnumberList)
                    : GetSubmitEntryDataPerDocSet(ft.AsycudaDocumentSetId, cplst);

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