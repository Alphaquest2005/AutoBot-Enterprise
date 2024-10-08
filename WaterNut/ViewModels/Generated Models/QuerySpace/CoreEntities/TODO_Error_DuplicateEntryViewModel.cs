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
    
	public partial class TODO_Error_DuplicateEntryViewModel_AutoGen : ViewModelBase<TODO_Error_DuplicateEntryViewModel_AutoGen>
	{

       private static readonly TODO_Error_DuplicateEntryViewModel_AutoGen instance;
       static TODO_Error_DuplicateEntryViewModel_AutoGen()
        {
            instance = new TODO_Error_DuplicateEntryViewModel_AutoGen();
        }

       public static TODO_Error_DuplicateEntryViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public TODO_Error_DuplicateEntryViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<TODO_Error_DuplicateEntry>(MessageToken.CurrentTODO_Error_DuplicateEntryChanged, OnCurrentTODO_Error_DuplicateEntryChanged);
            RegisterToReceiveMessages(MessageToken.TODO_Error_DuplicateEntryChanged, OnTODO_Error_DuplicateEntryChanged);
			RegisterToReceiveMessages(MessageToken.TODO_Error_DuplicateEntryFilterExpressionChanged, OnTODO_Error_DuplicateEntryFilterExpressionChanged);


 			// Recieve messages for Core Current Entities Changed
 

			TODO_Error_DuplicateEntry = new VirtualList<TODO_Error_DuplicateEntry>(vloader);
			TODO_Error_DuplicateEntry.LoadingStateChanged += TODO_Error_DuplicateEntry_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(TODO_Error_DuplicateEntry, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<TODO_Error_DuplicateEntry> _TODO_Error_DuplicateEntry = null;
        public VirtualList<TODO_Error_DuplicateEntry> TODO_Error_DuplicateEntry
        {
            get
            {
                return _TODO_Error_DuplicateEntry;
            }
            set
            {
                _TODO_Error_DuplicateEntry = value;
                NotifyPropertyChanged( x => x.TODO_Error_DuplicateEntry);
            }
        }

		 private void OnTODO_Error_DuplicateEntryFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => TODO_Error_DuplicateEntry.Refresh()).ConfigureAwait(false);
            SelectedTODO_Error_DuplicateEntry.Clear();
            NotifyPropertyChanged(x => SelectedTODO_Error_DuplicateEntry);
            BeginSendMessage(MessageToken.SelectedTODO_Error_DuplicateEntryChanged, new NotificationEventArgs(MessageToken.SelectedTODO_Error_DuplicateEntryChanged));
        }

		void TODO_Error_DuplicateEntry_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (TODO_Error_DuplicateEntry.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => TODO_Error_DuplicateEntry);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("TODO_Error_DuplicateEntry | Error occured..." + TODO_Error_DuplicateEntry.LastLoadingError.Message);
                    NotifyPropertyChanged(x => TODO_Error_DuplicateEntry);
                    break;
            }
           
        }

		
		public readonly TODO_Error_DuplicateEntryVirturalListLoader vloader = new TODO_Error_DuplicateEntryVirturalListLoader();

		private ObservableCollection<TODO_Error_DuplicateEntry> _selectedTODO_Error_DuplicateEntry = new ObservableCollection<TODO_Error_DuplicateEntry>();
        public ObservableCollection<TODO_Error_DuplicateEntry> SelectedTODO_Error_DuplicateEntry
        {
            get
            {
                return _selectedTODO_Error_DuplicateEntry;
            }
            set
            {
                _selectedTODO_Error_DuplicateEntry = value;
				BeginSendMessage(MessageToken.SelectedTODO_Error_DuplicateEntryChanged,
                                    new NotificationEventArgs(MessageToken.SelectedTODO_Error_DuplicateEntryChanged));
				 NotifyPropertyChanged(x => SelectedTODO_Error_DuplicateEntry);
            }
        }

        internal virtual void OnCurrentTODO_Error_DuplicateEntryChanged(object sender, NotificationEventArgs<TODO_Error_DuplicateEntry> e)
        {
            if(BaseViewModel.Instance.CurrentTODO_Error_DuplicateEntry != null) BaseViewModel.Instance.CurrentTODO_Error_DuplicateEntry.PropertyChanged += CurrentTODO_Error_DuplicateEntry__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentTODO_Error_DuplicateEntry);
        }   

            void CurrentTODO_Error_DuplicateEntry__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                 } 
        internal virtual void OnTODO_Error_DuplicateEntryChanged(object sender, NotificationEventArgs e)
        {
            _TODO_Error_DuplicateEntry.Refresh();
			NotifyPropertyChanged(x => this.TODO_Error_DuplicateEntry);
        }   


 
  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
		public void ViewAll()
        {
			vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";




			vloader.ClearNavigationExpression();
			_TODO_Error_DuplicateEntry.Refresh();
			NotifyPropertyChanged(x => this.TODO_Error_DuplicateEntry);
		}

		public async Task SelectAll()
        {
            IEnumerable<TODO_Error_DuplicateEntry> lst = null;
            using (var ctx = new TODO_Error_DuplicateEntryRepository())
            {
                lst = await ctx.GetTODO_Error_DuplicateEntryByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedTODO_Error_DuplicateEntry = new ObservableCollection<TODO_Error_DuplicateEntry>(lst);
        }

 

		private string _typeFilter;
        public string TypeFilter
        {
            get
            {
                return _typeFilter;
            }
            set
            {
                _typeFilter = value;
				NotifyPropertyChanged(x => TypeFilter);
                FilterData();
                
            }
        }	

 

		private Int32? _linesFilter;
        public Int32? LinesFilter
        {
            get
            {
                return _linesFilter;
            }
            set
            {
                _linesFilter = value;
				NotifyPropertyChanged(x => LinesFilter);
                FilterData();
                
            }
        }	

 

		private string _idFilter;
        public string idFilter
        {
            get
            {
                return _idFilter;
            }
            set
            {
                _idFilter = value;
				NotifyPropertyChanged(x => idFilter);
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

 

		private string _extended_customs_procedureFilter;
        public string Extended_customs_procedureFilter
        {
            get
            {
                return _extended_customs_procedureFilter;
            }
            set
            {
                _extended_customs_procedureFilter = value;
				NotifyPropertyChanged(x => Extended_customs_procedureFilter);
                FilterData();
                
            }
        }	

 
		private DateTime? _startRegistrationDateFilter = DateTime.Parse(string.Format("{0}/1/{1}", DateTime.Now.Month ,DateTime.Now.Year));
        public DateTime? StartRegistrationDateFilter
        {
            get
            {
                return _startRegistrationDateFilter;
            }
            set
            {
                _startRegistrationDateFilter = value;
				NotifyPropertyChanged(x => StartRegistrationDateFilter);
                FilterData();
                
            }
        }	

		private DateTime? _endRegistrationDateFilter = DateTime.Parse(string.Format("{1}/{0}/{2}", DateTime.DaysInMonth( DateTime.Now.Year,DateTime.Now.Month), DateTime.Now.Month, DateTime.Now.Year));
        public DateTime? EndRegistrationDateFilter
        {
            get
            {
                return _endRegistrationDateFilter;
            }
            set
            {
                _endRegistrationDateFilter = value;
				NotifyPropertyChanged(x => EndRegistrationDateFilter);
                FilterData();
                
            }
        }
 

		private DateTime? _registrationDateFilter;
        public DateTime? RegistrationDateFilter
        {
            get
            {
                return _registrationDateFilter;
            }
            set
            {
                _registrationDateFilter = value;
				NotifyPropertyChanged(x => RegistrationDateFilter);
                FilterData();
                
            }
        }	

 

		private string _referenceFilter;
        public string ReferenceFilter
        {
            get
            {
                return _referenceFilter;
            }
            set
            {
                _referenceFilter = value;
				NotifyPropertyChanged(x => ReferenceFilter);
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

			TODO_Error_DuplicateEntry.Refresh();
			NotifyPropertyChanged(x => this.TODO_Error_DuplicateEntry);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(TypeFilter) == false)
						res.Append(" && " + string.Format("Type.Contains(\"{0}\")",  TypeFilter));						
 

					if(LinesFilter.HasValue)
						res.Append(" && " + string.Format("Lines == {0}",  LinesFilter.ToString()));				 

									if(string.IsNullOrEmpty(idFilter) == false)
						res.Append(" && " + string.Format("id.Contains(\"{0}\")",  idFilter));						
 

									if(string.IsNullOrEmpty(DocumentTypeFilter) == false)
						res.Append(" && " + string.Format("DocumentType.Contains(\"{0}\")",  DocumentTypeFilter));						
 

									if(string.IsNullOrEmpty(CNumberFilter) == false)
						res.Append(" && " + string.Format("CNumber.Contains(\"{0}\")",  CNumberFilter));						
 

									if(string.IsNullOrEmpty(Extended_customs_procedureFilter) == false)
						res.Append(" && " + string.Format("Extended_customs_procedure.Contains(\"{0}\")",  Extended_customs_procedureFilter));						
 

 

				if (Convert.ToDateTime(StartRegistrationDateFilter).Date != DateTime.MinValue &&
		        Convert.ToDateTime(EndRegistrationDateFilter).Date != DateTime.MinValue) res.Append(" && (");

					if (Convert.ToDateTime(StartRegistrationDateFilter).Date != DateTime.MinValue)
						{
							if(StartRegistrationDateFilter.HasValue)
								res.Append(
                                            (Convert.ToDateTime(EndRegistrationDateFilter).Date != DateTime.MinValue?"":" && ") +
                                            string.Format("RegistrationDate >= \"{0}\"",  Convert.ToDateTime(StartRegistrationDateFilter).Date.ToString("MM/dd/yyyy")));
						}

					if (Convert.ToDateTime(EndRegistrationDateFilter).Date != DateTime.MinValue)
						{
							if(EndRegistrationDateFilter.HasValue)
								res.Append(" && " + string.Format("RegistrationDate <= \"{0}\"",  Convert.ToDateTime(EndRegistrationDateFilter).Date.AddHours(23).ToString("MM/dd/yyyy HH:mm:ss")));
						}

				if (Convert.ToDateTime(StartRegistrationDateFilter).Date != DateTime.MinValue &&
		        Convert.ToDateTime(EndRegistrationDateFilter).Date != DateTime.MinValue) res.Append(" )");

					if (Convert.ToDateTime(_registrationDateFilter).Date != DateTime.MinValue)
						{
							if(RegistrationDateFilter.HasValue)
								res.Append(" && " + string.Format("RegistrationDate == \"{0}\"",  Convert.ToDateTime(RegistrationDateFilter).Date.ToString("MM/dd/yyyy")));
						}
				 

									if(string.IsNullOrEmpty(ReferenceFilter) == false)
						res.Append(" && " + string.Format("Reference.Contains(\"{0}\")",  ReferenceFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<TODO_Error_DuplicateEntry> lst = null;
            using (var ctx = new TODO_Error_DuplicateEntryRepository())
            {
                lst = await ctx.GetTODO_Error_DuplicateEntryByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<TODO_Error_DuplicateEntryExcelLine, List<TODO_Error_DuplicateEntryExcelLine>>
            {
                dataToPrint = lst.Select(x => new TODO_Error_DuplicateEntryExcelLine
                {
 
                    Type = x.Type ,
                    
 
                    Lines = x.Lines ,
                    
 
                    id = x.id ,
                    
 
                    DocumentType = x.DocumentType ,
                    
 
                    CNumber = x.CNumber ,
                    
 
                    Extended_customs_procedure = x.Extended_customs_procedure ,
                    
 
                    RegistrationDate = x.RegistrationDate ,
                    
 
                    Reference = x.Reference 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class TODO_Error_DuplicateEntryExcelLine
        {
		 
                    public string Type { get; set; } 
                    
 
                    public Nullable<int> Lines { get; set; } 
                    
 
                    public string id { get; set; } 
                    
 
                    public string DocumentType { get; set; } 
                    
 
                    public string CNumber { get; set; } 
                    
 
                    public string Extended_customs_procedure { get; set; } 
                    
 
                    public Nullable<System.DateTime> RegistrationDate { get; set; } 
                    
 
                    public string Reference { get; set; } 
                    
        }

		
    }
}
		
