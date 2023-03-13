

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using DocumentDS.Business.Services;
using DocumentItemDS.Business.Services;
using WaterNut.Business.Entities;
using WaterNut.DataSpace;

namespace CoreEntities.Business.Services
{
  
    public partial class AsycudaDocumentService
    {
        public async Task SaveDocument(AsycudaDocument entity)
        {
            await WaterNut.DataSpace.BaseDataModel.Instance.SaveAsycudaDocument(entity).ConfigureAwait(false);
        }

        public async Task SaveDocumentCT(AsycudaDocument entity)
        {
            var ct = new DocumentCT
            {
                Document = await WaterNut.DataSpace.BaseDataModel.Instance.GetDocument(entity.ASYCUDA_Id, new List<string>()
                {
                    "xcuda_ASYCUDA_ExtendedProperties",
                    "xcuda_Identification",
                    "xcuda_Valuation.xcuda_Gs_Invoice",
                    "xcuda_Declarant",
                    "xcuda_General_information.xcuda_Country",
                    "xcuda_Property"

                }).ConfigureAwait(false)
            };

            using (var ctx = new xcuda_ItemService())
            {
                var res = (await ctx.Getxcuda_ItemByASYCUDA_Id(entity.ASYCUDA_Id.ToString(), new List<string>()
                {
                    "xcuda_Previous_doc"
                }).ConfigureAwait(false));
                if(res != null)
                ct.DocumentItems = res.ToList();
            }
            // bl
            foreach (var itm in ct.DocumentItems.Where(x => x.xcuda_Previous_doc.Summary_declaration != entity.BLNumber))
            {
                itm.xcuda_Previous_doc.StartTracking();
                itm.xcuda_Previous_doc.Summary_declaration = entity.BLNumber;
            }
            ct.Document.xcuda_Identification.StartTracking();
            ct.Document.xcuda_Identification.Manifest_reference_number = entity.Manifest_reference_number;
            ct.Document.xcuda_Valuation.xcuda_Gs_Invoice.StartTracking();
            ct.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code = entity.Currency_code;
            ct.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_rate = entity.Currency_rate.GetValueOrDefault();
            ct.Document.xcuda_ASYCUDA_ExtendedProperties.StartTracking();
            ct.Document.xcuda_ASYCUDA_ExtendedProperties.Description = entity.Description;
            ct.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId = entity.Customs_ProcedureId;
            ct.Document.xcuda_Declarant.StartTracking();
            ct.Document.xcuda_Declarant.Number =
                $"{entity.ReferenceNumber}-F{ct.Document.xcuda_ASYCUDA_ExtendedProperties.FileNumber}";
            ct.Document.xcuda_General_information.xcuda_Country.StartTracking();
            ct.Document.xcuda_General_information.xcuda_Country.Country_first_destination =
                entity.Country_first_destination;

            await WaterNut.DataSpace.BaseDataModel.Instance.SaveDocumentCt.Execute(ct).ConfigureAwait(false);
        }

        public async Task DeleteDocument(int asycudaDocumentId)
        {
            xcuda_ASYCUDA doc = null;
            using (var ctx = new xcuda_ASYCUDAService())
            {
                doc = await ctx.Getxcuda_ASYCUDAByKey(asycudaDocumentId.ToString(), new List<string>() { "xcuda_ASYCUDA_ExtendedProperties" }).ConfigureAwait(false);
            }
            await BaseDataModel.Instance.DeleteAsycudaDocument(doc).ConfigureAwait(false);
            await BaseDataModel.Instance.CalculateDocumentSetFreight(doc.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId).ConfigureAwait(false);
        }

        public async Task ExportDocument(string fileName, int asycudaDocumentId)
        {
            var doc = await WaterNut.DataSpace.BaseDataModel.Instance.GetDocument(asycudaDocumentId).ConfigureAwait(false);
            await WaterNut.DataSpace.BaseDataModel.Instance.ExportDocument(fileName, doc).ConfigureAwait(false);
        }

        public async Task<AsycudaDocument> NewDocument(int docSetId)
        {
            var docSet = await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false);
            var doc = WaterNut.DataSpace.BaseDataModel.Instance.CreateNewAsycudaDocument(docSet);
            using (var ctx = new AsycudaDocumentService())
            {
                return await ctx.GetAsycudaDocumentByKey(doc.ASYCUDA_Id.ToString()).ConfigureAwait(false);
            }
        }

        public void IM72Ex9Document(string filename)
        {
            BaseDataModel.Instance.IM72Ex9Document(filename);
        }
    }
}



