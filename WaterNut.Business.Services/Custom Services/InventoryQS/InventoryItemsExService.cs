using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using InventoryDS.Business.Entities;
using InventoryQS.Business.Entities;
using MoreLinq;
using Omu.ValueInjecter;
using WaterNut.Business.Services.Utils;

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


        public static async Task<Dictionary<string, (string ItemNumber, string ItemDescription, string TariffCode)>>
            ClassifiedItms(List<(string ItemNumber, string ItemDescription, string TariffCode)> Itms)
        {
            try
            {
                //var itms = await new DeepSeekApi().ClassifyItemsAsync(Itms).ConfigureAwait(false);
                var itms = Itms.DistinctBy(x => x.ItemNumber).ToDictionary(x => x.ItemNumber, x => x);
                var res = new Dictionary<string, (string ItemNumber, string ItemDescription, string TariffCode)>();


                foreach (var itm in itms)
                {
                    var tariffCode = await GetTariffCode(itm.Value.TariffCode).ConfigureAwait(false);

                    res.Add(itm.Key,
                        tariffCode == itm.Value.TariffCode
                            ? itm.Value
                            : (itm.Value.ItemNumber, itm.Value.ItemDescription, tariffCode));
                }


                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static async Task<string> GetTariffCode(string suspectedTariffCode)
        {
            if (string.IsNullOrEmpty(suspectedTariffCode))
                return suspectedTariffCode;

            var partialCode = suspectedTariffCode.Length >= 6
                ? suspectedTariffCode.Substring(0, 6)
                : suspectedTariffCode;

            var code90 = partialCode + "90";
            var code00 = partialCode + "00";

            using var context = new InventoryDSContext();
            return await context.TariffCodes
                .Where(x => x.RateofDuty != null)
                .Where(x => x.TariffCodeName == suspectedTariffCode
                            || x.TariffCodeName == code90
                            || x.TariffCodeName == code00
                            || x.TariffCodeName.StartsWith(partialCode))
                .OrderByDescending(x => x.TariffCodeName.Length)
                .ThenBy(x => x.TariffCodeName)
                .Select(x => x.TariffCodeName)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false) ?? suspectedTariffCode;
        }


    }
}



