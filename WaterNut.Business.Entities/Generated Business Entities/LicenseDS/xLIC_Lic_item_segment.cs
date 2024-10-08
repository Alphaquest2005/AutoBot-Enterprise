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

namespace LicenseDS.Business.Entities
{

    //[JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace="http://www.insight-software.com/WaterNut")]
    public partial class xLIC_Lic_item_segment : BaseEntity<xLIC_Lic_item_segment>, ITrackable 
    {
        [DataMember]
        public string Description 
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _description;
        [DataMember]
        public string Commodity_code 
        {
            get
            {
                return _commodity_code;
            }
            set
            {
                _commodity_code = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _commodity_code;
        [DataMember]
        public double Quantity_requested 
        {
            get
            {
                return _quantity_requested;
            }
            set
            {
                _quantity_requested = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        double _quantity_requested;
        [DataMember]
        public string Origin 
        {
            get
            {
                return _origin;
            }
            set
            {
                _origin = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _origin;
        [DataMember]
        public string Unit_of_measurement 
        {
            get
            {
                return _unit_of_measurement;
            }
            set
            {
                _unit_of_measurement = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _unit_of_measurement;
        [DataMember]
        public double Quantity_to_approve 
        {
            get
            {
                return _quantity_to_approve;
            }
            set
            {
                _quantity_to_approve = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        double _quantity_to_approve;
        [DataMember]
        public int LicenseId 
        {
            get
            {
                return _licenseid;
            }
            set
            {
                _licenseid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        int _licenseid;
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
        public xLIC_License xLIC_License { get; set; }
        [DataMember]
        public TODO_LicenceAvailableQty TODO_LicenceAvailableQty { get; set; }

 //       [DataMember]
 //       public TrackingState TrackingState { get; set; }
 //       [DataMember]
 //       public ICollection<string> ModifiedProperties { get; set; }
//        [DataMember]//JsonProperty,
 //       private Guid EntityIdentifier { get; set; }
    }
}


