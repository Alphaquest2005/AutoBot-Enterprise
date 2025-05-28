using System;
using System.Collections.Concurrent;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Asycuda421;
using Core.Common.UI;
using CoreEntities.Business.Entities;
using WaterNut.DataSpace.Asycuda;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    private async Task LoadAsycuda421(AsycudaDocumentSet docSet, bool importOnlyRegisteredDocument,
        bool importTariffCodes,
        bool noMessages, bool overwriteExisting, bool linkPi, string f, ConcurrentQueue<Exception> exceptions)
    {
        StatusModel.StatusUpdate();
        try
        {
            var a = ASYCUDA.LoadFromFile(f);
            using (var ctx = new CoreEntitiesContext())
            {
                var declarants = ctx.ApplicationSettings
                    .Include(x => x.Declarants)
                    .First(x => x.ApplicationSettingsId == docSet.ApplicationSettingsId).Declarants;
                var fileCode = a.Warehouse.Identification.Text.FirstOrDefault() ??
                               a.Declarant.Declarant_code.Text.FirstOrDefault();
                if(BaseDataModel.Instance.CurrentApplicationSettings.AllowXBond == "Visible")
                if (!declarants.Any(x => fileCode.Contains(x.DeclarantCode)))
                    //throw new ApplicationException(
                    //    $"Could not import file - '{f} - The file is for another warehouse{fileCode}. While this Warehouse is {declarants.First().DeclarantCode}");
                   return;
            }


            if (a != null)
            {
                var importer = new AsycudaToDataBase421
                {
                    UpdateItemsTariffCode = importTariffCodes,
                    ImportOnlyRegisteredDocuments = importOnlyRegisteredDocument,
                    OverwriteExisting = overwriteExisting,
                    NoMessages = noMessages,
                    LinkPi = linkPi
                };
                await importer.SaveToDatabase(a, docSet, new FileInfo(f)).ConfigureAwait(false);
            }

            Debug.WriteLine(f);
        }

        catch (Exception Ex)
        {
            // if (!noMessages && (bool) !Ex?.InnerException?.Message.StartsWith("Please Import pCNumber"))
            exceptions.Enqueue(
                new ApplicationException(
                    $"Could not import file - '{f}. Error:{(Ex.InnerException ?? Ex).Message} Stacktrace:{(Ex.InnerException ?? Ex).StackTrace}"));
        }
    }
}