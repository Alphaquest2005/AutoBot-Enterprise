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
    public partial class TODO_DiscrepanciesExecutionReport : BaseEntity<TODO_DiscrepanciesExecutionReport>, ITrackable 
    {
        [DataMember]
        public long Id 
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
        long _id;
        [DataMember]
        public int EntryDataDetailsId 
        {
            get
            {
                return _entrydatadetailsid;
            }
            set
            {
                _entrydatadetailsid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        int _entrydatadetailsid;
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
        public Nullable<bool> IsClassified 
        {
            get
            {
                return _isclassified;
            }
            set
            {
                _isclassified = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<bool> _isclassified;
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
        public Nullable<double> InvoiceQty 
        {
            get
            {
                return _invoiceqty;
            }
            set
            {
                _invoiceqty = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<double> _invoiceqty;
        [DataMember]
        public Nullable<double> ReceivedQty 
        {
            get
            {
                return _receivedqty;
            }
            set
            {
                _receivedqty = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<double> _receivedqty;
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
        public string Status 
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _status;
        [DataMember]
        public string CNumber 
        {
            get
            {
                return _cnumber;
            }
            set
            {
                _cnumber = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _cnumber;
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
        [DataMember]
        public string EmailId 
        {
            get
            {
                return _emailid;
            }
            set
            {
                _emailid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _emailid;
        [DataMember]
        public string xCNumber 
        {
            get
            {
                return _xcnumber;
            }
            set
            {
                _xcnumber = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _xcnumber;
        [DataMember]
        public Nullable<int> xLineNumber 
        {
            get
            {
                return _xlinenumber;
            }
            set
            {
                _xlinenumber = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<int> _xlinenumber;
        [DataMember]
        public Nullable<System.DateTime> xRegistrationDate 
        {
            get
            {
                return _xregistrationdate;
            }
            set
            {
                _xregistrationdate = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<System.DateTime> _xregistrationdate;
        [DataMember]
        public string DocumentType 
        {
            get
            {
                return _documenttype;
            }
            set
            {
                _documenttype = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _documenttype;
        [DataMember]
        public string CustomsProcedure 
        {
            get
            {
                return _customsprocedure;
            }
            set
            {
                _customsprocedure = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _customsprocedure;
        [DataMember]
        public Nullable<int> ASYCUDA_Id 
        {
            get
            {
                return _asycuda_id;
            }
            set
            {
                _asycuda_id = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<int> _asycuda_id;

 //       [DataMember]
 //       public TrackingState TrackingState { get; set; }
 //       [DataMember]
 //       public ICollection<string> ModifiedProperties { get; set; }
//        [DataMember]//JsonProperty,
 //       private Guid EntityIdentifier { get; set; }
    }
}


