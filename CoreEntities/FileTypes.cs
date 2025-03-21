//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CoreEntities
{
    using System;
    using System.Collections.Generic;
    
    public partial class FileTypes
    {
        public FileTypes()
        {
            this.FileTypeMappings = new HashSet<FileTypeMappings>();
            this.FileTypeActions = new HashSet<FileTypeActions>();
            this.FileTypeContacts = new HashSet<FileTypeContacts>();
            this.AsycudaDocumentSet_Attachments = new HashSet<AsycudaDocumentSet_Attachments>();
            this.ChildFileTypes = new HashSet<FileTypes>();
            this.EmailFileTypes = new HashSet<EmailFileTypes>();
            this.ImportActions = new HashSet<ImportActions>();
            this.FileTypeReplaceRegex = new HashSet<FileTypeReplaceRegex>();
            this.AsycudaDocumentSetAttachments = new HashSet<AsycudaDocumentSetAttachments>();
        }
    
        public int Id { get; set; }
        public int ApplicationSettingsId { get; set; }
        public string FilePattern { get; set; }
        public bool CreateDocumentSet { get; set; }
        public bool DocumentSpecific { get; set; }
        public string DocumentCode { get; set; }
        public bool ReplyToMail { get; set; }
        public Nullable<int> FileGroupId { get; set; }
        public bool MergeEmails { get; set; }
        public bool CopyEntryData { get; set; }
        public Nullable<int> ParentFileTypeId { get; set; }
        public Nullable<bool> OverwriteFiles { get; set; }
        public Nullable<bool> HasFiles { get; set; }
        public Nullable<int> OldFileTypeId { get; set; }
        public Nullable<bool> ReplicateHeaderRow { get; set; }
        public Nullable<bool> IsImportable { get; set; }
        public Nullable<int> MaxFileSizeInMB { get; set; }
        public Nullable<int> FileInfoId { get; set; }
        public string Description { get; set; }
        public string DocSetRefernece { get; set; }
    
        public virtual ApplicationSettings ApplicationSettings { get; set; }
        public virtual ICollection<FileTypeMappings> FileTypeMappings { get; set; }
        public virtual ICollection<FileTypeActions> FileTypeActions { get; set; }
        public virtual ICollection<FileTypeContacts> FileTypeContacts { get; set; }
        public virtual ICollection<AsycudaDocumentSet_Attachments> AsycudaDocumentSet_Attachments { get; set; }
        public virtual FileGroups FileGroups { get; set; }
        public virtual ICollection<FileTypes> ChildFileTypes { get; set; }
        public virtual FileTypes ParentFileTypes { get; set; }
        public virtual ICollection<EmailFileTypes> EmailFileTypes { get; set; }
        public virtual ICollection<ImportActions> ImportActions { get; set; }
        public virtual ICollection<FileTypeReplaceRegex> FileTypeReplaceRegex { get; set; }
        public virtual FileImporterInfo FileImporterInfos { get; set; }
        public virtual ICollection<AsycudaDocumentSetAttachments> AsycudaDocumentSetAttachments { get; set; }
    }
}
