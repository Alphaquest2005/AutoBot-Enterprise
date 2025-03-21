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
    public partial class ShipmentMIS_POs : BaseEntity<ShipmentMIS_POs>, ITrackable 
    {
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
        public string InvoiceNo 
        {
            get
            {
                return _invoiceno;
            }
            set
            {
                _invoiceno = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _invoiceno;
        [DataMember]
        public System.DateTime InvoiceDate 
        {
            get
            {
                return _invoicedate;
            }
            set
            {
                _invoicedate = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        System.DateTime _invoicedate;
        [DataMember]
        public Nullable<double> SubTotal 
        {
            get
            {
                return _subtotal;
            }
            set
            {
                _subtotal = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<double> _subtotal;
        [DataMember]
        public Nullable<double> InvoiceTotal 
        {
            get
            {
                return _invoicetotal;
            }
            set
            {
                _invoicetotal = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<double> _invoicetotal;
        [DataMember]
        public Nullable<int> InvoiceId 
        {
            get
            {
                return _invoiceid;
            }
            set
            {
                _invoiceid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<int> _invoiceid;
        [DataMember]
        public string SupplierCode 
        {
            get
            {
                return _suppliercode;
            }
            set
            {
                _suppliercode = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _suppliercode;
        [DataMember]
        public string SourceFile 
        {
            get
            {
                return _sourcefile;
            }
            set
            {
                _sourcefile = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _sourcefile;
        [DataMember]
        public Nullable<int> ImportedLines 
        {
            get
            {
                return _importedlines;
            }
            set
            {
                _importedlines = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<int> _importedlines;

 //       [DataMember]
 //       public TrackingState TrackingState { get; set; }
 //       [DataMember]
 //       public ICollection<string> ModifiedProperties { get; set; }
//        [DataMember]//JsonProperty,
 //       private Guid EntityIdentifier { get; set; }
    }
}


