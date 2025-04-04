using System;
using System.Linq;
using Asycuda421; // Assuming ASYCUDAItem is here
using DocumentItemDS.Business.Entities; // Assuming xcuda_Tarification, xcuda_Supplementary_unit are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void Save_Supplementary_unit(xcuda_Tarification t, ASYCUDAItem ai)
        {
            for (var i = 0; i < ai.Tarification.Supplementary_unit.Count; i++) // Potential NullReferenceException
            {
                var au = ai.Tarification.Supplementary_unit.ElementAt(i); // Potential NullReferenceException

                if (au.Suppplementary_unit_code.Text.Count == 0) continue; // Potential NullReferenceException

                var su = t.xcuda_Supplementary_unit.ElementAtOrDefault(i); // Potential NullReferenceException
                if (su == null)
                {
                    su = new xcuda_Supplementary_unit(true) { Tarification_Id = t.Item_Id, TrackingState = TrackingState.Added };
                    t.Unordered_xcuda_Supplementary_unit.Add(su); // Potential NullReferenceException
                }

                su.Suppplementary_unit_quantity = Convert.ToDouble(string.IsNullOrEmpty(au.Suppplementary_unit_quantity) ? "0" : au.Suppplementary_unit_quantity); // Potential NullReferenceException

                if (au.Suppplementary_unit_code.Text.Count > 0) // Potential NullReferenceException
                    su.Suppplementary_unit_code = au.Suppplementary_unit_code.Text[0]; // Potential NullReferenceException

                if (au.Suppplementary_unit_name.Text.Count > 0) // Potential NullReferenceException
                    su.Suppplementary_unit_name = au.Suppplementary_unit_name.Text[0]; // Potential NullReferenceException

                if (i == 0) su.IsFirstRow = true;
            }
        }
    }
}