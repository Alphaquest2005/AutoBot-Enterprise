
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading.Tasks;
using AllocationQS.Client.Contracts;
using AllocationQS.Client.Services;
using Core.Common.Contracts;
using CoreEntities.Client.Contracts;
using CoreEntities.Client.DTO;
using WaterNut.Client.Services;

namespace CoreEntities.Client.Services
{
   [Export(typeof(AsycudaSalesAllocationsExClient))]
        [Export(typeof(IAsycudaSalesAllocationsExService))]
        [Export(typeof(IClientService))]
        [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DocumentSetClient : ClientService<IDocumentSetService>, IDocumentSetService, IDisposable
    {
        
        public async Task DeleteDocuments(int docSetId)
        {
            await Channel.DeleteDocuments(docSetId).ConfigureAwait(false);
        }

        public async Task DeleteDocumentSet(int docSetId)
        {
            await Channel.DeleteDocumentSet(docSetId).ConfigureAwait(false);
        }

        public async Task ImportDocuments(int asycudaDocumentSetId, List<string> fileNames, bool onlyRegisteredDocuments, bool importTariffCodes, bool noMessages,
            bool overwriteExisting, bool linkPi)
        {
            await Channel.ImportDocuments(asycudaDocumentSetId, fileNames,onlyRegisteredDocuments,importTariffCodes,noMessages,overwriteExisting, linkPi).ConfigureAwait(false);
        }

        public async Task ExportDocument(string fileName, int docId)
        {
            await Channel.ExportDocument(fileName, docId).ConfigureAwait(false);
        }

        public async Task ExportDocSet(int docSetId, string directoryName)
        {
            await Channel.ExportDocSet(docSetId, directoryName).ConfigureAwait(false);
        }

        public async Task SaveAsycudaDocumentSetEx(AsycudaDocumentSetEx asycudaDocumentSetEx)
        {
            await Channel.SaveAsycudaDocumentSetEx(asycudaDocumentSetEx).ConfigureAwait(false);
        }

        public async Task<AsycudaDocumentSetEx> NewDocumentSet(int applicationSettingsId)
        {
            return await Channel.NewDocumentSet(applicationSettingsId).ConfigureAwait(false);
        }

        public async Task BaseDataModelInitialize()
        {
            await Channel.BaseDataModelInitialize().ConfigureAwait(false);
        }


        public async Task CleanBond(int docSetId, bool perIM7)
        {
            await Channel.CleanBond(docSetId, perIM7).ConfigureAwait(false);
        }

        public async Task CleanEntries(int docSetId, IEnumerable<int> lst, bool perIM7)
        {
            await Channel.CleanEntries(docSetId, lst, perIM7).ConfigureAwait(false);
        }

       public async Task CleanLines(int docSetId, IEnumerable<int> lst, bool perIM7)
       {
           await Channel.CleanLines(docSetId, lst, perIM7).ConfigureAwait(false);
       }

       #region IDisposable implementation

        /// <summary>
        /// IDisposable.Dispose implementation, calls Dispose(true).
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose worker method. Handles graceful shutdown of the
        /// client even if it is an faulted state.
        /// </summary>
        /// <param name="disposing">Are we disposing (alternative
        /// is to be finalizing)</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (State != CommunicationState.Faulted)
                    {
                        Close();
                    }
                }
                finally
                {
                    if (State != CommunicationState.Closed)
                    {
                        Abort();
                    }
                    GC.SuppressFinalize(this);
                }
            }
        }



        #endregion




    }
}

