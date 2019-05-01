using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.UI;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;
using EntryDataQS.Business.Entities;
using SimpleMvvmToolkit;

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
        public static async Task AddDocToEntry(List<EntryDataEx> lst, AsycudaDocumentSet docSet)
        {
            await BaseDataModel.Instance.AddToEntry(lst, docSet).ConfigureAwait(false);
        }
        

        internal static async Task RemoveEntryData(EntryData entryData)
        {
            await BaseDataModel.Instance.RemoveEntryData(entryData).ConfigureAwait(false);
        }

        public async Task RemoveSelectedEntryData(List<EntryDataEx> lst)
        {
          
                StatusModel.StartStatusUpdate("Removing EntryData", lst.Count());
                var t = Task.Run(() =>
                {
                    using (var ctx = new EntryDataService())
                    {
                        foreach (var item in lst.ToList())
                        {

                            ctx.DeleteEntryData(item.InvoiceNo);
                            StatusModel.StatusUpdate();
                        }
                    }
                });
                await t.ConfigureAwait(false);

               
        }
    }
}