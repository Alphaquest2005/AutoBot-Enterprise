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
    
	public partial class ExpiredEntriesLstViewModel_AutoGen : ViewModelBase<ExpiredEntriesLstViewModel_AutoGen>
	{

       private static readonly ExpiredEntriesLstViewModel_AutoGen instance;
       static ExpiredEntriesLstViewModel_AutoGen()
        {
            instance = new ExpiredEntriesLstViewModel_AutoGen();
        }

       public static ExpiredEntriesLstViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public ExpiredEntriesLstViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<ExpiredEntriesLst>(MessageToken.CurrentExpiredEntriesLstChanged, OnCurrentExpiredEntriesLstChanged);
            RegisterToReceiveMessages(MessageToken.ExpiredEntriesLstChanged, OnExpiredEntriesLstChanged);
			RegisterToReceiveMessages(MessageToken.ExpiredEntriesLstFilterExpressionChanged, OnExpiredEntriesLstFilterExpressionChanged);


 			// Recieve messages for Core Current Entities Changed
 

			ExpiredEntriesLst = new VirtualList<ExpiredEntriesLst>(vloader);
			ExpiredEntriesLst.LoadingStateChanged += ExpiredEntriesLst_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(ExpiredEntriesLst, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<ExpiredEntriesLst> _ExpiredEntriesLst = null;
        public VirtualList<ExpiredEntriesLst> ExpiredEntriesLst
        {
            get
            {
                return _ExpiredEntriesLst;
            }
            set
            {
                _ExpiredEntriesLst = value;
                NotifyPropertyChanged( x => x.ExpiredEntriesLst);
            }
        }

		 private void OnExpiredEntriesLstFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => ExpiredEntriesLst.Refresh()).ConfigureAwait(false);
            SelectedExpiredEntriesLst.Clear();
            NotifyPropertyChanged(x => SelectedExpiredEntriesLst);
            BeginSendMessage(MessageToken.SelectedExpiredEntriesLstChanged, new NotificationEventArgs(MessageToken.SelectedExpiredEntriesLstChanged));
        }

		void ExpiredEntriesLst_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (ExpiredEntriesLst.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => ExpiredEntriesLst);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("ExpiredEntriesLst | Error occured..." + ExpiredEntriesLst.LastLoadingError.Message);
                    NotifyPropertyChanged(x => ExpiredEntriesLst);
                    break;
            }
           
        }

		
		public readonly ExpiredEntriesLstVirturalListLoader vloader = new ExpiredEntriesLstVirturalListLoader();

		private ObservableCollection<ExpiredEntriesLst> _selectedExpiredEntriesLst = new ObservableCollection<ExpiredEntriesLst>();
        public ObservableCollection<ExpiredEntriesLst> SelectedExpiredEntriesLst
        {
            get
            {
                return _selectedExpiredEntriesLst;
            }
            set
            {
                _selectedExpiredEntriesLst = value;
				BeginSendMessage(MessageToken.SelectedExpiredEntriesLstChanged,
                                    new NotificationEventArgs(MessageToken.SelectedExpiredEntriesLstChanged));
				 NotifyPropertyChanged(x => SelectedExpiredEntriesLst);
            }
        }

        internal virtual void OnCurrentExpiredEntriesLstChanged(object sender, NotificationEventArgs<ExpiredEntriesLst> e)
        {
            if(BaseViewModel.Instance.CurrentExpiredEntriesLst != null) BaseViewModel.Instance.CurrentExpiredEntriesLst.PropertyChanged += CurrentExpiredEntriesLst__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentExpiredEntriesLst);
        }   

            void CurrentExpiredEntriesLst__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    //if (e.PropertyName == "AddApplicationSettings")
                   // {
                   //    if(ApplicationSettings.Contains(CurrentExpiredEntriesLst.ApplicationSettings) == false) ApplicationSettings.Add(CurrentExpiredEntriesLst.ApplicationSettings);
                    //}
                 } 
        internal virtual void OnExpiredEntriesLstChanged(object sender, NotificationEventArgs e)
        {
            _ExpiredEntriesLst.Refresh();
			NotifyPropertyChanged(x => this.ExpiredEntriesLst);
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
					
                    ExpiredEntriesLst.Refresh();
					NotifyPropertyChanged(x => this.ExpiredEntriesLst);
				}
  
// Filtering Each Field except IDs
		public void ViewAll()
        {
			vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";




			vloader.ClearNavigationExpression();
			_ExpiredEntriesLst.Refresh();
			NotifyPropertyChanged(x => this.ExpiredEntriesLst);
		}

		public async Task SelectAll()
        {
            IEnumerable<ExpiredEntriesLst> lst = null;
            using (var ctx = new ExpiredEntriesLstRepository())
            {
                lst = await ctx.GetExpiredEntriesLstByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedExpiredEntriesLst = new ObservableCollection<ExpiredEntriesLst>(lst);
        }

 

		private string _officeFilter;
        public string OfficeFilter
        {
            get
            {
                return _officeFilter;
            }
            set
            {
                _officeFilter = value;
				NotifyPropertyChanged(x => OfficeFilter);
                FilterData();
                
            }
        }	

 

		private string _registrationSerialFilter;
        public string RegistrationSerialFilter
        {
            get
            {
                return _registrationSerialFilter;
            }
            set
            {
                _registrationSerialFilter = value;
				NotifyPropertyChanged(x => RegistrationSerialFilter);
                FilterData();
                
            }
        }	

 

		private string _registrationNumberFilter;
        public string RegistrationNumberFilter
        {
            get
            {
                return _registrationNumberFilter;
            }
            set
            {
                _registrationNumberFilter = value;
				NotifyPropertyChanged(x => RegistrationNumberFilter);
                FilterData();
                
            }
        }	

 

		private string _registrationDateFilter;
        public string RegistrationDateFilter
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

 

		private string _assessmentSerialFilter;
        public string AssessmentSerialFilter
        {
            get
            {
                return _assessmentSerialFilter;
            }
            set
            {
                _assessmentSerialFilter = value;
				NotifyPropertyChanged(x => AssessmentSerialFilter);
                FilterData();
                
            }
        }	

 

		private string _assessmentNumberFilter;
        public string AssessmentNumberFilter
        {
            get
            {
                return _assessmentNumberFilter;
            }
            set
            {
                _assessmentNumberFilter = value;
				NotifyPropertyChanged(x => AssessmentNumberFilter);
                FilterData();
                
            }
        }	

 

		private string _assessmentDateFilter;
        public string AssessmentDateFilter
        {
            get
            {
                return _assessmentDateFilter;
            }
            set
            {
                _assessmentDateFilter = value;
				NotifyPropertyChanged(x => AssessmentDateFilter);
                FilterData();
                
            }
        }	

 

		private string _declarantCodeFilter;
        public string DeclarantCodeFilter
        {
            get
            {
                return _declarantCodeFilter;
            }
            set
            {
                _declarantCodeFilter = value;
				NotifyPropertyChanged(x => DeclarantCodeFilter);
                FilterData();
                
            }
        }	

 

		private string _declarantReferenceFilter;
        public string DeclarantReferenceFilter
        {
            get
            {
                return _declarantReferenceFilter;
            }
            set
            {
                _declarantReferenceFilter = value;
				NotifyPropertyChanged(x => DeclarantReferenceFilter);
                FilterData();
                
            }
        }	

 

		private string _exporterFilter;
        public string ExporterFilter
        {
            get
            {
                return _exporterFilter;
            }
            set
            {
                _exporterFilter = value;
				NotifyPropertyChanged(x => ExporterFilter);
                FilterData();
                
            }
        }	

 

		private string _consigneeFilter;
        public string ConsigneeFilter
        {
            get
            {
                return _consigneeFilter;
            }
            set
            {
                _consigneeFilter = value;
				NotifyPropertyChanged(x => ConsigneeFilter);
                FilterData();
                
            }
        }	

 

		private string _expirationFilter;
        public string ExpirationFilter
        {
            get
            {
                return _expirationFilter;
            }
            set
            {
                _expirationFilter = value;
				NotifyPropertyChanged(x => ExpirationFilter);
                FilterData();
                
            }
        }	

 

		private string _generalProcedureFilter;
        public string GeneralProcedureFilter
        {
            get
            {
                return _generalProcedureFilter;
            }
            set
            {
                _generalProcedureFilter = value;
				NotifyPropertyChanged(x => GeneralProcedureFilter);
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

			ExpiredEntriesLst.Refresh();
			NotifyPropertyChanged(x => this.ExpiredEntriesLst);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(OfficeFilter) == false)
						res.Append(" && " + string.Format("Office.Contains(\"{0}\")",  OfficeFilter));						
 

									if(string.IsNullOrEmpty(RegistrationSerialFilter) == false)
						res.Append(" && " + string.Format("RegistrationSerial.Contains(\"{0}\")",  RegistrationSerialFilter));						
 

									if(string.IsNullOrEmpty(RegistrationNumberFilter) == false)
						res.Append(" && " + string.Format("RegistrationNumber.Contains(\"{0}\")",  RegistrationNumberFilter));						
 

									if(string.IsNullOrEmpty(RegistrationDateFilter) == false)
						res.Append(" && " + string.Format("RegistrationDate.Contains(\"{0}\")",  RegistrationDateFilter));						
 

									if(string.IsNullOrEmpty(AssessmentSerialFilter) == false)
						res.Append(" && " + string.Format("AssessmentSerial.Contains(\"{0}\")",  AssessmentSerialFilter));						
 

									if(string.IsNullOrEmpty(AssessmentNumberFilter) == false)
						res.Append(" && " + string.Format("AssessmentNumber.Contains(\"{0}\")",  AssessmentNumberFilter));						
 

									if(string.IsNullOrEmpty(AssessmentDateFilter) == false)
						res.Append(" && " + string.Format("AssessmentDate.Contains(\"{0}\")",  AssessmentDateFilter));						
 

									if(string.IsNullOrEmpty(DeclarantCodeFilter) == false)
						res.Append(" && " + string.Format("DeclarantCode.Contains(\"{0}\")",  DeclarantCodeFilter));						
 

									if(string.IsNullOrEmpty(DeclarantReferenceFilter) == false)
						res.Append(" && " + string.Format("DeclarantReference.Contains(\"{0}\")",  DeclarantReferenceFilter));						
 

									if(string.IsNullOrEmpty(ExporterFilter) == false)
						res.Append(" && " + string.Format("Exporter.Contains(\"{0}\")",  ExporterFilter));						
 

									if(string.IsNullOrEmpty(ConsigneeFilter) == false)
						res.Append(" && " + string.Format("Consignee.Contains(\"{0}\")",  ConsigneeFilter));						
 

									if(string.IsNullOrEmpty(ExpirationFilter) == false)
						res.Append(" && " + string.Format("Expiration.Contains(\"{0}\")",  ExpirationFilter));						
 

									if(string.IsNullOrEmpty(GeneralProcedureFilter) == false)
						res.Append(" && " + string.Format("GeneralProcedure.Contains(\"{0}\")",  GeneralProcedureFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<ExpiredEntriesLst> lst = null;
            using (var ctx = new ExpiredEntriesLstRepository())
            {
                lst = await ctx.GetExpiredEntriesLstByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<ExpiredEntriesLstExcelLine, List<ExpiredEntriesLstExcelLine>>
            {
                dataToPrint = lst.Select(x => new ExpiredEntriesLstExcelLine
                {
 
                    Office = x.Office ,
                    
 
                    RegistrationSerial = x.RegistrationSerial ,
                    
 
                    RegistrationNumber = x.RegistrationNumber ,
                    
 
                    RegistrationDate = x.RegistrationDate ,
                    
 
                    AssessmentSerial = x.AssessmentSerial ,
                    
 
                    AssessmentNumber = x.AssessmentNumber ,
                    
 
                    AssessmentDate = x.AssessmentDate ,
                    
 
                    DeclarantCode = x.DeclarantCode ,
                    
 
                    DeclarantReference = x.DeclarantReference ,
                    
 
                    Exporter = x.Exporter ,
                    
 
                    Consignee = x.Consignee ,
                    
 
                    Expiration = x.Expiration ,
                    
 
                    GeneralProcedure = x.GeneralProcedure 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class ExpiredEntriesLstExcelLine
        {
		 
                    public string Office { get; set; } 
                    
 
                    public string RegistrationSerial { get; set; } 
                    
 
                    public string RegistrationNumber { get; set; } 
                    
 
                    public string RegistrationDate { get; set; } 
                    
 
                    public string AssessmentSerial { get; set; } 
                    
 
                    public string AssessmentNumber { get; set; } 
                    
 
                    public string AssessmentDate { get; set; } 
                    
 
                    public string DeclarantCode { get; set; } 
                    
 
                    public string DeclarantReference { get; set; } 
                    
 
                    public string Exporter { get; set; } 
                    
 
                    public string Consignee { get; set; } 
                    
 
                    public string Expiration { get; set; } 
                    
 
                    public string GeneralProcedure { get; set; } 
                    
        }

		
    }
}
		
