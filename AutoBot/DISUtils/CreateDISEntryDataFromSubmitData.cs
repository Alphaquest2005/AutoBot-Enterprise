using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming TODO_SubmitDiscrepanciesToCustoms, TODO_SubmitAllXMLToCustoms are here

namespace AutoBot
{
    public partial class DISUtils
    {
        private static IEnumerable<IGrouping<string, TODO_SubmitDiscrepanciesToCustoms>> CreateDISEntryDataFromSubmitData(List<TODO_SubmitAllXMLToCustoms> res)
        {
            IEnumerable<IGrouping<string, TODO_SubmitDiscrepanciesToCustoms>> lst;
            lst = res.Select(x => new TODO_SubmitDiscrepanciesToCustoms()
                {
                    CNumber = x.CNumber,
                    ApplicationSettingsId = x.ApplicationSettingsId,
                    AssessmentDate = x.AssessmentDate,
                    ASYCUDA_Id = x.ASYCUDA_Id,
                    AsycudaDocumentSetId = x.AsycudaDocumentSetId,
                    CustomsProcedure = x.CustomsProcedure,
                    DocumentType = x.DocumentType,
                    EmailId = x.EmailId,
                    ReferenceNumber = x.ReferenceNumber,
                    RegistrationDate = x.RegistrationDate,
                    Status = x.Status,
                    ToBePaid = x.ToBePaid
                })
                .GroupBy(x => x.EmailId);
            return lst;
        }
    }
}