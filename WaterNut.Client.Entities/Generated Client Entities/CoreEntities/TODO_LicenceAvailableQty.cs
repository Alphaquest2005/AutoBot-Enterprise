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
       public partial class TODO_LicenceAvailableQty: BaseEntity<TODO_LicenceAvailableQty>
    {
        DTO.TODO_LicenceAvailableQty todo_licenceavailableqty;
        public TODO_LicenceAvailableQty(DTO.TODO_LicenceAvailableQty dto )
        {
              todo_licenceavailableqty = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.TODO_LicenceAvailableQty>(todo_licenceavailableqty);

        }

        public DTO.TODO_LicenceAvailableQty DTO
        {
            get
            {
             return todo_licenceavailableqty;
            }
            set
            {
                todo_licenceavailableqty = value;
            }
        }
       [RequiredValidationAttribute(ErrorMessage= "RegistrationNumber is required")]
       
                
                [MaxLength(50, ErrorMessage = "RegistrationNumber has a max length of 50 letters ")]
public string RegistrationNumber
		{ 
		    get { return this.todo_licenceavailableqty.RegistrationNumber; }
			set
			{
			    if (value == this.todo_licenceavailableqty.RegistrationNumber) return;
				this.todo_licenceavailableqty.RegistrationNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("RegistrationNumber");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ApplicationSettings is required")]
       
public int ApplicationSettingsId
		{ 
		    get { return this.todo_licenceavailableqty.ApplicationSettingsId; }
			set
			{
			    if (value == this.todo_licenceavailableqty.ApplicationSettingsId) return;
				this.todo_licenceavailableqty.ApplicationSettingsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ApplicationSettingsId");
			}
		}
     

       
       
                
                [MaxLength(8, ErrorMessage = "TariffCode has a max length of 8 letters ")]
public string TariffCode
		{ 
		    get { return this.todo_licenceavailableqty.TariffCode; }
			set
			{
			    if (value == this.todo_licenceavailableqty.TariffCode) return;
				this.todo_licenceavailableqty.TariffCode = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("TariffCode");
			}
		}
     

       
       
                
                [MaxLength(255, ErrorMessage = "Origin has a max length of 255 letters ")]
public string Origin
		{ 
		    get { return this.todo_licenceavailableqty.Origin; }
			set
			{
			    if (value == this.todo_licenceavailableqty.Origin) return;
				this.todo_licenceavailableqty.Origin = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Origin");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Quantity_to_approve is required")]
       [NumberValidationAttribute]
public double Quantity_to_approve
		{ 
		    get { return this.todo_licenceavailableqty.Quantity_to_approve; }
			set
			{
			    if (value == this.todo_licenceavailableqty.Quantity_to_approve) return;
				this.todo_licenceavailableqty.Quantity_to_approve = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Quantity_to_approve");
			}
		}
     

       
       
public Nullable<System.DateTime> Application_date
		{ 
		    get { return this.todo_licenceavailableqty.Application_date; }
			set
			{
			    if (value == this.todo_licenceavailableqty.Application_date) return;
				this.todo_licenceavailableqty.Application_date = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Application_date");
			}
		}
     

       
       
public Nullable<System.DateTime> Importation_date
		{ 
		    get { return this.todo_licenceavailableqty.Importation_date; }
			set
			{
			    if (value == this.todo_licenceavailableqty.Importation_date) return;
				this.todo_licenceavailableqty.Importation_date = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Importation_date");
			}
		}
     

       
       
                
                [MaxLength(55, ErrorMessage = "Key has a max length of 55 letters ")]
public string Key
		{ 
		    get { return this.todo_licenceavailableqty.Key; }
			set
			{
			    if (value == this.todo_licenceavailableqty.Key) return;
				this.todo_licenceavailableqty.Key = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Key");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Balance is required")]
       [NumberValidationAttribute]
public double Balance
		{ 
		    get { return this.todo_licenceavailableqty.Balance; }
			set
			{
			    if (value == this.todo_licenceavailableqty.Balance) return;
				this.todo_licenceavailableqty.Balance = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Balance");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "License is required")]
       
public int LicenseId
		{ 
		    get { return this.todo_licenceavailableqty.LicenseId; }
			set
			{
			    if (value == this.todo_licenceavailableqty.LicenseId) return;
				this.todo_licenceavailableqty.LicenseId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("LicenseId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "SourceFile is required")]
       
                
                [MaxLength(300, ErrorMessage = "SourceFile has a max length of 300 letters ")]
public string SourceFile
		{ 
		    get { return this.todo_licenceavailableqty.SourceFile; }
			set
			{
			    if (value == this.todo_licenceavailableqty.SourceFile) return;
				this.todo_licenceavailableqty.SourceFile = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("SourceFile");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "DocumentReference has a max length of 50 letters ")]
public string DocumentReference
		{ 
		    get { return this.todo_licenceavailableqty.DocumentReference; }
			set
			{
			    if (value == this.todo_licenceavailableqty.DocumentReference) return;
				this.todo_licenceavailableqty.DocumentReference = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("DocumentReference");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Segment is required")]
       
public int SegmentId
		{ 
		    get { return this.todo_licenceavailableqty.SegmentId; }
			set
			{
			    if (value == this.todo_licenceavailableqty.SegmentId) return;
				this.todo_licenceavailableqty.SegmentId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("SegmentId");
			}
		}
     


        ChangeTrackingCollection<DTO.TODO_LicenceAvailableQty> _changeTracker;    
        public ChangeTrackingCollection<DTO.TODO_LicenceAvailableQty> ChangeTracker
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


