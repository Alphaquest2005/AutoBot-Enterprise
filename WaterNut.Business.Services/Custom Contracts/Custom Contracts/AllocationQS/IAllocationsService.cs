using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Business.Services;
using Core.Common.Contracts;
using CoreEntities.Business.Entities;

namespace AllocationQS.Business.Services
{
     [ServiceContract(Namespace = "http://www.insight-software.com/WaterNut")]
    public partial interface IAllocationsService : IBusinessService
    {

        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task CreateEx9(string filterExpression, bool perIM7, bool process7100, bool applyCurrentChecks,
            int AsycudaDocumentSetId, string documentType, string ex9BucketType, bool isGrouped,
            bool checkQtyAllocatedGreaterThanPiQuantity, bool checkForMultipleMonths, bool applyEx9Bucket,
            bool applyHistoricChecks, bool perInvoice, bool autoAssess, bool overPIcheck, bool universalPIcheck,
            bool itemPIcheck);
        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task CreateOPS(string filterExpression, int AsycudaDocumentSetId);
        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task ManuallyAllocate(int AllocationId, int PreviousItem_Id);
        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task ClearAllocations(IEnumerable<int> alst);
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task ClearAllAllocations(int applicationSettingsId);
        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task ClearAllocationsByFilter(string filterExpression);
        //[OperationContract][FaultContract(typeof(ValidationFault))]
        //Task CreateIncompOPS(string filterExpression, int AsycudaDocumentSetId);
        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task AllocateSales(ApplicationSettings applicationSettings, bool allocateToLastAdjustment);

        [OperationContract][FaultContract(typeof(ValidationFault))]
         Task ReBuildSalesReports();

        //[OperationContract][FaultContract(typeof(ValidationFault))]
        //Task ReBuildSalesReports(int asycuda_id);
    }
}

