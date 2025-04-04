using System.Data.Entity; // For Include
using System.IO;
using System.Linq;
using DocumentDS.Business.Entities; // Assuming AsycudaDocumentSet is here
using LicenseDS.Business.Entities; // Assuming Registered, LicenseDSContext, xLIC_License, xLIC_General_segment, xLIC_Lic_item_segment are here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class LicenseToDataBase
    {
        private LicenseDS.Business.Entities.Registered CreateLicense(AsycudaDocumentSet docSet, FileInfo file, string regNumber)
        {
            LicenseDS.Business.Entities.Registered ndoc;
            using (var ctx = new LicenseDSContext())
            {
                ndoc = ctx.xLIC_License.OfType<LicenseDS.Business.Entities.Registered>()
                    .Include(x => x.xLIC_General_segment)
                    .Include(x => x.xLIC_Lic_item_segment)
                    .FirstOrDefault(x => x.RegistrationNumber == regNumber);
                if (ndoc == null)
                {
                    // This calls CreateRegisteredLicense, which needs to be in its own partial class
                    ndoc = CreateRegisteredLicense(regNumber,file.FullName);
                }
                else
                {
                    // to fix wrong imported files or changed file paths
                    ndoc.SourceFile = file.FullName;
                    ctx.SaveChanges();
                }
            }
            //ndoc.SetupProperties(); // Original code was commented out
            return ndoc;
        }
    }
}