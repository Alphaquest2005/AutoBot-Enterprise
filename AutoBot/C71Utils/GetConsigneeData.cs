using System.Linq;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, AsycudaDocumentSet, Consignees, Customs_Procedure, ExportTemplates are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class C71Utils
    {
        private static string GetConsigneeData(out string consigneeName, out string consigneeAddress, int asycudaDocumentSetId)
        {
            // --- Fetch Consignee Info ---
            string consigneeCode = null;
            consigneeName = null;
            consigneeAddress = null;
            // Assuming ApplicationSettingsId is available in TODO_C71ToCreate

            if (asycudaDocumentSetId != 0)
            {
                using (var coreCtx = new CoreEntitiesContext())
                {
                    var docSetData = coreCtx.AsycudaDocumentSet
                        .Where(ds => ds.AsycudaDocumentSetId == asycudaDocumentSetId)
                        .Select(ds => new { ds.ConsigneeName, ds.Consignees.ConsigneeCode , ds.Customs_ProcedureId, ds.ApplicationSettingsId }) // Potential NullReferenceException if Consignees is null
                        .FirstOrDefault();

                    if (docSetData != null && !string.IsNullOrEmpty(docSetData.ConsigneeName))
                    {
                        consigneeCode = docSetData.ConsigneeCode;
                        consigneeName = docSetData.ConsigneeName;
                        int? docSetCustomsProcId = docSetData.Customs_ProcedureId;
                        int? docSetAppId = docSetData.ApplicationSettingsId;

                        // Fetch primary address
                        var cName = consigneeName;
                        var consignee = coreCtx.Consignees
                            .FirstOrDefault(c => c.ConsigneeName == cName && c.ApplicationSettingsId == docSetAppId.Value); // Potential NullReferenceException if docSetAppId is null
                        consigneeAddress = consignee?.Address;

                        // Fallback Logic
                        if (string.IsNullOrWhiteSpace(consigneeAddress) && docSetCustomsProcId.HasValue)
                        {
                            var customsProcedureStr = coreCtx.Customs_Procedure
                                .Where(cp => cp.Customs_ProcedureId == docSetCustomsProcId.Value)
                                .Select(cp => cp.CustomsProcedure)
                                .FirstOrDefault();

                            if (!string.IsNullOrEmpty(customsProcedureStr))
                            {
                                var exportTemplate = BaseDataModel.Instance.ExportTemplates // Assuming ExportTemplates exists
                                    .FirstOrDefault(et => et.ApplicationSettingsId == docSetAppId.Value // Potential NullReferenceException if docSetAppId is null
                                                          && et.Customs_Procedure == customsProcedureStr);
                                consigneeAddress = exportTemplate?.Consignee_Address;
                            }
                        }
                    }
                }
            }
            // --- End Fetch Consignee Info ---
            return consigneeCode;
        }
    }
}