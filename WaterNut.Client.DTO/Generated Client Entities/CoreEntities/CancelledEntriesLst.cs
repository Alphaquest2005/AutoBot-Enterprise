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
    public partial class CancelledEntriesLst : BaseEntity<CancelledEntriesLst>, ITrackable, IEquatable<CancelledEntriesLst>
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
        public string Office
		{ 
		    get { return _Office; }
			set
			{
			    if (value == _Office) return;
				_Office = value;
				NotifyPropertyChanged();//m => this.Office
			}
		}
        private string _Office;

        [DataMember]
        public string RegistrationNumber
		{ 
		    get { return _RegistrationNumber; }
			set
			{
			    if (value == _RegistrationNumber) return;
				_RegistrationNumber = value;
				NotifyPropertyChanged();//m => this.RegistrationNumber
			}
		}
        private string _RegistrationNumber;

        [DataMember]
        public string RegistrationDate
		{ 
		    get { return _RegistrationDate; }
			set
			{
			    if (value == _RegistrationDate) return;
				_RegistrationDate = value;
				NotifyPropertyChanged();//m => this.RegistrationDate
			}
		}
        private string _RegistrationDate;

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
    
    	bool IEquatable<CancelledEntriesLst>.Equals(CancelledEntriesLst other)
    	{
    		if (EntityIdentifier != default(Guid))
    			return EntityIdentifier == other.EntityIdentifier;
    		return false;
    	}
    }
}



