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

namespace OCR.Business.Entities
{

    //[JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace="http://www.insight-software.com/WaterNut")]
    public partial class Lines : BaseEntity<Lines>, ITrackable 
    {
        partial void AutoGenStartUp() //Lines()
        {
            this.Fields = new List<Fields>();
        }

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
        public int PartId 
        {
            get
            {
                return _partid;
            }
            set
            {
                _partid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        int _partid;
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
        public bool MultiLine 
        {
            get
            {
                return _multiline;
            }
            set
            {
                _multiline = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        bool _multiline;
        [DataMember]
        public int RegExId 
        {
            get
            {
                return _regexid;
            }
            set
            {
                _regexid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        int _regexid;
        [DataMember]
        public List<Fields> Fields { get; set; }
        [DataMember]
        public Parts Parts { get; set; }
        [DataMember]
        public RegularExpressions RegularExpressions { get; set; }

 //       [DataMember]
 //       public TrackingState TrackingState { get; set; }
 //       [DataMember]
 //       public ICollection<string> ModifiedProperties { get; set; }
//        [DataMember]//JsonProperty,
 //       private Guid EntityIdentifier { get; set; }
    }
}

