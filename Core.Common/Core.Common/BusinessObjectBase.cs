using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common
{
    public abstract class BusinessObjectBase
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
                    
                }
            }
        }
    }
}
