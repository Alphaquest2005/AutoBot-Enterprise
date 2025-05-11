using System;
using System.Collections.Concurrent;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Asycuda421;
using CoreEntities.Business.Entities;
using WaterNut.DataSpace.Asycuda;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    private void LoadC71(AsycudaDocumentSet docSet, string file, ref ConcurrentQueue<Exception> exceptions)
    {
        try
        {
            var a = Value_declaration_form.LoadFromFile(file);
            using (var ctx = new CoreEntitiesContext())
            {
                var declarants = ctx.ApplicationSettings
                    .Include(x => x.Declarants)
                    .First(x => x.ApplicationSettingsId == docSet.ApplicationSettingsId).Declarants;
                var fileCode = a.Identification_segment.Declarant_segment.Code.Text.FirstOrDefault();
                if (!declarants.Any(x => fileCode.Contains(x.DeclarantCode)))
                {
                    BaseDataModel.EmailExceptionHandlerAsync(new ApplicationException(
                            $"Could not import file - '{file} - The file is for another warehouse{fileCode}. While this Warehouse is {declarants.First().DeclarantCode}"),
                        true).GetAwaiter().GetResult();
                    return;
                }
            }


            if (a != null)
            {
                var importer = new C71ToDataBase();
                importer.SaveToDatabase(a, docSet, new FileInfo(file));
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