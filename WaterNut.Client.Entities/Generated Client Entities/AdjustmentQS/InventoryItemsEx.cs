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
       public partial class InventoryItemsEx: BaseEntity<InventoryItemsEx>
    {
        DTO.InventoryItemsEx inventoryitemsex;
        public InventoryItemsEx(DTO.InventoryItemsEx dto )
        {
              inventoryitemsex = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.InventoryItemsEx>(inventoryitemsex);

        }

        public DTO.InventoryItemsEx DTO
        {
            get
            {
             return inventoryitemsex;
            }
            set
            {
                inventoryitemsex = value;
            }
        }
        


       [RequiredValidationAttribute(ErrorMessage= "ItemNumber is required")]
       
                
                [MaxLength(20, ErrorMessage = "ItemNumber has a max length of 20 letters ")]
public string ItemNumber
		{ 
		    get { return this.inventoryitemsex.ItemNumber; }
			set
			{
			    if (value == this.inventoryitemsex.ItemNumber) return;
				this.inventoryitemsex.ItemNumber = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ItemNumber");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Description is required")]
       
                
                [MaxLength(255, ErrorMessage = "Description has a max length of 255 letters ")]
public string Description
		{ 
		    get { return this.inventoryitemsex.Description; }
			set
			{
			    if (value == this.inventoryitemsex.Description) return;
				this.inventoryitemsex.Description = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Description");
			}
		}
     

       
       
                
                [MaxLength(60, ErrorMessage = "Category has a max length of 60 letters ")]
public string Category
		{ 
		    get { return this.inventoryitemsex.Category; }
			set
			{
			    if (value == this.inventoryitemsex.Category) return;
				this.inventoryitemsex.Category = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Category");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "TariffCode has a max length of 50 letters ")]
public string TariffCode
		{ 
		    get { return this.inventoryitemsex.TariffCode; }
			set
			{
			    if (value == this.inventoryitemsex.TariffCode) return;
				this.inventoryitemsex.TariffCode = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("TariffCode");
			}
		}
     

       
       
public Nullable<System.DateTime> EntryTimeStamp
		{ 
		    get { return this.inventoryitemsex.EntryTimeStamp; }
			set
			{
			    if (value == this.inventoryitemsex.EntryTimeStamp) return;
				this.inventoryitemsex.EntryTimeStamp = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("EntryTimeStamp");
			}
		}
     

       
       
                
                [MaxLength(50, ErrorMessage = "SuppUnitCode2 has a max length of 50 letters ")]
public string SuppUnitCode2
		{ 
		    get { return this.inventoryitemsex.SuppUnitCode2; }
			set
			{
			    if (value == this.inventoryitemsex.SuppUnitCode2) return;
				this.inventoryitemsex.SuppUnitCode2 = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("SuppUnitCode2");
			}
		}
     

       
       [NumberValidationAttribute]
public Nullable<double> SuppQty
		{ 
		    get { return this.inventoryitemsex.SuppQty; }
			set
			{
			    if (value == this.inventoryitemsex.SuppQty) return;
				this.inventoryitemsex.SuppQty = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("SuppQty");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "ApplicationSettings is required")]
       
public int ApplicationSettingsId
		{ 
		    get { return this.inventoryitemsex.ApplicationSettingsId; }
			set
			{
			    if (value == this.inventoryitemsex.ApplicationSettingsId) return;
				this.inventoryitemsex.ApplicationSettingsId = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("ApplicationSettingsId");
			}
		}
     

        ObservableCollection<InventoryItemAliasEx> _InventoryItemAliasExes = null;
        public  ObservableCollection<InventoryItemAliasEx> InventoryItemAliasExes
		{
            
		    get 
				{ 
					if(_InventoryItemAliasExes != null) return _InventoryItemAliasExes;
					//if (this.inventoryitemsex.InventoryItemAliasExes == null) Debugger.Break();
					if(this.inventoryitemsex.InventoryItemAliasExes != null)
					{
						_InventoryItemAliasExes = new ObservableCollection<InventoryItemAliasEx>(this.inventoryitemsex.InventoryItemAliasExes.Select(x => new InventoryItemAliasEx(x)));
					}
					
						_InventoryItemAliasExes.CollectionChanged += InventoryItemAliasExes_CollectionChanged; 
					
					return _InventoryItemAliasExes; 
				}
			set
			{
			    if (Equals(value, _InventoryItemAliasExes)) return;
				if (value != null)
					this.inventoryitemsex.InventoryItemAliasExes = new ChangeTrackingCollection<DTO.InventoryItemAliasEx>(value.Select(x => x.DTO).ToList());
                _InventoryItemAliasExes = value;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				if (_InventoryItemAliasExes != null)
				_InventoryItemAliasExes.CollectionChanged += InventoryItemAliasExes_CollectionChanged;               
				NotifyPropertyChanged("InventoryItemAliasExes");
			}
		}
        
        void InventoryItemAliasExes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (InventoryItemAliasEx itm in e.NewItems)
                    {
                        if (itm != null)
                        inventoryitemsex.InventoryItemAliasExes.Add(itm.DTO);
                    }
                    if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (InventoryItemAliasEx itm in e.OldItems)
                    {
                        if (itm != null)
                        inventoryitemsex.InventoryItemAliasExes.Remove(itm.DTO);
                    }
					if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                    break;
                
            }
        }

        ObservableCollection<EntryDataDetail> _EntryDataDetails = null;
        public  ObservableCollection<EntryDataDetail> EntryDataDetails
		{
            
		    get 
				{ 
					if(_EntryDataDetails != null) return _EntryDataDetails;
					//if (this.inventoryitemsex.EntryDataDetails == null) Debugger.Break();
					if(this.inventoryitemsex.EntryDataDetails != null)
					{
						_EntryDataDetails = new ObservableCollection<EntryDataDetail>(this.inventoryitemsex.EntryDataDetails.Select(x => new EntryDataDetail(x)));
					}
					
						_EntryDataDetails.CollectionChanged += EntryDataDetails_CollectionChanged; 
					
					return _EntryDataDetails; 
				}
			set
			{
			    if (Equals(value, _EntryDataDetails)) return;
				if (value != null)
					this.inventoryitemsex.EntryDataDetails = new ChangeTrackingCollection<DTO.EntryDataDetail>(value.Select(x => x.DTO).ToList());
                _EntryDataDetails = value;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				if (_EntryDataDetails != null)
				_EntryDataDetails.CollectionChanged += EntryDataDetails_CollectionChanged;               
				NotifyPropertyChanged("EntryDataDetails");
			}
		}
        
        void EntryDataDetails_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (EntryDataDetail itm in e.NewItems)
                    {
                        if (itm != null)
                        inventoryitemsex.EntryDataDetails.Add(itm.DTO);
                    }
                    if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (EntryDataDetail itm in e.OldItems)
                    {
                        if (itm != null)
                        inventoryitemsex.EntryDataDetails.Remove(itm.DTO);
                    }
					if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                    break;
                
            }
        }

        ObservableCollection<AdjustmentOver> _AdjustmentOvers = null;
        public  ObservableCollection<AdjustmentOver> AdjustmentOvers
		{
            
		    get 
				{ 
					if(_AdjustmentOvers != null) return _AdjustmentOvers;
					//if (this.inventoryitemsex.AdjustmentOvers == null) Debugger.Break();
					if(this.inventoryitemsex.AdjustmentOvers != null)
					{
						_AdjustmentOvers = new ObservableCollection<AdjustmentOver>(this.inventoryitemsex.AdjustmentOvers.Select(x => new AdjustmentOver(x)));
					}
					
						_AdjustmentOvers.CollectionChanged += AdjustmentOvers_CollectionChanged; 
					
					return _AdjustmentOvers; 
				}
			set
			{
			    if (Equals(value, _AdjustmentOvers)) return;
				if (value != null)
					this.inventoryitemsex.AdjustmentOvers = new ChangeTrackingCollection<DTO.AdjustmentOver>(value.Select(x => x.DTO).ToList());
                _AdjustmentOvers = value;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				if (_AdjustmentOvers != null)
				_AdjustmentOvers.CollectionChanged += AdjustmentOvers_CollectionChanged;               
				NotifyPropertyChanged("AdjustmentOvers");
			}
		}
        
        void AdjustmentOvers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (AdjustmentOver itm in e.NewItems)
                    {
                        if (itm != null)
                        inventoryitemsex.AdjustmentOvers.Add(itm.DTO);
                    }
                    if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (AdjustmentOver itm in e.OldItems)
                    {
                        if (itm != null)
                        inventoryitemsex.AdjustmentOvers.Remove(itm.DTO);
                    }
					if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                    break;
                
            }
        }


        ChangeTrackingCollection<DTO.InventoryItemsEx> _changeTracker;    
        public ChangeTrackingCollection<DTO.InventoryItemsEx> ChangeTracker
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

