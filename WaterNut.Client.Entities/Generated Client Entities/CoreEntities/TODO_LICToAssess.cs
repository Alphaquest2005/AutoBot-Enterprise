﻿// <autogenerated>
//   This file was generated by T4 code generator AllClientEntities.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using TrackableEntities.Client;
using Core.Common.Client.Entities;
using CoreEntities.Client.DTO;


using Core.Common.Validation;

namespace CoreEntities.Client.Entities
{
       public partial class TODO_LICToAssess: BaseEntity<TODO_LICToAssess>
    {
        DTO.TODO_LICToAssess todo_lictoassess;
        public TODO_LICToAssess(DTO.TODO_LICToAssess dto )
        {
              todo_lictoassess = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.TODO_LICToAssess>(todo_lictoassess);

        }

        public DTO.TODO_LICToAssess DTO
        {
            get
            {
             return todo_lictoassess;
            }
            set
            {
                todo_lictoassess = value;
            }
        }
       [RequiredValidationAttribute(ErrorMessage= "AsycudaDocumentSet is required")]
       
public int AsycudaDocumentSetId
		{ 
		    get { return this.todo_lictoassess.AsycudaDocumentSetId; }
			set
			{
			    if (value == this.todo_lictoassess.AsycudaDocumentSetId) return;
				this.todo_lictoassess.AsycudaDocumentSetId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("AsycudaDocumentSetId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ApplicationSettings is required")]
       
public int ApplicationSettingsId
		{ 
		    get { return this.todo_lictoassess.ApplicationSettingsId; }
			set
			{
			    if (value == this.todo_lictoassess.ApplicationSettingsId) return;
				this.todo_lictoassess.ApplicationSettingsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ApplicationSettingsId");
			}
		}
     

       
       
                
                [MaxLength(3, ErrorMessage = "Country_of_origin_code has a max length of 3 letters ")]
public string Country_of_origin_code
		{ 
		    get { return this.todo_lictoassess.Country_of_origin_code; }
			set
			{
			    if (value == this.todo_lictoassess.Country_of_origin_code) return;
				this.todo_lictoassess.Country_of_origin_code = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Country_of_origin_code");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Currency_Code is required")]
       
                
                [MaxLength(3, ErrorMessage = "Currency_Code has a max length of 3 letters ")]
public string Currency_Code
		{ 
		    get { return this.todo_lictoassess.Currency_Code; }
			set
			{
			    if (value == this.todo_lictoassess.Currency_Code) return;
				this.todo_lictoassess.Currency_Code = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Currency_Code");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "Manifest_Number has a max length of 50 letters ")]
public string Manifest_Number
		{ 
		    get { return this.todo_lictoassess.Manifest_Number; }
			set
			{
			    if (value == this.todo_lictoassess.Manifest_Number) return;
				this.todo_lictoassess.Manifest_Number = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Manifest_Number");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "BLNumber has a max length of 50 letters ")]
public string BLNumber
		{ 
		    get { return this.todo_lictoassess.BLNumber; }
			set
			{
			    if (value == this.todo_lictoassess.BLNumber) return;
				this.todo_lictoassess.BLNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("BLNumber");
			}
		}
     

       
       
                
                [MaxLength(10, ErrorMessage = "Type_of_declaration has a max length of 10 letters ")]
public string Type_of_declaration
		{ 
		    get { return this.todo_lictoassess.Type_of_declaration; }
			set
			{
			    if (value == this.todo_lictoassess.Type_of_declaration) return;
				this.todo_lictoassess.Type_of_declaration = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Type_of_declaration");
			}
		}
     

       
       
                
                [MaxLength(10, ErrorMessage = "Declaration_gen_procedure_code has a max length of 10 letters ")]
public string Declaration_gen_procedure_code
		{ 
		    get { return this.todo_lictoassess.Declaration_gen_procedure_code; }
			set
			{
			    if (value == this.todo_lictoassess.Declaration_gen_procedure_code) return;
				this.todo_lictoassess.Declaration_gen_procedure_code = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Declaration_gen_procedure_code");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "Declarant_Reference_Number has a max length of 50 letters ")]
public string Declarant_Reference_Number
		{ 
		    get { return this.todo_lictoassess.Declarant_Reference_Number; }
			set
			{
			    if (value == this.todo_lictoassess.Declarant_Reference_Number) return;
				this.todo_lictoassess.Declarant_Reference_Number = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Declarant_Reference_Number");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<int> TotalInvoices
		{ 
		    get { return this.todo_lictoassess.TotalInvoices; }
			set
			{
			    if (value == this.todo_lictoassess.TotalInvoices) return;
				this.todo_lictoassess.TotalInvoices = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("TotalInvoices");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<int> DocumentsCount
		{ 
		    get { return this.todo_lictoassess.DocumentsCount; }
			set
			{
			    if (value == this.todo_lictoassess.DocumentsCount) return;
				this.todo_lictoassess.DocumentsCount = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("DocumentsCount");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> InvoiceTotal
		{ 
		    get { return this.todo_lictoassess.InvoiceTotal; }
			set
			{
			    if (value == this.todo_lictoassess.InvoiceTotal) return;
				this.todo_lictoassess.InvoiceTotal = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("InvoiceTotal");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<int> LicenseLines
		{ 
		    get { return this.todo_lictoassess.LicenseLines; }
			set
			{
			    if (value == this.todo_lictoassess.LicenseLines) return;
				this.todo_lictoassess.LicenseLines = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("LicenseLines");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> TotalCIF
		{ 
		    get { return this.todo_lictoassess.TotalCIF; }
			set
			{
			    if (value == this.todo_lictoassess.TotalCIF) return;
				this.todo_lictoassess.TotalCIF = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("TotalCIF");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<int> QtyLicensesRequired
		{ 
		    get { return this.todo_lictoassess.QtyLicensesRequired; }
			set
			{
			    if (value == this.todo_lictoassess.QtyLicensesRequired) return;
				this.todo_lictoassess.QtyLicensesRequired = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("QtyLicensesRequired");
			}
		}
     


        ChangeTrackingCollection<DTO.TODO_LICToAssess> _changeTracker;    
        public ChangeTrackingCollection<DTO.TODO_LICToAssess> ChangeTracker
        {
            get
            {
                return _changeTracker;
            }
        }

        public new TrackableEntities.TrackingState TrackingState
        {
            get
            {
                return this.DTO.TrackingState;
            }
            set
            {
                this.DTO.TrackingState = value;
                NotifyPropertyChanged("TrackingState");
            }
        }

    }
}

