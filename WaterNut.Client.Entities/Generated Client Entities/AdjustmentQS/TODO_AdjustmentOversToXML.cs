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
using AdjustmentQS.Client.DTO;


using Core.Common.Validation;

namespace AdjustmentQS.Client.Entities
{
       public partial class TODO_AdjustmentOversToXML: BaseEntity<TODO_AdjustmentOversToXML>
    {
        DTO.TODO_AdjustmentOversToXML todo_adjustmentoverstoxml;
        public TODO_AdjustmentOversToXML(DTO.TODO_AdjustmentOversToXML dto )
        {
              todo_adjustmentoverstoxml = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.TODO_AdjustmentOversToXML>(todo_adjustmentoverstoxml);

        }

        public DTO.TODO_AdjustmentOversToXML DTO
        {
            get
            {
             return todo_adjustmentoverstoxml;
            }
            set
            {
                todo_adjustmentoverstoxml = value;
            }
        }
       [RequiredValidationAttribute(ErrorMessage= "EntryDataDetails is required")]
       
public int EntryDataDetailsId
		{ 
		    get { return this.todo_adjustmentoverstoxml.EntryDataDetailsId; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.EntryDataDetailsId) return;
				this.todo_adjustmentoverstoxml.EntryDataDetailsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EntryDataDetailsId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "EntryData_ is required")]
       
public int EntryData_Id
		{ 
		    get { return this.todo_adjustmentoverstoxml.EntryData_Id; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.EntryData_Id) return;
				this.todo_adjustmentoverstoxml.EntryData_Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EntryData_Id");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "EntryDataId has a max length of 50 letters ")]
public string EntryDataId
		{ 
		    get { return this.todo_adjustmentoverstoxml.EntryDataId; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.EntryDataId) return;
				this.todo_adjustmentoverstoxml.EntryDataId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EntryDataId");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<int> LineNumber
		{ 
		    get { return this.todo_adjustmentoverstoxml.LineNumber; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.LineNumber) return;
				this.todo_adjustmentoverstoxml.LineNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("LineNumber");
			}
		}
     

       
       
                
                [MaxLength(20, ErrorMessage = "ItemNumber has a max length of 20 letters ")]
public string ItemNumber
		{ 
		    get { return this.todo_adjustmentoverstoxml.ItemNumber; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.ItemNumber) return;
				this.todo_adjustmentoverstoxml.ItemNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ItemNumber");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> Quantity
		{ 
		    get { return this.todo_adjustmentoverstoxml.Quantity; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.Quantity) return;
				this.todo_adjustmentoverstoxml.Quantity = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Quantity");
			}
		}
     

       
       
                
                [MaxLength(15, ErrorMessage = "Units has a max length of 15 letters ")]
public string Units
		{ 
		    get { return this.todo_adjustmentoverstoxml.Units; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.Units) return;
				this.todo_adjustmentoverstoxml.Units = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Units");
			}
		}
     

       
       
                
                [MaxLength(255, ErrorMessage = "ItemDescription has a max length of 255 letters ")]
public string ItemDescription
		{ 
		    get { return this.todo_adjustmentoverstoxml.ItemDescription; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.ItemDescription) return;
				this.todo_adjustmentoverstoxml.ItemDescription = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ItemDescription");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> Cost
		{ 
		    get { return this.todo_adjustmentoverstoxml.Cost; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.Cost) return;
				this.todo_adjustmentoverstoxml.Cost = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Cost");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> QtyAllocated
		{ 
		    get { return this.todo_adjustmentoverstoxml.QtyAllocated; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.QtyAllocated) return;
				this.todo_adjustmentoverstoxml.QtyAllocated = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("QtyAllocated");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> UnitWeight
		{ 
		    get { return this.todo_adjustmentoverstoxml.UnitWeight; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.UnitWeight) return;
				this.todo_adjustmentoverstoxml.UnitWeight = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("UnitWeight");
			}
		}
     

       
       
public Nullable<bool> DoNotAllocate
		{ 
		    get { return this.todo_adjustmentoverstoxml.DoNotAllocate; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.DoNotAllocate) return;
				this.todo_adjustmentoverstoxml.DoNotAllocate = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("DoNotAllocate");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "TariffCode has a max length of 50 letters ")]
public string TariffCode
		{ 
		    get { return this.todo_adjustmentoverstoxml.TariffCode; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.TariffCode) return;
				this.todo_adjustmentoverstoxml.TariffCode = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("TariffCode");
			}
		}
     

       
       
                
                [MaxLength(20, ErrorMessage = "CNumber has a max length of 20 letters ")]
public string CNumber
		{ 
		    get { return this.todo_adjustmentoverstoxml.CNumber; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.CNumber) return;
				this.todo_adjustmentoverstoxml.CNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("CNumber");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<int> CLineNumber
		{ 
		    get { return this.todo_adjustmentoverstoxml.CLineNumber; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.CLineNumber) return;
				this.todo_adjustmentoverstoxml.CLineNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("CLineNumber");
			}
		}
     

       
       
public Nullable<int> AsycudaDocumentSetId
		{ 
		    get { return this.todo_adjustmentoverstoxml.AsycudaDocumentSetId; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.AsycudaDocumentSetId) return;
				this.todo_adjustmentoverstoxml.AsycudaDocumentSetId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("AsycudaDocumentSetId");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> InvoiceQty
		{ 
		    get { return this.todo_adjustmentoverstoxml.InvoiceQty; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.InvoiceQty) return;
				this.todo_adjustmentoverstoxml.InvoiceQty = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("InvoiceQty");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> ReceivedQty
		{ 
		    get { return this.todo_adjustmentoverstoxml.ReceivedQty; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.ReceivedQty) return;
				this.todo_adjustmentoverstoxml.ReceivedQty = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ReceivedQty");
			}
		}
     

       
       
                
                [MaxLength(255, ErrorMessage = "PreviousInvoiceNumber has a max length of 255 letters ")]
public string PreviousInvoiceNumber
		{ 
		    get { return this.todo_adjustmentoverstoxml.PreviousInvoiceNumber; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.PreviousInvoiceNumber) return;
				this.todo_adjustmentoverstoxml.PreviousInvoiceNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("PreviousInvoiceNumber");
			}
		}
     

       
       
                
                [MaxLength(255, ErrorMessage = "PreviousCNumber has a max length of 255 letters ")]
public string PreviousCNumber
		{ 
		    get { return this.todo_adjustmentoverstoxml.PreviousCNumber; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.PreviousCNumber) return;
				this.todo_adjustmentoverstoxml.PreviousCNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("PreviousCNumber");
			}
		}
     

       
       
                
                [MaxLength(255, ErrorMessage = "Comment has a max length of 255 letters ")]
public string Comment
		{ 
		    get { return this.todo_adjustmentoverstoxml.Comment; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.Comment) return;
				this.todo_adjustmentoverstoxml.Comment = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Comment");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "Status has a max length of 50 letters ")]
public string Status
		{ 
		    get { return this.todo_adjustmentoverstoxml.Status; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.Status) return;
				this.todo_adjustmentoverstoxml.Status = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Status");
			}
		}
     

       
       
public Nullable<System.DateTime> EffectiveDate
		{ 
		    get { return this.todo_adjustmentoverstoxml.EffectiveDate; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.EffectiveDate) return;
				this.todo_adjustmentoverstoxml.EffectiveDate = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EffectiveDate");
			}
		}
     

       
       
                
                [MaxLength(4, ErrorMessage = "Currency has a max length of 4 letters ")]
public string Currency
		{ 
		    get { return this.todo_adjustmentoverstoxml.Currency; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.Currency) return;
				this.todo_adjustmentoverstoxml.Currency = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Currency");
			}
		}
     

       
       
public Nullable<int> ApplicationSettingsId
		{ 
		    get { return this.todo_adjustmentoverstoxml.ApplicationSettingsId; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.ApplicationSettingsId) return;
				this.todo_adjustmentoverstoxml.ApplicationSettingsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ApplicationSettingsId");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "Type has a max length of 50 letters ")]
public string Type
		{ 
		    get { return this.todo_adjustmentoverstoxml.Type; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.Type) return;
				this.todo_adjustmentoverstoxml.Type = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Type");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "DutyFreePaid has a max length of 50 letters ")]
public string DutyFreePaid
		{ 
		    get { return this.todo_adjustmentoverstoxml.DutyFreePaid; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.DutyFreePaid) return;
				this.todo_adjustmentoverstoxml.DutyFreePaid = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("DutyFreePaid");
			}
		}
     

       
       
                
                [MaxLength(255, ErrorMessage = "EmailId has a max length of 255 letters ")]
public string EmailId
		{ 
		    get { return this.todo_adjustmentoverstoxml.EmailId; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.EmailId) return;
				this.todo_adjustmentoverstoxml.EmailId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EmailId");
			}
		}
     

       
       
public Nullable<int> FileTypeId
		{ 
		    get { return this.todo_adjustmentoverstoxml.FileTypeId; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.FileTypeId) return;
				this.todo_adjustmentoverstoxml.FileTypeId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("FileTypeId");
			}
		}
     

       
       
public Nullable<System.DateTime> InvoiceDate
		{ 
		    get { return this.todo_adjustmentoverstoxml.InvoiceDate; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.InvoiceDate) return;
				this.todo_adjustmentoverstoxml.InvoiceDate = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("InvoiceDate");
			}
		}
     

       
       
                
                
public string Subject
		{ 
		    get { return this.todo_adjustmentoverstoxml.Subject; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.Subject) return;
				this.todo_adjustmentoverstoxml.Subject = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Subject");
			}
		}
     

       
       
public Nullable<System.DateTime> EmailDate
		{ 
		    get { return this.todo_adjustmentoverstoxml.EmailDate; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.EmailDate) return;
				this.todo_adjustmentoverstoxml.EmailDate = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EmailDate");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "AlreadyExecuted is required")]
       [NumberValidationAttribute]
public int AlreadyExecuted
		{ 
		    get { return this.todo_adjustmentoverstoxml.AlreadyExecuted; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.AlreadyExecuted) return;
				this.todo_adjustmentoverstoxml.AlreadyExecuted = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("AlreadyExecuted");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "Vendor has a max length of 50 letters ")]
public string Vendor
		{ 
		    get { return this.todo_adjustmentoverstoxml.Vendor; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.Vendor) return;
				this.todo_adjustmentoverstoxml.Vendor = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Vendor");
			}
		}
     

       
       
                
                
public string SourceFile
		{ 
		    get { return this.todo_adjustmentoverstoxml.SourceFile; }
			set
			{
			    if (value == this.todo_adjustmentoverstoxml.SourceFile) return;
				this.todo_adjustmentoverstoxml.SourceFile = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("SourceFile");
			}
		}
     


        ChangeTrackingCollection<DTO.TODO_AdjustmentOversToXML> _changeTracker;    
        public ChangeTrackingCollection<DTO.TODO_AdjustmentOversToXML> ChangeTracker
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


