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
       public partial class TODO_ERRReport_AsycudaLines: BaseEntity<TODO_ERRReport_AsycudaLines>
    {
        DTO.TODO_ERRReport_AsycudaLines todo_errreport_asycudalines;
        public TODO_ERRReport_AsycudaLines(DTO.TODO_ERRReport_AsycudaLines dto )
        {
              todo_errreport_asycudalines = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.TODO_ERRReport_AsycudaLines>(todo_errreport_asycudalines);

        }

        public DTO.TODO_ERRReport_AsycudaLines DTO
        {
            get
            {
             return todo_errreport_asycudalines;
            }
            set
            {
                todo_errreport_asycudalines = value;
            }
        }
       
       
public Nullable<int> ApplicationSettingsId
		{ 
		    get { return this.todo_errreport_asycudalines.ApplicationSettingsId; }
			set
			{
			    if (value == this.todo_errreport_asycudalines.ApplicationSettingsId) return;
				this.todo_errreport_asycudalines.ApplicationSettingsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ApplicationSettingsId");
			}
		}
     

       
       
                
                [MaxLength(20, ErrorMessage = "CNumber has a max length of 20 letters ")]
public string CNumber
		{ 
		    get { return this.todo_errreport_asycudalines.CNumber; }
			set
			{
			    if (value == this.todo_errreport_asycudalines.CNumber) return;
				this.todo_errreport_asycudalines.CNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("CNumber");
			}
		}
     

       
       
                
                [MaxLength(30, ErrorMessage = "Reference has a max length of 30 letters ")]
public string Reference
		{ 
		    get { return this.todo_errreport_asycudalines.Reference; }
			set
			{
			    if (value == this.todo_errreport_asycudalines.Reference) return;
				this.todo_errreport_asycudalines.Reference = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Reference");
			}
		}
     

       
       
public Nullable<System.DateTime> RegistrationDate
		{ 
		    get { return this.todo_errreport_asycudalines.RegistrationDate; }
			set
			{
			    if (value == this.todo_errreport_asycudalines.RegistrationDate) return;
				this.todo_errreport_asycudalines.RegistrationDate = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("RegistrationDate");
			}
		}
     

       
       
                
                [MaxLength(40, ErrorMessage = "DocumentType has a max length of 40 letters ")]
public string DocumentType
		{ 
		    get { return this.todo_errreport_asycudalines.DocumentType; }
			set
			{
			    if (value == this.todo_errreport_asycudalines.DocumentType) return;
				this.todo_errreport_asycudalines.DocumentType = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("DocumentType");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<int> LineNumber
		{ 
		    get { return this.todo_errreport_asycudalines.LineNumber; }
			set
			{
			    if (value == this.todo_errreport_asycudalines.LineNumber) return;
				this.todo_errreport_asycudalines.LineNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("LineNumber");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "ItemNumber has a max length of 50 letters ")]
public string ItemNumber
		{ 
		    get { return this.todo_errreport_asycudalines.ItemNumber; }
			set
			{
			    if (value == this.todo_errreport_asycudalines.ItemNumber) return;
				this.todo_errreport_asycudalines.ItemNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ItemNumber");
			}
		}
     

       
       
                
                [MaxLength(255, ErrorMessage = "Description has a max length of 255 letters ")]
public string Description
		{ 
		    get { return this.todo_errreport_asycudalines.Description; }
			set
			{
			    if (value == this.todo_errreport_asycudalines.Description) return;
				this.todo_errreport_asycudalines.Description = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Description");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Error is required")]
       
                
                [MaxLength(26, ErrorMessage = "Error has a max length of 26 letters ")]
public string Error
		{ 
		    get { return this.todo_errreport_asycudalines.Error; }
			set
			{
			    if (value == this.todo_errreport_asycudalines.Error) return;
				this.todo_errreport_asycudalines.Error = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Error");
			}
		}
     

       
       
                
                [MaxLength(66, ErrorMessage = "Info has a max length of 66 letters ")]
public string Info
		{ 
		    get { return this.todo_errreport_asycudalines.Info; }
			set
			{
			    if (value == this.todo_errreport_asycudalines.Info) return;
				this.todo_errreport_asycudalines.Info = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Info");
			}
		}
     


        ChangeTrackingCollection<DTO.TODO_ERRReport_AsycudaLines> _changeTracker;    
        public ChangeTrackingCollection<DTO.TODO_ERRReport_AsycudaLines> ChangeTracker
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


