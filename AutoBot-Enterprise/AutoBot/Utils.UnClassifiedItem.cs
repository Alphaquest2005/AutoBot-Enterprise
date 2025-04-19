using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Core.Common.Converters;
using Core.Common.Data.Contracts;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using TrackableEntities;
using TrackableEntities.Client;
using WaterNut.DataSpace;

namespace AutoBot
{
    public partial class Utils
    {
        public class UnClassifiedItem
        {
            public string InvoiceNo { get; set; }
            public int LineNumber { get; set; }
            public string ItemNumber { get; set; }
            public string ItemDescription { get; set; }
            public string TariffCode { get; set; }

        }
    }
}