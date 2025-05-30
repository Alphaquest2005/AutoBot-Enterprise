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
    
	public partial class TODO_ERRReport_ByItemNumberViewModel_AutoGen : ViewModelBase<TODO_ERRReport_ByItemNumberViewModel_AutoGen>
	{

       private static readonly TODO_ERRReport_ByItemNumberViewModel_AutoGen instance;
       static TODO_ERRReport_ByItemNumberViewModel_AutoGen()
        {
            instance = new TODO_ERRReport_ByItemNumberViewModel_AutoGen();
        }

       public static TODO_ERRReport_ByItemNumberViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public TODO_ERRReport_ByItemNumberViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<TODO_ERRReport_ByItemNumber>(MessageToken.CurrentTODO_ERRReport_ByItemNumberChanged, OnCurrentTODO_ERRReport_ByItemNumberChanged);
            RegisterToReceiveMessages(MessageToken.TODO_ERRReport_ByItemNumberChanged, OnTODO_ERRReport_ByItemNumberChanged);
			RegisterToReceiveMessages(MessageToken.TODO_ERRReport_ByItemNumberFilterExpressionChanged, OnTODO_ERRReport_ByItemNumberFilterExpressionChanged);


 			// Recieve messages for Core Current Entities Changed
 

			TODO_ERRReport_ByItemNumber = new VirtualList<TODO_ERRReport_ByItemNumber>(vloader);
			TODO_ERRReport_ByItemNumber.LoadingStateChanged += TODO_ERRReport_ByItemNumber_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(TODO_ERRReport_ByItemNumber, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<TODO_ERRReport_ByItemNumber> _TODO_ERRReport_ByItemNumber = null;
        public VirtualList<TODO_ERRReport_ByItemNumber> TODO_ERRReport_ByItemNumber
        {
            get
            {
                return _TODO_ERRReport_ByItemNumber;
            }
            set
            {
                _TODO_ERRReport_ByItemNumber = value;
                NotifyPropertyChanged( x => x.TODO_ERRReport_ByItemNumber);
            }
        }

		 private void OnTODO_ERRReport_ByItemNumberFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => TODO_ERRReport_ByItemNumber.Refresh()).ConfigureAwait(false);
            SelectedTODO_ERRReport_ByItemNumber.Clear();
            NotifyPropertyChanged(x => SelectedTODO_ERRReport_ByItemNumber);
            BeginSendMessage(MessageToken.SelectedTODO_ERRReport_ByItemNumberChanged, new NotificationEventArgs(MessageToken.SelectedTODO_ERRReport_ByItemNumberChanged));
        }

		void TODO_ERRReport_ByItemNumber_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (TODO_ERRReport_ByItemNumber.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => TODO_ERRReport_ByItemNumber);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("TODO_ERRReport_ByItemNumber | Error occured..." + TODO_ERRReport_ByItemNumber.LastLoadingError.Message);
                    NotifyPropertyChanged(x => TODO_ERRReport_ByItemNumber);
                    break;
            }
           
        }

		
		public readonly TODO_ERRReport_ByItemNumberVirturalListLoader vloader = new TODO_ERRReport_ByItemNumberVirturalListLoader();

		private ObservableCollection<TODO_ERRReport_ByItemNumber> _selectedTODO_ERRReport_ByItemNumber = new ObservableCollection<TODO_ERRReport_ByItemNumber>();
        public ObservableCollection<TODO_ERRReport_ByItemNumber> SelectedTODO_ERRReport_ByItemNumber
        {
            get
            {
                return _selectedTODO_ERRReport_ByItemNumber;
            }
            set
            {
                _selectedTODO_ERRReport_ByItemNumber = value;
				BeginSendMessage(MessageToken.SelectedTODO_ERRReport_ByItemNumberChanged,
                                    new NotificationEventArgs(MessageToken.SelectedTODO_ERRReport_ByItemNumberChanged));
				 NotifyPropertyChanged(x => SelectedTODO_ERRReport_ByItemNumber);
            }
        }

        internal virtual void OnCurrentTODO_ERRReport_ByItemNumberChanged(object sender, NotificationEventArgs<TODO_ERRReport_ByItemNumber> e)
        {
            if(BaseViewModel.Instance.CurrentTODO_ERRReport_ByItemNumber != null) BaseViewModel.Instance.CurrentTODO_ERRReport_ByItemNumber.PropertyChanged += CurrentTODO_ERRReport_ByItemNumber__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentTODO_ERRReport_ByItemNumber);
        }   

            void CurrentTODO_ERRReport_ByItemNumber__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    //if (e.PropertyName == "AddApplicationSettings")
                   // {
                   //    if(ApplicationSettings.Contains(CurrentTODO_ERRReport_ByItemNumber.ApplicationSettings) == false) ApplicationSettings.Add(CurrentTODO_ERRReport_ByItemNumber.ApplicationSettings);
                    //}
                 } 
        internal virtual void OnTODO_ERRReport_ByItemNumberChanged(object sender, NotificationEventArgs e)
        {
            _TODO_ERRReport_ByItemNumber.Refresh();
			NotifyPropertyChanged(x => this.TODO_ERRReport_ByItemNumber);
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
					
                    TODO_ERRReport_ByItemNumber.Refresh();
					NotifyPropertyChanged(x => this.TODO_ERRReport_ByItemNumber);
				}
  
// Filtering Each Field except IDs
		public void ViewAll()
        {
			vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";




			vloader.ClearNavigationExpression();
			_TODO_ERRReport_ByItemNumber.Refresh();
			NotifyPropertyChanged(x => this.TODO_ERRReport_ByItemNumber);
		}

		public async Task SelectAll()
        {
            IEnumerable<TODO_ERRReport_ByItemNumber> lst = null;
            using (var ctx = new TODO_ERRReport_ByItemNumberRepository())
            {
                lst = await ctx.GetTODO_ERRReport_ByItemNumberByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedTODO_ERRReport_ByItemNumber = new ObservableCollection<TODO_ERRReport_ByItemNumber>(lst);
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

 

		private string _descriptionFilter;
        public string DescriptionFilter
        {
            get
            {
                return _descriptionFilter;
            }
            set
            {
                _descriptionFilter = value;
				NotifyPropertyChanged(x => DescriptionFilter);
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

 

		private string _infoFilter;
        public string InfoFilter
        {
            get
            {
                return _infoFilter;
            }
            set
            {
                _infoFilter = value;
				NotifyPropertyChanged(x => InfoFilter);
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

			TODO_ERRReport_ByItemNumber.Refresh();
			NotifyPropertyChanged(x => this.TODO_ERRReport_ByItemNumber);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(CNumberFilter) == false)
						res.Append(" && " + string.Format("CNumber.Contains(\"{0}\")",  CNumberFilter));						
 

									if(string.IsNullOrEmpty(ReferenceFilter) == false)
						res.Append(" && " + string.Format("Reference.Contains(\"{0}\")",  ReferenceFilter));						
 

 

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
				 

									if(string.IsNullOrEmpty(DocumentTypeFilter) == false)
						res.Append(" && " + string.Format("DocumentType.Contains(\"{0}\")",  DocumentTypeFilter));						
 

					if(LineNumberFilter.HasValue)
						res.Append(" && " + string.Format("LineNumber == {0}",  LineNumberFilter.ToString()));				 

									if(string.IsNullOrEmpty(ItemNumberFilter) == false)
						res.Append(" && " + string.Format("ItemNumber.Contains(\"{0}\")",  ItemNumberFilter));						
 

									if(string.IsNullOrEmpty(DescriptionFilter) == false)
						res.Append(" && " + string.Format("Description.Contains(\"{0}\")",  DescriptionFilter));						
 

									if(string.IsNullOrEmpty(ErrorFilter) == false)
						res.Append(" && " + string.Format("Error.Contains(\"{0}\")",  ErrorFilter));						
 

									if(string.IsNullOrEmpty(InfoFilter) == false)
						res.Append(" && " + string.Format("Info.Contains(\"{0}\")",  InfoFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<TODO_ERRReport_ByItemNumber> lst = null;
            using (var ctx = new TODO_ERRReport_ByItemNumberRepository())
            {
                lst = await ctx.GetTODO_ERRReport_ByItemNumberByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<TODO_ERRReport_ByItemNumberExcelLine, List<TODO_ERRReport_ByItemNumberExcelLine>>
            {
                dataToPrint = lst.Select(x => new TODO_ERRReport_ByItemNumberExcelLine
                {
 
                    CNumber = x.CNumber ,
                    
 
                    Reference = x.Reference ,
                    
 
                    RegistrationDate = x.RegistrationDate ,
                    
 
                    DocumentType = x.DocumentType ,
                    
 
                    LineNumber = x.LineNumber ,
                    
 
                    ItemNumber = x.ItemNumber ,
                    
 
                    Description = x.Description ,
                    
 
                    Error = x.Error ,
                    
 
                    Info = x.Info 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class TODO_ERRReport_ByItemNumberExcelLine
        {
		 
                    public string CNumber { get; set; } 
                    
 
                    public string Reference { get; set; } 
                    
 
                    public Nullable<System.DateTime> RegistrationDate { get; set; } 
                    
 
                    public string DocumentType { get; set; } 
                    
 
                    public Nullable<int> LineNumber { get; set; } 
                    
 
                    public string ItemNumber { get; set; } 
                    
 
                    public string Description { get; set; } 
                    
 
                    public string Error { get; set; } 
                    
 
                    public string Info { get; set; } 
                    
        }

		
    }
}
		
