//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WaterNutDB
{
    #pragma warning disable 1573
    using System;
    using System.Collections.Generic;
    
    public partial class xcuda_Valuation
    {
        public string Calculation_working_mode { get; set; }
        public float Total_cost { get; set; }
        public float Total_CIF { get; set; }
        public int ASYCUDA_Id { get; set; }
    
        public virtual xcuda_ASYCUDA xcuda_ASYCUDA { get; set; }
        public virtual xcuda_Gs_deduction xcuda_Gs_deduction { get; set; }
        public virtual xcuda_Gs_external_freight xcuda_Gs_external_freight { get; set; }
        public virtual xcuda_Gs_insurance xcuda_Gs_insurance { get; set; }
        public virtual xcuda_Gs_internal_freight xcuda_Gs_internal_freight { get; set; }
        public virtual xcuda_Gs_Invoice xcuda_Gs_Invoice { get; set; }
        public virtual xcuda_Gs_other_cost xcuda_Gs_other_cost { get; set; }
        public virtual xcuda_Total xcuda_Total { get; set; }
        public virtual xcuda_Weight xcuda_Weight { get; set; }
    }
}
