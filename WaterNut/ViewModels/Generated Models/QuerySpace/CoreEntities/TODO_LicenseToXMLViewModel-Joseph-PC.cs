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
    
	public partial class TODO_LicenseToXMLViewModel_AutoGen : ViewModelBase<TODO_LicenseToXMLViewModel_AutoGen>
	{

       private static readonly TODO_LicenseToXMLViewModel_AutoGen instance;
       static TODO_LicenseToXMLViewModel_AutoGen()
        {
            instance = new TODO_LicenseToXMLViewModel_AutoGen();
        }

       public static TODO_LicenseToXMLViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public TODO_LicenseToXMLViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<TODO_LicenseToXML>(MessageToken.CurrentTODO_LicenseToXMLChanged, OnCurrentTODO_LicenseToXMLChanged);
            RegisterToReceiveMessages(MessageToken.TODO_LicenseToXMLChanged, OnTODO_LicenseToXMLChanged);
			RegisterToReceiveMessages(MessageToken.TODO_LicenseToXMLFilterExpressionChanged, OnTODO_LicenseToXMLFilterExpressionChanged);


 			// Recieve messages for Core Current Entities Changed
 

			TODO_LicenseToXML = new VirtualList<TODO_LicenseToXML>(vloader);
			TODO_LicenseToXML.LoadingStateChanged += TODO_LicenseToXML_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(TODO_LicenseToXML, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<TODO_LicenseToXML> _TODO_LicenseToXML = null;
        public VirtualList<TODO_LicenseToXML> TODO_LicenseToXML
        {
            get
            {
                return _TODO_LicenseToXML;
            }
            set
            {
                _TODO_LicenseToXML = value;
            }
        }

		 private void OnTODO_LicenseToXMLFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			TODO_LicenseToXML.Refresh();
            SelectedTODO_LicenseToXML.Clear();
            NotifyPropertyChanged(x => SelectedTODO_LicenseToXML);
            BeginSendMessage(MessageToken.SelectedTODO_LicenseToXMLChanged, new NotificationEventArgs(MessageToken.SelectedTODO_LicenseToXMLChanged));
        }

		void TODO_LicenseToXML_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (TODO_LicenseToXML.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => TODO_LicenseToXML);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("TODO_LicenseToXML | Error occured..." + TODO_LicenseToXML.LastLoadingError.Message);
                    NotifyPropertyChanged(x => TODO_LicenseToXML);
                    break;
            }
           
        }

		
		public readonly TODO_LicenseToXMLVirturalListLoader vloader = new TODO_LicenseToXMLVirturalListLoader();

		private ObservableCollection<TODO_LicenseToXML> _selectedTODO_LicenseToXML = new ObservableCollection<TODO_LicenseToXML>();
        public ObservableCollection<TODO_LicenseToXML> SelectedTODO_LicenseToXML
        {
            get
            {
                return _selectedTODO_LicenseToXML;
            }
            set
            {
                _selectedTODO_LicenseToXML = value;
				BeginSendMessage(MessageToken.SelectedTODO_LicenseToXMLChanged,
                                    new NotificationEventArgs(MessageToken.SelectedTODO_LicenseToXMLChanged));
				 NotifyPropertyChanged(x => SelectedTODO_LicenseToXML);
            }
        }

        internal virtual void OnCurrentTODO_LicenseToXMLChanged(object sender, NotificationEventArgs<TODO_LicenseToXML> e)
        {
            if(BaseViewModel.Instance.CurrentTODO_LicenseToXML != null) BaseViewModel.Instance.CurrentTODO_LicenseToXML.PropertyChanged += CurrentTODO_LicenseToXML__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentTODO_LicenseToXML);
        }   

            void CurrentTODO_LicenseToXML__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                 } 
        internal virtual void OnTODO_LicenseToXMLChanged(object sender, NotificationEventArgs e)
        {
            _TODO_LicenseToXML.Refresh();
			NotifyPropertyChanged(x => this.TODO_LicenseToXML);
        }   


 
  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
		public void ViewAll()
        {
			vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";




			vloader.ClearNavigationExpression();
			_TODO_LicenseToXML.Refresh();
			NotifyPropertyChanged(x => this.TODO_LicenseToXML);
		}

		public async Task SelectAll()
        {
            IEnumerable<TODO_LicenseToXML> lst = null;
            using (var ctx = new TODO_LicenseToXMLRepository())
            {
                lst = await ctx.GetTODO_LicenseToXMLByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedTODO_LicenseToXML = new ObservableCollection<TODO_LicenseToXML>(lst);
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

 

		private string _country_of_origin_codeFilter;
        public string Country_of_origin_codeFilter
        {
            get
            {
                return _country_of_origin_codeFilter;
            }
            set
            {
                _country_of_origin_codeFilter = value;
				NotifyPropertyChanged(x => Country_of_origin_codeFilter);
                FilterData();
                
            }
        }	

 

		private string _uOMFilter;
        public string UOMFilter
        {
            get
            {
                return _uOMFilter;
            }
            set
            {
                _uOMFilter = value;
				NotifyPropertyChanged(x => UOMFilter);
                FilterData();
                
            }
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

 

		private string _licenseDescriptionFilter;
        public string LicenseDescriptionFilter
        {
            get
            {
                return _licenseDescriptionFilter;
            }
            set
            {
                _licenseDescriptionFilter = value;
				NotifyPropertyChanged(x => LicenseDescriptionFilter);
                FilterData();
                
            }
        }	

 

		private string _sourcefileFilter;
        public string sourcefileFilter
        {
            get
            {
                return _sourcefileFilter;
            }
            set
            {
                _sourcefileFilter = value;
				NotifyPropertyChanged(x => sourcefileFilter);
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

			TODO_LicenseToXML.Refresh();
			NotifyPropertyChanged(x => this.TODO_LicenseToXML);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(TariffCodeFilter) == false)
						res.Append(" && " + string.Format("TariffCode.Contains(\"{0}\")",  TariffCodeFilter));						
 

					if(QuantityFilter.HasValue)
						res.Append(" && " + string.Format("Quantity == {0}",  QuantityFilter.ToString()));				 

									if(string.IsNullOrEmpty(Declarant_Reference_NumberFilter) == false)
						res.Append(" && " + string.Format("Declarant_Reference_Number.Contains(\"{0}\")",  Declarant_Reference_NumberFilter));						
 

									if(string.IsNullOrEmpty(Country_of_origin_codeFilter) == false)
						res.Append(" && " + string.Format("Country_of_origin_code.Contains(\"{0}\")",  Country_of_origin_codeFilter));						
 

									if(string.IsNullOrEmpty(UOMFilter) == false)
						res.Append(" && " + string.Format("UOM.Contains(\"{0}\")",  UOMFilter));						
 

									if(string.IsNullOrEmpty(EntryDataIdFilter) == false)
						res.Append(" && " + string.Format("EntryDataId.Contains(\"{0}\")",  EntryDataIdFilter));						
 

									if(string.IsNullOrEmpty(LicenseDescriptionFilter) == false)
						res.Append(" && " + string.Format("LicenseDescription.Contains(\"{0}\")",  LicenseDescriptionFilter));						
 

									if(string.IsNullOrEmpty(sourcefileFilter) == false)
						res.Append(" && " + string.Format("sourcefile.Contains(\"{0}\")",  sourcefileFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<TODO_LicenseToXML> lst = null;
            using (var ctx = new TODO_LicenseToXMLRepository())
            {
                lst = await ctx.GetTODO_LicenseToXMLByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<TODO_LicenseToXMLExcelLine, List<TODO_LicenseToXMLExcelLine>>
            {
                dataToPrint = lst.Select(x => new TODO_LicenseToXMLExcelLine
                {
 
                    TariffCode = x.TariffCode ,
                    
 
                    Quantity = x.Quantity ,
                    
 
                    Declarant_Reference_Number = x.Declarant_Reference_Number ,
                    
 
                    Country_of_origin_code = x.Country_of_origin_code ,
                    
 
                    UOM = x.UOM ,
                    
 
                    EntryDataId = x.EntryDataId ,
                    
 
                    LicenseDescription = x.LicenseDescription ,
                    
 
                    sourcefile = x.sourcefile 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public class TODO_LicenseToXMLExcelLine
        {
		 
                    public string TariffCode { get; set; } 
                    
 
                    public Nullable<double> Quantity { get; set; } 
                    
 
                    public string Declarant_Reference_Number { get; set; } 
                    
 
                    public string Country_of_origin_code { get; set; } 
                    
 
                    public string UOM { get; set; } 
                    
 
                    public string EntryDataId { get; set; } 
                    
 
                    public string LicenseDescription { get; set; } 
                    
 
                    public string sourcefile { get; set; } 
                    
        }

		
    }
}
		