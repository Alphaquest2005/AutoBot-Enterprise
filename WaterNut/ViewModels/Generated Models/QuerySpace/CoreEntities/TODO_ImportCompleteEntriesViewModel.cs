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
    
	public partial class TODO_ImportCompleteEntriesViewModel_AutoGen : ViewModelBase<TODO_ImportCompleteEntriesViewModel_AutoGen>
	{

       private static readonly TODO_ImportCompleteEntriesViewModel_AutoGen instance;
       static TODO_ImportCompleteEntriesViewModel_AutoGen()
        {
            instance = new TODO_ImportCompleteEntriesViewModel_AutoGen();
        }

       public static TODO_ImportCompleteEntriesViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public TODO_ImportCompleteEntriesViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<TODO_ImportCompleteEntries>(MessageToken.CurrentTODO_ImportCompleteEntriesChanged, OnCurrentTODO_ImportCompleteEntriesChanged);
            RegisterToReceiveMessages(MessageToken.TODO_ImportCompleteEntriesChanged, OnTODO_ImportCompleteEntriesChanged);
			RegisterToReceiveMessages(MessageToken.TODO_ImportCompleteEntriesFilterExpressionChanged, OnTODO_ImportCompleteEntriesFilterExpressionChanged);


 			// Recieve messages for Core Current Entities Changed
 

			TODO_ImportCompleteEntries = new VirtualList<TODO_ImportCompleteEntries>(vloader);
			TODO_ImportCompleteEntries.LoadingStateChanged += TODO_ImportCompleteEntries_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(TODO_ImportCompleteEntries, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<TODO_ImportCompleteEntries> _TODO_ImportCompleteEntries = null;
        public VirtualList<TODO_ImportCompleteEntries> TODO_ImportCompleteEntries
        {
            get
            {
                return _TODO_ImportCompleteEntries;
            }
            set
            {
                _TODO_ImportCompleteEntries = value;
            }
        }

		 private void OnTODO_ImportCompleteEntriesFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			TODO_ImportCompleteEntries.Refresh();
            SelectedTODO_ImportCompleteEntries.Clear();
            NotifyPropertyChanged(x => SelectedTODO_ImportCompleteEntries);
            BeginSendMessage(MessageToken.SelectedTODO_ImportCompleteEntriesChanged, new NotificationEventArgs(MessageToken.SelectedTODO_ImportCompleteEntriesChanged));
        }

		void TODO_ImportCompleteEntries_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (TODO_ImportCompleteEntries.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => TODO_ImportCompleteEntries);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("TODO_ImportCompleteEntries | Error occured..." + TODO_ImportCompleteEntries.LastLoadingError.Message);
                    NotifyPropertyChanged(x => TODO_ImportCompleteEntries);
                    break;
            }
           
        }

		
		public readonly TODO_ImportCompleteEntriesVirturalListLoader vloader = new TODO_ImportCompleteEntriesVirturalListLoader();

		private ObservableCollection<TODO_ImportCompleteEntries> _selectedTODO_ImportCompleteEntries = new ObservableCollection<TODO_ImportCompleteEntries>();
        public ObservableCollection<TODO_ImportCompleteEntries> SelectedTODO_ImportCompleteEntries
        {
            get
            {
                return _selectedTODO_ImportCompleteEntries;
            }
            set
            {
                _selectedTODO_ImportCompleteEntries = value;
				BeginSendMessage(MessageToken.SelectedTODO_ImportCompleteEntriesChanged,
                                    new NotificationEventArgs(MessageToken.SelectedTODO_ImportCompleteEntriesChanged));
				 NotifyPropertyChanged(x => SelectedTODO_ImportCompleteEntries);
            }
        }

        internal virtual void OnCurrentTODO_ImportCompleteEntriesChanged(object sender, NotificationEventArgs<TODO_ImportCompleteEntries> e)
        {
            if(BaseViewModel.Instance.CurrentTODO_ImportCompleteEntries != null) BaseViewModel.Instance.CurrentTODO_ImportCompleteEntries.PropertyChanged += CurrentTODO_ImportCompleteEntries__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentTODO_ImportCompleteEntries);
        }   

            void CurrentTODO_ImportCompleteEntries__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                 } 
        internal virtual void OnTODO_ImportCompleteEntriesChanged(object sender, NotificationEventArgs e)
        {
            _TODO_ImportCompleteEntries.Refresh();
			NotifyPropertyChanged(x => this.TODO_ImportCompleteEntries);
        }   


 
  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
		public void ViewAll()
        {
			vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";




			vloader.ClearNavigationExpression();
			_TODO_ImportCompleteEntries.Refresh();
			NotifyPropertyChanged(x => this.TODO_ImportCompleteEntries);
		}

		public async Task SelectAll()
        {
            IEnumerable<TODO_ImportCompleteEntries> lst = null;
            using (var ctx = new TODO_ImportCompleteEntriesRepository())
            {
                lst = await ctx.GetTODO_ImportCompleteEntriesByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedTODO_ImportCompleteEntries = new ObservableCollection<TODO_ImportCompleteEntries>(lst);
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

			TODO_ImportCompleteEntries.Refresh();
			NotifyPropertyChanged(x => this.TODO_ImportCompleteEntries);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(EntryDataIdFilter) == false)
						res.Append(" && " + string.Format("EntryDataId.Contains(\"{0}\")",  EntryDataIdFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<TODO_ImportCompleteEntries> lst = null;
            using (var ctx = new TODO_ImportCompleteEntriesRepository())
            {
                lst = await ctx.GetTODO_ImportCompleteEntriesByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<TODO_ImportCompleteEntriesExcelLine, List<TODO_ImportCompleteEntriesExcelLine>>
            {
                dataToPrint = lst.Select(x => new TODO_ImportCompleteEntriesExcelLine
                {
 
                    EntryDataId = x.EntryDataId 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public class TODO_ImportCompleteEntriesExcelLine
        {
		 
                    public string EntryDataId { get; set; } 
                    
        }

		
    }
}
		