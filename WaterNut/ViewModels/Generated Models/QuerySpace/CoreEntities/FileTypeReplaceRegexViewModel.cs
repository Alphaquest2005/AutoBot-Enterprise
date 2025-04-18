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
    
	public partial class FileTypeReplaceRegexViewModel_AutoGen : ViewModelBase<FileTypeReplaceRegexViewModel_AutoGen>
	{

       private static readonly FileTypeReplaceRegexViewModel_AutoGen instance;
       static FileTypeReplaceRegexViewModel_AutoGen()
        {
            instance = new FileTypeReplaceRegexViewModel_AutoGen();
        }

       public static FileTypeReplaceRegexViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public FileTypeReplaceRegexViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<FileTypeReplaceRegex>(MessageToken.CurrentFileTypeReplaceRegexChanged, OnCurrentFileTypeReplaceRegexChanged);
            RegisterToReceiveMessages(MessageToken.FileTypeReplaceRegexChanged, OnFileTypeReplaceRegexChanged);
			RegisterToReceiveMessages(MessageToken.FileTypeReplaceRegexFilterExpressionChanged, OnFileTypeReplaceRegexFilterExpressionChanged);

 
			RegisterToReceiveMessages<FileTypes>(MessageToken.CurrentFileTypesChanged, OnCurrentFileTypesChanged);

 			// Recieve messages for Core Current Entities Changed
 

			FileTypeReplaceRegex = new VirtualList<FileTypeReplaceRegex>(vloader);
			FileTypeReplaceRegex.LoadingStateChanged += FileTypeReplaceRegex_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(FileTypeReplaceRegex, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<FileTypeReplaceRegex> _FileTypeReplaceRegex = null;
        public VirtualList<FileTypeReplaceRegex> FileTypeReplaceRegex
        {
            get
            {
                return _FileTypeReplaceRegex;
            }
            set
            {
                _FileTypeReplaceRegex = value;
                NotifyPropertyChanged( x => x.FileTypeReplaceRegex);
            }
        }

		 private void OnFileTypeReplaceRegexFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => FileTypeReplaceRegex.Refresh()).ConfigureAwait(false);
            SelectedFileTypeReplaceRegex.Clear();
            NotifyPropertyChanged(x => SelectedFileTypeReplaceRegex);
            BeginSendMessage(MessageToken.SelectedFileTypeReplaceRegexChanged, new NotificationEventArgs(MessageToken.SelectedFileTypeReplaceRegexChanged));
        }

		void FileTypeReplaceRegex_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (FileTypeReplaceRegex.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => FileTypeReplaceRegex);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("FileTypeReplaceRegex | Error occured..." + FileTypeReplaceRegex.LastLoadingError.Message);
                    NotifyPropertyChanged(x => FileTypeReplaceRegex);
                    break;
            }
           
        }

		
		public readonly FileTypeReplaceRegexVirturalListLoader vloader = new FileTypeReplaceRegexVirturalListLoader();

		private ObservableCollection<FileTypeReplaceRegex> _selectedFileTypeReplaceRegex = new ObservableCollection<FileTypeReplaceRegex>();
        public ObservableCollection<FileTypeReplaceRegex> SelectedFileTypeReplaceRegex
        {
            get
            {
                return _selectedFileTypeReplaceRegex;
            }
            set
            {
                _selectedFileTypeReplaceRegex = value;
				BeginSendMessage(MessageToken.SelectedFileTypeReplaceRegexChanged,
                                    new NotificationEventArgs(MessageToken.SelectedFileTypeReplaceRegexChanged));
				 NotifyPropertyChanged(x => SelectedFileTypeReplaceRegex);
            }
        }

        internal virtual void OnCurrentFileTypeReplaceRegexChanged(object sender, NotificationEventArgs<FileTypeReplaceRegex> e)
        {
            if(BaseViewModel.Instance.CurrentFileTypeReplaceRegex != null) BaseViewModel.Instance.CurrentFileTypeReplaceRegex.PropertyChanged += CurrentFileTypeReplaceRegex__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentFileTypeReplaceRegex);
        }   

            void CurrentFileTypeReplaceRegex__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    //if (e.PropertyName == "AddFileTypes")
                   // {
                   //    if(FileTypes.Contains(CurrentFileTypeReplaceRegex.FileTypes) == false) FileTypes.Add(CurrentFileTypeReplaceRegex.FileTypes);
                    //}
                 } 
        internal virtual void OnFileTypeReplaceRegexChanged(object sender, NotificationEventArgs e)
        {
            _FileTypeReplaceRegex.Refresh();
			NotifyPropertyChanged(x => this.FileTypeReplaceRegex);
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

				FileTypeReplaceRegex.Refresh();
				NotifyPropertyChanged(x => this.FileTypeReplaceRegex);
                // SendMessage(MessageToken.FileTypeReplaceRegexChanged, new NotificationEventArgs(MessageToken.FileTypeReplaceRegexChanged));
                                          
                BaseViewModel.Instance.CurrentFileTypeReplaceRegex = null;
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
			_FileTypeReplaceRegex.Refresh();
			NotifyPropertyChanged(x => this.FileTypeReplaceRegex);
		}

		public async Task SelectAll()
        {
            IEnumerable<FileTypeReplaceRegex> lst = null;
            using (var ctx = new FileTypeReplaceRegexRepository())
            {
                lst = await ctx.GetFileTypeReplaceRegexByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedFileTypeReplaceRegex = new ObservableCollection<FileTypeReplaceRegex>(lst);
        }

 

		private string _regexFilter;
        public string RegexFilter
        {
            get
            {
                return _regexFilter;
            }
            set
            {
                _regexFilter = value;
				NotifyPropertyChanged(x => RegexFilter);
                FilterData();
                
            }
        }	

 

		private string _replacementRegExFilter;
        public string ReplacementRegExFilter
        {
            get
            {
                return _replacementRegExFilter;
            }
            set
            {
                _replacementRegExFilter = value;
				NotifyPropertyChanged(x => ReplacementRegExFilter);
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

			FileTypeReplaceRegex.Refresh();
			NotifyPropertyChanged(x => this.FileTypeReplaceRegex);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(RegexFilter) == false)
						res.Append(" && " + string.Format("Regex.Contains(\"{0}\")",  RegexFilter));						
 

									if(string.IsNullOrEmpty(ReplacementRegExFilter) == false)
						res.Append(" && " + string.Format("ReplacementRegEx.Contains(\"{0}\")",  ReplacementRegExFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<FileTypeReplaceRegex> lst = null;
            using (var ctx = new FileTypeReplaceRegexRepository())
            {
                lst = await ctx.GetFileTypeReplaceRegexByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<FileTypeReplaceRegexExcelLine, List<FileTypeReplaceRegexExcelLine>>
            {
                dataToPrint = lst.Select(x => new FileTypeReplaceRegexExcelLine
                {
 
                    Regex = x.Regex ,
                    
 
                    ReplacementRegEx = x.ReplacementRegEx 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class FileTypeReplaceRegexExcelLine
        {
		 
                    public string Regex { get; set; } 
                    
 
                    public string ReplacementRegEx { get; set; } 
                    
        }

		
    }
}
		
