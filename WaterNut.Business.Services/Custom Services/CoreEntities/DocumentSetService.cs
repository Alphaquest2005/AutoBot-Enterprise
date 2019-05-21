using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading.Tasks;
using AllocationQS.Business.Services;
using Core.Common.Contracts;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using DocumentDS.Business.Services;
using Omu.ValueInjecter;


namespace CoreEntities.Business.Services
{
    [Export(typeof(IAsycudaSalesAllocationsExService))]
    [Export(typeof(IBusinessService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession,
                     ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class DocumentSetService : IDocumentSetService, IDisposable
    {
        public async Task DeleteDocuments(int docSetId)
        {
            await WaterNut.DataSpace.BaseDataModel.Instance.ClearAsycudaDocumentSet(docSetId).ConfigureAwait(false);
        }

        public async Task DeleteDocumentSet(int docSetId)
        {
            using (var ctx = new AsycudaDocumentSetService())
            {
                await ctx.DeleteAsycudaDocumentSet(docSetId.ToString()).ConfigureAwait(false);
            }
        }

        public async Task ImportDocuments(int asycudaDocumentSetId, List<string> fileNames, bool onlyRegisteredDocuments, bool importTariffCodes, bool noMessages, bool overwriteExisting, bool linkPi)
        {
            var docset =
                await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(asycudaDocumentSetId)
                    .ConfigureAwait(false);
            await
                WaterNut.DataSpace.BaseDataModel.Instance.ImportDocuments(docset, fileNames, onlyRegisteredDocuments,
                    importTariffCodes, noMessages, overwriteExisting, linkPi).ConfigureAwait(false);
        }

        public async Task ExportDocument(string fileName, int docId)
        {
            var doc = await WaterNut.DataSpace.BaseDataModel.Instance.GetDocument(docId).ConfigureAwait(false);
            await WaterNut.DataSpace.BaseDataModel.Instance.ExportDocument(fileName, doc).ConfigureAwait(false);
        }

        public async Task ExportDocSet(int docSetId, string directoryName)
        {
            await WaterNut.DataSpace.BaseDataModel.Instance.ExportDocSet(docSetId, directoryName, true).ConfigureAwait(false);
        }

        public async Task SaveAsycudaDocumentSetEx(AsycudaDocumentSetEx asycudaDocumentSetEx)
        {
            var docset = new AsycudaDocumentSet();
            asycudaDocumentSetEx.ModifiedProperties = null;
            docset.InjectFrom(asycudaDocumentSetEx);

            await WaterNut.DataSpace.DocumentDS.DataModels.BaseDataModel.Instance.SaveAsycudaDocumentSet(docset).ConfigureAwait(false);
        }

        public async Task<AsycudaDocumentSetEx> NewDocumentSet(int applicationSettingsId)
        {
            var docset = await WaterNut.DataSpace.BaseDataModel.Instance.CreateAsycudaDocumentSet(applicationSettingsId).ConfigureAwait(false);
            using (var ctx = new AsycudaDocumentSetExService())
            {
                return await ctx.GetAsycudaDocumentSetExByKey(docset.AsycudaDocumentSetId.ToString()).ConfigureAwait(false);
            }
        }

        public async Task BaseDataModelInitialize()
        {
          
          await WaterNut.DataSpace.BaseDataModel.Initialization.ConfigureAwait(false);

        }

        public async Task CleanEntries(int docSetId, IEnumerable<int> lst, bool perIM7)
        {
            var docSet = await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false);
            await WaterNut.DataSpace.CreateIM9.Instance.CleanEntries(docSet,lst, perIM7).ConfigureAwait(false);
        }

        public async Task CleanLines(int docSetId, IEnumerable<int> lst, bool perIM7)
        {
            var docSet = await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false);
            await WaterNut.DataSpace.CreateIM9.Instance.CleanLines(docSet, lst, perIM7).ConfigureAwait(false);
        }

        public async Task CleanBond(int docSetId, bool perIM7)
        {
            var docSet = await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false);
            await WaterNut.DataSpace.CreateIM9.Instance.CleanBond(docSet, perIM7).ConfigureAwait(false);
        }

        #region IDisposable Members

        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        #endregion
    }
}

