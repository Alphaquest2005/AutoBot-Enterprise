

using System.ServiceModel;
using System.Threading.Tasks;

namespace SalesDataQS.Client.Contracts
{
   
    public partial interface ISalesDataDetailService 
    {
        [OperationContract]
         Task SaveSalesDataDetail(DTO.SalesDataDetail salesDataDetail);
    }
}

