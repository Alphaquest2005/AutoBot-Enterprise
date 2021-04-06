using System.ComponentModel.Composition.Hosting;
using Core.Common.Contracts;

namespace Core.Common
{
    public static class ClientObjectBase
    {
        private static CompositionContainer container;

        public static CompositionContainer Container
        {
            get => container;
            set
            {
                if (value != container)
                {
                    container = value;
                    ClientFactory = container.GetExportedValue<IClientServiceFactory>();
                }
            }
        }

        public static IClientServiceFactory ClientFactory { get; private set; }
    }
}