using System.Linq;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Identification is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void SaveManifestReferenceNumber(xcuda_Identification di)
        {
            //xcuda_Manifest_reference_number r = di.xcuda_Manifest_reference_number;//.FirstOrDefault(); // Original code was commented out
            //if (r == null)
            //{
            //    r = new xcuda_Manifest_reference_number();
            //    di.xcuda_Manifest_reference_number = r;
            //    //di.xcuda_Manifest_reference_number.Add(r);
            //}
            if (a.Identification.Manifest_reference_number.Text.Count != 0) // Assuming 'a' is accessible field, Potential NullReferenceException
                di.Manifest_reference_number = a.Identification.Manifest_reference_number.Text[0]; // Potential NullReferenceException
        }
    }
}