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
        private static void AddMissingRequiredFieldValues(WaterNut.DataSpace.Invoice tmp, List<dynamic> csvLines)
        {
            var requiredFieldsList = tmp.Lines.SelectMany(x => x.OCR_Lines.Fields)
                .Where(z => z.IsRequired && !string.IsNullOrEmpty(z.FieldValue?.Value)).ToList();
            foreach (var field in requiredFieldsList)
            {
                foreach (var doc in ((List<IDictionary<string, object>>)csvLines.First()).Where(doc =>
                             !doc.Keys.Contains(field.Field)))
                {
                    doc.Add(field.Field, field.FieldValue.Value);
                }
            }
        }
    }
}