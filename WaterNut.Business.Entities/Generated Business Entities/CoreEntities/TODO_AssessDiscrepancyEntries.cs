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

namespace CoreEntities.Business.Entities
{

    //[JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace="http://www.insight-software.com/WaterNut")]
    public partial class TODO_AssessDiscrepancyEntries : BaseEntity<TODO_AssessDiscrepancyEntries>, ITrackable 
    {
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
        public string AdjustmentType 
        {
            get
            {
                return _adjustmenttype;
            }
            set
            {
                _adjustmenttype = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _adjustmenttype;
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
        public string Declarant_Reference_Number 
        {
            get
            {
                return _declarant_reference_number;
            }
            set
            {
                _declarant_reference_number = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _declarant_reference_number;

 //       [DataMember]
 //       public TrackingState TrackingState { get; set; }
 //       [DataMember]
 //       public ICollection<string> ModifiedProperties { get; set; }
//        [DataMember]//JsonProperty,
 //       private Guid EntityIdentifier { get; set; }
    }
}


