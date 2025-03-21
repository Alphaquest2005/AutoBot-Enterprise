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

namespace DocumentDS.Business.Entities
{

    //[JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace="http://www.insight-software.com/WaterNut")]
    public partial class Container : BaseEntity<Container>, ITrackable 
    {
        [DataMember]
        public string Container_identity 
        {
            get
            {
                return _container_identity;
            }
            set
            {
                _container_identity = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _container_identity;
        [DataMember]
        public string Container_type 
        {
            get
            {
                return _container_type;
            }
            set
            {
                _container_type = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _container_type;
        [DataMember]
        public string Empty_full_indicator 
        {
            get
            {
                return _empty_full_indicator;
            }
            set
            {
                _empty_full_indicator = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _empty_full_indicator;
        [DataMember]
        public Nullable<double> Gross_weight 
        {
            get
            {
                return _gross_weight;
            }
            set
            {
                _gross_weight = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<double> _gross_weight;
        [DataMember]
        public string Goods_description 
        {
            get
            {
                return _goods_description;
            }
            set
            {
                _goods_description = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _goods_description;
        [DataMember]
        public int AsycudaDocumentSetId 
        {
            get
            {
                return _asycudadocumentsetid;
            }
            set
            {
                _asycudadocumentsetid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        int _asycudadocumentsetid;
        [DataMember]
        public Nullable<double> TotalValue 
        {
            get
            {
                return _totalvalue;
            }
            set
            {
                _totalvalue = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<double> _totalvalue;
        [DataMember]
        public Nullable<System.DateTime> ShipDate 
        {
            get
            {
                return _shipdate;
            }
            set
            {
                _shipdate = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<System.DateTime> _shipdate;
        [DataMember]
        public Nullable<System.DateTime> DeliveryDate 
        {
            get
            {
                return _deliverydate;
            }
            set
            {
                _deliverydate = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<System.DateTime> _deliverydate;
        [DataMember]
        public string Seal 
        {
            get
            {
                return _seal;
            }
            set
            {
                _seal = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _seal;
        [DataMember]
        public AsycudaDocumentSet AsycudaDocumentSet { get; set; }

 //       [DataMember]
 //       public TrackingState TrackingState { get; set; }
 //       [DataMember]
 //       public ICollection<string> ModifiedProperties { get; set; }
//        [DataMember]//JsonProperty,
 //       private Guid EntityIdentifier { get; set; }
    }
}


