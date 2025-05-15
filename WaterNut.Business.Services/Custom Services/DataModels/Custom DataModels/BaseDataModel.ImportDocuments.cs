using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asycuda421;
using DocumentDS.Business.Entities;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    public async Task ImportDocuments(int asycudaDocumentSetId, List<string> fileNames,
        bool importOnlyRegisteredDocument, bool importTariffCodes, bool noMessages, bool overwriteExisting,
        bool linkPi, Serilog.ILogger log)
    {
        using (var ctx = new DocumentDSContext())
        {
            var docSet =
                ctx.AsycudaDocumentSets.FirstOrDefault(x => x.AsycudaDocumentSetId == asycudaDocumentSetId);
            if (docSet == null) throw new ApplicationException("Document Set with reference not found");
            await Task.Run(() =>
                    ImportDocuments(docSet, importOnlyRegisteredDocument, importTariffCodes, noMessages,
                        overwriteExisting, linkPi, fileNames, log))
                .ConfigureAwait(false);
        }
    }

    public async Task ImportDocuments(AsycudaDocumentSet docSet, IEnumerable<string> fileNames,
        bool importOnlyRegisteredDocument, bool importTariffCodes, bool noMessages, bool overwriteExisting,
        bool linkPi, Serilog.ILogger log)
    {
        await Task.Run(() =>
                ImportDocuments(docSet, importOnlyRegisteredDocument, importTariffCodes, noMessages,
                    overwriteExisting, linkPi, fileNames, log))
            .ConfigureAwait(false);
    }

    private async Task ImportDocuments(AsycudaDocumentSet docSet, bool importOnlyRegisteredDocument,
        bool importTariffCodes, bool noMessages,
        bool overwriteExisting, bool linkPi, IEnumerable<string> fileNames, Serilog.ILogger log)
    {
        var exceptions = new ConcurrentQueue<Exception>();
        //Parallel.ForEach(fileNames,
        //    new ParallelOptions
        //        {MaxDegreeOfParallelism = 1}, // Environment.ProcessorCount * // have to fix deadlock issue first
        //    f => //
        foreach (var f in fileNames)
        {
            try
            {
                if (ASYCUDA.CanLoadFromFile(f))
                {
                    await LoadAsycuda421(docSet, importOnlyRegisteredDocument, importTariffCodes, noMessages,
                        overwriteExisting, linkPi, f, exceptions).ConfigureAwait(false);
                }
                else if (Value_declaration_form.CanLoadFromFile(f))
                {
                    LoadC71(docSet, f, ref exceptions, log);
                }
                else if (Licence.CanLoadFromFile(f))
                {
                    await LoadLicence(docSet, f, exceptions).ConfigureAwait(false);
                }
                else
                {
                    if (!noMessages)
                        throw new ApplicationException($"Can not Load file '{f}'");
                }
            }
            catch (Exception ex)
            {
                exceptions.Enqueue(ex);
            }
        }
        // );

        if (exceptions.Count > 0)
            throw new ApplicationException(exceptions.FirstOrDefault().Message + "|" +
                                           exceptions.FirstOrDefault().StackTrace);
    }
}