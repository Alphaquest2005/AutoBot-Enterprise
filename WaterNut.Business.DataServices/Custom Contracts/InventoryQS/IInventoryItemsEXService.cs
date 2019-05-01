
using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Contracts;
using InventoryQS.Business.Entities;

namespace InventoryQS.Business.Contracts
{
  
    public partial interface IInventoryItemsExService
    {
        [OperationContract]
        Task AssignTariffToItms(System.Collections.IList list, TariffCodes tariffCodes);

    }
}

