using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.UI;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;

namespace WaterNut.DataSpace
{
	public class EntryDataModel 
    {
        private static readonly EntryDataModel instance;
        static EntryDataModel()
        {
            instance = new EntryDataModel();
        }

        public static EntryDataModel Instance
        {
            get { return instance; }
        }
       
        

        internal static async Task RemoveEntryData(EntryData entryData)
        {
            await BaseDataModel.Instance.RemoveEntryData(entryData.EntryDataId).ConfigureAwait(false);
        }

        public async Task RemoveSelectedEntryData(IEnumerable<string> lst)
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