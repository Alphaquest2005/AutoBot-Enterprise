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

namespace PreviousDocumentQS.Client.DTO
{

   // [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace="http://www.insight-software.com/WaterNut")]
    public partial class PreviousDocument : BaseEntity<PreviousDocument>, ITrackable, IEquatable<PreviousDocument>
    {
        [DataMember]
        public int ASYCUDA_Id
		{ 
		    get { return _ASYCUDA_Id; }
			set
			{
			    if (value == _ASYCUDA_Id) return;
				_ASYCUDA_Id = value;
				NotifyPropertyChanged();//m => this.ASYCUDA_Id
			}
		}
        private int _ASYCUDA_Id;

        [DataMember]
        public string id
		{ 
		    get { return _id; }
			set
			{
			    if (value == _id) return;
				_id = value;
				NotifyPropertyChanged();//m => this.id
			}
		}
        private string _id;

        [DataMember]
        public string CNumber
		{ 
		    get { return _CNumber; }
			set
			{
			    if (value == _CNumber) return;
				_CNumber = value;
				NotifyPropertyChanged();//m => this.CNumber
			}
		}
        private string _CNumber;

        [DataMember]
        public Nullable<System.DateTime> RegistrationDate
		{ 
		    get { return _RegistrationDate; }
			set
			{
			    if (value == _RegistrationDate) return;
				_RegistrationDate = value;
				NotifyPropertyChanged();//m => this.RegistrationDate
			}
		}
        private Nullable<System.DateTime> _RegistrationDate;

        [DataMember]
        public Nullable<bool> IsManuallyAssessed
		{ 
		    get { return _IsManuallyAssessed; }
			set
			{
			    if (value == _IsManuallyAssessed) return;
				_IsManuallyAssessed = value;
				NotifyPropertyChanged();//m => this.IsManuallyAssessed
			}
		}
        private Nullable<bool> _IsManuallyAssessed;

        [DataMember]
        public string ReferenceNumber
		{ 
		    get { return _ReferenceNumber; }
			set
			{
			    if (value == _ReferenceNumber) return;
				_ReferenceNumber = value;
				NotifyPropertyChanged();//m => this.ReferenceNumber
			}
		}
        private string _ReferenceNumber;

        [DataMember]
        public Nullable<System.DateTime> EffectiveRegistrationDate
		{ 
		    get { return _EffectiveRegistrationDate; }
			set
			{
			    if (value == _EffectiveRegistrationDate) return;
				_EffectiveRegistrationDate = value;
				NotifyPropertyChanged();//m => this.EffectiveRegistrationDate
			}
		}
        private Nullable<System.DateTime> _EffectiveRegistrationDate;

        [DataMember]
        public Nullable<double> TotalValue
		{ 
		    get { return _TotalValue; }
			set
			{
			    if (value == _TotalValue) return;
				_TotalValue = value;
				NotifyPropertyChanged();//m => this.TotalValue
			}
		}
        private Nullable<double> _TotalValue;

        [DataMember]
        public Nullable<double> AllocatedValue
		{ 
		    get { return _AllocatedValue; }
			set
			{
			    if (value == _AllocatedValue) return;
				_AllocatedValue = value;
				NotifyPropertyChanged();//m => this.AllocatedValue
			}
		}
        private Nullable<double> _AllocatedValue;

        [DataMember]
        public Nullable<double> PiValue
		{ 
		    get { return _PiValue; }
			set
			{
			    if (value == _PiValue) return;
				_PiValue = value;
				NotifyPropertyChanged();//m => this.PiValue
			}
		}
        private Nullable<double> _PiValue;

        [DataMember]
        public Nullable<int> AsycudaDocumentSetId
		{ 
		    get { return _AsycudaDocumentSetId; }
			set
			{
			    if (value == _AsycudaDocumentSetId) return;
				_AsycudaDocumentSetId = value;
				NotifyPropertyChanged();//m => this.AsycudaDocumentSetId
			}
		}
        private Nullable<int> _AsycudaDocumentSetId;

        [DataMember]
        public Nullable<bool> DoNotAllocate
		{ 
		    get { return _DoNotAllocate; }
			set
			{
			    if (value == _DoNotAllocate) return;
				_DoNotAllocate = value;
				NotifyPropertyChanged();//m => this.DoNotAllocate
			}
		}
        private Nullable<bool> _DoNotAllocate;

        [DataMember]
        public string Description
		{ 
		    get { return _Description; }
			set
			{
			    if (value == _Description) return;
				_Description = value;
				NotifyPropertyChanged();//m => this.Description
			}
		}
        private string _Description;

        [DataMember]
        public string BLNumber
		{ 
		    get { return _BLNumber; }
			set
			{
			    if (value == _BLNumber) return;
				_BLNumber = value;
				NotifyPropertyChanged();//m => this.BLNumber
			}
		}
        private string _BLNumber;

        [DataMember]
        public Nullable<int> Lines
		{ 
		    get { return _Lines; }
			set
			{
			    if (value == _Lines) return;
				_Lines = value;
				NotifyPropertyChanged();//m => this.Lines
			}
		}
        private Nullable<int> _Lines;

        [DataMember]
        public string DocumentType
		{ 
		    get { return _DocumentType; }
			set
			{
			    if (value == _DocumentType) return;
				_DocumentType = value;
				NotifyPropertyChanged();//m => this.DocumentType
			}
		}
        private string _DocumentType;

        [DataMember]
        public Nullable<int> ApplicationSettingsId
		{ 
		    get { return _ApplicationSettingsId; }
			set
			{
			    if (value == _ApplicationSettingsId) return;
				_ApplicationSettingsId = value;
				NotifyPropertyChanged();//m => this.ApplicationSettingsId
			}
		}
        private Nullable<int> _ApplicationSettingsId;

       
        [DataMember]
        public ChangeTrackingCollection<PreviousDocumentItem> PreviousDocumentItems
		{
		    get { return _PreviousDocumentItems; }
			set
			{
			    if (Equals(value, _PreviousDocumentItems)) return;
				_PreviousDocumentItems = value;
				NotifyPropertyChanged();//m => this.PreviousDocumentItems
			}
		}
        private ChangeTrackingCollection<PreviousDocumentItem> _PreviousDocumentItems = new ChangeTrackingCollection<PreviousDocumentItem>();

   //     [DataMember]
   //     public TrackingState TrackingState { get; set; }

   //     [DataMember]
   //     public ICollection<string> ModifiedProperties { get; set; }
        
    //  [DataMember]//JsonProperty, 
    //	private Guid EntityIdentifier { get; set; }
    
    //	[DataMember]//JsonProperty, 
    //	private Guid _entityIdentity = default(Guid);
    
    	bool IEquatable<PreviousDocument>.Equals(PreviousDocument other)
    	{
    		if (EntityIdentifier != default(Guid))
    			return EntityIdentifier == other.EntityIdentifier;
    		return false;
    	}
    }
}


