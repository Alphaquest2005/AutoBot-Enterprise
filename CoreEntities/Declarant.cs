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
    
    public partial class Declarant
    {
        public int Id { get; set; }
        public int ApplicationSettingsId { get; set; }
        public string DeclarantCode { get; set; }
        public bool IsDefault { get; set; }
    
        public virtual ApplicationSettings ApplicationSettings { get; set; }
    }
}