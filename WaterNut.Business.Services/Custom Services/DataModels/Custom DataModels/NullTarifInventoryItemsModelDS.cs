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
                     var itm = await ctx.GetInventoryItemByKey(item).ConfigureAwait(false);
                     itm.TariffCode = tariffCode;
                     await ctx.UpdateInventoryItem(itm).ConfigureAwait(false);
                 }
             }
            
             
         }

	    //internal async Task MapInventoryToAsycuda()
	    //{
	    //    try
	    //    {
	    //        var ilst = new List<InventoryItem>();
	    //        using (var ctx = new InventoryItemService() {StartTracking = true})
	    //        {
	    //            ilst.AddRange(await ctx.GetInventoryItems(new List<string>()
	    //            {
	    //                "InventoryAsycudaMappings"
	    //            }).ConfigureAwait(false));
	    //        }

	    //        var alst = new List<xcuda_Item>();
	    //        using (var ctx = new xcuda_ItemService())
	    //        {
	    //            alst.AddRange(ctx.Getxcuda_ItemByExpressionLst(
	    //                new List<string>()
	    //                {
     //                       "(AsycudaDocument.DocumentType == \"IM7\" || AsycudaDocument.DocumentType == \"OS7\") && (AsycudaDocument.CNumber != null || AsycudaDocument.IsManuallyAssessed == true)"
     //                   },
	    //                new List<string>()
	    //                {
	    //                    "SubItems",
	    //                    "AsycudaDocument",
	    //                    "xcuda_Tarification.xcuda_HScode",
	    //                    "xcuda_Tarification.xcuda_Supplementary_unit"
	    //                }).Result.Distinct()); //, "EX"

	    //        }

	    //        if (BaseDataModel.Instance.CurrentApplicationSettings.ItemDescriptionContainsAsycudaAttribute == true)
	    //        {
	    //            // get inventory
	    //            foreach (var itm in ilst)
	    //            {
	    //                var invDescrip = itm.Description;

	    //                string attrib = invDescrip.Split('|').Length > 2
	    //                    ? invDescrip.Split('|')[2].ToUpper().Replace(" ", "")
	    //                    : null;
	    //                var res = AllocationsBaseModel.Instance.GetAsycudaEntriesWithItemNumber(alst, attrib, invDescrip,
	    //                    new List<string>() {itm.ItemNumber}).ToList();
	    //                foreach (var ae in res)
	    //                {
	    //                    if (!itm.InventoryAsycudaMappings.Any(x => x.Item_Id == ae.Item_Id))
	    //                    {
	    //                        itm.InventoryAsycudaMappings.Add(new InventoryAsycudaMapping(true)
	    //                        {
	    //                            ItemNumber = itm.ItemNumber,
	    //                            Item_Id = ae.Item_Id,
	    //                            TrackingState = TrackingState.Added
	    //                        });
	    //                    }
	    //                }
	    //            }


	    //            ilst.AsParallel(new ParallelLinqOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }).ForAll(x =>
	    //            {
	    //                using (var ctx = new InventoryDSContext())
	    //                {
	    //                    ctx.ApplyChanges(x);
	    //                    ctx.SaveChanges();
	    //                }
	    //            });

	    //            //await MapWhereItemDescriptionContainsAsycudaAttribute().ConfigureAwait(false);
	    //        }
	    //        else
	    //        {
	    //            //await MapByMatchingSalestoAsycudaEntriesOnItemNumber().ConfigureAwait(false);
	    //        }

	    //    }
	    //    catch (Exception)
	    //    {
     //           throw;
	    //    }

	    //}


	}
}