using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using CoreObjects = System.Data.Entity.Core.Objects; // Added alias
using System.Data.Entity.Core;

namespace WaterNut.DataLayer
{
    static class Extensions
    {
        public static void SetCommandTimeout(this IObjectContextAdapter db, TimeSpan? timeout)
        {
            db.ObjectContext.CommandTimeout = timeout.HasValue ? (int?)timeout.Value.TotalSeconds : null;
        }

    }

    public partial class WaterNutDBEntities 
    {
       
        public override int SaveChanges(SaveOptions options)
        {
            try
            {
                var now = DateTime.UtcNow;

                var entities = ObjectStateManager.GetObjectStateEntries(System.Data.Entity.EntityState.Added | System.Data.Entity.EntityState.Modified) // Explicitly qualify EntityState
                                        .Select(e => e.Entity)
                                        .OfType<IHasEntryTimeStamp>();

                foreach (var entry in entities)
                {

                    // IHasEntryTimeStamp lastModified = entry as IHasEntryTimeStamp;
                    //if (lastModified != null)
                    entry.EntryTimeStamp = now;

                }

                return base.SaveChanges(options);
            }
            catch (System.Data.Entity.Core.UpdateException) // Qualified UpdateException
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public interface IHasEntryTimeStamp
    {
        DateTime? EntryTimeStamp { get; set; }
    }

}




