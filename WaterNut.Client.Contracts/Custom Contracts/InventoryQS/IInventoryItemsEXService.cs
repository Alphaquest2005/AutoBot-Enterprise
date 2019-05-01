
using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Contracts;
using InventoryQS.Client.DTO;

namespace InventoryQS.Client.Contracts
{
  
    public partial interface IInventoryItemsExService
    {
        [OperationContract]
        Task AssignTariffToItms(IEnumerable<string> list, string tariffCodes);

        [OperationContract]
        Task ValidateExistingTariffCodes(int docSetId);


        [OperationContract]
        Task SaveInventoryItemsEx(InventoryItemsEx olditm);

    }
}

