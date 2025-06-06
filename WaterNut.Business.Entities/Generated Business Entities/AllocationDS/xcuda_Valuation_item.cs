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
    public partial class xcuda_Valuation_item : BaseEntity<xcuda_Valuation_item>, ITrackable 
    {
        [DataMember]
        public double Total_cost_itm 
        {
            get
            {
                return _total_cost_itm;
            }
            set
            {
                _total_cost_itm = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        double _total_cost_itm;
        [DataMember]
        public double Total_CIF_itm 
        {
            get
            {
                return _total_cif_itm;
            }
            set
            {
                _total_cif_itm = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        double _total_cif_itm;
        [DataMember]
        public Nullable<double> Rate_of_adjustement 
        {
            get
            {
                return _rate_of_adjustement;
            }
            set
            {
                _rate_of_adjustement = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<double> _rate_of_adjustement;
        [DataMember]
        public double Statistical_value 
        {
            get
            {
                return _statistical_value;
            }
            set
            {
                _statistical_value = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        double _statistical_value;
        [DataMember]
        public string Alpha_coeficient_of_apportionment 
        {
            get
            {
                return _alpha_coeficient_of_apportionment;
            }
            set
            {
                _alpha_coeficient_of_apportionment = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _alpha_coeficient_of_apportionment;
        [DataMember]
        public int Item_Id 
        {
            get
            {
                return _item_id;
            }
            set
            {
                _item_id = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        int _item_id;
        [DataMember]
        public xcuda_Item xcuda_Item { get; set; }
        [DataMember]
        public xcuda_Item_Invoice xcuda_Item_Invoice { get; set; }
        [DataMember]
        public xcuda_Weight_itm xcuda_Weight_itm { get; set; }

 //       [DataMember]
 //       public TrackingState TrackingState { get; set; }
 //       [DataMember]
 //       public ICollection<string> ModifiedProperties { get; set; }
//        [DataMember]//JsonProperty,
 //       private Guid EntityIdentifier { get; set; }
    }
}


