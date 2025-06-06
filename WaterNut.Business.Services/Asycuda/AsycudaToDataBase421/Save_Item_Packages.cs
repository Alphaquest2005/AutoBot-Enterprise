using System;
using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDAItem is here
using Core.Common.Utils; // Assuming StringExtensions.Truncate is here
using DocumentItemDS.Business.Entities; // Assuming xcuda_Item, xcuda_Packages are here
using TrackableEntities; // Assuming TrackingState is here
// Need using for DIBaseDataModel if Savexcuda_Packages is uncommented

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task Save_Item_Packages(xcuda_Item di, ASYCUDAItem ai)
        {
            var p = di.xcuda_Packages.FirstOrDefault(); // Potential NullReferenceException
            if (p == null)
            {
                p = new xcuda_Packages(true) { Item_Id = di.Item_Id, TrackingState = TrackingState.Added };
                di.xcuda_Packages.Add(p); // Potential NullReferenceException
            }
            p.Kind_of_packages_code = ai.Packages.Kind_of_packages_code.Text.FirstOrDefault(); // Potential NullReferenceException
            p.Kind_of_packages_name = ai.Packages.Kind_of_packages_name.Text.FirstOrDefault(); // Potential NullReferenceException
            p.Number_of_packages = string.IsNullOrEmpty(ai.Packages.Number_of_packages)? 0 : Convert.ToSingle(ai.Packages.Number_of_packages); // Potential NullReferenceException

            if (ai.Packages.Marks1_of_packages.Text.Count > 0) // Potential NullReferenceException
                p.Marks1_of_packages = ai.Packages.Marks1_of_packages.Text[0].Truncate(40); // Potential NullReferenceException

            if (ai.Packages.Marks2_of_packages.Text.Count > 0) // Potential NullReferenceException
                p.Marks2_of_packages = ai.Packages.Marks2_of_packages.Text[0].Truncate(40); // Potential NullReferenceException

            //await DIBaseDataModel.Instance.Savexcuda_Packages(p).ConfigureAwait(false); // Assuming Savexcuda_Packages exists
            // Added await Task.CompletedTask to make the method async as declared
            await Task.CompletedTask;
        }
    }
}