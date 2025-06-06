using System;
using System.Linq;
using System.Threading.Tasks;
using Asycuda421; // Assuming ASYCUDA is here
using DocumentDS.Business.Entities; // Assuming xcuda_Container is here
using TrackableEntities; // Assuming TrackingState is here
// Need using for DBaseDataModel if Savexcuda_Container is uncommented

namespace WaterNut.DataSpace.Asycuda
{
    public partial class AsycudaToDataBase421
    {
        private async Task SaveContainer()
        {
            try
            {
                foreach (var ac in a.Container) // Assuming 'a' is accessible field
                {
                    var c = da.Document.xcuda_Container.FirstOrDefault(x => x.Container_identity == ac.Container_identity); // Assuming 'da' is accessible field, Potential NullReferenceException
                    if (c == null)
                    {
                        c = new xcuda_Container(true) { ASYCUDA_Id = da.Document.ASYCUDA_Id, TrackingState = TrackingState.Added }; // Potential NullReferenceException
                        da.Document.xcuda_Container.Add(c); // Potential NullReferenceException
                    }

                    c.Container_identity = ac.Container_identity;
                    c.Container_type = ac.Container_type;
                    c.Empty_full_indicator = ac.Empty_full_indicator;
                    c.Goods_description = ac.Goods_description.Text.FirstOrDefault(); // Potential NullReferenceException
                    c.Gross_weight = Convert.ToSingle(ac.Gross_weight.Text.FirstOrDefault()); // Potential NullReferenceException, FormatException
                    c.Item_Number = ac.Item_Number;
                    c.Packages_number = ac.Packages_number;
                    c.Packages_type = ac.Packages_type;
                    c.Packages_weight = Convert.ToSingle(ac.Packages_weight); // FormatException
                }

                //await DBaseDataModel.Instance.Savexcuda_Container(c).ConfigureAwait(false); // Assuming Savexcuda_Container exists
            }
            catch (Exception Ex)
            {
                throw;
            }
            // Added await Task.CompletedTask to make the method async as declared
            await Task.CompletedTask;
        }
    }
}