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
    
    public partial class Customs_Procedure
    {
        public int Document_TypeId { get; set; }
        public int Customs_ProcedureId { get; set; }
        public string Extended_customs_procedure { get; set; }
        public string National_customs_procedure { get; set; }
        public Nullable<bool> IsDefault { get; set; }
        public Nullable<bool> IsImportExport { get; set; }
        public string CustomsProcedure { get; set; }
    
        public virtual Document_Type Document_Type { get; set; }
    }
}