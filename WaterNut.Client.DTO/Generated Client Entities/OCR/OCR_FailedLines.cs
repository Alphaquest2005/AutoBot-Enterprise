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

namespace OCR.Client.DTO
{

   // [JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace="http://www.insight-software.com/WaterNut")]
    public partial class OCR_FailedLines : BaseEntity<OCR_FailedLines>, ITrackable, IEquatable<OCR_FailedLines>
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
        public int DocSetAttachmentId
		{ 
		    get { return _DocSetAttachmentId; }
			set
			{
			    if (value == _DocSetAttachmentId) return;
				_DocSetAttachmentId = value;
				NotifyPropertyChanged();//m => this.DocSetAttachmentId
			}
		}
        private int _DocSetAttachmentId;

        [DataMember]
        public int LineId
		{ 
		    get { return _LineId; }
			set
			{
			    if (value == _LineId) return;
				_LineId = value;
				NotifyPropertyChanged();//m => this.LineId
			}
		}
        private int _LineId;

        [DataMember]
        public bool Resolved
		{ 
		    get { return _Resolved; }
			set
			{
			    if (value == _Resolved) return;
				_Resolved = value;
				NotifyPropertyChanged();//m => this.Resolved
			}
		}
        private bool _Resolved;

       
        [DataMember]
        public ImportErrors ImportErrors
		{
		    get { return _ImportErrors; }
			set
			{
			    if (value == _ImportErrors) return;
				_ImportErrors = value;
                ImportErrorsChangeTracker = _ImportErrors == null ? null
                    : new ChangeTrackingCollection<ImportErrors> { _ImportErrors };
				NotifyPropertyChanged();//m => this.ImportErrors
			}
		}
        private ImportErrors _ImportErrors;
        private ChangeTrackingCollection<ImportErrors> ImportErrorsChangeTracker { get; set; }

        [DataMember]
        public ChangeTrackingCollection<OCR_FailedFields> OCR_FailedFields
		{
		    get { return _OCR_FailedFields; }
			set
			{
			    if (Equals(value, _OCR_FailedFields)) return;
				_OCR_FailedFields = value;
				NotifyPropertyChanged();//m => this.OCR_FailedFields
			}
		}
        private ChangeTrackingCollection<OCR_FailedFields> _OCR_FailedFields = new ChangeTrackingCollection<OCR_FailedFields>();

        [DataMember]
        public Lines OCR_Lines
		{
		    get { return _OCR_Lines; }
			set
			{
			    if (value == _OCR_Lines) return;
				_OCR_Lines = value;
                OCR_LinesChangeTracker = _OCR_Lines == null ? null
                    : new ChangeTrackingCollection<Lines> { _OCR_Lines };
				NotifyPropertyChanged();//m => this.OCR_Lines
			}
		}
        private Lines _OCR_Lines;
        private ChangeTrackingCollection<Lines> OCR_LinesChangeTracker { get; set; }

   //     [DataMember]
   //     public TrackingState TrackingState { get; set; }

   //     [DataMember]
   //     public ICollection<string> ModifiedProperties { get; set; }
        
    //  [DataMember]//JsonProperty, 
    //	private Guid EntityIdentifier { get; set; }
    
    //	[DataMember]//JsonProperty, 
    //	private Guid _entityIdentity = default(Guid);
    
    	bool IEquatable<OCR_FailedLines>.Equals(OCR_FailedLines other)
    	{
    		if (EntityIdentifier != default(Guid))
    			return EntityIdentifier == other.EntityIdentifier;
    		return false;
    	}
    }
}


