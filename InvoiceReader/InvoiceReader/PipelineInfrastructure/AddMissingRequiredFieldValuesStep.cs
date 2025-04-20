using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreEntities.Business.Entities; // Assuming Invoice and related entities are here
using OCR.Business.Entities; // Assuming OCR_Lines and Fields are here

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class AddMissingRequiredFieldValuesStep : IPipelineStep<InvoiceProcessingContext>
    {
        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            if (context.Template == null || context.CsvLines == null)
            {
                // Required data is missing
                return false;
            }

            // Logic from the original AddMissingRequiredFieldValues method
            var requiredFieldsList = context.Template.Lines.SelectMany(x => x.OCR_Lines.Fields)
                .Where(z => z.IsRequired && !string.IsNullOrEmpty(z.FieldValue?.Value)).ToList();
            foreach (var field in requiredFieldsList)
            {
                foreach (var doc in ((List<IDictionary<string, object>>)context.CsvLines.First()).Where(doc =>
                             !doc.Keys.Contains(field.Field)))
                {
                    doc.Add(field.Field, field.FieldValue.Value);
                }
            }

            System.Console.WriteLine(
                $"[OCR DEBUG] Pipeline Step: Added missing required field values.");

            return true; // Indicate success
        }
    }
}