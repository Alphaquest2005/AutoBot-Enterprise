using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using TrackableEntities;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    public async Task LinkExistingPreviousItems(xcuda_ASYCUDA da, List<xcuda_Item> documentItems, bool update)
    {
        //get all previous items for this document
        try
        {
            IEnumerable<xcuda_PreviousItem> plst;

            plst = await DocumentItemDS.DataModels.BaseDataModel.Instance.Searchxcuda_PreviousItem(
                new List<string>
                {
                    $"Prev_reg_nbr == \"{da.xcuda_Identification.xcuda_Registration.Number}\""
                }).ConfigureAwait(false);


            if (plst.Any() == false)
                return; // || da.xcuda_Identification.xcuda_Type.DisplayName == "IM7"// im7s created from ex9 document can have previousitems... have to remove these
            foreach (var itm in documentItems)
            {
                var pplst = plst.Where(x => x.Previous_item_number == itm.LineNumber &&
                                            x.Prev_decl_HS_spec == itm.ItemNumber);

                foreach (var p in pplst)
                {
                    var ep = new global::DocumentItemDS.Business.Entities.EntryPreviousItems(true)
                    {
                        Item_Id = itm.Item_Id,
                        xcuda_Item = itm,
                        PreviousItem_Id = p.PreviousItem_Id,
                        xcuda_PreviousItem = p,
                        TrackingState = TrackingState.Added
                    };
                    itm.xcuda_PreviousItems.Add(ep);
                    if (!update) continue;
                    using (var ctx = new DocumentItemDSContext())
                    {
                        await ctx.Database.ExecuteSqlCommandAsync($@"INSERT INTO EntryPreviousItems
                                                                                (PreviousItem_Id, Item_Id)
                                                                                VALUES ({ep.PreviousItem_Id}, {ep.Item_Id})")
                            .ConfigureAwait(false);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}