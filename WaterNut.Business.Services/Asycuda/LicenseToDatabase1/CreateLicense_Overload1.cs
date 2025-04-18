using System.Data.Entity; // For Include
using System.IO;
using System.Linq;
using DocumentDS.Business.Entities; // Assuming AsycudaDocumentSet is here
using LicenseDS.Business.Entities; // Assuming Registered, LicenseDSContext, xLIC_License, xLIC_General_segment, xLIC_Lic_item_segment are here

namespace WaterNut.DataSpace.Asycuda
{
    // Assuming this is part of the same partial class C71ToDataBase as the previous file
    public partial class C71ToDataBase
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
                    // These call methods which need to be in their own partial classes
                    ndoc = CreateRegisteredLicense(regNumber,file.FullName);
                    AttachLicenseToDocSet(docSet, file); // Note: This version calls AttachLicenseToDocSet without regNumber
                }
                // else block removed compared to LicenseToDatabase.cs
            }
            //ndoc.SetupProperties(); // Original code was commented out
            return ndoc;
        }
    }
}