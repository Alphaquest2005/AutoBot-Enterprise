using System;
using System.IO;
using System.Linq;
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using AutoBot.Services; // Assuming SikuliAutomationService is here

namespace AutoBot
{
    public partial class EX9Utils
    {
        public static void DownloadHistoryWithInvoices(int trytimes, string script, bool redownload = false)
        {
            try
            {
                var directoryName = BaseDataModel.GetDocSetDirectoryName("Imports"); // Assuming GetDocSetDirectoryName exists
                Console.WriteLine("Download History With Invoices");
                var lcont = 0; // lcont is declared but never used

                var startDate = DateTime.Parse("9/1/2023");//BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate; // Assuming OpeningStockDate exists
                var endDate =  DateTime.Now;
                var months = (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month;
                foreach (var month in Enumerable.Range(1, months))
                {
                    var overviewFile = Path.Combine(directoryName, "OverView.txt");
                    if(File.Exists(overviewFile)) File.Delete(overviewFile);
                    var sDate = endDate.AddMonths(-(month));
                    var eDate = endDate.AddMonths(-(month));

                    // This calls GetMonths, which needs to be in its own partial class
                    var sMonth = GetMonths(DateTime.Now.Month,sDate.Month)  ;
                    var sYear = DateTime.Now.Year - sDate.Year;

                    // This calls GetMonths, which needs to be in its own partial class
                    var eMonth = GetMonths(DateTime.Now.Month, eDate.Month); //DateTime.Now.Month - eDate.Month ;
                    var eYear = DateTime.Now.Year - eDate.Year;

                    SikuliAutomationService.RetryImport(trytimes, script, redownload, directoryName, sMonth, sYear, eMonth, eYear, sDate.Year, true); // Assuming RetryImport exists with this signature
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}