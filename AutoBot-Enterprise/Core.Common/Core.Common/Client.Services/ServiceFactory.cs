using System.ComponentModel.Composition;
using Core.Common.Contracts;

namespace Core.Common.Client.Services
{
    [Export(typeof(IClientServiceFactory))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ServiceFactory : IClientServiceFactory
    {
        TClient IClientServiceFactory.CreateClient<TClient>()
        {
            return ClientObjectBase.Container.GetExportedValue<TClient>();
        }
    }
}