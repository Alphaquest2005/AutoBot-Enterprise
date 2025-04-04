using System.Linq;
using System.Threading.Tasks;
using DocumentDS.Business.Entities; // Assuming Customs_Procedure is here
using DocumentDS.Business.Services; // Assuming Customs_ProcedureService is here
using DocumentItemDS.Business.Entities; // Assuming xcuda_Tarification is here
using TrackableEntities; // Assuming TrackingState is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using DBaseDataModel = WaterNut.DataSpace.DocumentDS.DataModels.BaseDataModel; // Alias for clarity

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task<Customs_Procedure> SaveCustomsProcedure(xcuda_Tarification t)
        {
            var cp = BaseDataModel.Instance.Customs_ProcedureCache.Data.FirstOrDefault(x => x.Extended_customs_procedure == t.Extended_customs_procedure // Assuming Customs_ProcedureCache exists
                                                            && x.National_customs_procedure == t.National_customs_procedure);
            //    (await DBaseDataModel.Instance.SearchCustoms_Procedure(new List<string>()
            //{
            //    string.Format("Extended_customs_procedure == \"{0}\"", t.Extended_customs_procedure),
            //    string.Format("National_customs_procedure == \"{0}\"", t.National_customs_procedure)
            //}).ConfigureAwait(false)).FirstOrDefault();
            if (cp == null)
            {
                var scp = BaseDataModel.Instance.Customs_ProcedureCache.Data.FirstOrDefault(x => x.Extended_customs_procedure == t.Extended_customs_procedure // Assuming Customs_ProcedureCache exists
                                                           && x.National_customs_procedure == t.National_customs_procedure);
                // if scp is null return
                if (scp == null) return null;

                cp = new Customs_Procedure(true)
                {
                    Extended_customs_procedure = t.Extended_customs_procedure,
                    National_customs_procedure = t.National_customs_procedure,
                    BondTypeId = BaseDataModel.Instance.CurrentApplicationSettings.BondTypeId, // Assuming BondTypeId exists
                    CustomsOperationId = scp.CustomsOperationId,
                    TrackingState = TrackingState.Added
                };

                using (var ctx = new Customs_ProcedureService()) // Assuming Customs_ProcedureService exists
                {
                    cp = await ctx.UpdateCustoms_Procedure(cp).ConfigureAwait(false); // Assuming UpdateCustoms_Procedure exists
                }

                    BaseDataModel.Instance.Customs_ProcedureCache.AddItem(cp); // Assuming AddItem exists
            }
            da.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId = cp.Customs_ProcedureId; // Assuming 'da' is accessible field, Potential NullReferenceException
            da.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure = cp; // Assuming 'da' is accessible field, Potential NullReferenceException
            return cp;
        }
    }
}