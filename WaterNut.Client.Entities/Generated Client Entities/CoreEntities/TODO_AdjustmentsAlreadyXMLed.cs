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
       public partial class TODO_AdjustmentsAlreadyXMLed: BaseEntity<TODO_AdjustmentsAlreadyXMLed>
    {
        DTO.TODO_AdjustmentsAlreadyXMLed todo_adjustmentsalreadyxmled;
        public TODO_AdjustmentsAlreadyXMLed(DTO.TODO_AdjustmentsAlreadyXMLed dto )
        {
              todo_adjustmentsalreadyxmled = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.TODO_AdjustmentsAlreadyXMLed>(todo_adjustmentsalreadyxmled);

        }

        public DTO.TODO_AdjustmentsAlreadyXMLed DTO
        {
            get
            {
             return todo_adjustmentsalreadyxmled;
            }
            set
            {
                todo_adjustmentsalreadyxmled = value;
            }
        }
       [RequiredValidationAttribute(ErrorMessage= "EntryDataDetails is required")]
       
public int EntryDataDetailsId
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.EntryDataDetailsId; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.EntryDataDetailsId) return;
				this.todo_adjustmentsalreadyxmled.EntryDataDetailsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EntryDataDetailsId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ApplicationSettings is required")]
       
public int ApplicationSettingsId
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.ApplicationSettingsId; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.ApplicationSettingsId) return;
				this.todo_adjustmentsalreadyxmled.ApplicationSettingsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ApplicationSettingsId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "AsycudaDocumentSet is required")]
       
public int AsycudaDocumentSetId
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.AsycudaDocumentSetId; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.AsycudaDocumentSetId) return;
				this.todo_adjustmentsalreadyxmled.AsycudaDocumentSetId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("AsycudaDocumentSetId");
			}
		}
     

       
       
public Nullable<bool> IsClassified
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.IsClassified; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.IsClassified) return;
				this.todo_adjustmentsalreadyxmled.IsClassified = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("IsClassified");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "AdjustmentType has a max length of 50 letters ")]
public string AdjustmentType
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.AdjustmentType; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.AdjustmentType) return;
				this.todo_adjustmentsalreadyxmled.AdjustmentType = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("AdjustmentType");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "InvoiceNo is required")]
       
                
                [MaxLength(50, ErrorMessage = "InvoiceNo has a max length of 50 letters ")]
public string InvoiceNo
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.InvoiceNo; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.InvoiceNo) return;
				this.todo_adjustmentsalreadyxmled.InvoiceNo = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("InvoiceNo");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> InvoiceQty
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.InvoiceQty; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.InvoiceQty) return;
				this.todo_adjustmentsalreadyxmled.InvoiceQty = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("InvoiceQty");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> ReceivedQty
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.ReceivedQty; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.ReceivedQty) return;
				this.todo_adjustmentsalreadyxmled.ReceivedQty = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ReceivedQty");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "InvoiceDate is required")]
       
public System.DateTime InvoiceDate
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.InvoiceDate; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.InvoiceDate) return;
				this.todo_adjustmentsalreadyxmled.InvoiceDate = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("InvoiceDate");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ItemNumber is required")]
       
                
                [MaxLength(20, ErrorMessage = "ItemNumber has a max length of 20 letters ")]
public string ItemNumber
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.ItemNumber; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.ItemNumber) return;
				this.todo_adjustmentsalreadyxmled.ItemNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ItemNumber");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "Status has a max length of 50 letters ")]
public string Status
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.Status; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.Status) return;
				this.todo_adjustmentsalreadyxmled.Status = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Status");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "Declarant_Reference_Number has a max length of 50 letters ")]
public string Declarant_Reference_Number
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.Declarant_Reference_Number; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.Declarant_Reference_Number) return;
				this.todo_adjustmentsalreadyxmled.Declarant_Reference_Number = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Declarant_Reference_Number");
			}
		}
     

       
       
                
                [MaxLength(20, ErrorMessage = "pCNumber has a max length of 20 letters ")]
public string pCNumber
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.pCNumber; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.pCNumber) return;
				this.todo_adjustmentsalreadyxmled.pCNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("pCNumber");
			}
		}
     

       
       
public Nullable<System.DateTime> RegistrationDate
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.RegistrationDate; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.RegistrationDate) return;
				this.todo_adjustmentsalreadyxmled.RegistrationDate = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("RegistrationDate");
			}
		}
     

       
       
                
                [MaxLength(30, ErrorMessage = "ReferenceNumber has a max length of 30 letters ")]
public string ReferenceNumber
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.ReferenceNumber; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.ReferenceNumber) return;
				this.todo_adjustmentsalreadyxmled.ReferenceNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ReferenceNumber");
			}
		}
     

       
       
                
                [MaxLength(40, ErrorMessage = "DocumentType has a max length of 40 letters ")]
public string DocumentType
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.DocumentType; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.DocumentType) return;
				this.todo_adjustmentsalreadyxmled.DocumentType = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("DocumentType");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "EntryData_ is required")]
       
public int EntryData_Id
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.EntryData_Id; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.EntryData_Id) return;
				this.todo_adjustmentsalreadyxmled.EntryData_Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EntryData_Id");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "PreviousCNumber has a max length of 50 letters ")]
public string PreviousCNumber
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.PreviousCNumber; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.PreviousCNumber) return;
				this.todo_adjustmentsalreadyxmled.PreviousCNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("PreviousCNumber");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "Type has a max length of 50 letters ")]
public string Type
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.Type; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.Type) return;
				this.todo_adjustmentsalreadyxmled.Type = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Type");
			}
		}
     

       
       
public Nullable<System.DateTime> EffectiveDate
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.EffectiveDate; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.EffectiveDate) return;
				this.todo_adjustmentsalreadyxmled.EffectiveDate = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EffectiveDate");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ItemDescription is required")]
       
                
                [MaxLength(255, ErrorMessage = "ItemDescription has a max length of 255 letters ")]
public string ItemDescription
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.ItemDescription; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.ItemDescription) return;
				this.todo_adjustmentsalreadyxmled.ItemDescription = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ItemDescription");
			}
		}
     

       
       
public Nullable<int> EmailId
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.EmailId; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.EmailId) return;
				this.todo_adjustmentsalreadyxmled.EmailId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EmailId");
			}
		}
     

       
       
public Nullable<int> FileTypeId
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.FileTypeId; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.FileTypeId) return;
				this.todo_adjustmentsalreadyxmled.FileTypeId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("FileTypeId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Quantity is required")]
       [NumberValidationAttribute]
public double Quantity
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.Quantity; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.Quantity) return;
				this.todo_adjustmentsalreadyxmled.Quantity = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Quantity");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Cost is required")]
       [NumberValidationAttribute]
public double Cost
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.Cost; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.Cost) return;
				this.todo_adjustmentsalreadyxmled.Cost = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Cost");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Subject is required")]
       
                
                
public string Subject
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.Subject; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.Subject) return;
				this.todo_adjustmentsalreadyxmled.Subject = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Subject");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "EmailDate is required")]
       
public System.DateTime EmailDate
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.EmailDate; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.EmailDate) return;
				this.todo_adjustmentsalreadyxmled.EmailDate = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EmailDate");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<int> LineNumber
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.LineNumber; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.LineNumber) return;
				this.todo_adjustmentsalreadyxmled.LineNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("LineNumber");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "PreviousInvoiceNumber has a max length of 50 letters ")]
public string PreviousInvoiceNumber
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.PreviousInvoiceNumber; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.PreviousInvoiceNumber) return;
				this.todo_adjustmentsalreadyxmled.PreviousInvoiceNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("PreviousInvoiceNumber");
			}
		}
     

       
       
                
                [MaxLength(255, ErrorMessage = "Comment has a max length of 255 letters ")]
public string Comment
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.Comment; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.Comment) return;
				this.todo_adjustmentsalreadyxmled.Comment = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Comment");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "DutyFreePaid is required")]
       
                
                [MaxLength(9, ErrorMessage = "DutyFreePaid has a max length of 9 letters ")]
public string DutyFreePaid
		{ 
		    get { return this.todo_adjustmentsalreadyxmled.DutyFreePaid; }
			set
			{
			    if (value == this.todo_adjustmentsalreadyxmled.DutyFreePaid) return;
				this.todo_adjustmentsalreadyxmled.DutyFreePaid = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("DutyFreePaid");
			}
		}
     


        ChangeTrackingCollection<DTO.TODO_AdjustmentsAlreadyXMLed> _changeTracker;    
        public ChangeTrackingCollection<DTO.TODO_AdjustmentsAlreadyXMLed> ChangeTracker
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

