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

using EntryDataQS.Client.Entities;
using EntryDataQS.Client.Repositories;
//using WaterNut.Client.Repositories;
        
using CoreEntities.Client.Entities;


namespace WaterNut.QuerySpace.EntryDataQS.ViewModels
{
    
	public partial class AsycudaDocumentEntryDataLineViewModel_AutoGen : ViewModelBase<AsycudaDocumentEntryDataLineViewModel_AutoGen>
	{

       private static readonly AsycudaDocumentEntryDataLineViewModel_AutoGen instance;
       static AsycudaDocumentEntryDataLineViewModel_AutoGen()
        {
            instance = new AsycudaDocumentEntryDataLineViewModel_AutoGen();
        }

       public static AsycudaDocumentEntryDataLineViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public AsycudaDocumentEntryDataLineViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<AsycudaDocumentEntryDataLine>(MessageToken.CurrentAsycudaDocumentEntryDataLineChanged, OnCurrentAsycudaDocumentEntryDataLineChanged);
            RegisterToReceiveMessages(MessageToken.AsycudaDocumentEntryDataLinesChanged, OnAsycudaDocumentEntryDataLinesChanged);
			RegisterToReceiveMessages(MessageToken.AsycudaDocumentEntryDataLinesFilterExpressionChanged, OnAsycudaDocumentEntryDataLinesFilterExpressionChanged);


 			// Recieve messages for Core Current Entities Changed
 

			AsycudaDocumentEntryDataLines = new VirtualList<AsycudaDocumentEntryDataLine>(vloader);
			AsycudaDocumentEntryDataLines.LoadingStateChanged += AsycudaDocumentEntryDataLines_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(AsycudaDocumentEntryDataLines, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<AsycudaDocumentEntryDataLine> _AsycudaDocumentEntryDataLines = null;
        public VirtualList<AsycudaDocumentEntryDataLine> AsycudaDocumentEntryDataLines
        {
            get
            {
                return _AsycudaDocumentEntryDataLines;
            }
            set
            {
                _AsycudaDocumentEntryDataLines = value;
                NotifyPropertyChanged( x => x.AsycudaDocumentEntryDataLines);
            }
        }

		 private void OnAsycudaDocumentEntryDataLinesFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => AsycudaDocumentEntryDataLines.Refresh()).ConfigureAwait(false);
            SelectedAsycudaDocumentEntryDataLines.Clear();
            NotifyPropertyChanged(x => SelectedAsycudaDocumentEntryDataLines);
            BeginSendMessage(MessageToken.SelectedAsycudaDocumentEntryDataLinesChanged, new NotificationEventArgs(MessageToken.SelectedAsycudaDocumentEntryDataLinesChanged));
        }

		void AsycudaDocumentEntryDataLines_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (AsycudaDocumentEntryDataLines.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => AsycudaDocumentEntryDataLines);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("AsycudaDocumentEntryDataLines | Error occured..." + AsycudaDocumentEntryDataLines.LastLoadingError.Message);
                    NotifyPropertyChanged(x => AsycudaDocumentEntryDataLines);
                    break;
            }
           
        }

		
		public readonly AsycudaDocumentEntryDataLineVirturalListLoader vloader = new AsycudaDocumentEntryDataLineVirturalListLoader();

		private ObservableCollection<AsycudaDocumentEntryDataLine> _selectedAsycudaDocumentEntryDataLines = new ObservableCollection<AsycudaDocumentEntryDataLine>();
        public ObservableCollection<AsycudaDocumentEntryDataLine> SelectedAsycudaDocumentEntryDataLines
        {
            get
            {
                return _selectedAsycudaDocumentEntryDataLines;
            }
            set
            {
                _selectedAsycudaDocumentEntryDataLines = value;
				BeginSendMessage(MessageToken.SelectedAsycudaDocumentEntryDataLinesChanged,
                                    new NotificationEventArgs(MessageToken.SelectedAsycudaDocumentEntryDataLinesChanged));
				 NotifyPropertyChanged(x => SelectedAsycudaDocumentEntryDataLines);
            }
        }

        internal virtual void OnCurrentAsycudaDocumentEntryDataLineChanged(object sender, NotificationEventArgs<AsycudaDocumentEntryDataLine> e)
        {
            if(BaseViewModel.Instance.CurrentAsycudaDocumentEntryDataLine != null) BaseViewModel.Instance.CurrentAsycudaDocumentEntryDataLine.PropertyChanged += CurrentAsycudaDocumentEntryDataLine__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentAsycudaDocumentEntryDataLine);
        }   

            void CurrentAsycudaDocumentEntryDataLine__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                 } 
        internal virtual void OnAsycudaDocumentEntryDataLinesChanged(object sender, NotificationEventArgs e)
        {
            _AsycudaDocumentEntryDataLines.Refresh();
			NotifyPropertyChanged(x => this.AsycudaDocumentEntryDataLines);
        }   


 
  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
		public void ViewAll()
        {
			vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";




			vloader.ClearNavigationExpression();
			_AsycudaDocumentEntryDataLines.Refresh();
			NotifyPropertyChanged(x => this.AsycudaDocumentEntryDataLines);
		}

		public async Task SelectAll()
        {
            IEnumerable<AsycudaDocumentEntryDataLine> lst = null;
            using (var ctx = new AsycudaDocumentEntryDataLineRepository())
            {
                lst = await ctx.GetAsycudaDocumentEntryDataLinesByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedAsycudaDocumentEntryDataLines = new ObservableCollection<AsycudaDocumentEntryDataLine>(lst);
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

 
		private DateTime? _startEntryDataDateFilter = DateTime.Parse(string.Format("{0}/1/{1}", DateTime.Now.Month ,DateTime.Now.Year));
        public DateTime? StartEntryDataDateFilter
        {
            get
            {
                return _startEntryDataDateFilter;
            }
            set
            {
                _startEntryDataDateFilter = value;
				NotifyPropertyChanged(x => StartEntryDataDateFilter);
                FilterData();
                
            }
        }	

		private DateTime? _endEntryDataDateFilter = DateTime.Parse(string.Format("{1}/{0}/{2}", DateTime.DaysInMonth( DateTime.Now.Year,DateTime.Now.Month), DateTime.Now.Month, DateTime.Now.Year));
        public DateTime? EndEntryDataDateFilter
        {
            get
            {
                return _endEntryDataDateFilter;
            }
            set
            {
                _endEntryDataDateFilter = value;
				NotifyPropertyChanged(x => EndEntryDataDateFilter);
                FilterData();
                
            }
        }
 

		private DateTime? _entryDataDateFilter;
        public DateTime? EntryDataDateFilter
        {
            get
            {
                return _entryDataDateFilter;
            }
            set
            {
                _entryDataDateFilter = value;
				NotifyPropertyChanged(x => EntryDataDateFilter);
                FilterData();
                
            }
        }	

 

		private string _entryTypeFilter;
        public string EntryTypeFilter
        {
            get
            {
                return _entryTypeFilter;
            }
            set
            {
                _entryTypeFilter = value;
				NotifyPropertyChanged(x => EntryTypeFilter);
                FilterData();
                
            }
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

 

		private Double? _costFilter;
        public Double? CostFilter
        {
            get
            {
                return _costFilter;
            }
            set
            {
                _costFilter = value;
				NotifyPropertyChanged(x => CostFilter);
                FilterData();
                
            }
        }	

 

		private string _previousInvoiceNumberFilter;
        public string PreviousInvoiceNumberFilter
        {
            get
            {
                return _previousInvoiceNumberFilter;
            }
            set
            {
                _previousInvoiceNumberFilter = value;
				NotifyPropertyChanged(x => PreviousInvoiceNumberFilter);
                FilterData();
                
            }
        }	

 

		private string _entryDataDetailsKeyFilter;
        public string EntryDataDetailsKeyFilter
        {
            get
            {
                return _entryDataDetailsKeyFilter;
            }
            set
            {
                _entryDataDetailsKeyFilter = value;
				NotifyPropertyChanged(x => EntryDataDetailsKeyFilter);
                FilterData();
                
            }
        }	

 

		private Int32? _lineNumberFilter;
        public Int32? LineNumberFilter
        {
            get
            {
                return _lineNumberFilter;
            }
            set
            {
                _lineNumberFilter = value;
				NotifyPropertyChanged(x => LineNumberFilter);
                FilterData();
                
            }
        }	

 

		private string _commentFilter;
        public string CommentFilter
        {
            get
            {
                return _commentFilter;
            }
            set
            {
                _commentFilter = value;
				NotifyPropertyChanged(x => CommentFilter);
                FilterData();
                
            }
        }	

 

		private string _itemDescriptionFilter;
        public string ItemDescriptionFilter
        {
            get
            {
                return _itemDescriptionFilter;
            }
            set
            {
                _itemDescriptionFilter = value;
				NotifyPropertyChanged(x => ItemDescriptionFilter);
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

			AsycudaDocumentEntryDataLines.Refresh();
			NotifyPropertyChanged(x => this.AsycudaDocumentEntryDataLines);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(EntryDataIdFilter) == false)
						res.Append(" && " + string.Format("EntryDataId.Contains(\"{0}\")",  EntryDataIdFilter));						
 

 

				if (Convert.ToDateTime(StartEntryDataDateFilter).Date != DateTime.MinValue &&
		        Convert.ToDateTime(EndEntryDataDateFilter).Date != DateTime.MinValue) res.Append(" && (");

					if (Convert.ToDateTime(StartEntryDataDateFilter).Date != DateTime.MinValue)
						{
							if(StartEntryDataDateFilter.HasValue)
								res.Append(
                                            (Convert.ToDateTime(EndEntryDataDateFilter).Date != DateTime.MinValue?"":" && ") +
                                            string.Format("EntryDataDate >= \"{0}\"",  Convert.ToDateTime(StartEntryDataDateFilter).Date.ToString("MM/dd/yyyy")));
						}

					if (Convert.ToDateTime(EndEntryDataDateFilter).Date != DateTime.MinValue)
						{
							if(EndEntryDataDateFilter.HasValue)
								res.Append(" && " + string.Format("EntryDataDate <= \"{0}\"",  Convert.ToDateTime(EndEntryDataDateFilter).Date.AddHours(23).ToString("MM/dd/yyyy HH:mm:ss")));
						}

				if (Convert.ToDateTime(StartEntryDataDateFilter).Date != DateTime.MinValue &&
		        Convert.ToDateTime(EndEntryDataDateFilter).Date != DateTime.MinValue) res.Append(" )");

					if (Convert.ToDateTime(_entryDataDateFilter).Date != DateTime.MinValue)
						{
							if(EntryDataDateFilter.HasValue)
								res.Append(" && " + string.Format("EntryDataDate == \"{0}\"",  Convert.ToDateTime(EntryDataDateFilter).Date.ToString("MM/dd/yyyy")));
						}
				 

									if(string.IsNullOrEmpty(EntryTypeFilter) == false)
						res.Append(" && " + string.Format("EntryType.Contains(\"{0}\")",  EntryTypeFilter));						
 

									if(string.IsNullOrEmpty(ItemNumberFilter) == false)
						res.Append(" && " + string.Format("ItemNumber.Contains(\"{0}\")",  ItemNumberFilter));						
 

					if(QuantityFilter.HasValue)
						res.Append(" && " + string.Format("Quantity == {0}",  QuantityFilter.ToString()));				 

					if(CostFilter.HasValue)
						res.Append(" && " + string.Format("Cost == {0}",  CostFilter.ToString()));				 

									if(string.IsNullOrEmpty(PreviousInvoiceNumberFilter) == false)
						res.Append(" && " + string.Format("PreviousInvoiceNumber.Contains(\"{0}\")",  PreviousInvoiceNumberFilter));						
 

									if(string.IsNullOrEmpty(EntryDataDetailsKeyFilter) == false)
						res.Append(" && " + string.Format("EntryDataDetailsKey.Contains(\"{0}\")",  EntryDataDetailsKeyFilter));						
 

					if(LineNumberFilter.HasValue)
						res.Append(" && " + string.Format("LineNumber == {0}",  LineNumberFilter.ToString()));				 

									if(string.IsNullOrEmpty(CommentFilter) == false)
						res.Append(" && " + string.Format("Comment.Contains(\"{0}\")",  CommentFilter));						
 

									if(string.IsNullOrEmpty(ItemDescriptionFilter) == false)
						res.Append(" && " + string.Format("ItemDescription.Contains(\"{0}\")",  ItemDescriptionFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<AsycudaDocumentEntryDataLine> lst = null;
            using (var ctx = new AsycudaDocumentEntryDataLineRepository())
            {
                lst = await ctx.GetAsycudaDocumentEntryDataLinesByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<AsycudaDocumentEntryDataLineExcelLine, List<AsycudaDocumentEntryDataLineExcelLine>>
            {
                dataToPrint = lst.Select(x => new AsycudaDocumentEntryDataLineExcelLine
                {
 
                    EntryDataId = x.EntryDataId ,
                    
 
                    EntryDataDate = x.EntryDataDate ,
                    
 
                    EntryType = x.EntryType ,
                    
 
                    ItemNumber = x.ItemNumber ,
                    
 
                    Quantity = x.Quantity ,
                    
 
                    Cost = x.Cost ,
                    
 
                    PreviousInvoiceNumber = x.PreviousInvoiceNumber ,
                    
 
                    EntryDataDetailsKey = x.EntryDataDetailsKey ,
                    
 
                    LineNumber = x.LineNumber ,
                    
 
                    Comment = x.Comment ,
                    
 
                    ItemDescription = x.ItemDescription 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class AsycudaDocumentEntryDataLineExcelLine
        {
		 
                    public string EntryDataId { get; set; } 
                    
 
                    public System.DateTime EntryDataDate { get; set; } 
                    
 
                    public string EntryType { get; set; } 
                    
 
                    public string ItemNumber { get; set; } 
                    
 
                    public double Quantity { get; set; } 
                    
 
                    public double Cost { get; set; } 
                    
 
                    public string PreviousInvoiceNumber { get; set; } 
                    
 
                    public string EntryDataDetailsKey { get; set; } 
                    
 
                    public Nullable<int> LineNumber { get; set; } 
                    
 
                    public string Comment { get; set; } 
                    
 
                    public string ItemDescription { get; set; } 
                    
        }

		
    }
}
		