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
       public partial class TODO_DocumentsToDelete: BaseEntity<TODO_DocumentsToDelete>
    {
        DTO.TODO_DocumentsToDelete todo_documentstodelete;
        public TODO_DocumentsToDelete(DTO.TODO_DocumentsToDelete dto )
        {
              todo_documentstodelete = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.TODO_DocumentsToDelete>(todo_documentstodelete);

        }

        public DTO.TODO_DocumentsToDelete DTO
        {
            get
            {
             return todo_documentstodelete;
            }
            set
            {
                todo_documentstodelete = value;
            }
        }
       [RequiredValidationAttribute(ErrorMessage= "ASYCUDA_ is required")]
       
public int ASYCUDA_Id
		{ 
		    get { return this.todo_documentstodelete.ASYCUDA_Id; }
			set
			{
			    if (value == this.todo_documentstodelete.ASYCUDA_Id) return;
				this.todo_documentstodelete.ASYCUDA_Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ASYCUDA_Id");
			}
		}
     

       
       
                
                [MaxLength(40, ErrorMessage = "DocumentType has a max length of 40 letters ")]
public string DocumentType
		{ 
		    get { return this.todo_documentstodelete.DocumentType; }
			set
			{
			    if (value == this.todo_documentstodelete.DocumentType) return;
				this.todo_documentstodelete.DocumentType = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("DocumentType");
			}
		}
     

       
       
                
                [MaxLength(30, ErrorMessage = "Reference has a max length of 30 letters ")]
public string Reference
		{ 
		    get { return this.todo_documentstodelete.Reference; }
			set
			{
			    if (value == this.todo_documentstodelete.Reference) return;
				this.todo_documentstodelete.Reference = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Reference");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ApplicationSettings is required")]
       
public int ApplicationSettingsId
		{ 
		    get { return this.todo_documentstodelete.ApplicationSettingsId; }
			set
			{
			    if (value == this.todo_documentstodelete.ApplicationSettingsId) return;
				this.todo_documentstodelete.ApplicationSettingsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ApplicationSettingsId");
			}
		}
     


        ChangeTrackingCollection<DTO.TODO_DocumentsToDelete> _changeTracker;    
        public ChangeTrackingCollection<DTO.TODO_DocumentsToDelete> ChangeTracker
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

