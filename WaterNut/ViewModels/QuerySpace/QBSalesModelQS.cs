using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Core.Common.UI;
using SalesDataQS.Client.Repositories;
using SimpleMvvmToolkit;

namespace WaterNut.QuerySpace.QuickBooksQS.ViewModels
{
    public class QBSalesModel : ViewModelBase<QBSalesModel>
    {
		
         private static readonly QBSalesModel instance;
         static QBSalesModel()
        {
            instance = new QBSalesModel();
        }

         public static QBSalesModel Instance
        {
            get { return instance; }
        }

        private QBSalesModel()
        {
            
        }
        private bool _importInventory = true;
		public bool ImportInventory
		{
			get
			{
				return _importInventory;
			}
			set
			{
				_importInventory = value;
				NotifyPropertyChanged(x => ImportInventory);
			}
		}

        private bool _importSales = true;
		public bool ImportSales
		{
			get
			{
				return _importSales;
			}
			set
			{
				_importSales = value;
				NotifyPropertyChanged(x => ImportSales);
			}
		}


        private DateTime _startDate = DateTime.Parse($"{DateTime.Now.Month}/1/{DateTime.Now.Year}");
		public DateTime StartDate
		{
			get { return _startDate; }
			set
			{
				_startDate = value;
				NotifyPropertyChanged(x => StartDate);
			}
		}



        private DateTime _endDate = DateTime.Parse(string.Format("{0}/{2}/{1}", DateTime.Now.Month, DateTime.Now.Year, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)));
		public DateTime EndDate
		{
			get { return _endDate; }
			set
			{
				_endDate = value;
				NotifyPropertyChanged(x => EndDate);
			}
		}




        internal async Task DownloadQBData()
        {
            StatusModel.Timer("Getting QuickBooks Data");
            if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx != null)
            {

               await SalesDataRepository.Instance.DownloadQBData(StartDate, EndDate, ImportSales, ImportInventory,
                    CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId).ConfigureAwait(false);

                MessageBus.Default.BeginNotify(EntryDataQS.MessageToken.EntryDataExChanged, null,
                                                     new NotificationEventArgs(EntryDataQS.MessageToken.EntryDataExChanged));
                MessageBus.Default.BeginNotify(EntryDataQS.MessageToken.EntryDataDetailsExesChanged, null,
                                                     new NotificationEventArgs(EntryDataQS.MessageToken.EntryDataDetailsExesChanged));

                MessageBus.Default.BeginNotify(QuerySpace.SalesDataQS.MessageToken.SalesDatasChanged, null,
                                                     new NotificationEventArgs(QuerySpace.SalesDataQS.MessageToken.SalesDatasChanged));
                MessageBus.Default.BeginNotify(QuerySpace.SalesDataQS.MessageToken.SalesDataDetailsChanged, null,
                                                     new NotificationEventArgs(QuerySpace.SalesDataQS.MessageToken.SalesDataDetailsChanged));
                MessageBus.Default.BeginNotify(QuerySpace.SalesDataQS.MessageToken.SalesDatasChanged, null, 
                                new NotificationEventArgs(QuerySpace.SalesDataQS.MessageToken.SalesDatasChanged));
                MessageBus.Default.BeginNotify(QuerySpace.SalesDataQS.MessageToken.SalesDataDetailsChanged, null,
                                new NotificationEventArgs(QuerySpace.SalesDataQS.MessageToken.SalesDataDetailsChanged));
				
				
			}
			else
			{
				MessageBox.Show("Please Select Asycuda Document Set");
			}

            StatusModel.StopStatusUpdate();
        }
    }
}