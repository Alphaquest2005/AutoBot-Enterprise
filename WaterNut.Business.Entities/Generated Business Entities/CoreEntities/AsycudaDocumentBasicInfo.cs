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
    public partial class AsycudaDocumentBasicInfo : BaseEntity<AsycudaDocumentBasicInfo>, ITrackable 
    {
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
        public int ASYCUDA_Id 
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
        int _asycuda_id;
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
        public string Extended_customs_procedure 
        {
            get
            {
                return _extended_customs_procedure;
            }
            set
            {
                _extended_customs_procedure = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _extended_customs_procedure;
        [DataMember]
        public string National_customs_procedure 
        {
            get
            {
                return _national_customs_procedure;
            }
            set
            {
                _national_customs_procedure = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _national_customs_procedure;
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
        public Nullable<System.DateTime> AssessmentDate 
        {
            get
            {
                return _assessmentdate;
            }
            set
            {
                _assessmentdate = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<System.DateTime> _assessmentdate;
        [DataMember]
        public Nullable<System.DateTime> ExpiryDate 
        {
            get
            {
                return _expirydate;
            }
            set
            {
                _expirydate = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<System.DateTime> _expirydate;
        [DataMember]
        public string Reference 
        {
            get
            {
                return _reference;
            }
            set
            {
                _reference = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _reference;
        [DataMember]
        public Nullable<bool> IsManuallyAssessed 
        {
            get
            {
                return _ismanuallyassessed;
            }
            set
            {
                _ismanuallyassessed = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<bool> _ismanuallyassessed;
        [DataMember]
        public Nullable<bool> Cancelled 
        {
            get
            {
                return _cancelled;
            }
            set
            {
                _cancelled = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<bool> _cancelled;
        [DataMember]
        public Nullable<bool> DoNotAllocate 
        {
            get
            {
                return _donotallocate;
            }
            set
            {
                _donotallocate = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<bool> _donotallocate;
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
        public bool ImportComplete 
        {
            get
            {
                return _importcomplete;
            }
            set
            {
                _importcomplete = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        bool _importcomplete;
        [DataMember]
        public int Customs_ProcedureId 
        {
            get
            {
                return _customs_procedureid;
            }
            set
            {
                _customs_procedureid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        int _customs_procedureid;

 //       [DataMember]
 //       public TrackingState TrackingState { get; set; }
 //       [DataMember]
 //       public ICollection<string> ModifiedProperties { get; set; }
//        [DataMember]//JsonProperty,
 //       private Guid EntityIdentifier { get; set; }
    }
}

