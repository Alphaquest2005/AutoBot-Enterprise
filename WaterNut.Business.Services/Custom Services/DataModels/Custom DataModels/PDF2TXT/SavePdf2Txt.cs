using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

namespace WaterNut.DataSpace
{
    public class SavePDFModel
    {
        private static readonly SavePDFModel instance;

        static SavePDFModel()
        {
            instance = new SavePDFModel();
        }

        public static SavePDFModel Instance
        {
            get { return instance; }
        }

        public async Task ProcessDroppedFile(string fileName, string fileType, AsycudaDocumentSet docSet, bool overWriteExisting)
        {

            //get the text
            var wStr = Core.Common.PDF2TXT.Pdf2Txt.Instance.ExtractTextFromPdf(fileName);

            var elst = await GetEntryData(wStr).ConfigureAwait(false);

            await ImportInventory(elst).ConfigureAwait(false);

           var flst = FixExistingEntryData(elst);
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

        private IEnumerable<EntryData> FixExistingEntryData(List<EntryData> elst)
        {
            var exceptions = new ConcurrentQueue<Exception>();
            Parallel.ForEach(elst, itm =>
            {
                try
                {
                    using (var ctx = new EntryDataService())
                    {
                        if (ctx.GetEntryDataByKey(itm.EntryDataId) != null)
                        {
                            itm = null;
                        }
                    }
                }
                catch (Exception ex)
                {

                    exceptions.Enqueue(ex);
                }
            });
            if (exceptions.Count > 0) throw new AggregateException(exceptions);
            return elst.Where(x => x != null);
        }

        private async Task ImportInventory(IEnumerable<EntryData> elst)
        {
            var lst =
                elst.SelectMany(
                    x =>
                        x.EntryDataDetails.Select(
                            y =>
                                new InventoryItem(true)
                                {
                                    ItemNumber = y.ItemNumber,
                                    Description = y.ItemDescription,
                                    TrackingState = TrackingState.Added
                                }));
            var exceptions = new ConcurrentQueue<Exception>();
            lst.AsParallel(new ParallelLinqOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }).ForAll(itm =>
            {
                try
                {

                    if (BaseDataModel.Instance.GetInventoryItem(x => x.ItemNumber == itm.ItemNumber) == null)
                        using (var ctx = new InventoryItemService())
                        {
                            ctx.UpdateInventoryItem(itm).Wait();
                        }
                    
                }
                catch (Exception ex)
                {

                    exceptions.Enqueue(ex);
                }
            });
            if (exceptions.Count > 0) throw new AggregateException(exceptions);
        }



        private async Task<List<EntryData>> GetEntryData(string wStr)
        {
            var res = new List<EntryData>();

            var entryData = new EntryData();

            var container_StatusPat = @"COST U LESS, INC\.\n*\*\*.COMMERCIAL INVOICE\*\*\*.*\nContainer :\s(?<ContainerNo>\d{3}-\d{4}-\d{6}).Status : (?<Status>\w*)\n";
            var bookingPat = @"Booking #:\n(?<BookingNo>[\w\-]*)";



            //get Regex Groups
            var entryDetailspat =
                @"(?<ItemNumber>\d{5,})\s(?<ItemDescription>[\w,<,\s,\&,\%]{1,})\s(?<Quantity>\d{1,4})\s(?<CS>\d+)\s(?<Cost>[\d,\,]+\.\d{2})\s(?<ExtCost>[\d,\,]+\.\d{2})\s(?<IF>[\d,\,]+\.\d{2})?\s?(?<ExtCIF>[\d,\,]+\.\d{2})\s(?<OriginCountry>\w{2,})\n(?<ExtItemDescription>[\w,\<,\s,\-\./]{1,})?\n?BRB.+HTS.+(?<TariffCode>\d{10})\s?%?\n?Duty\s?%\s?\:\s(?<DutyPercent>\d{1,3}\.\d{2})\s\%?\s?.+\:\s(?<DutyAmt>\d{1,3}\.\d{2})";

            var entryDetailsRegx = new Regex(entryDetailspat, RegexOptions.Compiled);

           

            foreach (Match m in entryDetailsRegx.Matches(wStr))
            {
                var ed = new EntryDataDetails(true) { TrackingState = TrackingState.Added };
                ed.ItemNumber = m.Groups["ItemNumber"].Value;
                ed.ItemDescription = m.Groups["ItemDescription"].Value;
                ed.Cost = Convert.ToDouble(m.Groups["Cost"].Value);
                ed.LineNumber = m.Index;
                ed.Quantity = Convert.ToDouble(m.Groups["Quantity"].Value);
                ed.Units = m.Groups["Unit"].Value;
            }

            return res;
        }
    }
}
