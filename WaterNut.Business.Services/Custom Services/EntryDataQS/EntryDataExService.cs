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
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using Serilog; // Added Serilog using
using Serilog.Context; // Added Serilog.Context using

namespace EntryDataQS.Business.Services
{

    public partial class EntryDataExService // Removed : IEntryDataExService
    {
        public async Task AddDocToEntry(IEnumerable<string> lst, int docSetId, bool perInvoice, bool combineEntryDataInSameFile, bool checkPackages, Serilog.ILogger log)
        {
            var docSet = await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false);
            await WaterNut.DataSpace.BaseDataModel.Instance.AddToEntry(lst, docSet, perInvoice, combineEntryDataInSameFile, false, checkPackages, log).ConfigureAwait(false);
        }



        // Reverted to original signature without ILogger
        public async Task SaveCSV(string droppedFilePath, string fileType, int docSetId, bool overWriteExisting)
        {
            // Added LogContext for logging within this method
            using (LogContext.PushProperty("EntryDataExService", nameof(SaveCSV)))
            {
                try
                {
                    var docSet = new List<AsycudaDocumentSet>()
                    {
                        await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId)
                            .ConfigureAwait(false)
                    };

                    var importableFileTypes = await FileTypeManager
                        .GetImportableFileType(fileType, FileTypeManager.FileFormats.Csv, droppedFilePath).ConfigureAwait(false);

                    var dfileType = importableFileTypes.FirstOrDefault(
                            x =>
                                Regex.IsMatch(droppedFilePath, x.FilePattern, RegexOptions.IgnoreCase) &&
                                x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);

                    if (dfileType == null) // for filenames not in database
                    {
                        // Re-fetch if not found, or ensure the logic correctly handles the list if GetImportableFileType returns multiple.
                        // This assumes GetImportableFileType is intended to return a list and we need to re-evaluate or take the first.
                        // If GetImportableFileType was meant to return a single best match, its internal logic might need adjustment.
                        // For now, mirroring the original logic of taking the first if the specific match fails.
                        var allImportableTypes = await FileTypeManager
                            .GetImportableFileType(fileType, FileTypeManager.FileFormats.Csv, droppedFilePath).ConfigureAwait(false);
                        dfileType = allImportableTypes.FirstOrDefault(); // Changed from .First() to .FirstOrDefault() for safety
                    }

                    if (dfileType != null && dfileType.CopyEntryData) // Added null check for dfileType
                    {
                        // Pass null for logger for now, will address this call chain later
                        var documentSetToAdd = await EntryDocSetUtils.GetAsycudaDocumentSet(null, dfileType.DocSetRefernece, true).ConfigureAwait(false);
                        if (documentSetToAdd != null) // Ensure we don't add null
                        {
                            docSet.Add(documentSetToAdd);
                        }
                    }

                    // Pass null for logger for now, will address this call chain later
                    await WaterNut.DataSpace.SaveCSVModel.Instance.ProcessDroppedFile(droppedFilePath, dfileType, docSet,
                        overWriteExisting, null).ConfigureAwait(false);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }


    }
}
