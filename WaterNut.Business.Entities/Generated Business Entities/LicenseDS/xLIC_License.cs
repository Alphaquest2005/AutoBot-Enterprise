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
    public partial class xLIC_License : BaseEntity<xLIC_License>, ITrackable 
    {
        partial void AutoGenStartUp() //xLIC_License()
        {
            this.xLIC_Lic_item_segment = new List<xLIC_Lic_item_segment>();
        }

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
        public xLIC_General_segment xLIC_General_segment { get; set; }
        [DataMember]
        public List<xLIC_Lic_item_segment> xLIC_Lic_item_segment { get; set; }

 //       [DataMember]
 //       public TrackingState TrackingState { get; set; }
 //       [DataMember]
 //       public ICollection<string> ModifiedProperties { get; set; }
//        [DataMember]//JsonProperty,
 //       private Guid EntityIdentifier { get; set; }
    }
}


