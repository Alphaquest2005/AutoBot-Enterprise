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
    
	public partial class FileTypeMappingsViewModel_AutoGen : ViewModelBase<FileTypeMappingsViewModel_AutoGen>
	{

       private static readonly FileTypeMappingsViewModel_AutoGen instance;
       static FileTypeMappingsViewModel_AutoGen()
        {
            instance = new FileTypeMappingsViewModel_AutoGen();
        }

       public static FileTypeMappingsViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public FileTypeMappingsViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<FileTypeMappings>(MessageToken.CurrentFileTypeMappingsChanged, OnCurrentFileTypeMappingsChanged);
            RegisterToReceiveMessages(MessageToken.FileTypeMappingsChanged, OnFileTypeMappingsChanged);
			RegisterToReceiveMessages(MessageToken.FileTypeMappingsFilterExpressionChanged, OnFileTypeMappingsFilterExpressionChanged);

 
			RegisterToReceiveMessages<FileTypes>(MessageToken.CurrentFileTypesChanged, OnCurrentFileTypesChanged);

 			// Recieve messages for Core Current Entities Changed
 

			FileTypeMappings = new VirtualList<FileTypeMappings>(vloader);
			FileTypeMappings.LoadingStateChanged += FileTypeMappings_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(FileTypeMappings, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<FileTypeMappings> _FileTypeMappings = null;
        public VirtualList<FileTypeMappings> FileTypeMappings
        {
            get
            {
                return _FileTypeMappings;
            }
            set
            {
                _FileTypeMappings = value;
                NotifyPropertyChanged( x => x.FileTypeMappings);
            }
        }

		 private void OnFileTypeMappingsFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => FileTypeMappings.Refresh()).ConfigureAwait(false);
            SelectedFileTypeMappings.Clear();
            NotifyPropertyChanged(x => SelectedFileTypeMappings);
            BeginSendMessage(MessageToken.SelectedFileTypeMappingsChanged, new NotificationEventArgs(MessageToken.SelectedFileTypeMappingsChanged));
        }

		void FileTypeMappings_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (FileTypeMappings.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => FileTypeMappings);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("FileTypeMappings | Error occured..." + FileTypeMappings.LastLoadingError.Message);
                    NotifyPropertyChanged(x => FileTypeMappings);
                    break;
            }
           
        }

		
		public readonly FileTypeMappingsVirturalListLoader vloader = new FileTypeMappingsVirturalListLoader();

		private ObservableCollection<FileTypeMappings> _selectedFileTypeMappings = new ObservableCollection<FileTypeMappings>();
        public ObservableCollection<FileTypeMappings> SelectedFileTypeMappings
        {
            get
            {
                return _selectedFileTypeMappings;
            }
            set
            {
                _selectedFileTypeMappings = value;
				BeginSendMessage(MessageToken.SelectedFileTypeMappingsChanged,
                                    new NotificationEventArgs(MessageToken.SelectedFileTypeMappingsChanged));
				 NotifyPropertyChanged(x => SelectedFileTypeMappings);
            }
        }

        internal virtual void OnCurrentFileTypeMappingsChanged(object sender, NotificationEventArgs<FileTypeMappings> e)
        {
            if(BaseViewModel.Instance.CurrentFileTypeMappings != null) BaseViewModel.Instance.CurrentFileTypeMappings.PropertyChanged += CurrentFileTypeMappings__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentFileTypeMappings);
        }   

            void CurrentFileTypeMappings__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    //if (e.PropertyName == "AddFileTypes")
                   // {
                   //    if(FileTypes.Contains(CurrentFileTypeMappings.FileTypes) == false) FileTypes.Add(CurrentFileTypeMappings.FileTypes);
                    //}
                 } 
        internal virtual void OnFileTypeMappingsChanged(object sender, NotificationEventArgs e)
        {
            _FileTypeMappings.Refresh();
			NotifyPropertyChanged(x => this.FileTypeMappings);
        }   


 	
		 internal virtual void OnCurrentFileTypesChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<FileTypes> e)
			{
			if(ViewCurrentFileTypes == false) return;
			if (e.Data == null || e.Data.Id == null)
                {
                    vloader.FilterExpression = "None";
                }
                else
                {
				vloader.FilterExpression = string.Format("FileTypeId == {0}", e.Data.Id.ToString());
                 }

				FileTypeMappings.Refresh();
				NotifyPropertyChanged(x => this.FileTypeMappings);
                // SendMessage(MessageToken.FileTypeMappingsChanged, new NotificationEventArgs(MessageToken.FileTypeMappingsChanged));
                                          
                BaseViewModel.Instance.CurrentFileTypeMappings = null;
			}

  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
 	
		 bool _viewCurrentFileTypes = false;
         public bool ViewCurrentFileTypes
         {
             get
             {
                 return _viewCurrentFileTypes;
             }
             set
             {
                 _viewCurrentFileTypes = value;
                 NotifyPropertyChanged(x => x.ViewCurrentFileTypes);
                FilterData();
             }
         }
		public void ViewAll()
        {
		vloader.FilterExpression = "All";




			vloader.ClearNavigationExpression();
			_FileTypeMappings.Refresh();
			NotifyPropertyChanged(x => this.FileTypeMappings);
		}

		public async Task SelectAll()
        {
            IEnumerable<FileTypeMappings> lst = null;
            using (var ctx = new FileTypeMappingsRepository())
            {
                lst = await ctx.GetFileTypeMappingsByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedFileTypeMappings = new ObservableCollection<FileTypeMappings>(lst);
        }

 

		private string _originalNameFilter;
        public string OriginalNameFilter
        {
            get
            {
                return _originalNameFilter;
            }
            set
            {
                _originalNameFilter = value;
				NotifyPropertyChanged(x => OriginalNameFilter);
                FilterData();
                
            }
        }	

 

		private string _destinationNameFilter;
        public string DestinationNameFilter
        {
            get
            {
                return _destinationNameFilter;
            }
            set
            {
                _destinationNameFilter = value;
				NotifyPropertyChanged(x => DestinationNameFilter);
                FilterData();
                
            }
        }	

 

		private string _dataTypeFilter;
        public string DataTypeFilter
        {
            get
            {
                return _dataTypeFilter;
            }
            set
            {
                _dataTypeFilter = value;
				NotifyPropertyChanged(x => DataTypeFilter);
                FilterData();
                
            }
        }	

 

		private Boolean? _requiredFilter;
        public Boolean? RequiredFilter
        {
            get
            {
                return _requiredFilter;
            }
            set
            {
                _requiredFilter = value;
				NotifyPropertyChanged(x => RequiredFilter);
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

 

		private Boolean? _replaceOnlyNullsFilter;
        public Boolean? ReplaceOnlyNullsFilter
        {
            get
            {
                return _replaceOnlyNullsFilter;
            }
            set
            {
                _replaceOnlyNullsFilter = value;
				NotifyPropertyChanged(x => ReplaceOnlyNullsFilter);
                FilterData();
                
            }
        }	

 

		private Boolean? _replicateColumnValuesFilter;
        public Boolean? ReplicateColumnValuesFilter
        {
            get
            {
                return _replicateColumnValuesFilter;
            }
            set
            {
                _replicateColumnValuesFilter = value;
				NotifyPropertyChanged(x => ReplicateColumnValuesFilter);
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

			FileTypeMappings.Refresh();
			NotifyPropertyChanged(x => this.FileTypeMappings);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(OriginalNameFilter) == false)
						res.Append(" && " + string.Format("OriginalName.Contains(\"{0}\")",  OriginalNameFilter));						
 

									if(string.IsNullOrEmpty(DestinationNameFilter) == false)
						res.Append(" && " + string.Format("DestinationName.Contains(\"{0}\")",  DestinationNameFilter));						
 

									if(string.IsNullOrEmpty(DataTypeFilter) == false)
						res.Append(" && " + string.Format("DataType.Contains(\"{0}\")",  DataTypeFilter));						
 

									if(RequiredFilter.HasValue)
						res.Append(" && " + string.Format("Required == {0}",  RequiredFilter));						
 

									if(string.IsNullOrEmpty(CommentsFilter) == false)
						res.Append(" && " + string.Format("Comments.Contains(\"{0}\")",  CommentsFilter));						
 

									if(ReplaceOnlyNullsFilter.HasValue)
						res.Append(" && " + string.Format("ReplaceOnlyNulls == {0}",  ReplaceOnlyNullsFilter));						
 

									if(ReplicateColumnValuesFilter.HasValue)
						res.Append(" && " + string.Format("ReplicateColumnValues == {0}",  ReplicateColumnValuesFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<FileTypeMappings> lst = null;
            using (var ctx = new FileTypeMappingsRepository())
            {
                lst = await ctx.GetFileTypeMappingsByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<FileTypeMappingsExcelLine, List<FileTypeMappingsExcelLine>>
            {
                dataToPrint = lst.Select(x => new FileTypeMappingsExcelLine
                {
 
                    OriginalName = x.OriginalName ,
                    
 
                    DestinationName = x.DestinationName ,
                    
 
                    DataType = x.DataType ,
                    
 
                    Required = x.Required ,
                    
 
                    Comments = x.Comments ,
                    
 
                    ReplaceOnlyNulls = x.ReplaceOnlyNulls ,
                    
 
                    ReplicateColumnValues = x.ReplicateColumnValues 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class FileTypeMappingsExcelLine
        {
		 
                    public string OriginalName { get; set; } 
                    
 
                    public string DestinationName { get; set; } 
                    
 
                    public string DataType { get; set; } 
                    
 
                    public bool Required { get; set; } 
                    
 
                    public string Comments { get; set; } 
                    
 
                    public bool ReplaceOnlyNulls { get; set; } 
                    
 
                    public bool ReplicateColumnValues { get; set; } 
                    
        }

		
    }
}
		
