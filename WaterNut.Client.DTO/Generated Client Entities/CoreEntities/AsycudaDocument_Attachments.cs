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
    public partial class AsycudaDocument_Attachments : BaseEntity<AsycudaDocument_Attachments>, ITrackable, IEquatable<AsycudaDocument_Attachments>
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
        public int AttachmentId
		{ 
		    get { return _AttachmentId; }
			set
			{
			    if (value == _AttachmentId) return;
				_AttachmentId = value;
				NotifyPropertyChanged();//m => this.AttachmentId
			}
		}
        private int _AttachmentId;

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
        public Attachments Attachments
		{
		    get { return _Attachments; }
			set
			{
			    if (value == _Attachments) return;
				_Attachments = value;
                AttachmentsChangeTracker = _Attachments == null ? null
                    : new ChangeTrackingCollection<Attachments> { _Attachments };
				NotifyPropertyChanged();//m => this.Attachments
			}
		}
        private Attachments _Attachments;
        private ChangeTrackingCollection<Attachments> AttachmentsChangeTracker { get; set; }

   //     [DataMember]
   //     public TrackingState TrackingState { get; set; }

   //     [DataMember]
   //     public ICollection<string> ModifiedProperties { get; set; }
        
    //  [DataMember]//JsonProperty, 
    //	private Guid EntityIdentifier { get; set; }
    
    //	[DataMember]//JsonProperty, 
    //	private Guid _entityIdentity = default(Guid);
    
    	bool IEquatable<AsycudaDocument_Attachments>.Equals(AsycudaDocument_Attachments other)
    	{
    		if (EntityIdentifier != default(Guid))
    			return EntityIdentifier == other.EntityIdentifier;
    		return false;
    	}
    }
}



