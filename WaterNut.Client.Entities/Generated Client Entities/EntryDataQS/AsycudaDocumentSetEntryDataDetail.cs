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
using EntryDataQS.Client.DTO;


using Core.Common.Validation;

namespace EntryDataQS.Client.Entities
{
       public partial class AsycudaDocumentSetEntryDataDetail: BaseEntity<AsycudaDocumentSetEntryDataDetail>
    {
        DTO.AsycudaDocumentSetEntryDataDetail asycudadocumentsetentrydatadetail;
        public AsycudaDocumentSetEntryDataDetail(DTO.AsycudaDocumentSetEntryDataDetail dto )
        {
              asycudadocumentsetentrydatadetail = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.AsycudaDocumentSetEntryDataDetail>(asycudadocumentsetentrydatadetail);

        }

        public DTO.AsycudaDocumentSetEntryDataDetail DTO
        {
            get
            {
             return asycudadocumentsetentrydatadetail;
            }
            set
            {
                asycudadocumentsetentrydatadetail = value;
            }
        }
       [RequiredValidationAttribute(ErrorMessage= "AsycudaDocumentSet is required")]
       
public int AsycudaDocumentSetId
		{ 
		    get { return this.asycudadocumentsetentrydatadetail.AsycudaDocumentSetId; }
			set
			{
			    if (value == this.asycudadocumentsetentrydatadetail.AsycudaDocumentSetId) return;
				this.asycudadocumentsetentrydatadetail.AsycudaDocumentSetId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("AsycudaDocumentSetId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "EntryDataDetails is required")]
       
public int EntryDataDetailsId
		{ 
		    get { return this.asycudadocumentsetentrydatadetail.EntryDataDetailsId; }
			set
			{
			    if (value == this.asycudadocumentsetentrydatadetail.EntryDataDetailsId) return;
				this.asycudadocumentsetentrydatadetail.EntryDataDetailsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EntryDataDetailsId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= " is required")]
       
public long Id
		{ 
		    get { return this.asycudadocumentsetentrydatadetail.Id; }
			set
			{
			    if (value == this.asycudadocumentsetentrydatadetail.Id) return;
				this.asycudadocumentsetentrydatadetail.Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Id");
			}
		}
     

       private EntryDataDetailsEx _EntryDataDetailsEx;
        public  EntryDataDetailsEx EntryDataDetailsEx
		{
		    get
               { 
                  if (this.asycudadocumentsetentrydatadetail != null)
                   {
                       if (_EntryDataDetailsEx != null)
                       {
                           if (this.asycudadocumentsetentrydatadetail.EntryDataDetailsEx !=
                               _EntryDataDetailsEx.DTO)
                           {
                                if (this.asycudadocumentsetentrydatadetail.EntryDataDetailsEx  != null)
                               _EntryDataDetailsEx = new EntryDataDetailsEx(this.asycudadocumentsetentrydatadetail.EntryDataDetailsEx);
                           }
                       }
                       else
                       {
                             if (this.asycudadocumentsetentrydatadetail.EntryDataDetailsEx  != null)
                           _EntryDataDetailsEx = new EntryDataDetailsEx(this.asycudadocumentsetentrydatadetail.EntryDataDetailsEx);
                       }
                   }


             //       if (_EntryDataDetailsEx != null) return _EntryDataDetailsEx;
                       
             //       var i = new EntryDataDetailsEx(){TrackingState = TrackingState.Added};
			//		//if (this.asycudadocumentsetentrydatadetail.EntryDataDetailsEx == null) Debugger.Break();
			//		if (this.asycudadocumentsetentrydatadetail.EntryDataDetailsEx != null)
            //        {
            //           i. = this.asycudadocumentsetentrydatadetail.EntryDataDetailsEx;
            //        }
            //        else
            //        {
            //            this.asycudadocumentsetentrydatadetail.EntryDataDetailsEx = i.;
             //       }
                           
            //        _EntryDataDetailsEx = i;
                     
                    return _EntryDataDetailsEx;
               }
			set
			{
			    if (value == _EntryDataDetailsEx) return;
                _EntryDataDetailsEx = value;
                if(value != null)
                     this.asycudadocumentsetentrydatadetail.EntryDataDetailsEx = value.DTO;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                NotifyPropertyChanged("EntryDataDetailsEx");
			}
		}
        


        ChangeTrackingCollection<DTO.AsycudaDocumentSetEntryDataDetail> _changeTracker;    
        public ChangeTrackingCollection<DTO.AsycudaDocumentSetEntryDataDetail> ChangeTracker
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


