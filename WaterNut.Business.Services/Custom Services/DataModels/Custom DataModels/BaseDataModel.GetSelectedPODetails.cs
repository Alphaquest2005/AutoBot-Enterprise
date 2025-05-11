using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using MoreLinq.Extensions;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    public Task<IEnumerable<EntryDataDetails>> GetSelectedPODetails(List<string> elst,
                                                                    int asycudaDocumentSetId)
    {
        try
        {
            var res = new List<EntryDataDetails>();
            if (!elst.Any()) return Task.FromResult<IEnumerable<EntryDataDetails>>(res);
            {
                using (var ctx = new EntryDataDSContext())
                {
                    foreach (var item in elst.Where(x => x != null))
                    {
                        var entryDataDetailses = ctx.EntryDataDetails
                            .Include(x => x.EntryDataDetailsEx)
                            .Include(x => x.InventoryItems)
                            .Include(x => x.InventoryItemEx)
                            .Include(x => x.EntryData.EntryDataTotals)
                            .Include(x => x.EntryData.EntryDataEx)
                            .Include(x => x.EntryData.DocumentType)
                            .Include(x => x.EntryData.Suppliers)
                            .Where(x => x.EntryDataId == item
                                        && ((x.EntryDataDetailsEx.AsycudaDocumentSetId == asycudaDocumentSetId &&
                                             x.EntryDataDetailsEx.SystemDocumentSets == null) ||
                                            (x.EntryDataDetailsEx.AsycudaDocumentSetId != asycudaDocumentSetId ||
                                             x.EntryDataDetailsEx.SystemDocumentSets != null))
                                        && x.EntryData.EntryDataEx != null
                            )
                            .ToList().DistinctBy(x => x.EntryDataDetailsId);

                        var res1 = entryDataDetailses.Where(x =>
                            Instance.CurrentApplicationSettings.AssessIM7 != true || Math.Abs(
                                x.EntryData.EntryDataEx.ExpectedTotal -
                                (x.EntryData.InvoiceTotal ?? x.EntryData.EntryDataEx
                                    .ExpectedTotal)) < 0.01);


                        res.AddRange(res1);
                    }
                }
            }

            return Task.FromResult<IEnumerable<EntryDataDetails>>(res.OrderBy(x => x.EntryDataDetailsId));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Task<IEnumerable<EntryDataDetails>> GetSelectedPODetails(List<int> lst)
    {
        try
        {
            var res = new ConcurrentDictionary<int, EntryDataDetails>();
            if (lst.Any())
                Parallel.ForEach(lst.Where(x => x != 0),
                    new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount}, item => //
                    {
                        using (var ctx = new EntryDataDSContext())
                        {
                            ctx.Database.CommandTimeout = 10;
                            var entryDataDetailses = ctx.EntryDataDetails
                                .Include(x => x.EntryDataDetailsEx)
                                .Include(x => x.InventoryItems)
                                .Include(x => x.InventoryItemEx)
                                .Where(x => x.EntryDataDetailsId == item
                                    //   && x.EntryData.EntryDataEx != null
                                )
                                .Where(x => Math.Abs((double) (x.EntryData.EntryDataEx.ExpectedTotal -
                                                               (x.EntryData.InvoiceTotal == null ||
                                                                x.EntryData.InvoiceTotal == 0
                                                                   ? x.EntryData.EntryDataEx.ExpectedTotal
                                                                   : x.EntryData.InvoiceTotal))) <
                                            0.01)
                                .First();

                            // had to do all this stupidity because ef not loading the warehouse info because its derived...smh

                            entryDataDetailses.EntryData = ctx.EntryData.OfType<PurchaseOrders>()
                                .Include(x => x.Suppliers)
                                .Include(x => x.EntryDataEx)
                                .Include(x => x.DocumentType)
                                .Include(x => x.EntryDataTotals)
                                .Include(x => x.WarehouseInfo)
                                .First(x => x.EntryData_Id == entryDataDetailses.EntryData_Id);


                            res.AddOrUpdate(item, entryDataDetailses, (i, details) => details);
                        }
                    });


            return Task.FromResult<IEnumerable<EntryDataDetails>>(res.Values.OrderBy(x =>
                    x.EntryDataDetailsId));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}