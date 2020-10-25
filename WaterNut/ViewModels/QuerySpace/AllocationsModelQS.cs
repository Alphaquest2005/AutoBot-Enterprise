using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AllocationQS.Client.Entities;
using AllocationQS.Client.Repositories;
using Core.Common.UI;
using CoreEntities.Client.Repositories;
using SalesDataQS.Client.Entities;
using PreviousDocumentQS.Client.Entities;
using SimpleMvvmToolkit;
using WaterNut.QuerySpace.PreviousDocumentQS.ViewModels;
using AsycudaDocumentItem = CoreEntities.Client.Entities.AsycudaDocumentItem;
using System.Data.Entity;
using Core.Common.UI.DataVirtualization;
using CoreEntities.Client.Entities;

namespace WaterNut.QuerySpace.AllocationQS.ViewModels
{
    public partial class AsycudaSalesAndAdjustmentAllocationsExViewModel_AutoGen
    {
        partial void OnCreated()
        {
            _endInvoiceDateFilter = DateTime.Parse(string.Format("{0}/{2}/{1}", DateTime.Now.Month, DateTime.Now.Year, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)));

            _pRegistrationDateFilter = DateTime.MinValue;
            _startpRegistrationDateFilter = DateTime.MinValue;
            _endpRegistrationDateFilter = DateTime.MinValue;
            _xRegistrationDateFilter = DateTime.MinValue;
            _startxRegistrationDateFilter = DateTime.MinValue;
            _endxRegistrationDateFilter = DateTime.MinValue;
            _startpExpiryDateFilter = DateTime.MinValue;
            _endpExpiryDateFilter = DateTime.MinValue;
            _endAssessmentDateFilter = DateTime.MinValue;
            _startAssessmentDateFilter = DateTime.MinValue;
        }

        
    }
    public partial class AllocationsModel: AsycudaSalesAndAdjustmentAllocationsExViewModel_AutoGen
    {
        private static readonly AllocationsModel instance;
        static AllocationsModel()
        {
            instance = new AllocationsModel(){DisableBaseFilterData = true};
        }

        public static AllocationsModel Instance
        {
            get { return instance; }
        }

        private AllocationsModel()
        {

            Process7100 = false;
            ApplyCurrentChecks = false;            
            //FilterData();

            RegisterToReceiveMessages<SalesDataDetail>(SalesDataQS.MessageToken.CurrentSalesDataDetailChanged, OnCurrentSalesDataDetailChanged);
            RegisterToReceiveMessages<PreviousDocumentItem>(PreviousDocumentQS.MessageToken.CurrentPreviousDocumentItemChanged, OnCurrentPreviousDocumentItemChanged);
            RegisterToReceiveMessages<PreviousDocument>(PreviousDocumentQS.MessageToken.CurrentPreviousDocumentChanged, OnCurrentPreviousDocumentChanged);
            RegisterToReceiveMessages<AsycudaSalesAndAdjustmentAllocationsEx>(MessageToken.CurrentAsycudaSalesAllocationsExChanged, OnCurrentAsycudaSalesAllocationsExChanged1);
            RegisterToReceiveMessages<AsycudaDocumentItem>(CoreEntities.MessageToken.CurrentAsycudaDocumentItemChanged, OnCurrentAsycudaDocumentItemChanged);
            RegisterToReceiveMessages<ApplicationSettings>(CoreEntities.MessageToken.CurrentApplicationSettingsChanged, OnCurrentApplicationSettingsChanged);


            this.PropertyChanged += AllocationsModel_PropertyChanged;
        }

        private void OnCurrentApplicationSettingsChanged(object sender, NotificationEventArgs<ApplicationSettings> e)
        {
            FilterData();
            
        }

        private void OnCurrentAsycudaSalesAllocationsExChanged1(object sender, NotificationEventArgs<AsycudaSalesAndAdjustmentAllocationsEx> e)
        {
            
        }

        void AllocationsModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Contains("Filter")) FilterData();
          
        }

        private void OnCurrentAsycudaDocumentItemChanged(object sender, NotificationEventArgs<AsycudaDocumentItem> e)
        {
            if (e.Data == null || e.Data.Item_Id == null)
            {
                vloader.FilterExpression = null;
            }
            else
            {
                vloader.FilterExpression = string.Format("xBond_Item_Id == {0} || PreviousItem_Id == {0}", e.Data.Item_Id.ToString());
            }
            
            AsycudaSalesAllocationsExs.Refresh();
            NotifyPropertyChanged(x => AsycudaSalesAllocationsExs);
        }

        private dynamic _AsycudaSalesAllocationsExs = null;
        public new dynamic AsycudaSalesAllocationsExs
        {
            get
            {
                return AsycudaSalesAndAdjustmentAllocationsExes;
            }
            set
            {
                AsycudaSalesAndAdjustmentAllocationsExes = value;
                NotifyPropertyChanged(x => x.AsycudaSalesAndAdjustmentAllocationsExes);
            }
        }

        private new void OnCurrentAsycudaSalesAllocationsExChanged1(object sender, NotificationEventArgs<AsycudaSalesAllocationsEx> e)
        {
       
        }

        private new void OnCurrentPreviousDocumentChanged(object sender, NotificationEventArgs<PreviousDocument> e)
        {
            if ( e.Data != null && PreviousDocumentItemsModel.Instance.ManualMode == false)
            {

                vloader.FilterExpression  = $"pASYCUDA_Id == {e.Data.ASYCUDA_Id.ToString()}";
                AsycudaSalesAllocationsExs.Refresh();
                NotifyPropertyChanged(x => AsycudaSalesAllocationsExs);
               
            }

        }

        private new void OnCurrentPreviousDocumentItemChanged(object sender, NotificationEventArgs<PreviousDocumentItem> e)
        {
           // if (BaseViewModel.Instance.CurrentAsycudaSalesAllocationsEx == null) return;
           if (e.Data != null && PreviousDocumentItemsModel.Instance.ManualMode == false )
            {
               //&& BaseViewModel.Instance.CurrentAsycudaSalesAllocationsEx.PreviousItem_Id != e.Data.Item_Id
                vloader.FilterExpression = $"PreviousItem_Id == {e.Data.Item_Id}";
                AsycudaSalesAllocationsExs.Refresh();
                NotifyPropertyChanged(x => AsycudaSalesAllocationsExs);
                

            }
        }

        private void OnCurrentSalesDataDetailChanged(object sender, NotificationEventArgs<SalesDataDetail> e)
        {
            if (e.Data != null && PreviousDocumentItemsModel.Instance.ManualMode == false)
            {
                // AllAllocations();
                vloader.FilterExpression = $"EntryDataDetailsId == {e.Data.EntryDataDetailsId}";
                AsycudaSalesAllocationsExs.Refresh();

                NotifyPropertyChanged(x => AsycudaSalesAllocationsExs);
                
            }
        }





   


         bool _viewIncompleteAllocations = false;
        public bool ViewIncompleteAllocations
        {
            get
            {
                return _viewIncompleteAllocations;
            }
            set
            {
                _viewIncompleteAllocations = value;
                if (_viewIncompleteAllocations == true)
                {
                    _viewXFilter = false;
                    _viewOPSFilter = false;
                    _viewERRFilter = false;
                }
                FilterData();
                NotifyPropertyChanged(x => ViewIncompleteAllocations);
            }
        }

         bool _viewERRFilter = false;
        public bool ViewERRFilter
        {
            get
            {
                return _viewERRFilter;
            }
            set
            {
                _viewERRFilter = value;
                if (_viewERRFilter == true)
                {
                    _viewXFilter = false;
                    _viewOPSFilter = false;
                    _viewIncompleteAllocations = false;
                }
                FilterData();
            }
        }

         bool _viewXFilter = false;
        public bool ViewXFilter
        {
            get
            {
                return _viewXFilter;
            }
            set
            {
                _viewXFilter = value;
                if (_viewXFilter == true)
                {
                    _viewERRFilter = false;
                    _viewOPSFilter = false;
                    _viewIncompleteAllocations = false;
                }
                FilterData();
                NotifyPropertyChanged(x => ViewXFilter);
            }
        }

        bool _viewNonXFilter = false;
        public bool ViewNonXFilter
        {
            get
            {
                return _viewNonXFilter;
            }
            set
            {
                _viewNonXFilter = value;
                if (_viewNonXFilter == true)
                {
                    _viewERRFilter = false;
                    _viewOPSFilter = false;
                    _viewXFilter = false;
                    _viewIncompleteAllocations = false;
                }
                FilterData();
                NotifyPropertyChanged(x => ViewNonXFilter);
            }
        }


         bool _viewOPSFilter = false;
        public bool ViewOPSFilter
        {
            get
            {
                return _viewOPSFilter;
            }
            set
            {
                _viewOPSFilter = value;
                if (_viewOPSFilter == true)
                {
                    _viewERRFilter = false;
                    _viewXFilter = false;
                    _viewIncompleteAllocations = false;
                    FilterData();
                }
                NotifyPropertyChanged(x => ViewOPSFilter);
                
            }
        }



         private bool _dutyFreeFilter = false;
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

         private bool _dutyPaidFilter = false;
        public bool DutyPaidFilter
        {
            get
            {
                return _dutyPaidFilter;
            }
            set
            {
                _dutyPaidFilter = value;

                FilterData();// FilterDutyFreePaid();
                NotifyPropertyChanged(x => DutyPaidFilter);
            }
        }


         bool _viewDirtyAllocations = false;
        public bool ViewDirtyAllocations
        {
            get
            {
                return _viewDirtyAllocations;
            }
            set
            {
                _viewDirtyAllocations = value;

                FilterData();// FilterDutyFreePaid();
                NotifyPropertyChanged(x => ViewDirtyAllocations);
            }
        }

        public override void FilterData()
        {
           var res = GetAutoPropertyFilterString();
            res.Append(" && " +
                       $"(ApplicationSettingsId == \"{CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}\")");


            if (string.IsNullOrEmpty(CNumberFilter) == false)
                res.Append(" && " + $"(pCNumber == \"{CNumberFilter}\")");

            if (string.IsNullOrEmpty(PrevLineFilter) == false)
                res.Append(" && " + $"(pLineNumber == \"{PrevLineFilter}\")");

            if (string.IsNullOrEmpty(ReferenceNumberFilter) == false)
                res.Append(" && " + $"(pReferenceNumber.Contains(\"{ReferenceNumberFilter}\"))");


            ApplyDutyFreePaidFilters(res);

            ApplyViewFilters(res);

            UpdateVLoaderFilterExpression(res);

            AsycudaSalesAllocationsExs.Refresh();
            NotifyPropertyChanged(x => this.AsycudaSalesAllocationsExs);
        }

        private void UpdateVLoaderFilterExpression(StringBuilder res)
        {
            if (res.Length == 0 && vloader.NavigationExpression.Count != 0) res.Append("&& All");
            if (res.Length > 0)
            {
                vloader.FilterExpression = res.ToString().Trim().Substring(2).Trim();
            }
            else
            {
                if (vloader.FilterExpression != "All") vloader.FilterExpression = null;
            }
        }

        private void ApplyDutyFreePaidFilters(StringBuilder res)
        {
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
        }

        private void ApplyViewFilters(StringBuilder res)
        {
            if (ViewIncompleteAllocations || ViewXFilter || ViewNonXFilter || ViewERRFilter || ViewOPSFilter || ViewDirtyAllocations)
                res.Append("&& (");
            if (ViewIncompleteAllocations == true)
            {
                res.Append("InvoiceNo != null" +
                           "&& SalesQtyAllocated != 0" +
                           "&& SalesQtyAllocated != SalesQuantity" +
                           "&& Status == null");
                
            }
            if (ViewXFilter == true)
            {
                
                res.Append("PreviousItem_Id != null" +
                    "&& (xBond_Item_Id == 0)" +
                    "&& (pIsAssessed == true)" +
                    "&& (QtyAllocated != null && EntryDataDetailsId != null)" +
                    //"&& (pRegistrationDate != DateTime.MinValue)" +
                    //"&& (pCNumber != null)" +
                    "&& (PiQuantity < pQtyAllocated)" +
                    "&& (Status == null || Status == \"\")" +
                    (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.AllowNonXEntries == "Visible"? $"&& (Invalid != true && (pExpiryDate >= \"{DateTime.Now.ToShortDateString()}\" || pExpiryDate == null) && (Status == null || Status == \"\"))"
                        : ""));
                //&& (PreviousEntry.AsycudaDocument.IsManuallyAssessed == null
                //    || PreviousEntry.AsycudaDocument.IsManuallyAssessed != true))
               
            }

            if (ViewNonXFilter == true)
            {
                
                res.Append($"Invalid == true || (pExpiryDate < \"{DateTime.Now.ToShortDateString()}\")");
                //&& (PreviousEntry.AsycudaDocument.IsManuallyAssessed == null
                //    || PreviousEntry.AsycudaDocument.IsManuallyAssessed != true))

            }

            if (ViewERRFilter == true)
            {
                //res.Append("PreviousItem_Id == 0" +
                //           "&& SalesQtyAllocated == 0" +
                //           "&& SalesQuantity > 0 " +
                //           "&& Cost > 0");
                res.Append("Status != null");

            }
            if (ViewOPSFilter == true)
            {
                res.Append("PreviousItem_Id == 0" +
                           "&& SalesQtyAllocated == 0 " +
                           "&& SalesQuantity > 0 " +
                           "&& Cost > 0");
                
            }
            if (ViewDirtyAllocations == true)
            {
                res.Append("(DoNotAllocateSales == null || DoNotAllocateSales != true)" +
                           "&& PreviousItem_Id != null" +
                           "&& (pQtyAllocated - QtyAllocated) != PiQuantity");
                
            }

            if (ViewIncompleteAllocations || ViewXFilter || ViewNonXFilter || ViewERRFilter || ViewOPSFilter || ViewDirtyAllocations)
                res.Append(")");
        }

        string _referenceNumberFilter = "";
        public  string ReferenceNumberFilter
        {
            get
            {
                return _referenceNumberFilter;
            }
            set
            {
                _referenceNumberFilter = value;
                FilterData();
                NotifyPropertyChanged(x => ReferenceNumberFilter);
            }
        }


         string _cNumberFilter = "";
        public string CNumberFilter
        {
            get
            {
                return _cNumberFilter;
            }
            set
            {
                _cNumberFilter = value;
                FilterData();
                NotifyPropertyChanged(x => CNumberFilter);
            }
        }
        
         string _prevLineFilter = "";
        public string PrevLineFilter
        {
            get
            {
                return _prevLineFilter;
            }
            set
            {
                _prevLineFilter = value;
                FilterData();
            }
        }

     
        private  bool _process7100 = false;
        public bool Process7100
        {
            get { return _process7100; }

            set
            {
                _process7100 = value;
                if (_process7100 == true)
                {
                   
                    NotifyPropertyChanged(x => Process7100);
                }
            }
        }

        public bool ApplyCurrentChecks { get; set; }
        public  bool PerIM7 { get; set; }



        public async Task GoToxBondEntry(string xBond_Item_Id)
        {
            
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentItem = AsycudaDocumentItemRepository.Instance.GetAsycudaDocumentItem(xBond_Item_Id).Result;
                    Core.Common.UI.BaseViewModel.Slider.MoveTo("AsycudaDocumentsExP");
                });
               
           
        }



        internal async Task CreateEx9()
        {
            StatusModel.Timer("Creating Ex9");
            if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx == null)
            {
                MessageBox.Show("Please select a Document Set.");
                return;
            }
            await AsycudaSalesAllocationsExRepository.Instance.CreateEx9(vloader.FilterExpression + ($" && pRegistrationDate >= \"{CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.OpeningStockDate}\"")
                  ,
                            PerIM7,
                            Process7100,
                            ApplyCurrentChecks, CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId, "Sales","Historic",true, true, true, true, true, false,true, true, true).ConfigureAwait(false);
           MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged, this, new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged));
          StatusModel.StopStatusUpdate();
            MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
           
        }

        internal async Task CreateOPS()
        {
            StatusModel.Timer("Creating OPS");
            await AsycudaSalesAllocationsExRepository.Instance.CreateOPS(vloader.FilterExpression, CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId).ConfigureAwait(false);
           
            ViewAll();
            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentsChanged, null,
                new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentsChanged));

            StatusModel.StopStatusUpdate();
            MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            
        }

        internal async Task CreateErrorOPS()
        {
            StatusModel.Timer("Creating Error OPS");

            if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx == null)
            {
                MessageBox.Show("Please Select a Asycuda Document Set before proceding");
                return;
            }

            await
                AsycudaSalesAllocationsExRepository.Instance.CreateOPS(vloader.FilterExpression,
                    CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId).ConfigureAwait(false);

           ViewAll();

            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentsChanged, null,
                new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentsChanged));


            MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            StatusModel.StopStatusUpdate();
        }

        internal async Task ManuallyAllocate(AsycudaSalesAndAdjustmentAllocationsEx callo, PreviousDocumentItem previousEntry)
        {
            
            if (callo.ItemNumber != previousEntry.ItemNumber) //BaseViewModel.Instance.CurrentAsycudaSalesAllocationsEx
            {
                var res = MessageBox.Show(
                    $"The Sales ItemNumber '{callo.ItemNumber}' is not the same as Previous Entry ItemNumber '{previousEntry.ItemNumber}' do you want to continue?"
                    , "Inventory Item Mis-match", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.No) return;
            }

            if (Convert.ToDouble(callo.SalesQuantity) > (previousEntry.ItemQuantity - previousEntry.QtyAllocated))
            {
                var res = MessageBox.Show(
                    $"The Sales Quantity '{callo.SalesQuantity.ToString()}' is larger than Previous Entry Quantity Available to allocate '{(previousEntry.ItemQuantity - previousEntry.QtyAllocated).ToString()}' do you want to continue with partial fill?"
                    , "Partial Allocation", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.No) return;
            }

           await AsycudaSalesAllocationsExRepository.Instance.ManuallyAllocate(callo.AllocationId, previousEntry.Item_Id).ConfigureAwait(false);

            MessageBus.Default.BeginNotify(QuerySpace.AllocationQS.MessageToken.AsycudaSalesAllocationsExsChanged, null,
                                           new NotificationEventArgs(QuerySpace.AllocationQS.MessageToken.AsycudaSalesAllocationsExsChanged));   
        }

        internal async Task CreateIncompOPS()
        {
            StatusModel.Timer("Creating Create Incomplete OPS");
            StatusModel.Timer("Creating Error OPS");

            if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx == null)
            {
                MessageBox.Show("Please Select a Asycuda Document Set before proceding");
                return;
            }

            await AsycudaSalesAllocationsExRepository.Instance.CreateIncompOPS(vloader.FilterExpression,
                CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId).ConfigureAwait(false);

            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentsChanged, null,
                new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentsChanged));

            StatusModel.StopStatusUpdate();
            MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            
        }

        public async Task ReBuildSalesReports()
        {
            StatusModel.Timer("Rebuilding Sales Reports");
            await AsycudaSalesAllocationsExRepository.Instance.ReBuildSalesReports().ConfigureAwait(false);
            StatusModel.StopStatusUpdate();

           
            MessageBus.Default.BeginNotify(QuerySpace.AllocationQS.MessageToken.AsycudaSalesAllocationsExsChanged, null,
                                        new NotificationEventArgs(QuerySpace.AllocationQS.MessageToken.AsycudaSalesAllocationsExsChanged));
            
            StatusModel.StopStatusUpdate();
            MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            

        }

        public async Task AllocateSales(bool allocateToLastAdjustment)
        {
            StatusModel.Timer("Allocating Sales");

            var res = MessageBox.Show("Clear Allocations?", "Delete Existing Sales Allocations", MessageBoxButton.YesNoCancel);
            if (res == MessageBoxResult.Yes)
            {
                await AsycudaSalesAllocationsExRepository.Instance.ClearAllocations(CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ConfigureAwait(false);
                
            }

            if (res == MessageBoxResult.Cancel)
            {
                StatusModel.StopStatusUpdate();
                return;
            }

            await AsycudaSalesAllocationsExRepository.Instance.AllocateSales(
                CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings, allocateToLastAdjustment).ConfigureAwait(false);

            MessageBus.Default.BeginNotify(MessageToken.AsycudaSalesAllocationsExsChanged, null,
                        new NotificationEventArgs(MessageToken.AsycudaSalesAllocationsExsChanged));

            MessageBus.Default.BeginNotify(QuerySpace.CoreEntities.MessageToken.AsycudaDocumentsChanged, null,
                        new NotificationEventArgs(QuerySpace.CoreEntities.MessageToken.AsycudaDocumentsChanged));

            MessageBus.Default.BeginNotify(QuerySpace.PreviousDocumentQS.MessageToken.PreviousDocumentItemsChanged, null,
                        new NotificationEventArgs(QuerySpace.PreviousDocumentQS.MessageToken.PreviousDocumentItemsChanged));

            StatusModel.StopStatusUpdate();
            MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            
        }

        public async Task ClearAllocations(bool all)
        {
              var res = MessageBox.Show("Are you Sure you Want to Delete " + (all == true ? "All" : "Selected") + " Sales Allocations?", "Clear Sales Allocations", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {

                if (all)
                {
                    await AsycudaSalesAllocationsExRepository.Instance.ClearAllocations(vloader.FilterExpression).ConfigureAwait(false);
                }
                else
                {
                    //await AsycudaSalesAllocationsExRepository.Instance.ClearAllocations(SelectedAsycudaSalesAllocationsExs).ConfigureAwait(false);
                }
            
            MessageBus.Default.BeginNotify(QuerySpace.EntryDataQS.MessageToken.EntryDataDetailsExesChanged, null,
                                        new NotificationEventArgs(QuerySpace.EntryDataQS.MessageToken.EntryDataDetailsExesChanged));

            MessageBus.Default.BeginNotify(QuerySpace.AllocationQS.MessageToken.AsycudaSalesAllocationsExsChanged, null,
                                        new NotificationEventArgs(QuerySpace.AllocationQS.MessageToken.AsycudaSalesAllocationsExsChanged));


            MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public async Task EX9AllSales(bool overwrite)
        {
            try
            {
                

                //var saleInfo = BaseDataModel.CurrentSalesInfo();
                //if (saleInfo.Item3.AsycudaDocumentSetId == 0) return;
                var res = MessageBox.Show("Ex9 All Allocated Sales?", "Ex9 All Allocated Sales", MessageBoxButton.YesNoCancel);
                if (res == MessageBoxResult.Yes)
                {
                    StatusModel.Timer("Ex9 All Allocated Sales");
                   
                     var filterExpression = vloader.FilterExpression +
                    // "&& (ItemNumber == \"WA99004\")" +//A002416,A002402,X35019044,AB111510
                    // "&&  PreviousItem_Id == 388376" +
                    " && PreviousItem_Id != null" +
                    //"&& (xBond_Item_Id == 0)" + not relevant because it could be assigned to another sale but not exwarehoused
                    " && (QtyAllocated != null && EntryDataDetailsId != null)" +
                    " && (PiQuantity < pQtyAllocated)" +
                    //"&& (pQuantity - pQtyAllocated  < 0.001)" + // prevents spill over allocations
                    " && (Status == null || Status == \"\")" +
                    (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.AllowNonXEntries == "Visible"
                        ? $" && (Invalid != true && (pExpiryDate >= \"{DateTime.Now.ToShortDateString()}\" || pExpiryDate == null) && (Status == null || Status == \"\"))"
                        : "") +
                    ($" && pRegistrationDate >= \"{CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.OpeningStockDate}\"");



                    await AsycudaSalesAllocationsExRepository.Instance.CreateEx9(filterExpression, false, false, false, CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId, "Sales", "Historic", true, true, true, false, false, false, true, true, true).ConfigureAwait(false);

                }

                if (res == MessageBoxResult.Cancel)
                {
                    StatusModel.StopStatusUpdate();
                    return;
                }

               
                MessageBus.Default.BeginNotify(MessageToken.AsycudaSalesAllocationsExsChanged, null,
                    new NotificationEventArgs(MessageToken.AsycudaSalesAllocationsExsChanged));

                MessageBus.Default.BeginNotify(QuerySpace.CoreEntities.MessageToken.AsycudaDocumentsChanged, null,
                    new NotificationEventArgs(QuerySpace.CoreEntities.MessageToken.AsycudaDocumentsChanged));

                MessageBus.Default.BeginNotify(QuerySpace.PreviousDocumentQS.MessageToken.PreviousDocumentItemsChanged, null,
                    new NotificationEventArgs(QuerySpace.PreviousDocumentQS.MessageToken.PreviousDocumentItemsChanged));
                

                StatusModel.StopStatusUpdate();
                MessageBox.Show("Complete", "Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}