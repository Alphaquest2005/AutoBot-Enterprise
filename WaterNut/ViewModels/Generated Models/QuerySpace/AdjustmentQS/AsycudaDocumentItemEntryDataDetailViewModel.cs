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
    
	public partial class AsycudaDocumentItemEntryDataDetailViewModel_AutoGen : ViewModelBase<AsycudaDocumentItemEntryDataDetailViewModel_AutoGen>
	{

       private static readonly AsycudaDocumentItemEntryDataDetailViewModel_AutoGen instance;
       static AsycudaDocumentItemEntryDataDetailViewModel_AutoGen()
        {
            instance = new AsycudaDocumentItemEntryDataDetailViewModel_AutoGen();
        }

       public static AsycudaDocumentItemEntryDataDetailViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public AsycudaDocumentItemEntryDataDetailViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<AsycudaDocumentItemEntryDataDetail>(MessageToken.CurrentAsycudaDocumentItemEntryDataDetailChanged, OnCurrentAsycudaDocumentItemEntryDataDetailChanged);
            RegisterToReceiveMessages(MessageToken.AsycudaDocumentItemEntryDataDetailsChanged, OnAsycudaDocumentItemEntryDataDetailsChanged);
			RegisterToReceiveMessages(MessageToken.AsycudaDocumentItemEntryDataDetailsFilterExpressionChanged, OnAsycudaDocumentItemEntryDataDetailsFilterExpressionChanged);

 
			RegisterToReceiveMessages<AdjustmentOver>(MessageToken.CurrentAdjustmentOverChanged, OnCurrentAdjustmentOverChanged);
 
			RegisterToReceiveMessages<AdjustmentShort>(MessageToken.CurrentAdjustmentShortChanged, OnCurrentAdjustmentShortChanged);
 
			RegisterToReceiveMessages<AdjustmentDetail>(MessageToken.CurrentAdjustmentDetailChanged, OnCurrentAdjustmentDetailChanged);

 			// Recieve messages for Core Current Entities Changed
                        RegisterToReceiveMessages<ApplicationSettings>(CoreEntities.MessageToken.CurrentApplicationSettingsChanged, OnCurrentApplicationSettingsChanged);
                        RegisterToReceiveMessages<AsycudaDocumentSet>(CoreEntities.MessageToken.CurrentAsycudaDocumentSetChanged, OnCurrentAsycudaDocumentSetChanged);
 

			AsycudaDocumentItemEntryDataDetails = new VirtualList<AsycudaDocumentItemEntryDataDetail>(vloader);
			AsycudaDocumentItemEntryDataDetails.LoadingStateChanged += AsycudaDocumentItemEntryDataDetails_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(AsycudaDocumentItemEntryDataDetails, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<AsycudaDocumentItemEntryDataDetail> _AsycudaDocumentItemEntryDataDetails = null;
        public VirtualList<AsycudaDocumentItemEntryDataDetail> AsycudaDocumentItemEntryDataDetails
        {
            get
            {
                return _AsycudaDocumentItemEntryDataDetails;
            }
            set
            {
                _AsycudaDocumentItemEntryDataDetails = value;
                NotifyPropertyChanged( x => x.AsycudaDocumentItemEntryDataDetails);
            }
        }

		 private void OnAsycudaDocumentItemEntryDataDetailsFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => AsycudaDocumentItemEntryDataDetails.Refresh()).ConfigureAwait(false);
            SelectedAsycudaDocumentItemEntryDataDetails.Clear();
            NotifyPropertyChanged(x => SelectedAsycudaDocumentItemEntryDataDetails);
            BeginSendMessage(MessageToken.SelectedAsycudaDocumentItemEntryDataDetailsChanged, new NotificationEventArgs(MessageToken.SelectedAsycudaDocumentItemEntryDataDetailsChanged));
        }

		void AsycudaDocumentItemEntryDataDetails_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (AsycudaDocumentItemEntryDataDetails.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => AsycudaDocumentItemEntryDataDetails);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("AsycudaDocumentItemEntryDataDetails | Error occured..." + AsycudaDocumentItemEntryDataDetails.LastLoadingError.Message);
                    NotifyPropertyChanged(x => AsycudaDocumentItemEntryDataDetails);
                    break;
            }
           
        }

		
		public readonly AsycudaDocumentItemEntryDataDetailVirturalListLoader vloader = new AsycudaDocumentItemEntryDataDetailVirturalListLoader();

		private ObservableCollection<AsycudaDocumentItemEntryDataDetail> _selectedAsycudaDocumentItemEntryDataDetails = new ObservableCollection<AsycudaDocumentItemEntryDataDetail>();
        public ObservableCollection<AsycudaDocumentItemEntryDataDetail> SelectedAsycudaDocumentItemEntryDataDetails
        {
            get
            {
                return _selectedAsycudaDocumentItemEntryDataDetails;
            }
            set
            {
                _selectedAsycudaDocumentItemEntryDataDetails = value;
				BeginSendMessage(MessageToken.SelectedAsycudaDocumentItemEntryDataDetailsChanged,
                                    new NotificationEventArgs(MessageToken.SelectedAsycudaDocumentItemEntryDataDetailsChanged));
				 NotifyPropertyChanged(x => SelectedAsycudaDocumentItemEntryDataDetails);
            }
        }

        internal virtual void OnCurrentAsycudaDocumentItemEntryDataDetailChanged(object sender, NotificationEventArgs<AsycudaDocumentItemEntryDataDetail> e)
        {
            if(BaseViewModel.Instance.CurrentAsycudaDocumentItemEntryDataDetail != null) BaseViewModel.Instance.CurrentAsycudaDocumentItemEntryDataDetail.PropertyChanged += CurrentAsycudaDocumentItemEntryDataDetail__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentAsycudaDocumentItemEntryDataDetail);
        }   

            void CurrentAsycudaDocumentItemEntryDataDetail__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    //if (e.PropertyName == "AddAdjustmentOver")
                   // {
                   //    if(AdjustmentOvers.Contains(CurrentAsycudaDocumentItemEntryDataDetail.AdjustmentOver) == false) AdjustmentOvers.Add(CurrentAsycudaDocumentItemEntryDataDetail.AdjustmentOver);
                    //}
                    //if (e.PropertyName == "AddAdjustmentShort")
                   // {
                   //    if(AdjustmentShorts.Contains(CurrentAsycudaDocumentItemEntryDataDetail.AdjustmentShort) == false) AdjustmentShorts.Add(CurrentAsycudaDocumentItemEntryDataDetail.AdjustmentShort);
                    //}
                    //if (e.PropertyName == "AddAdjustmentDetail")
                   // {
                   //    if(AdjustmentDetails.Contains(CurrentAsycudaDocumentItemEntryDataDetail.AdjustmentDetail) == false) AdjustmentDetails.Add(CurrentAsycudaDocumentItemEntryDataDetail.AdjustmentDetail);
                    //}
                    //if (e.PropertyName == "AddApplicationSettings")
                   // {
                   //    if(ApplicationSettings.Contains(CurrentAsycudaDocumentItemEntryDataDetail.ApplicationSettings) == false) ApplicationSettings.Add(CurrentAsycudaDocumentItemEntryDataDetail.ApplicationSettings);
                    //}
                    //if (e.PropertyName == "AddAsycudaDocumentSet")
                   // {
                   //    if(AsycudaDocumentSet.Contains(CurrentAsycudaDocumentItemEntryDataDetail.AsycudaDocumentSet) == false) AsycudaDocumentSet.Add(CurrentAsycudaDocumentItemEntryDataDetail.AsycudaDocumentSet);
                    //}
                 } 
        internal virtual void OnAsycudaDocumentItemEntryDataDetailsChanged(object sender, NotificationEventArgs e)
        {
            _AsycudaDocumentItemEntryDataDetails.Refresh();
			NotifyPropertyChanged(x => this.AsycudaDocumentItemEntryDataDetails);
        }   


 	
		 internal virtual void OnCurrentAdjustmentOverChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<AdjustmentOver> e)
			{
			if(ViewCurrentAdjustmentOver == false) return;
			if (e.Data == null || e.Data.EntryDataDetailsId == null)
                {
                    vloader.FilterExpression = "None";
                }
                else
                {
				vloader.FilterExpression = string.Format("EntryDataDetailsId == {0}", e.Data.EntryDataDetailsId.ToString());
                 }

				AsycudaDocumentItemEntryDataDetails.Refresh();
				NotifyPropertyChanged(x => this.AsycudaDocumentItemEntryDataDetails);
                // SendMessage(MessageToken.AsycudaDocumentItemEntryDataDetailsChanged, new NotificationEventArgs(MessageToken.AsycudaDocumentItemEntryDataDetailsChanged));
                			}
	
		 internal virtual void OnCurrentAdjustmentShortChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<AdjustmentShort> e)
			{
			if(ViewCurrentAdjustmentShort == false) return;
			if (e.Data == null || e.Data.EntryDataDetailsId == null)
                {
                    vloader.FilterExpression = "None";
                }
                else
                {
				vloader.FilterExpression = string.Format("EntryDataDetailsId == {0}", e.Data.EntryDataDetailsId.ToString());
                 }

				AsycudaDocumentItemEntryDataDetails.Refresh();
				NotifyPropertyChanged(x => this.AsycudaDocumentItemEntryDataDetails);
                // SendMessage(MessageToken.AsycudaDocumentItemEntryDataDetailsChanged, new NotificationEventArgs(MessageToken.AsycudaDocumentItemEntryDataDetailsChanged));
                			}
	
		 internal virtual void OnCurrentAdjustmentDetailChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<AdjustmentDetail> e)
			{
			if(ViewCurrentAdjustmentDetail == false) return;
			if (e.Data == null || e.Data.EntryDataDetailsId == null)
                {
                    vloader.FilterExpression = "None";
                }
                else
                {
				vloader.FilterExpression = string.Format("EntryDataDetailsId == {0}", e.Data.EntryDataDetailsId.ToString());
                 }

				AsycudaDocumentItemEntryDataDetails.Refresh();
				NotifyPropertyChanged(x => this.AsycudaDocumentItemEntryDataDetails);
                // SendMessage(MessageToken.AsycudaDocumentItemEntryDataDetailsChanged, new NotificationEventArgs(MessageToken.AsycudaDocumentItemEntryDataDetailsChanged));
                			}

  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
                internal virtual void OnCurrentApplicationSettingsChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<ApplicationSettings> e)
				{
				if (e.Data == null || e.Data.ApplicationSettingsId == null)
                {
                    vloader.FilterExpression = null;
                }
                else
                {
                    vloader.FilterExpression = string.Format("ApplicationSettingsId == {0}", e.Data.ApplicationSettingsId.ToString());
                }
					
                    AsycudaDocumentItemEntryDataDetails.Refresh();
					NotifyPropertyChanged(x => this.AsycudaDocumentItemEntryDataDetails);
				}
                internal virtual void OnCurrentAsycudaDocumentSetChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<AsycudaDocumentSet> e)
				{
				if (e.Data == null || e.Data.AsycudaDocumentSetId == null)
                {
                    vloader.FilterExpression = null;
                }
                else
                {
                    vloader.FilterExpression = string.Format("AsycudaDocumentSetId == {0}", e.Data.AsycudaDocumentSetId.ToString());
                }
					
                    AsycudaDocumentItemEntryDataDetails.Refresh();
					NotifyPropertyChanged(x => this.AsycudaDocumentItemEntryDataDetails);
				}
  
// Filtering Each Field except IDs
 	
		 bool _viewCurrentAdjustmentOver = false;
         public bool ViewCurrentAdjustmentOver
         {
             get
             {
                 return _viewCurrentAdjustmentOver;
             }
             set
             {
                 _viewCurrentAdjustmentOver = value;
                 NotifyPropertyChanged(x => x.ViewCurrentAdjustmentOver);
                FilterData();
             }
         }
 	
		 bool _viewCurrentAdjustmentShort = false;
         public bool ViewCurrentAdjustmentShort
         {
             get
             {
                 return _viewCurrentAdjustmentShort;
             }
             set
             {
                 _viewCurrentAdjustmentShort = value;
                 NotifyPropertyChanged(x => x.ViewCurrentAdjustmentShort);
                FilterData();
             }
         }
 	
		 bool _viewCurrentAdjustmentDetail = false;
         public bool ViewCurrentAdjustmentDetail
         {
             get
             {
                 return _viewCurrentAdjustmentDetail;
             }
             set
             {
                 _viewCurrentAdjustmentDetail = value;
                 NotifyPropertyChanged(x => x.ViewCurrentAdjustmentDetail);
                FilterData();
             }
         }
		public void ViewAll()
        {
			vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";




			vloader.ClearNavigationExpression();
			_AsycudaDocumentItemEntryDataDetails.Refresh();
			NotifyPropertyChanged(x => this.AsycudaDocumentItemEntryDataDetails);
		}

		public async Task SelectAll()
        {
            IEnumerable<AsycudaDocumentItemEntryDataDetail> lst = null;
            using (var ctx = new AsycudaDocumentItemEntryDataDetailRepository())
            {
                lst = await ctx.GetAsycudaDocumentItemEntryDataDetailsByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedAsycudaDocumentItemEntryDataDetails = new ObservableCollection<AsycudaDocumentItemEntryDataDetail>(lst);
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

 

		private string _keyFilter;
        public string keyFilter
        {
            get
            {
                return _keyFilter;
            }
            set
            {
                _keyFilter = value;
				NotifyPropertyChanged(x => keyFilter);
                FilterData();
                
            }
        }	

 

		private string _documentTypeFilter;
        public string DocumentTypeFilter
        {
            get
            {
                return _documentTypeFilter;
            }
            set
            {
                _documentTypeFilter = value;
				NotifyPropertyChanged(x => DocumentTypeFilter);
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

 

		private Boolean? _importCompleteFilter;
        public Boolean? ImportCompleteFilter
        {
            get
            {
                return _importCompleteFilter;
            }
            set
            {
                _importCompleteFilter = value;
				NotifyPropertyChanged(x => ImportCompleteFilter);
                FilterData();
                
            }
        }	

 

		private string _customsProcedureFilter;
        public string CustomsProcedureFilter
        {
            get
            {
                return _customsProcedureFilter;
            }
            set
            {
                _customsProcedureFilter = value;
				NotifyPropertyChanged(x => CustomsProcedureFilter);
                FilterData();
                
            }
        }	

 

		private Int32? _asycuda_idFilter;
        public Int32? Asycuda_idFilter
        {
            get
            {
                return _asycuda_idFilter;
            }
            set
            {
                _asycuda_idFilter = value;
				NotifyPropertyChanged(x => Asycuda_idFilter);
                FilterData();
                
            }
        }	

 

		private string _entryDataTypeFilter;
        public string EntryDataTypeFilter
        {
            get
            {
                return _entryDataTypeFilter;
            }
            set
            {
                _entryDataTypeFilter = value;
				NotifyPropertyChanged(x => EntryDataTypeFilter);
                FilterData();
                
            }
        }	

 

		private string _cNumberFilter;
        public string CNumberFilter
        {
            get
            {
                return _cNumberFilter;
            }
            set
            {
                _cNumberFilter = value;
				NotifyPropertyChanged(x => CNumberFilter);
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

 

		private string _customsOperationFilter;
        public string CustomsOperationFilter
        {
            get
            {
                return _customsOperationFilter;
            }
            set
            {
                _customsOperationFilter = value;
				NotifyPropertyChanged(x => CustomsOperationFilter);
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

			AsycudaDocumentItemEntryDataDetails.Refresh();
			NotifyPropertyChanged(x => this.AsycudaDocumentItemEntryDataDetails);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(ItemNumberFilter) == false)
						res.Append(" && " + string.Format("ItemNumber.Contains(\"{0}\")",  ItemNumberFilter));						
 

									if(string.IsNullOrEmpty(keyFilter) == false)
						res.Append(" && " + string.Format("key.Contains(\"{0}\")",  keyFilter));						
 

									if(string.IsNullOrEmpty(DocumentTypeFilter) == false)
						res.Append(" && " + string.Format("DocumentType.Contains(\"{0}\")",  DocumentTypeFilter));						
 

					if(QuantityFilter.HasValue)
						res.Append(" && " + string.Format("Quantity == {0}",  QuantityFilter.ToString()));				 

									if(ImportCompleteFilter.HasValue)
						res.Append(" && " + string.Format("ImportComplete == {0}",  ImportCompleteFilter));						
 

									if(string.IsNullOrEmpty(CustomsProcedureFilter) == false)
						res.Append(" && " + string.Format("CustomsProcedure.Contains(\"{0}\")",  CustomsProcedureFilter));						
 

					if(Asycuda_idFilter.HasValue)
						res.Append(" && " + string.Format("Asycuda_id == {0}",  Asycuda_idFilter.ToString()));				 

									if(string.IsNullOrEmpty(EntryDataTypeFilter) == false)
						res.Append(" && " + string.Format("EntryDataType.Contains(\"{0}\")",  EntryDataTypeFilter));						
 

									if(string.IsNullOrEmpty(CNumberFilter) == false)
						res.Append(" && " + string.Format("CNumber.Contains(\"{0}\")",  CNumberFilter));						
 

					if(LineNumberFilter.HasValue)
						res.Append(" && " + string.Format("LineNumber == {0}",  LineNumberFilter.ToString()));				 

									if(string.IsNullOrEmpty(CustomsOperationFilter) == false)
						res.Append(" && " + string.Format("CustomsOperation.Contains(\"{0}\")",  CustomsOperationFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<AsycudaDocumentItemEntryDataDetail> lst = null;
            using (var ctx = new AsycudaDocumentItemEntryDataDetailRepository())
            {
                lst = await ctx.GetAsycudaDocumentItemEntryDataDetailsByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<AsycudaDocumentItemEntryDataDetailExcelLine, List<AsycudaDocumentItemEntryDataDetailExcelLine>>
            {
                dataToPrint = lst.Select(x => new AsycudaDocumentItemEntryDataDetailExcelLine
                {
 
                    ItemNumber = x.ItemNumber ,
                    
 
                    key = x.key ,
                    
 
                    DocumentType = x.DocumentType ,
                    
 
                    Quantity = x.Quantity ,
                    
 
                    ImportComplete = x.ImportComplete ,
                    
 
                    CustomsProcedure = x.CustomsProcedure ,
                    
 
                    Asycuda_id = x.Asycuda_id ,
                    
 
                    EntryDataType = x.EntryDataType ,
                    
 
                    CNumber = x.CNumber ,
                    
 
                    LineNumber = x.LineNumber ,
                    
 
                    CustomsOperation = x.CustomsOperation 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class AsycudaDocumentItemEntryDataDetailExcelLine
        {
		 
                    public string ItemNumber { get; set; } 
                    
 
                    public string key { get; set; } 
                    
 
                    public string DocumentType { get; set; } 
                    
 
                    public Nullable<double> Quantity { get; set; } 
                    
 
                    public bool ImportComplete { get; set; } 
                    
 
                    public string CustomsProcedure { get; set; } 
                    
 
                    public int Asycuda_id { get; set; } 
                    
 
                    public string EntryDataType { get; set; } 
                    
 
                    public string CNumber { get; set; } 
                    
 
                    public int LineNumber { get; set; } 
                    
 
                    public string CustomsOperation { get; set; } 
                    
        }

		
    }
}
		
