﻿// <autogenerated>
//   This file was generated by T4 code generator AllViewModels.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using SimpleMvvmToolkit;
using System;
using System.Windows;
using System.Windows.Data;
using System.Text;
using Core.Common.UI.DataVirtualization;
using System.Collections.Generic;
using Core.Common.UI;
using Core.Common.Converters;

using InventoryQS.Client.Entities;
using InventoryQS.Client.Repositories;
//using WaterNut.Client.Repositories;
        
using CoreEntities.Client.Entities;


namespace WaterNut.QuerySpace.InventoryQS.ViewModels
{
    
	public partial class EntryDataDetailsExViewModel_AutoGen : ViewModelBase<EntryDataDetailsExViewModel_AutoGen>
	{

       private static readonly EntryDataDetailsExViewModel_AutoGen instance;
       static EntryDataDetailsExViewModel_AutoGen()
        {
            instance = new EntryDataDetailsExViewModel_AutoGen();
        }

       public static EntryDataDetailsExViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public EntryDataDetailsExViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<EntryDataDetailsEx>(MessageToken.CurrentEntryDataDetailsExChanged, OnCurrentEntryDataDetailsExChanged);
            RegisterToReceiveMessages(MessageToken.EntryDataDetailsExChanged, OnEntryDataDetailsExChanged);
			RegisterToReceiveMessages(MessageToken.EntryDataDetailsExFilterExpressionChanged, OnEntryDataDetailsExFilterExpressionChanged);

 
			RegisterToReceiveMessages<InventoryItemsEx>(MessageToken.CurrentInventoryItemsExChanged, OnCurrentInventoryItemsExChanged);

 			// Recieve messages for Core Current Entities Changed
                        RegisterToReceiveMessages<AsycudaDocumentSet>(CoreEntities.MessageToken.CurrentAsycudaDocumentSetChanged, OnCurrentAsycudaDocumentSetChanged);
                        RegisterToReceiveMessages<ApplicationSettings>(CoreEntities.MessageToken.CurrentApplicationSettingsChanged, OnCurrentApplicationSettingsChanged);
 

			EntryDataDetailsEx = new VirtualList<EntryDataDetailsEx>(vloader);
			EntryDataDetailsEx.LoadingStateChanged += EntryDataDetailsEx_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(EntryDataDetailsEx, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<EntryDataDetailsEx> _EntryDataDetailsEx = null;
        public VirtualList<EntryDataDetailsEx> EntryDataDetailsEx
        {
            get
            {
                return _EntryDataDetailsEx;
            }
            set
            {
                _EntryDataDetailsEx = value;
                NotifyPropertyChanged( x => x.EntryDataDetailsEx);
            }
        }

		 private void OnEntryDataDetailsExFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => EntryDataDetailsEx.Refresh()).ConfigureAwait(false);
            SelectedEntryDataDetailsEx.Clear();
            NotifyPropertyChanged(x => SelectedEntryDataDetailsEx);
            BeginSendMessage(MessageToken.SelectedEntryDataDetailsExChanged, new NotificationEventArgs(MessageToken.SelectedEntryDataDetailsExChanged));
        }

		void EntryDataDetailsEx_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (EntryDataDetailsEx.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => EntryDataDetailsEx);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("EntryDataDetailsEx | Error occured..." + EntryDataDetailsEx.LastLoadingError.Message);
                    NotifyPropertyChanged(x => EntryDataDetailsEx);
                    break;
            }
           
        }

		
		public readonly EntryDataDetailsExVirturalListLoader vloader = new EntryDataDetailsExVirturalListLoader();

		private ObservableCollection<EntryDataDetailsEx> _selectedEntryDataDetailsEx = new ObservableCollection<EntryDataDetailsEx>();
        public ObservableCollection<EntryDataDetailsEx> SelectedEntryDataDetailsEx
        {
            get
            {
                return _selectedEntryDataDetailsEx;
            }
            set
            {
                _selectedEntryDataDetailsEx = value;
				BeginSendMessage(MessageToken.SelectedEntryDataDetailsExChanged,
                                    new NotificationEventArgs(MessageToken.SelectedEntryDataDetailsExChanged));
				 NotifyPropertyChanged(x => SelectedEntryDataDetailsEx);
            }
        }

        internal virtual void OnCurrentEntryDataDetailsExChanged(object sender, NotificationEventArgs<EntryDataDetailsEx> e)
        {
            if(BaseViewModel.Instance.CurrentEntryDataDetailsEx != null) BaseViewModel.Instance.CurrentEntryDataDetailsEx.PropertyChanged += CurrentEntryDataDetailsEx__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentEntryDataDetailsEx);
        }   

            void CurrentEntryDataDetailsEx__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    //if (e.PropertyName == "AddInventoryItemsEx")
                   // {
                   //    if(InventoryItemsEx.Contains(CurrentEntryDataDetailsEx.InventoryItemsEx) == false) InventoryItemsEx.Add(CurrentEntryDataDetailsEx.InventoryItemsEx);
                    //}
                    //if (e.PropertyName == "AddAsycudaDocumentSet")
                   // {
                   //    if(AsycudaDocumentSet.Contains(CurrentEntryDataDetailsEx.AsycudaDocumentSet) == false) AsycudaDocumentSet.Add(CurrentEntryDataDetailsEx.AsycudaDocumentSet);
                    //}
                    //if (e.PropertyName == "AddApplicationSettings")
                   // {
                   //    if(ApplicationSettings.Contains(CurrentEntryDataDetailsEx.ApplicationSettings) == false) ApplicationSettings.Add(CurrentEntryDataDetailsEx.ApplicationSettings);
                    //}
                 } 
        internal virtual void OnEntryDataDetailsExChanged(object sender, NotificationEventArgs e)
        {
            _EntryDataDetailsEx.Refresh();
			NotifyPropertyChanged(x => this.EntryDataDetailsEx);
        }   


 	
		 internal virtual void OnCurrentInventoryItemsExChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<InventoryItemsEx> e)
			{
			if(ViewCurrentInventoryItemsEx == false) return;
			if (e.Data == null || e.Data.InventoryItemId == null)
                {
                    vloader.FilterExpression = "None";
                }
                else
                {
				vloader.FilterExpression = string.Format("InventoryItemId == {0}", e.Data.InventoryItemId.ToString());
                 }

				EntryDataDetailsEx.Refresh();
				NotifyPropertyChanged(x => this.EntryDataDetailsEx);
                // SendMessage(MessageToken.EntryDataDetailsExChanged, new NotificationEventArgs(MessageToken.EntryDataDetailsExChanged));
                                          
                BaseViewModel.Instance.CurrentEntryDataDetailsEx = null;
			}

  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
                internal virtual void OnCurrentAsycudaDocumentSetChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<AsycudaDocumentSet> e)
				{
				if (e.Data == null || e.Data.AsycudaDocumentSetId == null)
                {
                    vloader.FilterExpression = null;
                }
                else
                {
                    vloader.FilterExpression = string.Format("AsycudaDocumentSetId == {0}", e.Data.AsycudaDocumentSetId.ToString());
                }
					
                    EntryDataDetailsEx.Refresh();
					NotifyPropertyChanged(x => this.EntryDataDetailsEx);
				}
                internal virtual void OnCurrentApplicationSettingsChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<ApplicationSettings> e)
				{
				if (e.Data == null || e.Data.ApplicationSettingsId == null)
                {
                    vloader.FilterExpression = null;
                }
                else
                {
                    vloader.FilterExpression = string.Format("ApplicationSettingsId == {0}", e.Data.ApplicationSettingsId.ToString());
                }
					
                    EntryDataDetailsEx.Refresh();
					NotifyPropertyChanged(x => this.EntryDataDetailsEx);
				}
  
// Filtering Each Field except IDs
 	
		 bool _viewCurrentInventoryItemsEx = false;
         public bool ViewCurrentInventoryItemsEx
         {
             get
             {
                 return _viewCurrentInventoryItemsEx;
             }
             set
             {
                 _viewCurrentInventoryItemsEx = value;
                 NotifyPropertyChanged(x => x.ViewCurrentInventoryItemsEx);
                FilterData();
             }
         }
		public void ViewAll()
        {
			vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";




			vloader.ClearNavigationExpression();
			_EntryDataDetailsEx.Refresh();
			NotifyPropertyChanged(x => this.EntryDataDetailsEx);
		}

		public async Task SelectAll()
        {
            IEnumerable<EntryDataDetailsEx> lst = null;
            using (var ctx = new EntryDataDetailsExRepository())
            {
                lst = await ctx.GetEntryDataDetailsExByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedEntryDataDetailsEx = new ObservableCollection<EntryDataDetailsEx>(lst);
        }

 

		private string _entryDataIdFilter;
        public string EntryDataIdFilter
        {
            get
            {
                return _entryDataIdFilter;
            }
            set
            {
                _entryDataIdFilter = value;
				NotifyPropertyChanged(x => EntryDataIdFilter);
                FilterData();
                
            }
        }	

 

		private Int32? _lineNumberFilter;
        public Int32? LineNumberFilter
        {
            get
            {
                return _lineNumberFilter;
            }
            set
            {
                _lineNumberFilter = value;
				NotifyPropertyChanged(x => LineNumberFilter);
                FilterData();
                
            }
        }	

 

		private string _itemNumberFilter;
        public string ItemNumberFilter
        {
            get
            {
                return _itemNumberFilter;
            }
            set
            {
                _itemNumberFilter = value;
				NotifyPropertyChanged(x => ItemNumberFilter);
                FilterData();
                
            }
        }	

 

		private Single? _quantityFilter;
        public Single? QuantityFilter
        {
            get
            {
                return _quantityFilter;
            }
            set
            {
                _quantityFilter = value;
				NotifyPropertyChanged(x => QuantityFilter);
                FilterData();
                
            }
        }	

 

		private string _unitsFilter;
        public string UnitsFilter
        {
            get
            {
                return _unitsFilter;
            }
            set
            {
                _unitsFilter = value;
				NotifyPropertyChanged(x => UnitsFilter);
                FilterData();
                
            }
        }	

 

		private string _itemDescriptionFilter;
        public string ItemDescriptionFilter
        {
            get
            {
                return _itemDescriptionFilter;
            }
            set
            {
                _itemDescriptionFilter = value;
				NotifyPropertyChanged(x => ItemDescriptionFilter);
                FilterData();
                
            }
        }	

 

		private Single? _costFilter;
        public Single? CostFilter
        {
            get
            {
                return _costFilter;
            }
            set
            {
                _costFilter = value;
				NotifyPropertyChanged(x => CostFilter);
                FilterData();
                
            }
        }	

 

		private Double? _qtyAllocatedFilter;
        public Double? QtyAllocatedFilter
        {
            get
            {
                return _qtyAllocatedFilter;
            }
            set
            {
                _qtyAllocatedFilter = value;
				NotifyPropertyChanged(x => QtyAllocatedFilter);
                FilterData();
                
            }
        }	

 

		private Single? _unitWeightFilter;
        public Single? UnitWeightFilter
        {
            get
            {
                return _unitWeightFilter;
            }
            set
            {
                _unitWeightFilter = value;
				NotifyPropertyChanged(x => UnitWeightFilter);
                FilterData();
                
            }
        }	

 

		private Boolean? _doNotAllocateFilter;
        public Boolean? DoNotAllocateFilter
        {
            get
            {
                return _doNotAllocateFilter;
            }
            set
            {
                _doNotAllocateFilter = value;
				NotifyPropertyChanged(x => DoNotAllocateFilter);
                FilterData();
                
            }
        }	

 

		private string _tariffCodeFilter;
        public string TariffCodeFilter
        {
            get
            {
                return _tariffCodeFilter;
            }
            set
            {
                _tariffCodeFilter = value;
				NotifyPropertyChanged(x => TariffCodeFilter);
                FilterData();
                
            }
        }	

 

		private string _cNumberFilter;
        public string CNumberFilter
        {
            get
            {
                return _cNumberFilter;
            }
            set
            {
                _cNumberFilter = value;
				NotifyPropertyChanged(x => CNumberFilter);
                FilterData();
                
            }
        }	

 

		private Int32? _cLineNumberFilter;
        public Int32? CLineNumberFilter
        {
            get
            {
                return _cLineNumberFilter;
            }
            set
            {
                _cLineNumberFilter = value;
				NotifyPropertyChanged(x => CLineNumberFilter);
                FilterData();
                
            }
        }	

 

		private Boolean? _downloadedFilter;
        public Boolean? DownloadedFilter
        {
            get
            {
                return _downloadedFilter;
            }
            set
            {
                _downloadedFilter = value;
				NotifyPropertyChanged(x => DownloadedFilter);
                FilterData();
                
            }
        }	

 

		private string _dutyFreePaidFilter;
        public string DutyFreePaidFilter
        {
            get
            {
                return _dutyFreePaidFilter;
            }
            set
            {
                _dutyFreePaidFilter = value;
				NotifyPropertyChanged(x => DutyFreePaidFilter);
                FilterData();
                
            }
        }	

 

		private Single? _totalFilter;
        public Single? TotalFilter
        {
            get
            {
                return _totalFilter;
            }
            set
            {
                _totalFilter = value;
				NotifyPropertyChanged(x => TotalFilter);
                FilterData();
                
            }
        }	

 

		private Double? _invoiceQtyFilter;
        public Double? InvoiceQtyFilter
        {
            get
            {
                return _invoiceQtyFilter;
            }
            set
            {
                _invoiceQtyFilter = value;
				NotifyPropertyChanged(x => InvoiceQtyFilter);
                FilterData();
                
            }
        }	

 

		private Double? _receivedQtyFilter;
        public Double? ReceivedQtyFilter
        {
            get
            {
                return _receivedQtyFilter;
            }
            set
            {
                _receivedQtyFilter = value;
				NotifyPropertyChanged(x => ReceivedQtyFilter);
                FilterData();
                
            }
        }	

 

		private string _statusFilter;
        public string StatusFilter
        {
            get
            {
                return _statusFilter;
            }
            set
            {
                _statusFilter = value;
				NotifyPropertyChanged(x => StatusFilter);
                FilterData();
                
            }
        }	

 

		private string _previousInvoiceNumberFilter;
        public string PreviousInvoiceNumberFilter
        {
            get
            {
                return _previousInvoiceNumberFilter;
            }
            set
            {
                _previousInvoiceNumberFilter = value;
				NotifyPropertyChanged(x => PreviousInvoiceNumberFilter);
                FilterData();
                
            }
        }	

 

		private string _previousCNumberFilter;
        public string PreviousCNumberFilter
        {
            get
            {
                return _previousCNumberFilter;
            }
            set
            {
                _previousCNumberFilter = value;
				NotifyPropertyChanged(x => PreviousCNumberFilter);
                FilterData();
                
            }
        }	

 

		private string _commentFilter;
        public string CommentFilter
        {
            get
            {
                return _commentFilter;
            }
            set
            {
                _commentFilter = value;
				NotifyPropertyChanged(x => CommentFilter);
                FilterData();
                
            }
        }	

 
		private DateTime? _startEffectiveDateFilter = DateTime.Parse(string.Format("{0}/1/{1}", DateTime.Now.Month ,DateTime.Now.Year));
        public DateTime? StartEffectiveDateFilter
        {
            get
            {
                return _startEffectiveDateFilter;
            }
            set
            {
                _startEffectiveDateFilter = value;
				NotifyPropertyChanged(x => StartEffectiveDateFilter);
                FilterData();
                
            }
        }	

		private DateTime? _endEffectiveDateFilter = DateTime.Parse(string.Format("{1}/{0}/{2}", DateTime.DaysInMonth( DateTime.Now.Year,DateTime.Now.Month), DateTime.Now.Month, DateTime.Now.Year));
        public DateTime? EndEffectiveDateFilter
        {
            get
            {
                return _endEffectiveDateFilter;
            }
            set
            {
                _endEffectiveDateFilter = value;
				NotifyPropertyChanged(x => EndEffectiveDateFilter);
                FilterData();
                
            }
        }
 

		private DateTime? _effectiveDateFilter;
        public DateTime? EffectiveDateFilter
        {
            get
            {
                return _effectiveDateFilter;
            }
            set
            {
                _effectiveDateFilter = value;
				NotifyPropertyChanged(x => EffectiveDateFilter);
                FilterData();
                
            }
        }	

 

		private Boolean? _isReconciledFilter;
        public Boolean? IsReconciledFilter
        {
            get
            {
                return _isReconciledFilter;
            }
            set
            {
                _isReconciledFilter = value;
				NotifyPropertyChanged(x => IsReconciledFilter);
                FilterData();
                
            }
        }	

 

		private Double? _lastCostFilter;
        public Double? LastCostFilter
        {
            get
            {
                return _lastCostFilter;
            }
            set
            {
                _lastCostFilter = value;
				NotifyPropertyChanged(x => LastCostFilter);
                FilterData();
                
            }
        }	

 

		private Double? _taxAmountFilter;
        public Double? TaxAmountFilter
        {
            get
            {
                return _taxAmountFilter;
            }
            set
            {
                _taxAmountFilter = value;
				NotifyPropertyChanged(x => TaxAmountFilter);
                FilterData();
                
            }
        }	

 

		private string _emailIdFilter;
        public string EmailIdFilter
        {
            get
            {
                return _emailIdFilter;
            }
            set
            {
                _emailIdFilter = value;
				NotifyPropertyChanged(x => EmailIdFilter);
                FilterData();
                
            }
        }	

 

		private string _nameFilter;
        public string NameFilter
        {
            get
            {
                return _nameFilter;
            }
            set
            {
                _nameFilter = value;
				NotifyPropertyChanged(x => NameFilter);
                FilterData();
                
            }
        }	

 

		private Int32? _fileLineNumberFilter;
        public Int32? FileLineNumberFilter
        {
            get
            {
                return _fileLineNumberFilter;
            }
            set
            {
                _fileLineNumberFilter = value;
				NotifyPropertyChanged(x => FileLineNumberFilter);
                FilterData();
                
            }
        }	

 

		private Double? _volumeLitersFilter;
        public Double? VolumeLitersFilter
        {
            get
            {
                return _volumeLitersFilter;
            }
            set
            {
                _volumeLitersFilter = value;
				NotifyPropertyChanged(x => VolumeLitersFilter);
                FilterData();
                
            }
        }	

 

		private Int32? _previousCLineNumberFilter;
        public Int32? PreviousCLineNumberFilter
        {
            get
            {
                return _previousCLineNumberFilter;
            }
            set
            {
                _previousCLineNumberFilter = value;
				NotifyPropertyChanged(x => PreviousCLineNumberFilter);
                FilterData();
                
            }
        }	

 
		internal bool DisableBaseFilterData = false;
        public virtual void FilterData()
	    {
	        FilterData(null);
	    }
		public void FilterData(StringBuilder res = null)
		{
		    if (DisableBaseFilterData) return;
			if(res == null) res = GetAutoPropertyFilterString();
			if (res.Length == 0 && vloader.NavigationExpression.Count != 0) res.Append("&& All");					
			if (res.Length > 0)
            {
                vloader.FilterExpression = res.ToString().Trim().Substring(2).Trim();
            }
            else
            {
                 if (vloader.FilterExpression != "All") vloader.FilterExpression = null;
            }

			EntryDataDetailsEx.Refresh();
			NotifyPropertyChanged(x => this.EntryDataDetailsEx);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(EntryDataIdFilter) == false)
						res.Append(" && " + string.Format("EntryDataId.Contains(\"{0}\")",  EntryDataIdFilter));						
 

					if(LineNumberFilter.HasValue)
						res.Append(" && " + string.Format("LineNumber == {0}",  LineNumberFilter.ToString()));				 

									if(string.IsNullOrEmpty(ItemNumberFilter) == false)
						res.Append(" && " + string.Format("ItemNumber.Contains(\"{0}\")",  ItemNumberFilter));						
 

					if(QuantityFilter.HasValue)
						res.Append(" && " + string.Format("Quantity == {0}",  QuantityFilter.ToString()));				 

									if(string.IsNullOrEmpty(UnitsFilter) == false)
						res.Append(" && " + string.Format("Units.Contains(\"{0}\")",  UnitsFilter));						
 

									if(string.IsNullOrEmpty(ItemDescriptionFilter) == false)
						res.Append(" && " + string.Format("ItemDescription.Contains(\"{0}\")",  ItemDescriptionFilter));						
 

					if(CostFilter.HasValue)
						res.Append(" && " + string.Format("Cost == {0}",  CostFilter.ToString()));				 

					if(QtyAllocatedFilter.HasValue)
						res.Append(" && " + string.Format("QtyAllocated == {0}",  QtyAllocatedFilter.ToString()));				 

					if(UnitWeightFilter.HasValue)
						res.Append(" && " + string.Format("UnitWeight == {0}",  UnitWeightFilter.ToString()));				 

									if(DoNotAllocateFilter.HasValue)
						res.Append(" && " + string.Format("DoNotAllocate == {0}",  DoNotAllocateFilter));						
 

									if(string.IsNullOrEmpty(TariffCodeFilter) == false)
						res.Append(" && " + string.Format("TariffCode.Contains(\"{0}\")",  TariffCodeFilter));						
 

									if(string.IsNullOrEmpty(CNumberFilter) == false)
						res.Append(" && " + string.Format("CNumber.Contains(\"{0}\")",  CNumberFilter));						
 

					if(CLineNumberFilter.HasValue)
						res.Append(" && " + string.Format("CLineNumber == {0}",  CLineNumberFilter.ToString()));				 

									if(DownloadedFilter.HasValue)
						res.Append(" && " + string.Format("Downloaded == {0}",  DownloadedFilter));						
 

									if(string.IsNullOrEmpty(DutyFreePaidFilter) == false)
						res.Append(" && " + string.Format("DutyFreePaid.Contains(\"{0}\")",  DutyFreePaidFilter));						
 

					if(TotalFilter.HasValue)
						res.Append(" && " + string.Format("Total == {0}",  TotalFilter.ToString()));				 

					if(InvoiceQtyFilter.HasValue)
						res.Append(" && " + string.Format("InvoiceQty == {0}",  InvoiceQtyFilter.ToString()));				 

					if(ReceivedQtyFilter.HasValue)
						res.Append(" && " + string.Format("ReceivedQty == {0}",  ReceivedQtyFilter.ToString()));				 

									if(string.IsNullOrEmpty(StatusFilter) == false)
						res.Append(" && " + string.Format("Status.Contains(\"{0}\")",  StatusFilter));						
 

									if(string.IsNullOrEmpty(PreviousInvoiceNumberFilter) == false)
						res.Append(" && " + string.Format("PreviousInvoiceNumber.Contains(\"{0}\")",  PreviousInvoiceNumberFilter));						
 

									if(string.IsNullOrEmpty(PreviousCNumberFilter) == false)
						res.Append(" && " + string.Format("PreviousCNumber.Contains(\"{0}\")",  PreviousCNumberFilter));						
 

									if(string.IsNullOrEmpty(CommentFilter) == false)
						res.Append(" && " + string.Format("Comment.Contains(\"{0}\")",  CommentFilter));						
 

 

				if (Convert.ToDateTime(StartEffectiveDateFilter).Date != DateTime.MinValue &&
		        Convert.ToDateTime(EndEffectiveDateFilter).Date != DateTime.MinValue) res.Append(" && (");

					if (Convert.ToDateTime(StartEffectiveDateFilter).Date != DateTime.MinValue)
						{
							if(StartEffectiveDateFilter.HasValue)
								res.Append(
                                            (Convert.ToDateTime(EndEffectiveDateFilter).Date != DateTime.MinValue?"":" && ") +
                                            string.Format("EffectiveDate >= \"{0}\"",  Convert.ToDateTime(StartEffectiveDateFilter).Date.ToString("MM/dd/yyyy")));
						}

					if (Convert.ToDateTime(EndEffectiveDateFilter).Date != DateTime.MinValue)
						{
							if(EndEffectiveDateFilter.HasValue)
								res.Append(" && " + string.Format("EffectiveDate <= \"{0}\"",  Convert.ToDateTime(EndEffectiveDateFilter).Date.AddHours(23).ToString("MM/dd/yyyy HH:mm:ss")));
						}

				if (Convert.ToDateTime(StartEffectiveDateFilter).Date != DateTime.MinValue &&
		        Convert.ToDateTime(EndEffectiveDateFilter).Date != DateTime.MinValue) res.Append(" )");

					if (Convert.ToDateTime(_effectiveDateFilter).Date != DateTime.MinValue)
						{
							if(EffectiveDateFilter.HasValue)
								res.Append(" && " + string.Format("EffectiveDate == \"{0}\"",  Convert.ToDateTime(EffectiveDateFilter).Date.ToString("MM/dd/yyyy")));
						}
				 

									if(IsReconciledFilter.HasValue)
						res.Append(" && " + string.Format("IsReconciled == {0}",  IsReconciledFilter));						
 

					if(LastCostFilter.HasValue)
						res.Append(" && " + string.Format("LastCost == {0}",  LastCostFilter.ToString()));				 

					if(TaxAmountFilter.HasValue)
						res.Append(" && " + string.Format("TaxAmount == {0}",  TaxAmountFilter.ToString()));				 

									if(string.IsNullOrEmpty(EmailIdFilter) == false)
						res.Append(" && " + string.Format("EmailId.Contains(\"{0}\")",  EmailIdFilter));						
 

									if(string.IsNullOrEmpty(NameFilter) == false)
						res.Append(" && " + string.Format("Name.Contains(\"{0}\")",  NameFilter));						
 

					if(FileLineNumberFilter.HasValue)
						res.Append(" && " + string.Format("FileLineNumber == {0}",  FileLineNumberFilter.ToString()));				 

					if(VolumeLitersFilter.HasValue)
						res.Append(" && " + string.Format("VolumeLiters == {0}",  VolumeLitersFilter.ToString()));				 

					if(PreviousCLineNumberFilter.HasValue)
						res.Append(" && " + string.Format("PreviousCLineNumber == {0}",  PreviousCLineNumberFilter.ToString()));							return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<EntryDataDetailsEx> lst = null;
            using (var ctx = new EntryDataDetailsExRepository())
            {
                lst = await ctx.GetEntryDataDetailsExByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<EntryDataDetailsExExcelLine, List<EntryDataDetailsExExcelLine>>
            {
                dataToPrint = lst.Select(x => new EntryDataDetailsExExcelLine
                {
 
                    EntryDataId = x.EntryDataId ,
                    
 
                    LineNumber = x.LineNumber ,
                    
 
                    ItemNumber = x.ItemNumber ,
                    
 
                    Quantity = x.Quantity ,
                    
 
                    Units = x.Units ,
                    
 
                    ItemDescription = x.ItemDescription ,
                    
 
                    Cost = x.Cost ,
                    
 
                    QtyAllocated = x.QtyAllocated ,
                    
 
                    UnitWeight = x.UnitWeight ,
                    
 
                    DoNotAllocate = x.DoNotAllocate ,
                    
 
                    TariffCode = x.TariffCode ,
                    
 
                    CNumber = x.CNumber ,
                    
 
                    CLineNumber = x.CLineNumber ,
                    
 
                    Downloaded = x.Downloaded ,
                    
 
                    DutyFreePaid = x.DutyFreePaid ,
                    
 
                    Total = x.Total ,
                    
 
                    InvoiceQty = x.InvoiceQty ,
                    
 
                    ReceivedQty = x.ReceivedQty ,
                    
 
                    Status = x.Status ,
                    
 
                    PreviousInvoiceNumber = x.PreviousInvoiceNumber ,
                    
 
                    PreviousCNumber = x.PreviousCNumber ,
                    
 
                    Comment = x.Comment ,
                    
 
                    EffectiveDate = x.EffectiveDate ,
                    
 
                    IsReconciled = x.IsReconciled ,
                    
 
                    LastCost = x.LastCost ,
                    
 
                    TaxAmount = x.TaxAmount ,
                    
 
                    EmailId = x.EmailId ,
                    
 
                    Name = x.Name ,
                    
 
                    FileLineNumber = x.FileLineNumber ,
                    
 
                    VolumeLiters = x.VolumeLiters ,
                    
 
                    PreviousCLineNumber = x.PreviousCLineNumber 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class EntryDataDetailsExExcelLine
        {
		 
                    public string EntryDataId { get; set; } 
                    
 
                    public Nullable<int> LineNumber { get; set; } 
                    
 
                    public string ItemNumber { get; set; } 
                    
 
                    public float Quantity { get; set; } 
                    
 
                    public string Units { get; set; } 
                    
 
                    public string ItemDescription { get; set; } 
                    
 
                    public float Cost { get; set; } 
                    
 
                    public double QtyAllocated { get; set; } 
                    
 
                    public float UnitWeight { get; set; } 
                    
 
                    public Nullable<bool> DoNotAllocate { get; set; } 
                    
 
                    public string TariffCode { get; set; } 
                    
 
                    public string CNumber { get; set; } 
                    
 
                    public Nullable<int> CLineNumber { get; set; } 
                    
 
                    public Nullable<bool> Downloaded { get; set; } 
                    
 
                    public string DutyFreePaid { get; set; } 
                    
 
                    public Nullable<float> Total { get; set; } 
                    
 
                    public Nullable<double> InvoiceQty { get; set; } 
                    
 
                    public Nullable<double> ReceivedQty { get; set; } 
                    
 
                    public string Status { get; set; } 
                    
 
                    public string PreviousInvoiceNumber { get; set; } 
                    
 
                    public string PreviousCNumber { get; set; } 
                    
 
                    public string Comment { get; set; } 
                    
 
                    public Nullable<System.DateTime> EffectiveDate { get; set; } 
                    
 
                    public Nullable<bool> IsReconciled { get; set; } 
                    
 
                    public Nullable<double> LastCost { get; set; } 
                    
 
                    public Nullable<double> TaxAmount { get; set; } 
                    
 
                    public string EmailId { get; set; } 
                    
 
                    public string Name { get; set; } 
                    
 
                    public Nullable<int> FileLineNumber { get; set; } 
                    
 
                    public Nullable<double> VolumeLiters { get; set; } 
                    
 
                    public Nullable<int> PreviousCLineNumber { get; set; } 
                    
        }

		
    }
}
		
