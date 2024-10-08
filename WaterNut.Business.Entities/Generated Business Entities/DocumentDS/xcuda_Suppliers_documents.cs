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
    public partial class xcuda_Suppliers_documents : BaseEntity<xcuda_Suppliers_documents>, ITrackable 
    {
        [DataMember]
        public string Suppliers_document_date 
        {
            get
            {
                return _suppliers_document_date;
            }
            set
            {
                _suppliers_document_date = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _suppliers_document_date;
        [DataMember]
        public int Suppliers_documents_Id 
        {
            get
            {
                return _suppliers_documents_id;
            }
            set
            {
                _suppliers_documents_id = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        int _suppliers_documents_id;
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
        [DataMember]
        public string Suppliers_document_itmlink 
        {
            get
            {
                return _suppliers_document_itmlink;
            }
            set
            {
                _suppliers_document_itmlink = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _suppliers_document_itmlink;
        [DataMember]
        public string Suppliers_document_code 
        {
            get
            {
                return _suppliers_document_code;
            }
            set
            {
                _suppliers_document_code = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _suppliers_document_code;
        [DataMember]
        public string Suppliers_document_name 
        {
            get
            {
                return _suppliers_document_name;
            }
            set
            {
                _suppliers_document_name = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _suppliers_document_name;
        [DataMember]
        public string Suppliers_document_country 
        {
            get
            {
                return _suppliers_document_country;
            }
            set
            {
                _suppliers_document_country = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _suppliers_document_country;
        [DataMember]
        public string Suppliers_document_city 
        {
            get
            {
                return _suppliers_document_city;
            }
            set
            {
                _suppliers_document_city = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _suppliers_document_city;
        [DataMember]
        public string Suppliers_document_street 
        {
            get
            {
                return _suppliers_document_street;
            }
            set
            {
                _suppliers_document_street = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _suppliers_document_street;
        [DataMember]
        public string Suppliers_document_telephone 
        {
            get
            {
                return _suppliers_document_telephone;
            }
            set
            {
                _suppliers_document_telephone = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _suppliers_document_telephone;
        [DataMember]
        public string Suppliers_document_fax 
        {
            get
            {
                return _suppliers_document_fax;
            }
            set
            {
                _suppliers_document_fax = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _suppliers_document_fax;
        [DataMember]
        public string Suppliers_document_zip_code 
        {
            get
            {
                return _suppliers_document_zip_code;
            }
            set
            {
                _suppliers_document_zip_code = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _suppliers_document_zip_code;
        [DataMember]
        public string Suppliers_document_invoice_nbr 
        {
            get
            {
                return _suppliers_document_invoice_nbr;
            }
            set
            {
                _suppliers_document_invoice_nbr = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _suppliers_document_invoice_nbr;
        [DataMember]
        public string Suppliers_document_invoice_amt 
        {
            get
            {
                return _suppliers_document_invoice_amt;
            }
            set
            {
                _suppliers_document_invoice_amt = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _suppliers_document_invoice_amt;
        [DataMember]
        public string Suppliers_document_type_code 
        {
            get
            {
                return _suppliers_document_type_code;
            }
            set
            {
                _suppliers_document_type_code = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _suppliers_document_type_code;
        [DataMember]
        public xcuda_ASYCUDA xcuda_ASYCUDA { get; set; }

 //       [DataMember]
 //       public TrackingState TrackingState { get; set; }
 //       [DataMember]
 //       public ICollection<string> ModifiedProperties { get; set; }
//        [DataMember]//JsonProperty,
 //       private Guid EntityIdentifier { get; set; }
    }
}


