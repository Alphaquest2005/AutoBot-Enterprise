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
    
	public partial class EmailMappingViewModel_AutoGen : ViewModelBase<EmailMappingViewModel_AutoGen>
	{

       private static readonly EmailMappingViewModel_AutoGen instance;
       static EmailMappingViewModel_AutoGen()
        {
            instance = new EmailMappingViewModel_AutoGen();
        }

       public static EmailMappingViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public EmailMappingViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<EmailMapping>(MessageToken.CurrentEmailMappingChanged, OnCurrentEmailMappingChanged);
            RegisterToReceiveMessages(MessageToken.EmailMappingChanged, OnEmailMappingChanged);
			RegisterToReceiveMessages(MessageToken.EmailMappingFilterExpressionChanged, OnEmailMappingFilterExpressionChanged);

 
			RegisterToReceiveMessages<ApplicationSettings>(MessageToken.CurrentApplicationSettingsChanged, OnCurrentApplicationSettingsChanged);

 			// Recieve messages for Core Current Entities Changed
 

			EmailMapping = new VirtualList<EmailMapping>(vloader);
			EmailMapping.LoadingStateChanged += EmailMapping_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(EmailMapping, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<EmailMapping> _EmailMapping = null;
        public VirtualList<EmailMapping> EmailMapping
        {
            get
            {
                return _EmailMapping;
            }
            set
            {
                _EmailMapping = value;
                NotifyPropertyChanged( x => x.EmailMapping);
            }
        }

		 private void OnEmailMappingFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => EmailMapping.Refresh()).ConfigureAwait(false);
            SelectedEmailMapping.Clear();
            NotifyPropertyChanged(x => SelectedEmailMapping);
            BeginSendMessage(MessageToken.SelectedEmailMappingChanged, new NotificationEventArgs(MessageToken.SelectedEmailMappingChanged));
        }

		void EmailMapping_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (EmailMapping.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => EmailMapping);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("EmailMapping | Error occured..." + EmailMapping.LastLoadingError.Message);
                    NotifyPropertyChanged(x => EmailMapping);
                    break;
            }
           
        }

		
		public readonly EmailMappingVirturalListLoader vloader = new EmailMappingVirturalListLoader();

		private ObservableCollection<EmailMapping> _selectedEmailMapping = new ObservableCollection<EmailMapping>();
        public ObservableCollection<EmailMapping> SelectedEmailMapping
        {
            get
            {
                return _selectedEmailMapping;
            }
            set
            {
                _selectedEmailMapping = value;
				BeginSendMessage(MessageToken.SelectedEmailMappingChanged,
                                    new NotificationEventArgs(MessageToken.SelectedEmailMappingChanged));
				 NotifyPropertyChanged(x => SelectedEmailMapping);
            }
        }

        internal virtual void OnCurrentEmailMappingChanged(object sender, NotificationEventArgs<EmailMapping> e)
        {
            if(BaseViewModel.Instance.CurrentEmailMapping != null) BaseViewModel.Instance.CurrentEmailMapping.PropertyChanged += CurrentEmailMapping__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentEmailMapping);
        }   

            void CurrentEmailMapping__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    //if (e.PropertyName == "AddApplicationSettings")
                   // {
                   //    if(ApplicationSettings.Contains(CurrentEmailMapping.ApplicationSettings) == false) ApplicationSettings.Add(CurrentEmailMapping.ApplicationSettings);
                    //}
                 } 
        internal virtual void OnEmailMappingChanged(object sender, NotificationEventArgs e)
        {
            _EmailMapping.Refresh();
			NotifyPropertyChanged(x => this.EmailMapping);
        }   


 	
		 internal virtual void OnCurrentApplicationSettingsChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<ApplicationSettings> e)
			{
			if(ViewCurrentApplicationSettings == false) return;
			if (e.Data == null || e.Data.ApplicationSettingsId == null)
                {
                    vloader.FilterExpression = "None";
                }
                else
                {
				vloader.FilterExpression = string.Format("ApplicationSettingsId == {0}", e.Data.ApplicationSettingsId.ToString());
                 }

				EmailMapping.Refresh();
				NotifyPropertyChanged(x => this.EmailMapping);
                // SendMessage(MessageToken.EmailMappingChanged, new NotificationEventArgs(MessageToken.EmailMappingChanged));
                                          
                BaseViewModel.Instance.CurrentEmailMapping = null;
			}

  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
 	
		 bool _viewCurrentApplicationSettings = false;
         public bool ViewCurrentApplicationSettings
         {
             get
             {
                 return _viewCurrentApplicationSettings;
             }
             set
             {
                 _viewCurrentApplicationSettings = value;
                 NotifyPropertyChanged(x => x.ViewCurrentApplicationSettings);
                FilterData();
             }
         }
		public void ViewAll()
        {
			vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";




			vloader.ClearNavigationExpression();
			_EmailMapping.Refresh();
			NotifyPropertyChanged(x => this.EmailMapping);
		}

		public async Task SelectAll()
        {
            IEnumerable<EmailMapping> lst = null;
            using (var ctx = new EmailMappingRepository())
            {
                lst = await ctx.GetEmailMappingByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedEmailMapping = new ObservableCollection<EmailMapping>(lst);
        }

 

		private string _patternFilter;
        public string PatternFilter
        {
            get
            {
                return _patternFilter;
            }
            set
            {
                _patternFilter = value;
				NotifyPropertyChanged(x => PatternFilter);
                FilterData();
                
            }
        }	

 

		private Boolean? _isSingleEmailFilter;
        public Boolean? IsSingleEmailFilter
        {
            get
            {
                return _isSingleEmailFilter;
            }
            set
            {
                _isSingleEmailFilter = value;
				NotifyPropertyChanged(x => IsSingleEmailFilter);
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

 

		private Boolean? _infoFirstFilter;
        public Boolean? InfoFirstFilter
        {
            get
            {
                return _infoFirstFilter;
            }
            set
            {
                _infoFirstFilter = value;
				NotifyPropertyChanged(x => InfoFirstFilter);
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

			EmailMapping.Refresh();
			NotifyPropertyChanged(x => this.EmailMapping);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(PatternFilter) == false)
						res.Append(" && " + string.Format("Pattern.Contains(\"{0}\")",  PatternFilter));						
 

									if(IsSingleEmailFilter.HasValue)
						res.Append(" && " + string.Format("IsSingleEmail == {0}",  IsSingleEmailFilter));						
 

									if(string.IsNullOrEmpty(ReplacementValueFilter) == false)
						res.Append(" && " + string.Format("ReplacementValue.Contains(\"{0}\")",  ReplacementValueFilter));						
 

									if(InfoFirstFilter.HasValue)
						res.Append(" && " + string.Format("InfoFirst == {0}",  InfoFirstFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<EmailMapping> lst = null;
            using (var ctx = new EmailMappingRepository())
            {
                lst = await ctx.GetEmailMappingByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<EmailMappingExcelLine, List<EmailMappingExcelLine>>
            {
                dataToPrint = lst.Select(x => new EmailMappingExcelLine
                {
 
                    Pattern = x.Pattern ,
                    
 
                    IsSingleEmail = x.IsSingleEmail ,
                    
 
                    ReplacementValue = x.ReplacementValue ,
                    
 
                    InfoFirst = x.InfoFirst 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class EmailMappingExcelLine
        {
		 
                    public string Pattern { get; set; } 
                    
 
                    public Nullable<bool> IsSingleEmail { get; set; } 
                    
 
                    public string ReplacementValue { get; set; } 
                    
 
                    public Nullable<bool> InfoFirst { get; set; } 
                    
        }

		
    }
}
		
