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
       public partial class EmailMappingRexExs: BaseEntity<EmailMappingRexExs>
    {
        DTO.EmailMappingRexExs emailmappingrexexs;
        public EmailMappingRexExs(DTO.EmailMappingRexExs dto )
        {
              emailmappingrexexs = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.EmailMappingRexExs>(emailmappingrexexs);

        }

        public DTO.EmailMappingRexExs DTO
        {
            get
            {
             return emailmappingrexexs;
            }
            set
            {
                emailmappingrexexs = value;
            }
        }
       [RequiredValidationAttribute(ErrorMessage= " is required")]
       
public int Id
		{ 
		    get { return this.emailmappingrexexs.Id; }
			set
			{
			    if (value == this.emailmappingrexexs.Id) return;
				this.emailmappingrexexs.Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Id");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "EmailMapping is required")]
       
public int EmailMappingId
		{ 
		    get { return this.emailmappingrexexs.EmailMappingId; }
			set
			{
			    if (value == this.emailmappingrexexs.EmailMappingId) return;
				this.emailmappingrexexs.EmailMappingId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EmailMappingId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ReplacementRegex is required")]
       
                
                [MaxLength(50, ErrorMessage = "ReplacementRegex has a max length of 50 letters ")]
public string ReplacementRegex
		{ 
		    get { return this.emailmappingrexexs.ReplacementRegex; }
			set
			{
			    if (value == this.emailmappingrexexs.ReplacementRegex) return;
				this.emailmappingrexexs.ReplacementRegex = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ReplacementRegex");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "ReplacementValue has a max length of 50 letters ")]
public string ReplacementValue
		{ 
		    get { return this.emailmappingrexexs.ReplacementValue; }
			set
			{
			    if (value == this.emailmappingrexexs.ReplacementValue) return;
				this.emailmappingrexexs.ReplacementValue = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ReplacementValue");
			}
		}
     

       private EmailMapping _EmailMapping;
        public  EmailMapping EmailMapping
		{
		    get
               { 
                  if (this.emailmappingrexexs != null)
                   {
                       if (_EmailMapping != null)
                       {
                           if (this.emailmappingrexexs.EmailMapping !=
                               _EmailMapping.DTO)
                           {
                                if (this.emailmappingrexexs.EmailMapping  != null)
                               _EmailMapping = new EmailMapping(this.emailmappingrexexs.EmailMapping);
                           }
                       }
                       else
                       {
                             if (this.emailmappingrexexs.EmailMapping  != null)
                           _EmailMapping = new EmailMapping(this.emailmappingrexexs.EmailMapping);
                       }
                   }


             //       if (_EmailMapping != null) return _EmailMapping;
                       
             //       var i = new EmailMapping(){TrackingState = TrackingState.Added};
			//		//if (this.emailmappingrexexs.EmailMapping == null) Debugger.Break();
			//		if (this.emailmappingrexexs.EmailMapping != null)
            //        {
            //           i. = this.emailmappingrexexs.EmailMapping;
            //        }
            //        else
            //        {
            //            this.emailmappingrexexs.EmailMapping = i.;
             //       }
                           
            //        _EmailMapping = i;
                     
                    return _EmailMapping;
               }
			set
			{
			    if (value == _EmailMapping) return;
                _EmailMapping = value;
                if(value != null)
                     this.emailmappingrexexs.EmailMapping = value.DTO;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                NotifyPropertyChanged("EmailMapping");
			}
		}
        


        ChangeTrackingCollection<DTO.EmailMappingRexExs> _changeTracker;    
        public ChangeTrackingCollection<DTO.EmailMappingRexExs> ChangeTracker
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

