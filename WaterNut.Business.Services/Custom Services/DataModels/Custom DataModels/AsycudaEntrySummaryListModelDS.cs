using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DocumentItemDS.Business.Entities;
using DocumentItemDS.Business.Services;

namespace WaterNut.DataSpace
{
    public class AsycudaEntrySummaryListModel
    {
        static AsycudaEntrySummaryListModel()
        {
            Instance = new AsycudaEntrySummaryListModel();
        }

        public static AsycudaEntrySummaryListModel Instance { get; }

        internal async Task RemoveSelectedItems(IEnumerable<int> lst)
        {
            var ASYCUDA_Id = new List<int>();
            using (var ctx = new DocumentItemDSContext {StartTracking = true})
            {
                foreach (var item_id in lst)
                {
                    var item =
                        ctx.xcuda_Item.Include(x => x.xcuda_PreviousItem).FirstOrDefault(x => x.Item_Id == item_id);

                    if (item == null) continue;
                    ctx.Database.ExecuteSqlCommand(
                        $"delete from AsycudaSalesAllocations where PreviousItem_Id = '{item.Item_Id}'");
                    if (!ASYCUDA_Id.Contains(item.ASYCUDA_Id)) ASYCUDA_Id.Add(item.ASYCUDA_Id);
                    ctx.xcuda_Item.Remove(item);
                }


                ctx.SaveChanges();
            }

            foreach (var doc in ASYCUDA_Id) await ReorderDocumentItems(doc);
        }

        public static async Task ReorderDocumentItems(int ASYCUDA_Id)
        {
            List<xcuda_Item> rlst;
            using (var ctx = new xcuda_ItemService())
            {
                rlst =
                    (await ctx.Getxcuda_ItemByExpression($"ASYCUDA_Id == {ASYCUDA_Id}",
                            new List<string>
                            {
                                "xcuda_PreviousItem"
                            })
                        .ConfigureAwait(false))
                    .OrderBy(x => x.LineNumber)
                    .ToList();
            }

            //if (!rlst.Where(x => x.xcuda_PreviousItem != null).Select(x => x.xcuda_PreviousItem).Any()) return;

            for (var i = 0; i < rlst.Count(); i++)
            {
                rlst.ElementAt(i).LineNumber = i + 1;
                if (rlst.ElementAt(i).xcuda_PreviousItem != null)
                {
                    rlst.ElementAt(i).xcuda_PreviousItem.StartTracking();
                    rlst.ElementAt(i).xcuda_PreviousItem.Current_item_number = (i + 1);
                }
            }

            using (var ctx = new xcuda_PreviousItemService())
            {
                foreach (var p in rlst.Select(x => x.xcuda_PreviousItem))
                {
                    if (p == null) continue;
                    p.xcuda_Item = null;
                    await ctx.Updatexcuda_PreviousItem(p).ConfigureAwait(false);
                }
            }

            using (var ctx = new DocumentItemDSContext())
            {
                foreach (var i in rlst)
                    ctx.Database.ExecuteSqlCommand("update xcuda_Item" +
                                                   $" set linenumber = {i.LineNumber}" +
                                                   $" where Item_Id = {i.Item_Id}");
            }
        }
    }
}