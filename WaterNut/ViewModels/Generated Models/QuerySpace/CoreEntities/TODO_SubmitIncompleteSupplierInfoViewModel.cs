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
    
	public partial class TODO_SubmitIncompleteSupplierInfoViewModel_AutoGen : ViewModelBase<TODO_SubmitIncompleteSupplierInfoViewModel_AutoGen>
	{

       private static readonly TODO_SubmitIncompleteSupplierInfoViewModel_AutoGen instance;
       static TODO_SubmitIncompleteSupplierInfoViewModel_AutoGen()
        {
            instance = new TODO_SubmitIncompleteSupplierInfoViewModel_AutoGen();
        }

       public static TODO_SubmitIncompleteSupplierInfoViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public TODO_SubmitIncompleteSupplierInfoViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<TODO_SubmitIncompleteSupplierInfo>(MessageToken.CurrentTODO_SubmitIncompleteSupplierInfoChanged, OnCurrentTODO_SubmitIncompleteSupplierInfoChanged);
            RegisterToReceiveMessages(MessageToken.TODO_SubmitIncompleteSupplierInfoChanged, OnTODO_SubmitIncompleteSupplierInfoChanged);
			RegisterToReceiveMessages(MessageToken.TODO_SubmitIncompleteSupplierInfoFilterExpressionChanged, OnTODO_SubmitIncompleteSupplierInfoFilterExpressionChanged);


 			// Recieve messages for Core Current Entities Changed
 

			TODO_SubmitIncompleteSupplierInfo = new VirtualList<TODO_SubmitIncompleteSupplierInfo>(vloader);
			TODO_SubmitIncompleteSupplierInfo.LoadingStateChanged += TODO_SubmitIncompleteSupplierInfo_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(TODO_SubmitIncompleteSupplierInfo, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<TODO_SubmitIncompleteSupplierInfo> _TODO_SubmitIncompleteSupplierInfo = null;
        public VirtualList<TODO_SubmitIncompleteSupplierInfo> TODO_SubmitIncompleteSupplierInfo
        {
            get
            {
                return _TODO_SubmitIncompleteSupplierInfo;
            }
            set
            {
                _TODO_SubmitIncompleteSupplierInfo = value;
                NotifyPropertyChanged( x => x.TODO_SubmitIncompleteSupplierInfo);
            }
        }

		 private void OnTODO_SubmitIncompleteSupplierInfoFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => TODO_SubmitIncompleteSupplierInfo.Refresh()).ConfigureAwait(false);
            SelectedTODO_SubmitIncompleteSupplierInfo.Clear();
            NotifyPropertyChanged(x => SelectedTODO_SubmitIncompleteSupplierInfo);
            BeginSendMessage(MessageToken.SelectedTODO_SubmitIncompleteSupplierInfoChanged, new NotificationEventArgs(MessageToken.SelectedTODO_SubmitIncompleteSupplierInfoChanged));
        }

		void TODO_SubmitIncompleteSupplierInfo_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (TODO_SubmitIncompleteSupplierInfo.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => TODO_SubmitIncompleteSupplierInfo);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("TODO_SubmitIncompleteSupplierInfo | Error occured..." + TODO_SubmitIncompleteSupplierInfo.LastLoadingError.Message);
                    NotifyPropertyChanged(x => TODO_SubmitIncompleteSupplierInfo);
                    break;
            }
           
        }

		
		public readonly TODO_SubmitIncompleteSupplierInfoVirturalListLoader vloader = new TODO_SubmitIncompleteSupplierInfoVirturalListLoader();

		private ObservableCollection<TODO_SubmitIncompleteSupplierInfo> _selectedTODO_SubmitIncompleteSupplierInfo = new ObservableCollection<TODO_SubmitIncompleteSupplierInfo>();
        public ObservableCollection<TODO_SubmitIncompleteSupplierInfo> SelectedTODO_SubmitIncompleteSupplierInfo
        {
            get
            {
                return _selectedTODO_SubmitIncompleteSupplierInfo;
            }
            set
            {
                _selectedTODO_SubmitIncompleteSupplierInfo = value;
				BeginSendMessage(MessageToken.SelectedTODO_SubmitIncompleteSupplierInfoChanged,
                                    new NotificationEventArgs(MessageToken.SelectedTODO_SubmitIncompleteSupplierInfoChanged));
				 NotifyPropertyChanged(x => SelectedTODO_SubmitIncompleteSupplierInfo);
            }
        }

        internal virtual void OnCurrentTODO_SubmitIncompleteSupplierInfoChanged(object sender, NotificationEventArgs<TODO_SubmitIncompleteSupplierInfo> e)
        {
            if(BaseViewModel.Instance.CurrentTODO_SubmitIncompleteSupplierInfo != null) BaseViewModel.Instance.CurrentTODO_SubmitIncompleteSupplierInfo.PropertyChanged += CurrentTODO_SubmitIncompleteSupplierInfo__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentTODO_SubmitIncompleteSupplierInfo);
        }   

            void CurrentTODO_SubmitIncompleteSupplierInfo__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                 } 
        internal virtual void OnTODO_SubmitIncompleteSupplierInfoChanged(object sender, NotificationEventArgs e)
        {
            _TODO_SubmitIncompleteSupplierInfo.Refresh();
			NotifyPropertyChanged(x => this.TODO_SubmitIncompleteSupplierInfo);
        }   


 
  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
		public void ViewAll()
        {
			vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";




			vloader.ClearNavigationExpression();
			_TODO_SubmitIncompleteSupplierInfo.Refresh();
			NotifyPropertyChanged(x => this.TODO_SubmitIncompleteSupplierInfo);
		}

		public async Task SelectAll()
        {
            IEnumerable<TODO_SubmitIncompleteSupplierInfo> lst = null;
            using (var ctx = new TODO_SubmitIncompleteSupplierInfoRepository())
            {
                lst = await ctx.GetTODO_SubmitIncompleteSupplierInfoByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedTODO_SubmitIncompleteSupplierInfo = new ObservableCollection<TODO_SubmitIncompleteSupplierInfo>(lst);
        }

 

		private string _supplierCodeFilter;
        public string SupplierCodeFilter
        {
            get
            {
                return _supplierCodeFilter;
            }
            set
            {
                _supplierCodeFilter = value;
				NotifyPropertyChanged(x => SupplierCodeFilter);
                FilterData();
                
            }
        }	

 

		private string _countryCodeFilter;
        public string CountryCodeFilter
        {
            get
            {
                return _countryCodeFilter;
            }
            set
            {
                _countryCodeFilter = value;
				NotifyPropertyChanged(x => CountryCodeFilter);
                FilterData();
                
            }
        }	

 

		private string _supplierNameFilter;
        public string SupplierNameFilter
        {
            get
            {
                return _supplierNameFilter;
            }
            set
            {
                _supplierNameFilter = value;
				NotifyPropertyChanged(x => SupplierNameFilter);
                FilterData();
                
            }
        }	

 

		private string _streetFilter;
        public string StreetFilter
        {
            get
            {
                return _streetFilter;
            }
            set
            {
                _streetFilter = value;
				NotifyPropertyChanged(x => StreetFilter);
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

			TODO_SubmitIncompleteSupplierInfo.Refresh();
			NotifyPropertyChanged(x => this.TODO_SubmitIncompleteSupplierInfo);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(SupplierCodeFilter) == false)
						res.Append(" && " + string.Format("SupplierCode.Contains(\"{0}\")",  SupplierCodeFilter));						
 

									if(string.IsNullOrEmpty(CountryCodeFilter) == false)
						res.Append(" && " + string.Format("CountryCode.Contains(\"{0}\")",  CountryCodeFilter));						
 

									if(string.IsNullOrEmpty(SupplierNameFilter) == false)
						res.Append(" && " + string.Format("SupplierName.Contains(\"{0}\")",  SupplierNameFilter));						
 

									if(string.IsNullOrEmpty(StreetFilter) == false)
						res.Append(" && " + string.Format("Street.Contains(\"{0}\")",  StreetFilter));						
 

									if(string.IsNullOrEmpty(EntryDataIdFilter) == false)
						res.Append(" && " + string.Format("EntryDataId.Contains(\"{0}\")",  EntryDataIdFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<TODO_SubmitIncompleteSupplierInfo> lst = null;
            using (var ctx = new TODO_SubmitIncompleteSupplierInfoRepository())
            {
                lst = await ctx.GetTODO_SubmitIncompleteSupplierInfoByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<TODO_SubmitIncompleteSupplierInfoExcelLine, List<TODO_SubmitIncompleteSupplierInfoExcelLine>>
            {
                dataToPrint = lst.Select(x => new TODO_SubmitIncompleteSupplierInfoExcelLine
                {
 
                    SupplierCode = x.SupplierCode ,
                    
 
                    CountryCode = x.CountryCode ,
                    
 
                    SupplierName = x.SupplierName ,
                    
 
                    Street = x.Street ,
                    
 
                    EntryDataId = x.EntryDataId 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class TODO_SubmitIncompleteSupplierInfoExcelLine
        {
		 
                    public string SupplierCode { get; set; } 
                    
 
                    public string CountryCode { get; set; } 
                    
 
                    public string SupplierName { get; set; } 
                    
 
                    public string Street { get; set; } 
                    
 
                    public string EntryDataId { get; set; } 
                    
        }

		
    }
}
		
