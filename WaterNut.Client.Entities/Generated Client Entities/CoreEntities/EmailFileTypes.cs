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
       public partial class EmailFileTypes: BaseEntity<EmailFileTypes>
    {
        DTO.EmailFileTypes emailfiletypes;
        public EmailFileTypes(DTO.EmailFileTypes dto )
        {
              emailfiletypes = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.EmailFileTypes>(emailfiletypes);

        }

        public DTO.EmailFileTypes DTO
        {
            get
            {
             return emailfiletypes;
            }
            set
            {
                emailfiletypes = value;
            }
        }
       [RequiredValidationAttribute(ErrorMessage= " is required")]
       
public int Id
		{ 
		    get { return this.emailfiletypes.Id; }
			set
			{
			    if (value == this.emailfiletypes.Id) return;
				this.emailfiletypes.Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Id");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "EmailMapping is required")]
       
public int EmailMappingId
		{ 
		    get { return this.emailfiletypes.EmailMappingId; }
			set
			{
			    if (value == this.emailfiletypes.EmailMappingId) return;
				this.emailfiletypes.EmailMappingId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EmailMappingId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "FileType is required")]
       
public int FileTypeId
		{ 
		    get { return this.emailfiletypes.FileTypeId; }
			set
			{
			    if (value == this.emailfiletypes.FileTypeId) return;
				this.emailfiletypes.FileTypeId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("FileTypeId");
			}
		}
     

       private EmailMapping _EmailMapping;
        public  EmailMapping EmailMapping
		{
		    get
               { 
                  if (this.emailfiletypes != null)
                   {
                       if (_EmailMapping != null)
                       {
                           if (this.emailfiletypes.EmailMapping !=
                               _EmailMapping.DTO)
                           {
                                if (this.emailfiletypes.EmailMapping  != null)
                               _EmailMapping = new EmailMapping(this.emailfiletypes.EmailMapping);
                           }
                       }
                       else
                       {
                             if (this.emailfiletypes.EmailMapping  != null)
                           _EmailMapping = new EmailMapping(this.emailfiletypes.EmailMapping);
                       }
                   }


             //       if (_EmailMapping != null) return _EmailMapping;
                       
             //       var i = new EmailMapping(){TrackingState = TrackingState.Added};
			//		//if (this.emailfiletypes.EmailMapping == null) Debugger.Break();
			//		if (this.emailfiletypes.EmailMapping != null)
            //        {
            //           i. = this.emailfiletypes.EmailMapping;
            //        }
            //        else
            //        {
            //            this.emailfiletypes.EmailMapping = i.;
             //       }
                           
            //        _EmailMapping = i;
                     
                    return _EmailMapping;
               }
			set
			{
			    if (value == _EmailMapping) return;
                _EmailMapping = value;
                if(value != null)
                     this.emailfiletypes.EmailMapping = value.DTO;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                NotifyPropertyChanged("EmailMapping");
			}
		}
        

       private FileTypes _FileTypes;
        public  FileTypes FileTypes
		{
		    get
               { 
                  if (this.emailfiletypes != null)
                   {
                       if (_FileTypes != null)
                       {
                           if (this.emailfiletypes.FileTypes !=
                               _FileTypes.DTO)
                           {
                                if (this.emailfiletypes.FileTypes  != null)
                               _FileTypes = new FileTypes(this.emailfiletypes.FileTypes);
                           }
                       }
                       else
                       {
                             if (this.emailfiletypes.FileTypes  != null)
                           _FileTypes = new FileTypes(this.emailfiletypes.FileTypes);
                       }
                   }


             //       if (_FileTypes != null) return _FileTypes;
                       
             //       var i = new FileTypes(){TrackingState = TrackingState.Added};
			//		//if (this.emailfiletypes.FileTypes == null) Debugger.Break();
			//		if (this.emailfiletypes.FileTypes != null)
            //        {
            //           i. = this.emailfiletypes.FileTypes;
            //        }
            //        else
            //        {
            //            this.emailfiletypes.FileTypes = i.;
             //       }
                           
            //        _FileTypes = i;
                     
                    return _FileTypes;
               }
			set
			{
			    if (value == _FileTypes) return;
                _FileTypes = value;
                if(value != null)
                     this.emailfiletypes.FileTypes = value.DTO;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                NotifyPropertyChanged("FileTypes");
			}
		}
        


        ChangeTrackingCollection<DTO.EmailFileTypes> _changeTracker;    
        public ChangeTrackingCollection<DTO.EmailFileTypes> ChangeTracker
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

