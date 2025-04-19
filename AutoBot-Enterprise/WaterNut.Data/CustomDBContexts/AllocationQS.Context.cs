using System.Data.Entity;
using CoreEntities.Business.Entities;
using AllocationQS.Business.Entities.Mapping;


namespace AllocationQS.Business.Entities
{
    public partial class AllocationQSContext 
    {
        partial void OnModelCreatingExtentsion(DbModelBuilder modelBuilder)
        {
            //var s = AsycudaDocumentSetExs.SqlQuery();
            Database.CommandTimeout = 0;
        }
    }
}

 	
