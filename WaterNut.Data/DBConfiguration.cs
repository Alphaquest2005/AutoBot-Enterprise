using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterNut.Data
{
    public class DBConfiguration:DbConfiguration
    {
        public DBConfiguration()
        {
           // SetTransactionHandler(SqlProviderServices.ProviderInvariantName, () => new CommitFailureHandler()); 
            SetExecutionStrategy("System.Data.SqlClient", () => new DBExecutionStrategy(20, new TimeSpan(0,0,0,2)));
        }
    }
}
