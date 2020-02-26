
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using TrackableEntities.Client;
using WaterNut.DataSpace;

namespace EntryDataQS.Business.Services
{

    public partial class EntryDataExService
    {
        public async Task AddDocToEntry(IEnumerable<string> lst, int docSetId, bool perInvoice, bool combineEntryDataInSameFile)
        {
            var docSet = await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false);
            await WaterNut.DataSpace.BaseDataModel.Instance.AddToEntry(lst, docSet, perInvoice, combineEntryDataInSameFile, false).ConfigureAwait(false);
        }



        public async Task SaveCSV(string droppedFilePath, string fileType, int docSetId, bool overWriteExisting)
        {
            var docSet = new List<AsycudaDocumentSet>() {await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false)};
            using (var ctx = new CoreEntitiesContext())
            {
                var dfileType = ctx.FileTypes.ToList().FirstOrDefault(x =>
                    Regex.IsMatch(droppedFilePath, x.FilePattern, RegexOptions.IgnoreCase) && x.Type == fileType);
                if (dfileType == null) // for filenames not in database
                {
                    dfileType = ctx.FileTypes.First(x => x.Type == fileType);
                }
                if(dfileType.CopyEntryData)docSet.Add(await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(dfileType.AsycudaDocumentSetId).ConfigureAwait(false));
                await WaterNut.DataSpace.SaveCSVModel.Instance.ProcessDroppedFile(droppedFilePath, dfileType, docSet,
                   overWriteExisting).ConfigureAwait(false);
            }
                
        }

        public async Task SavePDF(string droppedFilePath, string fileType, int docSetId, bool overwrite)
        {
            
            int? emailId = 0;
            int? fileTypeId = 0;
            using (var ctx = new CoreEntitiesContext())
            {

                var res = ctx.AsycudaDocumentSet_Attachments.Where(x => x.Attachments.FilePath == droppedFilePath)
                    .Select(x => new { x.EmailUniqueId, x.FileTypeId }).FirstOrDefault();
                emailId = res?.EmailUniqueId;
                fileTypeId = res?.FileTypeId;

                var dfileType = ctx.FileTypes.ToList().FirstOrDefault(x =>
                    Regex.IsMatch(droppedFilePath, x.FilePattern, RegexOptions.IgnoreCase) && x.Type == fileType);
                if (dfileType == null) // for filenames not in database
                {
                    dfileType = ctx.FileTypes.First(x => x.Type == fileType);
                }

                dfileType.AsycudaDocumentSetId = docSetId;
               InvoiceReader.Import(droppedFilePath, fileTypeId, emailId, overwrite, SaveCSVModel.Instance.GetDocSets(dfileType), dfileType.Type);
            }
            
        }


    }
}



