﻿// <autogenerated>
//   This file was generated by T4 code generator AllServices.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Contracts;
using OversShortQS.Business.Entities;

namespace OversShortQS.Business.Contracts
{

    public partial interface IOverShortDetailsEXService 
    {
        [OperationContract]
        Task MatchToCurrentItem(AsycudaDocumentItem currentDocumentItem, OverShortDetailsEX osd);
        
        [OperationContract]
        Task RemoveOverShortDetail(OverShortDetailsEX osd);
    }
}

