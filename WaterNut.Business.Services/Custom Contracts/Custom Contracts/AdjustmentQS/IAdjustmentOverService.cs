using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Business.Services;

namespace AdjustmentQS.Business.Services
{
    public partial interface IAdjustmentOverService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task CreateOPS(string filterExpression, bool perInvoice, string adjustmentType, int asycudaDocumentSetId);
    }
}
