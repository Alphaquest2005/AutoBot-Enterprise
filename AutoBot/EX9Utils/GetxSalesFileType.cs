using System.Collections.Generic;
using CoreEntities.Business.Entities; // Assuming FileTypes is here
using WaterNut.Business.Services.Utils; // Assuming FileTypeManager is here

namespace AutoBot
{
    public partial class EX9Utils
    {
        public static List<FileTypes> GetxSalesFileType(string fileName)
        {
            return FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.xSales, FileTypeManager.FileFormats.Csv, fileName); // Assuming FileTypeManager exists with these members
        }
    }
}