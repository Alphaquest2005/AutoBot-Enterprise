using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using CoreEntities.Business.Entities;
using WaterNut.Business.Services.Importers;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;

namespace AutoBotUtilities
{
    public static class XLSXProcessor
    {
        public static void Xlsx2csv(FileInfo[] files, List<FileTypes> fileTypes, bool? overwrite = null )
        {
            try
            {
                foreach (var file in files)
                {
                   
                    var result = XLSXUtils.ExtractTables(file);


                    if (result.Tables.Contains("MisMatches") && result.Tables.Contains("POTemplate")) XLSXUtils.ReadMISMatches(result.Tables["MisMatches"], result.Tables["POTemplate"]);

                    var mainTable = result.Tables[0];
                    var rows = XLSXUtils.FixupDataSet(mainTable);

                    foreach (var fileType in fileTypes)
                    {
                        var fileText = XLSXUtils.GetText(fileType, rows, result.Tables[0]);

                        if (ProcessUnknownFileType(fileType, file, rows)) continue;

                        // all xlsx suppose to have child filetypes
                        if (!fileType.ChildFileTypes.Any())
                            throw new ApplicationException("XLSX Filetypes supposed to have children");

                        var output = XLSXUtils.CreateCSVFile(file, fileText);

                        XLSXUtils.FixCSVFile(fileType, overwrite, output);
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public static bool ProcessUnknownFileType(FileTypes fileType, FileInfo file, List<DataRow> rows)
        {
            if (fileType.ChildFileTypes.FirstOrDefault(x => x.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Unknown) != null)
            {
                FileTypeManager.SendBackTooBigEmail(file, fileType);

                var rFileType = XLSXUtils.DetectFileType(fileType, file, rows);
                if (fileType.Id != rFileType.Id)
                {
                    ImportUtils.ExecuteDataSpecificFileActions(rFileType, new FileInfo[] { file },
                        BaseDataModel.Instance.CurrentApplicationSettings);
                    ImportUtils.ExecuteNonSpecificFileActions(rFileType, new FileInfo[] { file },
                        BaseDataModel.Instance.CurrentApplicationSettings);
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