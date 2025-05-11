using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using WaterNut.Business.Services.Utils;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace WaterNut.DataSpace
{
    public class NewEntryDataProcessorNoSave : INewEntryDataProcessor
    {
        

        private Dictionary<string, IEntryDataCreator> entryDataCreators = new Dictionary<string, IEntryDataCreator>()
        {
            {FileTypeManager.EntryTypes.Sales, new SalesCreator()},
            {FileTypeManager.EntryTypes.Inv, new InvoiceCreator()},
            {FileTypeManager.EntryTypes.Po, new POCreator()},
            {FileTypeManager.EntryTypes.ShipmentInvoice, new POCreator()},
            {FileTypeManager.EntryTypes.Ops, new OPSCreator()},
            {FileTypeManager.EntryTypes.Adj , new ADJCreator(FileTypeManager.EntryTypes.Adj)},
            {FileTypeManager.EntryTypes.Dis ,new ADJCreator(FileTypeManager.EntryTypes.Dis) },
            {FileTypeManager.EntryTypes.Rcon , new ADJCreator(FileTypeManager.EntryTypes.Rcon)},

        };

      

        public Task<EntryData> Execute(FileTypes fileType, List<AsycudaDocumentSet> docSet,
                                       RawEntryDataValue rawEntryData)
        {
            var applicationSettingsId = rawEntryData.EntryData.ApplicationSettingsId;
            var entryDataId = rawEntryData.EntryData.EntryDataId;

            var entryData = entryDataCreators[fileType.FileImporterInfos.EntryType]
                .Create(docSet, rawEntryData, applicationSettingsId, entryDataId);

            return Task.FromResult<EntryData>(entryData);
        }
    }
}