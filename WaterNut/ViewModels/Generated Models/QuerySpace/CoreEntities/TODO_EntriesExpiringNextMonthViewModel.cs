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
    
	public partial class TODO_EntriesExpiringNextMonthViewModel_AutoGen : ViewModelBase<TODO_EntriesExpiringNextMonthViewModel_AutoGen>
	{

       private static readonly TODO_EntriesExpiringNextMonthViewModel_AutoGen instance;
       static TODO_EntriesExpiringNextMonthViewModel_AutoGen()
        {
            instance = new TODO_EntriesExpiringNextMonthViewModel_AutoGen();
        }

       public static TODO_EntriesExpiringNextMonthViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public TODO_EntriesExpiringNextMonthViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<TODO_EntriesExpiringNextMonth>(MessageToken.CurrentTODO_EntriesExpiringNextMonthChanged, OnCurrentTODO_EntriesExpiringNextMonthChanged);
            RegisterToReceiveMessages(MessageToken.TODO_EntriesExpiringNextMonthChanged, OnTODO_EntriesExpiringNextMonthChanged);
			RegisterToReceiveMessages(MessageToken.TODO_EntriesExpiringNextMonthFilterExpressionChanged, OnTODO_EntriesExpiringNextMonthFilterExpressionChanged);


 			// Recieve messages for Core Current Entities Changed
 

			TODO_EntriesExpiringNextMonth = new VirtualList<TODO_EntriesExpiringNextMonth>(vloader);
			TODO_EntriesExpiringNextMonth.LoadingStateChanged += TODO_EntriesExpiringNextMonth_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(TODO_EntriesExpiringNextMonth, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<TODO_EntriesExpiringNextMonth> _TODO_EntriesExpiringNextMonth = null;
        public VirtualList<TODO_EntriesExpiringNextMonth> TODO_EntriesExpiringNextMonth
        {
            get
            {
                return _TODO_EntriesExpiringNextMonth;
            }
            set
            {
                _TODO_EntriesExpiringNextMonth = value;
            }
        }

		 private void OnTODO_EntriesExpiringNextMonthFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			TODO_EntriesExpiringNextMonth.Refresh();
            SelectedTODO_EntriesExpiringNextMonth.Clear();
            NotifyPropertyChanged(x => SelectedTODO_EntriesExpiringNextMonth);
            BeginSendMessage(MessageToken.SelectedTODO_EntriesExpiringNextMonthChanged, new NotificationEventArgs(MessageToken.SelectedTODO_EntriesExpiringNextMonthChanged));
        }

		void TODO_EntriesExpiringNextMonth_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (TODO_EntriesExpiringNextMonth.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => TODO_EntriesExpiringNextMonth);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("TODO_EntriesExpiringNextMonth | Error occured..." + TODO_EntriesExpiringNextMonth.LastLoadingError.Message);
                    NotifyPropertyChanged(x => TODO_EntriesExpiringNextMonth);
                    break;
            }
           
        }

		
		public readonly TODO_EntriesExpiringNextMonthVirturalListLoader vloader = new TODO_EntriesExpiringNextMonthVirturalListLoader();

		private ObservableCollection<TODO_EntriesExpiringNextMonth> _selectedTODO_EntriesExpiringNextMonth = new ObservableCollection<TODO_EntriesExpiringNextMonth>();
        public ObservableCollection<TODO_EntriesExpiringNextMonth> SelectedTODO_EntriesExpiringNextMonth
        {
            get
            {
                return _selectedTODO_EntriesExpiringNextMonth;
            }
            set
            {
                _selectedTODO_EntriesExpiringNextMonth = value;
				BeginSendMessage(MessageToken.SelectedTODO_EntriesExpiringNextMonthChanged,
                                    new NotificationEventArgs(MessageToken.SelectedTODO_EntriesExpiringNextMonthChanged));
				 NotifyPropertyChanged(x => SelectedTODO_EntriesExpiringNextMonth);
            }
        }

        internal virtual void OnCurrentTODO_EntriesExpiringNextMonthChanged(object sender, NotificationEventArgs<TODO_EntriesExpiringNextMonth> e)
        {
            if(BaseViewModel.Instance.CurrentTODO_EntriesExpiringNextMonth != null) BaseViewModel.Instance.CurrentTODO_EntriesExpiringNextMonth.PropertyChanged += CurrentTODO_EntriesExpiringNextMonth__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentTODO_EntriesExpiringNextMonth);
        }   

            void CurrentTODO_EntriesExpiringNextMonth__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                 } 
        internal virtual void OnTODO_EntriesExpiringNextMonthChanged(object sender, NotificationEventArgs e)
        {
            _TODO_EntriesExpiringNextMonth.Refresh();
			NotifyPropertyChanged(x => this.TODO_EntriesExpiringNextMonth);
        }   


 
  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
		public void ViewAll()
        {
			vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";




			vloader.ClearNavigationExpression();
			_TODO_EntriesExpiringNextMonth.Refresh();
			NotifyPropertyChanged(x => this.TODO_EntriesExpiringNextMonth);
		}

		public async Task SelectAll()
        {
            IEnumerable<TODO_EntriesExpiringNextMonth> lst = null;
            using (var ctx = new TODO_EntriesExpiringNextMonthRepository())
            {
                lst = await ctx.GetTODO_EntriesExpiringNextMonthByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedTODO_EntriesExpiringNextMonth = new ObservableCollection<TODO_EntriesExpiringNextMonth>(lst);
        }

 
		private DateTime? _startExpiryDateFilter = DateTime.Parse(string.Format("{0}/1/{1}", DateTime.Now.Month ,DateTime.Now.Year));
        public DateTime? StartExpiryDateFilter
        {
            get
            {
                return _startExpiryDateFilter;
            }
            set
            {
                _startExpiryDateFilter = value;
				NotifyPropertyChanged(x => StartExpiryDateFilter);
                FilterData();
                
            }
        }	

		private DateTime? _endExpiryDateFilter = DateTime.Parse(string.Format("{1}/{0}/{2}", DateTime.DaysInMonth( DateTime.Now.Year,DateTime.Now.Month), DateTime.Now.Month, DateTime.Now.Year));
        public DateTime? EndExpiryDateFilter
        {
            get
            {
                return _endExpiryDateFilter;
            }
            set
            {
                _endExpiryDateFilter = value;
				NotifyPropertyChanged(x => EndExpiryDateFilter);
                FilterData();
                
            }
        }
 

		private DateTime? _expiryDateFilter;
        public DateTime? ExpiryDateFilter
        {
            get
            {
                return _expiryDateFilter;
            }
            set
            {
                _expiryDateFilter = value;
				NotifyPropertyChanged(x => ExpiryDateFilter);
                FilterData();
                
            }
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

			TODO_EntriesExpiringNextMonth.Refresh();
			NotifyPropertyChanged(x => this.TODO_EntriesExpiringNextMonth);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

 

				if (Convert.ToDateTime(StartExpiryDateFilter).Date != DateTime.MinValue &&
		        Convert.ToDateTime(EndExpiryDateFilter).Date != DateTime.MinValue) res.Append(" && (");

					if (Convert.ToDateTime(StartExpiryDateFilter).Date != DateTime.MinValue)
						{
							if(StartExpiryDateFilter.HasValue)
								res.Append(
                                            (Convert.ToDateTime(EndExpiryDateFilter).Date != DateTime.MinValue?"":" && ") +
                                            string.Format("ExpiryDate >= \"{0}\"",  Convert.ToDateTime(StartExpiryDateFilter).Date.ToString("MM/dd/yyyy")));
						}

					if (Convert.ToDateTime(EndExpiryDateFilter).Date != DateTime.MinValue)
						{
							if(EndExpiryDateFilter.HasValue)
								res.Append(" && " + string.Format("ExpiryDate <= \"{0}\"",  Convert.ToDateTime(EndExpiryDateFilter).Date.AddHours(23).ToString("MM/dd/yyyy HH:mm:ss")));
						}

				if (Convert.ToDateTime(StartExpiryDateFilter).Date != DateTime.MinValue &&
		        Convert.ToDateTime(EndExpiryDateFilter).Date != DateTime.MinValue) res.Append(" )");

					if (Convert.ToDateTime(_expiryDateFilter).Date != DateTime.MinValue)
						{
							if(ExpiryDateFilter.HasValue)
								res.Append(" && " + string.Format("ExpiryDate == \"{0}\"",  Convert.ToDateTime(ExpiryDateFilter).Date.ToString("MM/dd/yyyy")));
						}
				 

									if(string.IsNullOrEmpty(TypeFilter) == false)
						res.Append(" && " + string.Format("Type.Contains(\"{0}\")",  TypeFilter));						
 

									if(string.IsNullOrEmpty(DocumentTypeFilter) == false)
						res.Append(" && " + string.Format("DocumentType.Contains(\"{0}\")",  DocumentTypeFilter));						
 

									if(string.IsNullOrEmpty(CNumberFilter) == false)
						res.Append(" && " + string.Format("CNumber.Contains(\"{0}\")",  CNumberFilter));						
 

 

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
			IEnumerable<TODO_EntriesExpiringNextMonth> lst = null;
            using (var ctx = new TODO_EntriesExpiringNextMonthRepository())
            {
                lst = await ctx.GetTODO_EntriesExpiringNextMonthByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<TODO_EntriesExpiringNextMonthExcelLine, List<TODO_EntriesExpiringNextMonthExcelLine>>
            {
                dataToPrint = lst.Select(x => new TODO_EntriesExpiringNextMonthExcelLine
                {
 
                    ExpiryDate = x.ExpiryDate ,
                    
 
                    Type = x.Type ,
                    
 
                    DocumentType = x.DocumentType ,
                    
 
                    CNumber = x.CNumber ,
                    
 
                    RegistrationDate = x.RegistrationDate ,
                    
 
                    Reference = x.Reference 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public class TODO_EntriesExpiringNextMonthExcelLine
        {
		 
                    public Nullable<System.DateTime> ExpiryDate { get; set; } 
                    
 
                    public string Type { get; set; } 
                    
 
                    public string DocumentType { get; set; } 
                    
 
                    public string CNumber { get; set; } 
                    
 
                    public Nullable<System.DateTime> RegistrationDate { get; set; } 
                    
 
                    public string Reference { get; set; } 
                    
        }

		
    }
}
		