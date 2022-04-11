using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows;
using System.Windows.Controls;
using AllocationQS.Client.Repositories;
using Core.Common.Converters;
using Core.Common.UI;
using CoreEntities.Client.Entities;
using CoreEntities.Client.Repositories;
using EntryDataQS.Client.Repositories;
using SalesDataQS.Client.Repositories;
using SimpleMvvmToolkit;
using WaterNut.QuerySpace.CoreEntities.ViewModels;


namespace WaterNut.QuerySpace.AllocationQS.ViewModels
{
    public class SalesReportModel : ViewModelBase<SalesReportModel> //BaseViewModel
    {
        private static readonly SalesReportModel instance;

        static SalesReportModel()
        {
            instance = new SalesReportModel();
        }

        public static SalesReportModel Instance
        {
            get { return instance; }
        }

        private SalesReportModel()
        {
            RegisterToReceiveMessages<AsycudaDocument>(CoreEntities.MessageToken.CurrentAsycudaDocumentChanged,
                OnCurrentAsycudaDocumentChanged);
        }

        private async void OnCurrentAsycudaDocumentChanged(object sender, NotificationEventArgs<AsycudaDocument> e)
        {
            if (e.Data != null)
            {
                var sdata = await GetDocumentSalesReport(e.Data.ASYCUDA_Id).ConfigureAwait(false);
                SalesReportData = sdata == null
                    ? new ObservableCollection<SaleReportLine>()
                    : new ObservableCollection<SaleReportLine>(sdata);
            }
        }

        public static async Task<IEnumerable<SaleReportLine>> GetDocumentSalesReport(int ASYCUDA_Id)
        {
            var alst = await Ex9SalesReport(ASYCUDA_Id).ConfigureAwait(false);
            if (alst.Any()) return alst;

            alst = await IM9AdjustmentsReport(ASYCUDA_Id).ConfigureAwait(false);
            return alst;
        }

        private static async Task<ObservableCollection<SaleReportLine>> IM9AdjustmentsReport(int ASYCUDA_Id)
        {
            try
            {
                using (var ctx = new AdjustmentShortAllocationRepository())
                {
                    var alst =
                        (await ctx.GetAdjustmentShortAllocationsByExpression(
                                $"xASYCUDA_Id == {ASYCUDA_Id} " + "&& EntryDataDetailsId != null " +
                                "&& PreviousItem_Id != null" + "&& pRegistrationDate != null")
                            .ConfigureAwait(false)).ToList();

                    var d =
                        alst.Where(x => x.xLineNumber != null)
                            .Where(x => !string.IsNullOrEmpty(x.pCNumber))// prevent pre assessed entries
                            .Where(x => x.pItemNumber.Length <= 20) // to match the entry
                            .OrderBy(s => s.xLineNumber)
                            .ThenBy(s => s.InvoiceNo)
                            .Select(s => new SaleReportLine
                            {
                                Line = Convert.ToInt32(s.xLineNumber),
                                Date = Convert.ToDateTime(s.InvoiceDate),
                                InvoiceNo = s.InvoiceNo,
                                Comment = s.Comment,
                                ItemNumber = s.ItemNumber,
                                ItemDescription = s.ItemDescription,
                                TariffCode = s.TariffCode,
                                SalesFactor = Convert.ToDouble(s.SalesFactor),
                                SalesQuantity = Convert.ToDouble(s.QtyAllocated),

                                xQuantity = Convert.ToDouble(s.xQuantity), // Convert.ToDouble(s.QtyAllocated),
                                Price = Convert.ToDouble(s.Cost),
                                SalesType = s.DutyFreePaid,
                                GrossSales = Convert.ToDouble(s.TotalValue),
                                PreviousCNumber = s.pCNumber,
                                PreviousLineNumber = s.pLineNumber.ToString(),
                                PreviousRegDate = Convert.ToDateTime(s.pRegistrationDate).ToShortDateString(),
                                CIFValue =
                                    (Convert.ToDouble(s.Total_CIF_itm) / Convert.ToDouble(s.pQuantity)) *
                                    Convert.ToDouble(s.QtyAllocated),
                                DutyLiablity =
                                    (Convert.ToDouble(s.DutyLiability) / Convert.ToDouble(s.pQuantity)) *
                                    Convert.ToDouble(s.QtyAllocated)
                            }).Distinct();



                    return new ObservableCollection<SaleReportLine>(d);


                }
            }
            catch (Exception Ex)
            {
            }

            return null;
        }

        private static async Task<ObservableCollection<SaleReportLine>> Ex9SalesReport(int ASYCUDA_Id)
        {
            try
            {
                using (var ctx = new AsycudaSalesAllocationsExRepository())
                {
                    var alst =
                        (await ctx.GetAsycudaSalesAllocationsExsByExpression(
                                $"xASYCUDA_Id == {ASYCUDA_Id} " + "&& EntryDataDetailsId != null " +
                                "&& PreviousItem_Id != null" + "&& pRegistrationDate != null")
                            .ConfigureAwait(false)).ToList();

                    var d =
                        alst.Where(x => x.xLineNumber != null)
                            .Where(x => !string.IsNullOrEmpty(x.pCNumber))// prevent pre assessed entries
                            .Where(x => x.pItemNumber.Length <= 20) // to match the entry
                            .OrderBy(s => s.xLineNumber)
                            .ThenBy(s => s.InvoiceNo)
                            .Select(s => new SaleReportLine
                            {
                                Line = Convert.ToInt32(s.xLineNumber),
                                Date = Convert.ToDateTime(s.InvoiceDate),
                                InvoiceNo = s.InvoiceNo,
                                CustomerName = s.CustomerName,
                                ItemNumber = s.ItemNumber,
                                ItemDescription = s.ItemDescription,
                                TariffCode = s.TariffCode,
                                SalesFactor = Convert.ToDouble(s.SalesFactor),
                                SalesQuantity = Convert.ToDouble(s.QtyAllocated),

                                xQuantity = Convert.ToDouble(s.xQuantity), // Convert.ToDouble(s.QtyAllocated),
                                Price = Convert.ToDouble(s.Cost),
                                SalesType = s.DutyFreePaid,
                                GrossSales = Convert.ToDouble(s.TotalValue),
                                PreviousCNumber = s.pCNumber,
                                PreviousLineNumber = s.pLineNumber.ToString(),
                                PreviousRegDate = Convert.ToDateTime(s.pRegistrationDate).ToShortDateString(),
                                CIFValue =
                                    (Convert.ToDouble(s.Total_CIF_itm) / Convert.ToDouble(s.pQuantity)) *
                                    Convert.ToDouble(s.QtyAllocated),
                                DutyLiablity =
                                    (Convert.ToDouble(s.DutyLiability) / Convert.ToDouble(s.pQuantity)) *
                                    Convert.ToDouble(s.QtyAllocated)
                            }).Distinct();


                    
                    return new ObservableCollection<SaleReportLine>(d);
                        
                   
                }
            }
            catch (Exception Ex)
            {
            }

            return null;

        }


        private ObservableCollection<SaleReportLine> _salesReportData = new ObservableCollection<SaleReportLine>();

        public ObservableCollection<SaleReportLine> SalesReportData
        {
            get { return _salesReportData; }
            set
            {
                _salesReportData = value;
                NotifyPropertyChanged(x => SalesReportData);
            }
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




        internal async Task Send2Excel(string path, DataGrid GridData)
        {
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {

                await Task.Factory.StartNew(() =>
                {
                    var s = new ExportToCSV<SaleReportLine, List<SaleReportLine>>();
                    s.StartUp();
                    
                        try
                        {
                            var data = GridData.ItemsSource.OfType<SaleReportLine>();
                            if (data != null)
                            {
                                
                                s.dataToPrint = data.ToList();
                                s.SaveReport(path);
                            }
                            
                            StatusModel.StatusUpdate();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                   
                    s.ShutDown();
                },
                    CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public async Task ExportDocSetSalesReport(int asycudaDocumentSetId, string folder)
        {
            var doclst =
                await
                    SalesDataRepository.Instance.GetSalesDocuments(
                        asycudaDocumentSetId)
                        .ConfigureAwait(false);
            if (doclst == null || !doclst.ToList().Any()) return;
            StatusModel.StartStatusUpdate("Exporting Files", doclst.Count());

            var exceptions = new ConcurrentQueue<Exception>();

            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {

                await Task.Factory.StartNew(() =>
                {
                    var s = new ExportToCSV<SaleReportLine, List<SaleReportLine>>();
                    s.StartUp();
                    foreach (var doc in doclst)
                    {
                        try
                        {
                            var data = GetDocumentSalesReport(doc.ASYCUDA_Id).Result;
                            if (data != null)
                            {
                                string path = Path.Combine(folder,
                                    !string.IsNullOrEmpty(doc.CNumber) ? doc.CNumber : doc.ReferenceNumber + ".csv.pdf");
                                s.dataToPrint = data.ToList();
                                s.SaveReport(path);
                            }
                            else
                            {
                                File.Create(Path.Combine(folder, doc.CNumber ?? doc.ReferenceNumber + ".csv.pdf"));
                            }
                            StatusModel.StatusUpdate();
                        }
                        catch (Exception ex)
                        {
                            exceptions.Enqueue(ex);
                        }
                    }
                    s.ShutDown();
                },
                    CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
            if (exceptions.Count > 0) throw new AggregateException(exceptions);
        }

        public async Task Send2Excel(string folder, AsycudaDocument doc)
        {
            if (doc == null) return;


            var cp = CoreEntities.ViewModels.BaseViewModel.Instance.CustomsProcedures.First(x =>
                x.Customs_ProcedureId == doc.Customs_ProcedureId.GetValueOrDefault());

            if (cp.ExportSupportingEntryData??false)
            {
                if (cp.CustomsOperations.Name == "Exwarehouse")
                {
                    await ExportSalesFile( folder, doc).ConfigureAwait(false);
                }
                else
                {
                    await ExportEntryData(folder, doc).ConfigureAwait(false);
                }
            }
            
            
        }

        private static async Task ExportEntryData(string folder, AsycudaDocument doc)
        {
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
                await Task.Factory.StartNew(() =>
                    {
                        var s = new ExportToCSV<EntryDataLine, List<EntryDataLine>>();
                        s.StartUp();

                        try
                        {
                            folder = Path.GetDirectoryName(folder);
                            var data = GetDocumentEntryData(doc.ASYCUDA_Id).Result.OrderBy(x => x.LineNumber);
                            if (data != null)
                            {
                                string path = Path.Combine(folder,
                                    !string.IsNullOrEmpty(doc.CNumber) ? doc.CNumber : doc.ReferenceNumber + ".csv");

                                s.dataToPrint = data.ToList();
                                s.SaveReport(path);
                            }
                            else
                            {
                                s.dataToPrint = new List<EntryDataLine>();
                                File.Create(Path.Combine(folder, doc.CNumber ?? doc.ReferenceNumber + ".csv"));
                            }

                            StatusModel.StatusUpdate();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                        s.ShutDown();
                    },
                    CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        private static async Task<ObservableCollection<EntryDataLine>> GetDocumentEntryData(int ASYCUDA_Id)
        {
            try
            {
                using (var ctx = new AsycudaDocumentEntryDataLineRepository())
                {
                    var alst =
                        (await ctx.GetAsycudaDocumentEntryDataLinesByExpression(
                                $"AsycudaDocumentId == {ASYCUDA_Id} ")
                            .ConfigureAwait(false)).ToList();

                    var d =
                        alst
                            .Select(s => new EntryDataLine()
                            {
                                LineNumber = s.LineNumber.GetValueOrDefault(),
                                Date = Convert.ToDateTime(s.EntryDataDate),
                                InvoiceNo = s.EntryDataId,
                                
                                ItemNumber = s.ItemNumber,
                                ItemDescription = s.ItemDescription,
                                Quantity = s.Quantity,
                                Cost = s.Cost,
                                PreviousInvoiceNumber = s.PreviousInvoiceNumber,
                                EntryDataDetailsKey = s.EntryDataDetailsKey,
                                Comment = s.Comment
                            }).Distinct();



                    return new ObservableCollection<EntryDataLine>(d);


                }
            }
            catch (Exception Ex)
            {
            }

            return null;

        }

        private static async Task ExportSalesFile(string folder, AsycudaDocument doc)
        {
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
                await Task.Factory.StartNew(() =>
                    {
                        var s = new ExportToCSV<SaleReportLine, List<SaleReportLine>>();
                        s.StartUp();

                        try
                        {
                            folder = Path.GetDirectoryName(folder);
                            var data = GetDocumentSalesReport(doc.ASYCUDA_Id).Result;
                            if (data != null)
                            {
                                string path = Path.Combine(folder,
                                    !string.IsNullOrEmpty(doc.CNumber) ? doc.CNumber : doc.ReferenceNumber + ".csv");

                                s.dataToPrint = data.ToList();
                                s.SaveReport(path);
                            }
                            else
                            {
                                s.dataToPrint = new List<SaleReportLine>();
                                File.Create(Path.Combine(folder, doc.CNumber ?? doc.ReferenceNumber + ".csv"));
                            }

                            StatusModel.StatusUpdate();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                        s.ShutDown();
                    },
                    CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }
    }
    public class EntryDataLine
    {
        public int LineNumber { get; set; }
        public DateTime Date { get; set; }
        public string InvoiceNo { get; set; }
        public string ItemNumber { get; set; }
        public string ItemDescription { get; set; }
        public double Quantity { get; set; }
        public double Cost { get; set; }
        public string EntryDataType { get; set; }
        public string PreviousInvoiceNumber { get; set; }
        public string Comment { get; set; }
        public string EntryDataDetailsKey { get; set; }
    }
}