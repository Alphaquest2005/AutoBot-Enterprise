using System;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Property, xcuda_Nbers are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void SaveNbers(xcuda_Property p)
        {
            var n = p.xcuda_Nbers;//.FirstOrDefault(); // Potential NullReferenceException
            if (n == null)
            {
                n = new xcuda_Nbers(true) { ASYCUDA_Id = p.ASYCUDA_Id, TrackingState = TrackingState.Added };
                p.xcuda_Nbers = n; // Potential NullReferenceException
                //  p.xcuda_Nbers.Add(n);
            }
            n.Number_of_loading_lists = a.Property.Nbers.Number_of_loading_lists; // Assuming 'a' is accessible field, Potential NullReferenceException
            n.Total_number_of_packages = Convert.ToSingle(string.IsNullOrEmpty(a.Property.Nbers.Total_number_of_packages) ? "0" : a.Property.Nbers.Total_number_of_packages); // Assuming 'a' is accessible field, Potential NullReferenceException, FormatException
            n.Total_number_of_items = a.Property.Nbers.Total_number_of_items; // Assuming 'a' is accessible field, Potential NullReferenceException
        }
    }
}