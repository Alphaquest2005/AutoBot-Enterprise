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
       public partial class Attachments: BaseEntity<Attachments>
    {
        DTO.Attachments attachments;
        public Attachments(DTO.Attachments dto )
        {
              attachments = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.Attachments>(attachments);

        }

        public DTO.Attachments DTO
        {
            get
            {
             return attachments;
            }
            set
            {
                attachments = value;
            }
        }
        


       [RequiredValidationAttribute(ErrorMessage= " is required")]
       
public int Id
		{ 
		    get { return this.attachments.Id; }
			set
			{
			    if (value == this.attachments.Id) return;
				this.attachments.Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Id");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "FilePath is required")]
       
                
                [MaxLength(500, ErrorMessage = "FilePath has a max length of 500 letters ")]
public string FilePath
		{ 
		    get { return this.attachments.FilePath; }
			set
			{
			    if (value == this.attachments.FilePath) return;
				this.attachments.FilePath = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("FilePath");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "DocumentCode is required")]
       
                
                [MaxLength(50, ErrorMessage = "DocumentCode has a max length of 50 letters ")]
public string DocumentCode
		{ 
		    get { return this.attachments.DocumentCode; }
			set
			{
			    if (value == this.attachments.DocumentCode) return;
				this.attachments.DocumentCode = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("DocumentCode");
			}
		}
     

        ObservableCollection<AsycudaDocumentSet_Attachments> _AsycudaDocumentSet_Attachments = null;
        public  ObservableCollection<AsycudaDocumentSet_Attachments> AsycudaDocumentSet_Attachments
		{
            
		    get 
				{ 
					if(_AsycudaDocumentSet_Attachments != null) return _AsycudaDocumentSet_Attachments;
					//if (this.attachments.AsycudaDocumentSet_Attachments == null) Debugger.Break();
					if(this.attachments.AsycudaDocumentSet_Attachments != null)
					{
						_AsycudaDocumentSet_Attachments = new ObservableCollection<AsycudaDocumentSet_Attachments>(this.attachments.AsycudaDocumentSet_Attachments.Select(x => new AsycudaDocumentSet_Attachments(x)));
					}
					
						_AsycudaDocumentSet_Attachments.CollectionChanged += AsycudaDocumentSet_Attachments_CollectionChanged; 
					
					return _AsycudaDocumentSet_Attachments; 
				}
			set
			{
			    if (Equals(value, _AsycudaDocumentSet_Attachments)) return;
				if (value != null)
					this.attachments.AsycudaDocumentSet_Attachments = new ChangeTrackingCollection<DTO.AsycudaDocumentSet_Attachments>(value.Select(x => x.DTO).ToList());
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
                        attachments.AsycudaDocumentSet_Attachments.Add(itm.DTO);
                    }
                    if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (AsycudaDocumentSet_Attachments itm in e.OldItems)
                    {
                        if (itm != null)
                        attachments.AsycudaDocumentSet_Attachments.Remove(itm.DTO);
                    }
					if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                    break;
                
            }
        }


        ChangeTrackingCollection<DTO.Attachments> _changeTracker;    
        public ChangeTrackingCollection<DTO.Attachments> ChangeTracker
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

