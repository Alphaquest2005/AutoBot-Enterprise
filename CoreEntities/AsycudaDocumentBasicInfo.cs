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
    
    public partial class AsycudaDocumentBasicInfo
    {
        public int AsycudaDocumentSetId { get; set; }
        public int ASYCUDA_Id { get; set; }
        public string DocumentType { get; set; }
        public string CNumber { get; set; }
        public string Extended_customs_procedure { get; set; }
        public string National_customs_procedure { get; set; }
        public Nullable<System.DateTime> RegistrationDate { get; set; }
        public Nullable<System.DateTime> AssessmentDate { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public string Reference { get; set; }
        public Nullable<bool> IsManuallyAssessed { get; set; }
        public Nullable<bool> Cancelled { get; set; }
        public Nullable<bool> DoNotAllocate { get; set; }
        public int ApplicationSettingsId { get; set; }
        public bool ImportComplete { get; set; }
        public int Customs_ProcedureId { get; set; }
    }
}