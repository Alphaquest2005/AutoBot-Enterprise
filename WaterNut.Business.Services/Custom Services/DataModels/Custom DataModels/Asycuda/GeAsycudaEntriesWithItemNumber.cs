using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using Core.Common.UI;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingXcudaItems;

namespace WaterNut.DataSpace
{
    public class GeAsycudaEntriesWithItemNumber
    {
        public static Task<(ConcurrentDictionary<int, xcuda_Item> asycudaItems, IEnumerable<AllocationsBaseModel.ItemEntries>
                asycudaEntries)> GetAsycudaEntriesWithItemNumber(
                List<(string ItemNumber, int InventoryItemId)> itemList, int? asycudaDocumentSetId)
        {
            try
            {
                StatusModel.Timer("Getting Data - Asycuda Entries...");
                //string itmnumber = "WMHP24-72";
                IEnumerable<AllocationsBaseModel.ItemEntries> asycudaEntries = null;


                var lst = GetXcudaItems(asycudaDocumentSetId, itemList);
               

                    // var res2 = lst.Where(x => x.ItemNumber == "PRM/84101");
                    var asycudaItems =
                    new ConcurrentDictionary<int, xcuda_Item>(
                        Enumerable.ToDictionary<xcuda_Item, int, xcuda_Item>(lst, x => x.Item_Id, x => x));
                asycudaEntries = Enumerable.Where<xcuda_Item>(lst, x => x != null)
                    .Where(x => !string.IsNullOrEmpty(x.ItemNumber))
                    .GroupBy(s => s.ItemNumber.ToUpper().Trim())
                    .Select(g => new AllocationsBaseModel.ItemEntries
                    {
                        Key = g.Key.Trim(),
                        EntriesList = g.AsEnumerable()
                            .OrderBy(x =>
                                x.AsycudaDocument.EffectiveRegistrationDate ??
                                Convert.ToDateTime(x.AsycudaDocument.RegistrationDate))
                            .ToList()
                    });
                return Task.FromResult((asycudaItems, asycudaEntries));


                //var res = asycudaEntries.Where(x => x.Key.Contains("8309"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static List<xcuda_Item> GetXcudaItems(int? asycudaDocumentSetId, List<(string ItemNumber, int InventoryItemId)> xlst)
        {

            return OldSalesAllocator.isDBMem == true
                ? Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingXcudaItems.GetXcudaItems
                    .Execute(asycudaDocumentSetId, xlst)
                :  new GetXcudaItemsMem().Execute(asycudaDocumentSetId, xlst);
        }
    }
}