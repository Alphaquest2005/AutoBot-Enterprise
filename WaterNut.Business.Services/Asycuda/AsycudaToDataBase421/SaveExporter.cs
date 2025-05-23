using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Traders, xcuda_Exporter are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private void SaveExporter(xcuda_Traders t)
        {
            var e = t.xcuda_Exporter; // Potential NullReferenceException
            if (e == null)
            {
                e = new xcuda_Exporter(true) { Traders_Id = t.Traders_Id, TrackingState = TrackingState.Added };
                t.xcuda_Exporter = e; // Potential NullReferenceException
            }

            if (a.Traders.Exporter.Exporter_name.Text.Count != 0) // Assuming 'a' is accessible field, Potential NullReferenceException
            {
                e.Exporter_name = a.Traders.Exporter.Exporter_name.Text[0]; // Potential NullReferenceException
            }

            if (a.Traders.Exporter.Exporter_code.Text.Count != 0) // Assuming 'a' is accessible field, Potential NullReferenceException
            {
                e.Exporter_code = a.Traders.Exporter.Exporter_code.Text[0]; // Potential NullReferenceException
            }
        }
    }
}