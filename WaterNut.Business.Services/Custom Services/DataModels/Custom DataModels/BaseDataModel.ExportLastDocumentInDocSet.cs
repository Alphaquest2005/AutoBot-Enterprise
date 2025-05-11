using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.ServiceModel;
using Core.Common.Business.Services;
using Core.Common.UI;
using DocumentDS.Business.Entities;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    internal void ExportLastDocumentInDocSet(AsycudaDocumentSet docSet, string directoryName, bool overWrite)
    {
        StatusModel.StartStatusUpdate("Exporting Files", docSet.Documents.Count());
        var exceptions = new ConcurrentQueue<Exception>();
        if (!Directory.Exists(directoryName)) return;
        if (File.Exists(Path.Combine(directoryName, "Instructions.txt")))
            File.Delete(Path.Combine(directoryName, "Instructions.txt"));
        if (File.Exists(Path.Combine(directoryName, "InstructionResults.txt")))
            File.Delete(Path.Combine(directoryName, "InstructionResults.txt"));

        foreach (var doc in docSet.Documents.OrderByDescending(x => x.ASYCUDA_Id))

            try
            {
                var fileInfo = new FileInfo(Path.Combine(directoryName, doc.ReferenceNumber + ".xml"));
                if (overWrite || !File.Exists(fileInfo.FullName))
                    Instance.DocToXML(
                        Instance.CurrentApplicationSettings.DataFolder == null
                            ? fileInfo.DirectoryName
                            : Path.Combine(Instance.CurrentApplicationSettings.DataFolder,
                                docSet.Declarant_Reference_Number), doc, fileInfo);


                StatusModel.StatusUpdate();
            }
            catch (Exception ex)
            {
                exceptions.Enqueue(
                    new ApplicationException(
                        $"Could not import file - '{doc.ReferenceNumber}. Error:{ex.Message} Stacktrace:{ex.StackTrace}"));
            }

        ////}

        if (exceptions.Count <= 0) return;
        var fault = new ValidationFault
        {
            Result = false,
            Message = exceptions.First().Message,
            Description = exceptions.First().StackTrace
        };
        throw new FaultException<ValidationFault>(fault, new FaultReason(fault.Message));
    }
}