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
    public partial class XSales_UnAllocated : BaseEntity<XSales_UnAllocated>, ITrackable 
    {
        [DataMember]
        public int Line 
        {
            get
            {
                return _line;
            }
            set
            {
                _line = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        int _line;
        [DataMember]
        public System.DateTime Date 
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
        System.DateTime _date;
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
        public string CustomerName 
        {
            get
            {
                return _customername;
            }
            set
            {
                _customername = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _customername;
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
        public string TariffCode 
        {
            get
            {
                return _tariffcode;
            }
            set
            {
                _tariffcode = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _tariffcode;
        [DataMember]
        public Nullable<double> SalesQuantity 
        {
            get
            {
                return _salesquantity;
            }
            set
            {
                _salesquantity = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<double> _salesquantity;
        [DataMember]
        public Nullable<double> SalesFactor 
        {
            get
            {
                return _salesfactor;
            }
            set
            {
                _salesfactor = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<double> _salesfactor;
        [DataMember]
        public double xQuantity 
        {
            get
            {
                return _xquantity;
            }
            set
            {
                _xquantity = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        double _xquantity;
        [DataMember]
        public Nullable<double> Price 
        {
            get
            {
                return _price;
            }
            set
            {
                _price = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<double> _price;
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
        public System.DateTime pRegDate 
        {
            get
            {
                return _pregdate;
            }
            set
            {
                _pregdate = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        System.DateTime _pregdate;
        [DataMember]
        public Nullable<double> CIFValue 
        {
            get
            {
                return _cifvalue;
            }
            set
            {
                _cifvalue = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<double> _cifvalue;
        [DataMember]
        public Nullable<double> DutyLiablity 
        {
            get
            {
                return _dutyliablity;
            }
            set
            {
                _dutyliablity = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<double> _dutyliablity;
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
        public Nullable<int> SalesLineNumber 
        {
            get
            {
                return _saleslinenumber;
            }
            set
            {
                _saleslinenumber = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<int> _saleslinenumber;
        [DataMember]
        public int xItemId 
        {
            get
            {
                return _xitemid;
            }
            set
            {
                _xitemid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        int _xitemid;
        [DataMember]
        public int pItemId 
        {
            get
            {
                return _pitemid;
            }
            set
            {
                _pitemid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        int _pitemid;

 //       [DataMember]
 //       public TrackingState TrackingState { get; set; }
 //       [DataMember]
 //       public ICollection<string> ModifiedProperties { get; set; }
//        [DataMember]//JsonProperty,
 //       private Guid EntityIdentifier { get; set; }
    }
}

