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
    public partial class TODO_AdjustmentsAlreadyXMLed : BaseEntity<TODO_AdjustmentsAlreadyXMLed>, ITrackable 
    {
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
        public Nullable<System.DateTime> RegistrationDate 
        {
            get
            {
                return _registrationdate;
            }
            set
            {
                _registrationdate = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<System.DateTime> _registrationdate;
        [DataMember]
        public string ReferenceNumber 
        {
            get
            {
                return _referencenumber;
            }
            set
            {
                _referencenumber = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _referencenumber;
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
        public string PreviousCNumber 
        {
            get
            {
                return _previouscnumber;
            }
            set
            {
                _previouscnumber = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _previouscnumber;
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
        [DataMember]
        public Nullable<System.DateTime> EffectiveDate 
        {
            get
            {
                return _effectivedate;
            }
            set
            {
                _effectivedate = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<System.DateTime> _effectivedate;
        [DataMember]
        public string ItemDescription 
        {
            get
            {
                return _itemdescription;
            }
            set
            {
                _itemdescription = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _itemdescription;
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
        public Nullable<int> FileTypeId 
        {
            get
            {
                return _filetypeid;
            }
            set
            {
                _filetypeid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<int> _filetypeid;
        [DataMember]
        public double Quantity 
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
        double _quantity;
        [DataMember]
        public double Cost 
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
        double _cost;
        [DataMember]
        public string Subject 
        {
            get
            {
                return _subject;
            }
            set
            {
                _subject = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _subject;
        [DataMember]
        public System.DateTime EmailDate 
        {
            get
            {
                return _emaildate;
            }
            set
            {
                _emaildate = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        System.DateTime _emaildate;
        [DataMember]
        public Nullable<int> LineNumber 
        {
            get
            {
                return _linenumber;
            }
            set
            {
                _linenumber = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<int> _linenumber;
        [DataMember]
        public string PreviousInvoiceNumber 
        {
            get
            {
                return _previousinvoicenumber;
            }
            set
            {
                _previousinvoicenumber = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _previousinvoicenumber;
        [DataMember]
        public string Comment 
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _comment;
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
        public Nullable<int> pLineNumber 
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
        Nullable<int> _plinenumber;

 //       [DataMember]
 //       public TrackingState TrackingState { get; set; }
 //       [DataMember]
 //       public ICollection<string> ModifiedProperties { get; set; }
//        [DataMember]//JsonProperty,
 //       private Guid EntityIdentifier { get; set; }
    }
}


