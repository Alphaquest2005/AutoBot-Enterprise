
using TrackableEntities.Common;
using Core.Common.Client.Services;
using Core.Common.Client.Repositories;
using EntryDataQS.Client.Services;
using EntryDataQS.Client.Entities;
using EntryDataQS.Client.DTO;
using Core.Common.Business.Services;
using System.Diagnostics;
using TrackableEntities.Client;


using System.Threading.Tasks;
using System.Linq;
using Core.Common;
using System.ComponentModel;
using System.Collections.Generic;
using System;
using System.ServiceModel;

using ContainerEx = EntryDataQS.Client.Entities.ContainerEx;

namespace EntryDataQS.Client.Repositories 
{
    public partial class ContainerExRepository 
    {
        public async Task SaveContainer(ContainerEx container)
        {
            using (var ctx = new ContainerExClient())
            {
                await ctx.SaveContainer(container.DTO).ConfigureAwait(false);
            }
        }
      
    }
}

