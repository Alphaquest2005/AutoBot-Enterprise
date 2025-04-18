﻿// <autogenerated>
//   This file was generated by T4 code generator AllBusinessEntities.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
//using Newtonsoft.Json;

using Core.Common.Business.Entities;
using WaterNut.Interfaces;
using TrackableEntities;

namespace DocumentDS.Business.Entities
{

    //[JsonObject(IsReference = true)]
    [DataContract(IsReference = true, Namespace="http://www.insight-software.com/WaterNut")]
    public partial class FileType : BaseEntity<FileType>, ITrackable 
    {
        partial void AutoGenStartUp() //FileType()
        {
            this.AsycudaDocumentSet_Attachments = new List<AsycudaDocumentSet_Attachments>();
            this.FileTypes1 = new List<FileType>();
        }

        [DataMember]
        public int Id 
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        int _id;
        [DataMember]
        public int ApplicationSettingsId 
        {
            get
            {
                return _applicationsettingsid;
            }
            set
            {
                _applicationsettingsid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        int _applicationsettingsid;
        [DataMember]
        public string FilePattern 
        {
            get
            {
                return _filepattern;
            }
            set
            {
                _filepattern = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _filepattern;
        [DataMember]
        public bool CreateDocumentSet 
        {
            get
            {
                return _createdocumentset;
            }
            set
            {
                _createdocumentset = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        bool _createdocumentset;
        [DataMember]
        public bool DocumentSpecific 
        {
            get
            {
                return _documentspecific;
            }
            set
            {
                _documentspecific = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        bool _documentspecific;
        [DataMember]
        public string DocumentCode 
        {
            get
            {
                return _documentcode;
            }
            set
            {
                _documentcode = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _documentcode;
        [DataMember]
        public bool ReplyToMail 
        {
            get
            {
                return _replytomail;
            }
            set
            {
                _replytomail = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        bool _replytomail;
        [DataMember]
        public Nullable<int> FileGroupId 
        {
            get
            {
                return _filegroupid;
            }
            set
            {
                _filegroupid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<int> _filegroupid;
        [DataMember]
        public bool MergeEmails 
        {
            get
            {
                return _mergeemails;
            }
            set
            {
                _mergeemails = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        bool _mergeemails;
        [DataMember]
        public bool CopyEntryData 
        {
            get
            {
                return _copyentrydata;
            }
            set
            {
                _copyentrydata = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        bool _copyentrydata;
        [DataMember]
        public Nullable<int> ParentFileTypeId 
        {
            get
            {
                return _parentfiletypeid;
            }
            set
            {
                _parentfiletypeid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<int> _parentfiletypeid;
        [DataMember]
        public Nullable<bool> OverwriteFiles 
        {
            get
            {
                return _overwritefiles;
            }
            set
            {
                _overwritefiles = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<bool> _overwritefiles;
        [DataMember]
        public Nullable<bool> HasFiles 
        {
            get
            {
                return _hasfiles;
            }
            set
            {
                _hasfiles = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<bool> _hasfiles;
        [DataMember]
        public Nullable<int> OldFileTypeId 
        {
            get
            {
                return _oldfiletypeid;
            }
            set
            {
                _oldfiletypeid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<int> _oldfiletypeid;
        [DataMember]
        public Nullable<bool> ReplicateHeaderRow 
        {
            get
            {
                return _replicateheaderrow;
            }
            set
            {
                _replicateheaderrow = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<bool> _replicateheaderrow;
        [DataMember]
        public Nullable<bool> IsImportable 
        {
            get
            {
                return _isimportable;
            }
            set
            {
                _isimportable = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<bool> _isimportable;
        [DataMember]
        public Nullable<int> MaxFileSizeInMB 
        {
            get
            {
                return _maxfilesizeinmb;
            }
            set
            {
                _maxfilesizeinmb = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<int> _maxfilesizeinmb;
        [DataMember]
        public Nullable<int> FileInfoId 
        {
            get
            {
                return _fileinfoid;
            }
            set
            {
                _fileinfoid = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        Nullable<int> _fileinfoid;
        [DataMember]
        public string Description 
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _description;
        [DataMember]
        public string DocSetRefernece 
        {
            get
            {
                return _docsetrefernece;
            }
            set
            {
                _docsetrefernece = value;
                //if(this.TrackingState == TrackingState.Unchanged) this.TrackingState = TrackingState.Modified;  
                NotifyPropertyChanged();
            }
        }
        string _docsetrefernece;
        [DataMember]
        public List<AsycudaDocumentSet_Attachments> AsycudaDocumentSet_Attachments { get; set; }
        [DataMember]
        public List<FileType> FileTypes1 { get; set; }
        [DataMember]
        public FileType FileType1 { get; set; }

 //       [DataMember]
 //       public TrackingState TrackingState { get; set; }
 //       [DataMember]
 //       public ICollection<string> ModifiedProperties { get; set; }
//        [DataMember]//JsonProperty,
 //       private Guid EntityIdentifier { get; set; }
    }
}


