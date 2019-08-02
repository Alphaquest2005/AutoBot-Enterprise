using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Common.CSV;
using Core.Common.Utils;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;
using InventoryDS.Business.Entities;
using InventoryDS.Business.Services;
using MoreLinq;
using TrackableEntities;
using AsycudaDocumentSetEntryData = EntryDataDS.Business.Entities.AsycudaDocumentSetEntryData;


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

        public async Task<bool> ExtractEntryData(string fileType, string[] lines, string[] headings, string csvType,
            AsycudaDocumentSet docSet, bool overWriteExisting, int? emailId, int? fileTypeId)
        {
            try
            {


                if (docSet == null)
                {
                    throw new ApplicationException("Please select Document Set before proceding!");

                }

                var mapping = new Dictionary<string, int>();
                GetMappings(mapping, headings);
                var eslst = GetCSVDataSummayList(lines, mapping, headings);

                if (eslst == null) return true;


                if (csvType == "QB9")
                {
                    foreach (var item in eslst)
                    {
                        item.ItemNumber = item.ItemNumber.Split(':').Last();
                    }
                }

                await ImportInventory(eslst, docSet).ConfigureAwait(false);

                if (await ImportEntryData(fileType, eslst, docSet, overWriteExisting, emailId, fileTypeId)
                    .ConfigureAwait(false)) return true;
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }



        private async Task<bool> ImportEntryData(string fileType, List<CSVDataSummary> eslst, AsycudaDocumentSet docSet,
            bool overWriteExisting, int? emailId, int? fileTypeId)
        {
            try
            {

           
            var ed = (from es in eslst
                     group es by new { es.EntryDataId, es.EntryDataDate, es.CustomerName, es.SupplierCode, es.Currency }
                         into g
                         //let supplier = EntryDataDS.DataModels.BaseDataModel.Instance.SearchSuppliers(new List<string>()
                         //{
                         //    string.Format("SupplierCode == \"{0}\"",g.Key.SupplierCode)
                         //})

                         //where supplier != null
                         select new
                         {
                             EntryData =
                                 new
                                 {
                                     EntryDataId = g.Key.EntryDataId,
                                     EntryDataDate = g.Key.EntryDataDate,
                                     AsycudaDocumentSetId = docSet.AsycudaDocumentSetId,
                                     ApplicationSettingsId = docSet.ApplicationSettingsId,
                                     CustomerName = g.Key.CustomerName,
                                     Tax = g.Sum(x => x.Tax),
                                     Supplier = g.Key.SupplierCode,
                                     Currency = g.Key.Currency,
                                     EmailId = emailId,
                                     FileTypeId = fileTypeId,
                                     
                                 },
                             EntryDataDetails = g.Select(x => new  EntryDataDetails()
                             {
                                 EntryDataId = x.EntryDataId,
                                 ItemNumber = x.ItemNumber.ToUpper(),
                                 ItemDescription = x.ItemDescription,
                                 Cost = Convert.ToDouble(x.Cost),
                                 Quantity = Convert.ToDouble(x.Quantity),
                                 Units = x.Units,
                                 Freight = x.Freight,
                                 Weight = x.Weight,
                                 InternalFreight = x.InternalFreight,
                                 InvoiceQty = x.InvoiceQuantity,
                                ReceivedQty = x.ReceivedQuantity,
                                 TaxAmount = x.Tax,
                                 CNumber = x.CNumber,
                                 PreviousInvoiceNumber = x.PreviousInvoiceNumber,
                                 Comment = x.Comment,
                                 EffectiveDate = x.EffectiveDate,

                             }),
                             f = g.Select(x => new
                             {
                                 TotalWeight = x.TotalWeight,
                                 TotalFreight = x.TotalFreight,
                                 TotalInternalFreight = x.TotalInternalFreight,
                                
                             }),
                             InventoryItems = g.DistinctBy(x => new {x.ItemNumber, x.ItemAlias}).Select(x => new {x.ItemNumber,x.ItemAlias})
                         }).ToList();

            if (ed == null) return true;


            List<EntryData> eLst = null;

                foreach (var item in ed)
                {
                    if (!item.EntryDataDetails.Any()) throw new ApplicationException(item.EntryData.EntryDataId + " has no details");

                    List<EntryDataDetails> details = new List<EntryDataDetails>();
                    
                    // check Existing items
                    var olded = await GetEntryData(item.EntryData.EntryDataId, item.EntryData.EntryDataDate, item.EntryData.ApplicationSettingsId).ConfigureAwait(false);

                    if (olded != null)
                    {
                        if (overWriteExisting)
                        {
                            await ClearEntryDataDetails(olded).ConfigureAwait(false);
                            await DeleteEntryData(olded).ConfigureAwait(false);
                            details = item.EntryDataDetails.ToList();
                        }
                        else
                        {


                            var l = 0;
                            foreach (var nEd in item.EntryDataDetails.ToList())
                            {
                                l += 1;

                                var oEd = olded.EntryDataDetails.FirstOrDefault(x =>
                                    x.LineNumber == l && x.ItemNumber == nEd.ItemNumber);

                                if (oEd == null) continue;
                                if (Math.Abs(nEd.Quantity - oEd.Quantity) < .0001 &&
                                    Math.Abs(nEd.Cost - oEd.Cost) < .0001)
                                {
                                    
                                    continue;
                                }
                                details.Add(nEd);
                                new EntryDataDetailsService().DeleteEntryDataDetails(oEd.EntryDataDetailsId.ToString())
                                    .Wait();

                            }
                        }
                    }
                    if (olded == null) details = item.EntryDataDetails.ToList();
                    if (overWriteExisting || olded == null)
                    {

                        

                        switch (fileType)
                        {
                            case "Sales":

                                var EDsale = new Sales(true)
                                {
                                    ApplicationSettingsId = item.EntryData.ApplicationSettingsId,
                                    EntryDataId = item.EntryData.EntryDataId,
                                    EntryDataDate = item.EntryData.EntryDataDate,
                                    INVNumber = item.EntryData.EntryDataId,
                                    CustomerName = item.EntryData.CustomerName,
                                    EmailId = item.EntryData.EmailId,
                                    FileTypeId = item.EntryData.FileTypeId,
                                    Currency = string.IsNullOrEmpty(item.EntryData.Currency)
                                        ? null
                                        : item.EntryData.Currency,
                                    TrackingState = TrackingState.Added
                                };
                                EDsale.AsycudaDocumentSets.Add(new AsycudaDocumentSetEntryData(true)
                                {
                                    AsycudaDocumentSetId = item.EntryData.AsycudaDocumentSetId,
                                    EntryDataId = item.EntryData.EntryDataId,
                                    TrackingState = TrackingState.Added
                                });
                                await CreateSales(EDsale).ConfigureAwait(false);
                                break;
                            case "PO":
                                var EDpo = new PurchaseOrders(true)
                                {
                                    ApplicationSettingsId = item.EntryData.ApplicationSettingsId,
                                    EntryDataId = item.EntryData.EntryDataId,
                                    EntryDataDate = item.EntryData.EntryDataDate,
                                    PONumber = item.EntryData.EntryDataId,
                                    TrackingState = TrackingState.Added,
                                    TotalFreight = item.f.Sum(x => x.TotalFreight),
                                    TotalInternalFreight = item.f.Sum(x => x.TotalInternalFreight),
                                    TotalWeight = item.f.Sum(x => x.TotalWeight),
                                    EmailId = item.EntryData.EmailId,
                                    FileTypeId = item.EntryData.FileTypeId,
                                    Currency = string.IsNullOrEmpty(item.EntryData.Currency)
                                        ? null
                                        : item.EntryData.Currency,
                                };
                                EDpo.AsycudaDocumentSets.Add(new AsycudaDocumentSetEntryData(true)
                                {
                                    AsycudaDocumentSetId = item.EntryData.AsycudaDocumentSetId,
                                    EntryDataId = item.EntryData.EntryDataId,
                                    TrackingState = TrackingState.Added
                                });
                                await CreatePurchaseOrders(EDpo).ConfigureAwait(false);
                                break;
                            case "OPS":
                                var EDops = new OpeningStock(true)
                                {
                                    ApplicationSettingsId = item.EntryData.ApplicationSettingsId,
                                    EntryDataId = item.EntryData.EntryDataId,
                                    EntryDataDate = item.EntryData.EntryDataDate,
                                    OPSNumber = item.EntryData.EntryDataId,
                                    TrackingState = TrackingState.Added,
                                    TotalFreight = item.f.Sum(x => x.TotalFreight),
                                    TotalInternalFreight = item.f.Sum(x => x.TotalInternalFreight),
                                    TotalWeight = item.f.Sum(x => x.TotalWeight),
                                    EmailId = item.EntryData.EmailId,
                                    FileTypeId = item.EntryData.FileTypeId,
                                    Currency = string.IsNullOrEmpty(item.EntryData.Currency)
                                        ? null
                                        : item.EntryData.Currency,
                                };
                                EDops.AsycudaDocumentSets.Add(new AsycudaDocumentSetEntryData(true)
                                {
                                    AsycudaDocumentSetId = item.EntryData.AsycudaDocumentSetId,
                                    EntryDataId = item.EntryData.EntryDataId,
                                    TrackingState = TrackingState.Added
                                });
                                await CreateOpeningStock(EDops).ConfigureAwait(false);
                                break;
                            case "ADJ":
                                var EDadj = new Adjustments(true)
                                {
                                    ApplicationSettingsId = item.EntryData.ApplicationSettingsId,
                                    EntryDataId = item.EntryData.EntryDataId,
                                    EntryDataDate = item.EntryData.EntryDataDate,
                                    TrackingState = TrackingState.Added,
                                    TotalFreight = item.f.Sum(x => x.TotalFreight),
                                    TotalInternalFreight = item.f.Sum(x => x.TotalInternalFreight),
                                    TotalWeight = item.f.Sum(x => x.TotalWeight),
                                    EmailId = item.EntryData.EmailId,
                                    FileTypeId = item.EntryData.FileTypeId,
                                    Currency = string.IsNullOrEmpty(item.EntryData.Currency)
                                        ? null
                                        : item.EntryData.Currency,
                                    Type = "ADJ"
                                };
                                EDadj.AsycudaDocumentSets.Add(new AsycudaDocumentSetEntryData(true)
                                {
                                    AsycudaDocumentSetId = item.EntryData.AsycudaDocumentSetId,
                                    EntryDataId = item.EntryData.EntryDataId,
                                    TrackingState = TrackingState.Added
                                });
                                await CreateAdjustments(EDadj).ConfigureAwait(false);
                                break;
                            case "DIS":
                                var EDdis = new Adjustments(true)
                                {
                                    ApplicationSettingsId = item.EntryData.ApplicationSettingsId,
                                    EntryDataId = item.EntryData.EntryDataId,
                                    EntryDataDate = item.EntryData.EntryDataDate,
                                    TrackingState = TrackingState.Added,
                                    TotalFreight = item.f.Sum(x => x.TotalFreight),
                                    TotalInternalFreight = item.f.Sum(x => x.TotalInternalFreight),
                                    TotalWeight = item.f.Sum(x => x.TotalWeight),
                                    EmailId = item.EntryData.EmailId,
                                    FileTypeId = item.EntryData.FileTypeId,
                                    Currency = string.IsNullOrEmpty(item.EntryData.Currency)
                                        ? null
                                        : item.EntryData.Currency,
                                    Type = "DIS"
                                };
                                EDdis.AsycudaDocumentSets.Add(new AsycudaDocumentSetEntryData(true)
                                {
                                    AsycudaDocumentSetId = item.EntryData.AsycudaDocumentSetId,
                                    EntryDataId = item.EntryData.EntryDataId,
                                    TrackingState = TrackingState.Added
                                });
                                await CreateAdjustments(EDdis).ConfigureAwait(false);
                                break;
                            case "RCON":
                                var EDrcon = new Adjustments(true)
                                {
                                    ApplicationSettingsId = item.EntryData.ApplicationSettingsId,
                                    EntryDataId = item.EntryData.EntryDataId,
                                    EntryDataDate = item.EntryData.EntryDataDate,
                                    TrackingState = TrackingState.Added,
                                    TotalFreight = item.f.Sum(x => x.TotalFreight),
                                    TotalInternalFreight = item.f.Sum(x => x.TotalInternalFreight),
                                    TotalWeight = item.f.Sum(x => x.TotalWeight),
                                    EmailId = item.EntryData.EmailId,
                                    FileTypeId = item.EntryData.FileTypeId,
                                    Currency = string.IsNullOrEmpty(item.EntryData.Currency)
                                        ? null
                                        : item.EntryData.Currency,
                                    Type = "RCON"
                                };
                                EDrcon.AsycudaDocumentSets.Add(new AsycudaDocumentSetEntryData(true)
                                {
                                    AsycudaDocumentSetId = item.EntryData.AsycudaDocumentSetId,
                                    EntryDataId = item.EntryData.EntryDataId,
                                    TrackingState = TrackingState.Added
                                });
                                await CreateAdjustments(EDrcon).ConfigureAwait(false);
                                break;
                            default:
                                throw new ApplicationException("Unknown FileType");

                        }
                    }

                    

                    using (var ctx = new EntryDataDetailsService())
                    {
                        var lineNumber = 0;
                        foreach (var e in details)
                        {
                            lineNumber += 1;
                            await ctx.CreateEntryDataDetails(new EntryDataDetails(true)
                            {
                                EntryDataId = e.EntryDataId,
                                ItemNumber = e.ItemNumber.Truncate(20),
                                ItemDescription = e.ItemDescription,
                                Quantity = e.Quantity,
                                Cost = e.Cost,
                                Units = e.Units,
                                TrackingState = TrackingState.Added,
                                Freight = e.Freight,
                                Weight = e.Weight,
                                InternalFreight = e.InternalFreight,
                                ReceivedQty = e.ReceivedQty,
                                InvoiceQty = e.InvoiceQty,
                                CNumber = string.IsNullOrEmpty(e.CNumber) ? null : e.CNumber,
                                PreviousInvoiceNumber = string.IsNullOrEmpty(e.PreviousInvoiceNumber)? null : e.PreviousInvoiceNumber,
                                Comment = string.IsNullOrEmpty(e.Comment) ? null : e.Comment,
                                LineNumber = lineNumber,
                                EffectiveDate = e.EffectiveDate,
                                TaxAmount = e.TaxAmount,
                            }).ConfigureAwait(false);
                        }
                    }

                    using (var ctx = new InventoryDSContext(){StartTracking = true})
                    {
                        
                       foreach (var e in item.InventoryItems.Where(x => !string.IsNullOrEmpty(x.ItemAlias) && x.ItemAlias != x.ItemNumber).ToList())
                       {
                           var inventoryItem = ctx.InventoryItems
                               .Include("InventoryItemAlias")
                               .First(x => x.ApplicationSettingsId == item.EntryData.ApplicationSettingsId &&
                                           x.ItemNumber == e.ItemNumber);
                           if (inventoryItem == null) continue;
                           {
                               if (inventoryItem.InventoryItemAlias.FirstOrDefault(x => x.AliasName == e.ItemAlias) ==
                                   null)
                               {
                                   inventoryItem.InventoryItemAlias.Add(new InventoryItemAlia(true)
                                   {
                                       InventoryItemId = inventoryItem.Id,
                                       AliasName = e.ItemAlias.Truncate(20),

                                   });
                                    
                               }
                           }

                       }

                        ctx.SaveChanges();

                    }

                }




                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<Adjustments> CreateAdjustments(Adjustments eDadj)
        {
            using (var ctx = new AdjustmentsService())
            {
                return await ctx.CreateAdjustments(eDadj).ConfigureAwait(false);
            }
        }

        private async Task<EntryData> GetEntryData(string entryDataId, DateTime entryDateTime,
            int applicationSettingsId)
        {
            using (var ctx = new EntryDataService())
            {
                return
                    (await ctx.GetEntryDataByExpressionLst(new List<string>()
                    {
                        $"EntryDataId == \"{entryDataId}\"",
                       // $"EntryDataDate == \"{entryDateTime.ToString("yyyy-MMM-dd")}\"",
                        $"ApplicationSettingsId == \"{applicationSettingsId}\"",
                    }, new List<string>() {"AsycudaDocumentSets", "EntryDataDetails"}).ConfigureAwait(false)).FirstOrDefault();
                //eLst.FirstOrDefault(x => x.EntryDataId == item.e.EntryDataId && x.EntryDataDate != item.e.EntryDataDate);
            }
        }


        private void GetMappings(Dictionary<string, int> mapping, string[] headings)
        {
            for (var i = 0; i < headings.Count(); i++)
            {
                var h = headings[i].Trim().ToUpper();

                if (h == "") continue;

                if ("INVNO|Reciept #|NUM|Invoice #|Invoice#".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("EntryDataId", i);
                    continue;
                }

                if ("DATE|Invoice Date".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("EntryDataDate", i);
                    continue;
                }

                if ("ItemNumber|ITEM-#|Item Code|Product Code".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("ItemNumber", i);
                    continue;
                }

                if ("DESCRIPTION|MEMO|Item Description|ItemDescription|Description 1".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("ItemDescription", i);
                    continue;
                }

                if ("QUANTITY|QTY".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("Quantity", i);
                    continue;
                }

                if ("ItemAlias".ToUpper().Contains(h.ToUpper()))//Manufact. SKU|
                {
                    mapping.Add("ItemAlias", i);
                    continue;
                }

                if ("PRICE|COST".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("Cost", i);
                    continue;
                }

                if ("TotalCost".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("TotalCost", i);
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
                if ("TAX1|Tax".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("Tax", i);
                    continue;
                }

                if ("TariffCode".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("TariffCode", i);
                    continue;
                }

                if ("Freight".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("Freight", i);
                    continue;
                }
                if ("Weight".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("Weight", i);
                    continue;
                }
                if ("InternalFreight".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("InternalFreight", i);
                    continue;
                }

                //-------------------------
                if ("Cnumber".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("CNumber", i);
                    continue;
                }

                if ("Invoice Quantity|From Quantity".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("InvoiceQuantity", i);
                    continue;
                }

                if ("Received Quantity|To Quantity".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("ReceivedQuantity", i);
                    continue;
                }
               
                if ("Currency".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("Currency", i);
                    continue;
                }

                if ("Comment".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("Comment", i);
                    continue;
                }
                
                if ("PreviousInvoiceNumber".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("PreviousInvoiceNumber", i);
                    continue;
                }

                if ("EffectiveDate|Effective Date".ToUpper().Contains(h.ToUpper()))
                {
                    mapping.Add("EffectiveDate", i);
                    continue;
                }
            }
        }


        private List<CSVDataSummary> GetCSVDataSummayList(string[] lines, Dictionary<string, int> mapping,
            string[] headings)
        {
            int i = 0;
            try
            {
                var eslst = new List<CSVDataSummary>();

                for (i = 1; i < lines.Count(); i++)
                {

                    var d = GetCSVDataFromLine(lines[i], mapping, headings);
                    if (d != null)
                    {
                        eslst.Add(d);
                    }
                }
                return eslst;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }



        private CSVDataSummary GetCSVDataFromLine(string line, Dictionary<string, int> map, string[] headings)
        {
            var ImportActions = new Dictionary<string, Action<CSVDataSummary,Dictionary<string,int>,string[]>>()
            {
                {"EntryDataId", (c,mapping,splits) => c.EntryDataId = splits[mapping["EntryDataId"]] },
                {"EntryDataDate",(c,mapping,splits) => c.EntryDataDate = DateTime.Parse(string.IsNullOrEmpty(splits[mapping["EntryDataDate"]]) ? DateTime.MinValue.ToShortDateString() : splits[mapping["EntryDataDate"]].Replace("�", ""), CultureInfo.CurrentCulture)},
                {"ItemNumber",(c,mapping,splits) => c.ItemNumber = splits[mapping["ItemNumber"]]},
                {"ItemAlias",(c,mapping,splits) => c.ItemAlias = mapping.ContainsKey("ItemAlias") ? splits[mapping["ItemAlias"]] : ""},
                {"ItemDescription",(c,mapping,splits) => c.ItemDescription = splits[mapping["ItemDescription"]]},
                {"Cost", (c,mapping,splits) => c.Cost = !mapping.ContainsKey("Cost") ? 0 : Convert.ToSingle(string.IsNullOrEmpty(splits[mapping["Cost"]]) ? "0" : splits[mapping["Cost"]].Replace("$","").Replace("�", "").Trim())},
                {"Quantity", (c,mapping,splits) => c.Quantity = Convert.ToSingle(splits[mapping["Quantity"]].Replace("�", ""))},
                {"Units", (c,mapping,splits) => c.Units = mapping.ContainsKey("Units") ? splits[mapping["Units"]] : ""},
                {"CustomerName", (c,mapping,splits) => c.CustomerName = mapping.ContainsKey("CustomerName") ? splits[mapping["CustomerName"]] : ""},
                {"Tax", (c,mapping,splits) => c.Tax = Convert.ToSingle(mapping.ContainsKey("Tax") ? splits[mapping["Tax"]] : "0")},
                {"TariffCode", (c,mapping,splits) => c.TariffCode = mapping.ContainsKey("TariffCode") ? splits[mapping["TariffCode"]] : ""},
                {"SupplierCode", (c,mapping,splits) => c.SupplierCode = mapping.ContainsKey("SupplierCode") ? splits[mapping["SupplierCode"]] : ""},
                {"Freight", (c,mapping,splits) => c.Freight = Convert.ToSingle(mapping.ContainsKey("Freight") && !string.IsNullOrEmpty(splits[mapping["Freight"]]) ? splits[mapping["Freight"]] : "0")},
                {"Weight", (c,mapping,splits) => c.Weight = Convert.ToSingle(mapping.ContainsKey("Weight") && !string.IsNullOrEmpty(splits[mapping["Weight"]]) ? splits[mapping["Weight"]] : "0")},
                {"InternalFreight", (c,mapping,splits) => c.InternalFreight = Convert.ToSingle(mapping.ContainsKey("InternalFreight") && !string.IsNullOrEmpty(splits[mapping["InternalFreight"]]) ? splits[mapping["InternalFreight"]] : "0")},
                {"TotalFreight", (c,mapping,splits) => c.TotalFreight = Convert.ToSingle(mapping.ContainsKey("TotalFreight") && !string.IsNullOrEmpty(splits[mapping["TotalFreight"]]) ? splits[mapping["TotalFreight"]] : "0")},
                {"TotalWeight", (c,mapping,splits) => c.TotalWeight = Convert.ToSingle(mapping.ContainsKey("TotalWeight") && !string.IsNullOrEmpty(splits[mapping["TotalWeight"]]) ? splits[mapping["TotalWeight"]] : "0")},
                {"TotalInternalFreight", (c,mapping,splits) => c.TotalInternalFreight = Convert.ToSingle(mapping.ContainsKey("TotalInternalFreight") && !string.IsNullOrEmpty(splits[mapping["TotalInternalFreight"]]) ? splits[mapping["TotalInternalFreight"]] : "0")},
                {"CNumber", (c,mapping,splits) => c.CNumber = mapping.ContainsKey("CNumber") ? splits[mapping["CNumber"]] : ""},
                {"InvoiceQuantity", (c,mapping,splits) => c.InvoiceQuantity = mapping.ContainsKey("InvoiceQuantity") ? Convert.ToSingle(splits[mapping["InvoiceQuantity"]]) : 0},
                {"ReceivedQuantity", (c,mapping,splits) => c.ReceivedQuantity = mapping.ContainsKey("ReceivedQuantity") ? Convert.ToSingle(splits[mapping["ReceivedQuantity"]]) : 0},
                {"Currency", (c,mapping,splits) => c.Currency = mapping.ContainsKey("Currency") ? splits[mapping["Currency"]] : ""},
                {"Comment", (c,mapping,splits) => c.Comment = mapping.ContainsKey("Comment") ? splits[mapping["Comment"]] : ""},
                {"PreviousInvoiceNumber", (c,mapping,splits) => c.PreviousInvoiceNumber = mapping.ContainsKey("PreviousInvoiceNumber") ? splits[mapping["PreviousInvoiceNumber"]] : ""},
                {"EffectiveDate", (c,mapping,splits) => c.EffectiveDate = mapping.ContainsKey("EffectiveDate")  && !string.IsNullOrEmpty(splits[mapping["EffectiveDate"]]) ? DateTime.Parse(splits[mapping["EffectiveDate"]], CultureInfo.CurrentCulture)  : (DateTime?) null},
                {"TotalCost", (c,mapping,splits) => c.Cost = Convert.ToSingle(splits[mapping["TotalCost"]].Replace("$", "")) /c.Quantity},
                //{"", (c,mapping,splits) => c.},
            };

            try
            {
                if (string.IsNullOrEmpty(line)) return null;
                var splits = line.Replace("�", "").CsvSplit().Select(x => x.Trim()).ToArray();
                if (!map.Keys.Contains("EntryDataId"))
                    throw new ApplicationException("Invoice# not Mapped");
                if (!map.Keys.Contains("ItemNumber"))
                    throw new ApplicationException("ItemNumber not Mapped");
                if (splits[map["EntryDataId"]] != "" && splits[map["ItemNumber"]] != "")
                {
                    var res = new CSVDataSummary();
                    foreach (var key in map.Keys)
                    {
                        try
                        {
                            ImportActions[key].Invoke(res, map, splits);
                        }
                        catch (Exception e)
                        {
                            var message = $"Could not Import '{headings[map[key]]}' from Line:'{line}'. Error:{e.Message}";
                            Console.WriteLine(e);
                            throw new ApplicationException(message);
                        }

                    }

                    return res;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        class CSVDataSummary
        {
            
            public string EntryDataId { get; set; }
            public DateTime EntryDataDate { get; set; }
            public string ItemNumber { get; set; }
            public string ItemAlias { get; set; }
            public string ItemDescription { get; set; }
            public Single Quantity { get; set; }
            public Single Cost { get; set; }
            public string Units { get; set; }
            public string CustomerName { get; set; }
            public Single Tax { get; set; }
            public string TariffCode { get; set; }

            public string SupplierCode { get; set; }

            public double? Freight { get; set; }

            public double? Weight { get; set; }

            public double? InternalFreight { get; set; }
            public double? TotalFreight { get; set; }

            public double? TotalWeight { get; set; }

            public double? TotalInternalFreight { get; set; }
            public string CNumber { get; set; }
            public float InvoiceQuantity { get; set; }
            public float ReceivedQuantity { get; set; }
            public string Currency { get; set; }
            public string Comment { get; set; }

            public string PreviousInvoiceNumber { get; set; }
            public DateTime? EffectiveDate { get; set; }
        }

        private async Task ImportInventory(List<CSVDataSummary> eslst, AsycudaDocumentSet docSet)
        {
            var itmlst = from i in eslst
                         group i by i.ItemNumber.ToUpper()
                             into g
                             select new { ItemNumber = g.Key, g.FirstOrDefault().ItemDescription, g.FirstOrDefault().TariffCode };

            using (var ctx = new InventoryItemService(){StartTracking = true})
            {
                foreach (var item in itmlst)
                {
                    var i = (await ctx.GetInventoryItemsByExpression($"ItemNumber == \"{item.ItemNumber}\" && ApplicationSettingsId == \"{docSet.ApplicationSettingsId}\"", null, true).ConfigureAwait(false)).FirstOrDefault();
                    if (i == null)
                    {
                        i = new InventoryItem(true)
                        {
                            ApplicationSettingsId = docSet.ApplicationSettingsId,
                            Description = item.ItemDescription,
                            ItemNumber = item.ItemNumber.Truncate(20),
                           TrackingState = TrackingState.Added
                        };
                        if (!string.IsNullOrEmpty(item.TariffCode)) i.TariffCode = item.TariffCode;
                        await ctx.CreateInventoryItem(i).ConfigureAwait(false);
                       
                    }
                    else
                    {
                        i.StartTracking();
                        i.Description = item.ItemDescription;
                        if (!string.IsNullOrEmpty(item.TariffCode)) i.TariffCode = item.TariffCode;
                        await ctx.UpdateInventoryItem(i).ConfigureAwait(false);
                       
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