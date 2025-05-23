using System;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDAPrev_decl is here
using DocumentItemDS.Business.Entities; // Assuming xcuda_Item, xcuda_PreviousItem are here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task LinkPIItem(ASYCUDAPrev_decl ai, xcuda_Item itm, xcuda_PreviousItem pi, bool noMessages)
        {
            // This calls GetOriginalEntryItemId, which needs to be in its own partial class
            var itemId = await GetOriginalEntryItemId(ai, itm.ItemNumber, noMessages).ConfigureAwait(false);

            var bl =
                $"{ai.Prev_decl_office_code} {ai.Prev_decl_reg_year} C {ai.Prev_decl_reg_number} art. {ai.Prev_decl_item_number}";

            if (itemId != 0)
            {
                // This calls LinkPi2Item, which needs to be in its own partial class
                await LinkPi2Item(itemId, pi).ConfigureAwait(false);
            }
            else
            {
                if (!NoMessages) // Assuming NoMessages is accessible field
                {
                    throw new ApplicationException("Can not find Original entry: " + bl);
                }
            }
        }
    }
}