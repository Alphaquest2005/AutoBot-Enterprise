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

namespace AdjustmentQS.Client.DTO
{

   // [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace="http://www.insight-software.com/WaterNut")]
    public partial class SystemDocumentSet : BaseEntity<SystemDocumentSet>, ITrackable, IEquatable<SystemDocumentSet>
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
        public ChangeTrackingCollection<AdjustmentDetail> AdjustmentDetails
		{
		    get { return _AdjustmentDetails; }
			set
			{
			    if (Equals(value, _AdjustmentDetails)) return;
				_AdjustmentDetails = value;
				NotifyPropertyChanged();//m => this.AdjustmentDetails
			}
		}
        private ChangeTrackingCollection<AdjustmentDetail> _AdjustmentDetails = new ChangeTrackingCollection<AdjustmentDetail>();

   //     [DataMember]
   //     public TrackingState TrackingState { get; set; }

   //     [DataMember]
   //     public ICollection<string> ModifiedProperties { get; set; }
        
    //  [DataMember]//JsonProperty, 
    //	private Guid EntityIdentifier { get; set; }
    
    //	[DataMember]//JsonProperty, 
    //	private Guid _entityIdentity = default(Guid);
    
    	bool IEquatable<SystemDocumentSet>.Equals(SystemDocumentSet other)
    	{
    		if (EntityIdentifier != default(Guid))
    			return EntityIdentifier == other.EntityIdentifier;
    		return false;
    	}
    }
}


