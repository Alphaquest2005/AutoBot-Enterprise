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

using OCR.Client.Entities;
using OCR.Client.Repositories;
//using WaterNut.Client.Repositories;
        
using CoreEntities.Client.Entities;


namespace WaterNut.QuerySpace.OCR.ViewModels
{
    
	public partial class OCR_PartLineFieldsViewModel_AutoGen : ViewModelBase<OCR_PartLineFieldsViewModel_AutoGen>
	{

       private static readonly OCR_PartLineFieldsViewModel_AutoGen instance;
       static OCR_PartLineFieldsViewModel_AutoGen()
        {
            instance = new OCR_PartLineFieldsViewModel_AutoGen();
        }

       public static OCR_PartLineFieldsViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public OCR_PartLineFieldsViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<OCR_PartLineFields>(MessageToken.CurrentOCR_PartLineFieldsChanged, OnCurrentOCR_PartLineFieldsChanged);
            RegisterToReceiveMessages(MessageToken.OCR_PartLineFieldsChanged, OnOCR_PartLineFieldsChanged);
			RegisterToReceiveMessages(MessageToken.OCR_PartLineFieldsFilterExpressionChanged, OnOCR_PartLineFieldsFilterExpressionChanged);


 			// Recieve messages for Core Current Entities Changed
 

			OCR_PartLineFields = new VirtualList<OCR_PartLineFields>(vloader);
			OCR_PartLineFields.LoadingStateChanged += OCR_PartLineFields_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(OCR_PartLineFields, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<OCR_PartLineFields> _OCR_PartLineFields = null;
        public VirtualList<OCR_PartLineFields> OCR_PartLineFields
        {
            get
            {
                return _OCR_PartLineFields;
            }
            set
            {
                _OCR_PartLineFields = value;
                NotifyPropertyChanged( x => x.OCR_PartLineFields);
            }
        }

		 private void OnOCR_PartLineFieldsFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => OCR_PartLineFields.Refresh()).ConfigureAwait(false);
            SelectedOCR_PartLineFields.Clear();
            NotifyPropertyChanged(x => SelectedOCR_PartLineFields);
            BeginSendMessage(MessageToken.SelectedOCR_PartLineFieldsChanged, new NotificationEventArgs(MessageToken.SelectedOCR_PartLineFieldsChanged));
        }

		void OCR_PartLineFields_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (OCR_PartLineFields.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => OCR_PartLineFields);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("OCR_PartLineFields | Error occured..." + OCR_PartLineFields.LastLoadingError.Message);
                    NotifyPropertyChanged(x => OCR_PartLineFields);
                    break;
            }
           
        }

		
		public readonly OCR_PartLineFieldsVirturalListLoader vloader = new OCR_PartLineFieldsVirturalListLoader();

		private ObservableCollection<OCR_PartLineFields> _selectedOCR_PartLineFields = new ObservableCollection<OCR_PartLineFields>();
        public ObservableCollection<OCR_PartLineFields> SelectedOCR_PartLineFields
        {
            get
            {
                return _selectedOCR_PartLineFields;
            }
            set
            {
                _selectedOCR_PartLineFields = value;
				BeginSendMessage(MessageToken.SelectedOCR_PartLineFieldsChanged,
                                    new NotificationEventArgs(MessageToken.SelectedOCR_PartLineFieldsChanged));
				 NotifyPropertyChanged(x => SelectedOCR_PartLineFields);
            }
        }

        internal virtual void OnCurrentOCR_PartLineFieldsChanged(object sender, NotificationEventArgs<OCR_PartLineFields> e)
        {
            if(BaseViewModel.Instance.CurrentOCR_PartLineFields != null) BaseViewModel.Instance.CurrentOCR_PartLineFields.PropertyChanged += CurrentOCR_PartLineFields__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentOCR_PartLineFields);
        }   

            void CurrentOCR_PartLineFields__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                 } 
        internal virtual void OnOCR_PartLineFieldsChanged(object sender, NotificationEventArgs e)
        {
            _OCR_PartLineFields.Refresh();
			NotifyPropertyChanged(x => this.OCR_PartLineFields);
        }   


 
  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
		public void ViewAll()
        {
		vloader.FilterExpression = "All";




			vloader.ClearNavigationExpression();
			_OCR_PartLineFields.Refresh();
			NotifyPropertyChanged(x => this.OCR_PartLineFields);
		}

		public async Task SelectAll()
        {
            IEnumerable<OCR_PartLineFields> lst = null;
            using (var ctx = new OCR_PartLineFieldsRepository())
            {
                lst = await ctx.GetOCR_PartLineFieldsByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedOCR_PartLineFields = new ObservableCollection<OCR_PartLineFields>(lst);
        }

 

		private string _invoiceFilter;
        public string InvoiceFilter
        {
            get
            {
                return _invoiceFilter;
            }
            set
            {
                _invoiceFilter = value;
				NotifyPropertyChanged(x => InvoiceFilter);
                FilterData();
                
            }
        }	

 

		private string _partFilter;
        public string PartFilter
        {
            get
            {
                return _partFilter;
            }
            set
            {
                _partFilter = value;
				NotifyPropertyChanged(x => PartFilter);
                FilterData();
                
            }
        }	

 

		private string _lineFilter;
        public string LineFilter
        {
            get
            {
                return _lineFilter;
            }
            set
            {
                _lineFilter = value;
				NotifyPropertyChanged(x => LineFilter);
                FilterData();
                
            }
        }	

 

		private string _regExFilter;
        public string RegExFilter
        {
            get
            {
                return _regExFilter;
            }
            set
            {
                _regExFilter = value;
				NotifyPropertyChanged(x => RegExFilter);
                FilterData();
                
            }
        }	

 

		private string _keyFilter;
        public string KeyFilter
        {
            get
            {
                return _keyFilter;
            }
            set
            {
                _keyFilter = value;
				NotifyPropertyChanged(x => KeyFilter);
                FilterData();
                
            }
        }	

 

		private string _fieldFilter;
        public string FieldFilter
        {
            get
            {
                return _fieldFilter;
            }
            set
            {
                _fieldFilter = value;
				NotifyPropertyChanged(x => FieldFilter);
                FilterData();
                
            }
        }	

 

		private string _entityTypeFilter;
        public string EntityTypeFilter
        {
            get
            {
                return _entityTypeFilter;
            }
            set
            {
                _entityTypeFilter = value;
				NotifyPropertyChanged(x => EntityTypeFilter);
                FilterData();
                
            }
        }	

 

		private Boolean? _isRequiredFilter;
        public Boolean? IsRequiredFilter
        {
            get
            {
                return _isRequiredFilter;
            }
            set
            {
                _isRequiredFilter = value;
				NotifyPropertyChanged(x => IsRequiredFilter);
                FilterData();
                
            }
        }	

 

		private string _dataTypeFilter;
        public string DataTypeFilter
        {
            get
            {
                return _dataTypeFilter;
            }
            set
            {
                _dataTypeFilter = value;
				NotifyPropertyChanged(x => DataTypeFilter);
                FilterData();
                
            }
        }	

 

		private string _valueFilter;
        public string ValueFilter
        {
            get
            {
                return _valueFilter;
            }
            set
            {
                _valueFilter = value;
				NotifyPropertyChanged(x => ValueFilter);
                FilterData();
                
            }
        }	

 

		private Boolean? _appendValuesFilter;
        public Boolean? AppendValuesFilter
        {
            get
            {
                return _appendValuesFilter;
            }
            set
            {
                _appendValuesFilter = value;
				NotifyPropertyChanged(x => AppendValuesFilter);
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

			OCR_PartLineFields.Refresh();
			NotifyPropertyChanged(x => this.OCR_PartLineFields);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(InvoiceFilter) == false)
						res.Append(" && " + string.Format("Invoice.Contains(\"{0}\")",  InvoiceFilter));						
 

									if(string.IsNullOrEmpty(PartFilter) == false)
						res.Append(" && " + string.Format("Part.Contains(\"{0}\")",  PartFilter));						
 

									if(string.IsNullOrEmpty(LineFilter) == false)
						res.Append(" && " + string.Format("Line.Contains(\"{0}\")",  LineFilter));						
 

									if(string.IsNullOrEmpty(RegExFilter) == false)
						res.Append(" && " + string.Format("RegEx.Contains(\"{0}\")",  RegExFilter));						
 

									if(string.IsNullOrEmpty(KeyFilter) == false)
						res.Append(" && " + string.Format("Key.Contains(\"{0}\")",  KeyFilter));						
 

									if(string.IsNullOrEmpty(FieldFilter) == false)
						res.Append(" && " + string.Format("Field.Contains(\"{0}\")",  FieldFilter));						
 

									if(string.IsNullOrEmpty(EntityTypeFilter) == false)
						res.Append(" && " + string.Format("EntityType.Contains(\"{0}\")",  EntityTypeFilter));						
 

									if(IsRequiredFilter.HasValue)
						res.Append(" && " + string.Format("IsRequired == {0}",  IsRequiredFilter));						
 

									if(string.IsNullOrEmpty(DataTypeFilter) == false)
						res.Append(" && " + string.Format("DataType.Contains(\"{0}\")",  DataTypeFilter));						
 

									if(string.IsNullOrEmpty(ValueFilter) == false)
						res.Append(" && " + string.Format("Value.Contains(\"{0}\")",  ValueFilter));						
 

									if(AppendValuesFilter.HasValue)
						res.Append(" && " + string.Format("AppendValues == {0}",  AppendValuesFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<OCR_PartLineFields> lst = null;
            using (var ctx = new OCR_PartLineFieldsRepository())
            {
                lst = await ctx.GetOCR_PartLineFieldsByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<OCR_PartLineFieldsExcelLine, List<OCR_PartLineFieldsExcelLine>>
            {
                dataToPrint = lst.Select(x => new OCR_PartLineFieldsExcelLine
                {
 
                    Invoice = x.Invoice ,
                    
 
                    Part = x.Part ,
                    
 
                    Line = x.Line ,
                    
 
                    RegEx = x.RegEx ,
                    
 
                    Key = x.Key ,
                    
 
                    Field = x.Field ,
                    
 
                    EntityType = x.EntityType ,
                    
 
                    IsRequired = x.IsRequired ,
                    
 
                    DataType = x.DataType ,
                    
 
                    Value = x.Value ,
                    
 
                    AppendValues = x.AppendValues 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public class OCR_PartLineFieldsExcelLine
        {
		 
                    public string Invoice { get; set; } 
                    
 
                    public string Part { get; set; } 
                    
 
                    public string Line { get; set; } 
                    
 
                    public string RegEx { get; set; } 
                    
 
                    public string Key { get; set; } 
                    
 
                    public string Field { get; set; } 
                    
 
                    public string EntityType { get; set; } 
                    
 
                    public Nullable<bool> IsRequired { get; set; } 
                    
 
                    public string DataType { get; set; } 
                    
 
                    public string Value { get; set; } 
                    
 
                    public Nullable<bool> AppendValues { get; set; } 
                    
        }

		
    }
}
		