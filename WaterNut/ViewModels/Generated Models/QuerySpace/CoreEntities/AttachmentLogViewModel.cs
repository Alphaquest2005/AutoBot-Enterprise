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
    
	public partial class AttachmentLogViewModel_AutoGen : ViewModelBase<AttachmentLogViewModel_AutoGen>
	{

       private static readonly AttachmentLogViewModel_AutoGen instance;
       static AttachmentLogViewModel_AutoGen()
        {
            instance = new AttachmentLogViewModel_AutoGen();
        }

       public static AttachmentLogViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public AttachmentLogViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<AttachmentLog>(MessageToken.CurrentAttachmentLogChanged, OnCurrentAttachmentLogChanged);
            RegisterToReceiveMessages(MessageToken.AttachmentLogChanged, OnAttachmentLogChanged);
			RegisterToReceiveMessages(MessageToken.AttachmentLogFilterExpressionChanged, OnAttachmentLogFilterExpressionChanged);

 
			RegisterToReceiveMessages<AsycudaDocumentSet_Attachments>(MessageToken.CurrentAsycudaDocumentSet_AttachmentsChanged, OnCurrentAsycudaDocumentSet_AttachmentsChanged);

 			// Recieve messages for Core Current Entities Changed
 

			AttachmentLog = new VirtualList<AttachmentLog>(vloader);
			AttachmentLog.LoadingStateChanged += AttachmentLog_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(AttachmentLog, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<AttachmentLog> _AttachmentLog = null;
        public VirtualList<AttachmentLog> AttachmentLog
        {
            get
            {
                return _AttachmentLog;
            }
            set
            {
                _AttachmentLog = value;
            }
        }

		 private void OnAttachmentLogFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			AttachmentLog.Refresh();
            SelectedAttachmentLog.Clear();
            NotifyPropertyChanged(x => SelectedAttachmentLog);
            BeginSendMessage(MessageToken.SelectedAttachmentLogChanged, new NotificationEventArgs(MessageToken.SelectedAttachmentLogChanged));
        }

		void AttachmentLog_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (AttachmentLog.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => AttachmentLog);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("AttachmentLog | Error occured..." + AttachmentLog.LastLoadingError.Message);
                    NotifyPropertyChanged(x => AttachmentLog);
                    break;
            }
           
        }

		
		public readonly AttachmentLogVirturalListLoader vloader = new AttachmentLogVirturalListLoader();

		private ObservableCollection<AttachmentLog> _selectedAttachmentLog = new ObservableCollection<AttachmentLog>();
        public ObservableCollection<AttachmentLog> SelectedAttachmentLog
        {
            get
            {
                return _selectedAttachmentLog;
            }
            set
            {
                _selectedAttachmentLog = value;
				BeginSendMessage(MessageToken.SelectedAttachmentLogChanged,
                                    new NotificationEventArgs(MessageToken.SelectedAttachmentLogChanged));
				 NotifyPropertyChanged(x => SelectedAttachmentLog);
            }
        }

        internal void OnCurrentAttachmentLogChanged(object sender, NotificationEventArgs<AttachmentLog> e)
        {
            if(BaseViewModel.Instance.CurrentAttachmentLog != null) BaseViewModel.Instance.CurrentAttachmentLog.PropertyChanged += CurrentAttachmentLog__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentAttachmentLog);
        }   

            void CurrentAttachmentLog__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    //if (e.PropertyName == "AddAsycudaDocumentSet_Attachments")
                   // {
                   //    if(AsycudaDocumentSet_Attachments.Contains(CurrentAttachmentLog.AsycudaDocumentSet_Attachments) == false) AsycudaDocumentSet_Attachments.Add(CurrentAttachmentLog.AsycudaDocumentSet_Attachments);
                    //}
                 } 
        internal void OnAttachmentLogChanged(object sender, NotificationEventArgs e)
        {
            _AttachmentLog.Refresh();
			NotifyPropertyChanged(x => this.AttachmentLog);
        }   


 	
		 internal void OnCurrentAsycudaDocumentSet_AttachmentsChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<AsycudaDocumentSet_Attachments> e)
			{
			if(ViewCurrentAsycudaDocumentSet_Attachments == false) return;
			if (e.Data == null || e.Data.Id == null)
                {
                    vloader.FilterExpression = "None";
                }
                else
                {
				vloader.FilterExpression = string.Format("DocSetAttachment == {0}", e.Data.Id.ToString());
                 }

				AttachmentLog.Refresh();
				NotifyPropertyChanged(x => this.AttachmentLog);
                // SendMessage(MessageToken.AttachmentLogChanged, new NotificationEventArgs(MessageToken.AttachmentLogChanged));
                                          
                BaseViewModel.Instance.CurrentAttachmentLog = null;
			}

  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
 	
		 bool _viewCurrentAsycudaDocumentSet_Attachments = false;
         public bool ViewCurrentAsycudaDocumentSet_Attachments
         {
             get
             {
                 return _viewCurrentAsycudaDocumentSet_Attachments;
             }
             set
             {
                 _viewCurrentAsycudaDocumentSet_Attachments = value;
                 NotifyPropertyChanged(x => x.ViewCurrentAsycudaDocumentSet_Attachments);
                FilterData();
             }
         }
		public void ViewAll()
        {
		vloader.FilterExpression = "All";




			vloader.ClearNavigationExpression();
			_AttachmentLog.Refresh();
			NotifyPropertyChanged(x => this.AttachmentLog);
		}

		public async Task SelectAll()
        {
            IEnumerable<AttachmentLog> lst = null;
            using (var ctx = new AttachmentLogRepository())
            {
                lst = await ctx.GetAttachmentLogByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedAttachmentLog = new ObservableCollection<AttachmentLog>(lst);
        }

 

		private Int32? _docSetAttachmentFilter;
        public Int32? DocSetAttachmentFilter
        {
            get
            {
                return _docSetAttachmentFilter;
            }
            set
            {
                _docSetAttachmentFilter = value;
				NotifyPropertyChanged(x => DocSetAttachmentFilter);
                FilterData();
                
            }
        }	

 

		private string _statusFilter;
        public string StatusFilter
        {
            get
            {
                return _statusFilter;
            }
            set
            {
                _statusFilter = value;
				NotifyPropertyChanged(x => StatusFilter);
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

			AttachmentLog.Refresh();
			NotifyPropertyChanged(x => this.AttachmentLog);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

					if(DocSetAttachmentFilter.HasValue)
						res.Append(" && " + string.Format("DocSetAttachment == {0}",  DocSetAttachmentFilter.ToString()));				 

									if(string.IsNullOrEmpty(StatusFilter) == false)
						res.Append(" && " + string.Format("Status.Contains(\"{0}\")",  StatusFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<AttachmentLog> lst = null;
            using (var ctx = new AttachmentLogRepository())
            {
                lst = await ctx.GetAttachmentLogByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<AttachmentLogExcelLine, List<AttachmentLogExcelLine>>
            {
                dataToPrint = lst.Select(x => new AttachmentLogExcelLine
                {
 
                    DocSetAttachment = x.DocSetAttachment ,
                    
 
                    Status = x.Status 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public class AttachmentLogExcelLine
        {
		 
                    public int DocSetAttachment { get; set; } 
                    
 
                    public string Status { get; set; } 
                    
        }

		
    }
}
		