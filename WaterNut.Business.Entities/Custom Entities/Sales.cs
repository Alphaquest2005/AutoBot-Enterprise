

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
//using Newtonsoft.Json;
using TrackableEntities;
using Core.Common.Business.Entities;


namespace AllocationDS.Business.Entities
{
  
    public partial class Sales
    {
        [IgnoreDataMember]
        [NotMapped]
        public string DutyFreePaid
        {
            get { return this.TaxAmount == 0 ? "Duty Free" : "Duty Paid"; }
        }
    }
}


