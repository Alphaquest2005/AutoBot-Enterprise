using System.ServiceModel;
using Core.Common.Business.Services;
using Core.Common.Contracts;

namespace CoreEntities.Business.Services
{
    [ServiceContract(Namespace = "http://www.insight-software.com/WaterNut")]
    public interface ISystemService: IBusinessService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        bool ValidateInstallation();
    }
}
