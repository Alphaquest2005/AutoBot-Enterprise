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
       public partial class TODO_ERRReport_AsycudaEntries: BaseEntity<TODO_ERRReport_AsycudaEntries>
    {
        DTO.TODO_ERRReport_AsycudaEntries todo_errreport_asycudaentries;
        public TODO_ERRReport_AsycudaEntries(DTO.TODO_ERRReport_AsycudaEntries dto )
        {
              todo_errreport_asycudaentries = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.TODO_ERRReport_AsycudaEntries>(todo_errreport_asycudaentries);

        }

        public DTO.TODO_ERRReport_AsycudaEntries DTO
        {
            get
            {
             return todo_errreport_asycudaentries;
            }
            set
            {
                todo_errreport_asycudaentries = value;
            }
        }
       
       
public Nullable<int> ApplicationSettingsId
		{ 
		    get { return this.todo_errreport_asycudaentries.ApplicationSettingsId; }
			set
			{
			    if (value == this.todo_errreport_asycudaentries.ApplicationSettingsId) return;
				this.todo_errreport_asycudaentries.ApplicationSettingsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ApplicationSettingsId");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "CNumber has a max length of 50 letters ")]
public string CNumber
		{ 
		    get { return this.todo_errreport_asycudaentries.CNumber; }
			set
			{
			    if (value == this.todo_errreport_asycudaentries.CNumber) return;
				this.todo_errreport_asycudaentries.CNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("CNumber");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "Reference has a max length of 50 letters ")]
public string Reference
		{ 
		    get { return this.todo_errreport_asycudaentries.Reference; }
			set
			{
			    if (value == this.todo_errreport_asycudaentries.Reference) return;
				this.todo_errreport_asycudaentries.Reference = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Reference");
			}
		}
     

       
       
public Nullable<System.DateTime> RegistrationDate
		{ 
		    get { return this.todo_errreport_asycudaentries.RegistrationDate; }
			set
			{
			    if (value == this.todo_errreport_asycudaentries.RegistrationDate) return;
				this.todo_errreport_asycudaentries.RegistrationDate = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("RegistrationDate");
			}
		}
     

       
       
                
                [MaxLength(40, ErrorMessage = "DocumentType has a max length of 40 letters ")]
public string DocumentType
		{ 
		    get { return this.todo_errreport_asycudaentries.DocumentType; }
			set
			{
			    if (value == this.todo_errreport_asycudaentries.DocumentType) return;
				this.todo_errreport_asycudaentries.DocumentType = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("DocumentType");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Error is required")]
       
                
                [MaxLength(16, ErrorMessage = "Error has a max length of 16 letters ")]
public string Error
		{ 
		    get { return this.todo_errreport_asycudaentries.Error; }
			set
			{
			    if (value == this.todo_errreport_asycudaentries.Error) return;
				this.todo_errreport_asycudaentries.Error = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Error");
			}
		}
     

       
       
                
                [MaxLength(130, ErrorMessage = "Info has a max length of 130 letters ")]
public string Info
		{ 
		    get { return this.todo_errreport_asycudaentries.Info; }
			set
			{
			    if (value == this.todo_errreport_asycudaentries.Info) return;
				this.todo_errreport_asycudaentries.Info = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Info");
			}
		}
     


        ChangeTrackingCollection<DTO.TODO_ERRReport_AsycudaEntries> _changeTracker;    
        public ChangeTrackingCollection<DTO.TODO_ERRReport_AsycudaEntries> ChangeTracker
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


