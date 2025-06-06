using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDA is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here
// Removed alias for DBBaseDataModel

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task<bool> DeleteExistingDocument()
        {
            if (string.IsNullOrEmpty(a.Identification.Registration.Number)) // Potential NullReferenceException
            {
                if (ImportOnlyRegisteredDocuments) return true;
                a.Identification.Registration.Number = "0"; // Potential NullReferenceException
            }

            if (a.Identification.Registration.Date == "") // Potential NullReferenceException
                a.Identification.Registration.Date = DateTime.MinValue.ToShortDateString(); // Potential NullReferenceException

            // docs =   db.xcuda_ASYCUDA.Where(x => x.id == a.id).ToList();
           var   docs = (await BaseDataModel.Instance.Searchxcuda_ASYCUDA(new List<string>() // Using unqualified BaseDataModel
            {
                // $"id == \"{a.id}\""
                 $"id == \"{a.id}\" || (xcuda_Declarant.Number == \"{a.Declarant.Reference.Number.Text.FirstOrDefault()}\" && xcuda_Identification.xcuda_Registration.Number == null)" // Potential NullReferenceExceptions
            }, new List<string>()
            {
                "xcuda_Identification ",
                "xcuda_ASYCUDA_ExtendedProperties",
                "xcuda_Identification.xcuda_Registration",
                "xcuda_Identification.xcuda_Office_segment",
                "xcuda_Declarant"
            }).ConfigureAwait(false)).ToList();

            //// if (doc == null)
            //// {
            ////         using (var ctx = new xcuda_ASYCUDAService())
            ////         {
            ////                 doc = (await ctx.Getxcuda_ASYCUDAByExpressionLst(new List<string>()
            ////                 {
            ////                         string.Format("xcuda_Identification.xcuda_Registration.Number == \"{0}\"",
            ////                                 a.Identification.Registration.Number),
            ////                         string.Format("xcuda_Identification.xcuda_Registration.Date != null && Convert.ToDateTime(xcuda_Identification.xcuda_Registration.Date).Year == {0}",
            ////                                 Convert.ToDateTime(a.Identification.Registration.Date).Year),
            ////                         string.Format(
            ////                                 "xcuda_Identification.xcuda_Office_segment.Customs_clearance_office_code == \"{0}\"",
            ////                                 a.Identification.Office_segment.Customs_clearance_office_code.Text)
            ////                 }, new List<string>()
            ////                 {
            ////                         "xcuda_Identification ",
            ////                         "xcuda_ASYCUDA_ExtendedProperties",
            ////                         "xcuda_Identification.xcuda_Registration",
            ////                         "xcuda_Identification.xcuda_Office_segment",
            ////                         "xcuda_Declarant"
            ////                 }).ConfigureAwait(false)).FirstOrDefault();
            ////         }
            //// }
            //// check the declarant reference number
            // if (doc == null)
            // {
            //         doc =
            //                 db.xcuda_ASYCUDA.Where(x => x.xcuda_Identification.xcuda_Registration.Number == null
            //                                                                         && x.xcuda_Declarant != null
            //                                                                         && x.xcuda_Declarant.Number != null
            //                                                                         && x.xcuda_Declarant.Number.Replace(" ", "")
            //                                                                         == a.Declarant.Reference.Number.Replace(" ", ""))
            //                         .AsEnumerable()
            //                         .Where(c => c.RegistrationDate == DateTime.MinValue
            //                                                 || (c.RegistrationDate != DateTime.MinValue
            //                                                         &&
            //                                                         c.RegistrationDate.Year ==
            //                                                         Convert.ToDateTime(a.Identification.Registration.Date).Year)
            //                                                 &&
            //                                                 c.xcuda_Identification.xcuda_Office_segment.Customs_clearance_office_code ==
            //                                                 a.Identification.Office_segment.Customs_clearance_office_code.Text
            //                                                         .FirstOrDefault()
            //                         ).Distinct().ToList();
            // }

            foreach (var doc in docs)
            {
                if (!OverwriteExisting && doc.xcuda_ASYCUDA_ExtendedProperties.ImportComplete) return true; // Potential NullReferenceException
                await BaseDataModel.Instance.DeleteAsycudaDocument(doc.ASYCUDA_Id).ConfigureAwait(false); // Using unqualified BaseDataModel
            }
            return false;
        }
    }
}