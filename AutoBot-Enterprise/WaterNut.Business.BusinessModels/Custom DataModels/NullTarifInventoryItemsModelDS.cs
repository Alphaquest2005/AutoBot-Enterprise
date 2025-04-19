using System;
using System.Collections;
using System.Threading.Tasks;
using Core.Common.UI.DataVirtualization;
using InventoryDS.Business.Entities;
using InventoryDS.Business.Services;
using SimpleMvvmToolkit;


namespace WaterNut.DataSpace
{
	public partial class NullTarifInventoryItemsModel
	{
         private static readonly NullTarifInventoryItemsModel instance;
         static NullTarifInventoryItemsModel()
        {
            instance = new NullTarifInventoryItemsModel();
        }

         public static NullTarifInventoryItemsModel Instance
        {
            get { return instance; }
        }

         public static async Task AssignTariffToItms(IList list, TariffCode tariffCode)
         {
             if (tariffCode == null)
             {
                 throw new ApplicationException("Please Select TariffCode then Continue");
                 
             }
             using (var ctx = new InventoryItemService())
             {
                 foreach (VirtualListItem<InventoryQS.Business.Entities.InventoryItemsEx> item in list)
                 {
                     InventoryItem itm = await ctx.GetInventoryItemByKey(item.Data.ItemNumber);
                     item.Data.TariffCode = tariffCode.TariffCodeName;
                     itm.TariffCode = tariffCode.TariffCodeName;
                     await ctx.UpdateInventoryItem(itm).ConfigureAwait(false);
                 }
             }
            
             
         }




    }
}