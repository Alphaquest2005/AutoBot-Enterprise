using System.Collections.Generic;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using Omu.ValueInjecter;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    public async Task SaveAsycudaDocument(AsycudaDocument asycudaDocument)
    {
        asycudaDocument.ModifiedProperties = null;
        if (asycudaDocument == null) return;
        //get the original item
        var i = await GetDocument(asycudaDocument.ASYCUDA_Id, new List<string>
        {
            "xcuda_ASYCUDA_ExtendedProperties"
        }).ConfigureAwait(false);
        i.StartTracking();
        //null for now cuz there are no navigation properties involved.
        i.InjectFrom(asycudaDocument);

        //var i = new xcuda_ASYCUDA()
        //{
        //    ASYCUDA_Id = asycudaDocument.ASYCUDA_Id,
        //    xcuda_ASYCUDA_ExtendedProperties = new xcuda_ASYCUDA_ExtendedProperties()
        //    {
        //        ASYCUDA_Id = asycudaDocument.ASYCUDA_Id,
        //        TrackingState = TrackingState.Unchanged
        //    },
        //    TrackingState = TrackingState.Unchanged
        //};

        i.xcuda_ASYCUDA_ExtendedProperties.StartTracking();
        if (i.xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate !=
            asycudaDocument.EffectiveRegistrationDate)
            i.xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate =
                asycudaDocument.EffectiveRegistrationDate;
        if (i.xcuda_ASYCUDA_ExtendedProperties.DoNotAllocate != asycudaDocument.DoNotAllocate)
            i.xcuda_ASYCUDA_ExtendedProperties.DoNotAllocate = asycudaDocument.DoNotAllocate ?? false;
        if (i.xcuda_ASYCUDA_ExtendedProperties.ModifiedProperties != null)
        {
            await Save_xcuda_ASYCUDA(i).ConfigureAwait(false);
        }
    }
}