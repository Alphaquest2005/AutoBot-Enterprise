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
       public partial class InventoryItemAliasX: BaseEntity<InventoryItemAliasX>
    {
        DTO.InventoryItemAliasX inventoryitemaliasx;
        public InventoryItemAliasX(DTO.InventoryItemAliasX dto )
        {
              inventoryitemaliasx = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.InventoryItemAliasX>(inventoryitemaliasx);

        }

        public DTO.InventoryItemAliasX DTO
        {
            get
            {
             return inventoryitemaliasx;
            }
            set
            {
                inventoryitemaliasx = value;
            }
        }
       [RequiredValidationAttribute(ErrorMessage= "Alias is required")]
       
public int AliasId
		{ 
		    get { return this.inventoryitemaliasx.AliasId; }
			set
			{
			    if (value == this.inventoryitemaliasx.AliasId) return;
				this.inventoryitemaliasx.AliasId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("AliasId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ApplicationSettings is required")]
       
public int ApplicationSettingsId
		{ 
		    get { return this.inventoryitemaliasx.ApplicationSettingsId; }
			set
			{
			    if (value == this.inventoryitemaliasx.ApplicationSettingsId) return;
				this.inventoryitemaliasx.ApplicationSettingsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ApplicationSettingsId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "InventoryItem is required")]
       
public int InventoryItemId
		{ 
		    get { return this.inventoryitemaliasx.InventoryItemId; }
			set
			{
			    if (value == this.inventoryitemaliasx.InventoryItemId) return;
				this.inventoryitemaliasx.InventoryItemId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("InventoryItemId");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ItemNumber is required")]
       
                
                [MaxLength(20, ErrorMessage = "ItemNumber has a max length of 20 letters ")]
public string ItemNumber
		{ 
		    get { return this.inventoryitemaliasx.ItemNumber; }
			set
			{
			    if (value == this.inventoryitemaliasx.ItemNumber) return;
				this.inventoryitemaliasx.ItemNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ItemNumber");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "AliasName is required")]
       
                
                [MaxLength(20, ErrorMessage = "AliasName has a max length of 20 letters ")]
public string AliasName
		{ 
		    get { return this.inventoryitemaliasx.AliasName; }
			set
			{
			    if (value == this.inventoryitemaliasx.AliasName) return;
				this.inventoryitemaliasx.AliasName = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("AliasName");
			}
		}
     

       private InventoryItemX _InventoryItemsEx;
        public  InventoryItemX InventoryItemsEx
		{
		    get
               { 
                  if (this.inventoryitemaliasx != null)
                   {
                       if (_InventoryItemsEx != null)
                       {
                           if (this.inventoryitemaliasx.InventoryItemsEx !=
                               _InventoryItemsEx.DTO)
                           {
                                if (this.inventoryitemaliasx.InventoryItemsEx  != null)
                               _InventoryItemsEx = new InventoryItemX(this.inventoryitemaliasx.InventoryItemsEx);
                           }
                       }
                       else
                       {
                             if (this.inventoryitemaliasx.InventoryItemsEx  != null)
                           _InventoryItemsEx = new InventoryItemX(this.inventoryitemaliasx.InventoryItemsEx);
                       }
                   }


             //       if (_InventoryItemsEx != null) return _InventoryItemsEx;
                       
             //       var i = new InventoryItemX(){TrackingState = TrackingState.Added};
			//		//if (this.inventoryitemaliasx.InventoryItemsEx == null) Debugger.Break();
			//		if (this.inventoryitemaliasx.InventoryItemsEx != null)
            //        {
            //           i. = this.inventoryitemaliasx.InventoryItemsEx;
            //        }
            //        else
            //        {
            //            this.inventoryitemaliasx.InventoryItemsEx = i.;
             //       }
                           
            //        _InventoryItemsEx = i;
                     
                    return _InventoryItemsEx;
               }
			set
			{
			    if (value == _InventoryItemsEx) return;
                _InventoryItemsEx = value;
                if(value != null)
                     this.inventoryitemaliasx.InventoryItemsEx = value.DTO;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                NotifyPropertyChanged("InventoryItemsEx");
			}
		}
        


        ChangeTrackingCollection<DTO.InventoryItemAliasX> _changeTracker;    
        public ChangeTrackingCollection<DTO.InventoryItemAliasX> ChangeTracker
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

