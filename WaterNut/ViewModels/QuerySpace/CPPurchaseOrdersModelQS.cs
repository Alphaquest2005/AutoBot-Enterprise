using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using CounterPointQS.Client.Entities;
using CoreEntities.Client.Entities;
using CounterPointQS.Client.Repositories;
using SimpleMvvmToolkit;


namespace WaterNut.QuerySpace.CounterPointQS.ViewModels
{
    public partial class CPPurchaseOrdersModel: CounterPointPOsViewModel_AutoGen 
	{
        private static readonly CPPurchaseOrdersModel instance;
        static CPPurchaseOrdersModel()
        {
            if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.AllowCounterPoint != "Visible")
            {
                instance = null;
                return;
            }
                
            instance = new CPPurchaseOrdersModel()
            {
                DateFilter=DateTime.MinValue,
            };
        }

        public static CPPurchaseOrdersModel Instance
        {
            get { return instance; }
        }

        private CPPurchaseOrdersModel()
        {
            
        }


        internal async Task DownloadCPO(CounterPointPOs counterPointPOs)
        {

            //db.Refresh(System.Data.Objects.RefreshMode.StoreWins, db.EntryData);
            //CycleCurrentAsycudaDocument();
           await CounterPointPOsRepository.Instance.DownloadCPO(counterPointPOs,
                CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId).ConfigureAwait(false);

            MessageBus.Default.BeginNotify(EntryDataQS.MessageToken.EntryDataExChanged, new CPPurchaseOrdersModel(), new NotificationEventArgs(QuerySpace.EntryDataQS.MessageToken.EntryDataExChanged));
            MessageBus.Default.BeginNotify(MessageToken.CurrentCounterPointPOsChanged, new CPPurchaseOrdersModel(), new NotificationEventArgs(MessageToken.CurrentCounterPointPOsChanged));
            MessageBus.Default.BeginNotify(MessageToken.CounterPointPOsChanged, new CPPurchaseOrdersModel(), new NotificationEventArgs(MessageToken.CounterPointPOsChanged));

            MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);

        }

	    internal async Task SilentDownloadCPO(CounterPointPOs counterPointPOs)
	    {

	        //db.Refresh(System.Data.Objects.RefreshMode.StoreWins, db.EntryData);
	        //CycleCurrentAsycudaDocument();
	        await CounterPointPOsRepository.Instance.DownloadCPO(counterPointPOs,
	            CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId).ConfigureAwait(false);

	        
	    }

	    internal void NotifyComplete()
	    {
	        MessageBus.Default.BeginNotify(QuerySpace.EntryDataQS.MessageToken.EntryDataExChanged,
	            new CPPurchaseOrdersModel(),
	            new NotificationEventArgs(QuerySpace.EntryDataQS.MessageToken.EntryDataExChanged));
	        MessageBus.Default.BeginNotify(MessageToken.CurrentCounterPointPOsChanged, new CPPurchaseOrdersModel(),
	            new NotificationEventArgs(MessageToken.CurrentCounterPointPOsChanged));
	        MessageBus.Default.BeginNotify(MessageToken.CounterPointPOsChanged, new CPPurchaseOrdersModel(),
	            new NotificationEventArgs(MessageToken.CounterPointPOsChanged));

	        MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
	    }
	}
}