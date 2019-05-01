using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Core.Common.Contracts
{
    public interface IClientServiceFactory
    {
        TClient CreateClient<TClient>()
            where TClient : IClientService; 
       
    }
}
