using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;


using System.Linq;
using System.Data.Entity;
using System.Windows;
using Core.Common.UI;
using CounterPointQS.Client.Entities;
using CounterPointQS.Client.Repositories;
using CoreEntities.Client.Entities;
using SimpleMvvmToolkit;

namespace WaterNut.QuerySpace.CounterPointQS.ViewModels
{
    public class CPSalesModel : CounterPointSalesViewModel_AutoGen 
	{
		      
        private static readonly CPSalesModel instance;
        static CPSalesModel()
        {
            if (WaterNut.QuerySpace.CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings
                .AllowCounterPoint != "Hidden")
            {
                instance = new CPSalesModel() {DateFilter = DateTime.MinValue};
            }

        }

	    //private static void OnCurrentApplicationSettingsChanged(object sender, NotificationEventArgs<ApplicationSettings> e)
	    //{
     //       if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.AllowCounterPoint != "Visible")
     //       {
     //           instance = null;
     //           return;
     //       }
            
     //   }

        public static CPSalesModel Instance
        {
            get { return instance; }
        }

        private CPSalesModel()
        {
          //  RegisterToReceiveMessages<ApplicationSettings>(CoreEntities.MessageToken.CurrentApplicationSettingsChanged, OnCurrentApplicationSettingsChanged);

        }
        
        internal async Task DownloadCPSales(global::CounterPointQS.Client.Entities.CounterPointSales counterPointSales)
        {
            StatusModel.Timer("Importing CounterPoint Sales");
            if(CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx != null)
            {
           await CounterPointSalesRepository.Instance.DownloadCPSales(counterPointSales,
                CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId)
                .ConfigureAwait(false);
            
            MessageBus.Default.BeginNotify(EntryDataQS.MessageToken.EntryDataExChanged, null,
                                     new NotificationEventArgs(EntryDataQS.MessageToken.EntryDataExChanged));
            MessageBus.Default.BeginNotify(EntryDataQS.MessageToken.EntryDataDetailsExesChanged, null,
                                                 new NotificationEventArgs(EntryDataQS.MessageToken.EntryDataDetailsExesChanged));

            MessageBus.Default.BeginNotify(QuerySpace.SalesDataQS.MessageToken.SalesDatasChanged, null,
                                                 new NotificationEventArgs(QuerySpace.SalesDataQS.MessageToken.SalesDatasChanged));
            MessageBus.Default.BeginNotify(QuerySpace.SalesDataQS.MessageToken.SalesDataDetailsChanged, null,
                                                 new NotificationEventArgs(QuerySpace.SalesDataQS.MessageToken.SalesDataDetailsChanged));


            MessageBus.Default.BeginNotify(QuerySpace.CounterPointQS.MessageToken.CounterPointSalesChanged, null,
                                                 new NotificationEventArgs(QuerySpace.CounterPointQS.MessageToken.CounterPointSalesChanged));
              }
            else
            {
                MessageBox.Show("Please Select a Asycuda Document Set before downloading Sales");
            }    
            StatusModel.StopStatusUpdate();
        }

        internal async Task DownloadCPSalesDateRange()
        {
            if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx != null)
            {
               await CounterPointSalesRepository.Instance.DownloadCPSalesDateRange(StartDateFilter.GetValueOrDefault(),
                    EndDateFilter.GetValueOrDefault(),
                    CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId).ConfigureAwait(false);

                StatusModel.Timer("Refreshing Sales Data...");
                MessageBus.Default.BeginNotify(EntryDataQS.MessageToken.EntryDataExChanged, null,
                    new NotificationEventArgs(EntryDataQS.MessageToken.EntryDataExChanged));
                MessageBus.Default.BeginNotify(EntryDataQS.MessageToken.EntryDataDetailsExesChanged, null,
                    new NotificationEventArgs(EntryDataQS.MessageToken.EntryDataDetailsExesChanged));
                MessageBus.Default.BeginNotify(SalesDataQS.MessageToken.SalesDatasChanged, null,
                    new NotificationEventArgs(SalesDataQS.MessageToken.SalesDatasChanged));
                MessageBus.Default.BeginNotify(SalesDataQS.MessageToken.SalesDataDetailsChanged, null,
                    new NotificationEventArgs(SalesDataQS.MessageToken.SalesDataDetailsChanged));

                MessageBus.Default.BeginNotify(MessageToken.CounterPointSalesChanged, null,
                    new NotificationEventArgs(MessageToken.CounterPointSalesChanged));

                StatusModel.StopStatusUpdate();
                MessageBox.Show("Download Complete", "Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);


            }

            else
            {
                MessageBox.Show("Please Select a Asycuda Document Set before downloading Sales");
            }

        }
	}
}