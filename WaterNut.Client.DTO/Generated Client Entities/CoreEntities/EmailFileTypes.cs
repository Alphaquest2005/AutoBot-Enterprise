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
    public partial class EmailFileTypes : BaseEntity<EmailFileTypes>, ITrackable, IEquatable<EmailFileTypes>
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
        public int EmailMappingId
		{ 
		    get { return _EmailMappingId; }
			set
			{
			    if (value == _EmailMappingId) return;
				_EmailMappingId = value;
				NotifyPropertyChanged();//m => this.EmailMappingId
			}
		}
        private int _EmailMappingId;

        [DataMember]
        public int FileTypeId
		{ 
		    get { return _FileTypeId; }
			set
			{
			    if (value == _FileTypeId) return;
				_FileTypeId = value;
				NotifyPropertyChanged();//m => this.FileTypeId
			}
		}
        private int _FileTypeId;

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
        public EmailMapping EmailMapping
		{
		    get { return _EmailMapping; }
			set
			{
			    if (value == _EmailMapping) return;
				_EmailMapping = value;
                EmailMappingChangeTracker = _EmailMapping == null ? null
                    : new ChangeTrackingCollection<EmailMapping> { _EmailMapping };
				NotifyPropertyChanged();//m => this.EmailMapping
			}
		}
        private EmailMapping _EmailMapping;
        private ChangeTrackingCollection<EmailMapping> EmailMappingChangeTracker { get; set; }

        [DataMember]
        public FileTypes FileTypes
		{
		    get { return _FileTypes; }
			set
			{
			    if (value == _FileTypes) return;
				_FileTypes = value;
                FileTypesChangeTracker = _FileTypes == null ? null
                    : new ChangeTrackingCollection<FileTypes> { _FileTypes };
				NotifyPropertyChanged();//m => this.FileTypes
			}
		}
        private FileTypes _FileTypes;
        private ChangeTrackingCollection<FileTypes> FileTypesChangeTracker { get; set; }

   //     [DataMember]
   //     public TrackingState TrackingState { get; set; }

   //     [DataMember]
   //     public ICollection<string> ModifiedProperties { get; set; }
        
    //  [DataMember]//JsonProperty, 
    //	private Guid EntityIdentifier { get; set; }
    
    //	[DataMember]//JsonProperty, 
    //	private Guid _entityIdentity = default(Guid);
    
    	bool IEquatable<EmailFileTypes>.Equals(EmailFileTypes other)
    	{
    		if (EntityIdentifier != default(Guid))
    			return EntityIdentifier == other.EntityIdentifier;
    		return false;
    	}
    }
}


