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

using OCR.Client.Entities;
using OCR.Client.Repositories;
//using WaterNut.Client.Repositories;
        
using CoreEntities.Client.Entities;


namespace WaterNut.QuerySpace.OCR.ViewModels
{
    
	public partial class ImportErrorsViewModel_AutoGen : ViewModelBase<ImportErrorsViewModel_AutoGen>
	{

       private static readonly ImportErrorsViewModel_AutoGen instance;
       static ImportErrorsViewModel_AutoGen()
        {
            instance = new ImportErrorsViewModel_AutoGen();
        }

       public static ImportErrorsViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public ImportErrorsViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<ImportErrors>(MessageToken.CurrentImportErrorsChanged, OnCurrentImportErrorsChanged);
            RegisterToReceiveMessages(MessageToken.ImportErrorsChanged, OnImportErrorsChanged);
			RegisterToReceiveMessages(MessageToken.ImportErrorsFilterExpressionChanged, OnImportErrorsFilterExpressionChanged);


 			// Recieve messages for Core Current Entities Changed
 

			ImportErrors = new VirtualList<ImportErrors>(vloader);
			ImportErrors.LoadingStateChanged += ImportErrors_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(ImportErrors, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<ImportErrors> _ImportErrors = null;
        public VirtualList<ImportErrors> ImportErrors
        {
            get
            {
                return _ImportErrors;
            }
            set
            {
                _ImportErrors = value;
                NotifyPropertyChanged( x => x.ImportErrors);
            }
        }

		 private void OnImportErrorsFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => ImportErrors.Refresh()).ConfigureAwait(false);
            SelectedImportErrors.Clear();
            NotifyPropertyChanged(x => SelectedImportErrors);
            BeginSendMessage(MessageToken.SelectedImportErrorsChanged, new NotificationEventArgs(MessageToken.SelectedImportErrorsChanged));
        }

		void ImportErrors_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (ImportErrors.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => ImportErrors);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("ImportErrors | Error occured..." + ImportErrors.LastLoadingError.Message);
                    NotifyPropertyChanged(x => ImportErrors);
                    break;
            }
           
        }

		
		public readonly ImportErrorsVirturalListLoader vloader = new ImportErrorsVirturalListLoader();

		private ObservableCollection<ImportErrors> _selectedImportErrors = new ObservableCollection<ImportErrors>();
        public ObservableCollection<ImportErrors> SelectedImportErrors
        {
            get
            {
                return _selectedImportErrors;
            }
            set
            {
                _selectedImportErrors = value;
				BeginSendMessage(MessageToken.SelectedImportErrorsChanged,
                                    new NotificationEventArgs(MessageToken.SelectedImportErrorsChanged));
				 NotifyPropertyChanged(x => SelectedImportErrors);
            }
        }

        internal virtual void OnCurrentImportErrorsChanged(object sender, NotificationEventArgs<ImportErrors> e)
        {
            if(BaseViewModel.Instance.CurrentImportErrors != null) BaseViewModel.Instance.CurrentImportErrors.PropertyChanged += CurrentImportErrors__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentImportErrors);
        }   

            void CurrentImportErrors__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                 } 
        internal virtual void OnImportErrorsChanged(object sender, NotificationEventArgs e)
        {
            _ImportErrors.Refresh();
			NotifyPropertyChanged(x => this.ImportErrors);
        }   


 
  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
		public void ViewAll()
        {
		vloader.FilterExpression = "All";




			vloader.ClearNavigationExpression();
			_ImportErrors.Refresh();
			NotifyPropertyChanged(x => this.ImportErrors);
		}

		public async Task SelectAll()
        {
            IEnumerable<ImportErrors> lst = null;
            using (var ctx = new ImportErrorsRepository())
            {
                lst = await ctx.GetImportErrorsByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedImportErrors = new ObservableCollection<ImportErrors>(lst);
        }

 

		private string _pdfTextFilter;
        public string PdfTextFilter
        {
            get
            {
                return _pdfTextFilter;
            }
            set
            {
                _pdfTextFilter = value;
				NotifyPropertyChanged(x => PdfTextFilter);
                FilterData();
                
            }
        }	

 

		private string _errorFilter;
        public string ErrorFilter
        {
            get
            {
                return _errorFilter;
            }
            set
            {
                _errorFilter = value;
				NotifyPropertyChanged(x => ErrorFilter);
                FilterData();
                
            }
        }	

 
		private DateTime? _startEntryDateTimeFilter = DateTime.Parse(string.Format("{0}/1/{1}", DateTime.Now.Month ,DateTime.Now.Year));
        public DateTime? StartEntryDateTimeFilter
        {
            get
            {
                return _startEntryDateTimeFilter;
            }
            set
            {
                _startEntryDateTimeFilter = value;
				NotifyPropertyChanged(x => StartEntryDateTimeFilter);
                FilterData();
                
            }
        }	

		private DateTime? _endEntryDateTimeFilter = DateTime.Parse(string.Format("{1}/{0}/{2}", DateTime.DaysInMonth( DateTime.Now.Year,DateTime.Now.Month), DateTime.Now.Month, DateTime.Now.Year));
        public DateTime? EndEntryDateTimeFilter
        {
            get
            {
                return _endEntryDateTimeFilter;
            }
            set
            {
                _endEntryDateTimeFilter = value;
				NotifyPropertyChanged(x => EndEntryDateTimeFilter);
                FilterData();
                
            }
        }
 

		private DateTime? _entryDateTimeFilter;
        public DateTime? EntryDateTimeFilter
        {
            get
            {
                return _entryDateTimeFilter;
            }
            set
            {
                _entryDateTimeFilter = value;
				NotifyPropertyChanged(x => EntryDateTimeFilter);
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

			ImportErrors.Refresh();
			NotifyPropertyChanged(x => this.ImportErrors);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(PdfTextFilter) == false)
						res.Append(" && " + string.Format("PdfText.Contains(\"{0}\")",  PdfTextFilter));						
 

									if(string.IsNullOrEmpty(ErrorFilter) == false)
						res.Append(" && " + string.Format("Error.Contains(\"{0}\")",  ErrorFilter));						
 

 

				if (Convert.ToDateTime(StartEntryDateTimeFilter).Date != DateTime.MinValue &&
		        Convert.ToDateTime(EndEntryDateTimeFilter).Date != DateTime.MinValue) res.Append(" && (");

					if (Convert.ToDateTime(StartEntryDateTimeFilter).Date != DateTime.MinValue)
						{
							if(StartEntryDateTimeFilter.HasValue)
								res.Append(
                                            (Convert.ToDateTime(EndEntryDateTimeFilter).Date != DateTime.MinValue?"":" && ") +
                                            string.Format("EntryDateTime >= \"{0}\"",  Convert.ToDateTime(StartEntryDateTimeFilter).Date.ToString("MM/dd/yyyy")));
						}

					if (Convert.ToDateTime(EndEntryDateTimeFilter).Date != DateTime.MinValue)
						{
							if(EndEntryDateTimeFilter.HasValue)
								res.Append(" && " + string.Format("EntryDateTime <= \"{0}\"",  Convert.ToDateTime(EndEntryDateTimeFilter).Date.AddHours(23).ToString("MM/dd/yyyy HH:mm:ss")));
						}

				if (Convert.ToDateTime(StartEntryDateTimeFilter).Date != DateTime.MinValue &&
		        Convert.ToDateTime(EndEntryDateTimeFilter).Date != DateTime.MinValue) res.Append(" )");

					if (Convert.ToDateTime(_entryDateTimeFilter).Date != DateTime.MinValue)
						{
							if(EntryDateTimeFilter.HasValue)
								res.Append(" && " + string.Format("EntryDateTime == \"{0}\"",  Convert.ToDateTime(EntryDateTimeFilter).Date.ToString("MM/dd/yyyy")));
						}
							return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<ImportErrors> lst = null;
            using (var ctx = new ImportErrorsRepository())
            {
                lst = await ctx.GetImportErrorsByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<ImportErrorsExcelLine, List<ImportErrorsExcelLine>>
            {
                dataToPrint = lst.Select(x => new ImportErrorsExcelLine
                {
 
                    PdfText = x.PdfText ,
                    
 
                    Error = x.Error ,
                    
 
                    EntryDateTime = x.EntryDateTime 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class ImportErrorsExcelLine
        {
		 
                    public string PdfText { get; set; } 
                    
 
                    public string Error { get; set; } 
                    
 
                    public System.DateTime EntryDateTime { get; set; } 
                    
        }

		
    }
}
		
