
using TrackableEntities.Common;
using Core.Common.Client.Services;
using Core.Common.Client.Repositories;
using CoreEntities.Client.Services;
using CoreEntities.Client.Entities;
using CoreEntities.Client.DTO;
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

using AsycudaDocumentItem = CoreEntities.Client.Entities.AsycudaDocumentItem;

namespace CoreEntities.Client.Repositories 
{
   
    public partial class AsycudaDocumentItemRepository
    {
        public async Task RemoveSelectedItems(List<AsycudaDocumentItem> lst)
        {
            using (var ctx = new AsycudaDocumentItemClient())
            {
                await ctx.RemoveSelectedItems(lst.Select(x => x.Item_Id).ToList()).ConfigureAwait(false);
            }
        }

        public async Task SaveAsycudaDocumentItem(AsycudaDocumentItem asycudaDocumentItem)
        {
            using (var ctx = new AsycudaDocumentItemClient())
            {
                await ctx.SaveAsycudaDocumentItem(asycudaDocumentItem.DTO).ConfigureAwait(false);
            }
        }


      
    }
}

