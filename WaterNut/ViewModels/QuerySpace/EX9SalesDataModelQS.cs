using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
using System.Linq;
using System.Data.Entity;
using System.Collections.ObjectModel;
using EntryDataQS.Client.Repositories;
using SalesDataQS.Client.Entities;
using SalesDataQS.Client.Repositories;
using CoreEntities.Client.Entities;
using AllocationQS.Client.Entities;
using SimpleMvvmToolkit;




namespace WaterNut.QuerySpace.SalesDataQS.ViewModels
{
    public class Ex9SalesDataModel : SalesDataViewModel_AutoGen 
	{
        private static readonly Ex9SalesDataModel instance;
        static Ex9SalesDataModel()
        {
            instance = new Ex9SalesDataModel()
            {
                EntryDataDateFilter = DateTime.MinValue,
                StartEntryDataDateFilter = DateTime.MinValue,
                EndEntryDataDateFilter = DateTime.MinValue
                
            };
        }

        public static Ex9SalesDataModel Instance
        {
            get { return instance; }
        }
		private Ex9SalesDataModel()
		{
            RegisterToReceiveMessages<AsycudaSalesAndAdjustmentAllocationsEx>(AllocationQS.MessageToken.CurrentAsycudaSalesAndAdjustmentAllocationsExChanged, OnCurrentAsycudaSalesAllocationsExChanged);
            RegisterToReceiveMessages<AsycudaDocumentSetEx>(CoreEntities.MessageToken.CurrentAsycudaDocumentSetExChanged, OnCurrentAsycudaDocumentSetExChanged);
            RegisterToReceiveMessages<AsycudaDocument>(CoreEntities.MessageToken.CurrentAsycudaDocumentChanged, OnCurrentAsycudaDocumentChanged);
           // RegisterToReceiveMessages(MessageToken.SelectedSalesDatasChanged, OnSelectedSalesDatasChanged);
           // RegisterToReceiveMessages(MessageToken.SalesDatasFilterExpressionChanged, OnSalesDatasFilterExpressionChanged);
		}

	    private void OnCurrentAsycudaSalesAllocationsExChanged(object sender, NotificationEventArgs<AsycudaSalesAndAdjustmentAllocationsEx> e)
	    {
	        if (e.Data != null)
	        {
	            GetSalesData(e.Data.AllocationId);
	        }
        }

	    private void OnCurrentAsycudaDocumentChanged(object sender, NotificationEventArgs<AsycudaDocument> e)
        {
            if (e.Data != null)
            {
                vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";
                // vloader.NavExpression = string.Format("AsycudaDocumentSetId = {0}", e.Data.AsycudaDocumentSetId);
                vloader.SetNavigationExpression("AsycudaDocuments", $"AsycudaDocumentId == {e.Data.ASYCUDA_Id}");
                SalesDatas.Refresh();
            }
        }

        private void OnCurrentAsycudaDocumentSetExChanged(object sender, NotificationEventArgs<AsycudaDocumentSetEx> e)
        {
            if (e.Data != null)
            {
                vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";      
                vloader.SetNavigationExpression("AsycudaDocumentSets",
                    $"AsycudaDocumentSetId == {e.Data.AsycudaDocumentSetId}");
                SalesDatas.Refresh();
            }
        }

        //private void OnSalesDatasFilterExpressionChanged(object sender, NotificationEventArgs e)
        //{
        //    UpdateSelectedTotals();
        //}

        //private void UpdateSelectedTotals()
        //{
        //    var lst = SelectedSalesDatas.Where(x => x != null);
        //    SelectedTotal = Convert.ToDecimal(lst.Sum(x => x.Total));
        //}

        //private void OnSelectedSalesDatasChanged(object sender, NotificationEventArgs e)
        //{
        //    UpdateSelectedTotals();
        //}



        private void OnCurrentAsycudaSalesAllocationsExChanged(object sender, NotificationEventArgs<AsycudaSalesAllocationsEx> e)
        {
           // throw new NotImplementedException();
            if (e.Data != null)
            {
                GetSalesData(e.Data.AllocationId);
            }
        }

	    private void GetSalesData(int allocationId)
	    {
	        vloader.SetNavigationExpression("SalesDataAllocations", $"AllocationId == {allocationId}");
	        SalesDatas.Refresh();

	        var s = SalesDatas.FirstOrDefault();
	        if (s != null)
	        {
	            SelectedSalesDatas = new ObservableCollection<SalesData>() {s.Data};
	        }
	    }


	    public override void FilterData()
        {
            var res = GetAutoPropertyFilterString();
            //if (DutyPaidFilter == true)
            //{
            //    res.Append("&& TaxAmount != 0");
            //}
            //if (DutyFreeFilter == true)
            //{
            //    res.Append("&& TaxAmount == 0");
            //}

            if (!_dutyFreeFilter && !_dutyPaidFilter) res.Append("&& TaxAmount == null");
            if (_dutyFreeFilter || _dutyPaidFilter) res.Append("&& (");
            if (_dutyFreeFilter == true)
            {
                res.Append("TaxAmount == 0");
            }


            if (_dutyPaidFilter == true)
            {
                if (_dutyFreeFilter == true)
                {
                    res.Append("|| TaxAmount != 0");
                }
                else
                {
                    res.Append(" TaxAmount != 0");
                }
            }
            if (_dutyFreeFilter || _dutyPaidFilter) res.Append(")");

          FilterData(res);
        }



        bool _dutyFreeFilter = true;
        public bool DutyFreeFilter
        {
            get
            {
                return _dutyFreeFilter;
            }
            set
            {
                _dutyFreeFilter = value;
                
                FilterData();
                NotifyPropertyChanged(x => DutyFreeFilter);
            }
        }

        bool _dutyPaidFilter = true;
        public bool DutyPaidFilter
        {
            get
            {
                return _dutyPaidFilter;
            }
            set
            {
                _dutyPaidFilter = value;
               
                FilterData();
                NotifyPropertyChanged(x => DutyFreeFilter);
            }
        }

        //private decimal _total = 0;
        //public decimal Total
        //{
        //    get { return _total; }
        //    set
        //    {
        //        _total = value;
        //        NotifyPropertyChanged(x => Total);
        //    }
        //}

        //private decimal _selectedTotal = 0;
        //public decimal SelectedTotal
        //{
        //    get { return _selectedTotal; }
        //    set
        //    {
        //        _selectedTotal = value;
        //        NotifyPropertyChanged(x => SelectedTotal);
        //    }
        //}


        internal async Task RemoveSalesData(SalesData salesData)
        {
            throw new NotImplementedException();
        }

        internal async Task SaveCSV(string fileType)
        {
            await QuerySpace.SaveCSV.Instance.SaveCSVFile(fileType,
                CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId).ConfigureAwait(false);
        }

        internal async Task RemoveSelectedSalesData(List<SalesData> lst)
        {
            var res = MessageBox.Show("Are you sure you want to delete all Selected Items?", "Delete selected Items",
                  MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                await EntryDataExRepository.Instance.RemoveSelectedEntryDataEx(SelectedSalesDatas.Select(x => x.EntryDataId)).ConfigureAwait(false);

                MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentsChanged, null,
                    new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentsChanged));

                MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged, null,
                    new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged));

                MessageBus.Default.BeginNotify(CounterPointQS.MessageToken.CounterPointPOsChanged, null,
                    new NotificationEventArgs(CounterPointQS.MessageToken.CounterPointPOsChanged));

                MessageBus.Default.BeginNotify(MessageToken.SalesDatasChanged, null,
                    new NotificationEventArgs(MessageToken.SalesDatasChanged));

                MessageBus.Default.BeginNotify(MessageToken.SalesDatasFilterExpressionChanged, null,
                   new NotificationEventArgs(MessageToken.SalesDatasFilterExpressionChanged));

                MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                //EntryDataEx.Refresh();
            }
        }
    }
}