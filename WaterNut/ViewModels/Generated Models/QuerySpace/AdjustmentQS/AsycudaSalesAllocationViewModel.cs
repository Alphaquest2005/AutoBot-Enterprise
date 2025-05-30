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
    
	public partial class AsycudaSalesAllocationViewModel_AutoGen : ViewModelBase<AsycudaSalesAllocationViewModel_AutoGen>
	{

       private static readonly AsycudaSalesAllocationViewModel_AutoGen instance;
       static AsycudaSalesAllocationViewModel_AutoGen()
        {
            instance = new AsycudaSalesAllocationViewModel_AutoGen();
        }

       public static AsycudaSalesAllocationViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public AsycudaSalesAllocationViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<AsycudaSalesAllocation>(MessageToken.CurrentAsycudaSalesAllocationChanged, OnCurrentAsycudaSalesAllocationChanged);
            RegisterToReceiveMessages(MessageToken.AsycudaSalesAllocationsChanged, OnAsycudaSalesAllocationsChanged);
			RegisterToReceiveMessages(MessageToken.AsycudaSalesAllocationsFilterExpressionChanged, OnAsycudaSalesAllocationsFilterExpressionChanged);

 
			RegisterToReceiveMessages<EntryDataDetail>(MessageToken.CurrentEntryDataDetailChanged, OnCurrentEntryDataDetailChanged);
 
			RegisterToReceiveMessages<xcuda_Item>(MessageToken.Currentxcuda_ItemChanged, OnCurrentxcuda_ItemChanged);

 			// Recieve messages for Core Current Entities Changed
 

			AsycudaSalesAllocations = new VirtualList<AsycudaSalesAllocation>(vloader);
			AsycudaSalesAllocations.LoadingStateChanged += AsycudaSalesAllocations_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(AsycudaSalesAllocations, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<AsycudaSalesAllocation> _AsycudaSalesAllocations = null;
        public VirtualList<AsycudaSalesAllocation> AsycudaSalesAllocations
        {
            get
            {
                return _AsycudaSalesAllocations;
            }
            set
            {
                _AsycudaSalesAllocations = value;
                NotifyPropertyChanged( x => x.AsycudaSalesAllocations);
            }
        }

		 private void OnAsycudaSalesAllocationsFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => AsycudaSalesAllocations.Refresh()).ConfigureAwait(false);
            SelectedAsycudaSalesAllocations.Clear();
            NotifyPropertyChanged(x => SelectedAsycudaSalesAllocations);
            BeginSendMessage(MessageToken.SelectedAsycudaSalesAllocationsChanged, new NotificationEventArgs(MessageToken.SelectedAsycudaSalesAllocationsChanged));
        }

		void AsycudaSalesAllocations_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (AsycudaSalesAllocations.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => AsycudaSalesAllocations);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("AsycudaSalesAllocations | Error occured..." + AsycudaSalesAllocations.LastLoadingError.Message);
                    NotifyPropertyChanged(x => AsycudaSalesAllocations);
                    break;
            }
           
        }

		
		public readonly AsycudaSalesAllocationVirturalListLoader vloader = new AsycudaSalesAllocationVirturalListLoader();

		private ObservableCollection<AsycudaSalesAllocation> _selectedAsycudaSalesAllocations = new ObservableCollection<AsycudaSalesAllocation>();
        public ObservableCollection<AsycudaSalesAllocation> SelectedAsycudaSalesAllocations
        {
            get
            {
                return _selectedAsycudaSalesAllocations;
            }
            set
            {
                _selectedAsycudaSalesAllocations = value;
				BeginSendMessage(MessageToken.SelectedAsycudaSalesAllocationsChanged,
                                    new NotificationEventArgs(MessageToken.SelectedAsycudaSalesAllocationsChanged));
				 NotifyPropertyChanged(x => SelectedAsycudaSalesAllocations);
            }
        }

        internal virtual void OnCurrentAsycudaSalesAllocationChanged(object sender, NotificationEventArgs<AsycudaSalesAllocation> e)
        {
            if(BaseViewModel.Instance.CurrentAsycudaSalesAllocation != null) BaseViewModel.Instance.CurrentAsycudaSalesAllocation.PropertyChanged += CurrentAsycudaSalesAllocation__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentAsycudaSalesAllocation);
        }   

            void CurrentAsycudaSalesAllocation__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    //if (e.PropertyName == "AddEntryDataDetail")
                   // {
                   //    if(EntryDataDetails.Contains(CurrentAsycudaSalesAllocation.EntryDataDetail) == false) EntryDataDetails.Add(CurrentAsycudaSalesAllocation.EntryDataDetail);
                    //}
                    //if (e.PropertyName == "Addxcuda_Item")
                   // {
                   //    if(xcuda_Item.Contains(CurrentAsycudaSalesAllocation.xcuda_Item) == false) xcuda_Item.Add(CurrentAsycudaSalesAllocation.xcuda_Item);
                    //}
                 } 
        internal virtual void OnAsycudaSalesAllocationsChanged(object sender, NotificationEventArgs e)
        {
            _AsycudaSalesAllocations.Refresh();
			NotifyPropertyChanged(x => this.AsycudaSalesAllocations);
        }   


 	
		 internal virtual void OnCurrentEntryDataDetailChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<EntryDataDetail> e)
			{
			if(ViewCurrentEntryDataDetail == false) return;
			if (e.Data == null || e.Data.EntryDataDetailsId == null)
                {
                    vloader.FilterExpression = "None";
                }
                else
                {
				vloader.FilterExpression = string.Format("EntryDataDetailsId == {0}", e.Data.EntryDataDetailsId.ToString());
                 }

				AsycudaSalesAllocations.Refresh();
				NotifyPropertyChanged(x => this.AsycudaSalesAllocations);
                // SendMessage(MessageToken.AsycudaSalesAllocationsChanged, new NotificationEventArgs(MessageToken.AsycudaSalesAllocationsChanged));
                			}
	
		 internal virtual void OnCurrentxcuda_ItemChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<xcuda_Item> e)
			{
			if(ViewCurrentxcuda_Item == false) return;
			if (e.Data == null || e.Data.Item_Id == null)
                {
                    vloader.FilterExpression = "None";
                }
                else
                {
				vloader.FilterExpression = string.Format("PreviousItem_Id == {0}", e.Data.Item_Id.ToString());
                 }

				AsycudaSalesAllocations.Refresh();
				NotifyPropertyChanged(x => this.AsycudaSalesAllocations);
                // SendMessage(MessageToken.AsycudaSalesAllocationsChanged, new NotificationEventArgs(MessageToken.AsycudaSalesAllocationsChanged));
                			}

  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
 	
		 bool _viewCurrentEntryDataDetail = false;
         public bool ViewCurrentEntryDataDetail
         {
             get
             {
                 return _viewCurrentEntryDataDetail;
             }
             set
             {
                 _viewCurrentEntryDataDetail = value;
                 NotifyPropertyChanged(x => x.ViewCurrentEntryDataDetail);
                FilterData();
             }
         }
 	
		 bool _viewCurrentxcuda_Item = false;
         public bool ViewCurrentxcuda_Item
         {
             get
             {
                 return _viewCurrentxcuda_Item;
             }
             set
             {
                 _viewCurrentxcuda_Item = value;
                 NotifyPropertyChanged(x => x.ViewCurrentxcuda_Item);
                FilterData();
             }
         }
		public void ViewAll()
        {
		vloader.FilterExpression = "All";




			vloader.ClearNavigationExpression();
			_AsycudaSalesAllocations.Refresh();
			NotifyPropertyChanged(x => this.AsycudaSalesAllocations);
		}

		public async Task SelectAll()
        {
            IEnumerable<AsycudaSalesAllocation> lst = null;
            using (var ctx = new AsycudaSalesAllocationRepository())
            {
                lst = await ctx.GetAsycudaSalesAllocationsByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedAsycudaSalesAllocations = new ObservableCollection<AsycudaSalesAllocation>(lst);
        }

 

		private string _statusFilter;
        public string StatusFilter
        {
            get
            {
                return _statusFilter;
            }
            set
            {
                _statusFilter = value;
				NotifyPropertyChanged(x => StatusFilter);
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

 

		private Int32? _eANumberFilter;
        public Int32? EANumberFilter
        {
            get
            {
                return _eANumberFilter;
            }
            set
            {
                _eANumberFilter = value;
				NotifyPropertyChanged(x => EANumberFilter);
                FilterData();
                
            }
        }	

 

		private Int32? _sANumberFilter;
        public Int32? SANumberFilter
        {
            get
            {
                return _sANumberFilter;
            }
            set
            {
                _sANumberFilter = value;
				NotifyPropertyChanged(x => SANumberFilter);
                FilterData();
                
            }
        }	

 

		private string _xStatusFilter;
        public string xStatusFilter
        {
            get
            {
                return _xStatusFilter;
            }
            set
            {
                _xStatusFilter = value;
				NotifyPropertyChanged(x => xStatusFilter);
                FilterData();
                
            }
        }	

 

		private string _commentsFilter;
        public string CommentsFilter
        {
            get
            {
                return _commentsFilter;
            }
            set
            {
                _commentsFilter = value;
				NotifyPropertyChanged(x => CommentsFilter);
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

			AsycudaSalesAllocations.Refresh();
			NotifyPropertyChanged(x => this.AsycudaSalesAllocations);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(StatusFilter) == false)
						res.Append(" && " + string.Format("Status.Contains(\"{0}\")",  StatusFilter));						
 

					if(QtyAllocatedFilter.HasValue)
						res.Append(" && " + string.Format("QtyAllocated == {0}",  QtyAllocatedFilter.ToString()));				 

 

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
				 

					if(EANumberFilter.HasValue)
						res.Append(" && " + string.Format("EANumber == {0}",  EANumberFilter.ToString()));				 

					if(SANumberFilter.HasValue)
						res.Append(" && " + string.Format("SANumber == {0}",  SANumberFilter.ToString()));				 

									if(string.IsNullOrEmpty(xStatusFilter) == false)
						res.Append(" && " + string.Format("xStatus.Contains(\"{0}\")",  xStatusFilter));						
 

									if(string.IsNullOrEmpty(CommentsFilter) == false)
						res.Append(" && " + string.Format("Comments.Contains(\"{0}\")",  CommentsFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<AsycudaSalesAllocation> lst = null;
            using (var ctx = new AsycudaSalesAllocationRepository())
            {
                lst = await ctx.GetAsycudaSalesAllocationsByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<AsycudaSalesAllocationExcelLine, List<AsycudaSalesAllocationExcelLine>>
            {
                dataToPrint = lst.Select(x => new AsycudaSalesAllocationExcelLine
                {
 
                    Status = x.Status ,
                    
 
                    QtyAllocated = x.QtyAllocated ,
                    
 
                    EntryTimeStamp = x.EntryTimeStamp ,
                    
 
                    EANumber = x.EANumber ,
                    
 
                    SANumber = x.SANumber ,
                    
 
                    xStatus = x.Status ,
                    
 
                    Comments = x.Comments 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class AsycudaSalesAllocationExcelLine
        {
		 
                    public string Status { get; set; } 
                    
 
                    public double QtyAllocated { get; set; } 
                    
 
                    public Nullable<System.DateTime> EntryTimeStamp { get; set; } 
                    
 
                    public int EANumber { get; set; } 
                    
 
                    public int SANumber { get; set; } 
                    
 
                    public string xStatus { get; set; } 
                    
 
                    public string Comments { get; set; } 
                    
        }

		
    }
}
		
