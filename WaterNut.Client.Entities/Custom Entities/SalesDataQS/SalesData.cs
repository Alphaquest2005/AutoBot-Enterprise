


using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using WaterNut.Interfaces;

using SalesDataQS.Client.DTO;
using TrackableEntities.Client;
using Core.Common.Validation;

namespace SalesDataQS.Client.Entities
{
       public partial class SalesData
    {

           public string DutyFreePaid { get; set; }
       
    }
}


