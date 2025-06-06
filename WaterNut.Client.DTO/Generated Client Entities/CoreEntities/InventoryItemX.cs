﻿// <autogenerated>
//   This file was generated by T4 code generator AllClientEntities.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
//using Newtonsoft.Json;


using Core.Common.Client.DTO;
using TrackableEntities;
using TrackableEntities.Client;

namespace CoreEntities.Client.DTO
{

   // [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace="http://www.insight-software.com/WaterNut")]
    public partial class InventoryItemX : BaseEntity<InventoryItemX>, ITrackable, IEquatable<InventoryItemX>
    {
        [DataMember]
        public string ItemNumber
		{ 
		    get { return _ItemNumber; }
			set
			{
			    if (value == _ItemNumber) return;
				_ItemNumber = value;
				NotifyPropertyChanged();//m => this.ItemNumber
			}
		}
        private string _ItemNumber;

        [DataMember]
        public string Description
		{ 
		    get { return _Description; }
			set
			{
			    if (value == _Description) return;
				_Description = value;
				NotifyPropertyChanged();//m => this.Description
			}
		}
        private string _Description;

        [DataMember]
        public string Category
		{ 
		    get { return _Category; }
			set
			{
			    if (value == _Category) return;
				_Category = value;
				NotifyPropertyChanged();//m => this.Category
			}
		}
        private string _Category;

        [DataMember]
        public string TariffCode
		{ 
		    get { return _TariffCode; }
			set
			{
			    if (value == _TariffCode) return;
				_TariffCode = value;
				NotifyPropertyChanged();//m => this.TariffCode
			}
		}
        private string _TariffCode;

        [DataMember]
        public Nullable<System.DateTime> EntryTimeStamp
		{ 
		    get { return _EntryTimeStamp; }
			set
			{
			    if (value == _EntryTimeStamp) return;
				_EntryTimeStamp = value;
				NotifyPropertyChanged();//m => this.EntryTimeStamp
			}
		}
        private Nullable<System.DateTime> _EntryTimeStamp;

        [DataMember]
        public string SuppUnitCode2
		{ 
		    get { return _SuppUnitCode2; }
			set
			{
			    if (value == _SuppUnitCode2) return;
				_SuppUnitCode2 = value;
				NotifyPropertyChanged();//m => this.SuppUnitCode2
			}
		}
        private string _SuppUnitCode2;

        [DataMember]
        public Nullable<double> SuppQty
		{ 
		    get { return _SuppQty; }
			set
			{
			    if (value == _SuppQty) return;
				_SuppQty = value;
				NotifyPropertyChanged();//m => this.SuppQty
			}
		}
        private Nullable<double> _SuppQty;

        [DataMember]
        public int ApplicationSettingsId
		{ 
		    get { return _ApplicationSettingsId; }
			set
			{
			    if (value == _ApplicationSettingsId) return;
				_ApplicationSettingsId = value;
				NotifyPropertyChanged();//m => this.ApplicationSettingsId
			}
		}
        private int _ApplicationSettingsId;

        [DataMember]
        public int InventoryItemId
		{ 
		    get { return _InventoryItemId; }
			set
			{
			    if (value == _InventoryItemId) return;
				_InventoryItemId = value;
				NotifyPropertyChanged();//m => this.InventoryItemId
			}
		}
        private int _InventoryItemId;

       
        [DataMember]
        public ChangeTrackingCollection<InventoryItemAliasX> InventoryItemAliasEx
		{
		    get { return _InventoryItemAliasEx; }
			set
			{
			    if (Equals(value, _InventoryItemAliasEx)) return;
				_InventoryItemAliasEx = value;
				NotifyPropertyChanged();//m => this.InventoryItemAliasEx
			}
		}
        private ChangeTrackingCollection<InventoryItemAliasX> _InventoryItemAliasEx = new ChangeTrackingCollection<InventoryItemAliasX>();

        [DataMember]
        public ApplicationSettings ApplicationSettings
		{
		    get { return _ApplicationSettings; }
			set
			{
			    if (value == _ApplicationSettings) return;
				_ApplicationSettings = value;
                ApplicationSettingsChangeTracker = _ApplicationSettings == null ? null
                    : new ChangeTrackingCollection<ApplicationSettings> { _ApplicationSettings };
				NotifyPropertyChanged();//m => this.ApplicationSettings
			}
		}
        private ApplicationSettings _ApplicationSettings;
        private ChangeTrackingCollection<ApplicationSettings> ApplicationSettingsChangeTracker { get; set; }

        [DataMember]
        public ChangeTrackingCollection<AsycudaDocumentItem> AsycudaDocumentItem
		{
		    get { return _AsycudaDocumentItem; }
			set
			{
			    if (Equals(value, _AsycudaDocumentItem)) return;
				_AsycudaDocumentItem = value;
				NotifyPropertyChanged();//m => this.AsycudaDocumentItem
			}
		}
        private ChangeTrackingCollection<AsycudaDocumentItem> _AsycudaDocumentItem = new ChangeTrackingCollection<AsycudaDocumentItem>();

   //     [DataMember]
   //     public TrackingState TrackingState { get; set; }

   //     [DataMember]
   //     public ICollection<string> ModifiedProperties { get; set; }
        
    //  [DataMember]//JsonProperty, 
    //	private Guid EntityIdentifier { get; set; }
    
    //	[DataMember]//JsonProperty, 
    //	private Guid _entityIdentity = default(Guid);
    
    	bool IEquatable<InventoryItemX>.Equals(InventoryItemX other)
    	{
    		if (EntityIdentifier != default(Guid))
    			return EntityIdentifier == other.EntityIdentifier;
    		return false;
    	}
    }
}



