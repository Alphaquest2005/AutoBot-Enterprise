using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryDS.Business.Entities;
using InventoryQS.Business.Entities;
using Omu.ValueInjecter;

namespace InventoryQS.Business.Services
{
   
   
    public partial class InventoryItemsExService 
    {
        public async Task AssignTariffToItms(List<int> list, string tariffCodes)
        {
            await
                WaterNut.DataSpace.NullTarifInventoryItemsModel.Instance.AssignTariffToItms(list, tariffCodes)
                    .ConfigureAwait(false);
        }

        public async Task ValidateExistingTariffCodes(int docSetId)
        {
            var docSet = await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false);
            await WaterNut.DataSpace.BaseDataModel.Instance.ValidateExistingTariffCodes(docSet).ConfigureAwait(false);
        }

        //public async Task MapInventoryToAsycuda()
        //{
        //    await WaterNut.DataSpace.NullTarifInventoryItemsModel.Instance.MapInventoryToAsycuda().ConfigureAwait(false);
        //}

        public async Task SaveInventoryItemsEx(InventoryItemsEx olditm)
        {
            var itm = new InventoryDSContext().InventoryItems.First(x => x.Id == olditm.InventoryItemId && x.ApplicationSettingsId == olditm.ApplicationSettingsId);
            //itm.ApplicationSettingsId = olditm.ApplicationSettingsId;
            itm.TariffCode = olditm.TariffCode;

            await WaterNut.DataSpace.InventoryDS.DataModels.BaseDataModel.Instance.SaveInventoryItem(itm).ConfigureAwait(false);
        }
    }
}



