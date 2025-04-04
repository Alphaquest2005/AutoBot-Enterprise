using Asycuda421; // Assuming Value_declaration_form is here
using DocumentDS.Business.Entities; // Assuming AsycudaDocumentSet is here
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TrackableEntities.EF6; // Assuming ApplyChanges is here
using ValuationDS.Business.Entities; // Assuming ValuationDSContext, Registered, xC71_Value_declaration_form are here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class C71ToDataBase
    {
        // Assuming 'da' is a field accessible across partial classes
        // private ValuationDS.Business.Entities.Registered da;

        public void SaveToDatabase(Value_declaration_form adoc, AsycudaDocumentSet docSet, FileInfo file)
        {
            try
            {
                var mat = Regex.Match(file.FullName,
                    @"[A-Z\\ -.]*(?<RegNumber>[0-9]+)-C71.*.xml",
                    RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (!mat.Success) return;
                // Ensure Group 1 exists before accessing Captures
                if (mat.Groups.Count <= 1 || mat.Groups[1].Captures.Count == 0) return;
                var regNumber = mat.Groups[1].Captures[0].Value;

                // These call methods which need to be in their own partial classes
                da = CreateNewC71(docSet, file, regNumber, adoc.id); // Assuming 'da' is accessible field
                SaveValuationForm(adoc, da);
                SaveIdentification(adoc.Identification_segment, da.xC71_Identification_segment); // Potential NullReferenceException
                SaveDocumentRef(da);
                SaveItems(adoc.Item, da); // Potential NullReferenceException

                if (da.xC71_Identification_segment.xC71_Seller_segment.Address.Contains( // Potential NullReferenceException
                    docSet.Declarant_Reference_Number))
                {
                    // This calls AttachC71ToDocset, which needs to be in its own partial class
                    AttachC71ToDocset(docSet, file, da);
                    using (var ctx = new ValuationDSContext())
                    {
                        ctx.ApplyChanges(da); // Assuming ApplyChanges exists
                        ctx.SaveChanges();

                        var existingC71 = ctx.xC71_Value_declaration_form
                            .Where(x =>
                                x.xC71_Identification_segment.xC71_Seller_segment.Address.Contains( // Potential NullReferenceException
                                    docSet.Declarant_Reference_Number)
                                && x.Value_declaration_form_Id != da.Value_declaration_form_Id) // Potential NullReferenceException
                            .ToList();
                        ctx.xC71_Value_declaration_form.RemoveRange(existingC71);
                        ctx.SaveChanges();
                    }
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
        }
    }
}