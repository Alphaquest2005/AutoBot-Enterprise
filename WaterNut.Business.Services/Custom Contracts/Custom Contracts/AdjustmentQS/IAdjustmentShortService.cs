using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Business.Services;

namespace AdjustmentQS.Business.Services
{
    public partial interface IAdjustmentShortService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task AutoMatch(int applicationSettingsId);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task MatchToAsycudaItem(int entryDataDetailId, int itemId);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task CreateIM9(string filterExpression, bool perInvoice, bool process7100, int asycudaDocumentSetId,
            string ex9Type, string dutyFreePaid, string adjustmentType);
    }
}
