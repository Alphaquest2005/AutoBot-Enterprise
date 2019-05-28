
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
        public string DutyFreePaid => Math.Abs((TaxAmount ?? 0)) < 0.0001 ? "Duty Free" : "Duty Paid";
    }
}


