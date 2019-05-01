using Core.Common.Contracts;
using CoreEntities.Client.Services;
using WaterNut.Client.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterNut.Client.Bootstrapper
{
    public static class MEFLoader
    {
        public static CompositionContainer Init()
        {
            return Init(null);
        }

        public static  CompositionContainer Init(ICollection<ComposablePartCatalog> catalogParts)
        {
            var catalog = new AggregateCatalog();

            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ApplicationSettingsClient).Assembly));
            //catalog.Catalogs.Add(new AssemblyCatalog(typeof(ServiceFactory).Assembly));

            if (catalogParts != null)
                foreach (var part in catalogParts)
                    catalog.Catalogs.Add(part);

            var container = new CompositionContainer(catalog);

            return container;
        }

        

    }
}
