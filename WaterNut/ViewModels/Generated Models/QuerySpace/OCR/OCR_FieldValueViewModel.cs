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
    
	public partial class OCR_FieldValueViewModel_AutoGen : ViewModelBase<OCR_FieldValueViewModel_AutoGen>
	{

       private static readonly OCR_FieldValueViewModel_AutoGen instance;
       static OCR_FieldValueViewModel_AutoGen()
        {
            instance = new OCR_FieldValueViewModel_AutoGen();
        }

       public static OCR_FieldValueViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public OCR_FieldValueViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<OCR_FieldValue>(MessageToken.CurrentOCR_FieldValueChanged, OnCurrentOCR_FieldValueChanged);
            RegisterToReceiveMessages(MessageToken.OCR_FieldValueChanged, OnOCR_FieldValueChanged);
			RegisterToReceiveMessages(MessageToken.OCR_FieldValueFilterExpressionChanged, OnOCR_FieldValueFilterExpressionChanged);

 
			RegisterToReceiveMessages<Fields>(MessageToken.CurrentFieldsChanged, OnCurrentFieldChanged);

 			// Recieve messages for Core Current Entities Changed
 

			OCR_FieldValue = new VirtualList<OCR_FieldValue>(vloader);
			OCR_FieldValue.LoadingStateChanged += OCR_FieldValue_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(OCR_FieldValue, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<OCR_FieldValue> _OCR_FieldValue = null;
        public VirtualList<OCR_FieldValue> OCR_FieldValue
        {
            get
            {
                return _OCR_FieldValue;
            }
            set
            {
                _OCR_FieldValue = value;
            }
        }

		 private void OnOCR_FieldValueFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			OCR_FieldValue.Refresh();
            SelectedOCR_FieldValue.Clear();
            NotifyPropertyChanged(x => SelectedOCR_FieldValue);
            BeginSendMessage(MessageToken.SelectedOCR_FieldValueChanged, new NotificationEventArgs(MessageToken.SelectedOCR_FieldValueChanged));
        }

		void OCR_FieldValue_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (OCR_FieldValue.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => OCR_FieldValue);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("OCR_FieldValue | Error occured..." + OCR_FieldValue.LastLoadingError.Message);
                    NotifyPropertyChanged(x => OCR_FieldValue);
                    break;
            }
           
        }

		
		public readonly OCR_FieldValueVirturalListLoader vloader = new OCR_FieldValueVirturalListLoader();

		private ObservableCollection<OCR_FieldValue> _selectedOCR_FieldValue = new ObservableCollection<OCR_FieldValue>();
        public ObservableCollection<OCR_FieldValue> SelectedOCR_FieldValue
        {
            get
            {
                return _selectedOCR_FieldValue;
            }
            set
            {
                _selectedOCR_FieldValue = value;
				BeginSendMessage(MessageToken.SelectedOCR_FieldValueChanged,
                                    new NotificationEventArgs(MessageToken.SelectedOCR_FieldValueChanged));
				 NotifyPropertyChanged(x => SelectedOCR_FieldValue);
            }
        }

        internal virtual void OnCurrentOCR_FieldValueChanged(object sender, NotificationEventArgs<OCR_FieldValue> e)
        {
            if(BaseViewModel.Instance.CurrentOCR_FieldValue != null) BaseViewModel.Instance.CurrentOCR_FieldValue.PropertyChanged += CurrentOCR_FieldValue__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentOCR_FieldValue);
        }   

            void CurrentOCR_FieldValue__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    //if (e.PropertyName == "AddField")
                   // {
                   //    if(Fields.Contains(CurrentOCR_FieldValue.Field) == false) Fields.Add(CurrentOCR_FieldValue.Field);
                    //}
                 } 
        internal virtual void OnOCR_FieldValueChanged(object sender, NotificationEventArgs e)
        {
            _OCR_FieldValue.Refresh();
			NotifyPropertyChanged(x => this.OCR_FieldValue);
        }   


 	
		 internal virtual void OnCurrentFieldChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<Fields> e)
			{
			if(ViewCurrentField == false) return;
			if (e.Data == null || e.Data.Id == null)
                {
                    vloader.FilterExpression = "None";
                }
                else
                {
				vloader.FilterExpression = string.Format("Id == {0}", e.Data.Id.ToString());
                 }

				OCR_FieldValue.Refresh();
				NotifyPropertyChanged(x => this.OCR_FieldValue);
                // SendMessage(MessageToken.OCR_FieldValueChanged, new NotificationEventArgs(MessageToken.OCR_FieldValueChanged));
                			}

  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
 	
		 bool _viewCurrentField = false;
         public bool ViewCurrentField
         {
             get
             {
                 return _viewCurrentField;
             }
             set
             {
                 _viewCurrentField = value;
                 NotifyPropertyChanged(x => x.ViewCurrentField);
                FilterData();
             }
         }
		public void ViewAll()
        {
		vloader.FilterExpression = "All";




			vloader.ClearNavigationExpression();
			_OCR_FieldValue.Refresh();
			NotifyPropertyChanged(x => this.OCR_FieldValue);
		}

		public async Task SelectAll()
        {
            IEnumerable<OCR_FieldValue> lst = null;
            using (var ctx = new OCR_FieldValueRepository())
            {
                lst = await ctx.GetOCR_FieldValueByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedOCR_FieldValue = new ObservableCollection<OCR_FieldValue>(lst);
        }

 

		private string _valueFilter;
        public string ValueFilter
        {
            get
            {
                return _valueFilter;
            }
            set
            {
                _valueFilter = value;
				NotifyPropertyChanged(x => ValueFilter);
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

			OCR_FieldValue.Refresh();
			NotifyPropertyChanged(x => this.OCR_FieldValue);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(ValueFilter) == false)
						res.Append(" && " + string.Format("Value.Contains(\"{0}\")",  ValueFilter));						
			return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<OCR_FieldValue> lst = null;
            using (var ctx = new OCR_FieldValueRepository())
            {
                lst = await ctx.GetOCR_FieldValueByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<OCR_FieldValueExcelLine, List<OCR_FieldValueExcelLine>>
            {
                dataToPrint = lst.Select(x => new OCR_FieldValueExcelLine
                {
 
                    Value = x.Value 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public class OCR_FieldValueExcelLine
        {
		 
                    public string Value { get; set; } 
                    
        }

		
    }
}
		