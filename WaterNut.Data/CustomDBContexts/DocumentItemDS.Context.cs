
using System.Data.Entity;
using CoreEntities.Business.Entities;
using DocumentItemDS.Business.Entities.Mapping;


namespace DocumentItemDS.Business.Entities
{
    public partial class DocumentItemDSContext
    {
        partial void OnModelCreatingExtentsion(DbModelBuilder modelBuilder)
        {
            //var s = AsycudaDocumentSetExs.SqlQuery();
            Database.CommandTimeout = 0;
        }
    }
}

 	
