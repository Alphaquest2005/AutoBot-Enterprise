
using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using InventoryQS.Client.DTO;
using InventoryQS.Client.Contracts;
using WaterNut.Client.Services;
using Core.Common.Contracts;
using System.ComponentModel.Composition;


namespace InventoryQS.Client.Services
{
   
    public partial class InventoryItemsExClient 
    {

        public async Task AssignTariffToItms(List<int> list, string tariffCodes)
        {
            await Channel.AssignTariffToItms(list, tariffCodes).ConfigureAwait(false);
        }

        public async Task ValidateExistingTariffCodes(int docSetId)
        {
            await Channel.ValidateExistingTariffCodes(docSetId).ConfigureAwait(false);
        }



        public async Task SaveInventoryItemsEx(InventoryItemsEx olditm)
        {
            await Channel.SaveInventoryItemsEx(olditm).ConfigureAwait(false);
        }
    }
}

