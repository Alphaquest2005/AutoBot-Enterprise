﻿// <autogenerated>
//   This file was generated by T4 code generator AllServices.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using DocumentItemDS.Business.Entities;
using Core.Common.Business.Services;

namespace DocumentItemDS.Business.Services
{
    
    public partial interface Ixcuda_ItemService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<bool> Updatexcuda_Items(List<xcuda_Item> entities);
    }
}

