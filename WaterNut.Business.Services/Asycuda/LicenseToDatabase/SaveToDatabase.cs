using Asycuda421; // Assuming Licence is here
using DocumentDS.Business.Entities; // Assuming AsycudaDocumentSet is here
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrackableEntities.EF6; // Assuming ApplyChanges is here
using ValuationDS.Business.Entities; // Assuming Registered is here
using LicenseDS.Business.Entities; // Assuming LicenseDSContext is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class LicenseToDataBase
    {
        // Assuming 'da' is a field accessible across partial classes
        // private LicenseDS.Business.Entities.Registered da;

        public async Task SaveToDatabase(Licence adoc, AsycudaDocumentSet docSet, FileInfo file)
        {
            try
            {
                var mat = Regex.Match(file.FullName,
                    @"[A-Z\\ -.]*(?<RegNumber>[0-9]+)-LIC.*.xml",
                    RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (!mat.Success) return;
                // Ensure Group 1 exists before accessing Captures
                if (mat.Groups.Count <= 1 || mat.Groups[1].Captures.Count == 0) return;
                var regNumber = mat.Groups[1].Captures[0].Value;

                // These call methods which need to be in their own partial classes
                da = CreateLicense(docSet, file, regNumber); // Assuming 'da' is accessible field
                SaveGeneralInfo(adoc.General_segment, da.xLIC_General_segment); // Potential NullReferenceException
                SaveDocumentRef(da);
                SaveItems(adoc.Lic_item_segment, da); // Potential NullReferenceException
                AttachLicenseToDocSet(docSet, file, regNumber);

                using (var ctx = new LicenseDSContext())
                {
                    ctx.ApplyChanges(da); // Assuming ApplyChanges exists
                    ctx.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Final cleanup if needed
            }
            // Added await Task.CompletedTask to make the method async as declared
            await Task.CompletedTask;
        }
    }
}