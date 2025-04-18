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
       public partial class TODO_CreateDiscrepancyEntries: BaseEntity<TODO_CreateDiscrepancyEntries>
    {
        DTO.TODO_CreateDiscrepancyEntries todo_creatediscrepancyentries;
        public TODO_CreateDiscrepancyEntries(DTO.TODO_CreateDiscrepancyEntries dto )
        {
              todo_creatediscrepancyentries = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.TODO_CreateDiscrepancyEntries>(todo_creatediscrepancyentries);

        }

        public DTO.TODO_CreateDiscrepancyEntries DTO
        {
            get
            {
             return todo_creatediscrepancyentries;
            }
            set
            {
                todo_creatediscrepancyentries = value;
            }
        }
       [RequiredValidationAttribute(ErrorMessage= "ApplicationSettings is required")]
       
public int ApplicationSettingsId
		{ 
		    get { return this.todo_creatediscrepancyentries.ApplicationSettingsId; }
			set
			{
			    if (value == this.todo_creatediscrepancyentries.ApplicationSettingsId) return;
				this.todo_creatediscrepancyentries.ApplicationSettingsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ApplicationSettingsId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "AsycudaDocumentSet is required")]
       
public int AsycudaDocumentSetId
		{ 
		    get { return this.todo_creatediscrepancyentries.AsycudaDocumentSetId; }
			set
			{
			    if (value == this.todo_creatediscrepancyentries.AsycudaDocumentSetId) return;
				this.todo_creatediscrepancyentries.AsycudaDocumentSetId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("AsycudaDocumentSetId");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "AdjustmentType has a max length of 50 letters ")]
public string AdjustmentType
		{ 
		    get { return this.todo_creatediscrepancyentries.AdjustmentType; }
			set
			{
			    if (value == this.todo_creatediscrepancyentries.AdjustmentType) return;
				this.todo_creatediscrepancyentries.AdjustmentType = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("AdjustmentType");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "InvoiceNo is required")]
       
                
                [MaxLength(50, ErrorMessage = "InvoiceNo has a max length of 50 letters ")]
public string InvoiceNo
		{ 
		    get { return this.todo_creatediscrepancyentries.InvoiceNo; }
			set
			{
			    if (value == this.todo_creatediscrepancyentries.InvoiceNo) return;
				this.todo_creatediscrepancyentries.InvoiceNo = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("InvoiceNo");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "InvoiceDate is required")]
       
public System.DateTime InvoiceDate
		{ 
		    get { return this.todo_creatediscrepancyentries.InvoiceDate; }
			set
			{
			    if (value == this.todo_creatediscrepancyentries.InvoiceDate) return;
				this.todo_creatediscrepancyentries.InvoiceDate = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("InvoiceDate");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "Declarant_Reference_Number has a max length of 50 letters ")]
public string Declarant_Reference_Number
		{ 
		    get { return this.todo_creatediscrepancyentries.Declarant_Reference_Number; }
			set
			{
			    if (value == this.todo_creatediscrepancyentries.Declarant_Reference_Number) return;
				this.todo_creatediscrepancyentries.Declarant_Reference_Number = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Declarant_Reference_Number");
			}
		}
     


        ChangeTrackingCollection<DTO.TODO_CreateDiscrepancyEntries> _changeTracker;    
        public ChangeTrackingCollection<DTO.TODO_CreateDiscrepancyEntries> ChangeTracker
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


