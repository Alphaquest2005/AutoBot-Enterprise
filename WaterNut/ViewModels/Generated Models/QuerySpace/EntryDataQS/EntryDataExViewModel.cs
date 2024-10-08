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

using EntryDataQS.Client.Entities;
using EntryDataQS.Client.Repositories;
//using WaterNut.Client.Repositories;
        
using CoreEntities.Client.Entities;


namespace WaterNut.QuerySpace.EntryDataQS.ViewModels
{
    
	public partial class EntryDataExViewModel_AutoGen : ViewModelBase<EntryDataExViewModel_AutoGen>
	{

       private static readonly EntryDataExViewModel_AutoGen instance;
       static EntryDataExViewModel_AutoGen()
        {
            instance = new EntryDataExViewModel_AutoGen();
        }

       public static EntryDataExViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public EntryDataExViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<EntryDataEx>(MessageToken.CurrentEntryDataExChanged, OnCurrentEntryDataExChanged);
            RegisterToReceiveMessages(MessageToken.EntryDataExChanged, OnEntryDataExChanged);
			RegisterToReceiveMessages(MessageToken.EntryDataExFilterExpressionChanged, OnEntryDataExFilterExpressionChanged);


 			// Recieve messages for Core Current Entities Changed
                        RegisterToReceiveMessages<ApplicationSettings>(CoreEntities.MessageToken.CurrentApplicationSettingsChanged, OnCurrentApplicationSettingsChanged);
                        RegisterToReceiveMessages<AsycudaDocumentSet>(CoreEntities.MessageToken.CurrentAsycudaDocumentSetChanged, OnCurrentAsycudaDocumentSetChanged);
 

			EntryDataEx = new VirtualList<EntryDataEx>(vloader);
			EntryDataEx.LoadingStateChanged += EntryDataEx_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(EntryDataEx, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<EntryDataEx> _EntryDataEx = null;
        public VirtualList<EntryDataEx> EntryDataEx
        {
            get
            {
                return _EntryDataEx;
            }
            set
            {
                _EntryDataEx = value;
                NotifyPropertyChanged( x => x.EntryDataEx);
            }
        }

		 private void OnEntryDataExFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => EntryDataEx.Refresh()).ConfigureAwait(false);
            SelectedEntryDataEx.Clear();
            NotifyPropertyChanged(x => SelectedEntryDataEx);
            BeginSendMessage(MessageToken.SelectedEntryDataExChanged, new NotificationEventArgs(MessageToken.SelectedEntryDataExChanged));
        }

		void EntryDataEx_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (EntryDataEx.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => EntryDataEx);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("EntryDataEx | Error occured..." + EntryDataEx.LastLoadingError.Message);
                    NotifyPropertyChanged(x => EntryDataEx);
                    break;
            }
           
        }

		
		public readonly EntryDataExVirturalListLoader vloader = new EntryDataExVirturalListLoader();

		private ObservableCollection<EntryDataEx> _selectedEntryDataEx = new ObservableCollection<EntryDataEx>();
        public ObservableCollection<EntryDataEx> SelectedEntryDataEx
        {
            get
            {
                return _selectedEntryDataEx;
            }
            set
            {
                _selectedEntryDataEx = value;
				BeginSendMessage(MessageToken.SelectedEntryDataExChanged,
                                    new NotificationEventArgs(MessageToken.SelectedEntryDataExChanged));
				 NotifyPropertyChanged(x => SelectedEntryDataEx);
            }
        }

        internal virtual void OnCurrentEntryDataExChanged(object sender, NotificationEventArgs<EntryDataEx> e)
        {
            if(BaseViewModel.Instance.CurrentEntryDataEx != null) BaseViewModel.Instance.CurrentEntryDataEx.PropertyChanged += CurrentEntryDataEx__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentEntryDataEx);
        }   

            void CurrentEntryDataEx__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    //if (e.PropertyName == "AddApplicationSettings")
                   // {
                   //    if(ApplicationSettings.Contains(CurrentEntryDataEx.ApplicationSettings) == false) ApplicationSettings.Add(CurrentEntryDataEx.ApplicationSettings);
                    //}
                    //if (e.PropertyName == "AddAsycudaDocumentSet")
                   // {
                   //    if(AsycudaDocumentSet.Contains(CurrentEntryDataEx.AsycudaDocumentSet) == false) AsycudaDocumentSet.Add(CurrentEntryDataEx.AsycudaDocumentSet);
                    //}
                 } 
        internal virtual void OnEntryDataExChanged(object sender, NotificationEventArgs e)
        {
            _EntryDataEx.Refresh();
			NotifyPropertyChanged(x => this.EntryDataEx);
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
					
                    EntryDataEx.Refresh();
					NotifyPropertyChanged(x => this.EntryDataEx);
				}
                internal virtual void OnCurrentAsycudaDocumentSetChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<AsycudaDocumentSet> e)
				{
				if (e.Data == null || e.Data.AsycudaDocumentSetId == null)
                {
                    vloader.FilterExpression = null;
                }
                else
                {
                    vloader.FilterExpression = string.Format("AsycudaDocumentSetId == {0}", e.Data.AsycudaDocumentSetId.ToString());
                }
					
                    EntryDataEx.Refresh();
					NotifyPropertyChanged(x => this.EntryDataEx);
				}
  
// Filtering Each Field except IDs
		public void ViewAll()
        {
			vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";




			vloader.ClearNavigationExpression();
			_EntryDataEx.Refresh();
			NotifyPropertyChanged(x => this.EntryDataEx);
		}

		public async Task SelectAll()
        {
            IEnumerable<EntryDataEx> lst = null;
            using (var ctx = new EntryDataExRepository())
            {
                lst = await ctx.GetEntryDataExByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedEntryDataEx = new ObservableCollection<EntryDataEx>(lst);
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

 
		private DateTime? _startInvoiceDateFilter = DateTime.Parse(string.Format("{0}/1/{1}", DateTime.Now.Month ,DateTime.Now.Year));
        public DateTime? StartInvoiceDateFilter
        {
            get
            {
                return _startInvoiceDateFilter;
            }
            set
            {
                _startInvoiceDateFilter = value;
				NotifyPropertyChanged(x => StartInvoiceDateFilter);
                FilterData();
                
            }
        }	

		private DateTime? _endInvoiceDateFilter = DateTime.Parse(string.Format("{1}/{0}/{2}", DateTime.DaysInMonth( DateTime.Now.Year,DateTime.Now.Month), DateTime.Now.Month, DateTime.Now.Year));
        public DateTime? EndInvoiceDateFilter
        {
            get
            {
                return _endInvoiceDateFilter;
            }
            set
            {
                _endInvoiceDateFilter = value;
				NotifyPropertyChanged(x => EndInvoiceDateFilter);
                FilterData();
                
            }
        }
 

		private DateTime? _invoiceDateFilter;
        public DateTime? InvoiceDateFilter
        {
            get
            {
                return _invoiceDateFilter;
            }
            set
            {
                _invoiceDateFilter = value;
				NotifyPropertyChanged(x => InvoiceDateFilter);
                FilterData();
                
            }
        }	

 

		private string _invoiceNoFilter;
        public string InvoiceNoFilter
        {
            get
            {
                return _invoiceNoFilter;
            }
            set
            {
                _invoiceNoFilter = value;
				NotifyPropertyChanged(x => InvoiceNoFilter);
                FilterData();
                
            }
        }	

 

		private Int32? _importedLinesFilter;
        public Int32? ImportedLinesFilter
        {
            get
            {
                return _importedLinesFilter;
            }
            set
            {
                _importedLinesFilter = value;
				NotifyPropertyChanged(x => ImportedLinesFilter);
                FilterData();
                
            }
        }	

 

		private Int32? _totalLinesFilter;
        public Int32? TotalLinesFilter
        {
            get
            {
                return _totalLinesFilter;
            }
            set
            {
                _totalLinesFilter = value;
				NotifyPropertyChanged(x => TotalLinesFilter);
                FilterData();
                
            }
        }	

 

		private string _currencyFilter;
        public string CurrencyFilter
        {
            get
            {
                return _currencyFilter;
            }
            set
            {
                _currencyFilter = value;
				NotifyPropertyChanged(x => CurrencyFilter);
                FilterData();
                
            }
        }	

 

		private string _dutyFreePaidFilter;
        public string DutyFreePaidFilter
        {
            get
            {
                return _dutyFreePaidFilter;
            }
            set
            {
                _dutyFreePaidFilter = value;
				NotifyPropertyChanged(x => DutyFreePaidFilter);
                FilterData();
                
            }
        }	

 

		private string _emailIdFilter;
        public string EmailIdFilter
        {
            get
            {
                return _emailIdFilter;
            }
            set
            {
                _emailIdFilter = value;
				NotifyPropertyChanged(x => EmailIdFilter);
                FilterData();
                
            }
        }	

 

		private Double? _invoiceTotalFilter;
        public Double? InvoiceTotalFilter
        {
            get
            {
                return _invoiceTotalFilter;
            }
            set
            {
                _invoiceTotalFilter = value;
				NotifyPropertyChanged(x => InvoiceTotalFilter);
                FilterData();
                
            }
        }	

 

		private string _supplierCodeFilter;
        public string SupplierCodeFilter
        {
            get
            {
                return _supplierCodeFilter;
            }
            set
            {
                _supplierCodeFilter = value;
				NotifyPropertyChanged(x => SupplierCodeFilter);
                FilterData();
                
            }
        }	

 

		private Double? _importedTotalFilter;
        public Double? ImportedTotalFilter
        {
            get
            {
                return _importedTotalFilter;
            }
            set
            {
                _importedTotalFilter = value;
				NotifyPropertyChanged(x => ImportedTotalFilter);
                FilterData();
                
            }
        }	

 

		private Double? _expectedTotalFilter;
        public Double? ExpectedTotalFilter
        {
            get
            {
                return _expectedTotalFilter;
            }
            set
            {
                _expectedTotalFilter = value;
				NotifyPropertyChanged(x => ExpectedTotalFilter);
                FilterData();
                
            }
        }	

 

		private Double? _totalInternalFreightFilter;
        public Double? TotalInternalFreightFilter
        {
            get
            {
                return _totalInternalFreightFilter;
            }
            set
            {
                _totalInternalFreightFilter = value;
				NotifyPropertyChanged(x => TotalInternalFreightFilter);
                FilterData();
                
            }
        }	

 

		private Double? _totalInternalInsuranceFilter;
        public Double? TotalInternalInsuranceFilter
        {
            get
            {
                return _totalInternalInsuranceFilter;
            }
            set
            {
                _totalInternalInsuranceFilter = value;
				NotifyPropertyChanged(x => TotalInternalInsuranceFilter);
                FilterData();
                
            }
        }	

 

		private Double? _totalOtherCostFilter;
        public Double? TotalOtherCostFilter
        {
            get
            {
                return _totalOtherCostFilter;
            }
            set
            {
                _totalOtherCostFilter = value;
				NotifyPropertyChanged(x => TotalOtherCostFilter);
                FilterData();
                
            }
        }	

 

		private Double? _totalDeductionsFilter;
        public Double? TotalDeductionsFilter
        {
            get
            {
                return _totalDeductionsFilter;
            }
            set
            {
                _totalDeductionsFilter = value;
				NotifyPropertyChanged(x => TotalDeductionsFilter);
                FilterData();
                
            }
        }	

 

		private Double? _totalFreightFilter;
        public Double? TotalFreightFilter
        {
            get
            {
                return _totalFreightFilter;
            }
            set
            {
                _totalFreightFilter = value;
				NotifyPropertyChanged(x => TotalFreightFilter);
                FilterData();
                
            }
        }	

 

		private Double? _totalsFilter;
        public Double? TotalsFilter
        {
            get
            {
                return _totalsFilter;
            }
            set
            {
                _totalsFilter = value;
				NotifyPropertyChanged(x => TotalsFilter);
                FilterData();
                
            }
        }	

 

		private string _sourceFileFilter;
        public string SourceFileFilter
        {
            get
            {
                return _sourceFileFilter;
            }
            set
            {
                _sourceFileFilter = value;
				NotifyPropertyChanged(x => SourceFileFilter);
                FilterData();
                
            }
        }	

 

		private string _supplierInvoiceNoFilter;
        public string SupplierInvoiceNoFilter
        {
            get
            {
                return _supplierInvoiceNoFilter;
            }
            set
            {
                _supplierInvoiceNoFilter = value;
				NotifyPropertyChanged(x => SupplierInvoiceNoFilter);
                FilterData();
                
            }
        }	

 

		private Double? _taxFilter;
        public Double? TaxFilter
        {
            get
            {
                return _taxFilter;
            }
            set
            {
                _taxFilter = value;
				NotifyPropertyChanged(x => TaxFilter);
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

			EntryDataEx.Refresh();
			NotifyPropertyChanged(x => this.EntryDataEx);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(TypeFilter) == false)
						res.Append(" && " + string.Format("Type.Contains(\"{0}\")",  TypeFilter));						
 

 

				if (Convert.ToDateTime(StartInvoiceDateFilter).Date != DateTime.MinValue &&
		        Convert.ToDateTime(EndInvoiceDateFilter).Date != DateTime.MinValue) res.Append(" && (");

					if (Convert.ToDateTime(StartInvoiceDateFilter).Date != DateTime.MinValue)
						{
							if(StartInvoiceDateFilter.HasValue)
								res.Append(
                                            (Convert.ToDateTime(EndInvoiceDateFilter).Date != DateTime.MinValue?"":" && ") +
                                            string.Format("InvoiceDate >= \"{0}\"",  Convert.ToDateTime(StartInvoiceDateFilter).Date.ToString("MM/dd/yyyy")));
						}

					if (Convert.ToDateTime(EndInvoiceDateFilter).Date != DateTime.MinValue)
						{
							if(EndInvoiceDateFilter.HasValue)
								res.Append(" && " + string.Format("InvoiceDate <= \"{0}\"",  Convert.ToDateTime(EndInvoiceDateFilter).Date.AddHours(23).ToString("MM/dd/yyyy HH:mm:ss")));
						}

				if (Convert.ToDateTime(StartInvoiceDateFilter).Date != DateTime.MinValue &&
		        Convert.ToDateTime(EndInvoiceDateFilter).Date != DateTime.MinValue) res.Append(" )");

					if (Convert.ToDateTime(_invoiceDateFilter).Date != DateTime.MinValue)
						{
							if(InvoiceDateFilter.HasValue)
								res.Append(" && " + string.Format("InvoiceDate == \"{0}\"",  Convert.ToDateTime(InvoiceDateFilter).Date.ToString("MM/dd/yyyy")));
						}
				 

									if(string.IsNullOrEmpty(InvoiceNoFilter) == false)
						res.Append(" && " + string.Format("InvoiceNo.Contains(\"{0}\")",  InvoiceNoFilter));						
 

					if(ImportedLinesFilter.HasValue)
						res.Append(" && " + string.Format("ImportedLines == {0}",  ImportedLinesFilter.ToString()));				 

					if(TotalLinesFilter.HasValue)
						res.Append(" && " + string.Format("TotalLines == {0}",  TotalLinesFilter.ToString()));				 

									if(string.IsNullOrEmpty(CurrencyFilter) == false)
						res.Append(" && " + string.Format("Currency.Contains(\"{0}\")",  CurrencyFilter));						
 

									if(string.IsNullOrEmpty(DutyFreePaidFilter) == false)
						res.Append(" && " + string.Format("DutyFreePaid.Contains(\"{0}\")",  DutyFreePaidFilter));						
 

									if(string.IsNullOrEmpty(EmailIdFilter) == false)
						res.Append(" && " + string.Format("EmailId.Contains(\"{0}\")",  EmailIdFilter));						
 

					if(InvoiceTotalFilter.HasValue)
						res.Append(" && " + string.Format("InvoiceTotal == {0}",  InvoiceTotalFilter.ToString()));				 

									if(string.IsNullOrEmpty(SupplierCodeFilter) == false)
						res.Append(" && " + string.Format("SupplierCode.Contains(\"{0}\")",  SupplierCodeFilter));						
 

					if(ImportedTotalFilter.HasValue)
						res.Append(" && " + string.Format("ImportedTotal == {0}",  ImportedTotalFilter.ToString()));				 

					if(ExpectedTotalFilter.HasValue)
						res.Append(" && " + string.Format("ExpectedTotal == {0}",  ExpectedTotalFilter.ToString()));				 

					if(TotalInternalFreightFilter.HasValue)
						res.Append(" && " + string.Format("TotalInternalFreight == {0}",  TotalInternalFreightFilter.ToString()));				 

					if(TotalInternalInsuranceFilter.HasValue)
						res.Append(" && " + string.Format("TotalInternalInsurance == {0}",  TotalInternalInsuranceFilter.ToString()));				 

					if(TotalOtherCostFilter.HasValue)
						res.Append(" && " + string.Format("TotalOtherCost == {0}",  TotalOtherCostFilter.ToString()));				 

					if(TotalDeductionsFilter.HasValue)
						res.Append(" && " + string.Format("TotalDeductions == {0}",  TotalDeductionsFilter.ToString()));				 

					if(TotalFreightFilter.HasValue)
						res.Append(" && " + string.Format("TotalFreight == {0}",  TotalFreightFilter.ToString()));				 

					if(TotalsFilter.HasValue)
						res.Append(" && " + string.Format("Totals == {0}",  TotalsFilter.ToString()));				 

									if(string.IsNullOrEmpty(SourceFileFilter) == false)
						res.Append(" && " + string.Format("SourceFile.Contains(\"{0}\")",  SourceFileFilter));						
 

									if(string.IsNullOrEmpty(SupplierInvoiceNoFilter) == false)
						res.Append(" && " + string.Format("SupplierInvoiceNo.Contains(\"{0}\")",  SupplierInvoiceNoFilter));						
 

					if(TaxFilter.HasValue)
						res.Append(" && " + string.Format("Tax == {0}",  TaxFilter.ToString()));							return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<EntryDataEx> lst = null;
            using (var ctx = new EntryDataExRepository())
            {
                lst = await ctx.GetEntryDataExByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<EntryDataExExcelLine, List<EntryDataExExcelLine>>
            {
                dataToPrint = lst.Select(x => new EntryDataExExcelLine
                {
 
                    Type = x.Type ,
                    
 
                    InvoiceDate = x.InvoiceDate ,
                    
 
                    InvoiceNo = x.InvoiceNo ,
                    
 
                    ImportedLines = x.ImportedLines ,
                    
 
                    TotalLines = x.TotalLines ,
                    
 
                    Currency = x.Currency ,
                    
 
                    DutyFreePaid = x.DutyFreePaid ,
                    
 
                    EmailId = x.EmailId ,
                    
 
                    InvoiceTotal = x.InvoiceTotal ,
                    
 
                    SupplierCode = x.SupplierCode ,
                    
 
                    ImportedTotal = x.ImportedTotal ,
                    
 
                    ExpectedTotal = x.ExpectedTotal ,
                    
 
                    TotalInternalFreight = x.TotalInternalFreight ,
                    
 
                    TotalInternalInsurance = x.TotalInternalInsurance ,
                    
 
                    TotalOtherCost = x.TotalOtherCost ,
                    
 
                    TotalDeductions = x.TotalDeductions ,
                    
 
                    TotalFreight = x.TotalFreight ,
                    
 
                    Totals = x.Totals ,
                    
 
                    SourceFile = x.SourceFile ,
                    
 
                    SupplierInvoiceNo = x.SupplierInvoiceNo ,
                    
 
                    Tax = x.Tax 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class EntryDataExExcelLine
        {
		 
                    public string Type { get; set; } 
                    
 
                    public System.DateTime InvoiceDate { get; set; } 
                    
 
                    public string InvoiceNo { get; set; } 
                    
 
                    public Nullable<int> ImportedLines { get; set; } 
                    
 
                    public Nullable<int> TotalLines { get; set; } 
                    
 
                    public string Currency { get; set; } 
                    
 
                    public string DutyFreePaid { get; set; } 
                    
 
                    public string EmailId { get; set; } 
                    
 
                    public Nullable<double> InvoiceTotal { get; set; } 
                    
 
                    public string SupplierCode { get; set; } 
                    
 
                    public Nullable<double> ImportedTotal { get; set; } 
                    
 
                    public double ExpectedTotal { get; set; } 
                    
 
                    public double TotalInternalFreight { get; set; } 
                    
 
                    public double TotalInternalInsurance { get; set; } 
                    
 
                    public double TotalOtherCost { get; set; } 
                    
 
                    public double TotalDeductions { get; set; } 
                    
 
                    public double TotalFreight { get; set; } 
                    
 
                    public double Totals { get; set; } 
                    
 
                    public string SourceFile { get; set; } 
                    
 
                    public string SupplierInvoiceNo { get; set; } 
                    
 
                    public Nullable<double> Tax { get; set; } 
                    
        }

		
    }
}
		
