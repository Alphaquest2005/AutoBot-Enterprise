using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext, TODO_SubmitDiscrepanciesToCustoms, AsycudaDocument_Attachments, Attachments are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class DISUtils
    {
        // Assuming _databaseCommandTimeout is defined elsewhere or needs moving
        // private static readonly int _databaseCommandTimeout = 30;

        private static List<string> AttachDiscrepancyPdfs(List<TODO_SubmitDiscrepanciesToCustoms> emailIds)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = _databaseCommandTimeout;
                var pdfs = new List<string>();
                foreach (var itm in emailIds)
                {
                    var res = ctx.AsycudaDocument_Attachments.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id)
                        .Select(x => x.Attachments.FilePath).ToArray();
                    if (!res.Any())
                    {
                        BaseDataModel.LinkPDFs(new List<int>() { itm.ASYCUDA_Id }); // Assuming LinkPDFs exists
                        res = ctx.AsycudaDocument_Attachments.Where(x => x.AsycudaDocumentId == itm.ASYCUDA_Id)
                            .Select(x => x.Attachments.FilePath).ToArray();
                    }

                    pdfs.AddRange(res);
                }

                return pdfs;
            }
        }
    }
}