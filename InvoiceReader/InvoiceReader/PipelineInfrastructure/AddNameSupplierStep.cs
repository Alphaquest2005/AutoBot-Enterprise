using System;
using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities;
using OCR.Business.Entities;
using WaterNut.Business.Services.Utils;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using System.Threading.Tasks;
using System.Text;
using DocumentDS.Business.Entities;
using System.IO;
using WaterNut.DataSpace;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class ProcessInvoiceTemplateStep : IPipelineStep<InvoiceProcessingContext>
    {
        private static void AddNameSupplier(WaterNut.DataSpace.Invoice tmp, List<dynamic> csvLines)
        {
            if (csvLines.Count == 1 && !tmp.Lines.All(x => "Name, SupplierCode".Contains(x.OCR_Lines.Name)))
                foreach (var doc in ((List<IDictionary<string, object>>)csvLines.First()))
                {
                    if (!doc.Keys.Contains("SupplierCode")) doc.Add("SupplierCode", tmp.OcrInvoices.Name);
                    if (!doc.Keys.Contains("Name")) doc.Add("Name", tmp.OcrInvoices.Name);
                }
        }
    }
}