using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Business.Services;
using InventoryQS.Business.Entities;

namespace InventoryQS.Business.Services
{
  
    public partial interface IInventoryItemsExService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task AssignTariffToItms(List<int> list, string tariffCodes);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task ValidateExistingTariffCodes(int docSetId);

        //[OperationContract]
        //[FaultContract(typeof(ValidationFault))]
        //Task MapInventoryToAsycuda();

        [OperationContract]
        [FaultContract(typeof (ValidationFault))]
        Task SaveInventoryItemsEx(InventoryItemsEx olditm);

    }
}

