using System;
using System.Collections.Concurrent;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Asycuda421;
using CoreEntities.Business.Entities;
using WaterNut.DataSpace.Asycuda;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    private async Task LoadLicence(AsycudaDocumentSet docSet, string file, ConcurrentQueue<Exception> exceptions)
    {
        try
        {
            var a = Licence.LoadFromFile(file);
            var fileinfo = new FileInfo(file);
            if (a.General_segment.Exporter_address.Text.Any(x => x.Contains(fileinfo.Name.Replace("-LIC.xml", "")))
               ) return;
            using (var ctx = new CoreEntitiesContext())
            {
                var declarants = ctx.ApplicationSettings
                    .Include(x => x.Declarants)
                    .First(x => x.ApplicationSettingsId == docSet.ApplicationSettingsId).Declarants;
                var fileCode = a.General_segment.Importer_code.Text.FirstOrDefault();
                if (fileCode == null) return;
                if (!declarants.Any(x => fileCode.Contains(x.DeclarantCode)))
                    return;
                // throw new ApplicationException( $"Could not import file - '{file} - The file is for another warehouse{fileCode}. While this Warehouse is {declarants.First().DeclarantCode}");
            }


            if (a != null)
            {
                var importer = new LicenseToDataBase();
                await importer.SaveToDatabase(a, docSet, new FileInfo(file)).ConfigureAwait(false);
            }

            Debug.WriteLine(file);
        }

        catch (Exception Ex)
        {
            exceptions.Enqueue(
                new ApplicationException(
                    $"Could not import file - '{file}. Error:{(Ex.InnerException ?? Ex).Message} Stacktrace:{(Ex.InnerException ?? Ex).StackTrace}"));
        }
    }
}