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
        public static void Xlsx2csv(FileInfo[] files, FileTypes fileType, bool? overwrite = null )
        {
            try
            {
                foreach (var file in files)
                {
                   
                    var result = XLSXUtils.ExtractTables(file);


                    if (result.Tables.Contains("MisMatches") && result.Tables.Contains("POTemplate")) XLSXUtils.ReadMISMatches(result.Tables["MisMatches"], result.Tables["POTemplate"]);

                    var mainTable = result.Tables[0];
                    var rows = XLSXUtils.FixupDataSet(mainTable);
                    var fileText = XLSXUtils.GetText(fileType, rows, result.Tables[0]);

                    if (ProcessUnknownFileType(fileType, file, rows)) continue;
                    
                    var output = XLSXUtils.CreateCSVFile(file, fileText);


                    XLSXUtils.FixCSVFile(fileType, overwrite, output);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public static bool ProcessUnknownFileType(FileTypes fileType, FileInfo file, List<DataRow> rows)
        {
            if (fileType.ChildFileTypes.FirstOrDefault()?.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.Unknown)
            {
                FileTypeManager.SendBackTooBigEmail(file, fileType);

                var rFileType = XLSXUtils.DetectFileType(fileType, file, rows);

                ImportUtils.ExecuteDataSpecificFileActions(rFileType, new FileInfo[] { file },
                    BaseDataModel.Instance.CurrentApplicationSettings);
                ImportUtils.ExecuteNonSpecificFileActions(rFileType, new FileInfo[] { file },
                    BaseDataModel.Instance.CurrentApplicationSettings);

                return true;
            }

            return false;
        }
    }
}