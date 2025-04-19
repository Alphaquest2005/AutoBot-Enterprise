using System.Data.Entity;
using CoreEntities.Business.Entities;
using AllocationQS.Business.Entities.Mapping;


namespace AllocationDS.Business.Entities
{
    public partial class AllocationDSContext 
    {
        partial void OnModelCreatingExtentsion(DbModelBuilder modelBuilder)
        {
            //var s = AsycudaDocumentSetExs.SqlQuery();
            Database.CommandTimeout = 0;
            Configuration.AutoDetectChangesEnabled = false;
           
        }
    }
}

 	
