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
    public partial class Consignees : BaseEntity<Consignees>, ITrackable, IEquatable<Consignees>
    {
        [DataMember]
        public string ConsigneeName
		{ 
		    get { return _ConsigneeName; }
			set
			{
			    if (value == _ConsigneeName) return;
				_ConsigneeName = value;
				NotifyPropertyChanged();//m => this.ConsigneeName
			}
		}
        private string _ConsigneeName;

        [DataMember]
        public string ConsigneeCode
		{ 
		    get { return _ConsigneeCode; }
			set
			{
			    if (value == _ConsigneeCode) return;
				_ConsigneeCode = value;
				NotifyPropertyChanged();//m => this.ConsigneeCode
			}
		}
        private string _ConsigneeCode;

        [DataMember]
        public string Address
		{ 
		    get { return _Address; }
			set
			{
			    if (value == _Address) return;
				_Address = value;
				NotifyPropertyChanged();//m => this.Address
			}
		}
        private string _Address;

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

       
        [DataMember]
        public ChangeTrackingCollection<AsycudaDocumentSet> AsycudaDocumentSet
		{
		    get { return _AsycudaDocumentSet; }
			set
			{
			    if (Equals(value, _AsycudaDocumentSet)) return;
				_AsycudaDocumentSet = value;
				NotifyPropertyChanged();//m => this.AsycudaDocumentSet
			}
		}
        private ChangeTrackingCollection<AsycudaDocumentSet> _AsycudaDocumentSet = new ChangeTrackingCollection<AsycudaDocumentSet>();

   //     [DataMember]
   //     public TrackingState TrackingState { get; set; }

   //     [DataMember]
   //     public ICollection<string> ModifiedProperties { get; set; }
        
    //  [DataMember]//JsonProperty, 
    //	private Guid EntityIdentifier { get; set; }
    
    //	[DataMember]//JsonProperty, 
    //	private Guid _entityIdentity = default(Guid);
    
    	bool IEquatable<Consignees>.Equals(Consignees other)
    	{
    		if (EntityIdentifier != default(Guid))
    			return EntityIdentifier == other.EntityIdentifier;
    		return false;
    	}
    }
}



