// Assuming Invoice and related entities are here

// Assuming OCR_Lines and Fields are here

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class AddMissingRequiredFieldValuesStep : IPipelineStep<InvoiceProcessingContext>
    {
        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            if (context.Template != null && context.CsvLines != null)
            {
                var requiredFieldsList = GetRequiredFieldsWithValues(context);

                AddRequiredFieldValuesToDocuments(context, requiredFieldsList);

                Console.WriteLine($"[OCR DEBUG] Pipeline Step: Added missing required field values.");

                return true; // Indicate success
            }

            // Required data is missing
            return false;
        }

        private void AddRequiredFieldValuesToDocuments(InvoiceProcessingContext context, List<OCR.Business.Entities.Fields> requiredFieldsList)
        {
            foreach (var field in requiredFieldsList)
            {
                foreach (var doc in ((List<IDictionary<string, object>>)context.CsvLines.First()))
                {
                    AddMissingFieldToDocument(doc, field);
                }
            }
        }

        private List<OCR.Business.Entities.Fields> GetRequiredFieldsWithValues(InvoiceProcessingContext context)
        {
            return context.Template.Lines.SelectMany(x => x.OCR_Lines.Fields)
                .Where(z => z.IsRequired && !string.IsNullOrEmpty(z.FieldValue?.Value)).ToList();
        }

        private void AddMissingFieldToDocument(IDictionary<string, object> doc, OCR.Business.Entities.Fields field)
        {
            if (!doc.Keys.Contains(field.Field))
            {
                doc.Add(field.Field, field.FieldValue.Value);
            }
        }
    }
}
