
using System.Data.Entity;
using CoreEntities.Business.Entities;
using System.Data.Entity.Infrastructure;
using SalesDataQS.Business.Entities.Mapping;
using WaterNut.Data;


namespace SalesDataQS.Business.Entities
{
  
    public partial class SalesDataQSContext 
    {
        partial void OnModelCreatingExtentsion(DbModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<AsycudaDocumentItem>();
            modelBuilder.Ignore<LicenceSummary>();
            modelBuilder.Ignore<AsycudaDocumentSetEx>();
        }
    }
}

 	
