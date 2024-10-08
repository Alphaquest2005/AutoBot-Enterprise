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
using AdjustmentQS.Client.DTO;


using Core.Common.Validation;

namespace AdjustmentQS.Client.Entities
{
       public partial class AdjustmentOversAllocation: BaseEntity<AdjustmentOversAllocation>
    {
        DTO.AdjustmentOversAllocation adjustmentoversallocation;
        public AdjustmentOversAllocation(DTO.AdjustmentOversAllocation dto )
        {
              adjustmentoversallocation = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.AdjustmentOversAllocation>(adjustmentoversallocation);

        }

        public DTO.AdjustmentOversAllocation DTO
        {
            get
            {
             return adjustmentoversallocation;
            }
            set
            {
                adjustmentoversallocation = value;
            }
        }
       [RequiredValidationAttribute(ErrorMessage= "Allocation is required")]
       
public int AllocationId
		{ 
		    get { return this.adjustmentoversallocation.AllocationId; }
			set
			{
			    if (value == this.adjustmentoversallocation.AllocationId) return;
				this.adjustmentoversallocation.AllocationId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("AllocationId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "EntryDataDetails is required")]
       
public int EntryDataDetailsId
		{ 
		    get { return this.adjustmentoversallocation.EntryDataDetailsId; }
			set
			{
			    if (value == this.adjustmentoversallocation.EntryDataDetailsId) return;
				this.adjustmentoversallocation.EntryDataDetailsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EntryDataDetailsId");
			}
		}
     

       
       
public Nullable<int> PreviousItem_Id
		{ 
		    get { return this.adjustmentoversallocation.PreviousItem_Id; }
			set
			{
			    if (value == this.adjustmentoversallocation.PreviousItem_Id) return;
				this.adjustmentoversallocation.PreviousItem_Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("PreviousItem_Id");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Asycuda_ is required")]
       
public int Asycuda_Id
		{ 
		    get { return this.adjustmentoversallocation.Asycuda_Id; }
			set
			{
			    if (value == this.adjustmentoversallocation.Asycuda_Id) return;
				this.adjustmentoversallocation.Asycuda_Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Asycuda_Id");
			}
		}
     

       private EntryDataDetail _EntryDataDetail;
        public  EntryDataDetail EntryDataDetail
		{
		    get
               { 
                  if (this.adjustmentoversallocation != null)
                   {
                       if (_EntryDataDetail != null)
                       {
                           if (this.adjustmentoversallocation.EntryDataDetail !=
                               _EntryDataDetail.DTO)
                           {
                                if (this.adjustmentoversallocation.EntryDataDetail  != null)
                               _EntryDataDetail = new EntryDataDetail(this.adjustmentoversallocation.EntryDataDetail);
                           }
                       }
                       else
                       {
                             if (this.adjustmentoversallocation.EntryDataDetail  != null)
                           _EntryDataDetail = new EntryDataDetail(this.adjustmentoversallocation.EntryDataDetail);
                       }
                   }


             //       if (_EntryDataDetail != null) return _EntryDataDetail;
                       
             //       var i = new EntryDataDetail(){TrackingState = TrackingState.Added};
			//		//if (this.adjustmentoversallocation.EntryDataDetail == null) Debugger.Break();
			//		if (this.adjustmentoversallocation.EntryDataDetail != null)
            //        {
            //           i. = this.adjustmentoversallocation.EntryDataDetail;
            //        }
            //        else
            //        {
            //            this.adjustmentoversallocation.EntryDataDetail = i.;
             //       }
                           
            //        _EntryDataDetail = i;
                     
                    return _EntryDataDetail;
               }
			set
			{
			    if (value == _EntryDataDetail) return;
                _EntryDataDetail = value;
                if(value != null)
                     this.adjustmentoversallocation.EntryDataDetail = value.DTO;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                NotifyPropertyChanged("EntryDataDetail");
			}
		}
        

       private xcuda_Item _xcuda_Item;
        public  xcuda_Item xcuda_Item
		{
		    get
               { 
                  if (this.adjustmentoversallocation != null)
                   {
                       if (_xcuda_Item != null)
                       {
                           if (this.adjustmentoversallocation.xcuda_Item !=
                               _xcuda_Item.DTO)
                           {
                                if (this.adjustmentoversallocation.xcuda_Item  != null)
                               _xcuda_Item = new xcuda_Item(this.adjustmentoversallocation.xcuda_Item);
                           }
                       }
                       else
                       {
                             if (this.adjustmentoversallocation.xcuda_Item  != null)
                           _xcuda_Item = new xcuda_Item(this.adjustmentoversallocation.xcuda_Item);
                       }
                   }


             //       if (_xcuda_Item != null) return _xcuda_Item;
                       
             //       var i = new xcuda_Item(){TrackingState = TrackingState.Added};
			//		//if (this.adjustmentoversallocation.xcuda_Item == null) Debugger.Break();
			//		if (this.adjustmentoversallocation.xcuda_Item != null)
            //        {
            //           i. = this.adjustmentoversallocation.xcuda_Item;
            //        }
            //        else
            //        {
            //            this.adjustmentoversallocation.xcuda_Item = i.;
             //       }
                           
            //        _xcuda_Item = i;
                     
                    return _xcuda_Item;
               }
			set
			{
			    if (value == _xcuda_Item) return;
                _xcuda_Item = value;
                if(value != null)
                     this.adjustmentoversallocation.xcuda_Item = value.DTO;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                NotifyPropertyChanged("xcuda_Item");
			}
		}
        


        ChangeTrackingCollection<DTO.AdjustmentOversAllocation> _changeTracker;    
        public ChangeTrackingCollection<DTO.AdjustmentOversAllocation> ChangeTracker
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


