using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace
{
    public class CSVDataFileActions
    {
        public static Dictionary<string, Func<DataFile, Task<bool>>> Actions = new Dictionary<string, Func<DataFile, Task<bool>>>()
        {
            {FileTypeManager.EntryTypes.Rider, (dataFile) => new RiderImporter().Process(dataFile)},
            {FileTypeManager.EntryTypes.ExpiredEntries, (dataFile) => new ExpiredEntriesImporter().Process(dataFile)},
            {FileTypeManager.EntryTypes.CancelledEntries, (dataFile) => new CancelledEntriesImporter().Process(dataFile)},
            {FileTypeManager.EntryTypes.ShipmentInvoice,async (dataFile) => await new CSVShipmentImporter().ProcessAsync(dataFile).ConfigureAwait(false)},
            {FileTypeManager.EntryTypes.Po, async (dataFile) => await new EntryDataProcessor().Process(dataFile).ConfigureAwait(false)},
            {FileTypeManager.EntryTypes.Dis, async (dataFile) => await new EntryDataProcessor().Process(dataFile).ConfigureAwait(false)},
            {FileTypeManager.EntryTypes.Sales, async (dataFile) => await new EntryDataProcessor().Process(dataFile).ConfigureAwait(false)},
            {FileTypeManager.EntryTypes.Adj, async (dataFile) => await new EntryDataProcessor().Process(dataFile).ConfigureAwait(false)},
            {FileTypeManager.EntryTypes.Ops, async (dataFile) => await new EntryDataProcessor().Process(dataFile).ConfigureAwait(false)},

            {FileTypeManager.EntryTypes.ItemHistory, (dataFile) => new SaveItemHistory().Process(dataFile)},

        };
    }
}