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
    
    public partial class Contacts
    {
        public Contacts()
        {
            this.FileTypeContacts = new HashSet<FileTypeContacts>();
        }
    
        public int Id { get; set; }
        public string Role { get; set; }
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public int ApplicationSettingsId { get; set; }
        public string CellPhone { get; set; }
    
        public virtual ICollection<FileTypeContacts> FileTypeContacts { get; set; }
    }
}