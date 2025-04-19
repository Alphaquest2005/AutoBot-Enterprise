using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Core.Common.CSV;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;
using InventoryDS.Business.Entities;
using InventoryDS.Business.Services;
using SimpleMvvmToolkit;
using TrackableEntities;


namespace WaterNut.DataSpace
{
    public class SaveCsvEntryData
    {
        private static readonly SaveCsvEntryData instance;
        static SaveCsvEntryData()
        {
            instance = new SaveCsvEntryData();
        }

        public static SaveCsvEntryData Instance
        {
            get { return instance; }
        }

        public async Task<bool> ExtractEntryData(string fileType, string[] lines, string[] headings, string csvType, AsycudaDocumentSet docSet, bool overWriteExisting)
        {
            if (docSet == null)
            {
               throw new ApplicationException("Please select Document Set before proceding!");
               
            }
            var mapping = new Dictionary<string, int>();
             GetMappings(mapping, headings);
            var eslst = GetCSVDataSummayList(lines, mapping);

            if (eslst == null) return true;


            if (csvType == "QB9")
            {
                foreach (var item in eslst)
                {
                    item.ItemNumber = item.ItemNumber.Split(':').Last();
                }
            }

            await ImportInventory(eslst).ConfigureAwait(false);

            if (await ImportEntryData(fileType, eslst,docSet.AsycudaDocumentSetId, overWriteExisting).ConfigureAwait(false)) return true;
            return false;
        }



        private async Task<bool> ImportEntryData(string fileType, List<CSVDataSummary> eslst, int docSetId, bool overWriteExisting)
        {
            var ed =
                eslst.GroupBy(es => new {es.EntryDataId, es.EntryDataDate, es.CustomerName, es.Tax})
                .Select(g => new
                {
                    e =
                        new
                        {
                            EntryDataId = g.Key.EntryDataId,
                            EntryDataDate = g.Key.EntryDataDate,
                            AsycudaDocumentSetId = docSetId,
                            CustomerName = g.Key.CustomerName,
                            Tax = g.Key.Tax
                        },
                    d = g.Select(x => new EntryDataDetails()
                    {
                        EntryDataId = x.EntryDataId,
                        ItemNumber = x.ItemNumber.ToUpper(),
                        ItemDescription = x.ItemDescription,
                        Cost = x.Cost,
                        Quantity = x.Quantity,
                        Units = x.Units
                    })
                });

            if (ed == null) return true;


            List<EntryData> eLst = null;
            
            foreach (var item in ed)
            {
                // check Existing items
                var olded = await GetEntryData(item.e.EntryDataId, item.e.EntryDataDate).ConfigureAwait(false);
                    
                if (olded != null)
                {
                  

                    switch (overWriteExisting)
                    {
                        case true:
                            await ClearEntryDataDetails(olded).ConfigureAwait(false);
                            await DeleteEntryData(olded).ConfigureAwait(false);

                            break;
                        case false:
                            continue;
                
                    }
                }


                switch (fileType)
                {
                    case "Sales":

                        var EDsale = new Sales()
                        {
                            EntryDataId = item.e.EntryDataId,
                            EntryDataDate = item.e.EntryDataDate,
                            INVNumber = item.e.EntryDataId,
                            TaxAmount = item.e.Tax,
                            CustomerName = item.e.CustomerName,
                            TrackingState = TrackingState.Added
                        };
                        EDsale.AsycudaDocumentSets.Add(new AsycudaDocumentSetEntryData()
                        {
                            AsycudaDocumentSetId = item.e.AsycudaDocumentSetId,
                            EntryDataId = item.e.EntryDataId,
                            TrackingState = TrackingState.Added
                        });
                        await CreateSales(EDsale).ConfigureAwait(false);
                        break;
                    case "PO":
                        var EDpo = new PurchaseOrders()
                        {
                            EntryDataId = item.e.EntryDataId,
                            EntryDataDate = item.e.EntryDataDate,
                            PONumber = item.e.EntryDataId,
                            TrackingState = TrackingState.Added
                        };
                        EDpo.AsycudaDocumentSets.Add(new AsycudaDocumentSetEntryData()
                        {
                            AsycudaDocumentSetId = item.e.AsycudaDocumentSetId,
                            EntryDataId = item.e.EntryDataId,
                            TrackingState = TrackingState.Added
                        });
                        await CreatePurchaseOrders(EDpo).ConfigureAwait(false);
                        break;
                    case "OPS":
                        var EDops = new OpeningStock()
                        {
                            EntryDataId = item.e.EntryDataId,
                            EntryDataDate = item.e.EntryDataDate,
                            OPSNumber = item.e.EntryDataId,
                            TrackingState = TrackingState.Added
                        };
                        EDops.AsycudaDocumentSets.Add(new AsycudaDocumentSetEntryData()
                        {
                            AsycudaDocumentSetId = item.e.AsycudaDocumentSetId,
                            EntryDataId = item.e.EntryDataId,
                            TrackingState = TrackingState.Added
                        });
                        await CreateOpeningStock(EDops).ConfigureAwait(false);
                        break;
                    default:
                        MessageBox.Show("Unknown FileType");
                        return true;
                }

                if (item.d.Count() == 0) MessageBox.Show(item.e.EntryDataId + " has no details");

                var details = item.d;

                using (var ctx = new EntryDataDetailsService())
                {
                    foreach (var e in details)
                    {
                        await ctx.CreateEntryDataDetails(new EntryDataDetails()
                        {
                            EntryDataId = e.EntryDataId,
                            ItemNumber = e.ItemNumber,
                            ItemDescription = e.ItemDescription,
                            Quantity = e.Quantity,
                            Cost = e.Cost,
                            Units = e.Units,
                            TrackingState = TrackingState.Added
                        }).ConfigureAwait(false);
                    }
                }
            }

            //MessageBus.Default.BeginNotify(MessageToken.EntryDataExChanged, null,
            //    new NotificationEventArgs(MessageToken.EntryDataExChanged));

            //MessageBus.Default.BeginNotify(MessageToken.EntryDataDetailsExesChanged, null,
            //    new NotificationEventArgs(MessageToken.EntryDataDetailsExesChanged));
            return false;
        }

        private async Task<EntryData> GetEntryData(string entryDataId, DateTime entryDateTime)
        {
            using (var ctx = new EntryDataService())
            {
                return
                    (await ctx.GetEntryDataByExpressionLst(new List<string>()
                    {
                        string.Format("EntryDataId == \"{0}\"",entryDataId),
                        string.Format("EntryDataDate == \"{0}\"", entryDateTime.ToString("yyyy-MMM-dd"))
                    }).ConfigureAwait(false)).FirstOrDefault();
                //eLst.FirstOrDefault(x => x.EntryDataId == item.e.EntryDataId && x.EntryDataDate != item.e.EntryDataDate);
            }
        }


        private void GetMappings(Dictionary<string, int> mapping, string[] headings)
        {
            for (var i = 0; i < headings.Count(); i++)
            {
                var h = headings[i].Trim().ToUpper();

                if (h == "") continue;

                if ("INVNO|Reciept #|NUM|Invoice #".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("EntryDataId", i);
                    continue;
                }

                if ("DATE|Invoice Date".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("EntryDataDate", i);
                    continue;
                }

                if ("ItemNumber|ITEM-#|Item Code".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("ItemNumber", i);
                    continue;
                }

                if ("DESCRIPTION|MEMO|Item Description".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("ItemDescription", i);
                    continue;
                }

                if ("QUANTITY|QTY".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("Quantity", i);
                    continue;
                }


                if ("PRICE|COST|Sales Price".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("Cost", i);
                    continue;
                }

                if ("UNITS".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("Units", i);
                    continue;
                }

                if ("Customer".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("CustomerName", i);
                    continue;
                }
                if ("Tax".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("Tax", i);
                    continue;
                }

             

            }
        }


        private List<CSVDataSummary> GetCSVDataSummayList(string[] lines, Dictionary<string, int> mapping)
        {
            var eslst = new List<CSVDataSummary>();
            for (var i = 1; i < lines.Count(); i++)
            {
                var d = GetCSVDataFromLine(lines[i], mapping);
                if (d != null)
                {
                    eslst.Add(d);
                }
            }
            return eslst;
        }



        private CSVDataSummary GetCSVDataFromLine(string line, Dictionary<string, int> mapping)
        {
            try
            {
                var splits = line.CsvSplit();
                if (splits[mapping["EntryDataId"]] != "" && splits[mapping["ItemNumber"]] != "")
                {
                    return new CSVDataSummary()
                    {
                        EntryDataId = splits[mapping["EntryDataId"]],
                        EntryDataDate = DateTime.Parse(splits[mapping["EntryDataDate"]]),
                        ItemNumber = splits[mapping["ItemNumber"]],
                        ItemDescription = splits[mapping["ItemDescription"]],
                        Cost = Convert.ToSingle(splits[mapping["Cost"]]),
                        Quantity = Convert.ToSingle(splits[mapping["Quantity"]]),
                        Units = mapping.ContainsKey("Units") ? splits[mapping["Units"]] : "",
                        CustomerName = mapping.ContainsKey("CustomerName") ? splits[mapping["CustomerName"]] : "",
                        Tax = Convert.ToSingle(mapping.ContainsKey("Tax") ? splits[mapping["Tax"]] : "0")
                    };
                }
            }
            catch (Exception Ex)
            {
                throw;
            }
            return null;
        }

        class CSVDataSummary
        {
            public string EntryDataId { get; set; }
            public DateTime EntryDataDate { get; set; }
            public string ItemNumber { get; set; }
            public string ItemDescription { get; set; }
            public Single Quantity { get; set; }
            public Single Cost { get; set; }
            public string Units { get; set; }
            public string CustomerName { get; set; }
            public Single Tax { get; set; }
        }

        private async Task ImportInventory(List<CSVDataSummary> eslst)
        {
            var itmlst = from i in eslst
                         group i by i.ItemNumber.ToUpper()
                             into g
                             select new { ItemNumber = g.Key, g.FirstOrDefault().ItemDescription };

            using (var ctx = new InventoryItemService())
            {
                foreach (var item in itmlst)
                {
                    var i =
                        BaseDataModel.Instance.InventoryCache.GetSingle(
                            x => x.ItemNumber.ToUpper() == item.ItemNumber.ToUpper());

                    if (i == null)
                    {
                        i = new InventoryItem()
                        {
                            Description = item.ItemDescription,
                            ItemNumber = item.ItemNumber
                        };
                        await ctx.CreateInventoryItem(i).ConfigureAwait(false);
                        BaseDataModel.Instance.InventoryCache.AddItem(i);
                    }
                }
            }
        }

        private async Task<OpeningStock> CreateOpeningStock(OpeningStock EDops)
        {
            using (var ctx = new OpeningStockService())
            {
                return await ctx.CreateOpeningStock(EDops).ConfigureAwait(false);
            }
        }
        private async Task<PurchaseOrders> CreatePurchaseOrders(PurchaseOrders EDpo)
        {
            using (var ctx = new PurchaseOrdersService())
            {
                return await ctx.CreatePurchaseOrders(EDpo).ConfigureAwait(false);
            }
        }

        private async Task<Sales> CreateSales(Sales EDsale)
        {
            using (var ctx = new SalesService())
            {
                return await ctx.CreateSales(EDsale).ConfigureAwait(false);
            }
        }

        private async Task DeleteEntryData(EntryData olded)
        {
            using (var ctx = new EntryDataService())
            {
                await ctx.DeleteEntryData(olded.EntryDataId).ConfigureAwait(false);
            }
        }

        private async Task ClearEntryDataDetails(EntryData olded)
        {
            using (var ctx = new EntryDataDetailsService())
            {
                foreach (var itm in olded.EntryDataDetails.ToList())
                {
                    await ctx.DeleteEntryDataDetails(itm.EntryDataDetailsId.ToString()).ConfigureAwait(false);
                }
            }
        }
    }
}