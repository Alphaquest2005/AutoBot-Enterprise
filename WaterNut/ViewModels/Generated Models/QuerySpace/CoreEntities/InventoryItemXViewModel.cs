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
    
	public partial class InventoryItemXViewModel_AutoGen : ViewModelBase<InventoryItemXViewModel_AutoGen>
	{

       private static readonly InventoryItemXViewModel_AutoGen instance;
       static InventoryItemXViewModel_AutoGen()
        {
            instance = new InventoryItemXViewModel_AutoGen();
        }

       public static InventoryItemXViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public InventoryItemXViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<InventoryItemX>(MessageToken.CurrentInventoryItemXChanged, OnCurrentInventoryItemXChanged);
            RegisterToReceiveMessages(MessageToken.InventoryItemXChanged, OnInventoryItemXChanged);
			RegisterToReceiveMessages(MessageToken.InventoryItemXFilterExpressionChanged, OnInventoryItemXFilterExpressionChanged);

 
			RegisterToReceiveMessages<ApplicationSettings>(MessageToken.CurrentApplicationSettingsChanged, OnCurrentApplicationSettingsChanged);

 			// Recieve messages for Core Current Entities Changed
 

			InventoryItemX = new VirtualList<InventoryItemX>(vloader);
			InventoryItemX.LoadingStateChanged += InventoryItemX_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(InventoryItemX, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<InventoryItemX> _InventoryItemX = null;
        public VirtualList<InventoryItemX> InventoryItemX
        {
            get
            {
                return _InventoryItemX;
            }
            set
            {
                _InventoryItemX = value;
                NotifyPropertyChanged( x => x.InventoryItemX);
            }
        }

		 private void OnInventoryItemXFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => InventoryItemX.Refresh()).ConfigureAwait(false);
            SelectedInventoryItemX.Clear();
            NotifyPropertyChanged(x => SelectedInventoryItemX);
            BeginSendMessage(MessageToken.SelectedInventoryItemXChanged, new NotificationEventArgs(MessageToken.SelectedInventoryItemXChanged));
        }

		void InventoryItemX_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (InventoryItemX.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => InventoryItemX);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("InventoryItemX | Error occured..." + InventoryItemX.LastLoadingError.Message);
                    NotifyPropertyChanged(x => InventoryItemX);
                    break;
            }
           
        }

		
		public readonly InventoryItemXVirturalListLoader vloader = new InventoryItemXVirturalListLoader();

		private ObservableCollection<InventoryItemX> _selectedInventoryItemX = new ObservableCollection<InventoryItemX>();
        public ObservableCollection<InventoryItemX> SelectedInventoryItemX
        {
            get
            {
                return _selectedInventoryItemX;
            }
            set
            {
                _selectedInventoryItemX = value;
				BeginSendMessage(MessageToken.SelectedInventoryItemXChanged,
                                    new NotificationEventArgs(MessageToken.SelectedInventoryItemXChanged));
				 NotifyPropertyChanged(x => SelectedInventoryItemX);
            }
        }

        internal virtual void OnCurrentInventoryItemXChanged(object sender, NotificationEventArgs<InventoryItemX> e)
        {
            if(BaseViewModel.Instance.CurrentInventoryItemX != null) BaseViewModel.Instance.CurrentInventoryItemX.PropertyChanged += CurrentInventoryItemX__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentInventoryItemX);
        }   

            void CurrentInventoryItemX__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    //if (e.PropertyName == "AddApplicationSettings")
                   // {
                   //    if(ApplicationSettings.Contains(CurrentInventoryItemX.ApplicationSettings) == false) ApplicationSettings.Add(CurrentInventoryItemX.ApplicationSettings);
                    //}
                 } 
        internal virtual void OnInventoryItemXChanged(object sender, NotificationEventArgs e)
        {
            _InventoryItemX.Refresh();
			NotifyPropertyChanged(x => this.InventoryItemX);
        }   


 	
		 internal virtual void OnCurrentApplicationSettingsChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<ApplicationSettings> e)
			{
			if(ViewCurrentApplicationSettings == false) return;
			if (e.Data == null || e.Data.ApplicationSettingsId == null)
                {
                    vloader.FilterExpression = "None";
                }
                else
                {
				vloader.FilterExpression = string.Format("ApplicationSettingsId == {0}", e.Data.ApplicationSettingsId.ToString());
                 }

				InventoryItemX.Refresh();
				NotifyPropertyChanged(x => this.InventoryItemX);
                // SendMessage(MessageToken.InventoryItemXChanged, new NotificationEventArgs(MessageToken.InventoryItemXChanged));
                                          
                BaseViewModel.Instance.CurrentInventoryItemX = null;
			}

  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
 	
		 bool _viewCurrentApplicationSettings = false;
         public bool ViewCurrentApplicationSettings
         {
             get
             {
                 return _viewCurrentApplicationSettings;
             }
             set
             {
                 _viewCurrentApplicationSettings = value;
                 NotifyPropertyChanged(x => x.ViewCurrentApplicationSettings);
                FilterData();
             }
         }
		public void ViewAll()
        {
			vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";




			vloader.ClearNavigationExpression();
			_InventoryItemX.Refresh();
			NotifyPropertyChanged(x => this.InventoryItemX);
		}

		public async Task SelectAll()
        {
            IEnumerable<InventoryItemX> lst = null;
            using (var ctx = new InventoryItemXRepository())
            {
                lst = await ctx.GetInventoryItemXByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedInventoryItemX = new ObservableCollection<InventoryItemX>(lst);
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

 

		private string _descriptionFilter;
        public string DescriptionFilter
        {
            get
            {
                return _descriptionFilter;
            }
            set
            {
                _descriptionFilter = value;
				NotifyPropertyChanged(x => DescriptionFilter);
                FilterData();
                
            }
        }	

 

		private string _categoryFilter;
        public string CategoryFilter
        {
            get
            {
                return _categoryFilter;
            }
            set
            {
                _categoryFilter = value;
				NotifyPropertyChanged(x => CategoryFilter);
                FilterData();
                
            }
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

 

		private string _suppUnitCode2Filter;
        public string SuppUnitCode2Filter
        {
            get
            {
                return _suppUnitCode2Filter;
            }
            set
            {
                _suppUnitCode2Filter = value;
				NotifyPropertyChanged(x => SuppUnitCode2Filter);
                FilterData();
                
            }
        }	

 

		private Double? _suppQtyFilter;
        public Double? SuppQtyFilter
        {
            get
            {
                return _suppQtyFilter;
            }
            set
            {
                _suppQtyFilter = value;
				NotifyPropertyChanged(x => SuppQtyFilter);
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

			InventoryItemX.Refresh();
			NotifyPropertyChanged(x => this.InventoryItemX);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(ItemNumberFilter) == false)
						res.Append(" && " + string.Format("ItemNumber.Contains(\"{0}\")",  ItemNumberFilter));						
 

									if(string.IsNullOrEmpty(DescriptionFilter) == false)
						res.Append(" && " + string.Format("Description.Contains(\"{0}\")",  DescriptionFilter));						
 

									if(string.IsNullOrEmpty(CategoryFilter) == false)
						res.Append(" && " + string.Format("Category.Contains(\"{0}\")",  CategoryFilter));						
 

									if(string.IsNullOrEmpty(TariffCodeFilter) == false)
						res.Append(" && " + string.Format("TariffCode.Contains(\"{0}\")",  TariffCodeFilter));						
 

 

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
				 

									if(string.IsNullOrEmpty(SuppUnitCode2Filter) == false)
						res.Append(" && " + string.Format("SuppUnitCode2.Contains(\"{0}\")",  SuppUnitCode2Filter));						
 

					if(SuppQtyFilter.HasValue)
						res.Append(" && " + string.Format("SuppQty == {0}",  SuppQtyFilter.ToString()));							return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<InventoryItemX> lst = null;
            using (var ctx = new InventoryItemXRepository())
            {
                lst = await ctx.GetInventoryItemXByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<InventoryItemXExcelLine, List<InventoryItemXExcelLine>>
            {
                dataToPrint = lst.Select(x => new InventoryItemXExcelLine
                {
 
                    ItemNumber = x.ItemNumber ,
                    
 
                    Description = x.Description ,
                    
 
                    Category = x.Category ,
                    
 
                    TariffCode = x.TariffCode ,
                    
 
                    EntryTimeStamp = x.EntryTimeStamp ,
                    
 
                    SuppUnitCode2 = x.SuppUnitCode2 ,
                    
 
                    SuppQty = x.SuppQty 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class InventoryItemXExcelLine
        {
		 
                    public string ItemNumber { get; set; } 
                    
 
                    public string Description { get; set; } 
                    
 
                    public string Category { get; set; } 
                    
 
                    public string TariffCode { get; set; } 
                    
 
                    public Nullable<System.DateTime> EntryTimeStamp { get; set; } 
                    
 
                    public string SuppUnitCode2 { get; set; } 
                    
 
                    public Nullable<double> SuppQty { get; set; } 
                    
        }

		
    }
}
		
