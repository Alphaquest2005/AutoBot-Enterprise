using System.Data.Entity; // For Include
using System.IO;
using System.Linq;
using DocumentDS.Business.Entities; // Assuming AsycudaDocumentSet is here
using ValuationDS.Business.Entities; // Assuming Registered, ValuationDSContext, xC71_Value_declaration_form, xC71_Identification_segment, xC71_Buyer_segment, xC71_Declarant_segment, xC71_Seller_segment, xC71_Item are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class C71ToDataBase
    {
        private ValuationDS.Business.Entities.Registered CreateNewC71(AsycudaDocumentSet docSet, FileInfo file,string regNumber ,string id)
        {
            ValuationDS.Business.Entities.Registered  ndoc;//
            using (var ctx = new ValuationDSContext())
            {
                ndoc = ctx.xC71_Value_declaration_form.OfType<ValuationDS.Business.Entities.Registered>()
                    .Include(x => x.xC71_Identification_segment)
                    .Include(x => x.xC71_Identification_segment.xC71_Buyer_segment)
                    .Include(x => x.xC71_Identification_segment.xC71_Declarant_segment)
                    .Include(x => x.xC71_Identification_segment.xC71_Seller_segment)
                    .Include(x => x.xC71_Item)
                    .FirstOrDefault(x => x.id == id);
                if (ndoc == null)
                {
                    // This calls CreateNewRegisteredC71, which needs to be in its own partial class
                    ndoc = (ValuationDS.Business.Entities.Registered) CreateNewRegisteredC71();
                    ndoc.RegNumber = regNumber;
                    ndoc.SourceFile = file.FullName;
                    ndoc.ApplicationSettingsId =
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId; // Assuming CurrentApplicationSettings exists
                }
            }
            //ndoc.SetupProperties(); // Original code was commented out
            return ndoc;
        }
    }
}