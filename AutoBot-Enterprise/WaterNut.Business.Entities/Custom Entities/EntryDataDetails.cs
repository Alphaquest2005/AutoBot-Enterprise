
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
//using Newtonsoft.Json;
using TrackableEntities;
using Core.Common.Business.Entities;


namespace AllocationDS.Business.Entities
{
   
    public partial class EntryDataDetails 
    {
        public string DutyFreePaid => Math.Abs((double)(Sales.Tax == null ? TaxAmount ?? 0 : Sales.Tax)) < 0.0001 ? "Duty Free" : "Duty Paid";
    }
}


