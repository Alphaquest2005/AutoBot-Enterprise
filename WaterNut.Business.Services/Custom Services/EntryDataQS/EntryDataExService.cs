
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using EmailDownloader;
using TrackableEntities.Client;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;

namespace EntryDataQS.Business.Services
{

    public partial class EntryDataExService
    {
        public async Task AddDocToEntry(IEnumerable<string> lst, int docSetId, bool perInvoice, bool combineEntryDataInSameFile, bool checkPackages)
        {
            var docSet = await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false);
            await WaterNut.DataSpace.BaseDataModel.Instance.AddToEntry(lst, docSet, perInvoice, combineEntryDataInSameFile, false, checkPackages).ConfigureAwait(false);
        }



        public async Task SaveCSV(string droppedFilePath, string fileType, int docSetId, bool overWriteExisting)
        {
            try
            {


                var docSet = new List<AsycudaDocumentSet>()
                {
                    await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId)
                        .ConfigureAwait(false)
                };

                var dfileType = FileTypeManager
                    .GetImportableFileType(fileType, FileTypeManager.FileFormats.Csv, droppedFilePath).FirstOrDefault(
                        x =>
                            Regex.IsMatch(droppedFilePath, x.FilePattern, RegexOptions.IgnoreCase) &&
                            x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                if (dfileType == null) // for filenames not in database
                {
                    dfileType = FileTypeManager
                        .GetImportableFileType(fileType, FileTypeManager.FileFormats.Csv, droppedFilePath).First();
                }

                if (dfileType.CopyEntryData)
                {
                    docSet.Add(EntryDocSetUtils.GetAsycudaDocumentSet(dfileType.DocSetRefernece, true));
                }

                await WaterNut.DataSpace.SaveCSVModel.Instance.ProcessDroppedFile(droppedFilePath, dfileType, docSet,
                    overWriteExisting).ConfigureAwait(false);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task SavePDF(string droppedFilePath, string fileType, int docSetId, bool overwrite)
        {
            
                var res = new CoreEntitiesContext().AsycudaDocumentSet_Attachments.Where(x => x.Attachments.FilePath == droppedFilePath)
                    .Select(x => new { x.EmailId, x.FileTypeId }).FirstOrDefault();
                var emailId = res?.EmailId;
                var fileTypeId = res?.FileTypeId;

                var dfileType = FileTypeManager.GetImportableFileType(fileType, FileTypeManager.FileFormats.PDF, droppedFilePath).ToList().FirstOrDefault(x =>
                    Regex.IsMatch(droppedFilePath, x.FilePattern, RegexOptions.IgnoreCase));
                if (dfileType == null) // for filenames not in database
                {
                    dfileType = FileTypeManager.GetImportableFileType(fileType, FileTypeManager.FileFormats.PDF, droppedFilePath).First();
                }

                dfileType.AsycudaDocumentSetId = docSetId;
                var client = BaseDataModel.GetClient();
                InvoiceReader.Import(droppedFilePath, fileTypeId.GetValueOrDefault(), emailId, overwrite, Utils.GetDocSets(dfileType), dfileType, client);
            
            
        }
    }
}



