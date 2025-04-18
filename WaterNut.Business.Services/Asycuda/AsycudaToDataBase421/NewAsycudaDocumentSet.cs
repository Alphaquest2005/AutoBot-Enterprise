using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming AsycudaDocumentSet, DocumentDSContext, Document_Type, Customs_Procedure are here
using TrackableEntities; // Assuming TrackingState is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
using DBaseDataModel = WaterNut.DataSpace.DocumentDS.DataModels.BaseDataModel; // Alias for clarity

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        public static async Task<AsycudaDocumentSet> NewAsycudaDocumentSet(ASYCUDA a)
        {
            var ads = (await DBaseDataModel.Instance.SearchAsycudaDocumentSet(new List<string>() // Assuming SearchAsycudaDocumentSet exists
                {
                    $"Declarant_Reference_Number == \"{a.Declarant.Reference.Number}\"" // Potential NullReferenceException
                }).ConfigureAwait(false))
                .FirstOrDefault(); //.AsycudaDocumentSet.FirstOrDefault(d => d.Declarant_Reference_Number == a.Declarant.Reference.Number);
            if (ads == null)
            {
                ads = new AsycudaDocumentSet(true)
                {
                    TrackingState = TrackingState.Added,
                    Declarant_Reference_Number = a.Declarant.Reference.Number.Text.FirstOrDefault(), // Potential NullReferenceException
                    Currency_Code = a.Valuation.Gs_Invoice.Currency_code.Text.FirstOrDefault(), // Potential NullReferenceException
                };

                ads.Customs_Procedure = BaseDataModel.Instance.Customs_Procedures // Assuming Customs_Procedures exists
                    .FirstOrDefault(cp =>
                        cp.National_customs_procedure == a.Item.FirstOrDefault().Tarification.National_customs_procedure // Potential NullReferenceExceptions
                            .Text.FirstOrDefault()
                        && cp.Extended_customs_procedure == a.Item.FirstOrDefault().Tarification // Potential NullReferenceExceptions
                            .Extended_customs_procedure.Text.FirstOrDefault());
                if (ads.Customs_Procedure == null)
                {
                    var dt =
                        new DocumentDSContext().Document_Type
                            .FirstOrDefault(
                                d =>
                                    d.Type_of_declaration == a.Identification.Type.Type_of_declaration && // Potential NullReferenceExceptions
                                    d.Declaration_gen_procedure_code ==
                                    a.Identification.Type.Declaration_gen_procedure_code) // Potential NullReferenceExceptions
                        ?? new Document_Type(true)
                            {
                                Declaration_gen_procedure_code = a.Identification.Type.Declaration_gen_procedure_code, // Potential NullReferenceExceptions
                                Type_of_declaration = a.Identification.Type.Type_of_declaration, // Potential NullReferenceExceptions
                                TrackingState = TrackingState.Added
                            };
                    var cp = new Customs_Procedure(true)
                    {
                        Extended_customs_procedure =
                            a.Item[0].Tarification.Extended_customs_procedure.Text.FirstOrDefault(), // Potential NullReferenceExceptions
                        National_customs_procedure =
                            a.Item[0].Tarification.National_customs_procedure.Text.FirstOrDefault(), // Potential NullReferenceExceptions
                        Document_Type = dt,
                        TrackingState = TrackingState.Added
                    };
                    //await DBaseDataModel.Instance.SaveCustoms_Procedure(cp).ConfigureAwait(false); // Assuming SaveCustoms_Procedure exists
                    ads.Customs_Procedure = cp;
                }

                ads.Exchange_Rate = Convert.ToSingle(a.Valuation.Gs_Invoice.Currency_rate); // Potential NullReferenceException

                //await DBaseDataModel.Instance.SaveAsycudaDocumentSet(ads).ConfigureAwait(false); // Assuming SaveAsycudaDocumentSet exists

                return ads;
            }
            else
            {
                return ads;
            }
        }
    }
}