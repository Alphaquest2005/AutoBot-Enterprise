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

using CoreEntities.Client.Entities;
using CoreEntities.Client.Repositories;
//using WaterNut.Client.Repositories;


namespace WaterNut.QuerySpace.CoreEntities.ViewModels
{
    
	public partial class TODO_TotalAdjustmentsToProcessViewModel_AutoGen : ViewModelBase<TODO_TotalAdjustmentsToProcessViewModel_AutoGen>
	{

       private static readonly TODO_TotalAdjustmentsToProcessViewModel_AutoGen instance;
       static TODO_TotalAdjustmentsToProcessViewModel_AutoGen()
        {
            instance = new TODO_TotalAdjustmentsToProcessViewModel_AutoGen();
        }

       public static TODO_TotalAdjustmentsToProcessViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public TODO_TotalAdjustmentsToProcessViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<TODO_TotalAdjustmentsToProcess>(MessageToken.CurrentTODO_TotalAdjustmentsToProcessChanged, OnCurrentTODO_TotalAdjustmentsToProcessChanged);
            RegisterToReceiveMessages(MessageToken.TODO_TotalAdjustmentsToProcessChanged, OnTODO_TotalAdjustmentsToProcessChanged);
			RegisterToReceiveMessages(MessageToken.TODO_TotalAdjustmentsToProcessFilterExpressionChanged, OnTODO_TotalAdjustmentsToProcessFilterExpressionChanged);


 			// Recieve messages for Core Current Entities Changed
 

			TODO_TotalAdjustmentsToProcess = new VirtualList<TODO_TotalAdjustmentsToProcess>(vloader);
			TODO_TotalAdjustmentsToProcess.LoadingStateChanged += TODO_TotalAdjustmentsToProcess_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(TODO_TotalAdjustmentsToProcess, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<TODO_TotalAdjustmentsToProcess> _TODO_TotalAdjustmentsToProcess = null;
        public VirtualList<TODO_TotalAdjustmentsToProcess> TODO_TotalAdjustmentsToProcess
        {
            get
            {
                return _TODO_TotalAdjustmentsToProcess;
            }
            set
            {
                _TODO_TotalAdjustmentsToProcess = value;
                NotifyPropertyChanged( x => x.TODO_TotalAdjustmentsToProcess);
            }
        }

		 private void OnTODO_TotalAdjustmentsToProcessFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => TODO_TotalAdjustmentsToProcess.Refresh()).ConfigureAwait(false);
            SelectedTODO_TotalAdjustmentsToProcess.Clear();
            NotifyPropertyChanged(x => SelectedTODO_TotalAdjustmentsToProcess);
            BeginSendMessage(MessageToken.SelectedTODO_TotalAdjustmentsToProcessChanged, new NotificationEventArgs(MessageToken.SelectedTODO_TotalAdjustmentsToProcessChanged));
        }

		void TODO_TotalAdjustmentsToProcess_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (TODO_TotalAdjustmentsToProcess.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => TODO_TotalAdjustmentsToProcess);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("TODO_TotalAdjustmentsToProcess | Error occured..." + TODO_TotalAdjustmentsToProcess.LastLoadingError.Message);
                    NotifyPropertyChanged(x => TODO_TotalAdjustmentsToProcess);
                    break;
            }
           
        }

		
		public readonly TODO_TotalAdjustmentsToProcessVirturalListLoader vloader = new TODO_TotalAdjustmentsToProcessVirturalListLoader();

		private ObservableCollection<TODO_TotalAdjustmentsToProcess> _selectedTODO_TotalAdjustmentsToProcess = new ObservableCollection<TODO_TotalAdjustmentsToProcess>();
        public ObservableCollection<TODO_TotalAdjustmentsToProcess> SelectedTODO_TotalAdjustmentsToProcess
        {
            get
            {
                return _selectedTODO_TotalAdjustmentsToProcess;
            }
            set
            {
                _selectedTODO_TotalAdjustmentsToProcess = value;
				BeginSendMessage(MessageToken.SelectedTODO_TotalAdjustmentsToProcessChanged,
                                    new NotificationEventArgs(MessageToken.SelectedTODO_TotalAdjustmentsToProcessChanged));
				 NotifyPropertyChanged(x => SelectedTODO_TotalAdjustmentsToProcess);
            }
        }

        internal virtual void OnCurrentTODO_TotalAdjustmentsToProcessChanged(object sender, NotificationEventArgs<TODO_TotalAdjustmentsToProcess> e)
        {
            if(BaseViewModel.Instance.CurrentTODO_TotalAdjustmentsToProcess != null) BaseViewModel.Instance.CurrentTODO_TotalAdjustmentsToProcess.PropertyChanged += CurrentTODO_TotalAdjustmentsToProcess__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentTODO_TotalAdjustmentsToProcess);
        }   

            void CurrentTODO_TotalAdjustmentsToProcess__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    //if (e.PropertyName == "AddAsycudaDocumentSet")
                   // {
                   //    if(AsycudaDocumentSet.Contains(CurrentTODO_TotalAdjustmentsToProcess.AsycudaDocumentSet) == false) AsycudaDocumentSet.Add(CurrentTODO_TotalAdjustmentsToProcess.AsycudaDocumentSet);
                    //}
                 } 
        internal virtual void OnTODO_TotalAdjustmentsToProcessChanged(object sender, NotificationEventArgs e)
        {
            _TODO_TotalAdjustmentsToProcess.Refresh();
			NotifyPropertyChanged(x => this.TODO_TotalAdjustmentsToProcess);
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
					
                    TODO_TotalAdjustmentsToProcess.Refresh();
					NotifyPropertyChanged(x => this.TODO_TotalAdjustmentsToProcess);
				}
  
// Filtering Each Field except IDs
		public void ViewAll()
        {
			vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";




			vloader.ClearNavigationExpression();
			_TODO_TotalAdjustmentsToProcess.Refresh();
			NotifyPropertyChanged(x => this.TODO_TotalAdjustmentsToProcess);
		}

		public async Task SelectAll()
        {
            IEnumerable<TODO_TotalAdjustmentsToProcess> lst = null;
            using (var ctx = new TODO_TotalAdjustmentsToProcessRepository())
            {
                lst = await ctx.GetTODO_TotalAdjustmentsToProcessByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedTODO_TotalAdjustmentsToProcess = new ObservableCollection<TODO_TotalAdjustmentsToProcess>(lst);
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

 

		private Double? _quantityFilter;
        public Double? QuantityFilter
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

 

		private Double? _costFilter;
        public Double? CostFilter
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

 

		private Double? _unitWeightFilter;
        public Double? UnitWeightFilter
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

 

		private string _currencyFilter;
        public string CurrencyFilter
        {
            get
            {
                return _currencyFilter;
            }
            set
            {
                _currencyFilter = value;
				NotifyPropertyChanged(x => CurrencyFilter);
                FilterData();
                
            }
        }	

 

		private string _typeFilter;
        public string TypeFilter
        {
            get
            {
                return _typeFilter;
            }
            set
            {
                _typeFilter = value;
				NotifyPropertyChanged(x => TypeFilter);
                FilterData();
                
            }
        }	

 

		private string _declarant_Reference_NumberFilter;
        public string Declarant_Reference_NumberFilter
        {
            get
            {
                return _declarant_Reference_NumberFilter;
            }
            set
            {
                _declarant_Reference_NumberFilter = value;
				NotifyPropertyChanged(x => Declarant_Reference_NumberFilter);
                FilterData();
                
            }
        }	

 
		private DateTime? _startInvoiceDateFilter = DateTime.Parse(string.Format("{0}/1/{1}", DateTime.Now.Month ,DateTime.Now.Year));
        public DateTime? StartInvoiceDateFilter
        {
            get
            {
                return _startInvoiceDateFilter;
            }
            set
            {
                _startInvoiceDateFilter = value;
				NotifyPropertyChanged(x => StartInvoiceDateFilter);
                FilterData();
                
            }
        }	

		private DateTime? _endInvoiceDateFilter = DateTime.Parse(string.Format("{1}/{0}/{2}", DateTime.DaysInMonth( DateTime.Now.Year,DateTime.Now.Month), DateTime.Now.Month, DateTime.Now.Year));
        public DateTime? EndInvoiceDateFilter
        {
            get
            {
                return _endInvoiceDateFilter;
            }
            set
            {
                _endInvoiceDateFilter = value;
				NotifyPropertyChanged(x => EndInvoiceDateFilter);
                FilterData();
                
            }
        }
 

		private DateTime? _invoiceDateFilter;
        public DateTime? InvoiceDateFilter
        {
            get
            {
                return _invoiceDateFilter;
            }
            set
            {
                _invoiceDateFilter = value;
				NotifyPropertyChanged(x => InvoiceDateFilter);
                FilterData();
                
            }
        }	

 

		private string _subjectFilter;
        public string SubjectFilter
        {
            get
            {
                return _subjectFilter;
            }
            set
            {
                _subjectFilter = value;
				NotifyPropertyChanged(x => SubjectFilter);
                FilterData();
                
            }
        }	

 
		private DateTime? _startEmailDateFilter = DateTime.Parse(string.Format("{0}/1/{1}", DateTime.Now.Month ,DateTime.Now.Year));
        public DateTime? StartEmailDateFilter
        {
            get
            {
                return _startEmailDateFilter;
            }
            set
            {
                _startEmailDateFilter = value;
				NotifyPropertyChanged(x => StartEmailDateFilter);
                FilterData();
                
            }
        }	

		private DateTime? _endEmailDateFilter = DateTime.Parse(string.Format("{1}/{0}/{2}", DateTime.DaysInMonth( DateTime.Now.Year,DateTime.Now.Month), DateTime.Now.Month, DateTime.Now.Year));
        public DateTime? EndEmailDateFilter
        {
            get
            {
                return _endEmailDateFilter;
            }
            set
            {
                _endEmailDateFilter = value;
				NotifyPropertyChanged(x => EndEmailDateFilter);
                FilterData();
                
            }
        }
 

		private DateTime? _emailDateFilter;
        public DateTime? EmailDateFilter
        {
            get
            {
                return _emailDateFilter;
            }
            set
            {
                _emailDateFilter = value;
				NotifyPropertyChanged(x => EmailDateFilter);
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

			TODO_TotalAdjustmentsToProcess.Refresh();
			NotifyPropertyChanged(x => this.TODO_TotalAdjustmentsToProcess);
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

					if(InvoiceQtyFilter.HasValue)
						res.Append(" && " + string.Format("InvoiceQty == {0}",  InvoiceQtyFilter.ToString()));				 

					if(ReceivedQtyFilter.HasValue)
						res.Append(" && " + string.Format("ReceivedQty == {0}",  ReceivedQtyFilter.ToString()));				 

									if(string.IsNullOrEmpty(PreviousInvoiceNumberFilter) == false)
						res.Append(" && " + string.Format("PreviousInvoiceNumber.Contains(\"{0}\")",  PreviousInvoiceNumberFilter));						
 

									if(string.IsNullOrEmpty(PreviousCNumberFilter) == false)
						res.Append(" && " + string.Format("PreviousCNumber.Contains(\"{0}\")",  PreviousCNumberFilter));						
 

									if(string.IsNullOrEmpty(CommentFilter) == false)
						res.Append(" && " + string.Format("Comment.Contains(\"{0}\")",  CommentFilter));						
 

									if(string.IsNullOrEmpty(StatusFilter) == false)
						res.Append(" && " + string.Format("Status.Contains(\"{0}\")",  StatusFilter));						
 

 

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
				 

									if(string.IsNullOrEmpty(CurrencyFilter) == false)
						res.Append(" && " + string.Format("Currency.Contains(\"{0}\")",  CurrencyFilter));						
 

									if(string.IsNullOrEmpty(TypeFilter) == false)
						res.Append(" && " + string.Format("Type.Contains(\"{0}\")",  TypeFilter));						
 

									if(string.IsNullOrEmpty(Declarant_Reference_NumberFilter) == false)
						res.Append(" && " + string.Format("Declarant_Reference_Number.Contains(\"{0}\")",  Declarant_Reference_NumberFilter));						
 

 

				if (Convert.ToDateTime(StartInvoiceDateFilter).Date != DateTime.MinValue &&
		        Convert.ToDateTime(EndInvoiceDateFilter).Date != DateTime.MinValue) res.Append(" && (");

					if (Convert.ToDateTime(StartInvoiceDateFilter).Date != DateTime.MinValue)
						{
							if(StartInvoiceDateFilter.HasValue)
								res.Append(
                                            (Convert.ToDateTime(EndInvoiceDateFilter).Date != DateTime.MinValue?"":" && ") +
                                            string.Format("InvoiceDate >= \"{0}\"",  Convert.ToDateTime(StartInvoiceDateFilter).Date.ToString("MM/dd/yyyy")));
						}

					if (Convert.ToDateTime(EndInvoiceDateFilter).Date != DateTime.MinValue)
						{
							if(EndInvoiceDateFilter.HasValue)
								res.Append(" && " + string.Format("InvoiceDate <= \"{0}\"",  Convert.ToDateTime(EndInvoiceDateFilter).Date.AddHours(23).ToString("MM/dd/yyyy HH:mm:ss")));
						}

				if (Convert.ToDateTime(StartInvoiceDateFilter).Date != DateTime.MinValue &&
		        Convert.ToDateTime(EndInvoiceDateFilter).Date != DateTime.MinValue) res.Append(" )");

					if (Convert.ToDateTime(_invoiceDateFilter).Date != DateTime.MinValue)
						{
							if(InvoiceDateFilter.HasValue)
								res.Append(" && " + string.Format("InvoiceDate == \"{0}\"",  Convert.ToDateTime(InvoiceDateFilter).Date.ToString("MM/dd/yyyy")));
						}
				 

									if(string.IsNullOrEmpty(SubjectFilter) == false)
						res.Append(" && " + string.Format("Subject.Contains(\"{0}\")",  SubjectFilter));						
 

 

				if (Convert.ToDateTime(StartEmailDateFilter).Date != DateTime.MinValue &&
		        Convert.ToDateTime(EndEmailDateFilter).Date != DateTime.MinValue) res.Append(" && (");

					if (Convert.ToDateTime(StartEmailDateFilter).Date != DateTime.MinValue)
						{
							if(StartEmailDateFilter.HasValue)
								res.Append(
                                            (Convert.ToDateTime(EndEmailDateFilter).Date != DateTime.MinValue?"":" && ") +
                                            string.Format("EmailDate >= \"{0}\"",  Convert.ToDateTime(StartEmailDateFilter).Date.ToString("MM/dd/yyyy")));
						}

					if (Convert.ToDateTime(EndEmailDateFilter).Date != DateTime.MinValue)
						{
							if(EndEmailDateFilter.HasValue)
								res.Append(" && " + string.Format("EmailDate <= \"{0}\"",  Convert.ToDateTime(EndEmailDateFilter).Date.AddHours(23).ToString("MM/dd/yyyy HH:mm:ss")));
						}

				if (Convert.ToDateTime(StartEmailDateFilter).Date != DateTime.MinValue &&
		        Convert.ToDateTime(EndEmailDateFilter).Date != DateTime.MinValue) res.Append(" )");

					if (Convert.ToDateTime(_emailDateFilter).Date != DateTime.MinValue)
						{
							if(EmailDateFilter.HasValue)
								res.Append(" && " + string.Format("EmailDate == \"{0}\"",  Convert.ToDateTime(EmailDateFilter).Date.ToString("MM/dd/yyyy")));
						}
				 

									if(string.IsNullOrEmpty(DutyFreePaidFilter) == false)
						res.Append(" && " + string.Format("DutyFreePaid.Contains(\"{0}\")",  DutyFreePaidFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<TODO_TotalAdjustmentsToProcess> lst = null;
            using (var ctx = new TODO_TotalAdjustmentsToProcessRepository())
            {
                lst = await ctx.GetTODO_TotalAdjustmentsToProcessByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<TODO_TotalAdjustmentsToProcessExcelLine, List<TODO_TotalAdjustmentsToProcessExcelLine>>
            {
                dataToPrint = lst.Select(x => new TODO_TotalAdjustmentsToProcessExcelLine
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
                    
 
                    InvoiceQty = x.InvoiceQty ,
                    
 
                    ReceivedQty = x.ReceivedQty ,
                    
 
                    PreviousInvoiceNumber = x.PreviousInvoiceNumber ,
                    
 
                    PreviousCNumber = x.PreviousCNumber ,
                    
 
                    Comment = x.Comment ,
                    
 
                    Status = x.Status ,
                    
 
                    EffectiveDate = x.EffectiveDate ,
                    
 
                    Currency = x.Currency ,
                    
 
                    Type = x.Type ,
                    
 
                    Declarant_Reference_Number = x.Declarant_Reference_Number ,
                    
 
                    InvoiceDate = x.InvoiceDate ,
                    
 
                    Subject = x.Subject ,
                    
 
                    EmailDate = x.EmailDate ,
                    
 
                    DutyFreePaid = x.DutyFreePaid 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class TODO_TotalAdjustmentsToProcessExcelLine
        {
		 
                    public string EntryDataId { get; set; } 
                    
 
                    public Nullable<int> LineNumber { get; set; } 
                    
 
                    public string ItemNumber { get; set; } 
                    
 
                    public double Quantity { get; set; } 
                    
 
                    public string Units { get; set; } 
                    
 
                    public string ItemDescription { get; set; } 
                    
 
                    public double Cost { get; set; } 
                    
 
                    public double QtyAllocated { get; set; } 
                    
 
                    public double UnitWeight { get; set; } 
                    
 
                    public Nullable<bool> DoNotAllocate { get; set; } 
                    
 
                    public string TariffCode { get; set; } 
                    
 
                    public string CNumber { get; set; } 
                    
 
                    public Nullable<int> CLineNumber { get; set; } 
                    
 
                    public Nullable<double> InvoiceQty { get; set; } 
                    
 
                    public Nullable<double> ReceivedQty { get; set; } 
                    
 
                    public string PreviousInvoiceNumber { get; set; } 
                    
 
                    public string PreviousCNumber { get; set; } 
                    
 
                    public string Comment { get; set; } 
                    
 
                    public string Status { get; set; } 
                    
 
                    public Nullable<System.DateTime> EffectiveDate { get; set; } 
                    
 
                    public string Currency { get; set; } 
                    
 
                    public string Type { get; set; } 
                    
 
                    public string Declarant_Reference_Number { get; set; } 
                    
 
                    public System.DateTime InvoiceDate { get; set; } 
                    
 
                    public string Subject { get; set; } 
                    
 
                    public Nullable<System.DateTime> EmailDate { get; set; } 
                    
 
                    public string DutyFreePaid { get; set; } 
                    
        }

		
    }
}
		
