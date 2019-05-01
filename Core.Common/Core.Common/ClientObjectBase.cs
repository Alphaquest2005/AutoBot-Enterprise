using Core.Common.Contracts;
using System.ComponentModel.Composition.Hosting;


namespace Core.Common
{
    public static class ClientObjectBase
    {
        static CompositionContainer container;
        public static CompositionContainer Container
        {
            get
            {
                return container;
            }
            set
            {
                if (value != container)
                {
                    container = value;
                    clientFactory = container.GetExportedValue<IClientServiceFactory>();
                }
            }
        }

        static IClientServiceFactory clientFactory;
        public static IClientServiceFactory ClientFactory
        {
            get
            {
                return clientFactory;
            }
        }
    }
}
