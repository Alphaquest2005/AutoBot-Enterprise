using System.ComponentModel.Composition.Hosting;

namespace Core.Common
{
    public abstract class BusinessObjectBase
    {
        private static CompositionContainer container;

        public static CompositionContainer Container
        {
            get => container;
            set
            {
                if (value != container) container = value;
            }
        }
    }
}