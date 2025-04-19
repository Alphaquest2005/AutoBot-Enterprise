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
        private static bool ErrorState(string file, string emailId, string pdftxt, EmailDownloader.Client client,
            List<AsycudaDocumentSet> docSet,
            WaterNut.DataSpace.Invoice tmp, int fileTypeId, bool isLastdoc)
        {
            var failedlines = tmp.Lines.DistinctBy(x => x.OCR_Lines.Id).Where(z =>
                z.FailedFields.Any() || (z.OCR_Lines.Fields.Any(f => f.IsRequired && f.FieldValue?.Value == null) &&
                                         !z.Values.Any())).ToList();

            failedlines.AddRange(tmp.Parts.SelectMany(z => z.FailedLines).ToList());

            var allRequried = tmp.Lines.DistinctBy(x => x.OCR_Lines.Id).Where(z =>
                z.OCR_Lines.Fields.Any(f => f.IsRequired && (f.Field != "SupplierCode" && f.Field != "Name"))).ToList();


            if (
                //---------Auto Add name and supplier code make this check redundant
                //!tmp.Parts.Any(x => x.AllLines.Any(z =>
                //    z.Values.Values.Any(v =>
                //        v.Keys.Any(k => k.fields.Field == "Name") &&
                //        v.Values.Any(kv => kv == tmp.OcrInvoices.Name))))) ||
                failedlines.Count >= allRequried.Count && allRequried.Count > 0) return false;

            if ( //isLastdoc &&
                failedlines.Any() && failedlines.Count < tmp.Lines.Count &&
                (tmp.Parts.First().WasStarted || !tmp.Parts.First().OCR_Part.Start.Any()) &&
                tmp.Lines.SelectMany(x => x.Values.Values).Any())
            {
                InvoiceProcessingUtils.ReportUnimportedFile(docSet, file, emailId, fileTypeId, client, pdftxt.ToString(),
                    "Following fields failed to import",
                    failedlines);

                return true;
            }


            return false;
        }
    }
}