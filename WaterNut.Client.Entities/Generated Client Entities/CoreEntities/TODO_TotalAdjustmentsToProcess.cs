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
       public partial class TODO_TotalAdjustmentsToProcess: BaseEntity<TODO_TotalAdjustmentsToProcess>
    {
        DTO.TODO_TotalAdjustmentsToProcess todo_totaladjustmentstoprocess;
        public TODO_TotalAdjustmentsToProcess(DTO.TODO_TotalAdjustmentsToProcess dto )
        {
              todo_totaladjustmentstoprocess = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.TODO_TotalAdjustmentsToProcess>(todo_totaladjustmentstoprocess);

        }

        public DTO.TODO_TotalAdjustmentsToProcess DTO
        {
            get
            {
             return todo_totaladjustmentstoprocess;
            }
            set
            {
                todo_totaladjustmentstoprocess = value;
            }
        }
       [RequiredValidationAttribute(ErrorMessage= "EntryDataDetails is required")]
       
public int EntryDataDetailsId
		{ 
		    get { return this.todo_totaladjustmentstoprocess.EntryDataDetailsId; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.EntryDataDetailsId) return;
				this.todo_totaladjustmentstoprocess.EntryDataDetailsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EntryDataDetailsId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "EntryData is required")]
       
                
                [MaxLength(50, ErrorMessage = "EntryDataId has a max length of 50 letters ")]
public string EntryDataId
		{ 
		    get { return this.todo_totaladjustmentstoprocess.EntryDataId; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.EntryDataId) return;
				this.todo_totaladjustmentstoprocess.EntryDataId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EntryDataId");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<int> LineNumber
		{ 
		    get { return this.todo_totaladjustmentstoprocess.LineNumber; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.LineNumber) return;
				this.todo_totaladjustmentstoprocess.LineNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("LineNumber");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ItemNumber is required")]
       
                
                [MaxLength(20, ErrorMessage = "ItemNumber has a max length of 20 letters ")]
public string ItemNumber
		{ 
		    get { return this.todo_totaladjustmentstoprocess.ItemNumber; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.ItemNumber) return;
				this.todo_totaladjustmentstoprocess.ItemNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ItemNumber");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Quantity is required")]
       [NumberValidationAttribute]
public double Quantity
		{ 
		    get { return this.todo_totaladjustmentstoprocess.Quantity; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.Quantity) return;
				this.todo_totaladjustmentstoprocess.Quantity = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Quantity");
			}
		}
     

       
       
                
                [MaxLength(15, ErrorMessage = "Units has a max length of 15 letters ")]
public string Units
		{ 
		    get { return this.todo_totaladjustmentstoprocess.Units; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.Units) return;
				this.todo_totaladjustmentstoprocess.Units = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Units");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ItemDescription is required")]
       
                
                [MaxLength(255, ErrorMessage = "ItemDescription has a max length of 255 letters ")]
public string ItemDescription
		{ 
		    get { return this.todo_totaladjustmentstoprocess.ItemDescription; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.ItemDescription) return;
				this.todo_totaladjustmentstoprocess.ItemDescription = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ItemDescription");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Cost is required")]
       [NumberValidationAttribute]
public double Cost
		{ 
		    get { return this.todo_totaladjustmentstoprocess.Cost; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.Cost) return;
				this.todo_totaladjustmentstoprocess.Cost = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Cost");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "QtyAllocated is required")]
       [NumberValidationAttribute]
public double QtyAllocated
		{ 
		    get { return this.todo_totaladjustmentstoprocess.QtyAllocated; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.QtyAllocated) return;
				this.todo_totaladjustmentstoprocess.QtyAllocated = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("QtyAllocated");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "UnitWeight is required")]
       [NumberValidationAttribute]
public double UnitWeight
		{ 
		    get { return this.todo_totaladjustmentstoprocess.UnitWeight; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.UnitWeight) return;
				this.todo_totaladjustmentstoprocess.UnitWeight = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("UnitWeight");
			}
		}
     

       
       
public Nullable<bool> DoNotAllocate
		{ 
		    get { return this.todo_totaladjustmentstoprocess.DoNotAllocate; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.DoNotAllocate) return;
				this.todo_totaladjustmentstoprocess.DoNotAllocate = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("DoNotAllocate");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "TariffCode has a max length of 50 letters ")]
public string TariffCode
		{ 
		    get { return this.todo_totaladjustmentstoprocess.TariffCode; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.TariffCode) return;
				this.todo_totaladjustmentstoprocess.TariffCode = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("TariffCode");
			}
		}
     

       
       
                
                [MaxLength(20, ErrorMessage = "CNumber has a max length of 20 letters ")]
public string CNumber
		{ 
		    get { return this.todo_totaladjustmentstoprocess.CNumber; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.CNumber) return;
				this.todo_totaladjustmentstoprocess.CNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("CNumber");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<int> CLineNumber
		{ 
		    get { return this.todo_totaladjustmentstoprocess.CLineNumber; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.CLineNumber) return;
				this.todo_totaladjustmentstoprocess.CLineNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("CLineNumber");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "AsycudaDocumentSet is required")]
       
public int AsycudaDocumentSetId
		{ 
		    get { return this.todo_totaladjustmentstoprocess.AsycudaDocumentSetId; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.AsycudaDocumentSetId) return;
				this.todo_totaladjustmentstoprocess.AsycudaDocumentSetId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("AsycudaDocumentSetId");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> InvoiceQty
		{ 
		    get { return this.todo_totaladjustmentstoprocess.InvoiceQty; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.InvoiceQty) return;
				this.todo_totaladjustmentstoprocess.InvoiceQty = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("InvoiceQty");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> ReceivedQty
		{ 
		    get { return this.todo_totaladjustmentstoprocess.ReceivedQty; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.ReceivedQty) return;
				this.todo_totaladjustmentstoprocess.ReceivedQty = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ReceivedQty");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "PreviousInvoiceNumber has a max length of 50 letters ")]
public string PreviousInvoiceNumber
		{ 
		    get { return this.todo_totaladjustmentstoprocess.PreviousInvoiceNumber; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.PreviousInvoiceNumber) return;
				this.todo_totaladjustmentstoprocess.PreviousInvoiceNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("PreviousInvoiceNumber");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "PreviousCNumber has a max length of 50 letters ")]
public string PreviousCNumber
		{ 
		    get { return this.todo_totaladjustmentstoprocess.PreviousCNumber; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.PreviousCNumber) return;
				this.todo_totaladjustmentstoprocess.PreviousCNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("PreviousCNumber");
			}
		}
     

       
       
                
                [MaxLength(255, ErrorMessage = "Comment has a max length of 255 letters ")]
public string Comment
		{ 
		    get { return this.todo_totaladjustmentstoprocess.Comment; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.Comment) return;
				this.todo_totaladjustmentstoprocess.Comment = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Comment");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "Status has a max length of 50 letters ")]
public string Status
		{ 
		    get { return this.todo_totaladjustmentstoprocess.Status; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.Status) return;
				this.todo_totaladjustmentstoprocess.Status = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Status");
			}
		}
     

       
       
public Nullable<System.DateTime> EffectiveDate
		{ 
		    get { return this.todo_totaladjustmentstoprocess.EffectiveDate; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.EffectiveDate) return;
				this.todo_totaladjustmentstoprocess.EffectiveDate = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EffectiveDate");
			}
		}
     

       
       
                
                [MaxLength(4, ErrorMessage = "Currency has a max length of 4 letters ")]
public string Currency
		{ 
		    get { return this.todo_totaladjustmentstoprocess.Currency; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.Currency) return;
				this.todo_totaladjustmentstoprocess.Currency = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Currency");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ApplicationSettings is required")]
       
public int ApplicationSettingsId
		{ 
		    get { return this.todo_totaladjustmentstoprocess.ApplicationSettingsId; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.ApplicationSettingsId) return;
				this.todo_totaladjustmentstoprocess.ApplicationSettingsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ApplicationSettingsId");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "Type has a max length of 50 letters ")]
public string Type
		{ 
		    get { return this.todo_totaladjustmentstoprocess.Type; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.Type) return;
				this.todo_totaladjustmentstoprocess.Type = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Type");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "Declarant_Reference_Number has a max length of 50 letters ")]
public string Declarant_Reference_Number
		{ 
		    get { return this.todo_totaladjustmentstoprocess.Declarant_Reference_Number; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.Declarant_Reference_Number) return;
				this.todo_totaladjustmentstoprocess.Declarant_Reference_Number = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Declarant_Reference_Number");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "InvoiceDate is required")]
       
public System.DateTime InvoiceDate
		{ 
		    get { return this.todo_totaladjustmentstoprocess.InvoiceDate; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.InvoiceDate) return;
				this.todo_totaladjustmentstoprocess.InvoiceDate = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("InvoiceDate");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Subject is required")]
       
                
                
public string Subject
		{ 
		    get { return this.todo_totaladjustmentstoprocess.Subject; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.Subject) return;
				this.todo_totaladjustmentstoprocess.Subject = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Subject");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "EmailDate is required")]
       
public System.DateTime EmailDate
		{ 
		    get { return this.todo_totaladjustmentstoprocess.EmailDate; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.EmailDate) return;
				this.todo_totaladjustmentstoprocess.EmailDate = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EmailDate");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "DutyFreePaid is required")]
       
                
                [MaxLength(9, ErrorMessage = "DutyFreePaid has a max length of 9 letters ")]
public string DutyFreePaid
		{ 
		    get { return this.todo_totaladjustmentstoprocess.DutyFreePaid; }
			set
			{
			    if (value == this.todo_totaladjustmentstoprocess.DutyFreePaid) return;
				this.todo_totaladjustmentstoprocess.DutyFreePaid = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("DutyFreePaid");
			}
		}
     


        ChangeTrackingCollection<DTO.TODO_TotalAdjustmentsToProcess> _changeTracker;    
        public ChangeTrackingCollection<DTO.TODO_TotalAdjustmentsToProcess> ChangeTracker
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

