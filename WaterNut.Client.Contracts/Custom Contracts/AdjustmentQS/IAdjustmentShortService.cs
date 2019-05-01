using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using AdjustmentQS.Client.DTO;
using Core.Common.Contracts;

namespace AdjustmentQS.Client.Contracts
{
    public partial interface IAdjustmentShortService 
    {
        [OperationContract]
        Task AutoMatch();
        [OperationContract]
        Task CreateIM9(string filterExpression, bool perInvoice, bool process7100, int asycudaDocumentSetId,string ex9Type, string dutyFreePaid);
    }

    public partial interface IAdjustmentOverService
    {
        [OperationContract]
        Task CreateOPS(string filterExpression, object perInvoice, int asycudaDocumentSetId);
    }
}
