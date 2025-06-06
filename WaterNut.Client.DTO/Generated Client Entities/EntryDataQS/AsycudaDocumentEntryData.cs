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

namespace EntryDataQS.Client.DTO
{

   // [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace="http://www.insight-software.com/WaterNut")]
    public partial class AsycudaDocumentEntryData : BaseEntity<AsycudaDocumentEntryData>, ITrackable, IEquatable<AsycudaDocumentEntryData>
    {
        [DataMember]
        public int AsycudaDocumentId
		{ 
		    get { return _AsycudaDocumentId; }
			set
			{
			    if (value == _AsycudaDocumentId) return;
				_AsycudaDocumentId = value;
				NotifyPropertyChanged();//m => this.AsycudaDocumentId
			}
		}
        private int _AsycudaDocumentId;

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
        public int EntryData_Id
		{ 
		    get { return _EntryData_Id; }
			set
			{
			    if (value == _EntryData_Id) return;
				_EntryData_Id = value;
				NotifyPropertyChanged();//m => this.EntryData_Id
			}
		}
        private int _EntryData_Id;

       
        [DataMember]
        public EntryDataEx EntryDataEx
		{
		    get { return _EntryDataEx; }
			set
			{
			    if (value == _EntryDataEx) return;
				_EntryDataEx = value;
                EntryDataExChangeTracker = _EntryDataEx == null ? null
                    : new ChangeTrackingCollection<EntryDataEx> { _EntryDataEx };
				NotifyPropertyChanged();//m => this.EntryDataEx
			}
		}
        private EntryDataEx _EntryDataEx;
        private ChangeTrackingCollection<EntryDataEx> EntryDataExChangeTracker { get; set; }

        [DataMember]
        public EntryData EntryData
		{
		    get { return _EntryData; }
			set
			{
			    if (value == _EntryData) return;
				_EntryData = value;
                EntryDataChangeTracker = _EntryData == null ? null
                    : new ChangeTrackingCollection<EntryData> { _EntryData };
				NotifyPropertyChanged();//m => this.EntryData
			}
		}
        private EntryData _EntryData;
        private ChangeTrackingCollection<EntryData> EntryDataChangeTracker { get; set; }

   //     [DataMember]
   //     public TrackingState TrackingState { get; set; }

   //     [DataMember]
   //     public ICollection<string> ModifiedProperties { get; set; }
        
    //  [DataMember]//JsonProperty, 
    //	private Guid EntityIdentifier { get; set; }
    
    //	[DataMember]//JsonProperty, 
    //	private Guid _entityIdentity = default(Guid);
    
    	bool IEquatable<AsycudaDocumentEntryData>.Equals(AsycudaDocumentEntryData other)
    	{
    		if (EntityIdentifier != default(Guid))
    			return EntityIdentifier == other.EntityIdentifier;
    		return false;
    	}
    }
}



