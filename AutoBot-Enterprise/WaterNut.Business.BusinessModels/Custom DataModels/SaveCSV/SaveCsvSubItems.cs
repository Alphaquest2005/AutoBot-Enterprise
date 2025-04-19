using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Core.Common.CSV;
using Core.Common.UI;
using CoreEntities.Business.Services;
using DocumentItemDS.Business.Entities;
using DocumentItemDS.Business.Services;
using DocumentItemDS.Business.Services;
using InventoryDS.Business.Entities;
using InventoryDS.Business.Services;
using SimpleMvvmToolkit;
using TrackableEntities;
using TrackableEntities.Common;

using SubItemsService = DocumentItemDS.Business.Services.SubItemsService;


namespace WaterNut.DataSpace
{
    public class SaveCsvSubItems
    {
        private static readonly SaveCsvSubItems instance;
        static SaveCsvSubItems()
        {
            instance = new SaveCsvSubItems();
        }

        public static SaveCsvSubItems Instance
        {
            get { return instance; }
        }



        internal async Task<bool> ExtractSubItems(string fileType, string[] lines, string[] headings, string csvType)
        {
            var mapping = new Dictionary<string, int>();
            GetMappings(mapping, headings);
            var eslst = GetSubItemData(lines, mapping);

            if (eslst == null) return false ;

            await ImportInventory(eslst).ConfigureAwait(false);

            if (await ImportSubItems(eslst).ConfigureAwait(false)) return true;
            return false;

        }

        private async Task<bool> ImportSubItems(List<SubItemData> eslst)
        {
            try
            {
                
                var itmgrp = eslst.GroupBy(x => new {x.Precision_4, x.CNumber, x.RegistrationDate});
                StatusModel.StartStatusUpdate("Saving To Database", itmgrp.Count());
                foreach (var itm in itmgrp)
                {
                    StatusModel.StatusUpdate();
                    var xitm = (await GetXcudaItems(itm.Key).ConfigureAwait(false));
                    if (xitm == null)
                    {
                        MessageBox.Show(string.Format(
                            "Please Ensure Item '{0}' on CNumber '{1}' Date '{2}' is imported",
                            itm.Key.Precision_4,
                            itm.Key.CNumber,
                            itm.Key.RegistrationDate.ToShortDateString()));
                        return false;
                    }
                    var i = 0;
                    foreach (var xs in itm)
                    {
                        i += 1;
                        if (i % 300 == 0) await SaveXcuda_Item(xitm).ConfigureAwait(false);
                        var s = GetSubItem(xitm, xs);
                        UpdateSubItem(s, xs, xitm);
                    }

                    await SaveXcuda_Item(xitm).ConfigureAwait(false);

                }
                StatusModel.StopStatusUpdate();
             

                return true;
            }
            catch (Exception)
            {

                throw;
            }

        }

        private async Task SaveXcuda_Item(xcuda_Item xitm)
        {
            try
            {
                using (var ctx = new xcuda_ItemService())
                {
                    await ctx.Updatexcuda_Item(xitm).ConfigureAwait(false);
                   
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        private async Task SaveSubItem(SubItems s)
        {
            using (var ctx = new SubItemsService())
            {
               await ctx.UpdateSubItems(s).ConfigureAwait(false);
            }

        }

        private static void UpdateSubItem(SubItems s, SubItemData itm, xcuda_Item xitm)
        {
            if (s.ItemDescription != itm.ItemDescription) s.ItemDescription = itm.ItemDescription;
            if (s.Item_Id != xitm.Item_Id) s.Item_Id = xitm.Item_Id;
            if (s.QtyAllocated != itm.QtyAllocated) s.QtyAllocated = itm.QtyAllocated;
            if (s.Quantity != itm.Quantity) s.Quantity = itm.Quantity;
        }

        private static SubItems GetSubItem(xcuda_Item xitm, SubItemData subItemData)
        {
            SubItems s = null;
            //if (xitm.SubItems.Any())
            //{
                s = xitm.SubItems.FirstOrDefault(x => x.ItemNumber == subItemData.ItemNumber);
           // }
            if (s == null)
            {
            s = new SubItems()
                {
                    ItemNumber = subItemData.ItemNumber,
                    Item_Id = xitm.Item_Id,
                   // xcuda_Item = xitm,
                    TrackingState = TrackingState.Added
                };
            xitm.SubItems.Add(s);
            }
            return s;
        }

        private async Task<xcuda_Item> GetXcudaItems(dynamic itm)
        {
            try
            {

                using (var itmctx = new AsycudaDocumentItemService())
                {
                    var xitm =
                        await itmctx.GetAsycudaDocumentItemsByExpressionLst(new List<string>()
                        {
                            string.Format("ItemNumber == \"{0}\"", itm.Precision_4),
                            string.Format("AsycudaDocument.CNumber == \"{0}\"", itm.CNumber),
                            string.Format("AsycudaDocument.RegistrationDate == \"{0}\"",
                                itm.RegistrationDate.ToShortDateString())
                        },
                            new List<string>()
                            {
                                "SubItems"
                            }
                            ).ConfigureAwait(false);
                    if (xitm.FirstOrDefault() != null)
                    {
                        using (var dctx = new xcuda_ItemService())
                        {
                            return
                                await dctx.Getxcuda_ItemByKey(xitm.FirstOrDefault().Item_Id.ToString(),new List<string>() {"SubItems"}).ConfigureAwait(false);
                        }
                    }
                    return null;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        private async Task ImportInventory(List<SubItemData> eslst)
        {
            var itmlst = from i in eslst
                         group i by i.ItemNumber.ToUpper()
                             into g
                             select new { ItemNumber = g.Key, g.FirstOrDefault().ItemDescription };
            StatusModel.StartStatusUpdate("Updating Inventory", itmlst.Count());
            using (var ctx = new InventoryItemService())
            {
                var i = 0;
                foreach (var item in itmlst)
                {
                    i += 1;
                    StatusModel.StatusUpdate("Updating Inventory",i);
                    var inv =
                        (await ctx.GetInventoryItemsByExpressionLst(new List<string>()
                        {
                            string.Format("ItemNumber.ToUpper() == \"{0}\"",item.ItemNumber.ToUpper())
                        }).ConfigureAwait(false)).FirstOrDefault();
                    //FirstOrDefault(
                    //        x => x.ItemNumber.ToUpper() == item.ItemNumber.ToUpper());

                    if (inv == null)
                    {
                        inv = new InventoryItem()
                        {
                            Description = item.ItemDescription,
                            ItemNumber = item.ItemNumber,
                            TrackingState= TrackingState.Added
                        };
                        await ctx.UpdateInventoryItem(inv).ConfigureAwait(false);
                    }
                }
                StatusModel.StopStatusUpdate();
            }
        }

        private List<SubItemData> GetSubItemData(string[] lines, Dictionary<string, int> mapping)
        {
            try
            {

            
            var eslst = new List<SubItemData>();
            StatusModel.StartStatusUpdate("Extracting SubItems",lines.Length);
            for (var i = 1; i < lines.Count(); i++)
            {
                StatusModel.StatusUpdate("Extracting SubItems", i);
                var d = GetCSVDataFromLine(lines[i], mapping);
                if (d != null)
                {
                    eslst.Add(d);
                }
            }
            StatusModel.StopStatusUpdate();
            return eslst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private SubItemData GetCSVDataFromLine(string line, Dictionary<string, int> mapping)
        {
            try
            {
                var splits = line.CsvSplit();
                if (splits[mapping["Precision_4"]] != "" && splits[mapping["ItemNumber"]] != "")
                {
                    return new SubItemData()
                    {
                        Precision_4 = splits[mapping["Precision_4"]],
                        CNumber = splits[mapping["CNumber"]],
                        RegistrationDate = DateTime.Parse(splits[mapping["RegistrationDate"]]),
                        ItemNumber = splits[mapping["ItemNumber"]],
                        ItemDescription = splits[mapping["ItemDescription"]],
                        Quantity = Convert.ToDouble(splits[mapping["Quantity"]]),
                        QtyAllocated = mapping.ContainsKey("QtyAllocated") ? Convert.ToDouble(mapping["QtyAllocated"]) : 0
                    };
                }
            }
            catch (Exception Ex)
            {
                throw;
            }
            return null;
        }

        private void GetMappings(Dictionary<string, int> mapping, string[] headings)
        {
            for (var i = 0; i < headings.Count(); i++)
            {
                var h = headings[i].Trim().ToUpper();

                if (h == "") continue;
                
                if ("Precision_4".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("Precision_4", i);
                    continue;
                }

                if ("CNumber".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("CNumber", i);
                    continue;
                }

                if ("Date".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("RegistrationDate", i);
                    continue;
                }

                if ("ItemNumber".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("ItemNumber", i);
                    continue;
                }

                if ("ItemDescription".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("ItemDescription", i);
                    continue;
                }

                if ("Quantity".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("Quantity", i);
                    continue;
                }

                if ("QtyAllocated".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("QtyAllocated", i);
                    continue;
                }


            }
        }
        class SubItemData
        {
            public string Precision_4 { get; set; }
            public string CNumber { get; set; }
            public DateTime RegistrationDate { get; set; }
            public string ItemNumber { get; set; }
            public string ItemDescription { get; set; }
            public double Quantity { get; set; }
            public double QtyAllocated { get; set; }
        }
    }
}
