using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using CoreEntities.Client.DTO;
using EntryDataQS.Client.DTO;
using EntryDataQS.Client.Contracts;
using Core.Common.Client.Services;

using Core.Common.Contracts;
using System.ComponentModel.Composition;
namespace CoreEntities.Client.Services
{

    public partial class AsycudaDocumentClient 
    {
        public async Task SaveDocument(AsycudaDocument entity)
        {
            await Channel.SaveDocument(entity).ConfigureAwait(false);
        }

        public async Task SaveDocumentCT(AsycudaDocument entity)
        {
            await Channel.SaveDocumentCT(entity).ConfigureAwait(false);
        }

        public async Task DeleteDocument(int asycudaDocumentId)
        {
            await Channel.DeleteDocument(asycudaDocumentId).ConfigureAwait(false);
        }

        public async Task ExportDocument(string fileName, int asycudaDocumentId)
        {
            await Channel.ExportDocument(fileName, asycudaDocumentId).ConfigureAwait(false);
        }

        public async Task<AsycudaDocument> NewDocument(int docSetId)
        {
            return await Channel.NewDocument(docSetId).ConfigureAwait(false);
        }

        public void IM72Ex9Document(string filename)
        {
            Channel.IM72Ex9Document(filename);
        }
    }
}
