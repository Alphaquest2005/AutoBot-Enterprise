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
    
	public partial class ParameterSetParametersViewModel_AutoGen : ViewModelBase<ParameterSetParametersViewModel_AutoGen>
	{

       private static readonly ParameterSetParametersViewModel_AutoGen instance;
       static ParameterSetParametersViewModel_AutoGen()
        {
            instance = new ParameterSetParametersViewModel_AutoGen();
        }

       public static ParameterSetParametersViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public ParameterSetParametersViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<ParameterSetParameters>(MessageToken.CurrentParameterSetParametersChanged, OnCurrentParameterSetParametersChanged);
            RegisterToReceiveMessages(MessageToken.ParameterSetParametersChanged, OnParameterSetParametersChanged);
			RegisterToReceiveMessages(MessageToken.ParameterSetParametersFilterExpressionChanged, OnParameterSetParametersFilterExpressionChanged);

 
			RegisterToReceiveMessages<Parameters>(MessageToken.CurrentParametersChanged, OnCurrentParametersChanged);
 
			RegisterToReceiveMessages<ParameterSet>(MessageToken.CurrentParameterSetChanged, OnCurrentParameterSetChanged);

 			// Recieve messages for Core Current Entities Changed
 

			ParameterSetParameters = new VirtualList<ParameterSetParameters>(vloader);
			ParameterSetParameters.LoadingStateChanged += ParameterSetParameters_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(ParameterSetParameters, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<ParameterSetParameters> _ParameterSetParameters = null;
        public VirtualList<ParameterSetParameters> ParameterSetParameters
        {
            get
            {
                return _ParameterSetParameters;
            }
            set
            {
                _ParameterSetParameters = value;
                NotifyPropertyChanged( x => x.ParameterSetParameters);
            }
        }

		 private void OnParameterSetParametersFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => ParameterSetParameters.Refresh()).ConfigureAwait(false);
            SelectedParameterSetParameters.Clear();
            NotifyPropertyChanged(x => SelectedParameterSetParameters);
            BeginSendMessage(MessageToken.SelectedParameterSetParametersChanged, new NotificationEventArgs(MessageToken.SelectedParameterSetParametersChanged));
        }

		void ParameterSetParameters_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (ParameterSetParameters.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => ParameterSetParameters);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("ParameterSetParameters | Error occured..." + ParameterSetParameters.LastLoadingError.Message);
                    NotifyPropertyChanged(x => ParameterSetParameters);
                    break;
            }
           
        }

		
		public readonly ParameterSetParametersVirturalListLoader vloader = new ParameterSetParametersVirturalListLoader();

		private ObservableCollection<ParameterSetParameters> _selectedParameterSetParameters = new ObservableCollection<ParameterSetParameters>();
        public ObservableCollection<ParameterSetParameters> SelectedParameterSetParameters
        {
            get
            {
                return _selectedParameterSetParameters;
            }
            set
            {
                _selectedParameterSetParameters = value;
				BeginSendMessage(MessageToken.SelectedParameterSetParametersChanged,
                                    new NotificationEventArgs(MessageToken.SelectedParameterSetParametersChanged));
				 NotifyPropertyChanged(x => SelectedParameterSetParameters);
            }
        }

        internal virtual void OnCurrentParameterSetParametersChanged(object sender, NotificationEventArgs<ParameterSetParameters> e)
        {
            if(BaseViewModel.Instance.CurrentParameterSetParameters != null) BaseViewModel.Instance.CurrentParameterSetParameters.PropertyChanged += CurrentParameterSetParameters__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentParameterSetParameters);
        }   

            void CurrentParameterSetParameters__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    //if (e.PropertyName == "AddParameters")
                   // {
                   //    if(Parameters.Contains(CurrentParameterSetParameters.Parameters) == false) Parameters.Add(CurrentParameterSetParameters.Parameters);
                    //}
                    //if (e.PropertyName == "AddParameterSet")
                   // {
                   //    if(ParameterSet.Contains(CurrentParameterSetParameters.ParameterSet) == false) ParameterSet.Add(CurrentParameterSetParameters.ParameterSet);
                    //}
                 } 
        internal virtual void OnParameterSetParametersChanged(object sender, NotificationEventArgs e)
        {
            _ParameterSetParameters.Refresh();
			NotifyPropertyChanged(x => this.ParameterSetParameters);
        }   


 	
		 internal virtual void OnCurrentParametersChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<Parameters> e)
			{
			if(ViewCurrentParameters == false) return;
			if (e.Data == null || e.Data.Id == null)
                {
                    vloader.FilterExpression = "None";
                }
                else
                {
				vloader.FilterExpression = string.Format("ParameterId == {0}", e.Data.Id.ToString());
                 }

				ParameterSetParameters.Refresh();
				NotifyPropertyChanged(x => this.ParameterSetParameters);
                // SendMessage(MessageToken.ParameterSetParametersChanged, new NotificationEventArgs(MessageToken.ParameterSetParametersChanged));
                                          
                BaseViewModel.Instance.CurrentParameterSetParameters = null;
			}
	
		 internal virtual void OnCurrentParameterSetChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<ParameterSet> e)
			{
			if(ViewCurrentParameterSet == false) return;
			if (e.Data == null || e.Data.Id == null)
                {
                    vloader.FilterExpression = "None";
                }
                else
                {
				vloader.FilterExpression = string.Format("ParameterSetId == {0}", e.Data.Id.ToString());
                 }

				ParameterSetParameters.Refresh();
				NotifyPropertyChanged(x => this.ParameterSetParameters);
                // SendMessage(MessageToken.ParameterSetParametersChanged, new NotificationEventArgs(MessageToken.ParameterSetParametersChanged));
                                          
                BaseViewModel.Instance.CurrentParameterSetParameters = null;
			}

  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
 	
		 bool _viewCurrentParameters = false;
         public bool ViewCurrentParameters
         {
             get
             {
                 return _viewCurrentParameters;
             }
             set
             {
                 _viewCurrentParameters = value;
                 NotifyPropertyChanged(x => x.ViewCurrentParameters);
                FilterData();
             }
         }
 	
		 bool _viewCurrentParameterSet = false;
         public bool ViewCurrentParameterSet
         {
             get
             {
                 return _viewCurrentParameterSet;
             }
             set
             {
                 _viewCurrentParameterSet = value;
                 NotifyPropertyChanged(x => x.ViewCurrentParameterSet);
                FilterData();
             }
         }
		public void ViewAll()
        {
		vloader.FilterExpression = "All";




			vloader.ClearNavigationExpression();
			_ParameterSetParameters.Refresh();
			NotifyPropertyChanged(x => this.ParameterSetParameters);
		}

		public async Task SelectAll()
        {
            IEnumerable<ParameterSetParameters> lst = null;
            using (var ctx = new ParameterSetParametersRepository())
            {
                lst = await ctx.GetParameterSetParametersByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedParameterSetParameters = new ObservableCollection<ParameterSetParameters>(lst);
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

			ParameterSetParameters.Refresh();
			NotifyPropertyChanged(x => this.ParameterSetParameters);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<ParameterSetParameters> lst = null;
            using (var ctx = new ParameterSetParametersRepository())
            {
                lst = await ctx.GetParameterSetParametersByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<ParameterSetParametersExcelLine, List<ParameterSetParametersExcelLine>>
            {
                dataToPrint = lst.Select(x => new ParameterSetParametersExcelLine
                {
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class ParameterSetParametersExcelLine
        {
		        }

		
    }
}
		