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
    public partial class EntryDataExTotals : BaseEntity<EntryDataExTotals>, ITrackable 
    {
        [DataMember]
        public string EntryDataId 
        {
            get
            {
                return _entrydataid;
            }
            set
            {
                _entrydataid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _entrydataid;
        [DataMember]
        public Nullable<double> Total 
        {
            get
            {
                return _total;
            }
            set
            {
                _total = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<double> _total;
        [DataMember]
        public Nullable<double> AllocatedTotal 
        {
            get
            {
                return _allocatedtotal;
            }
            set
            {
                _allocatedtotal = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<double> _allocatedtotal;
        [DataMember]
        public Nullable<int> TotalLines 
        {
            get
            {
                return _totallines;
            }
            set
            {
                _totallines = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<int> _totallines;
        [DataMember]
        public Nullable<double> Tax 
        {
            get
            {
                return _tax;
            }
            set
            {
                _tax = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<double> _tax;
        [DataMember]
        public Nullable<int> ClassifiedLines 
        {
            get
            {
                return _classifiedlines;
            }
            set
            {
                _classifiedlines = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<int> _classifiedlines;
        [DataMember]
        public Nullable<int> LicenseLines 
        {
            get
            {
                return _licenselines;
            }
            set
            {
                _licenselines = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<int> _licenselines;
        [DataMember]
        public int EntryData_Id 
        {
            get
            {
                return _entrydata_id;
            }
            set
            {
                _entrydata_id = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        int _entrydata_id;
        [DataMember]
        public EntryData EntryData { get; set; }

 //       [DataMember]
 //       public TrackingState TrackingState { get; set; }
 //       [DataMember]
 //       public ICollection<string> ModifiedProperties { get; set; }
//        [DataMember]//JsonProperty,
 //       private Guid EntityIdentifier { get; set; }
    }
}

