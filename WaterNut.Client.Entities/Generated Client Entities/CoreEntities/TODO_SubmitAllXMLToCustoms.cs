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
       public partial class TODO_SubmitAllXMLToCustoms: BaseEntity<TODO_SubmitAllXMLToCustoms>
    {
        DTO.TODO_SubmitAllXMLToCustoms todo_submitallxmltocustoms;
        public TODO_SubmitAllXMLToCustoms(DTO.TODO_SubmitAllXMLToCustoms dto )
        {
              todo_submitallxmltocustoms = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.TODO_SubmitAllXMLToCustoms>(todo_submitallxmltocustoms);

        }

        public DTO.TODO_SubmitAllXMLToCustoms DTO
        {
            get
            {
             return todo_submitallxmltocustoms;
            }
            set
            {
                todo_submitallxmltocustoms = value;
            }
        }
       [RequiredValidationAttribute(ErrorMessage= " is required")]
       
public int Id
		{ 
		    get { return this.todo_submitallxmltocustoms.Id; }
			set
			{
			    if (value == this.todo_submitallxmltocustoms.Id) return;
				this.todo_submitallxmltocustoms.Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Id");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "CNumber has a max length of 50 letters ")]
public string CNumber
		{ 
		    get { return this.todo_submitallxmltocustoms.CNumber; }
			set
			{
			    if (value == this.todo_submitallxmltocustoms.CNumber) return;
				this.todo_submitallxmltocustoms.CNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("CNumber");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ASYCUDA_ is required")]
       
public int ASYCUDA_Id
		{ 
		    get { return this.todo_submitallxmltocustoms.ASYCUDA_Id; }
			set
			{
			    if (value == this.todo_submitallxmltocustoms.ASYCUDA_Id) return;
				this.todo_submitallxmltocustoms.ASYCUDA_Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ASYCUDA_Id");
			}
		}
     

       
       
public Nullable<System.DateTime> RegistrationDate
		{ 
		    get { return this.todo_submitallxmltocustoms.RegistrationDate; }
			set
			{
			    if (value == this.todo_submitallxmltocustoms.RegistrationDate) return;
				this.todo_submitallxmltocustoms.RegistrationDate = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("RegistrationDate");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "ReferenceNumber has a max length of 50 letters ")]
public string ReferenceNumber
		{ 
		    get { return this.todo_submitallxmltocustoms.ReferenceNumber; }
			set
			{
			    if (value == this.todo_submitallxmltocustoms.ReferenceNumber) return;
				this.todo_submitallxmltocustoms.ReferenceNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ReferenceNumber");
			}
		}
     

       
       
public Nullable<int> AsycudaDocumentSetId
		{ 
		    get { return this.todo_submitallxmltocustoms.AsycudaDocumentSetId; }
			set
			{
			    if (value == this.todo_submitallxmltocustoms.AsycudaDocumentSetId) return;
				this.todo_submitallxmltocustoms.AsycudaDocumentSetId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("AsycudaDocumentSetId");
			}
		}
     

       
       
                
                [MaxLength(20, ErrorMessage = "DocumentType has a max length of 20 letters ")]
public string DocumentType
		{ 
		    get { return this.todo_submitallxmltocustoms.DocumentType; }
			set
			{
			    if (value == this.todo_submitallxmltocustoms.DocumentType) return;
				this.todo_submitallxmltocustoms.DocumentType = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("DocumentType");
			}
		}
     

       
       
public Nullable<System.DateTime> AssessmentDate
		{ 
		    get { return this.todo_submitallxmltocustoms.AssessmentDate; }
			set
			{
			    if (value == this.todo_submitallxmltocustoms.AssessmentDate) return;
				this.todo_submitallxmltocustoms.AssessmentDate = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("AssessmentDate");
			}
		}
     

       
       
public Nullable<int> ApplicationSettingsId
		{ 
		    get { return this.todo_submitallxmltocustoms.ApplicationSettingsId; }
			set
			{
			    if (value == this.todo_submitallxmltocustoms.ApplicationSettingsId) return;
				this.todo_submitallxmltocustoms.ApplicationSettingsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ApplicationSettingsId");
			}
		}
     

       
       
public Nullable<int> EmailId
		{ 
		    get { return this.todo_submitallxmltocustoms.EmailId; }
			set
			{
			    if (value == this.todo_submitallxmltocustoms.EmailId) return;
				this.todo_submitallxmltocustoms.EmailId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EmailId");
			}
		}
     

       
       
                
                [MaxLength(11, ErrorMessage = "CustomsProcedure has a max length of 11 letters ")]
public string CustomsProcedure
		{ 
		    get { return this.todo_submitallxmltocustoms.CustomsProcedure; }
			set
			{
			    if (value == this.todo_submitallxmltocustoms.CustomsProcedure) return;
				this.todo_submitallxmltocustoms.CustomsProcedure = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("CustomsProcedure");
			}
		}
     

       
       
                
                [MaxLength(255, ErrorMessage = "FilePath has a max length of 255 letters ")]
public string FilePath
		{ 
		    get { return this.todo_submitallxmltocustoms.FilePath; }
			set
			{
			    if (value == this.todo_submitallxmltocustoms.FilePath) return;
				this.todo_submitallxmltocustoms.FilePath = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("FilePath");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "Status has a max length of 50 letters ")]
public string Status
		{ 
		    get { return this.todo_submitallxmltocustoms.Status; }
			set
			{
			    if (value == this.todo_submitallxmltocustoms.Status) return;
				this.todo_submitallxmltocustoms.Status = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Status");
			}
		}
     

       
       
public Nullable<int> SystemDocumentSetId
		{ 
		    get { return this.todo_submitallxmltocustoms.SystemDocumentSetId; }
			set
			{
			    if (value == this.todo_submitallxmltocustoms.SystemDocumentSetId) return;
				this.todo_submitallxmltocustoms.SystemDocumentSetId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("SystemDocumentSetId");
			}
		}
     


        ChangeTrackingCollection<DTO.TODO_SubmitAllXMLToCustoms> _changeTracker;    
        public ChangeTrackingCollection<DTO.TODO_SubmitAllXMLToCustoms> ChangeTracker
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

