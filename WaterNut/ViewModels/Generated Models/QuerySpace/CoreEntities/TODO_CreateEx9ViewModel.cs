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
    
	public partial class TODO_CreateEx9ViewModel_AutoGen : ViewModelBase<TODO_CreateEx9ViewModel_AutoGen>
	{

       private static readonly TODO_CreateEx9ViewModel_AutoGen instance;
       static TODO_CreateEx9ViewModel_AutoGen()
        {
            instance = new TODO_CreateEx9ViewModel_AutoGen();
        }

       public static TODO_CreateEx9ViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public TODO_CreateEx9ViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<TODO_CreateEx9>(MessageToken.CurrentTODO_CreateEx9Changed, OnCurrentTODO_CreateEx9Changed);
            RegisterToReceiveMessages(MessageToken.TODO_CreateEx9Changed, OnTODO_CreateEx9Changed);
			RegisterToReceiveMessages(MessageToken.TODO_CreateEx9FilterExpressionChanged, OnTODO_CreateEx9FilterExpressionChanged);


 			// Recieve messages for Core Current Entities Changed
 

			TODO_CreateEx9 = new VirtualList<TODO_CreateEx9>(vloader);
			TODO_CreateEx9.LoadingStateChanged += TODO_CreateEx9_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(TODO_CreateEx9, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<TODO_CreateEx9> _TODO_CreateEx9 = null;
        public VirtualList<TODO_CreateEx9> TODO_CreateEx9
        {
            get
            {
                return _TODO_CreateEx9;
            }
            set
            {
                _TODO_CreateEx9 = value;
                NotifyPropertyChanged( x => x.TODO_CreateEx9);
            }
        }

		 private void OnTODO_CreateEx9FilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => TODO_CreateEx9.Refresh()).ConfigureAwait(false);
            SelectedTODO_CreateEx9.Clear();
            NotifyPropertyChanged(x => SelectedTODO_CreateEx9);
            BeginSendMessage(MessageToken.SelectedTODO_CreateEx9Changed, new NotificationEventArgs(MessageToken.SelectedTODO_CreateEx9Changed));
        }

		void TODO_CreateEx9_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (TODO_CreateEx9.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => TODO_CreateEx9);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("TODO_CreateEx9 | Error occured..." + TODO_CreateEx9.LastLoadingError.Message);
                    NotifyPropertyChanged(x => TODO_CreateEx9);
                    break;
            }
           
        }

		
		public readonly TODO_CreateEx9VirturalListLoader vloader = new TODO_CreateEx9VirturalListLoader();

		private ObservableCollection<TODO_CreateEx9> _selectedTODO_CreateEx9 = new ObservableCollection<TODO_CreateEx9>();
        public ObservableCollection<TODO_CreateEx9> SelectedTODO_CreateEx9
        {
            get
            {
                return _selectedTODO_CreateEx9;
            }
            set
            {
                _selectedTODO_CreateEx9 = value;
				BeginSendMessage(MessageToken.SelectedTODO_CreateEx9Changed,
                                    new NotificationEventArgs(MessageToken.SelectedTODO_CreateEx9Changed));
				 NotifyPropertyChanged(x => SelectedTODO_CreateEx9);
            }
        }

        internal virtual void OnCurrentTODO_CreateEx9Changed(object sender, NotificationEventArgs<TODO_CreateEx9> e)
        {
            if(BaseViewModel.Instance.CurrentTODO_CreateEx9 != null) BaseViewModel.Instance.CurrentTODO_CreateEx9.PropertyChanged += CurrentTODO_CreateEx9__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentTODO_CreateEx9);
        }   

            void CurrentTODO_CreateEx9__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                 } 
        internal virtual void OnTODO_CreateEx9Changed(object sender, NotificationEventArgs e)
        {
            _TODO_CreateEx9.Refresh();
			NotifyPropertyChanged(x => this.TODO_CreateEx9);
        }   


 
  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
		public void ViewAll()
        {
			vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";




			vloader.ClearNavigationExpression();
			_TODO_CreateEx9.Refresh();
			NotifyPropertyChanged(x => this.TODO_CreateEx9);
		}

		public async Task SelectAll()
        {
            IEnumerable<TODO_CreateEx9> lst = null;
            using (var ctx = new TODO_CreateEx9Repository())
            {
                lst = await ctx.GetTODO_CreateEx9ByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedTODO_CreateEx9 = new ObservableCollection<TODO_CreateEx9>(lst);
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

 

		private Double? _pQuantityFilter;
        public Double? pQuantityFilter
        {
            get
            {
                return _pQuantityFilter;
            }
            set
            {
                _pQuantityFilter = value;
				NotifyPropertyChanged(x => pQuantityFilter);
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

			TODO_CreateEx9.Refresh();
			NotifyPropertyChanged(x => this.TODO_CreateEx9);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(ItemNumberFilter) == false)
						res.Append(" && " + string.Format("ItemNumber.Contains(\"{0}\")",  ItemNumberFilter));						
 

					if(QtyAllocatedFilter.HasValue)
						res.Append(" && " + string.Format("QtyAllocated == {0}",  QtyAllocatedFilter.ToString()));				 

					if(pQuantityFilter.HasValue)
						res.Append(" && " + string.Format("pQuantity == {0}",  pQuantityFilter.ToString()));							return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<TODO_CreateEx9> lst = null;
            using (var ctx = new TODO_CreateEx9Repository())
            {
                lst = await ctx.GetTODO_CreateEx9ByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<TODO_CreateEx9ExcelLine, List<TODO_CreateEx9ExcelLine>>
            {
                dataToPrint = lst.Select(x => new TODO_CreateEx9ExcelLine
                {
 
                    ItemNumber = x.ItemNumber ,
                    
 
                    QtyAllocated = x.QtyAllocated ,
                    
 
                    pQuantity = x.pQuantity 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class TODO_CreateEx9ExcelLine
        {
		 
                    public string ItemNumber { get; set; } 
                    
 
                    public Nullable<double> QtyAllocated { get; set; } 
                    
 
                    public Nullable<double> pQuantity { get; set; } 
                    
        }

		
    }
}
		
