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
    
	public partial class InfoMappingRegExViewModel_AutoGen : ViewModelBase<InfoMappingRegExViewModel_AutoGen>
	{

       private static readonly InfoMappingRegExViewModel_AutoGen instance;
       static InfoMappingRegExViewModel_AutoGen()
        {
            instance = new InfoMappingRegExViewModel_AutoGen();
        }

       public static InfoMappingRegExViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public InfoMappingRegExViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<InfoMappingRegEx>(MessageToken.CurrentInfoMappingRegExChanged, OnCurrentInfoMappingRegExChanged);
            RegisterToReceiveMessages(MessageToken.InfoMappingRegExChanged, OnInfoMappingRegExChanged);
			RegisterToReceiveMessages(MessageToken.InfoMappingRegExFilterExpressionChanged, OnInfoMappingRegExFilterExpressionChanged);

 
			RegisterToReceiveMessages<InfoMapping>(MessageToken.CurrentInfoMappingChanged, OnCurrentInfoMappingChanged);

 			// Recieve messages for Core Current Entities Changed
 

			InfoMappingRegEx = new VirtualList<InfoMappingRegEx>(vloader);
			InfoMappingRegEx.LoadingStateChanged += InfoMappingRegEx_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(InfoMappingRegEx, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<InfoMappingRegEx> _InfoMappingRegEx = null;
        public VirtualList<InfoMappingRegEx> InfoMappingRegEx
        {
            get
            {
                return _InfoMappingRegEx;
            }
            set
            {
                _InfoMappingRegEx = value;
            }
        }

		 private void OnInfoMappingRegExFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			InfoMappingRegEx.Refresh();
            SelectedInfoMappingRegEx.Clear();
            NotifyPropertyChanged(x => SelectedInfoMappingRegEx);
            BeginSendMessage(MessageToken.SelectedInfoMappingRegExChanged, new NotificationEventArgs(MessageToken.SelectedInfoMappingRegExChanged));
        }

		void InfoMappingRegEx_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (InfoMappingRegEx.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => InfoMappingRegEx);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("InfoMappingRegEx | Error occured..." + InfoMappingRegEx.LastLoadingError.Message);
                    NotifyPropertyChanged(x => InfoMappingRegEx);
                    break;
            }
           
        }

		
		public readonly InfoMappingRegExVirturalListLoader vloader = new InfoMappingRegExVirturalListLoader();

		private ObservableCollection<InfoMappingRegEx> _selectedInfoMappingRegEx = new ObservableCollection<InfoMappingRegEx>();
        public ObservableCollection<InfoMappingRegEx> SelectedInfoMappingRegEx
        {
            get
            {
                return _selectedInfoMappingRegEx;
            }
            set
            {
                _selectedInfoMappingRegEx = value;
				BeginSendMessage(MessageToken.SelectedInfoMappingRegExChanged,
                                    new NotificationEventArgs(MessageToken.SelectedInfoMappingRegExChanged));
				 NotifyPropertyChanged(x => SelectedInfoMappingRegEx);
            }
        }

        internal virtual void OnCurrentInfoMappingRegExChanged(object sender, NotificationEventArgs<InfoMappingRegEx> e)
        {
            if(BaseViewModel.Instance.CurrentInfoMappingRegEx != null) BaseViewModel.Instance.CurrentInfoMappingRegEx.PropertyChanged += CurrentInfoMappingRegEx__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentInfoMappingRegEx);
        }   

            void CurrentInfoMappingRegEx__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    //if (e.PropertyName == "AddInfoMapping")
                   // {
                   //    if(InfoMapping.Contains(CurrentInfoMappingRegEx.InfoMapping) == false) InfoMapping.Add(CurrentInfoMappingRegEx.InfoMapping);
                    //}
                 } 
        internal virtual void OnInfoMappingRegExChanged(object sender, NotificationEventArgs e)
        {
            _InfoMappingRegEx.Refresh();
			NotifyPropertyChanged(x => this.InfoMappingRegEx);
        }   


 	
		 internal virtual void OnCurrentInfoMappingChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<InfoMapping> e)
			{
			if(ViewCurrentInfoMapping == false) return;
			if (e.Data == null || e.Data.Id == null)
                {
                    vloader.FilterExpression = "None";
                }
                else
                {
				vloader.FilterExpression = string.Format("InfoMappingId == {0}", e.Data.Id.ToString());
                 }

				InfoMappingRegEx.Refresh();
				NotifyPropertyChanged(x => this.InfoMappingRegEx);
                // SendMessage(MessageToken.InfoMappingRegExChanged, new NotificationEventArgs(MessageToken.InfoMappingRegExChanged));
                                          
                BaseViewModel.Instance.CurrentInfoMappingRegEx = null;
			}

  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
 	
		 bool _viewCurrentInfoMapping = false;
         public bool ViewCurrentInfoMapping
         {
             get
             {
                 return _viewCurrentInfoMapping;
             }
             set
             {
                 _viewCurrentInfoMapping = value;
                 NotifyPropertyChanged(x => x.ViewCurrentInfoMapping);
                FilterData();
             }
         }
		public void ViewAll()
        {
		vloader.FilterExpression = "All";




			vloader.ClearNavigationExpression();
			_InfoMappingRegEx.Refresh();
			NotifyPropertyChanged(x => this.InfoMappingRegEx);
		}

		public async Task SelectAll()
        {
            IEnumerable<InfoMappingRegEx> lst = null;
            using (var ctx = new InfoMappingRegExRepository())
            {
                lst = await ctx.GetInfoMappingRegExByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedInfoMappingRegEx = new ObservableCollection<InfoMappingRegEx>(lst);
        }

 

		private string _keyRegXFilter;
        public string KeyRegXFilter
        {
            get
            {
                return _keyRegXFilter;
            }
            set
            {
                _keyRegXFilter = value;
				NotifyPropertyChanged(x => KeyRegXFilter);
                FilterData();
                
            }
        }	

 

		private string _fieldRxFilter;
        public string FieldRxFilter
        {
            get
            {
                return _fieldRxFilter;
            }
            set
            {
                _fieldRxFilter = value;
				NotifyPropertyChanged(x => FieldRxFilter);
                FilterData();
                
            }
        }	

 

		private string _keyReplaceRxFilter;
        public string KeyReplaceRxFilter
        {
            get
            {
                return _keyReplaceRxFilter;
            }
            set
            {
                _keyReplaceRxFilter = value;
				NotifyPropertyChanged(x => KeyReplaceRxFilter);
                FilterData();
                
            }
        }	

 

		private string _fieldReplaceRxFilter;
        public string FieldReplaceRxFilter
        {
            get
            {
                return _fieldReplaceRxFilter;
            }
            set
            {
                _fieldReplaceRxFilter = value;
				NotifyPropertyChanged(x => FieldReplaceRxFilter);
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

			InfoMappingRegEx.Refresh();
			NotifyPropertyChanged(x => this.InfoMappingRegEx);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(KeyRegXFilter) == false)
						res.Append(" && " + string.Format("KeyRegX.Contains(\"{0}\")",  KeyRegXFilter));						
 

									if(string.IsNullOrEmpty(FieldRxFilter) == false)
						res.Append(" && " + string.Format("FieldRx.Contains(\"{0}\")",  FieldRxFilter));						
 

									if(string.IsNullOrEmpty(KeyReplaceRxFilter) == false)
						res.Append(" && " + string.Format("KeyReplaceRx.Contains(\"{0}\")",  KeyReplaceRxFilter));						
 

									if(string.IsNullOrEmpty(FieldReplaceRxFilter) == false)
						res.Append(" && " + string.Format("FieldReplaceRx.Contains(\"{0}\")",  FieldReplaceRxFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<InfoMappingRegEx> lst = null;
            using (var ctx = new InfoMappingRegExRepository())
            {
                lst = await ctx.GetInfoMappingRegExByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<InfoMappingRegExExcelLine, List<InfoMappingRegExExcelLine>>
            {
                dataToPrint = lst.Select(x => new InfoMappingRegExExcelLine
                {
 
                    KeyRegX = x.KeyRegX ,
                    
 
                    FieldRx = x.FieldRx ,
                    
 
                    KeyReplaceRx = x.KeyReplaceRx ,
                    
 
                    FieldReplaceRx = x.FieldReplaceRx 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public class InfoMappingRegExExcelLine
        {
		 
                    public string KeyRegX { get; set; } 
                    
 
                    public string FieldRx { get; set; } 
                    
 
                    public string KeyReplaceRx { get; set; } 
                    
 
                    public string FieldReplaceRx { get; set; } 
                    
        }

		
    }
}
		