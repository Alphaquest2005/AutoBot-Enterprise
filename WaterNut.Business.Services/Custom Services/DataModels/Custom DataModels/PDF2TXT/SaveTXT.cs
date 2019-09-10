using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;
using InventoryDS.Business.Entities;
using InventoryDS.Business.Services;
using TrackableEntities;
using TrackableEntities.EF6;
using AsycudaDocumentSetEntryData = EntryDataDS.Business.Entities.AsycudaDocumentSetEntryData;

namespace WaterNut.DataSpace
{
    public class SaveTXT
    {
         private static readonly SaveTXT instance;
         private const string LineChkPat = @"HTS";
        private const string CasesTotalPat = @"CASES TOTAL\s*(?<CasesTotal>[\d\,\.]*)";
        private const string WeightTotalsPat = @"WEIGHT TOTAL\s*(?<TotalLBS>[\d\,\.]*)\s*lbs\s*(?<TotalKGS>[\d\,\.]*)\s*kg";
        private const string entryDetailspat = @"\n(?<ItemNumber>\d{5,})\s*(?<ItemDescription>[\<,\d,\w,\s,\-\./,\"",\#,.,\&,\',\%]{1,32})<?\s*(?<Quantity>[\d,\,\.]+)\s*(?<CS>\d+)\s*(?<Cost>[\d,\,]+\.\d{2})\s*(?<ExtCost>[\d,\,]+\.\d{2})\s*(?<IF>[\d,\,]+\.\d{2})?\s*(?<ExtCIF>[\d,\,,\.]+\.\d{2})\s*(?<OriginCountry>\w{3})*\s*(?<ExtItemDescription>[\<,\d,\s,\w,\-\./,\"",\#,\&,\',\%]{1,28})?<?\r\n\r\n\s*\w{3}\s*HTS\s*(?<TariffCode>\d{10})?\s*Duty\s*%\s*\:\s*(?<DutyPercent>\d{1,3}\.\d{2})\s*\%\s*Duty Amt :\s*(?<DutyAmt>[\d,\,]+\.\d{2})|HTS";
        const string container_StatusPat = @"Container :\s*(?<ContainerNo>\d{3}-\d{4}-\d{6}).*Status\s*:\s*(?<Status>\w*)";
        private const string containerDatePat = @"\*\*\*COMMERCIAL INVOICE\*\*\*\s*(?<ContainerDate>\d{2}-\w{3}-\d{2})\s*Page:";
        private const string InvoiceTotalPat = @"INVOICE TOTAL\s*\$(?<InvoiceTotal>[\d\,\.]*)\s(?<CurrencyCode>\w{3})";
        private const string palletTotalPat = @"PALLET TOTAL\s*(?<PalletTotal>[\d\,\.]*)";
        private const string containerIdentityPat = @" Del. Date :\s*\w*\s(?<DeliveryDate>\d{2}/\d{2}/\d{4})\s*Container :\s*(?<ContainerIdentity>\w{4}-\d{7})";

        private const string cubicMeasurementsPat = @"CUBE\s*(?<CubicFeet>[\d\,\.]*)\s*ft3\s*(?<CubicMeters>[\d\,\.]*)\s*m3";

        private const string shipDateSealPat =
            @"Ship Date :\s*\w*\s(?<ShipDate>\d{2}/\d{2}/\d{4})\s*Seal :\s*(?<Seal>\w*)";


        List<InventoryItem> itmList = new List<InventoryItem>();

        static SaveTXT()
        {
            instance = new SaveTXT();
        }

        public static SaveTXT Instance
        {
            get { return instance; }
        }

        public async Task ProcessDroppedFile(string fileName, string fileType, AsycudaDocumentSet docSet,
            bool overWriteExisting)
        {
            try
            {
                //get the text
                var wStr = ExtractTextFromFile(fileName);

                var container = await ConfigureContainer(wStr, docSet).ConfigureAwait(false);

                var elst = await GetEntryData(wStr, docSet, container).ConfigureAwait(false);

                await ConfigureDocSet(wStr, docSet).ConfigureAwait(false);

                await ImportInventory(elst).ConfigureAwait(false);

                var flst = FixExistingEntryData(elst);

                await SaveEntryData(flst).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }

        }

        private async Task<Container> ConfigureContainer(string wStr, AsycudaDocumentSet docSet)
        {
            try
            {


                var cnt = new Container() {TrackingState = TrackingState.Added};
                var invoiceTotalMat = GetMatchFromPattern(wStr, InvoiceTotalPat);
                var palletTotalMat = GetMatchFromPattern(wStr, palletTotalPat);
                var containerIdentityMat = GetMatchFromPattern(wStr, containerIdentityPat);
                var shipDateSealMat = GetMatchFromPattern(wStr, shipDateSealPat);
                var WeightTotalsMat = GetMatchFromPattern(wStr, WeightTotalsPat);
                var cubicPat = GetMatchFromPattern(wStr, cubicMeasurementsPat);


                if (containerIdentityPat != null)
                {
                    var cid = containerIdentityMat.Groups["ContainerIdentity"].Value;
                    using (var ctx = new ContainerService())
                    {
                        var res =
                            await
                                ctx.GetContainersByExpression($"Container_Identity == \"{cid}\"",
                                    new List<string>()
                                    {
                                        "ContainerAsycudaDocumentSets"
                                    })
                                    .ConfigureAwait(false);
                        if (res.FirstOrDefault() != null)
                        {
                            cnt = res.FirstOrDefault();
                            cnt.StartTracking();
                        }
                    }
                    cnt.Container_identity = cid;
                    cnt.DeliveryDate = Convert.ToDateTime(containerIdentityMat.Groups["DeliveryDate"].Value);
                }


                if (invoiceTotalMat != null)
                    cnt.TotalValue = Convert.ToDouble(invoiceTotalMat.Groups["InvoiceTotal"].Value);

                if (palletTotalPat != null)
                {
                    cnt.Packages_number = palletTotalMat.Groups["PalletTotal"].Value;
                    using (var ctx = new PackageTypesService())
                    {
                        var pklst =
                            await
                                ctx.GetPackageTypesByExpression("PackageDescription == \"Pallet\"")
                                    .ConfigureAwait(false);
                        cnt.Packages_type = pklst.FirstOrDefault().PackageType;
                    }

                }
                if (shipDateSealMat != null)
                {
                    cnt.ShipDate = Convert.ToDateTime(shipDateSealMat.Groups["ShipDate"].Value);
                    cnt.Seal = shipDateSealMat.Groups["Seal"].Value;
                }
                if (WeightTotalsMat != null)
                {
                    cnt.Gross_weight = Convert.ToDouble(WeightTotalsMat.Groups["TotalKGS"].Value);
                    cnt.Packages_weight = Convert.ToDouble(WeightTotalsMat.Groups["TotalKGS"].Value);
                }

                cnt.Goods_description = "Food Stuff";

                if (cubicPat != null)
                {
                    var m3 = Convert.ToDouble(cubicPat.Groups["CubicMeters"].Value);
                    cnt.Container_type = m3 > 34 ? "40RG" : "20RG";
                }

                if (!cnt.ContainerAsycudaDocumentSets.Any(x => x.AsycudaDocumentSetId == docSet.AsycudaDocumentSetId))
                    cnt.ContainerAsycudaDocumentSets.Add(new ContainerAsycudaDocumentSet()
                    {
                        AsycudaDocumentSetId = docSet.AsycudaDocumentSetId,
                        Container = cnt,
                        TrackingState = TrackingState.Added
                    });

                using (var ctx = new ContainerService())
                {
                    cnt = await ctx.UpdateContainer(cnt).ConfigureAwait(false);
                }

                return cnt;
            }
            catch (Exception)
            {

                throw;

            }
        }

        private async Task SaveEntryData(IEnumerable<EntryData> flst)
        {

            var exceptions = new ConcurrentQueue<Exception>();
            flst.AsParallel(new ParallelLinqOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }).ForAll(itm =>
            {
                try
                {
                    using (var ctx = new EntryDataDSContext())
                    {
                        ctx.ApplyChanges(itm);
                        ctx.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Enqueue(ex);
                }
            });
            if (exceptions.Count > 0) throw new AggregateException(exceptions);
        }

        private async Task ConfigureDocSet(string wStr, AsycudaDocumentSet docSet)
        {
            try
            {
                var casesTotalMat = GetMatchFromPattern(wStr, CasesTotalPat);
                var weightTotalMat = GetMatchFromPattern(wStr, WeightTotalsPat);

                if (weightTotalMat != null)
                    docSet.TotalWeight = Convert.ToDouble(weightTotalMat.Groups["TotalKGS"].Value);

                if (casesTotalMat != null)
                    docSet.TotalPackages = int.Parse(casesTotalMat.Groups["CasesTotal"].Value, NumberStyles.AllowThousands);



                await DocumentDS.DataModels.BaseDataModel.Instance.SaveAsycudaDocumentSet(docSet).ConfigureAwait(false);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private Match GetMatchFromPattern(string wStr, string pattern)
        {
            try
            {


                var regX = new Regex(pattern, RegexOptions.Compiled);

                if (regX.IsMatch(wStr))
                {
                    return regX.Match(wStr);
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private MatchCollection GetAllMatchesFromPattern(string wStr, string pattern)
        {
            var regX = new Regex(pattern, RegexOptions.Compiled);
            return regX.Matches(wStr);
           
           
        }

        private string ExtractTextFromFile(string fileName)
        {
            try
            {
                return System.IO.File.ReadAllText(fileName);
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        private IEnumerable<EntryData> FixExistingEntryData(List<EntryData> elst)
        {
            var exceptions = new ConcurrentQueue<Exception>();
            Parallel.ForEach(elst, itm =>
            {
                try
                {
                    using (var ctx = new EntryDataDSContext())
                    {
                        var pi = ctx.EntryData.Find(itm.EntryDataId);
                        if (pi != null)
                        {
                            ctx.EntryData.Remove(pi);
                            ctx.SaveChanges();
                            ctx.Database.ExecuteSqlCommand(
                                $"Delete from EntryData_PurchaseOrders where EntryDataId = '{itm.EntryDataId}'");
                        }
                    }
                }
                catch (Exception ex)
                {

                    exceptions.Enqueue(ex);
                }
            });
            if (exceptions.Count > 0) throw new AggregateException(exceptions);
            return elst;//.Where(x => x != null)
        }

        private async Task ImportInventory(IEnumerable<EntryData> elst)
        {
            try
            {

                var exceptions = new ConcurrentQueue<Exception>();
                itmList.AsParallel(new ParallelLinqOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }).ForAll(itm =>
                {
                    try
                    {

                        if (BaseDataModel.Instance.GetInventoryItem(x => x.ItemNumber == itm.ItemNumber) == null)
                        {
                            itm.TrackingState = TrackingState.Added;
                            using (var ctx = new InventoryItemService())
                            {
                                ctx.UpdateInventoryItem(itm).Wait();
                            }
                           
                        }
                        else
                        {
                            itm.TrackingState = TrackingState.Modified;
                            using (var ctx = new InventoryItemService())
                            {
                                ctx.UpdateInventoryItem(itm).Wait();
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        exceptions.Enqueue(ex);
                    }
                });
                if (exceptions.Count > 0) throw new AggregateException(exceptions);
            }
            catch (Exception)
            {

                throw;
            }
        }



        private async Task<List<EntryData>> GetEntryData(string wStr, AsycudaDocumentSet docSet, Container container)
        {
            try
            {

                //var bookingPat = @"Booking #:  (?<BookingNo>[\w\-]*)";
                var res = new List<EntryData>();

                var containerMatch = GetMatchFromPattern(wStr, container_StatusPat);
                var containerDateMat = GetMatchFromPattern(wStr, containerDatePat);
                var LineTotalMat = GetAllMatchesFromPattern(wStr, LineChkPat);

                var entryData = new PurchaseOrders(true) {TrackingState = TrackingState.Added};

                entryData.AsycudaDocumentSets.Add(new AsycudaDocumentSetEntryData(true)
                {
                    AsycudaDocumentSetId = docSet.AsycudaDocumentSetId,
                    EntryDataId = entryData.EntryDataId,
                    TrackingState = TrackingState.Added
                });


                if (containerMatch != null)
                {
                    entryData.EntryDataId = containerMatch.Groups["ContainerNo"].Value;
                    entryData.PONumber = containerMatch.Groups["ContainerNo"].Value;
                }
                if (containerDateMat != null)
                    entryData.EntryDataDate = Convert.ToDateTime(containerDateMat.Groups["ContainerDate"].Value);

                if (LineTotalMat != null)
                    entryData.ImportedLines = LineTotalMat.Count;

                entryData.InvoiceTotal = container.TotalValue;
                entryData.ContainerEntryData.Add(new ContainerEntryData()
                {
                    Container_Id = container.Container_Id,
                    EntryDataId = entryData.PONumber,
                    TrackingState = TrackingState.Added
                });


                //get Regex Groups

                var entryDetailsRegx = new Regex(entryDetailspat, RegexOptions.Compiled);
                //var lineChkMat = GetAllMatchesFromPattern(wStr, LineChkPat);

                var lcnt = 0;
                //var matches = entryDetailsRegx.Matches(wStr);
                //for (int i = 0; i < lineChkMat.Count; i++)
                //{

                //}
                foreach (Match m in entryDetailsRegx.Matches(wStr))
                {
                    var ed = new EntryDataDetails(true)
                    {
                        EntryDataId = entryData.EntryDataId,
                        TrackingState = TrackingState.Added
                    };
                    lcnt += 1;
                    if (string.IsNullOrEmpty(m.Groups["ItemNumber"].Value))
                    {
                        // missing line create blank
                        ed.ItemNumber = "Import Error";
                        ed.ItemDescription = "This line was not imported please manually create item.";
                        ed.Cost = 0;
                        ed.LineNumber = lcnt;
                        ed.Quantity = 0;
                        ed.Units = "";
                        entryData.EntryDataDetails.Add(ed);

                    }
                    else
                    {
                        ed.ItemNumber = m.Groups["ItemNumber"].Value;
                        ed.ItemDescription = m.Groups["ItemDescription"].Value + " " +
                                             m.Groups["ExtItemDescription"].Value;
                        ed.Cost = Convert.ToDouble(m.Groups["Cost"].Value);
                        ed.LineNumber = lcnt;
                        ed.Quantity = Convert.ToDouble(m.Groups["Quantity"].Value);
                        ed.Units = m.Groups["Unit"].Value;
                        entryData.EntryDataDetails.Add(ed);
                    }
                    
                    itmList.Add(new InventoryItem(true)
                    {
                        ItemNumber = ed.ItemNumber,
                        Description = ed.ItemDescription,
                        TariffCode =
                            string.IsNullOrEmpty(m.Groups["TariffCode"].Value)
                                ? null
                                : m.Groups["TariffCode"].Value.Substring(0, 8)
                    });
                }

                res.Add(entryData);


                return res;
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
    
}
