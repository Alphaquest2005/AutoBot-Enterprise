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
       public partial class InventoryItemX: BaseEntity<InventoryItemX>
    {
        DTO.InventoryItemX inventoryitemx;
        public InventoryItemX(DTO.InventoryItemX dto )
        {
              inventoryitemx = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.InventoryItemX>(inventoryitemx);

        }

        public DTO.InventoryItemX DTO
        {
            get
            {
             return inventoryitemx;
            }
            set
            {
                inventoryitemx = value;
            }
        }
        


       [RequiredValidationAttribute(ErrorMessage= "ItemNumber is required")]
       
                
                [MaxLength(20, ErrorMessage = "ItemNumber has a max length of 20 letters ")]
public string ItemNumber
		{ 
		    get { return this.inventoryitemx.ItemNumber; }
			set
			{
			    if (value == this.inventoryitemx.ItemNumber) return;
				this.inventoryitemx.ItemNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ItemNumber");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Description is required")]
       
                
                [MaxLength(255, ErrorMessage = "Description has a max length of 255 letters ")]
public string Description
		{ 
		    get { return this.inventoryitemx.Description; }
			set
			{
			    if (value == this.inventoryitemx.Description) return;
				this.inventoryitemx.Description = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Description");
			}
		}
     

       
       
                
                [MaxLength(60, ErrorMessage = "Category has a max length of 60 letters ")]
public string Category
		{ 
		    get { return this.inventoryitemx.Category; }
			set
			{
			    if (value == this.inventoryitemx.Category) return;
				this.inventoryitemx.Category = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Category");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "TariffCode has a max length of 50 letters ")]
public string TariffCode
		{ 
		    get { return this.inventoryitemx.TariffCode; }
			set
			{
			    if (value == this.inventoryitemx.TariffCode) return;
				this.inventoryitemx.TariffCode = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("TariffCode");
			}
		}
     

       
       
public Nullable<System.DateTime> EntryTimeStamp
		{ 
		    get { return this.inventoryitemx.EntryTimeStamp; }
			set
			{
			    if (value == this.inventoryitemx.EntryTimeStamp) return;
				this.inventoryitemx.EntryTimeStamp = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EntryTimeStamp");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "SuppUnitCode2 has a max length of 50 letters ")]
public string SuppUnitCode2
		{ 
		    get { return this.inventoryitemx.SuppUnitCode2; }
			set
			{
			    if (value == this.inventoryitemx.SuppUnitCode2) return;
				this.inventoryitemx.SuppUnitCode2 = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("SuppUnitCode2");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> SuppQty
		{ 
		    get { return this.inventoryitemx.SuppQty; }
			set
			{
			    if (value == this.inventoryitemx.SuppQty) return;
				this.inventoryitemx.SuppQty = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("SuppQty");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ApplicationSettings is required")]
       
public int ApplicationSettingsId
		{ 
		    get { return this.inventoryitemx.ApplicationSettingsId; }
			set
			{
			    if (value == this.inventoryitemx.ApplicationSettingsId) return;
				this.inventoryitemx.ApplicationSettingsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ApplicationSettingsId");
			}
		}
     

        ObservableCollection<InventoryItemAliasX> _InventoryItemAliasEx = null;
        public  ObservableCollection<InventoryItemAliasX> InventoryItemAliasEx
		{
            
		    get 
				{ 
					if(_InventoryItemAliasEx != null) return _InventoryItemAliasEx;
					//if (this.inventoryitemx.InventoryItemAliasEx == null) Debugger.Break();
					if(this.inventoryitemx.InventoryItemAliasEx != null)
					{
						_InventoryItemAliasEx = new ObservableCollection<InventoryItemAliasX>(this.inventoryitemx.InventoryItemAliasEx.Select(x => new InventoryItemAliasX(x)));
					}
					
						_InventoryItemAliasEx.CollectionChanged += InventoryItemAliasEx_CollectionChanged; 
					
					return _InventoryItemAliasEx; 
				}
			set
			{
			    if (Equals(value, _InventoryItemAliasEx)) return;
				if (value != null)
					this.inventoryitemx.InventoryItemAliasEx = new ChangeTrackingCollection<DTO.InventoryItemAliasX>(value.Select(x => x.DTO).ToList());
                _InventoryItemAliasEx = value;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				if (_InventoryItemAliasEx != null)
				_InventoryItemAliasEx.CollectionChanged += InventoryItemAliasEx_CollectionChanged;               
				NotifyPropertyChanged("InventoryItemAliasEx");
			}
		}
        
        void InventoryItemAliasEx_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (InventoryItemAliasX itm in e.NewItems)
                    {
                        if (itm != null)
                        inventoryitemx.InventoryItemAliasEx.Add(itm.DTO);
                    }
                    if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (InventoryItemAliasX itm in e.OldItems)
                    {
                        if (itm != null)
                        inventoryitemx.InventoryItemAliasEx.Remove(itm.DTO);
                    }
					if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                    break;
                
            }
        }

       private ApplicationSettings _ApplicationSettings;
        public  ApplicationSettings ApplicationSettings
		{
		    get
               { 
                  if (this.inventoryitemx != null)
                   {
                       if (_ApplicationSettings != null)
                       {
                           if (this.inventoryitemx.ApplicationSettings !=
                               _ApplicationSettings.DTO)
                           {
                                if (this.inventoryitemx.ApplicationSettings  != null)
                               _ApplicationSettings = new ApplicationSettings(this.inventoryitemx.ApplicationSettings);
                           }
                       }
                       else
                       {
                             if (this.inventoryitemx.ApplicationSettings  != null)
                           _ApplicationSettings = new ApplicationSettings(this.inventoryitemx.ApplicationSettings);
                       }
                   }


             //       if (_ApplicationSettings != null) return _ApplicationSettings;
                       
             //       var i = new ApplicationSettings(){TrackingState = TrackingState.Added};
			//		//if (this.inventoryitemx.ApplicationSettings == null) Debugger.Break();
			//		if (this.inventoryitemx.ApplicationSettings != null)
            //        {
            //           i. = this.inventoryitemx.ApplicationSettings;
            //        }
            //        else
            //        {
            //            this.inventoryitemx.ApplicationSettings = i.;
             //       }
                           
            //        _ApplicationSettings = i;
                     
                    return _ApplicationSettings;
               }
			set
			{
			    if (value == _ApplicationSettings) return;
                _ApplicationSettings = value;
                if(value != null)
                     this.inventoryitemx.ApplicationSettings = value.DTO;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                NotifyPropertyChanged("ApplicationSettings");
			}
		}
        

        ObservableCollection<AsycudaDocumentItem> _AsycudaDocumentItem = null;
        public  ObservableCollection<AsycudaDocumentItem> AsycudaDocumentItem
		{
            
		    get 
				{ 
					if(_AsycudaDocumentItem != null) return _AsycudaDocumentItem;
					//if (this.inventoryitemx.AsycudaDocumentItem == null) Debugger.Break();
					if(this.inventoryitemx.AsycudaDocumentItem != null)
					{
						_AsycudaDocumentItem = new ObservableCollection<AsycudaDocumentItem>(this.inventoryitemx.AsycudaDocumentItem.Select(x => new AsycudaDocumentItem(x)));
					}
					
						_AsycudaDocumentItem.CollectionChanged += AsycudaDocumentItem_CollectionChanged; 
					
					return _AsycudaDocumentItem; 
				}
			set
			{
			    if (Equals(value, _AsycudaDocumentItem)) return;
				if (value != null)
					this.inventoryitemx.AsycudaDocumentItem = new ChangeTrackingCollection<DTO.AsycudaDocumentItem>(value.Select(x => x.DTO).ToList());
                _AsycudaDocumentItem = value;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				if (_AsycudaDocumentItem != null)
				_AsycudaDocumentItem.CollectionChanged += AsycudaDocumentItem_CollectionChanged;               
				NotifyPropertyChanged("AsycudaDocumentItem");
			}
		}
        
        void AsycudaDocumentItem_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (AsycudaDocumentItem itm in e.NewItems)
                    {
                        if (itm != null)
                        inventoryitemx.AsycudaDocumentItem.Add(itm.DTO);
                    }
                    if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (AsycudaDocumentItem itm in e.OldItems)
                    {
                        if (itm != null)
                        inventoryitemx.AsycudaDocumentItem.Remove(itm.DTO);
                    }
					if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                    break;
                
            }
        }


        ChangeTrackingCollection<DTO.InventoryItemX> _changeTracker;    
        public ChangeTrackingCollection<DTO.InventoryItemX> ChangeTracker
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

