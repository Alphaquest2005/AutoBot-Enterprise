
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using WaterNut.Interfaces;
using Core.Common.Client.Entities;
using PreviousDocumentQS.Client.DTO;
using TrackableEntities.Client;
using TrackableEntities;
using Core.Common.Validation;

namespace PreviousDocumentQS.Client.Entities
{
       public partial class PreviousDocumentItem
    {
           public double QtyAllocated { get { return DFQtyAllocated + DPQtyAllocated; } }

    }
}


