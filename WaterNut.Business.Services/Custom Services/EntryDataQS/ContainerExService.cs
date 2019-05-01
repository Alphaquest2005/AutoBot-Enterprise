

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using EntryDataQS.Business.Entities;
using Omu.ValueInjecter;
using TrackableEntities;

namespace EntryDataQS.Business.Services
{
  public partial class ContainerExService 
    {
      public async Task SaveContainer(ContainerEx container)
      {
          container.ModifiedProperties = null;
          using (var ctx = new EntryDataDS.Business.Services.ContainerService(){StartTracking = true})
          {
              var cnt = await ctx.GetContainerByKey(container.Container_Id.ToString(), new List<string>()
              {
                  "ContainerAsycudaDocumentSets"
              }).ConfigureAwait(false) ?? new Container(true){TrackingState = TrackingState.Added};

              cnt.InjectFrom(container);

              if (!cnt.ContainerAsycudaDocumentSets.Any(x => x.AsycudaDocumentSetId == container.AsycudaDocumentSetId))
              {
                  cnt.ContainerAsycudaDocumentSets.Add(new ContainerAsycudaDocumentSet(true){Container = cnt,AsycudaDocumentSetId = container.AsycudaDocumentSetId.GetValueOrDefault(),TrackingState = TrackingState.Added});
              }
              cnt.ModifiedProperties = null;
              await ctx.UpdateContainer(cnt).ConfigureAwait(false);
          }
      }
    }
}



