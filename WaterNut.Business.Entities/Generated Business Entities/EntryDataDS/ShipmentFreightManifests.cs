﻿// <autogenerated>
//   This file was generated by T4 code generator AllBusinessEntities.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
//using Newtonsoft.Json;

using Core.Common.Business.Entities;
using WaterNut.Interfaces;
using TrackableEntities;

namespace EntryDataDS.Business.Entities
{

    //[JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace="http://www.insight-software.com/WaterNut")]
    public partial class ShipmentFreightManifests : BaseEntity<ShipmentFreightManifests>, ITrackable 
    {
        [DataMember]
        public int Id 
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        int _id;
        [DataMember]
        public int FreightDetailId 
        {
            get
            {
                return _freightdetailid;
            }
            set
            {
                _freightdetailid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        int _freightdetailid;
        [DataMember]
        public int FreightId 
        {
            get
            {
                return _freightid;
            }
            set
            {
                _freightid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        int _freightid;
        [DataMember]
        public int ManifestId 
        {
            get
            {
                return _manifestid;
            }
            set
            {
                _manifestid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        int _manifestid;
        [DataMember]
        public string RegistrationNumber 
        {
            get
            {
                return _registrationnumber;
            }
            set
            {
                _registrationnumber = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _registrationnumber;
        [DataMember]
        public string InvoiceNumber 
        {
            get
            {
                return _invoicenumber;
            }
            set
            {
                _invoicenumber = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _invoicenumber;
        [DataMember]
        public string WarehouseCode 
        {
            get
            {
                return _warehousecode;
            }
            set
            {
                _warehousecode = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _warehousecode;
        [DataMember]
        public ShipmentFreight ShipmentFreight { get; set; }
        [DataMember]
        public ShipmentManifest ShipmentManifest { get; set; }

 //       [DataMember]
 //       public TrackingState TrackingState { get; set; }
 //       [DataMember]
 //       public ICollection<string> ModifiedProperties { get; set; }
//        [DataMember]//JsonProperty,
 //       private Guid EntityIdentifier { get; set; }
    }
}


