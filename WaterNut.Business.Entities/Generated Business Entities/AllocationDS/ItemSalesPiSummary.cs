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

namespace AllocationDS.Business.Entities
{
    //[JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace="http://www.insight-software.com/WaterNut")]
    public partial class ItemSalesPiSummary : BaseEntity<ItemSalesPiSummary>, ITrackable 
    {
        [DataMember]
        public Nullable<int> PreviousItem_Id 
        {
            get
            {
                return _previousitem_id;
            }
            set
            {
                _previousitem_id = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<int> _previousitem_id;
        [DataMember]
        public string DutyFreePaid 
        {
            get
            {
                return _dutyfreepaid;
            }
            set
            {
                _dutyfreepaid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _dutyfreepaid;
        [DataMember]
        public int pLineNumber 
        {
            get
            {
                return _plinenumber;
            }
            set
            {
                _plinenumber = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        int _plinenumber;
        [DataMember]
        public double pQtyAllocated 
        {
            get
            {
                return _pqtyallocated;
            }
            set
            {
                _pqtyallocated = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        double _pqtyallocated;
        [DataMember]
        public string pCNumber 
        {
            get
            {
                return _pcnumber;
            }
            set
            {
                _pcnumber = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _pcnumber;
        [DataMember]
        public Nullable<System.DateTime> pRegistrationDate 
        {
            get
            {
                return _pregistrationdate;
            }
            set
            {
                _pregistrationdate = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<System.DateTime> _pregistrationdate;
        [DataMember]
        public Nullable<System.DateTime> pAssessmentDate 
        {
            get
            {
                return _passessmentdate;
            }
            set
            {
                _passessmentdate = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<System.DateTime> _passessmentdate;
        [DataMember]
        public Nullable<double> QtyAllocated 
        {
            get
            {
                return _qtyallocated;
            }
            set
            {
                _qtyallocated = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<double> _qtyallocated;
        [DataMember]
        public System.DateTime EntryDataDate 
        {
            get
            {
                return _entrydatadate;
            }
            set
            {
                _entrydatadate = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        System.DateTime _entrydatadate;
        [DataMember]
        public int ApplicationSettingsId 
        {
            get
            {
                return _applicationsettingsid;
            }
            set
            {
                _applicationsettingsid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        int _applicationsettingsid;
        [DataMember]
        public string ItemNumber 
        {
            get
            {
                return _itemnumber;
            }
            set
            {
                _itemnumber = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _itemnumber;
        [DataMember]
        public string Type 
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _type;

 //       [DataMember]
 //       public TrackingState TrackingState { get; set; }
 //       [DataMember]
 //       public ICollection<string> ModifiedProperties { get; set; }
//        [DataMember]//JsonProperty,
 //       private Guid EntityIdentifier { get; set; }
    }
}

