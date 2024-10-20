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

namespace OCR.Business.Entities
{

    //[JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace="http://www.insight-software.com/WaterNut")]
    public partial class OCR_FieldMappings : BaseEntity<OCR_FieldMappings>, ITrackable 
    {
        [DataMember]
        public int Id 
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
        int _id;
        [DataMember]
        public string Key 
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _key;
        [DataMember]
        public string Field 
        {
            get
            {
                return _field;
            }
            set
            {
                _field = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _field;
        [DataMember]
        public string EntityType 
        {
            get
            {
                return _entitytype;
            }
            set
            {
                _entitytype = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _entitytype;
        [DataMember]
        public bool IsRequired 
        {
            get
            {
                return _isrequired;
            }
            set
            {
                _isrequired = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        bool _isrequired;
        [DataMember]
        public string DataType 
        {
            get
            {
                return _datatype;
            }
            set
            {
                _datatype = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _datatype;
        [DataMember]
        public Nullable<bool> AppendValues 
        {
            get
            {
                return _appendvalues;
            }
            set
            {
                _appendvalues = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<bool> _appendvalues;
        [DataMember]
        public int FileTypeId 
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
        int _filetypeid;

 //       [DataMember]
 //       public TrackingState TrackingState { get; set; }
 //       [DataMember]
 //       public ICollection<string> ModifiedProperties { get; set; }
//        [DataMember]//JsonProperty,
 //       private Guid EntityIdentifier { get; set; }
    }
}


