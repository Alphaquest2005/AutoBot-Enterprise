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
       public partial class ImportActions: BaseEntity<ImportActions>
    {
        DTO.ImportActions importactions;
        public ImportActions(DTO.ImportActions dto )
        {
              importactions = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.ImportActions>(importactions);

        }

        public DTO.ImportActions DTO
        {
            get
            {
             return importactions;
            }
            set
            {
                importactions = value;
            }
        }
       [RequiredValidationAttribute(ErrorMessage= " is required")]
       
public int Id
		{ 
		    get { return this.importactions.Id; }
			set
			{
			    if (value == this.importactions.Id) return;
				this.importactions.Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Id");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "FileType is required")]
       
public int FileTypeId
		{ 
		    get { return this.importactions.FileTypeId; }
			set
			{
			    if (value == this.importactions.FileTypeId) return;
				this.importactions.FileTypeId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("FileTypeId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Name is required")]
       
                
                [MaxLength(50, ErrorMessage = "Name has a max length of 50 letters ")]
public string Name
		{ 
		    get { return this.importactions.Name; }
			set
			{
			    if (value == this.importactions.Name) return;
				this.importactions.Name = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Name");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Action is required")]
       
                
                [MaxLength(500, ErrorMessage = "Action has a max length of 500 letters ")]
public string Action
		{ 
		    get { return this.importactions.Action; }
			set
			{
			    if (value == this.importactions.Action) return;
				this.importactions.Action = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Action");
			}
		}
     

       private FileTypes _FileTypes;
        public  FileTypes FileTypes
		{
		    get
               { 
                  if (this.importactions != null)
                   {
                       if (_FileTypes != null)
                       {
                           if (this.importactions.FileTypes !=
                               _FileTypes.DTO)
                           {
                                if (this.importactions.FileTypes  != null)
                               _FileTypes = new FileTypes(this.importactions.FileTypes);
                           }
                       }
                       else
                       {
                             if (this.importactions.FileTypes  != null)
                           _FileTypes = new FileTypes(this.importactions.FileTypes);
                       }
                   }


             //       if (_FileTypes != null) return _FileTypes;
                       
             //       var i = new FileTypes(){TrackingState = TrackingState.Added};
			//		//if (this.importactions.FileTypes == null) Debugger.Break();
			//		if (this.importactions.FileTypes != null)
            //        {
            //           i. = this.importactions.FileTypes;
            //        }
            //        else
            //        {
            //            this.importactions.FileTypes = i.;
             //       }
                           
            //        _FileTypes = i;
                     
                    return _FileTypes;
               }
			set
			{
			    if (value == _FileTypes) return;
                _FileTypes = value;
                if(value != null)
                     this.importactions.FileTypes = value.DTO;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                NotifyPropertyChanged("FileTypes");
			}
		}
        


        ChangeTrackingCollection<DTO.ImportActions> _changeTracker;    
        public ChangeTrackingCollection<DTO.ImportActions> ChangeTracker
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

