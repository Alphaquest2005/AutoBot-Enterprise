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
    
	public partial class SystemDocumentSetViewModel_AutoGen : ViewModelBase<SystemDocumentSetViewModel_AutoGen>
	{

       private static readonly SystemDocumentSetViewModel_AutoGen instance;
       static SystemDocumentSetViewModel_AutoGen()
        {
            instance = new SystemDocumentSetViewModel_AutoGen();
        }

       public static SystemDocumentSetViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public SystemDocumentSetViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<SystemDocumentSet>(MessageToken.CurrentSystemDocumentSetChanged, OnCurrentSystemDocumentSetChanged);
            RegisterToReceiveMessages(MessageToken.SystemDocumentSetsChanged, OnSystemDocumentSetsChanged);
			RegisterToReceiveMessages(MessageToken.SystemDocumentSetsFilterExpressionChanged, OnSystemDocumentSetsFilterExpressionChanged);


 			// Recieve messages for Core Current Entities Changed
 

			SystemDocumentSets = new VirtualList<SystemDocumentSet>(vloader);
			SystemDocumentSets.LoadingStateChanged += SystemDocumentSets_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(SystemDocumentSets, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<SystemDocumentSet> _SystemDocumentSets = null;
        public VirtualList<SystemDocumentSet> SystemDocumentSets
        {
            get
            {
                return _SystemDocumentSets;
            }
            set
            {
                _SystemDocumentSets = value;
            }
        }

		 private void OnSystemDocumentSetsFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			SystemDocumentSets.Refresh();
            SelectedSystemDocumentSets.Clear();
            NotifyPropertyChanged(x => SelectedSystemDocumentSets);
            BeginSendMessage(MessageToken.SelectedSystemDocumentSetsChanged, new NotificationEventArgs(MessageToken.SelectedSystemDocumentSetsChanged));
        }

		void SystemDocumentSets_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (SystemDocumentSets.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => SystemDocumentSets);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("SystemDocumentSets | Error occured..." + SystemDocumentSets.LastLoadingError.Message);
                    NotifyPropertyChanged(x => SystemDocumentSets);
                    break;
            }
           
        }

		
		public readonly SystemDocumentSetVirturalListLoader vloader = new SystemDocumentSetVirturalListLoader();

		private ObservableCollection<SystemDocumentSet> _selectedSystemDocumentSets = new ObservableCollection<SystemDocumentSet>();
        public ObservableCollection<SystemDocumentSet> SelectedSystemDocumentSets
        {
            get
            {
                return _selectedSystemDocumentSets;
            }
            set
            {
                _selectedSystemDocumentSets = value;
				BeginSendMessage(MessageToken.SelectedSystemDocumentSetsChanged,
                                    new NotificationEventArgs(MessageToken.SelectedSystemDocumentSetsChanged));
				 NotifyPropertyChanged(x => SelectedSystemDocumentSets);
            }
        }

        internal virtual void OnCurrentSystemDocumentSetChanged(object sender, NotificationEventArgs<SystemDocumentSet> e)
        {
            if(BaseViewModel.Instance.CurrentSystemDocumentSet != null) BaseViewModel.Instance.CurrentSystemDocumentSet.PropertyChanged += CurrentSystemDocumentSet__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentSystemDocumentSet);
        }   

            void CurrentSystemDocumentSet__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                 } 
        internal virtual void OnSystemDocumentSetsChanged(object sender, NotificationEventArgs e)
        {
            _SystemDocumentSets.Refresh();
			NotifyPropertyChanged(x => this.SystemDocumentSets);
        }   


 
  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
		public void ViewAll()
        {
		vloader.FilterExpression = "All";




			vloader.ClearNavigationExpression();
			_SystemDocumentSets.Refresh();
			NotifyPropertyChanged(x => this.SystemDocumentSets);
		}

		public async Task SelectAll()
        {
            IEnumerable<SystemDocumentSet> lst = null;
            using (var ctx = new SystemDocumentSetRepository())
            {
                lst = await ctx.GetSystemDocumentSetsByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedSystemDocumentSets = new ObservableCollection<SystemDocumentSet>(lst);
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

			SystemDocumentSets.Refresh();
			NotifyPropertyChanged(x => this.SystemDocumentSets);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<SystemDocumentSet> lst = null;
            using (var ctx = new SystemDocumentSetRepository())
            {
                lst = await ctx.GetSystemDocumentSetsByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<SystemDocumentSetExcelLine, List<SystemDocumentSetExcelLine>>
            {
                dataToPrint = lst.Select(x => new SystemDocumentSetExcelLine
                {
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public class SystemDocumentSetExcelLine
        {
		        }

		
    }
}
		