using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using WaterNut.Business.Services.Importers;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;

namespace AutoBotUtilities
{
    using ExcelDataReader.Log;
    using Serilog;

    public static class XLSXProcessor
    {
        public static async Task Xlsx2csv(FileInfo[] files, List<FileTypes> fileTypes, ILogger log, bool? overwrite = null )
        {
            try
            {
                foreach (var file in files)
                {
                   
                    var result = XLSXUtils.ExtractTables(file);


                    if (result.Tables.Contains("MisMatches") && result.Tables.Contains("POTemplate")) await XLSXUtils.ReadMISMatches(result.Tables["MisMatches"], result.Tables["POTemplate"], log).ConfigureAwait(false);

                    var mainTable = result.Tables[0];
                    var rows = XLSXUtils.FixupDataSet(mainTable);

                    foreach (var fileType in fileTypes)
                    {
                        var fileText = XLSXUtils.GetText(fileType, rows, result.Tables[0]);

                        if (await ProcessUnknownFileType(fileType, file, rows, log).ConfigureAwait(false)) continue;

                        // all xlsx suppose to have child filetypes
                        if (!fileType.ChildFileTypes.Any())
                            throw new ApplicationException("XLSX Filetypes supposed to have children");

                        var output = XLSXUtils.CreateCSVFile(file, fileText);

                       await XLSXUtils.FixCSVFile(fileType, overwrite, output, log).ConfigureAwait(false);
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }


        public static async Task<bool> ProcessUnknownFileType(
            FileTypes fileType,
            FileInfo file,
            List<DataRow> rows,
            ILogger log)
        {
            if (fileType.ChildFileTypes.FirstOrDefault(x => x.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Unknown) != null)
            {
                await FileTypeManager.SendBackTooBigEmail(file, fileType, log).ConfigureAwait(false);

                var rFileType = await XLSXUtils.DetectFileType(fileType, file, rows).ConfigureAwait(false);
                if (fileType.Id != rFileType.Id)
                {
                    // Assuming these Execute methods are synchronous. If they are async, they would also need await.
                    // Based on the original error, the issue was with rFileType, not these calls themselves.
                    await new ImportUtils(log).ExecuteDataSpecificFileActions(rFileType, new FileInfo[] { file },
                        BaseDataModel.Instance.CurrentApplicationSettings).ConfigureAwait(false);
                    await new ImportUtils(log).ExecuteNonSpecificFileActions(rFileType, new FileInfo[] { file },
                        BaseDataModel.Instance.CurrentApplicationSettings).ConfigureAwait(false);
                }
                else
                {
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}