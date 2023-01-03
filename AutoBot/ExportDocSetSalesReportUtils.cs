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
            using (var sta = new StaTaskScheduler(1))
            {
                await Task.Factory.StartNew(() =>
                    {
                        var s = new ExportToCSV<EX9Utils.SaleReportLine, List<EX9Utils.SaleReportLine>>();
                        s.StartUp();
                        doclst.ForEach(doc =>  CreateSalesReport(folder, exceptions, doc, s));

                        s.ShutDown();
                    },
                    CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        private static void CreateSalesReport(string folder, ConcurrentQueue<Exception> exceptions, AsycudaDocument doc, ExportToCSV<EX9Utils.SaleReportLine, List<EX9Utils.SaleReportLine>> s)
        {
            try
            {
                var data = SalesUtils.GetDocumentSalesReport(doc.ASYCUDA_Id).Result;

                if (data != null)
                {
                    SaveSalesReport(folder, doc, s, data);
                }
                else
                {
                    File.Create(Path.Combine(folder, doc.CNumber ?? doc.ReferenceNumber + ".csv.pdf"));
                }
            }
            catch (Exception ex)
            {
                exceptions.Enqueue(ex);
            }
        }

        private static void SaveSalesReport(string folder, AsycudaDocument doc, ExportToCSV<EX9Utils.SaleReportLine, List<EX9Utils.SaleReportLine>> s, IEnumerable<EX9Utils.SaleReportLine> data)
        {
            var path = Path.Combine(folder,
                !string.IsNullOrEmpty(doc.CNumber)
                    ? doc.CNumber
                    : doc.ReferenceNumber + ".csv.pdf");
            s.dataToPrint = data.ToList();
            s.SaveReport(path);
        }

        private static async Task<IEnumerable<AsycudaDocument>> GetAsycudaDocuments(int asycudaDocumentSetId)
        {
            var doclst =
                await
                    new SalesDataService().GetSalesDocuments(
                            asycudaDocumentSetId)
                        .ConfigureAwait(false);
            return doclst;
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
    }
}