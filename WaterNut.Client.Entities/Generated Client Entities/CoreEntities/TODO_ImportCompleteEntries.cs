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
       public partial class TODO_ImportCompleteEntries: BaseEntity<TODO_ImportCompleteEntries>
    {
        DTO.TODO_ImportCompleteEntries todo_importcompleteentries;
        public TODO_ImportCompleteEntries(DTO.TODO_ImportCompleteEntries dto )
        {
              todo_importcompleteentries = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.TODO_ImportCompleteEntries>(todo_importcompleteentries);

        }

        public DTO.TODO_ImportCompleteEntries DTO
        {
            get
            {
             return todo_importcompleteentries;
            }
            set
            {
                todo_importcompleteentries = value;
            }
        }
       [RequiredValidationAttribute(ErrorMessage= "AsycudaDocumentSet is required")]
       
public int AsycudaDocumentSetId
		{ 
		    get { return this.todo_importcompleteentries.AsycudaDocumentSetId; }
			set
			{
			    if (value == this.todo_importcompleteentries.AsycudaDocumentSetId) return;
				this.todo_importcompleteentries.AsycudaDocumentSetId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("AsycudaDocumentSetId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ApplicationSettings is required")]
       
public int ApplicationSettingsId
		{ 
		    get { return this.todo_importcompleteentries.ApplicationSettingsId; }
			set
			{
			    if (value == this.todo_importcompleteentries.ApplicationSettingsId) return;
				this.todo_importcompleteentries.ApplicationSettingsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ApplicationSettingsId");
			}
		}
     

       
       
public Nullable<int> EmailId
		{ 
		    get { return this.todo_importcompleteentries.EmailId; }
			set
			{
			    if (value == this.todo_importcompleteentries.EmailId) return;
				this.todo_importcompleteentries.EmailId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EmailId");
			}
		}
     

       
       
public Nullable<int> FileTypeId
		{ 
		    get { return this.todo_importcompleteentries.FileTypeId; }
			set
			{
			    if (value == this.todo_importcompleteentries.FileTypeId) return;
				this.todo_importcompleteentries.FileTypeId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("FileTypeId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ASYCUDA_ is required")]
       
public int ASYCUDA_Id
		{ 
		    get { return this.todo_importcompleteentries.ASYCUDA_Id; }
			set
			{
			    if (value == this.todo_importcompleteentries.ASYCUDA_Id) return;
				this.todo_importcompleteentries.ASYCUDA_Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ASYCUDA_Id");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "EntryData is required")]
       
                
                [MaxLength(50, ErrorMessage = "EntryDataId has a max length of 50 letters ")]
public string EntryDataId
		{ 
		    get { return this.todo_importcompleteentries.EntryDataId; }
			set
			{
			    if (value == this.todo_importcompleteentries.EntryDataId) return;
				this.todo_importcompleteentries.EntryDataId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EntryDataId");
			}
		}
     


        ChangeTrackingCollection<DTO.TODO_ImportCompleteEntries> _changeTracker;    
        public ChangeTrackingCollection<DTO.TODO_ImportCompleteEntries> ChangeTracker
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

