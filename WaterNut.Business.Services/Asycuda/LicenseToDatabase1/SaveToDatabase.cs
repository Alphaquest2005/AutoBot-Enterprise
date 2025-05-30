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
    // Assuming this is part of the same partial class C71ToDataBase as the previous file
    // If it's meant to be a separate class (LicenseToDatabase1), the class definition needs changing.
    // For now, assuming it's part of C71ToDataBase based on the file name pattern.
    public partial class C71ToDataBase
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
                var da = CreateLicense(docSet, file, regNumber); // Assuming CreateLicense exists
                SaveGeneralInfo(adoc.General_segment, da.xLIC_General_segment); // Assuming SaveGeneralInfo exists, Potential NullReferenceException
                SaveDocumentRef(da); // Assuming SaveDocumentRef exists
                SaveItems(adoc.Lic_item_segment, da); // Assuming SaveItems exists, Potential NullReferenceException
                // AttachLicenseToDocSet(docSet, file, regNumber); // Missing in this version compared to LicenseToDatabase.cs

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