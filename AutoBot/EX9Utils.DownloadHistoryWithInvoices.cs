using System;
using System.IO;
using System.Linq;
using WaterNut.DataSpace;

namespace AutoBot;

public partial class EX9Utils
{
    public static void DownloadHistoryWithInvoices(int trytimes, string script, bool redownload = false)
    {
        try
        {
            var directoryName = BaseDataModel.GetDocSetDirectoryName("Imports");
            Console.WriteLine("Download History With Invoices");
            //var lcont = 0; // Unused variable

            var startDate =
                DateTime.Parse("9/1/2023"); //BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate;
            var endDate = DateTime.Now;
            var months = (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month;
            foreach (var month in Enumerable.Range(1, months))
            {
                var overviewFile = Path.Combine(directoryName, "OverView.txt");
                if (File.Exists(overviewFile)) File.Delete(overviewFile);
                var sDate = endDate.AddMonths(-(month));
                var eDate = endDate.AddMonths(-(month));

                var sMonth = GetMonths(DateTime.Now.Month, sDate.Month);
                var sYear = DateTime.Now.Year - sDate.Year;

                var eMonth = GetMonths(DateTime.Now.Month, eDate.Month); //DateTime.Now.Month - eDate.Month ;
                var eYear = DateTime.Now.Year - eDate.Year;


                Utils.RetryImport(trytimes, script, redownload, directoryName, sMonth, sYear, eMonth, eYear, sDate.Year,
                    true);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}