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
    
    public partial class SessionActions
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int ActionId { get; set; }
    
        public virtual Actions Actions { get; set; }
        public virtual Sessions Sessions { get; set; }
    }
}
