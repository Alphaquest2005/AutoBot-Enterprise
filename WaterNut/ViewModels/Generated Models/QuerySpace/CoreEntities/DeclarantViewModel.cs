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
    
	public partial class DeclarantViewModel_AutoGen : ViewModelBase<DeclarantViewModel_AutoGen>
	{

       private static readonly DeclarantViewModel_AutoGen instance;
       static DeclarantViewModel_AutoGen()
        {
            instance = new DeclarantViewModel_AutoGen();
        }

       public static DeclarantViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public DeclarantViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<Declarant>(MessageToken.CurrentDeclarantChanged, OnCurrentDeclarantChanged);
            RegisterToReceiveMessages(MessageToken.DeclarantsChanged, OnDeclarantsChanged);
			RegisterToReceiveMessages(MessageToken.DeclarantsFilterExpressionChanged, OnDeclarantsFilterExpressionChanged);

 
			RegisterToReceiveMessages<ApplicationSettings>(MessageToken.CurrentApplicationSettingsChanged, OnCurrentApplicationSettingsChanged);

 			// Recieve messages for Core Current Entities Changed
 

			Declarants = new VirtualList<Declarant>(vloader);
			Declarants.LoadingStateChanged += Declarants_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(Declarants, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<Declarant> _Declarants = null;
        public VirtualList<Declarant> Declarants
        {
            get
            {
                return _Declarants;
            }
            set
            {
                _Declarants = value;
                NotifyPropertyChanged( x => x.Declarants);
            }
        }

		 private void OnDeclarantsFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => Declarants.Refresh()).ConfigureAwait(false);
            SelectedDeclarants.Clear();
            NotifyPropertyChanged(x => SelectedDeclarants);
            BeginSendMessage(MessageToken.SelectedDeclarantsChanged, new NotificationEventArgs(MessageToken.SelectedDeclarantsChanged));
        }

		void Declarants_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (Declarants.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => Declarants);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("Declarants | Error occured..." + Declarants.LastLoadingError.Message);
                    NotifyPropertyChanged(x => Declarants);
                    break;
            }
           
        }

		
		public readonly DeclarantVirturalListLoader vloader = new DeclarantVirturalListLoader();

		private ObservableCollection<Declarant> _selectedDeclarants = new ObservableCollection<Declarant>();
        public ObservableCollection<Declarant> SelectedDeclarants
        {
            get
            {
                return _selectedDeclarants;
            }
            set
            {
                _selectedDeclarants = value;
				BeginSendMessage(MessageToken.SelectedDeclarantsChanged,
                                    new NotificationEventArgs(MessageToken.SelectedDeclarantsChanged));
				 NotifyPropertyChanged(x => SelectedDeclarants);
            }
        }

        internal virtual void OnCurrentDeclarantChanged(object sender, NotificationEventArgs<Declarant> e)
        {
            if(BaseViewModel.Instance.CurrentDeclarant != null) BaseViewModel.Instance.CurrentDeclarant.PropertyChanged += CurrentDeclarant__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentDeclarant);
        }   

            void CurrentDeclarant__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    //if (e.PropertyName == "AddApplicationSettings")
                   // {
                   //    if(ApplicationSettings.Contains(CurrentDeclarant.ApplicationSettings) == false) ApplicationSettings.Add(CurrentDeclarant.ApplicationSettings);
                    //}
                 } 
        internal virtual void OnDeclarantsChanged(object sender, NotificationEventArgs e)
        {
            _Declarants.Refresh();
			NotifyPropertyChanged(x => this.Declarants);
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

				Declarants.Refresh();
				NotifyPropertyChanged(x => this.Declarants);
                // SendMessage(MessageToken.DeclarantsChanged, new NotificationEventArgs(MessageToken.DeclarantsChanged));
                                          
                BaseViewModel.Instance.CurrentDeclarant = null;
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
			_Declarants.Refresh();
			NotifyPropertyChanged(x => this.Declarants);
		}

		public async Task SelectAll()
        {
            IEnumerable<Declarant> lst = null;
            using (var ctx = new DeclarantRepository())
            {
                lst = await ctx.GetDeclarantsByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedDeclarants = new ObservableCollection<Declarant>(lst);
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

 

		private Boolean? _isDefaultFilter;
        public Boolean? IsDefaultFilter
        {
            get
            {
                return _isDefaultFilter;
            }
            set
            {
                _isDefaultFilter = value;
				NotifyPropertyChanged(x => IsDefaultFilter);
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

			Declarants.Refresh();
			NotifyPropertyChanged(x => this.Declarants);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(DeclarantCodeFilter) == false)
						res.Append(" && " + string.Format("DeclarantCode.Contains(\"{0}\")",  DeclarantCodeFilter));						
 

									if(IsDefaultFilter.HasValue)
						res.Append(" && " + string.Format("IsDefault == {0}",  IsDefaultFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<Declarant> lst = null;
            using (var ctx = new DeclarantRepository())
            {
                lst = await ctx.GetDeclarantsByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<DeclarantExcelLine, List<DeclarantExcelLine>>
            {
                dataToPrint = lst.Select(x => new DeclarantExcelLine
                {
 
                    DeclarantCode = x.DeclarantCode ,
                    
 
                    IsDefault = x.IsDefault 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class DeclarantExcelLine
        {
		 
                    public string DeclarantCode { get; set; } 
                    
 
                    public bool IsDefault { get; set; } 
                    
        }

		
    }
}
		
