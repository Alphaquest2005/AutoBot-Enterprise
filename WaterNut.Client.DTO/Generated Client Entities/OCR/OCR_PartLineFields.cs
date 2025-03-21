﻿// <autogenerated>
//   This file was generated by T4 code generator AllClientEntities.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
//using Newtonsoft.Json;


using Core.Common.Client.DTO;
using TrackableEntities;
using TrackableEntities.Client;

namespace OCR.Client.DTO
{

   // [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace="http://www.insight-software.com/WaterNut")]
    public partial class OCR_PartLineFields : BaseEntity<OCR_PartLineFields>, ITrackable, IEquatable<OCR_PartLineFields>
    {
        [DataMember]
        public int Id
		{ 
		    get { return _Id; }
			set
			{
			    if (value == _Id) return;
				_Id = value;
				NotifyPropertyChanged();//m => this.Id
			}
		}
        private int _Id;

        [DataMember]
        public string Invoice
		{ 
		    get { return _Invoice; }
			set
			{
			    if (value == _Invoice) return;
				_Invoice = value;
				NotifyPropertyChanged();//m => this.Invoice
			}
		}
        private string _Invoice;

        [DataMember]
        public string Part
		{ 
		    get { return _Part; }
			set
			{
			    if (value == _Part) return;
				_Part = value;
				NotifyPropertyChanged();//m => this.Part
			}
		}
        private string _Part;

        [DataMember]
        public string Line
		{ 
		    get { return _Line; }
			set
			{
			    if (value == _Line) return;
				_Line = value;
				NotifyPropertyChanged();//m => this.Line
			}
		}
        private string _Line;

        [DataMember]
        public string RegEx
		{ 
		    get { return _RegEx; }
			set
			{
			    if (value == _RegEx) return;
				_RegEx = value;
				NotifyPropertyChanged();//m => this.RegEx
			}
		}
        private string _RegEx;

        [DataMember]
        public string Key
		{ 
		    get { return _Key; }
			set
			{
			    if (value == _Key) return;
				_Key = value;
				NotifyPropertyChanged();//m => this.Key
			}
		}
        private string _Key;

        [DataMember]
        public string Field
		{ 
		    get { return _Field; }
			set
			{
			    if (value == _Field) return;
				_Field = value;
				NotifyPropertyChanged();//m => this.Field
			}
		}
        private string _Field;

        [DataMember]
        public string EntityType
		{ 
		    get { return _EntityType; }
			set
			{
			    if (value == _EntityType) return;
				_EntityType = value;
				NotifyPropertyChanged();//m => this.EntityType
			}
		}
        private string _EntityType;

        [DataMember]
        public Nullable<bool> IsRequired
		{ 
		    get { return _IsRequired; }
			set
			{
			    if (value == _IsRequired) return;
				_IsRequired = value;
				NotifyPropertyChanged();//m => this.IsRequired
			}
		}
        private Nullable<bool> _IsRequired;

        [DataMember]
        public string DataType
		{ 
		    get { return _DataType; }
			set
			{
			    if (value == _DataType) return;
				_DataType = value;
				NotifyPropertyChanged();//m => this.DataType
			}
		}
        private string _DataType;

        [DataMember]
        public string Value
		{ 
		    get { return _Value; }
			set
			{
			    if (value == _Value) return;
				_Value = value;
				NotifyPropertyChanged();//m => this.Value
			}
		}
        private string _Value;

        [DataMember]
        public Nullable<bool> AppendValues
		{ 
		    get { return _AppendValues; }
			set
			{
			    if (value == _AppendValues) return;
				_AppendValues = value;
				NotifyPropertyChanged();//m => this.AppendValues
			}
		}
        private Nullable<bool> _AppendValues;

        [DataMember]
        public int FieldId
		{ 
		    get { return _FieldId; }
			set
			{
			    if (value == _FieldId) return;
				_FieldId = value;
				NotifyPropertyChanged();//m => this.FieldId
			}
		}
        private int _FieldId;

        [DataMember]
        public Nullable<int> ParentId
		{ 
		    get { return _ParentId; }
			set
			{
			    if (value == _ParentId) return;
				_ParentId = value;
				NotifyPropertyChanged();//m => this.ParentId
			}
		}
        private Nullable<int> _ParentId;

        [DataMember]
        public int LineId
		{ 
		    get { return _LineId; }
			set
			{
			    if (value == _LineId) return;
				_LineId = value;
				NotifyPropertyChanged();//m => this.LineId
			}
		}
        private int _LineId;

       
   //     [DataMember]
   //     public TrackingState TrackingState { get; set; }

   //     [DataMember]
   //     public ICollection<string> ModifiedProperties { get; set; }
        
    //  [DataMember]//JsonProperty, 
    //	private Guid EntityIdentifier { get; set; }
    
    //	[DataMember]//JsonProperty, 
    //	private Guid _entityIdentity = default(Guid);
    
    	bool IEquatable<OCR_PartLineFields>.Equals(OCR_PartLineFields other)
    	{
    		if (EntityIdentifier != default(Guid))
    			return EntityIdentifier == other.EntityIdentifier;
    		return false;
    	}
    }
}



