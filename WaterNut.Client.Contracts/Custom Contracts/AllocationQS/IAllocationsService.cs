
using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Contracts;
using AllocationQS.Client.DTO;
using CoreEntities.Client.DTO;
using PreviousDocumentQS.Client.DTO;

namespace AllocationQS.Client.Contracts
{
    [ServiceContract(Namespace = "http://www.insight-software.com/WaterNut")]
    public partial interface IAllocationsService : IClientService
    {
        [OperationContract]
        Task<IEnumerable<AsycudaSalesAllocationsEx>> GetAsycudaSalesAllocationsExesByDates(DateTime startDate, DateTime endDate);
        [OperationContract]
        Task CreateEx9(string filterExpression, bool perIM7, bool process7100, bool applyCurrentChecks, int AsycudaDocumentSetId);
        [OperationContract]
        Task CreateOPS(string filterExpression, int AsycudaDocumentSetId);
        [OperationContract]
        Task ManuallyAllocate(int AllocationId, int PreviousItem_Id);
        [OperationContract]
        Task ClearAllocations(IEnumerable<int> alst);
        [OperationContract]
        Task ClearAllAllocations();
        [OperationContract]
        Task ClearAllocationsByFilter(string filterExpression);
        [OperationContract]
        Task CreateIncompOPS(string filterExpression, int AsycudaDocumentSetId);
        [OperationContract]
        Task AllocateSales(bool itemDescriptionContainsAsycudaAttribute, bool allocateToLastAdjustment);

        [OperationContract]
        Task ReBuildSalesReports();

        //[OperationContract]
        //Task ReBuildSalesReports(int asycuda_Id);

    }
}

