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
       public partial class ActionDocSetLogs: BaseEntity<ActionDocSetLogs>
    {
        DTO.ActionDocSetLogs actiondocsetlogs;
        public ActionDocSetLogs(DTO.ActionDocSetLogs dto )
        {
              actiondocsetlogs = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.ActionDocSetLogs>(actiondocsetlogs);

        }

        public DTO.ActionDocSetLogs DTO
        {
            get
            {
             return actiondocsetlogs;
            }
            set
            {
                actiondocsetlogs = value;
            }
        }
       [RequiredValidationAttribute(ErrorMessage= "Acton is required")]
       
public int ActonId
		{ 
		    get { return this.actiondocsetlogs.ActonId; }
			set
			{
			    if (value == this.actiondocsetlogs.ActonId) return;
				this.actiondocsetlogs.ActonId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ActonId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "AsycudaDocumentSet is required")]
       
public int AsycudaDocumentSetId
		{ 
		    get { return this.actiondocsetlogs.AsycudaDocumentSetId; }
			set
			{
			    if (value == this.actiondocsetlogs.AsycudaDocumentSetId) return;
				this.actiondocsetlogs.AsycudaDocumentSetId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("AsycudaDocumentSetId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= " is required")]
       
public int Id
		{ 
		    get { return this.actiondocsetlogs.Id; }
			set
			{
			    if (value == this.actiondocsetlogs.Id) return;
				this.actiondocsetlogs.Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Id");
			}
		}
     

       private Actions _Actions;
        public  Actions Actions
		{
		    get
               { 
                  if (this.actiondocsetlogs != null)
                   {
                       if (_Actions != null)
                       {
                           if (this.actiondocsetlogs.Actions !=
                               _Actions.DTO)
                           {
                                if (this.actiondocsetlogs.Actions  != null)
                               _Actions = new Actions(this.actiondocsetlogs.Actions);
                           }
                       }
                       else
                       {
                             if (this.actiondocsetlogs.Actions  != null)
                           _Actions = new Actions(this.actiondocsetlogs.Actions);
                       }
                   }


             //       if (_Actions != null) return _Actions;
                       
             //       var i = new Actions(){TrackingState = TrackingState.Added};
			//		//if (this.actiondocsetlogs.Actions == null) Debugger.Break();
			//		if (this.actiondocsetlogs.Actions != null)
            //        {
            //           i. = this.actiondocsetlogs.Actions;
            //        }
            //        else
            //        {
            //            this.actiondocsetlogs.Actions = i.;
             //       }
                           
            //        _Actions = i;
                     
                    return _Actions;
               }
			set
			{
			    if (value == _Actions) return;
                _Actions = value;
                if(value != null)
                     this.actiondocsetlogs.Actions = value.DTO;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                NotifyPropertyChanged("Actions");
			}
		}
        


        ChangeTrackingCollection<DTO.ActionDocSetLogs> _changeTracker;    
        public ChangeTrackingCollection<DTO.ActionDocSetLogs> ChangeTracker
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

