using System;
using System.Data.Entity; // For Include
using System.IO;
using System.Linq;
using LicenseDS.Business.Entities; // Assuming LicenseDSContext, Registered, xLIC_License, xLIC_General_segment are here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class LicenseToDataBase
    {
        public static bool GetLicenceRegNumber(FileInfo file, out string regNumber)
        {
            using (var ctx = new LicenseDSContext())
            {
                var lic = ctx.xLIC_License.OfType<Registered>()
                    .Include(x => x.xLIC_General_segment)
                    .FirstOrDefault(x => x.SourceFile == file.FullName);
                if (lic != null)
                {
                    regNumber = DateTime.Parse(lic.xLIC_General_segment.Application_date).Year.ToString() + " " + // Potential NullReferenceException, FormatException
                       lic.RegistrationNumber;
                    return true;
                }
                else
                {
                    regNumber = null;
                    return false;
                }
            }
        }
    }
}