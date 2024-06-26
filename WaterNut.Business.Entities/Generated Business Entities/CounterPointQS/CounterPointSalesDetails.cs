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

namespace CounterPointQS.Business.Entities
{

    //[JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace="http://www.insight-software.com/WaterNut")]
    public partial class CounterPointSalesDetails : BaseEntity<CounterPointSalesDetails>, ITrackable 
    {
        [DataMember]
        public string INVNO 
        {
            get
            {
                return _invno;
            }
            set
            {
                _invno = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _invno;
        [DataMember]
        public Nullable<int> SEQ_NO 
        {
            get
            {
                return _seq_no;
            }
            set
            {
                _seq_no = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<int> _seq_no;
        [DataMember]
        public string ITEM_NO 
        {
            get
            {
                return _item_no;
            }
            set
            {
                _item_no = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _item_no;
        [DataMember]
        public string ITEM_DESCR 
        {
            get
            {
                return _item_descr;
            }
            set
            {
                _item_descr = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _item_descr;
        [DataMember]
        public Nullable<decimal> QUANTITY 
        {
            get
            {
                return _quantity;
            }
            set
            {
                _quantity = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<decimal> _quantity;
        [DataMember]
        public Nullable<decimal> COST 
        {
            get
            {
                return _cost;
            }
            set
            {
                _cost = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<decimal> _cost;
        [DataMember]
        public string ACCT_NO 
        {
            get
            {
                return _acct_no;
            }
            set
            {
                _acct_no = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _acct_no;
        [DataMember]
        public string CUSTOMER_NAME 
        {
            get
            {
                return _customer_name;
            }
            set
            {
                _customer_name = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _customer_name;
        [DataMember]
        public Nullable<System.DateTime> DATE 
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<System.DateTime> _date;
        [DataMember]
        public decimal TAX_AMT 
        {
            get
            {
                return _tax_amt;
            }
            set
            {
                _tax_amt = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        decimal _tax_amt;
        [DataMember]
        public Nullable<decimal> UNIT_WEIGHT 
        {
            get
            {
                return _unit_weight;
            }
            set
            {
                _unit_weight = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<decimal> _unit_weight;
        [DataMember]
        public string QTY_UNIT 
        {
            get
            {
                return _qty_unit;
            }
            set
            {
                _qty_unit = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _qty_unit;

 //       [DataMember]
 //       public TrackingState TrackingState { get; set; }
 //       [DataMember]
 //       public ICollection<string> ModifiedProperties { get; set; }
//        [DataMember]//JsonProperty,
 //       private Guid EntityIdentifier { get; set; }
    }
}


