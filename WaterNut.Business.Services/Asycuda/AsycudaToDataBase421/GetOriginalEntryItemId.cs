using System;
using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDAPrev_decl is here
using DocumentDS.Business.Entities; // Assuming DocumentDSContext, xcuda_ASYCUDA are here
using DocumentItemDS.Business.Entities; // Assuming DocumentItemDSContext, xcuda_Item are here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task<int> GetOriginalEntryItemId(ASYCUDAPrev_decl ai, string itemNumber, bool noMessages)
        {
            try
            {
                xcuda_ASYCUDA pdoc = null;
                using (var ctx = new DocumentDSContext())
                {
                    pdoc = ctx.xcuda_ASYCUDA.FirstOrDefault(
                        x =>
                            x.xcuda_Identification.xcuda_Registration.Date != null && // Potential NullReferenceExceptions
                            ((DateTime)x.xcuda_Identification.xcuda_Registration.Date).Year.ToString() == ai.Prev_decl_reg_year // Potential NullReferenceExceptions

                            && x.xcuda_Identification.xcuda_Registration.Number == ai.Prev_decl_reg_number && // Potential NullReferenceExceptions
                            x.xcuda_Identification.xcuda_Office_segment.Customs_clearance_office_code == // Potential NullReferenceExceptions
                            ai.Prev_decl_office_code);
                }

                if (pdoc == null)
                    if (noMessages)
                        return 0;
                    else
                    throw new ApplicationException(
                        $"Please Import pCNumber {ai.Prev_decl_reg_number} Year {ai.Prev_decl_reg_year} Office {ai.Prev_decl_office_code} before importing this file {a.Identification.Registration.Number}-{a.Identification.Registration.Date}"); // Assuming 'a' is accessible field, Potential NullReferenceExceptions
                using (var ctx = new DocumentItemDSContext())
                {
                    var itm = ctx.xcuda_Item.FirstOrDefault(
                        x => x.LineNumber.ToString() == ai.Prev_decl_item_number
                             && x.ASYCUDA_Id == pdoc.ASYCUDA_Id
                        //&& x.xcuda_Tarification.xcuda_HScode.Precision_4 == itemNumber // cuz of c#39457
                    );

                    if (itm != null) return itm.Item_Id;
                    return 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}