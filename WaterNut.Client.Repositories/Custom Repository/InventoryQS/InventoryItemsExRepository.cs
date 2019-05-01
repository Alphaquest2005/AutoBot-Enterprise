
using System.Collections.Generic;
using System.Threading.Tasks;

using System;
using InventoryQS.Client.Entities;
using InventoryQS.Client.Services;

namespace InventoryQS.Client.Repositories
{

    public partial class InventoryItemsExRepository
    {
        public async Task AssignTariffToItms(IEnumerable<string> list, string tariffCodes)
        {
            using (var ctx = new InventoryItemsExClient())
            {
                await ctx.AssignTariffToItms(list, tariffCodes).ConfigureAwait(false);
            }
        }

        public async Task ValidateExistingTariffCodes(int docSetId)
        {
            using (var ctx = new InventoryItemsExClient())
            {
                await ctx.ValidateExistingTariffCodes(docSetId).ConfigureAwait(false);
            }
        }



        public async Task SaveInventoryItemsEx(InventoryItemsEx olditm)
        {
            using (var ctx = new InventoryItemsExClient())
            {
                await ctx.SaveInventoryItemsEx(olditm.DTO).ConfigureAwait(false);
            }
        }

    }
}

