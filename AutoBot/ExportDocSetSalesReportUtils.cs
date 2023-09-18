using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Core.Common.Converters;
using CoreEntities.Business.Entities;
using MoreLinq;
using SalesDataQS.Business.Services;
using WaterNut.DataSpace;

namespace AutoBot
{
    public class ExportDocSetSalesReportUtils
    {
        public static async Task ExportDocSetSalesReport(int asycudaDocumentSetId, string folder)
        {
            var doclst = await GetAsycudaDocuments(asycudaDocumentSetId).ConfigureAwait(false);
            
            if (doclst == null || !doclst.ToList().Any()) return;

            await SaveSalesReport(folder, doclst).ConfigureAwait(false);
        }

        private static async Task SaveSalesReport(string folder, IEnumerable<AsycudaDocument> doclst, ConcurrentQueue<Exception> exceptions)
        {
           
                        
                        doclst.Where(x => x != null).ForEach(doc =>  CreateSalesReport(folder, exceptions, doc));

            
        }

        private static void CreateSalesReport(string folder, ConcurrentQueue<Exception> exceptions, AsycudaDocument doc)
        {
            try
            {
                CreateSalesReport(folder, doc);
            }
            catch (Exception ex)
            {
                exceptions.Enqueue(ex);
            }
        }

        public static void CreateSalesReport(string folder, AsycudaDocument doc)
        {
            var s = new ExportToCSV<EX9Utils.SaleReportLine, List<EX9Utils.SaleReportLine>>();
            var data = SalesUtils.GetDocumentSalesReport(doc.ASYCUDA_Id).Result.ToList();

            if (data.Any())
            {
                SaveSalesReport(folder, doc, s, data);
                SavePdf(folder, doc, data);
            }
            else
            {
                File.Create(Path.Combine(folder, doc.CNumber ?? doc.ReferenceNumber + ".pdf"));
            }
        }

        private static void SavePdf(string folder, AsycudaDocument doc, List<EX9Utils.SaleReportLine> data)
        {
            var path = Path.Combine(folder,
                !string.IsNullOrEmpty(doc.CNumber)
                    ? doc.CNumber
                    : doc.ReferenceNumber + ".pdf");

           new PDFCreator<EX9Utils.SaleReportLine>().CreatePDF(data, path);

        }



        private static void SaveSalesReport(string folder, AsycudaDocument doc,
            ExportToCSV<EX9Utils.SaleReportLine, List<EX9Utils.SaleReportLine>> s,
            IEnumerable<EX9Utils.SaleReportLine> data)
        {
            var path = Path.Combine(folder,
                !string.IsNullOrEmpty(doc.CNumber)
                    ? doc.CNumber
                    : doc.ReferenceNumber + ".csv");
            using (var sta = new StaTaskScheduler(1))
            {
                Task.Factory.StartNew(() =>
                    {
                        s.StartUp();
                        s.dataToPrint = data.ToList();
                        s.SaveReport(path);
                        s.ShutDown();

                    },
                    CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }

        }

        private static async Task<IEnumerable<AsycudaDocument>> GetAsycudaDocuments(int asycudaDocumentSetId)
        {
            var doclst =
                await
                    new SalesDataService().GetSalesDocuments(
                            asycudaDocumentSetId)
                        .ConfigureAwait(false);
            return doclst.Where(x => x != null).ToList();
        }

        public static async Task ExportLastDocSetSalesReport(int asycudaDocumentSetId, string folder)
        {
            var doclst = await SalesUtils.GetSalesDocumentsWithEntryData(asycudaDocumentSetId).ConfigureAwait(false);

            if (doclst == null || !doclst.ToList().Any()) return;

            await SaveSalesReport(folder, doclst).ConfigureAwait(false);
        }

        private static async Task SaveSalesReport(string folder, IEnumerable<AsycudaDocument> doclst)
        {
            var exceptions = new ConcurrentQueue<Exception>();

            await SaveSalesReport(folder, doclst, exceptions).ConfigureAwait(false);

            if (exceptions.Count > 0) throw new AggregateException(exceptions);
        }

        public static void CreateSalesReport(string folder, int asycudaId)
        {
            var doc = BaseDataModel.Instance.GetAsycudaDocument(asycudaId).Result;
            CreateSalesReport(folder, doc);
        }
    }
}