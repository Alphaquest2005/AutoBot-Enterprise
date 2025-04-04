using System;
using System.Linq;
using Asycuda421; // Assuming ASYCUDAItem is here
using DocumentItemDS.Business.Entities; // Assuming xcuda_Taxation, xcuda_Taxation_line are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void Save_Taxation_line(xcuda_Taxation t, ASYCUDAItem ai)
        {
            for (var i = 0; i < ai.Taxation.Taxation_line.Count; i++) // Potential NullReferenceException
            {
                var au = ai.Taxation.Taxation_line.ElementAt(i); // Potential NullReferenceException

                if (au.Duty_tax_code.Text.Count == 0) break; // Potential NullReferenceException

                var tl = t.xcuda_Taxation_line.ElementAtOrDefault(i); // Potential NullReferenceException
                if (tl == null)
                {
                    tl = new xcuda_Taxation_line(true) { TrackingState = TrackingState.Added };
                    t.xcuda_Taxation_line.Add(tl); // Potential NullReferenceException
                }

                tl.Duty_tax_amount = Convert.ToDouble(au.Duty_tax_amount); // Potential NullReferenceException
                tl.Duty_tax_Base = au.Duty_tax_Base; // Potential NullReferenceException
                tl.Duty_tax_code = au.Duty_tax_code.Text[0]; // Potential NullReferenceException

                if (au.Duty_tax_MP.Text.Count > 0) // Potential NullReferenceException
                    tl.Duty_tax_MP = au.Duty_tax_MP.Text[0]; // Potential NullReferenceException

                tl.Duty_tax_rate = Convert.ToDouble(au.Duty_tax_rate); // Potential NullReferenceException
            }
        }
    }
}