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

namespace CoreEntities.Client.DTO
{

   // [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace="http://www.insight-software.com/WaterNut")]
    public partial class AsycudaItemBasicInfo : BaseEntity<AsycudaItemBasicInfo>, ITrackable, IEquatable<AsycudaItemBasicInfo>
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
        public int Item_Id
		{ 
		    get { return _Item_Id; }
			set
			{
			    if (value == _Item_Id) return;
				_Item_Id = value;
				NotifyPropertyChanged();//m => this.Item_Id
			}
		}
        private int _Item_Id;

        [DataMember]
        public string ItemNumber
		{ 
		    get { return _ItemNumber; }
			set
			{
			    if (value == _ItemNumber) return;
				_ItemNumber = value;
				NotifyPropertyChanged();//m => this.ItemNumber
			}
		}
        private string _ItemNumber;

        [DataMember]
        public Nullable<double> ItemQuantity
		{ 
		    get { return _ItemQuantity; }
			set
			{
			    if (value == _ItemQuantity) return;
				_ItemQuantity = value;
				NotifyPropertyChanged();//m => this.ItemQuantity
			}
		}
        private Nullable<double> _ItemQuantity;

        [DataMember]
        public double DPQtyAllocated
		{ 
		    get { return _DPQtyAllocated; }
			set
			{
			    if (value == _DPQtyAllocated) return;
				_DPQtyAllocated = value;
				NotifyPropertyChanged();//m => this.DPQtyAllocated
			}
		}
        private double _DPQtyAllocated;

        [DataMember]
        public double DFQtyAllocated
		{ 
		    get { return _DFQtyAllocated; }
			set
			{
			    if (value == _DFQtyAllocated) return;
				_DFQtyAllocated = value;
				NotifyPropertyChanged();//m => this.DFQtyAllocated
			}
		}
        private double _DFQtyAllocated;

        [DataMember]
        public Nullable<bool> IsAssessed
		{ 
		    get { return _IsAssessed; }
			set
			{
			    if (value == _IsAssessed) return;
				_IsAssessed = value;
				NotifyPropertyChanged();//m => this.IsAssessed
			}
		}
        private Nullable<bool> _IsAssessed;

        [DataMember]
        public int LineNumber
		{ 
		    get { return _LineNumber; }
			set
			{
			    if (value == _LineNumber) return;
				_LineNumber = value;
				NotifyPropertyChanged();//m => this.LineNumber
			}
		}
        private int _LineNumber;

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
        public string Commercial_Description
		{ 
		    get { return _Commercial_Description; }
			set
			{
			    if (value == _Commercial_Description) return;
				_Commercial_Description = value;
				NotifyPropertyChanged();//m => this.Commercial_Description
			}
		}
        private string _Commercial_Description;

        [DataMember]
        public string TariffCode
		{ 
		    get { return _TariffCode; }
			set
			{
			    if (value == _TariffCode) return;
				_TariffCode = value;
				NotifyPropertyChanged();//m => this.TariffCode
			}
		}
        private string _TariffCode;

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
        public string EntryDataType
		{ 
		    get { return _EntryDataType; }
			set
			{
			    if (value == _EntryDataType) return;
				_EntryDataType = value;
				NotifyPropertyChanged();//m => this.EntryDataType
			}
		}
        private string _EntryDataType;

       
   //     [DataMember]
   //     public TrackingState TrackingState { get; set; }

   //     [DataMember]
   //     public ICollection<string> ModifiedProperties { get; set; }
        
    //  [DataMember]//JsonProperty, 
    //	private Guid EntityIdentifier { get; set; }
    
    //	[DataMember]//JsonProperty, 
    //	private Guid _entityIdentity = default(Guid);
    
    	bool IEquatable<AsycudaItemBasicInfo>.Equals(AsycudaItemBasicInfo other)
    	{
    		if (EntityIdentifier != default(Guid))
    			return EntityIdentifier == other.EntityIdentifier;
    		return false;
    	}
    }
}



