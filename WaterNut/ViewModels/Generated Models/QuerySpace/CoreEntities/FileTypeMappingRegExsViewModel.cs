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
    
	public partial class FileTypeMappingRegExsViewModel_AutoGen : ViewModelBase<FileTypeMappingRegExsViewModel_AutoGen>
	{

       private static readonly FileTypeMappingRegExsViewModel_AutoGen instance;
       static FileTypeMappingRegExsViewModel_AutoGen()
        {
            instance = new FileTypeMappingRegExsViewModel_AutoGen();
        }

       public static FileTypeMappingRegExsViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public FileTypeMappingRegExsViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<FileTypeMappingRegExs>(MessageToken.CurrentFileTypeMappingRegExsChanged, OnCurrentFileTypeMappingRegExsChanged);
            RegisterToReceiveMessages(MessageToken.FileTypeMappingRegExsChanged, OnFileTypeMappingRegExsChanged);
			RegisterToReceiveMessages(MessageToken.FileTypeMappingRegExsFilterExpressionChanged, OnFileTypeMappingRegExsFilterExpressionChanged);

 
			RegisterToReceiveMessages<FileTypeMappings>(MessageToken.CurrentFileTypeMappingsChanged, OnCurrentFileTypeMappingsChanged);

 			// Recieve messages for Core Current Entities Changed
 

			FileTypeMappingRegExs = new VirtualList<FileTypeMappingRegExs>(vloader);
			FileTypeMappingRegExs.LoadingStateChanged += FileTypeMappingRegExs_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(FileTypeMappingRegExs, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<FileTypeMappingRegExs> _FileTypeMappingRegExs = null;
        public VirtualList<FileTypeMappingRegExs> FileTypeMappingRegExs
        {
            get
            {
                return _FileTypeMappingRegExs;
            }
            set
            {
                _FileTypeMappingRegExs = value;
                NotifyPropertyChanged( x => x.FileTypeMappingRegExs);
            }
        }

		 private void OnFileTypeMappingRegExsFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => FileTypeMappingRegExs.Refresh()).ConfigureAwait(false);
            SelectedFileTypeMappingRegExs.Clear();
            NotifyPropertyChanged(x => SelectedFileTypeMappingRegExs);
            BeginSendMessage(MessageToken.SelectedFileTypeMappingRegExsChanged, new NotificationEventArgs(MessageToken.SelectedFileTypeMappingRegExsChanged));
        }

		void FileTypeMappingRegExs_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (FileTypeMappingRegExs.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => FileTypeMappingRegExs);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("FileTypeMappingRegExs | Error occured..." + FileTypeMappingRegExs.LastLoadingError.Message);
                    NotifyPropertyChanged(x => FileTypeMappingRegExs);
                    break;
            }
           
        }

		
		public readonly FileTypeMappingRegExsVirturalListLoader vloader = new FileTypeMappingRegExsVirturalListLoader();

		private ObservableCollection<FileTypeMappingRegExs> _selectedFileTypeMappingRegExs = new ObservableCollection<FileTypeMappingRegExs>();
        public ObservableCollection<FileTypeMappingRegExs> SelectedFileTypeMappingRegExs
        {
            get
            {
                return _selectedFileTypeMappingRegExs;
            }
            set
            {
                _selectedFileTypeMappingRegExs = value;
				BeginSendMessage(MessageToken.SelectedFileTypeMappingRegExsChanged,
                                    new NotificationEventArgs(MessageToken.SelectedFileTypeMappingRegExsChanged));
				 NotifyPropertyChanged(x => SelectedFileTypeMappingRegExs);
            }
        }

        internal virtual void OnCurrentFileTypeMappingRegExsChanged(object sender, NotificationEventArgs<FileTypeMappingRegExs> e)
        {
            if(BaseViewModel.Instance.CurrentFileTypeMappingRegExs != null) BaseViewModel.Instance.CurrentFileTypeMappingRegExs.PropertyChanged += CurrentFileTypeMappingRegExs__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentFileTypeMappingRegExs);
        }   

            void CurrentFileTypeMappingRegExs__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    //if (e.PropertyName == "AddFileTypeMappings")
                   // {
                   //    if(FileTypeMappings.Contains(CurrentFileTypeMappingRegExs.FileTypeMappings) == false) FileTypeMappings.Add(CurrentFileTypeMappingRegExs.FileTypeMappings);
                    //}
                 } 
        internal virtual void OnFileTypeMappingRegExsChanged(object sender, NotificationEventArgs e)
        {
            _FileTypeMappingRegExs.Refresh();
			NotifyPropertyChanged(x => this.FileTypeMappingRegExs);
        }   


 	
		 internal virtual void OnCurrentFileTypeMappingsChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<FileTypeMappings> e)
			{
			if(ViewCurrentFileTypeMappings == false) return;
			if (e.Data == null || e.Data.Id == null)
                {
                    vloader.FilterExpression = "None";
                }
                else
                {
				vloader.FilterExpression = string.Format("FileTypeMappingId == {0}", e.Data.Id.ToString());
                 }

				FileTypeMappingRegExs.Refresh();
				NotifyPropertyChanged(x => this.FileTypeMappingRegExs);
                // SendMessage(MessageToken.FileTypeMappingRegExsChanged, new NotificationEventArgs(MessageToken.FileTypeMappingRegExsChanged));
                                          
                BaseViewModel.Instance.CurrentFileTypeMappingRegExs = null;
			}

  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
 	
		 bool _viewCurrentFileTypeMappings = false;
         public bool ViewCurrentFileTypeMappings
         {
             get
             {
                 return _viewCurrentFileTypeMappings;
             }
             set
             {
                 _viewCurrentFileTypeMappings = value;
                 NotifyPropertyChanged(x => x.ViewCurrentFileTypeMappings);
                FilterData();
             }
         }
		public void ViewAll()
        {
		vloader.FilterExpression = "All";




			vloader.ClearNavigationExpression();
			_FileTypeMappingRegExs.Refresh();
			NotifyPropertyChanged(x => this.FileTypeMappingRegExs);
		}

		public async Task SelectAll()
        {
            IEnumerable<FileTypeMappingRegExs> lst = null;
            using (var ctx = new FileTypeMappingRegExsRepository())
            {
                lst = await ctx.GetFileTypeMappingRegExsByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedFileTypeMappingRegExs = new ObservableCollection<FileTypeMappingRegExs>(lst);
        }

 

		private string _replacementRegexFilter;
        public string ReplacementRegexFilter
        {
            get
            {
                return _replacementRegexFilter;
            }
            set
            {
                _replacementRegexFilter = value;
				NotifyPropertyChanged(x => ReplacementRegexFilter);
                FilterData();
                
            }
        }	

 

		private string _replacementValueFilter;
        public string ReplacementValueFilter
        {
            get
            {
                return _replacementValueFilter;
            }
            set
            {
                _replacementValueFilter = value;
				NotifyPropertyChanged(x => ReplacementValueFilter);
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

			FileTypeMappingRegExs.Refresh();
			NotifyPropertyChanged(x => this.FileTypeMappingRegExs);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(ReplacementRegexFilter) == false)
						res.Append(" && " + string.Format("ReplacementRegex.Contains(\"{0}\")",  ReplacementRegexFilter));						
 

									if(string.IsNullOrEmpty(ReplacementValueFilter) == false)
						res.Append(" && " + string.Format("ReplacementValue.Contains(\"{0}\")",  ReplacementValueFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<FileTypeMappingRegExs> lst = null;
            using (var ctx = new FileTypeMappingRegExsRepository())
            {
                lst = await ctx.GetFileTypeMappingRegExsByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<FileTypeMappingRegExsExcelLine, List<FileTypeMappingRegExsExcelLine>>
            {
                dataToPrint = lst.Select(x => new FileTypeMappingRegExsExcelLine
                {
 
                    ReplacementRegex = x.ReplacementRegex ,
                    
 
                    ReplacementValue = x.ReplacementValue 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class FileTypeMappingRegExsExcelLine
        {
		 
                    public string ReplacementRegex { get; set; } 
                    
 
                    public string ReplacementValue { get; set; } 
                    
        }

		
    }
}
		
