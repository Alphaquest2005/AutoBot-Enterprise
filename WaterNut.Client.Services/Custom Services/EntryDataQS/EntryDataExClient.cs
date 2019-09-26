
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


namespace EntryDataQS.Client.Services
{

    public partial class EntryDataExClient 
    {
        public async Task AddDocToEntry(IEnumerable<string> lst, int docSetId, bool perInvoice,
            bool combineEntryDataInSameFile)
        {
            await Channel.AddDocToEntry(lst, docSetId, perInvoice, combineEntryDataInSameFile).ConfigureAwait(false);
        }



        public async Task SaveCSV(string droppedFilePath, string fileType, int docSet,
            bool overWriteExisting)
        {
            await Channel.SaveCSV(droppedFilePath, fileType, docSet, overWriteExisting).ConfigureAwait(false);
        }

        public async Task SavePDF(string droppedFilePath, string fileType, int docSetId, bool overwrite)
        {
            await Channel.SavePDF(droppedFilePath, fileType, docSetId, overwrite).ConfigureAwait(false);
        }

        public async Task SaveTXT(string droppedFilePath, string fileType, int docSetId, bool overwrite)
        {
            await Channel.SaveTXT(droppedFilePath, fileType, docSetId, overwrite).ConfigureAwait(false);
        }
    }
}

