
using System;
using System.Linq;
using System.Reflection;
using CoreEntities.Business.Services;
using System.ComponentModel.Composition.Hosting;


namespace HRManager.Business.Bootstrapper
{
    public static class MEFLoader
    {
        public static CompositionContainer Init()
        {
            try
            {
                var catalog = new AggregateCatalog();

                catalog.Catalogs.Add(new AssemblyCatalog(typeof(ApplicationSettingsService).Assembly));
                var p = catalog.Parts.ToArray();

                var container = new CompositionContainer(catalog);

                return container; 
            }
            catch (Exception ex)
            {
               throw ex;
            }
        }



    }
}
