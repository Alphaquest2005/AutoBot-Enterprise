using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.UI;
using EntryDataDS.Business.Services;
using SalesDataQS.Business.Entities;


namespace WaterNut.DataSpace
{
    public class Ex9SalesDataModel : BaseDataModel
	{
            


        

      

        internal static void RemoveEntryData(SalesData entryData)
        {
            throw new NotImplementedException();
        }

        internal static void GetFile(string p)
        {
            throw new NotImplementedException();
        }

        internal async Task RemoveSelectedEntryData(IEnumerable<string> lst)
        {
            StatusModel.StartStatusUpdate("Removing EntryData", lst.Count());
            var t = Task.Run(() =>
            {
                using (var ctx = new EntryDataService())
                {
                    foreach (var item in lst.ToList())
                    {

                        ctx.DeleteEntryData(item).Wait();
                        StatusModel.StatusUpdate();
                    }
                }
            });
            await t.ConfigureAwait(false);
        }
    }
}