using System;
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using AutoBot.Services; // Assuming SikuliAutomationService is here

namespace AutoBot
{
    public partial class EX9Utils
    {
        public static void DownloadSalesFiles(int trytimes, string script, bool redownload = false)
        {
            try
            {
                var directoryName = BaseDataModel.GetDocSetDirectoryName("Imports"); // Assuming GetDocSetDirectoryName exists
                Console.WriteLine("Download Entries");
                var lcont = 0; // lcont is declared but never used

                SikuliAutomationService.RetryImport(trytimes, script, redownload, directoryName); // Assuming RetryImport exists
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}