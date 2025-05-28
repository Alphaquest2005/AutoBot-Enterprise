using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Added missing using
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using Serilog; // Added Serilog using
using Serilog.Context; // Added Serilog.Context using

namespace WaterNut.Business.Services.Importers.EntryData
{
    public class GetNewInventoryItems : IProcessor<InventoryDataItem>
    {
        private readonly FileTypes _fileType;
        private readonly List<AsycudaDocumentSet> _docSet;

        public GetNewInventoryItems(FileTypes fileType, List<AsycudaDocumentSet> docSet)
        {
            _fileType = fileType;
            _docSet = docSet;
        }

        // Revert signature to match IProcessor<InventoryDataItem> interface
        public async Task<Result<List<InventoryDataItem>>> Execute(List<InventoryDataItem> data, ILogger log)
        {
            // Added LogContext for logging within this method
            using (LogContext.PushProperty("GetNewInventoryItems", nameof(this.Execute)))
            {
                var inventorySource = InventorySourceFactory.GetInventorySource(_fileType);
                var newItems = data.Where(x => x.Item == null).Select(x => x.Data).ToList();
                // Synchronously wait for the async call result (potential blocking issue)
                // Pass Log.Logger as the ILogger argument
                var newInventoryItemFromData = await InventoryItemDataUtils.GetNewInventoryItemFromData(newItems, inventorySource, Log.Logger).ConfigureAwait(false);
                return new Result<List<InventoryDataItem>>(newInventoryItemFromData, true, "");
            }
        }

    }
}