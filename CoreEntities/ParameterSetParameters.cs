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
    
    public partial class ParameterSetParameters
    {
        public int Id { get; set; }
        public int ParameterSetId { get; set; }
        public int ParameterId { get; set; }
    
        public virtual Parameters Parameters { get; set; }
        public virtual ParameterSet ParameterSet { get; set; }
    }
}