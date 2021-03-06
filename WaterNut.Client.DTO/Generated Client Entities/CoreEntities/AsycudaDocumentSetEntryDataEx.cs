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
    public partial class AsycudaDocumentSetEntryDataEx : BaseEntity<AsycudaDocumentSetEntryDataEx>, ITrackable, IEquatable<AsycudaDocumentSetEntryDataEx>
    {
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
        public System.DateTime EntryDataDate
		{ 
		    get { return _EntryDataDate; }
			set
			{
			    if (value == _EntryDataDate) return;
				_EntryDataDate = value;
				NotifyPropertyChanged();//m => this.EntryDataDate
			}
		}
        private System.DateTime _EntryDataDate;

        [DataMember]
        public Nullable<double> InvoiceTotal
		{ 
		    get { return _InvoiceTotal; }
			set
			{
			    if (value == _InvoiceTotal) return;
				_InvoiceTotal = value;
				NotifyPropertyChanged();//m => this.InvoiceTotal
			}
		}
        private Nullable<double> _InvoiceTotal;

        [DataMember]
        public string SourceFile
		{ 
		    get { return _SourceFile; }
			set
			{
			    if (value == _SourceFile) return;
				_SourceFile = value;
				NotifyPropertyChanged();//m => this.SourceFile
			}
		}
        private string _SourceFile;

       
        [DataMember]
        public AsycudaDocumentSetEx AsycudaDocumentSetEx
		{
		    get { return _AsycudaDocumentSetEx; }
			set
			{
			    if (value == _AsycudaDocumentSetEx) return;
				_AsycudaDocumentSetEx = value;
                AsycudaDocumentSetExChangeTracker = _AsycudaDocumentSetEx == null ? null
                    : new ChangeTrackingCollection<AsycudaDocumentSetEx> { _AsycudaDocumentSetEx };
				NotifyPropertyChanged();//m => this.AsycudaDocumentSetEx
			}
		}
        private AsycudaDocumentSetEx _AsycudaDocumentSetEx;
        private ChangeTrackingCollection<AsycudaDocumentSetEx> AsycudaDocumentSetExChangeTracker { get; set; }

   //     [DataMember]
   //     public TrackingState TrackingState { get; set; }

   //     [DataMember]
   //     public ICollection<string> ModifiedProperties { get; set; }
        
    //  [DataMember]//JsonProperty, 
    //	private Guid EntityIdentifier { get; set; }
    
    //	[DataMember]//JsonProperty, 
    //	private Guid _entityIdentity = default(Guid);
    
    	bool IEquatable<AsycudaDocumentSetEntryDataEx>.Equals(AsycudaDocumentSetEntryDataEx other)
    	{
    		if (EntityIdentifier != default(Guid))
    			return EntityIdentifier == other.EntityIdentifier;
    		return false;
    	}
    }
}



