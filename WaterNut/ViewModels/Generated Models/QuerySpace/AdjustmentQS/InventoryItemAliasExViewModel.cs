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
    
	public partial class InventoryItemAliasExViewModel_AutoGen : ViewModelBase<InventoryItemAliasExViewModel_AutoGen>
	{

       private static readonly InventoryItemAliasExViewModel_AutoGen instance;
       static InventoryItemAliasExViewModel_AutoGen()
        {
            instance = new InventoryItemAliasExViewModel_AutoGen();
        }

       public static InventoryItemAliasExViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public InventoryItemAliasExViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<InventoryItemAliasEx>(MessageToken.CurrentInventoryItemAliasExChanged, OnCurrentInventoryItemAliasExChanged);
            RegisterToReceiveMessages(MessageToken.InventoryItemAliasExesChanged, OnInventoryItemAliasExesChanged);
			RegisterToReceiveMessages(MessageToken.InventoryItemAliasExesFilterExpressionChanged, OnInventoryItemAliasExesFilterExpressionChanged);

 
			RegisterToReceiveMessages<InventoryItemsEx>(MessageToken.CurrentInventoryItemsExChanged, OnCurrentInventoryItemsExChanged);

 			// Recieve messages for Core Current Entities Changed
 

			InventoryItemAliasExes = new VirtualList<InventoryItemAliasEx>(vloader);
			InventoryItemAliasExes.LoadingStateChanged += InventoryItemAliasExes_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(InventoryItemAliasExes, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<InventoryItemAliasEx> _InventoryItemAliasExes = null;
        public VirtualList<InventoryItemAliasEx> InventoryItemAliasExes
        {
            get
            {
                return _InventoryItemAliasExes;
            }
            set
            {
                _InventoryItemAliasExes = value;
            }
        }

		 private void OnInventoryItemAliasExesFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			InventoryItemAliasExes.Refresh();
            SelectedInventoryItemAliasExes.Clear();
            NotifyPropertyChanged(x => SelectedInventoryItemAliasExes);
            BeginSendMessage(MessageToken.SelectedInventoryItemAliasExesChanged, new NotificationEventArgs(MessageToken.SelectedInventoryItemAliasExesChanged));
        }

		void InventoryItemAliasExes_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (InventoryItemAliasExes.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => InventoryItemAliasExes);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("InventoryItemAliasExes | Error occured..." + InventoryItemAliasExes.LastLoadingError.Message);
                    NotifyPropertyChanged(x => InventoryItemAliasExes);
                    break;
            }
           
        }

		
		public readonly InventoryItemAliasExVirturalListLoader vloader = new InventoryItemAliasExVirturalListLoader();

		private ObservableCollection<InventoryItemAliasEx> _selectedInventoryItemAliasExes = new ObservableCollection<InventoryItemAliasEx>();
        public ObservableCollection<InventoryItemAliasEx> SelectedInventoryItemAliasExes
        {
            get
            {
                return _selectedInventoryItemAliasExes;
            }
            set
            {
                _selectedInventoryItemAliasExes = value;
				BeginSendMessage(MessageToken.SelectedInventoryItemAliasExesChanged,
                                    new NotificationEventArgs(MessageToken.SelectedInventoryItemAliasExesChanged));
				 NotifyPropertyChanged(x => SelectedInventoryItemAliasExes);
            }
        }

        internal void OnCurrentInventoryItemAliasExChanged(object sender, NotificationEventArgs<InventoryItemAliasEx> e)
        {
            if(BaseViewModel.Instance.CurrentInventoryItemAliasEx != null) BaseViewModel.Instance.CurrentInventoryItemAliasEx.PropertyChanged += CurrentInventoryItemAliasEx__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentInventoryItemAliasEx);
        }   

            void CurrentInventoryItemAliasEx__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    //if (e.PropertyName == "AddInventoryItemsEx")
                   // {
                   //    if(InventoryItemsExes.Contains(CurrentInventoryItemAliasEx.InventoryItemsEx) == false) InventoryItemsExes.Add(CurrentInventoryItemAliasEx.InventoryItemsEx);
                    //}
                 } 
        internal void OnInventoryItemAliasExesChanged(object sender, NotificationEventArgs e)
        {
            _InventoryItemAliasExes.Refresh();
			NotifyPropertyChanged(x => this.InventoryItemAliasExes);
        }   


 	
		 internal void OnCurrentInventoryItemsExChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<InventoryItemsEx> e)
			{
			if(ViewCurrentInventoryItemsEx == false) return;
			if (e.Data == null || e.Data.ItemNumber == null)
                {
                    vloader.FilterExpression = "None";
                }
                else
                {
				
				vloader.FilterExpression = string.Format("ItemNumber == \"{0}\"", e.Data.ItemNumber.ToString());
                }

				InventoryItemAliasExes.Refresh();
				NotifyPropertyChanged(x => this.InventoryItemAliasExes);
                // SendMessage(MessageToken.InventoryItemAliasExesChanged, new NotificationEventArgs(MessageToken.InventoryItemAliasExesChanged));
                			}

  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
 	
		 bool _viewCurrentInventoryItemsEx = false;
         public bool ViewCurrentInventoryItemsEx
         {
             get
             {
                 return _viewCurrentInventoryItemsEx;
             }
             set
             {
                 _viewCurrentInventoryItemsEx = value;
                 NotifyPropertyChanged(x => x.ViewCurrentInventoryItemsEx);
                FilterData();
             }
         }
		public void ViewAll()
        {
			vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";




			vloader.ClearNavigationExpression();
			_InventoryItemAliasExes.Refresh();
			NotifyPropertyChanged(x => this.InventoryItemAliasExes);
		}

		public async Task SelectAll()
        {
            IEnumerable<InventoryItemAliasEx> lst = null;
            using (var ctx = new InventoryItemAliasExRepository())
            {
                lst = await ctx.GetInventoryItemAliasExesByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedInventoryItemAliasExes = new ObservableCollection<InventoryItemAliasEx>(lst);
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

 

		private string _aliasNameFilter;
        public string AliasNameFilter
        {
            get
            {
                return _aliasNameFilter;
            }
            set
            {
                _aliasNameFilter = value;
				NotifyPropertyChanged(x => AliasNameFilter);
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

			InventoryItemAliasExes.Refresh();
			NotifyPropertyChanged(x => this.InventoryItemAliasExes);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(ItemNumberFilter) == false)
						res.Append(" && " + string.Format("ItemNumber.Contains(\"{0}\")",  ItemNumberFilter));						
 

									if(string.IsNullOrEmpty(AliasNameFilter) == false)
						res.Append(" && " + string.Format("AliasName.Contains(\"{0}\")",  AliasNameFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<InventoryItemAliasEx> lst = null;
            using (var ctx = new InventoryItemAliasExRepository())
            {
                lst = await ctx.GetInventoryItemAliasExesByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<InventoryItemAliasExExcelLine, List<InventoryItemAliasExExcelLine>>
            {
                dataToPrint = lst.Select(x => new InventoryItemAliasExExcelLine
                {
 
                    ItemNumber = x.ItemNumber ,
                    
 
                    AliasName = x.AliasName 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public class InventoryItemAliasExExcelLine
        {
		 
                    public string ItemNumber { get; set; } 
                    
 
                    public string AliasName { get; set; } 
                    
        }

		
    }
}
		