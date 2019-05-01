using Core.Common;
using Core.Common.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

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
