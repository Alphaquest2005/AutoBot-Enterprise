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
       public partial class Emails: BaseEntity<Emails>
    {
        DTO.Emails emails;
        public Emails(DTO.Emails dto )
        {
              emails = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.Emails>(emails);

        }

        public DTO.Emails DTO
        {
            get
            {
             return emails;
            }
            set
            {
                emails = value;
            }
        }
        


       [RequiredValidationAttribute(ErrorMessage= "EmailUnique is required")]
       
public int EmailUniqueId
		{ 
		    get { return this.emails.EmailUniqueId; }
			set
			{
			    if (value == this.emails.EmailUniqueId) return;
				this.emails.EmailUniqueId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EmailUniqueId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Subject is required")]
       
                
                
public string Subject
		{ 
		    get { return this.emails.Subject; }
			set
			{
			    if (value == this.emails.Subject) return;
				this.emails.Subject = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Subject");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "EmailDate is required")]
       
public System.DateTime EmailDate
		{ 
		    get { return this.emails.EmailDate; }
			set
			{
			    if (value == this.emails.EmailDate) return;
				this.emails.EmailDate = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EmailDate");
			}
		}
     

        ObservableCollection<AsycudaDocumentSet_Attachments> _AsycudaDocumentSet_Attachments = null;
        public  ObservableCollection<AsycudaDocumentSet_Attachments> AsycudaDocumentSet_Attachments
		{
            
		    get 
				{ 
					if(_AsycudaDocumentSet_Attachments != null) return _AsycudaDocumentSet_Attachments;
					//if (this.emails.AsycudaDocumentSet_Attachments == null) Debugger.Break();
					if(this.emails.AsycudaDocumentSet_Attachments != null)
					{
						_AsycudaDocumentSet_Attachments = new ObservableCollection<AsycudaDocumentSet_Attachments>(this.emails.AsycudaDocumentSet_Attachments.Select(x => new AsycudaDocumentSet_Attachments(x)));
					}
					
						_AsycudaDocumentSet_Attachments.CollectionChanged += AsycudaDocumentSet_Attachments_CollectionChanged; 
					
					return _AsycudaDocumentSet_Attachments; 
				}
			set
			{
			    if (Equals(value, _AsycudaDocumentSet_Attachments)) return;
				if (value != null)
					this.emails.AsycudaDocumentSet_Attachments = new ChangeTrackingCollection<DTO.AsycudaDocumentSet_Attachments>(value.Select(x => x.DTO).ToList());
                _AsycudaDocumentSet_Attachments = value;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				if (_AsycudaDocumentSet_Attachments != null)
				_AsycudaDocumentSet_Attachments.CollectionChanged += AsycudaDocumentSet_Attachments_CollectionChanged;               
				NotifyPropertyChanged("AsycudaDocumentSet_Attachments");
			}
		}
        
        void AsycudaDocumentSet_Attachments_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (AsycudaDocumentSet_Attachments itm in e.NewItems)
                    {
                        if (itm != null)
                        emails.AsycudaDocumentSet_Attachments.Add(itm.DTO);
                    }
                    if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (AsycudaDocumentSet_Attachments itm in e.OldItems)
                    {
                        if (itm != null)
                        emails.AsycudaDocumentSet_Attachments.Remove(itm.DTO);
                    }
					if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                    break;
                
            }
        }


        ChangeTrackingCollection<DTO.Emails> _changeTracker;    
        public ChangeTrackingCollection<DTO.Emails> ChangeTracker
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

