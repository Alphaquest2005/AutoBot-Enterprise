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
    
	public partial class LinesViewModel_AutoGen : ViewModelBase<LinesViewModel_AutoGen>
	{

       private static readonly LinesViewModel_AutoGen instance;
       static LinesViewModel_AutoGen()
        {
            instance = new LinesViewModel_AutoGen();
        }

       public static LinesViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public LinesViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<Lines>(MessageToken.CurrentLinesChanged, OnCurrentLinesChanged);
            RegisterToReceiveMessages(MessageToken.LinesChanged, OnLinesChanged);
			RegisterToReceiveMessages(MessageToken.LinesFilterExpressionChanged, OnLinesFilterExpressionChanged);

 
			RegisterToReceiveMessages<Parts>(MessageToken.CurrentPartsChanged, OnCurrentPartsChanged);
 
			RegisterToReceiveMessages<RegularExpressions>(MessageToken.CurrentRegularExpressionsChanged, OnCurrentRegularExpressionsChanged);

 			// Recieve messages for Core Current Entities Changed
 

			Lines = new VirtualList<Lines>(vloader);
			Lines.LoadingStateChanged += Lines_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(Lines, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<Lines> _Lines = null;
        public VirtualList<Lines> Lines
        {
            get
            {
                return _Lines;
            }
            set
            {
                _Lines = value;
                NotifyPropertyChanged( x => x.Lines);
            }
        }

		 private void OnLinesFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => Lines.Refresh()).ConfigureAwait(false);
            SelectedLines.Clear();
            NotifyPropertyChanged(x => SelectedLines);
            BeginSendMessage(MessageToken.SelectedLinesChanged, new NotificationEventArgs(MessageToken.SelectedLinesChanged));
        }

		void Lines_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (Lines.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => Lines);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("Lines | Error occured..." + Lines.LastLoadingError.Message);
                    NotifyPropertyChanged(x => Lines);
                    break;
            }
           
        }

		
		public readonly LinesVirturalListLoader vloader = new LinesVirturalListLoader();

		private ObservableCollection<Lines> _selectedLines = new ObservableCollection<Lines>();
        public ObservableCollection<Lines> SelectedLines
        {
            get
            {
                return _selectedLines;
            }
            set
            {
                _selectedLines = value;
				BeginSendMessage(MessageToken.SelectedLinesChanged,
                                    new NotificationEventArgs(MessageToken.SelectedLinesChanged));
				 NotifyPropertyChanged(x => SelectedLines);
            }
        }

        internal virtual void OnCurrentLinesChanged(object sender, NotificationEventArgs<Lines> e)
        {
            if(BaseViewModel.Instance.CurrentLines != null) BaseViewModel.Instance.CurrentLines.PropertyChanged += CurrentLines__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentLines);
        }   

            void CurrentLines__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    //if (e.PropertyName == "AddParts")
                   // {
                   //    if(Parts.Contains(CurrentLines.Parts) == false) Parts.Add(CurrentLines.Parts);
                    //}
                    //if (e.PropertyName == "AddRegularExpressions")
                   // {
                   //    if(RegularExpressions.Contains(CurrentLines.RegularExpressions) == false) RegularExpressions.Add(CurrentLines.RegularExpressions);
                    //}
                    //if (e.PropertyName == "AddParentLine")
                   // {
                   //    if(Lines.Contains(CurrentLines.ParentLine) == false) Lines.Add(CurrentLines.ParentLine);
                    //}
                 } 
        internal virtual void OnLinesChanged(object sender, NotificationEventArgs e)
        {
            _Lines.Refresh();
			NotifyPropertyChanged(x => this.Lines);
        }   


 	
		 internal virtual void OnCurrentPartsChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<Parts> e)
			{
			if(ViewCurrentParts == false) return;
			if (e.Data == null || e.Data.Id == null)
                {
                    vloader.FilterExpression = "None";
                }
                else
                {
				vloader.FilterExpression = string.Format("PartId == {0}", e.Data.Id.ToString());
                 }

				Lines.Refresh();
				NotifyPropertyChanged(x => this.Lines);
                // SendMessage(MessageToken.LinesChanged, new NotificationEventArgs(MessageToken.LinesChanged));
                			}
	
		 internal virtual void OnCurrentRegularExpressionsChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<RegularExpressions> e)
			{
			if(ViewCurrentRegularExpressions == false) return;
			if (e.Data == null || e.Data.Id == null)
                {
                    vloader.FilterExpression = "None";
                }
                else
                {
				vloader.FilterExpression = string.Format("RegExId == {0}", e.Data.Id.ToString());
                 }

				Lines.Refresh();
				NotifyPropertyChanged(x => this.Lines);
                // SendMessage(MessageToken.LinesChanged, new NotificationEventArgs(MessageToken.LinesChanged));
                			}

  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
 	
		 bool _viewCurrentParts = false;
         public bool ViewCurrentParts
         {
             get
             {
                 return _viewCurrentParts;
             }
             set
             {
                 _viewCurrentParts = value;
                 NotifyPropertyChanged(x => x.ViewCurrentParts);
                FilterData();
             }
         }
 	
		 bool _viewCurrentRegularExpressions = false;
         public bool ViewCurrentRegularExpressions
         {
             get
             {
                 return _viewCurrentRegularExpressions;
             }
             set
             {
                 _viewCurrentRegularExpressions = value;
                 NotifyPropertyChanged(x => x.ViewCurrentRegularExpressions);
                FilterData();
             }
         }
		public void ViewAll()
        {
		vloader.FilterExpression = "All";




			vloader.ClearNavigationExpression();
			_Lines.Refresh();
			NotifyPropertyChanged(x => this.Lines);
		}

		public async Task SelectAll()
        {
            IEnumerable<Lines> lst = null;
            using (var ctx = new LinesRepository())
            {
                lst = await ctx.GetLinesByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedLines = new ObservableCollection<Lines>(lst);
        }

 

		private string _nameFilter;
        public string NameFilter
        {
            get
            {
                return _nameFilter;
            }
            set
            {
                _nameFilter = value;
				NotifyPropertyChanged(x => NameFilter);
                FilterData();
                
            }
        }	

 

		private Boolean? _distinctValuesFilter;
        public Boolean? DistinctValuesFilter
        {
            get
            {
                return _distinctValuesFilter;
            }
            set
            {
                _distinctValuesFilter = value;
				NotifyPropertyChanged(x => DistinctValuesFilter);
                FilterData();
                
            }
        }	

 

		private Boolean? _isColumnFilter;
        public Boolean? IsColumnFilter
        {
            get
            {
                return _isColumnFilter;
            }
            set
            {
                _isColumnFilter = value;
				NotifyPropertyChanged(x => IsColumnFilter);
                FilterData();
                
            }
        }	

 

		private Boolean? _isActiveFilter;
        public Boolean? IsActiveFilter
        {
            get
            {
                return _isActiveFilter;
            }
            set
            {
                _isActiveFilter = value;
				NotifyPropertyChanged(x => IsActiveFilter);
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

			Lines.Refresh();
			NotifyPropertyChanged(x => this.Lines);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(NameFilter) == false)
						res.Append(" && " + string.Format("Name.Contains(\"{0}\")",  NameFilter));						
 

									if(DistinctValuesFilter.HasValue)
						res.Append(" && " + string.Format("DistinctValues == {0}",  DistinctValuesFilter));						
 

									if(IsColumnFilter.HasValue)
						res.Append(" && " + string.Format("IsColumn == {0}",  IsColumnFilter));						
 

									if(IsActiveFilter.HasValue)
						res.Append(" && " + string.Format("IsActive == {0}",  IsActiveFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<Lines> lst = null;
            using (var ctx = new LinesRepository())
            {
                lst = await ctx.GetLinesByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<LinesExcelLine, List<LinesExcelLine>>
            {
                dataToPrint = lst.Select(x => new LinesExcelLine
                {
 
                    Name = x.Name ,
                    
 
                    DistinctValues = x.DistinctValues ,
                    
 
                    IsColumn = x.IsColumn ,
                    
 
                    IsActive = x.IsActive 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class LinesExcelLine
        {
		 
                    public string Name { get; set; } 
                    
 
                    public Nullable<bool> DistinctValues { get; set; } 
                    
 
                    public Nullable<bool> IsColumn { get; set; } 
                    
 
                    public Nullable<bool> IsActive { get; set; } 
                    
        }

		
    }
}
		
