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
    
    public partial class FileImporterInfo
    {
        public FileImporterInfo()
        {
            this.FileTypes = new HashSet<FileTypes>();
        }
    
        public int Id { get; set; }
        public string EntryType { get; set; }
        public string Format { get; set; }
    
        public virtual ICollection<FileTypes> FileTypes { get; set; }
    }
}