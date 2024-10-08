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
    public partial class TODO_SubmitIncompleteSupplierInfo : BaseEntity<TODO_SubmitIncompleteSupplierInfo>, ITrackable, IEquatable<TODO_SubmitIncompleteSupplierInfo>
    {
        [DataMember]
        public string SupplierCode
		{ 
		    get { return _SupplierCode; }
			set
			{
			    if (value == _SupplierCode) return;
				_SupplierCode = value;
				NotifyPropertyChanged();//m => this.SupplierCode
			}
		}
        private string _SupplierCode;

        [DataMember]
        public string CountryCode
		{ 
		    get { return _CountryCode; }
			set
			{
			    if (value == _CountryCode) return;
				_CountryCode = value;
				NotifyPropertyChanged();//m => this.CountryCode
			}
		}
        private string _CountryCode;

        [DataMember]
        public string SupplierName
		{ 
		    get { return _SupplierName; }
			set
			{
			    if (value == _SupplierName) return;
				_SupplierName = value;
				NotifyPropertyChanged();//m => this.SupplierName
			}
		}
        private string _SupplierName;

        [DataMember]
        public string Street
		{ 
		    get { return _Street; }
			set
			{
			    if (value == _Street) return;
				_Street = value;
				NotifyPropertyChanged();//m => this.Street
			}
		}
        private string _Street;

        [DataMember]
        public string EntryDataId
		{ 
		    get { return _EntryDataId; }
			set
			{
			    if (value == _EntryDataId) return;
				_EntryDataId = value;
				NotifyPropertyChanged();//m => this.EntryDataId
			}
		}
        private string _EntryDataId;

        [DataMember]
        public int AsycudaDocumentSetId
		{ 
		    get { return _AsycudaDocumentSetId; }
			set
			{
			    if (value == _AsycudaDocumentSetId) return;
				_AsycudaDocumentSetId = value;
				NotifyPropertyChanged();//m => this.AsycudaDocumentSetId
			}
		}
        private int _AsycudaDocumentSetId;

        [DataMember]
        public int ApplicationSettingsId
		{ 
		    get { return _ApplicationSettingsId; }
			set
			{
			    if (value == _ApplicationSettingsId) return;
				_ApplicationSettingsId = value;
				NotifyPropertyChanged();//m => this.ApplicationSettingsId
			}
		}
        private int _ApplicationSettingsId;

       
   //     [DataMember]
   //     public TrackingState TrackingState { get; set; }

   //     [DataMember]
   //     public ICollection<string> ModifiedProperties { get; set; }
        
    //  [DataMember]//JsonProperty, 
    //	private Guid EntityIdentifier { get; set; }
    
    //	[DataMember]//JsonProperty, 
    //	private Guid _entityIdentity = default(Guid);
    
    	bool IEquatable<TODO_SubmitIncompleteSupplierInfo>.Equals(TODO_SubmitIncompleteSupplierInfo other)
    	{
    		if (EntityIdentifier != default(Guid))
    			return EntityIdentifier == other.EntityIdentifier;
    		return false;
    	}
    }
}



