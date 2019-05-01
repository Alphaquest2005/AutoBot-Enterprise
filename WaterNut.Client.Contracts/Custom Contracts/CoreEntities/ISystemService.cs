using System.ServiceModel;
using Core.Common.Contracts;

namespace CoreEntities.Client.Contracts
{
    [ServiceContract(Namespace = "http://www.insight-software.com/WaterNut")]
    public partial interface ISystemService : IClientService
    {
        [OperationContract]
        bool ValidateInstallation();

    }
}

