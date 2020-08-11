using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using AllocationDS.Business.Services;
using InventoryDS.Business.Entities;
using InventoryDS.Business.Services;
using TrackableEntities;
using TrackableEntities.EF6;
using InventoryItemService = InventoryDS.Business.Services.InventoryItemService;


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

         public async Task AssignTariffToItms(IEnumerable<string> lst, string tariffCode)
         {
             if (tariffCode == null)
             {
                 throw new ApplicationException("Please Select TariffCode then Continue");
             }

             using (var ctx = new InventoryItemService())
             {
                 foreach (string item in lst)
                 {
                     var itm = (await ctx.GetInventoryItemsByExpression($"ItemNumber == \"{item}\"").ConfigureAwait(false)).FirstOrDefault();
                     itm.TariffCode = tariffCode;
                     await ctx.UpdateInventoryItem(itm).ConfigureAwait(false);
                 }
             }
            
             
         }

	   


	}
}