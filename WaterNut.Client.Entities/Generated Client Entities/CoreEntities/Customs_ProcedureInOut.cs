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
       public partial class Customs_ProcedureInOut: BaseEntity<Customs_ProcedureInOut>
    {
        DTO.Customs_ProcedureInOut customs_procedureinout;
        public Customs_ProcedureInOut(DTO.Customs_ProcedureInOut dto )
        {
              customs_procedureinout = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.Customs_ProcedureInOut>(customs_procedureinout);

        }

        public DTO.Customs_ProcedureInOut DTO
        {
            get
            {
             return customs_procedureinout;
            }
            set
            {
                customs_procedureinout = value;
            }
        }
       [RequiredValidationAttribute(ErrorMessage= " is required")]
       
public int Id
		{ 
		    get { return this.customs_procedureinout.Id; }
			set
			{
			    if (value == this.customs_procedureinout.Id) return;
				this.customs_procedureinout.Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Id");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "WarehouseCustomsProcedure is required")]
       
public int WarehouseCustomsProcedureId
		{ 
		    get { return this.customs_procedureinout.WarehouseCustomsProcedureId; }
			set
			{
			    if (value == this.customs_procedureinout.WarehouseCustomsProcedureId) return;
				this.customs_procedureinout.WarehouseCustomsProcedureId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("WarehouseCustomsProcedureId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ExwarehouseCustomsProcedure is required")]
       
public int ExwarehouseCustomsProcedureId
		{ 
		    get { return this.customs_procedureinout.ExwarehouseCustomsProcedureId; }
			set
			{
			    if (value == this.customs_procedureinout.ExwarehouseCustomsProcedureId) return;
				this.customs_procedureinout.ExwarehouseCustomsProcedureId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ExwarehouseCustomsProcedureId");
			}
		}
     

       private Customs_Procedure _InCustomsProcedure;
        public  Customs_Procedure InCustomsProcedure
		{
		    get
               { 
                  if (this.customs_procedureinout != null)
                   {
                       if (_InCustomsProcedure != null)
                       {
                           if (this.customs_procedureinout.InCustomsProcedure !=
                               _InCustomsProcedure.DTO)
                           {
                                if (this.customs_procedureinout.InCustomsProcedure  != null)
                               _InCustomsProcedure = new Customs_Procedure(this.customs_procedureinout.InCustomsProcedure);
                           }
                       }
                       else
                       {
                             if (this.customs_procedureinout.InCustomsProcedure  != null)
                           _InCustomsProcedure = new Customs_Procedure(this.customs_procedureinout.InCustomsProcedure);
                       }
                   }


             //       if (_InCustomsProcedure != null) return _InCustomsProcedure;
                       
             //       var i = new Customs_Procedure(){TrackingState = TrackingState.Added};
			//		//if (this.customs_procedureinout.InCustomsProcedure == null) Debugger.Break();
			//		if (this.customs_procedureinout.InCustomsProcedure != null)
            //        {
            //           i. = this.customs_procedureinout.InCustomsProcedure;
            //        }
            //        else
            //        {
            //            this.customs_procedureinout.InCustomsProcedure = i.;
             //       }
                           
            //        _InCustomsProcedure = i;
                     
                    return _InCustomsProcedure;
               }
			set
			{
			    if (value == _InCustomsProcedure) return;
                _InCustomsProcedure = value;
                if(value != null)
                     this.customs_procedureinout.InCustomsProcedure = value.DTO;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                NotifyPropertyChanged("InCustomsProcedure");
			}
		}
        

       private Customs_Procedure _OutCustomsProcedure;
        public  Customs_Procedure OutCustomsProcedure
		{
		    get
               { 
                  if (this.customs_procedureinout != null)
                   {
                       if (_OutCustomsProcedure != null)
                       {
                           if (this.customs_procedureinout.OutCustomsProcedure !=
                               _OutCustomsProcedure.DTO)
                           {
                                if (this.customs_procedureinout.OutCustomsProcedure  != null)
                               _OutCustomsProcedure = new Customs_Procedure(this.customs_procedureinout.OutCustomsProcedure);
                           }
                       }
                       else
                       {
                             if (this.customs_procedureinout.OutCustomsProcedure  != null)
                           _OutCustomsProcedure = new Customs_Procedure(this.customs_procedureinout.OutCustomsProcedure);
                       }
                   }


             //       if (_OutCustomsProcedure != null) return _OutCustomsProcedure;
                       
             //       var i = new Customs_Procedure(){TrackingState = TrackingState.Added};
			//		//if (this.customs_procedureinout.OutCustomsProcedure == null) Debugger.Break();
			//		if (this.customs_procedureinout.OutCustomsProcedure != null)
            //        {
            //           i. = this.customs_procedureinout.OutCustomsProcedure;
            //        }
            //        else
            //        {
            //            this.customs_procedureinout.OutCustomsProcedure = i.;
             //       }
                           
            //        _OutCustomsProcedure = i;
                     
                    return _OutCustomsProcedure;
               }
			set
			{
			    if (value == _OutCustomsProcedure) return;
                _OutCustomsProcedure = value;
                if(value != null)
                     this.customs_procedureinout.OutCustomsProcedure = value.DTO;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                NotifyPropertyChanged("OutCustomsProcedure");
			}
		}
        


        ChangeTrackingCollection<DTO.Customs_ProcedureInOut> _changeTracker;    
        public ChangeTrackingCollection<DTO.Customs_ProcedureInOut> ChangeTracker
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

