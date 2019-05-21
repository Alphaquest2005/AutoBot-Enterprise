
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntryDataQS.Business.Services
{

    public partial class EntryDataExService
    {
        public async Task AddDocToEntry(IEnumerable<string> lst, int docSetId, bool perInvoice)
        {
            var docSet = await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false);
            await WaterNut.DataSpace.BaseDataModel.Instance.AddToEntry(lst, docSet, perInvoice).ConfigureAwait(false);
        }



        public async Task SaveCSV(string droppedFilePath, string fileType, int docSetId, bool overWriteExisting)
        {
            var docSet = await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false);
             await  WaterNut.DataSpace.SaveCSVModel.Instance.ProcessDroppedFile(droppedFilePath, fileType, docSet,
                overWriteExisting).ConfigureAwait(false);
        }

        public async Task SavePDF(string droppedFilePath, string fileType, int docSetId, bool overwrite)
        {
            var docSet = await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false);
            await WaterNut.DataSpace.SavePDFModel.Instance.ProcessDroppedFile(droppedFilePath, fileType, docSet,
                overwrite).ConfigureAwait(false);
        }

        public async Task SaveTXT(string droppedFilePath, string fileType, int docSetId, bool overwrite)
        {
            var docSet = await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false);
            await WaterNut.DataSpace.SaveTXT.Instance.ProcessDroppedFile(droppedFilePath, fileType, docSet,
                overwrite).ConfigureAwait(false);
        }
    }
}



