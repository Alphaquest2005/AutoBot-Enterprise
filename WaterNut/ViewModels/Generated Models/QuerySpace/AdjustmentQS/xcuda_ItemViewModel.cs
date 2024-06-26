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

using AdjustmentQS.Client.Entities;
using AdjustmentQS.Client.Repositories;
//using WaterNut.Client.Repositories;
        
using CoreEntities.Client.Entities;


namespace WaterNut.QuerySpace.AdjustmentQS.ViewModels
{
    
	public partial class xcuda_ItemViewModel_AutoGen : ViewModelBase<xcuda_ItemViewModel_AutoGen>
	{

       private static readonly xcuda_ItemViewModel_AutoGen instance;
       static xcuda_ItemViewModel_AutoGen()
        {
            instance = new xcuda_ItemViewModel_AutoGen();
        }

       public static xcuda_ItemViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public xcuda_ItemViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<xcuda_Item>(MessageToken.Currentxcuda_ItemChanged, OnCurrentxcuda_ItemChanged);
            RegisterToReceiveMessages(MessageToken.xcuda_ItemChanged, Onxcuda_ItemChanged);
			RegisterToReceiveMessages(MessageToken.xcuda_ItemFilterExpressionChanged, Onxcuda_ItemFilterExpressionChanged);


 			// Recieve messages for Core Current Entities Changed
 

			xcuda_Item = new VirtualList<xcuda_Item>(vloader);
			xcuda_Item.LoadingStateChanged += xcuda_Item_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(xcuda_Item, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<xcuda_Item> _xcuda_Item = null;
        public VirtualList<xcuda_Item> xcuda_Item
        {
            get
            {
                return _xcuda_Item;
            }
            set
            {
                _xcuda_Item = value;
                NotifyPropertyChanged( x => x.xcuda_Item);
            }
        }

		 private void Onxcuda_ItemFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => xcuda_Item.Refresh()).ConfigureAwait(false);
            Selectedxcuda_Item.Clear();
            NotifyPropertyChanged(x => Selectedxcuda_Item);
            BeginSendMessage(MessageToken.Selectedxcuda_ItemChanged, new NotificationEventArgs(MessageToken.Selectedxcuda_ItemChanged));
        }

		void xcuda_Item_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (xcuda_Item.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => xcuda_Item);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("xcuda_Item | Error occured..." + xcuda_Item.LastLoadingError.Message);
                    NotifyPropertyChanged(x => xcuda_Item);
                    break;
            }
           
        }

		
		public readonly xcuda_ItemVirturalListLoader vloader = new xcuda_ItemVirturalListLoader();

		private ObservableCollection<xcuda_Item> _selectedxcuda_Item = new ObservableCollection<xcuda_Item>();
        public ObservableCollection<xcuda_Item> Selectedxcuda_Item
        {
            get
            {
                return _selectedxcuda_Item;
            }
            set
            {
                _selectedxcuda_Item = value;
				BeginSendMessage(MessageToken.Selectedxcuda_ItemChanged,
                                    new NotificationEventArgs(MessageToken.Selectedxcuda_ItemChanged));
				 NotifyPropertyChanged(x => Selectedxcuda_Item);
            }
        }

        internal virtual void OnCurrentxcuda_ItemChanged(object sender, NotificationEventArgs<xcuda_Item> e)
        {
            if(BaseViewModel.Instance.Currentxcuda_Item != null) BaseViewModel.Instance.Currentxcuda_Item.PropertyChanged += Currentxcuda_Item__propertyChanged;
           // NotifyPropertyChanged(x => this.Currentxcuda_Item);
        }   

            void Currentxcuda_Item__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                 } 
        internal virtual void Onxcuda_ItemChanged(object sender, NotificationEventArgs e)
        {
            _xcuda_Item.Refresh();
			NotifyPropertyChanged(x => this.xcuda_Item);
        }   


 
  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
		public void ViewAll()
        {
		vloader.FilterExpression = "All";




			vloader.ClearNavigationExpression();
			_xcuda_Item.Refresh();
			NotifyPropertyChanged(x => this.xcuda_Item);
		}

		public async Task SelectAll()
        {
            IEnumerable<xcuda_Item> lst = null;
            using (var ctx = new xcuda_ItemRepository())
            {
                lst = await ctx.Getxcuda_ItemByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            Selectedxcuda_Item = new ObservableCollection<xcuda_Item>(lst);
        }

 

		private string _amount_deducted_from_licenceFilter;
        public string Amount_deducted_from_licenceFilter
        {
            get
            {
                return _amount_deducted_from_licenceFilter;
            }
            set
            {
                _amount_deducted_from_licenceFilter = value;
				NotifyPropertyChanged(x => Amount_deducted_from_licenceFilter);
                FilterData();
                
            }
        }	

 

		private string _quantity_deducted_from_licenceFilter;
        public string Quantity_deducted_from_licenceFilter
        {
            get
            {
                return _quantity_deducted_from_licenceFilter;
            }
            set
            {
                _quantity_deducted_from_licenceFilter = value;
				NotifyPropertyChanged(x => Quantity_deducted_from_licenceFilter);
                FilterData();
                
            }
        }	

 

		private string _licence_numberFilter;
        public string Licence_numberFilter
        {
            get
            {
                return _licence_numberFilter;
            }
            set
            {
                _licence_numberFilter = value;
				NotifyPropertyChanged(x => Licence_numberFilter);
                FilterData();
                
            }
        }	

 

		private string _free_text_1Filter;
        public string Free_text_1Filter
        {
            get
            {
                return _free_text_1Filter;
            }
            set
            {
                _free_text_1Filter = value;
				NotifyPropertyChanged(x => Free_text_1Filter);
                FilterData();
                
            }
        }	

 

		private string _free_text_2Filter;
        public string Free_text_2Filter
        {
            get
            {
                return _free_text_2Filter;
            }
            set
            {
                _free_text_2Filter = value;
				NotifyPropertyChanged(x => Free_text_2Filter);
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

 

		private Boolean? _isAssessedFilter;
        public Boolean? IsAssessedFilter
        {
            get
            {
                return _isAssessedFilter;
            }
            set
            {
                _isAssessedFilter = value;
				NotifyPropertyChanged(x => IsAssessedFilter);
                FilterData();
                
            }
        }	

 

		private Double? _dPQtyAllocatedFilter;
        public Double? DPQtyAllocatedFilter
        {
            get
            {
                return _dPQtyAllocatedFilter;
            }
            set
            {
                _dPQtyAllocatedFilter = value;
				NotifyPropertyChanged(x => DPQtyAllocatedFilter);
                FilterData();
                
            }
        }	

 

		private Double? _dFQtyAllocatedFilter;
        public Double? DFQtyAllocatedFilter
        {
            get
            {
                return _dFQtyAllocatedFilter;
            }
            set
            {
                _dFQtyAllocatedFilter = value;
				NotifyPropertyChanged(x => DFQtyAllocatedFilter);
                FilterData();
                
            }
        }	

 
		private DateTime? _startEntryTimeStampFilter = DateTime.Parse(string.Format("{0}/1/{1}", DateTime.Now.Month ,DateTime.Now.Year));
        public DateTime? StartEntryTimeStampFilter
        {
            get
            {
                return _startEntryTimeStampFilter;
            }
            set
            {
                _startEntryTimeStampFilter = value;
				NotifyPropertyChanged(x => StartEntryTimeStampFilter);
                FilterData();
                
            }
        }	

		private DateTime? _endEntryTimeStampFilter = DateTime.Parse(string.Format("{1}/{0}/{2}", DateTime.DaysInMonth( DateTime.Now.Year,DateTime.Now.Month), DateTime.Now.Month, DateTime.Now.Year));
        public DateTime? EndEntryTimeStampFilter
        {
            get
            {
                return _endEntryTimeStampFilter;
            }
            set
            {
                _endEntryTimeStampFilter = value;
				NotifyPropertyChanged(x => EndEntryTimeStampFilter);
                FilterData();
                
            }
        }
 

		private DateTime? _entryTimeStampFilter;
        public DateTime? EntryTimeStampFilter
        {
            get
            {
                return _entryTimeStampFilter;
            }
            set
            {
                _entryTimeStampFilter = value;
				NotifyPropertyChanged(x => EntryTimeStampFilter);
                FilterData();
                
            }
        }	

 

		private Boolean? _attributeOnlyAllocationFilter;
        public Boolean? AttributeOnlyAllocationFilter
        {
            get
            {
                return _attributeOnlyAllocationFilter;
            }
            set
            {
                _attributeOnlyAllocationFilter = value;
				NotifyPropertyChanged(x => AttributeOnlyAllocationFilter);
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

 

		private Boolean? _doNotEXFilter;
        public Boolean? DoNotEXFilter
        {
            get
            {
                return _doNotEXFilter;
            }
            set
            {
                _doNotEXFilter = value;
				NotifyPropertyChanged(x => DoNotEXFilter);
                FilterData();
                
            }
        }	

 

		private Boolean? _importCompleteFilter;
        public Boolean? ImportCompleteFilter
        {
            get
            {
                return _importCompleteFilter;
            }
            set
            {
                _importCompleteFilter = value;
				NotifyPropertyChanged(x => ImportCompleteFilter);
                FilterData();
                
            }
        }	

 

		private string _warehouseErrorFilter;
        public string WarehouseErrorFilter
        {
            get
            {
                return _warehouseErrorFilter;
            }
            set
            {
                _warehouseErrorFilter = value;
				NotifyPropertyChanged(x => WarehouseErrorFilter);
                FilterData();
                
            }
        }	

 

		private Double? _salesFactorFilter;
        public Double? SalesFactorFilter
        {
            get
            {
                return _salesFactorFilter;
            }
            set
            {
                _salesFactorFilter = value;
				NotifyPropertyChanged(x => SalesFactorFilter);
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

 

		private string _previousInvoiceLineNumberFilter;
        public string PreviousInvoiceLineNumberFilter
        {
            get
            {
                return _previousInvoiceLineNumberFilter;
            }
            set
            {
                _previousInvoiceLineNumberFilter = value;
				NotifyPropertyChanged(x => PreviousInvoiceLineNumberFilter);
                FilterData();
                
            }
        }	

 

		private string _previousInvoiceItemNumberFilter;
        public string PreviousInvoiceItemNumberFilter
        {
            get
            {
                return _previousInvoiceItemNumberFilter;
            }
            set
            {
                _previousInvoiceItemNumberFilter = value;
				NotifyPropertyChanged(x => PreviousInvoiceItemNumberFilter);
                FilterData();
                
            }
        }	

 

		private string _entryDataTypeFilter;
        public string EntryDataTypeFilter
        {
            get
            {
                return _entryDataTypeFilter;
            }
            set
            {
                _entryDataTypeFilter = value;
				NotifyPropertyChanged(x => EntryDataTypeFilter);
                FilterData();
                
            }
        }	

 

		private Int32? _upgradeKeyFilter;
        public Int32? UpgradeKeyFilter
        {
            get
            {
                return _upgradeKeyFilter;
            }
            set
            {
                _upgradeKeyFilter = value;
				NotifyPropertyChanged(x => UpgradeKeyFilter);
                FilterData();
                
            }
        }	

 

		private string _previousInvoiceKeyFilter;
        public string PreviousInvoiceKeyFilter
        {
            get
            {
                return _previousInvoiceKeyFilter;
            }
            set
            {
                _previousInvoiceKeyFilter = value;
				NotifyPropertyChanged(x => PreviousInvoiceKeyFilter);
                FilterData();
                
            }
        }	

 

		private string _xWarehouseErrorFilter;
        public string xWarehouseErrorFilter
        {
            get
            {
                return _xWarehouseErrorFilter;
            }
            set
            {
                _xWarehouseErrorFilter = value;
				NotifyPropertyChanged(x => xWarehouseErrorFilter);
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

			xcuda_Item.Refresh();
			NotifyPropertyChanged(x => this.xcuda_Item);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(Amount_deducted_from_licenceFilter) == false)
						res.Append(" && " + string.Format("Amount_deducted_from_licence.Contains(\"{0}\")",  Amount_deducted_from_licenceFilter));						
 

									if(string.IsNullOrEmpty(Quantity_deducted_from_licenceFilter) == false)
						res.Append(" && " + string.Format("Quantity_deducted_from_licence.Contains(\"{0}\")",  Quantity_deducted_from_licenceFilter));						
 

									if(string.IsNullOrEmpty(Licence_numberFilter) == false)
						res.Append(" && " + string.Format("Licence_number.Contains(\"{0}\")",  Licence_numberFilter));						
 

									if(string.IsNullOrEmpty(Free_text_1Filter) == false)
						res.Append(" && " + string.Format("Free_text_1.Contains(\"{0}\")",  Free_text_1Filter));						
 

									if(string.IsNullOrEmpty(Free_text_2Filter) == false)
						res.Append(" && " + string.Format("Free_text_2.Contains(\"{0}\")",  Free_text_2Filter));						
 

					if(LineNumberFilter.HasValue)
						res.Append(" && " + string.Format("LineNumber == {0}",  LineNumberFilter.ToString()));				 

									if(IsAssessedFilter.HasValue)
						res.Append(" && " + string.Format("IsAssessed == {0}",  IsAssessedFilter));						
 

					if(DPQtyAllocatedFilter.HasValue)
						res.Append(" && " + string.Format("DPQtyAllocated == {0}",  DPQtyAllocatedFilter.ToString()));				 

					if(DFQtyAllocatedFilter.HasValue)
						res.Append(" && " + string.Format("DFQtyAllocated == {0}",  DFQtyAllocatedFilter.ToString()));				 

 

				if (Convert.ToDateTime(StartEntryTimeStampFilter).Date != DateTime.MinValue &&
		        Convert.ToDateTime(EndEntryTimeStampFilter).Date != DateTime.MinValue) res.Append(" && (");

					if (Convert.ToDateTime(StartEntryTimeStampFilter).Date != DateTime.MinValue)
						{
							if(StartEntryTimeStampFilter.HasValue)
								res.Append(
                                            (Convert.ToDateTime(EndEntryTimeStampFilter).Date != DateTime.MinValue?"":" && ") +
                                            string.Format("EntryTimeStamp >= \"{0}\"",  Convert.ToDateTime(StartEntryTimeStampFilter).Date.ToString("MM/dd/yyyy")));
						}

					if (Convert.ToDateTime(EndEntryTimeStampFilter).Date != DateTime.MinValue)
						{
							if(EndEntryTimeStampFilter.HasValue)
								res.Append(" && " + string.Format("EntryTimeStamp <= \"{0}\"",  Convert.ToDateTime(EndEntryTimeStampFilter).Date.AddHours(23).ToString("MM/dd/yyyy HH:mm:ss")));
						}

				if (Convert.ToDateTime(StartEntryTimeStampFilter).Date != DateTime.MinValue &&
		        Convert.ToDateTime(EndEntryTimeStampFilter).Date != DateTime.MinValue) res.Append(" )");

					if (Convert.ToDateTime(_entryTimeStampFilter).Date != DateTime.MinValue)
						{
							if(EntryTimeStampFilter.HasValue)
								res.Append(" && " + string.Format("EntryTimeStamp == \"{0}\"",  Convert.ToDateTime(EntryTimeStampFilter).Date.ToString("MM/dd/yyyy")));
						}
				 

									if(AttributeOnlyAllocationFilter.HasValue)
						res.Append(" && " + string.Format("AttributeOnlyAllocation == {0}",  AttributeOnlyAllocationFilter));						
 

									if(DoNotAllocateFilter.HasValue)
						res.Append(" && " + string.Format("DoNotAllocate == {0}",  DoNotAllocateFilter));						
 

									if(DoNotEXFilter.HasValue)
						res.Append(" && " + string.Format("DoNotEX == {0}",  DoNotEXFilter));						
 

									if(ImportCompleteFilter.HasValue)
						res.Append(" && " + string.Format("ImportComplete == {0}",  ImportCompleteFilter));						
 

									if(string.IsNullOrEmpty(WarehouseErrorFilter) == false)
						res.Append(" && " + string.Format("WarehouseError.Contains(\"{0}\")",  WarehouseErrorFilter));						
 

					if(SalesFactorFilter.HasValue)
						res.Append(" && " + string.Format("SalesFactor == {0}",  SalesFactorFilter.ToString()));				 

									if(string.IsNullOrEmpty(PreviousInvoiceNumberFilter) == false)
						res.Append(" && " + string.Format("PreviousInvoiceNumber.Contains(\"{0}\")",  PreviousInvoiceNumberFilter));						
 

									if(string.IsNullOrEmpty(PreviousInvoiceLineNumberFilter) == false)
						res.Append(" && " + string.Format("PreviousInvoiceLineNumber.Contains(\"{0}\")",  PreviousInvoiceLineNumberFilter));						
 

									if(string.IsNullOrEmpty(PreviousInvoiceItemNumberFilter) == false)
						res.Append(" && " + string.Format("PreviousInvoiceItemNumber.Contains(\"{0}\")",  PreviousInvoiceItemNumberFilter));						
 

									if(string.IsNullOrEmpty(EntryDataTypeFilter) == false)
						res.Append(" && " + string.Format("EntryDataType.Contains(\"{0}\")",  EntryDataTypeFilter));						
 

					if(UpgradeKeyFilter.HasValue)
						res.Append(" && " + string.Format("UpgradeKey == {0}",  UpgradeKeyFilter.ToString()));				 

									if(string.IsNullOrEmpty(PreviousInvoiceKeyFilter) == false)
						res.Append(" && " + string.Format("PreviousInvoiceKey.Contains(\"{0}\")",  PreviousInvoiceKeyFilter));						
 

									if(string.IsNullOrEmpty(xWarehouseErrorFilter) == false)
						res.Append(" && " + string.Format("xWarehouseError.Contains(\"{0}\")",  xWarehouseErrorFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<xcuda_Item> lst = null;
            using (var ctx = new xcuda_ItemRepository())
            {
                lst = await ctx.Getxcuda_ItemByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<xcuda_ItemExcelLine, List<xcuda_ItemExcelLine>>
            {
                dataToPrint = lst.Select(x => new xcuda_ItemExcelLine
                {
 
                    Amount_deducted_from_licence = x.Amount_deducted_from_licence ,
                    
 
                    Quantity_deducted_from_licence = x.Quantity_deducted_from_licence ,
                    
 
                    Licence_number = x.Licence_number ,
                    
 
                    Free_text_1 = x.Free_text_1 ,
                    
 
                    Free_text_2 = x.Free_text_2 ,
                    
 
                    LineNumber = x.LineNumber ,
                    
 
                    IsAssessed = x.IsAssessed ,
                    
 
                    DPQtyAllocated = x.DPQtyAllocated ,
                    
 
                    DFQtyAllocated = x.DFQtyAllocated ,
                    
 
                    EntryTimeStamp = x.EntryTimeStamp ,
                    
 
                    AttributeOnlyAllocation = x.AttributeOnlyAllocation ,
                    
 
                    DoNotAllocate = x.DoNotAllocate ,
                    
 
                    DoNotEX = x.DoNotEX ,
                    
 
                    ImportComplete = x.ImportComplete ,
                    
 
                    WarehouseError = x.WarehouseError ,
                    
 
                    SalesFactor = x.SalesFactor ,
                    
 
                    PreviousInvoiceNumber = x.PreviousInvoiceNumber ,
                    
 
                    PreviousInvoiceLineNumber = x.PreviousInvoiceLineNumber ,
                    
 
                    PreviousInvoiceItemNumber = x.PreviousInvoiceItemNumber ,
                    
 
                    EntryDataType = x.EntryDataType ,
                    
 
                    UpgradeKey = x.UpgradeKey ,
                    
 
                    PreviousInvoiceKey = x.PreviousInvoiceKey ,
                    
 
                    xWarehouseError = x.xWarehouseError 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class xcuda_ItemExcelLine
        {
		 
                    public string Amount_deducted_from_licence { get; set; } 
                    
 
                    public string Quantity_deducted_from_licence { get; set; } 
                    
 
                    public string Licence_number { get; set; } 
                    
 
                    public string Free_text_1 { get; set; } 
                    
 
                    public string Free_text_2 { get; set; } 
                    
 
                    public int LineNumber { get; set; } 
                    
 
                    public Nullable<bool> IsAssessed { get; set; } 
                    
 
                    public double DPQtyAllocated { get; set; } 
                    
 
                    public double DFQtyAllocated { get; set; } 
                    
 
                    public Nullable<System.DateTime> EntryTimeStamp { get; set; } 
                    
 
                    public Nullable<bool> AttributeOnlyAllocation { get; set; } 
                    
 
                    public Nullable<bool> DoNotAllocate { get; set; } 
                    
 
                    public Nullable<bool> DoNotEX { get; set; } 
                    
 
                    public bool ImportComplete { get; set; } 
                    
 
                    public string WarehouseError { get; set; } 
                    
 
                    public double SalesFactor { get; set; } 
                    
 
                    public string PreviousInvoiceNumber { get; set; } 
                    
 
                    public string PreviousInvoiceLineNumber { get; set; } 
                    
 
                    public string PreviousInvoiceItemNumber { get; set; } 
                    
 
                    public string EntryDataType { get; set; } 
                    
 
                    public Nullable<int> UpgradeKey { get; set; } 
                    
 
                    public string PreviousInvoiceKey { get; set; } 
                    
 
                    public string xWarehouseError { get; set; } 
                    
        }

		
    }
}
		
