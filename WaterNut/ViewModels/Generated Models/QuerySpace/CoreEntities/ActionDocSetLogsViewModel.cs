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
    
	public partial class ActionDocSetLogsViewModel_AutoGen : ViewModelBase<ActionDocSetLogsViewModel_AutoGen>
	{

       private static readonly ActionDocSetLogsViewModel_AutoGen instance;
       static ActionDocSetLogsViewModel_AutoGen()
        {
            instance = new ActionDocSetLogsViewModel_AutoGen();
        }

       public static ActionDocSetLogsViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public ActionDocSetLogsViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<ActionDocSetLogs>(MessageToken.CurrentActionDocSetLogsChanged, OnCurrentActionDocSetLogsChanged);
            RegisterToReceiveMessages(MessageToken.ActionDocSetLogsChanged, OnActionDocSetLogsChanged);
			RegisterToReceiveMessages(MessageToken.ActionDocSetLogsFilterExpressionChanged, OnActionDocSetLogsFilterExpressionChanged);

 
			RegisterToReceiveMessages<Actions>(MessageToken.CurrentActionsChanged, OnCurrentActionsChanged);

 			// Recieve messages for Core Current Entities Changed
 

			ActionDocSetLogs = new VirtualList<ActionDocSetLogs>(vloader);
			ActionDocSetLogs.LoadingStateChanged += ActionDocSetLogs_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(ActionDocSetLogs, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<ActionDocSetLogs> _ActionDocSetLogs = null;
        public VirtualList<ActionDocSetLogs> ActionDocSetLogs
        {
            get
            {
                return _ActionDocSetLogs;
            }
            set
            {
                _ActionDocSetLogs = value;
            }
        }

		 private void OnActionDocSetLogsFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			ActionDocSetLogs.Refresh();
            SelectedActionDocSetLogs.Clear();
            NotifyPropertyChanged(x => SelectedActionDocSetLogs);
            BeginSendMessage(MessageToken.SelectedActionDocSetLogsChanged, new NotificationEventArgs(MessageToken.SelectedActionDocSetLogsChanged));
        }

		void ActionDocSetLogs_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (ActionDocSetLogs.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => ActionDocSetLogs);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("ActionDocSetLogs | Error occured..." + ActionDocSetLogs.LastLoadingError.Message);
                    NotifyPropertyChanged(x => ActionDocSetLogs);
                    break;
            }
           
        }

		
		public readonly ActionDocSetLogsVirturalListLoader vloader = new ActionDocSetLogsVirturalListLoader();

		private ObservableCollection<ActionDocSetLogs> _selectedActionDocSetLogs = new ObservableCollection<ActionDocSetLogs>();
        public ObservableCollection<ActionDocSetLogs> SelectedActionDocSetLogs
        {
            get
            {
                return _selectedActionDocSetLogs;
            }
            set
            {
                _selectedActionDocSetLogs = value;
				BeginSendMessage(MessageToken.SelectedActionDocSetLogsChanged,
                                    new NotificationEventArgs(MessageToken.SelectedActionDocSetLogsChanged));
				 NotifyPropertyChanged(x => SelectedActionDocSetLogs);
            }
        }

        internal virtual void OnCurrentActionDocSetLogsChanged(object sender, NotificationEventArgs<ActionDocSetLogs> e)
        {
            if(BaseViewModel.Instance.CurrentActionDocSetLogs != null) BaseViewModel.Instance.CurrentActionDocSetLogs.PropertyChanged += CurrentActionDocSetLogs__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentActionDocSetLogs);
        }   

            void CurrentActionDocSetLogs__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    //if (e.PropertyName == "AddActions")
                   // {
                   //    if(Actions.Contains(CurrentActionDocSetLogs.Actions) == false) Actions.Add(CurrentActionDocSetLogs.Actions);
                    //}
                 } 
        internal virtual void OnActionDocSetLogsChanged(object sender, NotificationEventArgs e)
        {
            _ActionDocSetLogs.Refresh();
			NotifyPropertyChanged(x => this.ActionDocSetLogs);
        }   


 	
		 internal virtual void OnCurrentActionsChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<Actions> e)
			{
			if(ViewCurrentActions == false) return;
			if (e.Data == null || e.Data.Id == null)
                {
                    vloader.FilterExpression = "None";
                }
                else
                {
				vloader.FilterExpression = string.Format("ActonId == {0}", e.Data.Id.ToString());
                 }

				ActionDocSetLogs.Refresh();
				NotifyPropertyChanged(x => this.ActionDocSetLogs);
                // SendMessage(MessageToken.ActionDocSetLogsChanged, new NotificationEventArgs(MessageToken.ActionDocSetLogsChanged));
                                          
                BaseViewModel.Instance.CurrentActionDocSetLogs = null;
			}

  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
 	
		 bool _viewCurrentActions = false;
         public bool ViewCurrentActions
         {
             get
             {
                 return _viewCurrentActions;
             }
             set
             {
                 _viewCurrentActions = value;
                 NotifyPropertyChanged(x => x.ViewCurrentActions);
                FilterData();
             }
         }
		public void ViewAll()
        {
		vloader.FilterExpression = "All";




			vloader.ClearNavigationExpression();
			_ActionDocSetLogs.Refresh();
			NotifyPropertyChanged(x => this.ActionDocSetLogs);
		}

		public async Task SelectAll()
        {
            IEnumerable<ActionDocSetLogs> lst = null;
            using (var ctx = new ActionDocSetLogsRepository())
            {
                lst = await ctx.GetActionDocSetLogsByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedActionDocSetLogs = new ObservableCollection<ActionDocSetLogs>(lst);
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

			ActionDocSetLogs.Refresh();
			NotifyPropertyChanged(x => this.ActionDocSetLogs);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<ActionDocSetLogs> lst = null;
            using (var ctx = new ActionDocSetLogsRepository())
            {
                lst = await ctx.GetActionDocSetLogsByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<ActionDocSetLogsExcelLine, List<ActionDocSetLogsExcelLine>>
            {
                dataToPrint = lst.Select(x => new ActionDocSetLogsExcelLine
                {
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public class ActionDocSetLogsExcelLine
        {
		        }

		
    }
}
		