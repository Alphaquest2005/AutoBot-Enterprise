using System.Threading.Tasks;
using InventoryDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public partial class TariffCodesModel
    {
        private static readonly TariffCodesModel instance;
        static TariffCodesModel()
        {
            instance = new TariffCodesModel();
        }

        public static TariffCodesModel Instance
        {
            get { return instance; }
        }
        public async Task AssignTariffCodeToItm(InventoryItem itm, TariffCode t)
        {
            

            itm.TariffCode = t.TariffCodeName;
            await
                BaseDataModel.Instance.SaveInventoryItem(itm)
                    .ConfigureAwait(false);
        }
    }
}
