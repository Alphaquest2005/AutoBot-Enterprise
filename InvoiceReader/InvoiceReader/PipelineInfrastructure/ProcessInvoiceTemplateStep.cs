using System;
using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities;
using OCR.Business.Entities;
using WaterNut.Business.Services.Utils;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using System.Threading.Tasks; // Added using directive
using System.Text; // Added using directive
using DocumentDS.Business.Entities; // Added using directive
using System.IO; // Added using directive

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class ProcessInvoiceTemplateStep : IPipelineStep<InvoiceProcessingContext>
    {
        private readonly WaterNut.DataSpace.Invoice _template;
        private readonly bool _isLastTemplate;

        public ProcessInvoiceTemplateStep(Invoice template, bool isLastTemplate)
        {
            _template = template;
            _isLastTemplate = isLastTemplate;
        }








    }
}