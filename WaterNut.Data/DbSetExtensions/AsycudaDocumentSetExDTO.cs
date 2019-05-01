using System.Data.Entity;
using System.Data.Entity.Core.Common.EntitySql;
using System.Linq.Expressions;
using Core.Common.Data;
using CoreEntities.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterNut.Data;

namespace WaterNut.Data.DbSetExtensions
{
    public static class AsycudaDocumentSetExDTOExtentions
    {
        //public static IQueryable<AsycudaDocumentDTO> IAsycudaDocuments(this AsycudaDocumentSetExDTO entity, DbContext ctx = null)
        //{
            
        //    if (ctx == null) ctx = new CoreEntitiesContext();
        //    var res = ctx.CreateNavigationQuery(entity, p => p.AsycudaDocuments);
        //    return res;
        //}
    }
}
