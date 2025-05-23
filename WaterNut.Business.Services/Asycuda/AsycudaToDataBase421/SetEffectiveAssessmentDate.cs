using System;
using System.Linq;
using WaterNut.Business.Entities; // Assuming DocumentCT is here
using WaterNut.Business.Services.Utils; // Assuming FileTypeManager is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void SetEffectiveAssessmentDate(DocumentCT documentCt, string commentsFreeText)
        {
            if (string.IsNullOrEmpty(commentsFreeText) || !commentsFreeText.Contains("EffectiveAssessmentDate:")) return;
            var strlst = commentsFreeText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var res = strlst.First(x => x.Contains("EffectiveAssessmentDate:")); // InvalidOperationException if no match
            documentCt.Document.xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate = FileTypeManager.ImportAnyDate(res.Replace("EffectiveAssessmentDate:","")); // Assuming ImportAnyDate exists, Potential NullReferenceException
        }
    }
}