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
    
	public partial class TODO_LICToCreateViewModel_AutoGen : ViewModelBase<TODO_LICToCreateViewModel_AutoGen>
	{

       private static readonly TODO_LICToCreateViewModel_AutoGen instance;
       static TODO_LICToCreateViewModel_AutoGen()
        {
            instance = new TODO_LICToCreateViewModel_AutoGen();
        }

       public static TODO_LICToCreateViewModel_AutoGen Instance
        {
            get { return instance; }
        }

       private readonly object lockObject = new object();
  
  

        public TODO_LICToCreateViewModel_AutoGen()
        {
          
            RegisterToReceiveMessages<TODO_LICToCreate>(MessageToken.CurrentTODO_LICToCreateChanged, OnCurrentTODO_LICToCreateChanged);
            RegisterToReceiveMessages(MessageToken.TODO_LICToCreateChanged, OnTODO_LICToCreateChanged);
			RegisterToReceiveMessages(MessageToken.TODO_LICToCreateFilterExpressionChanged, OnTODO_LICToCreateFilterExpressionChanged);


 			// Recieve messages for Core Current Entities Changed
 

			TODO_LICToCreate = new VirtualList<TODO_LICToCreate>(vloader);
			TODO_LICToCreate.LoadingStateChanged += TODO_LICToCreate_LoadingStateChanged;
            BindingOperations.EnableCollectionSynchronization(TODO_LICToCreate, lockObject);
			
            OnCreated();        
            OnTotals();
        }

        partial void OnCreated();
        partial void OnTotals();

		private VirtualList<TODO_LICToCreate> _TODO_LICToCreate = null;
        public VirtualList<TODO_LICToCreate> TODO_LICToCreate
        {
            get
            {
                return _TODO_LICToCreate;
            }
            set
            {
                _TODO_LICToCreate = value;
                NotifyPropertyChanged( x => x.TODO_LICToCreate);
            }
        }

		 private void OnTODO_LICToCreateFilterExpressionChanged(object sender, NotificationEventArgs e)
        {
			Task.Run(() => TODO_LICToCreate.Refresh()).ConfigureAwait(false);
            SelectedTODO_LICToCreate.Clear();
            NotifyPropertyChanged(x => SelectedTODO_LICToCreate);
            BeginSendMessage(MessageToken.SelectedTODO_LICToCreateChanged, new NotificationEventArgs(MessageToken.SelectedTODO_LICToCreateChanged));
        }

		void TODO_LICToCreate_LoadingStateChanged(object sender, EventArgs e)
        {
            switch (TODO_LICToCreate.LoadingState)
            {
                case QueuedBackgroundWorkerState.Processing:
                    StatusModel.Timer("Getting Data...");
                    break;
                case QueuedBackgroundWorkerState.Standby: 
                    StatusModel.StopStatusUpdate();
                    NotifyPropertyChanged(x => TODO_LICToCreate);
                    break;
                case QueuedBackgroundWorkerState.StoppedByError:
                    StatusModel.Error("TODO_LICToCreate | Error occured..." + TODO_LICToCreate.LastLoadingError.Message);
                    NotifyPropertyChanged(x => TODO_LICToCreate);
                    break;
            }
           
        }

		
		public readonly TODO_LICToCreateVirturalListLoader vloader = new TODO_LICToCreateVirturalListLoader();

		private ObservableCollection<TODO_LICToCreate> _selectedTODO_LICToCreate = new ObservableCollection<TODO_LICToCreate>();
        public ObservableCollection<TODO_LICToCreate> SelectedTODO_LICToCreate
        {
            get
            {
                return _selectedTODO_LICToCreate;
            }
            set
            {
                _selectedTODO_LICToCreate = value;
				BeginSendMessage(MessageToken.SelectedTODO_LICToCreateChanged,
                                    new NotificationEventArgs(MessageToken.SelectedTODO_LICToCreateChanged));
				 NotifyPropertyChanged(x => SelectedTODO_LICToCreate);
            }
        }

        internal virtual void OnCurrentTODO_LICToCreateChanged(object sender, NotificationEventArgs<TODO_LICToCreate> e)
        {
            if(BaseViewModel.Instance.CurrentTODO_LICToCreate != null) BaseViewModel.Instance.CurrentTODO_LICToCreate.PropertyChanged += CurrentTODO_LICToCreate__propertyChanged;
           // NotifyPropertyChanged(x => this.CurrentTODO_LICToCreate);
        }   

            void CurrentTODO_LICToCreate__propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                 } 
        internal virtual void OnTODO_LICToCreateChanged(object sender, NotificationEventArgs e)
        {
            _TODO_LICToCreate.Refresh();
			NotifyPropertyChanged(x => this.TODO_LICToCreate);
        }   


 
  			// Core Current Entities Changed
			// theorticall don't need this cuz i am inheriting from core entities baseview model so changes should flow up to here
  
// Filtering Each Field except IDs
		public void ViewAll()
        {
			vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";




			vloader.ClearNavigationExpression();
			_TODO_LICToCreate.Refresh();
			NotifyPropertyChanged(x => this.TODO_LICToCreate);
		}

		public async Task SelectAll()
        {
            IEnumerable<TODO_LICToCreate> lst = null;
            using (var ctx = new TODO_LICToCreateRepository())
            {
                lst = await ctx.GetTODO_LICToCreateByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
            SelectedTODO_LICToCreate = new ObservableCollection<TODO_LICToCreate>(lst);
        }

 

		private string _country_of_origin_codeFilter;
        public string Country_of_origin_codeFilter
        {
            get
            {
                return _country_of_origin_codeFilter;
            }
            set
            {
                _country_of_origin_codeFilter = value;
				NotifyPropertyChanged(x => Country_of_origin_codeFilter);
                FilterData();
                
            }
        }	

 

		private string _currency_CodeFilter;
        public string Currency_CodeFilter
        {
            get
            {
                return _currency_CodeFilter;
            }
            set
            {
                _currency_CodeFilter = value;
				NotifyPropertyChanged(x => Currency_CodeFilter);
                FilterData();
                
            }
        }	

 

		private string _manifest_NumberFilter;
        public string Manifest_NumberFilter
        {
            get
            {
                return _manifest_NumberFilter;
            }
            set
            {
                _manifest_NumberFilter = value;
				NotifyPropertyChanged(x => Manifest_NumberFilter);
                FilterData();
                
            }
        }	

 

		private string _bLNumberFilter;
        public string BLNumberFilter
        {
            get
            {
                return _bLNumberFilter;
            }
            set
            {
                _bLNumberFilter = value;
				NotifyPropertyChanged(x => BLNumberFilter);
                FilterData();
                
            }
        }	

 

		private string _type_of_declarationFilter;
        public string Type_of_declarationFilter
        {
            get
            {
                return _type_of_declarationFilter;
            }
            set
            {
                _type_of_declarationFilter = value;
				NotifyPropertyChanged(x => Type_of_declarationFilter);
                FilterData();
                
            }
        }	

 

		private string _declaration_gen_procedure_codeFilter;
        public string Declaration_gen_procedure_codeFilter
        {
            get
            {
                return _declaration_gen_procedure_codeFilter;
            }
            set
            {
                _declaration_gen_procedure_codeFilter = value;
				NotifyPropertyChanged(x => Declaration_gen_procedure_codeFilter);
                FilterData();
                
            }
        }	

 

		private string _declarant_Reference_NumberFilter;
        public string Declarant_Reference_NumberFilter
        {
            get
            {
                return _declarant_Reference_NumberFilter;
            }
            set
            {
                _declarant_Reference_NumberFilter = value;
				NotifyPropertyChanged(x => Declarant_Reference_NumberFilter);
                FilterData();
                
            }
        }	

 

		private Int32? _totalInvoicesFilter;
        public Int32? TotalInvoicesFilter
        {
            get
            {
                return _totalInvoicesFilter;
            }
            set
            {
                _totalInvoicesFilter = value;
				NotifyPropertyChanged(x => TotalInvoicesFilter);
                FilterData();
                
            }
        }	

 

		private Int32? _documentsCountFilter;
        public Int32? DocumentsCountFilter
        {
            get
            {
                return _documentsCountFilter;
            }
            set
            {
                _documentsCountFilter = value;
				NotifyPropertyChanged(x => DocumentsCountFilter);
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

 

		private Int32? _licenseLinesFilter;
        public Int32? LicenseLinesFilter
        {
            get
            {
                return _licenseLinesFilter;
            }
            set
            {
                _licenseLinesFilter = value;
				NotifyPropertyChanged(x => LicenseLinesFilter);
                FilterData();
                
            }
        }	

 

		private Double? _totalCIFFilter;
        public Double? TotalCIFFilter
        {
            get
            {
                return _totalCIFFilter;
            }
            set
            {
                _totalCIFFilter = value;
				NotifyPropertyChanged(x => TotalCIFFilter);
                FilterData();
                
            }
        }	

 

		private Int32? _qtyLicensesRequiredFilter;
        public Int32? QtyLicensesRequiredFilter
        {
            get
            {
                return _qtyLicensesRequiredFilter;
            }
            set
            {
                _qtyLicensesRequiredFilter = value;
				NotifyPropertyChanged(x => QtyLicensesRequiredFilter);
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

			TODO_LICToCreate.Refresh();
			NotifyPropertyChanged(x => this.TODO_LICToCreate);
		}		  



		internal virtual StringBuilder GetAutoPropertyFilterString()
		{
		var res = new StringBuilder();
 

									if(string.IsNullOrEmpty(Country_of_origin_codeFilter) == false)
						res.Append(" && " + string.Format("Country_of_origin_code.Contains(\"{0}\")",  Country_of_origin_codeFilter));						
 

									if(string.IsNullOrEmpty(Currency_CodeFilter) == false)
						res.Append(" && " + string.Format("Currency_Code.Contains(\"{0}\")",  Currency_CodeFilter));						
 

									if(string.IsNullOrEmpty(Manifest_NumberFilter) == false)
						res.Append(" && " + string.Format("Manifest_Number.Contains(\"{0}\")",  Manifest_NumberFilter));						
 

									if(string.IsNullOrEmpty(BLNumberFilter) == false)
						res.Append(" && " + string.Format("BLNumber.Contains(\"{0}\")",  BLNumberFilter));						
 

									if(string.IsNullOrEmpty(Type_of_declarationFilter) == false)
						res.Append(" && " + string.Format("Type_of_declaration.Contains(\"{0}\")",  Type_of_declarationFilter));						
 

									if(string.IsNullOrEmpty(Declaration_gen_procedure_codeFilter) == false)
						res.Append(" && " + string.Format("Declaration_gen_procedure_code.Contains(\"{0}\")",  Declaration_gen_procedure_codeFilter));						
 

									if(string.IsNullOrEmpty(Declarant_Reference_NumberFilter) == false)
						res.Append(" && " + string.Format("Declarant_Reference_Number.Contains(\"{0}\")",  Declarant_Reference_NumberFilter));						
 

					if(TotalInvoicesFilter.HasValue)
						res.Append(" && " + string.Format("TotalInvoices == {0}",  TotalInvoicesFilter.ToString()));				 

					if(DocumentsCountFilter.HasValue)
						res.Append(" && " + string.Format("DocumentsCount == {0}",  DocumentsCountFilter.ToString()));				 

					if(InvoiceTotalFilter.HasValue)
						res.Append(" && " + string.Format("InvoiceTotal == {0}",  InvoiceTotalFilter.ToString()));				 

					if(LicenseLinesFilter.HasValue)
						res.Append(" && " + string.Format("LicenseLines == {0}",  LicenseLinesFilter.ToString()));				 

					if(TotalCIFFilter.HasValue)
						res.Append(" && " + string.Format("TotalCIF == {0}",  TotalCIFFilter.ToString()));				 

					if(QtyLicensesRequiredFilter.HasValue)
						res.Append(" && " + string.Format("QtyLicensesRequired == {0}",  QtyLicensesRequiredFilter.ToString()));							return res.ToString().StartsWith(" &&") || res.Length == 0 ? res:  res.Insert(0," && ");		
		}

// Send to Excel Implementation


        public async Task Send2Excel()
        {
			IEnumerable<TODO_LICToCreate> lst = null;
            using (var ctx = new TODO_LICToCreateRepository())
            {
                lst = await ctx.GetTODO_LICToCreateByExpressionNav(vloader.FilterExpression, vloader.NavigationExpression).ConfigureAwait(continueOnCapturedContext: false);
            }
             if (lst == null || !lst.Any()) 
              {
                   MessageBox.Show("No Data to Send to Excel");
                   return;
               }
            var s = new ExportToCSV<TODO_LICToCreateExcelLine, List<TODO_LICToCreateExcelLine>>
            {
                dataToPrint = lst.Select(x => new TODO_LICToCreateExcelLine
                {
 
                    Country_of_origin_code = x.Country_of_origin_code ,
                    
 
                    Currency_Code = x.Currency_Code ,
                    
 
                    Manifest_Number = x.Manifest_Number ,
                    
 
                    BLNumber = x.BLNumber ,
                    
 
                    Type_of_declaration = x.Type_of_declaration ,
                    
 
                    Declaration_gen_procedure_code = x.Declaration_gen_procedure_code ,
                    
 
                    Declarant_Reference_Number = x.Declarant_Reference_Number ,
                    
 
                    TotalInvoices = x.TotalInvoices ,
                    
 
                    DocumentsCount = x.DocumentsCount ,
                    
 
                    InvoiceTotal = x.InvoiceTotal ,
                    
 
                    LicenseLines = x.LicenseLines ,
                    
 
                    TotalCIF = x.TotalCIF ,
                    
 
                    QtyLicensesRequired = x.QtyLicensesRequired 
                    
                }).ToList()
            };
            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {
               await Task.Factory.StartNew(s.GenerateReport, CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
        }

        public partial class TODO_LICToCreateExcelLine
        {
		 
                    public string Country_of_origin_code { get; set; } 
                    
 
                    public string Currency_Code { get; set; } 
                    
 
                    public string Manifest_Number { get; set; } 
                    
 
                    public string BLNumber { get; set; } 
                    
 
                    public string Type_of_declaration { get; set; } 
                    
 
                    public string Declaration_gen_procedure_code { get; set; } 
                    
 
                    public string Declarant_Reference_Number { get; set; } 
                    
 
                    public Nullable<int> TotalInvoices { get; set; } 
                    
 
                    public Nullable<int> DocumentsCount { get; set; } 
                    
 
                    public Nullable<double> InvoiceTotal { get; set; } 
                    
 
                    public Nullable<int> LicenseLines { get; set; } 
                    
 
                    public Nullable<double> TotalCIF { get; set; } 
                    
 
                    public Nullable<int> QtyLicensesRequired { get; set; } 
                    
        }

		
    }
}
		
