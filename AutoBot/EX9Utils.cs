using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows.Forms;
using AllocationQS.Business.Entities;
using Core.Common.Converters;
using Core.Common.Data.Contracts;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using Newtonsoft.Json.Linq;
using TrackableEntities;
using TrackableEntities.Client;
using WaterNut.Business.Entities;
using WaterNut.Business.Services.Importers;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;

namespace AutoBot
{
    public partial class EX9Utils
    {
        public static async Task RecreateEx9(int months)
        {
            var genDocs = await CreateEX9Utils.CreateEx9(true, months).ConfigureAwait(false);
           

            if (Enumerable.Any<DocumentCT>(genDocs)) //reexwarehouse process
            {
                await ExportEx9Entries(months).ConfigureAwait(false);
                await AssessEx9Entries(months).ConfigureAwait(false);
                DownloadSalesFiles(10, "IM7", false);
                await DocumentUtils.ImportSalesEntries(true).ConfigureAwait(false);
                await ImportWarehouseErrorsUtils.ImportWarehouseErrors(months).ConfigureAwait(false);
                await RecreateEx9(months).ConfigureAwait(false);
                Application.Exit();
            }
            else // reimport and submit to customs
            {
                PDFUtils.LinkPDFs();
                await SubmitSalesXmlToCustomsUtils.SubmitSalesXMLToCustoms(months).ConfigureAwait(false);
                EntryDocSetUtils.CleanupEntries();
                Application.Exit();
            }
        }


        public static async Task ExportEx9Entries(int months)
        {
            Console.WriteLine("Export EX9 Entries");
            try
            {
                var saleInfo =  await BaseDataModel.CurrentSalesInfo(months).ConfigureAwait(false);

                await ExportDocSetSalesReportUtils.ExportDocSetSalesReport(saleInfo.DocSet.AsycudaDocumentSetId,
                    BaseDataModel.GetDocSetDirectoryName(saleInfo.DocSet.Declarant_Reference_Number)).ConfigureAwait(false);

                await BaseDataModel.Instance.ExportDocSet(saleInfo.DocSet.AsycudaDocumentSetId,
                    BaseDataModel.GetDocSetDirectoryName(saleInfo.DocSet.Declarant_Reference_Number), true).ConfigureAwait(false);


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static async Task AssessEx9Entries(int months)
        {
            var currentSalesInfo = await BaseDataModel.CurrentSalesInfo(months).ConfigureAwait(false);
            AssessSalesEntry(currentSalesInfo.Item3
                .Declarant_Reference_Number);
        }

        public static async Task AssessSalesEntry(string docReference)
        {
            var assessComplete = await Utils.AssessComplete(GetInstructionFile(docReference),
                GetInstructionResultsFile(docReference)).ConfigureAwait(false);
            while (docReference != null && assessComplete.success == false)
                Utils.RunSiKuLi(BaseDataModel.GetDocSetDirectoryName(docReference), "AssessIM7",
                    assessComplete.lcontValue.ToString()); //RunSiKuLi(directoryName, "SaveIM7", lcont.ToString());
        }

       

        private static string GetInstructionResultsFile(string docReference) => Path.Combine(BaseDataModel.GetDocSetDirectoryName(docReference), "InstructionResults.txt");

        private static string GetInstructionFile(string docReference) => Path.Combine(BaseDataModel.GetDocSetDirectoryName(docReference), "Instructions.txt");

        public static void DownloadSalesFiles(int trytimes, string script, bool redownload = false)
        {
            try
            {
                var directoryName = BaseDataModel.GetDocSetDirectoryName("Imports");
                Console.WriteLine("Download Entries");
                var lcont = 0;

                Utils.RetryImport(trytimes, script, redownload, directoryName);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        private static int GetMonths(int nowMonth, int sDateMonth)
        {
            int currentMonth = nowMonth; // June
            int targetMonth = sDateMonth; // July
            int monthsBetween = 0;

            while (currentMonth != targetMonth)
            {
                currentMonth--;
                if (currentMonth == 0)
                {
                    currentMonth = 12;
                }
                monthsBetween++;
            }
            return monthsBetween;
        }


        public class SaleReportLine
        {
            public int Line { get; set; }
            public DateTime Date { get; set; }
            public string InvoiceNo { get; set; }
            public string CustomerName { get; set; }
            public string ItemNumber { get; set; }
            public string ItemDescription { get; set; }
            public string TariffCode { get; set; }
            public double SalesQuantity { get; set; }

            public double SalesFactor { get; set; }
            public double xQuantity { get; set; }
            public double Price { get; set; }
            public string SalesType { get; set; }
            public double GrossSales { get; set; }
            public string PreviousCNumber { get; set; }
            public string PreviousLineNumber { get; set; }
            public string PreviousRegDate { get; set; }
            public double CIFValue { get; set; }
            public double DutyLiablity { get; set; }
            public string Comment { get; set; }
        }

        public static async Task RecreateEx9(FileTypes filetype, FileInfo[] files)
        {
            var genDocs = await CreateEX9Utils.CreateEx9(true, -1).ConfigureAwait(false);
            var saleInfo = await BaseDataModel.CurrentSalesInfo(-1).ConfigureAwait(false);
            filetype.AsycudaDocumentSetId = saleInfo.DocSet.AsycudaDocumentSetId;
            if (Enumerable.Any<DocumentCT>(genDocs)) //reexwarehouse process
            {
                filetype.ProcessNextStep.AddRange(new List<string>() { "ExportEx9Entries", "AssessEx9Entries", "DownloadPOFiles", "ImportSalesEntries", "ImportWarehouseErrors", "RecreateEx9" });
            }
            else // reimport and submit to customs
            {
                filetype.ProcessNextStep.AddRange(new List<string>() { "LinkPDFs", "SubmitToCustoms", "CleanupEntries", "Kill" });
            }
        }

        public static async Task ImportXSalesFiles(string testFile)
        {
            var fileTypes = await GetxSalesFileType(testFile).ConfigureAwait(false);
            foreach (var fileType in fileTypes)
            {
                await new FileTypeImporter(fileType).Import(testFile).ConfigureAwait(false);
            }
           
        }

        public static Task<List<FileTypes>> GetxSalesFileType(string fileName)
        {
            return Utils.GetFileType(FileTypeManager.EntryTypes.xSales, FileTypeManager.FileFormats.Csv, fileName);
        }
    }

}