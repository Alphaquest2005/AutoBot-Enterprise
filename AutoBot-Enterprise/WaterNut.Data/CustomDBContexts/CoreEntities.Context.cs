


using System.Data.Entity;
using System.Linq;
using CoreEntities.Business.Entities;
using CoreEntities.Business.Entities.Mapping;
using WaterNut.Data;


namespace CoreEntities.Business.Entities
{
    public partial class CoreEntitiesContext
    {

        
        partial void OnModelCreatingExtentsion(DbModelBuilder modelBuilder)
        {
            //var s = AsycudaDocumentSetExs.SqlQuery();
            Database.CommandTimeout = 0;
        }
		
    }
}

 	
