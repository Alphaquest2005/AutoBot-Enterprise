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
    
	public partial class AttachmentsViewModel_AutoGen : ViewModelBase<AttachmentsViewModel_AutoGen>
	{

       private static readonly AttachmentsViewModel_AutoGen instance;
       static AttachmentsViewModel_AutoGen()
        {
            instance = new AttachmentsViewModel_AutoGen();
        }

       public static AttachmentsViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public AttachmentsViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<Attachments>(MessageToken.CurrentAttachmentsChanged, OnCurrentAttachmentsChanged);
            RegisterToReceiveMessages(MessageToken.AttachmentsChanged, OnAttachmentsChanged);
			RegisterToReceiveMessages(MessageToken.AttachmentsFilterExpressionChanged, OnAttachmentsFilterExpressionChanged);


 			// Recieve messages for Core Current Entities Changed
 

			Attachments = new VirtualList<Attachments>(vloader);
			Attachments.LoadingStateChanged += Attachments_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(Attachments, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<Attachments> _Attachments = null;
        public VirtualList<Attachments> Attachments
        {
            get
            {
                return _Attachments;
            }
            set
            {
                _Attachments = value;
            }
        }

		 private void OnAttachmentsFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Attachments.Refresh();
            SelectedAttachments.Clear();
            NotifyPropertyChanged(x => SelectedAttachments);
            BeginSendMessage(MessageToken.SelectedAttachmentsChanged, new NotificationEventArgs(MessageToken.SelectedAttachmentsChanged));
        }

		void Attachments_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (Attachments.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => Attachments);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("Attachments | Error occured..." + Attachments.LastLoadingError.Message);
                    NotifyPropertyChanged(x => Attachments);
                    break;
            }
           
        }

		
		public readonly AttachmentsVirturalListLoader vloader = new AttachmentsVirturalListLoader();

		private ObservableCollection<Attachments> _selectedAttachments = new ObservableCollection<Attachments>();
        public ObservableCollection<Attachments> SelectedAttachments
        {
            get
            {
                return _selectedAttachments;
            }
            set
            {
                _selectedAttachments = value;
				BeginSendMessage(MessageToken.SelectedAttachmentsChanged,
                                    new NotificationEventArgs(MessageToken.SelectedAttachmentsChanged));
				 NotifyPropertyChanged(x => SelectedAttachments);
            }
        }

        internal void OnCurrentAttachmentsChanged(object sender, NotificationEventArgs<Attachments> e)
        {
            if(BaseViewModel.Instance.CurrentAttachments != null) BaseViewModel.Instance.CurrentAttachments.PropertyChanged += CurrentAttachments__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentAttachments);
        }   

            void CurrentAttachments__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                 } 
        internal void OnAttachmentsChanged(object sender, NotificationEventArgs e)
        {
            _Attachments.Refresh();
			NotifyPropertyChanged(x => this.Attachments);
        }   


 
  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
		public void ViewAll()
        {
		vloader.FilterExpression = "All";




			vloader.ClearNavigationExpression();
			_Attachments.Refresh();
			NotifyPropertyChanged(x => this.Attachments);
		}

		public async Task SelectAll()
        {
            IEnumerable<Attachments> lst = null;
            using (var ctx = new AttachmentsRepository())
            {
                lst = await ctx.GetAttachmentsByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedAttachments = new ObservableCollection<Attachments>(lst);
        }

 

		private string _filePathFilter;
        public string FilePathFilter
        {
            get
            {
                return _filePathFilter;
            }
            set
            {
                _filePathFilter = value;
				NotifyPropertyChanged(x => FilePathFilter);
                FilterData();
                
            }
        }	

 

		private string _documentCodeFilter;
        public string DocumentCodeFilter
        {
            get
            {
                return _documentCodeFilter;
            }
            set
            {
                _documentCodeFilter = value;
				NotifyPropertyChanged(x => DocumentCodeFilter);
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

			Attachments.Refresh();
			NotifyPropertyChanged(x => this.Attachments);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(FilePathFilter) == false)
						res.Append(" && " + string.Format("FilePath.Contains(\"{0}\")",  FilePathFilter));						
 

									if(string.IsNullOrEmpty(DocumentCodeFilter) == false)
						res.Append(" && " + string.Format("DocumentCode.Contains(\"{0}\")",  DocumentCodeFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<Attachments> lst = null;
            using (var ctx = new AttachmentsRepository())
            {
                lst = await ctx.GetAttachmentsByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<AttachmentsExcelLine, List<AttachmentsExcelLine>>
            {
                dataToPrint = lst.Select(x => new AttachmentsExcelLine
                {
 
                    FilePath = x.FilePath ,
                    
 
                    DocumentCode = x.DocumentCode 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public class AttachmentsExcelLine
        {
		 
                    public string FilePath { get; set; } 
                    
 
                    public string DocumentCode { get; set; } 
                    
        }

		
    }
}
		