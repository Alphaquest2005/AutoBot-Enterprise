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
    
    public partial class Parameters
    {
        public Parameters()
        {
            this.ParameterSetParameters = new HashSet<ParameterSetParameters>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    
        public virtual ICollection<ParameterSetParameters> ParameterSetParameters { get; set; }
    }
}
