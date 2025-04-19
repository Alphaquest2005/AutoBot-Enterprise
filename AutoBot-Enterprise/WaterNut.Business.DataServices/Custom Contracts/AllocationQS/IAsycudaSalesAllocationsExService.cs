
using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Business.Services;
using Core.Common.Contracts;
using AllocationQS.Business.Entities;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using PreviousDocumentQS.Business.Entities;

namespace WaterNut.Business.DataServices
{
    
    public partial interface IAsycudaSalesAllocationsExService
    {

        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task CreateEx9(string filterExpression, bool perIM7, bool applyEx9Bucket, bool breakOnMonthYear, int AsycudaDocumentSetId);
        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task CreateOPS(string filterExpression, int AsycudaDocumentSetId);
        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task ManuallyAllocate(int AllocationId, int PreviousItem_Id);
        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task ClearAllocations(IEnumerable<int> alst);
        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task ClearAllocations(string filterExpression);
        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task CreateIncompOPS(string filterExpression, int AsycudaDocumentSetId);
        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task AllocateSales(bool itemDescriptionContainsAsycudaAttribute);

    }
}

