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
using OCR.Client.DTO;


using Core.Common.Validation;

namespace OCR.Client.Entities
{
       public partial class RecuringPart: BaseEntity<RecuringPart>
    {
        DTO.RecuringPart recuringpart;
        public RecuringPart(DTO.RecuringPart dto )
        {
              recuringpart = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.RecuringPart>(recuringpart);

        }

        public DTO.RecuringPart DTO
        {
            get
            {
             return recuringpart;
            }
            set
            {
                recuringpart = value;
            }
        }
       [RequiredValidationAttribute(ErrorMessage= " is required")]
       
public int Id
		{ 
		    get { return this.recuringpart.Id; }
			set
			{
			    if (value == this.recuringpart.Id) return;
				this.recuringpart.Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Id");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "IsComposite is required")]
       
public bool IsComposite
		{ 
		    get { return this.recuringpart.IsComposite; }
			set
			{
			    if (value == this.recuringpart.IsComposite) return;
				this.recuringpart.IsComposite = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("IsComposite");
			}
		}
     

       private Parts _Parts;
        public  Parts Parts
		{
		    get
               { 
                  if (this.recuringpart != null)
                   {
                       if (_Parts != null)
                       {
                           if (this.recuringpart.Parts !=
                               _Parts.DTO)
                           {
                                if (this.recuringpart.Parts  != null)
                               _Parts = new Parts(this.recuringpart.Parts);
                           }
                       }
                       else
                       {
                             if (this.recuringpart.Parts  != null)
                           _Parts = new Parts(this.recuringpart.Parts);
                       }
                   }


             //       if (_Parts != null) return _Parts;
                       
             //       var i = new Parts(){TrackingState = TrackingState.Added};
			//		//if (this.recuringpart.Parts == null) Debugger.Break();
			//		if (this.recuringpart.Parts != null)
            //        {
            //           i. = this.recuringpart.Parts;
            //        }
            //        else
            //        {
            //            this.recuringpart.Parts = i.;
             //       }
                           
            //        _Parts = i;
                     
                    return _Parts;
               }
			set
			{
			    if (value == _Parts) return;
                _Parts = value;
                if(value != null)
                     this.recuringpart.Parts = value.DTO;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                NotifyPropertyChanged("Parts");
			}
		}
        


        ChangeTrackingCollection<DTO.RecuringPart> _changeTracker;    
        public ChangeTrackingCollection<DTO.RecuringPart> ChangeTracker
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


