using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using TrackableEntities;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    private void AttachContainer(int asycudaDocumentSetId)
    {
        List<xcuda_ASYCUDA> docs;
        AsycudaDocumentSet container;
        using (var ctx = new DocumentDSContext { StartTracking = true })
        {
            container = ctx.AsycudaDocumentSets.Include(x => x.Container)
                .FirstOrDefault(x => x.AsycudaDocumentSetId == asycudaDocumentSetId);
            if (container?.Container == null) return;


            docs = ctx.xcuda_ASYCUDA
                .Include(x => x.xcuda_Container)
                .Where(x => x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId == asycudaDocumentSetId)
                .ToList();
        }

        var res = new List<xcuda_Container>();
        foreach (var xCon in docs)
        {
            var firstItem =
                new CoreEntitiesContext().AsycudaDocumentItems
                    .First(x => x.AsycudaDocumentId == xCon.ASYCUDA_Id && x.LineNumber == "1");

            var c = xCon.xcuda_Container.FirstOrDefault();
            if (c == null)
                c = new xcuda_Container(true)
                {
                    ASYCUDA_Id = xCon.ASYCUDA_Id,
                    TrackingState = TrackingState.Added
                };

            c.Container_identity = container.Container.Container_identity;
            c.Container_type = container.Container.Container_type;
            c.Empty_full_indicator = container.Container.Empty_full_indicator;
            c.Goods_description = firstItem.Commercial_Description;
            c.Packages_number = firstItem.Number_of_packages?.ToString(CultureInfo.InvariantCulture);
            c.Packages_type = "PK";
            c.Item_Number = firstItem.LineNumber;
            c.Packages_weight = (double)firstItem.Gross_weight_itm;
            res.Add(c);
        }

        using (var ctx = new DocumentDSContext())
        {
            foreach (var itm in res)
            {
                ctx.xcuda_Container.Add(itm);
                ctx.SaveChanges();
            }
        }
    }
}