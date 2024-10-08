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
       public partial class TODO_SubmitDocSetWithIncompleteInvoices: BaseEntity<TODO_SubmitDocSetWithIncompleteInvoices>
    {
        DTO.TODO_SubmitDocSetWithIncompleteInvoices todo_submitdocsetwithincompleteinvoices;
        public TODO_SubmitDocSetWithIncompleteInvoices(DTO.TODO_SubmitDocSetWithIncompleteInvoices dto )
        {
              todo_submitdocsetwithincompleteinvoices = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.TODO_SubmitDocSetWithIncompleteInvoices>(todo_submitdocsetwithincompleteinvoices);

        }

        public DTO.TODO_SubmitDocSetWithIncompleteInvoices DTO
        {
            get
            {
             return todo_submitdocsetwithincompleteinvoices;
            }
            set
            {
                todo_submitdocsetwithincompleteinvoices = value;
            }
        }
       [RequiredValidationAttribute(ErrorMessage= "AsycudaDocumentSet is required")]
       
public int AsycudaDocumentSetId
		{ 
		    get { return this.todo_submitdocsetwithincompleteinvoices.AsycudaDocumentSetId; }
			set
			{
			    if (value == this.todo_submitdocsetwithincompleteinvoices.AsycudaDocumentSetId) return;
				this.todo_submitdocsetwithincompleteinvoices.AsycudaDocumentSetId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("AsycudaDocumentSetId");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "Declarant_Reference_Number has a max length of 50 letters ")]
public string Declarant_Reference_Number
		{ 
		    get { return this.todo_submitdocsetwithincompleteinvoices.Declarant_Reference_Number; }
			set
			{
			    if (value == this.todo_submitdocsetwithincompleteinvoices.Declarant_Reference_Number) return;
				this.todo_submitdocsetwithincompleteinvoices.Declarant_Reference_Number = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Declarant_Reference_Number");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<int> TotalInvoices
		{ 
		    get { return this.todo_submitdocsetwithincompleteinvoices.TotalInvoices; }
			set
			{
			    if (value == this.todo_submitdocsetwithincompleteinvoices.TotalInvoices) return;
				this.todo_submitdocsetwithincompleteinvoices.TotalInvoices = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("TotalInvoices");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<int> ImportedInvoices
		{ 
		    get { return this.todo_submitdocsetwithincompleteinvoices.ImportedInvoices; }
			set
			{
			    if (value == this.todo_submitdocsetwithincompleteinvoices.ImportedInvoices) return;
				this.todo_submitdocsetwithincompleteinvoices.ImportedInvoices = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ImportedInvoices");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "InvoiceDate is required")]
       
public System.DateTime InvoiceDate
		{ 
		    get { return this.todo_submitdocsetwithincompleteinvoices.InvoiceDate; }
			set
			{
			    if (value == this.todo_submitdocsetwithincompleteinvoices.InvoiceDate) return;
				this.todo_submitdocsetwithincompleteinvoices.InvoiceDate = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("InvoiceDate");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "InvoiceNo is required")]
       
                
                [MaxLength(50, ErrorMessage = "InvoiceNo has a max length of 50 letters ")]
public string InvoiceNo
		{ 
		    get { return this.todo_submitdocsetwithincompleteinvoices.InvoiceNo; }
			set
			{
			    if (value == this.todo_submitdocsetwithincompleteinvoices.InvoiceNo) return;
				this.todo_submitdocsetwithincompleteinvoices.InvoiceNo = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("InvoiceNo");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> InvoiceTotal
		{ 
		    get { return this.todo_submitdocsetwithincompleteinvoices.InvoiceTotal; }
			set
			{
			    if (value == this.todo_submitdocsetwithincompleteinvoices.InvoiceTotal) return;
				this.todo_submitdocsetwithincompleteinvoices.InvoiceTotal = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("InvoiceTotal");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ApplicationSettings is required")]
       
public int ApplicationSettingsId
		{ 
		    get { return this.todo_submitdocsetwithincompleteinvoices.ApplicationSettingsId; }
			set
			{
			    if (value == this.todo_submitdocsetwithincompleteinvoices.ApplicationSettingsId) return;
				this.todo_submitdocsetwithincompleteinvoices.ApplicationSettingsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ApplicationSettingsId");
			}
		}
     

       
       
                
                [MaxLength(255, ErrorMessage = "EmailId has a max length of 255 letters ")]
public string EmailId
		{ 
		    get { return this.todo_submitdocsetwithincompleteinvoices.EmailId; }
			set
			{
			    if (value == this.todo_submitdocsetwithincompleteinvoices.EmailId) return;
				this.todo_submitdocsetwithincompleteinvoices.EmailId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EmailId");
			}
		}
     


        ChangeTrackingCollection<DTO.TODO_SubmitDocSetWithIncompleteInvoices> _changeTracker;    
        public ChangeTrackingCollection<DTO.TODO_SubmitDocSetWithIncompleteInvoices> ChangeTracker
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


