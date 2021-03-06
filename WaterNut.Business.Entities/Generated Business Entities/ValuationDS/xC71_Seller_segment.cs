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

namespace ValuationDS.Business.Entities
{

    //[JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace="http://www.insight-software.com/WaterNut")]
    public partial class xC71_Seller_segment : BaseEntity<xC71_Seller_segment>, ITrackable 
    {
        [DataMember]
        public string Name 
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _name;
        [DataMember]
        public string Address 
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _address;
        [DataMember]
        public int Identification_segment_Id 
        {
            get
            {
                return _identification_segment_id;
            }
            set
            {
                _identification_segment_id = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        int _identification_segment_id;
        [DataMember]
        public string CountryCode 
        {
            get
            {
                return _countrycode;
            }
            set
            {
                _countrycode = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _countrycode;
        [DataMember]
        public xC71_Identification_segment xC71_Identification_segment { get; set; }

 //       [DataMember]
 //       public TrackingState TrackingState { get; set; }
 //       [DataMember]
 //       public ICollection<string> ModifiedProperties { get; set; }
//        [DataMember]//JsonProperty,
 //       private Guid EntityIdentifier { get; set; }
    }
}


