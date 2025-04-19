using Core.Common.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using TrackableEntities;
using TrackableEntities.EF6;




namespace Core.Common.Data
{
   
        public abstract class BaseRepository<U> : IDataRepository, IDisposable
       where U : DbContext, new()
        {
            public BaseRepository(string conn)
            {
                bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
                if (designMode)
                {
                    context = (U)Activator.CreateInstance(typeof(U), conn);
                }
                else
                {
                    context = (U)Activator.CreateInstance(typeof(U));
                }
                
               

            }
            public BaseRepository()
            {

                context = (U)Activator.CreateInstance(typeof(U));

            }
            public U context;


            public bool SaveChanges<T>(T obj) where T : ITrackable
            {
                // context.AttachTo(EntitySet,emp);

                try
                {
                    if (obj == null) return false;

                    context.ApplyChanges(obj);
                    context.SaveChanges();
                   // obj.AcceptChanges();
                    return true;

                }
                catch (Exception ex)
                {

                    throw ex;
                }
                
                   // return false;
               

            }
            public void Delete(ITrackable obj) 
            {
               
                try
                {

                    if (obj == null) return;
                   // var c = context.ObjectStateManager.GetObjectStateEntry(obj);
                    DbSet ds = context.Set(obj.GetType());
                    ds.Attach(obj);//c.EntitySet.Name
                    ds.Remove(obj);
                    context.SaveChanges();
                    //obj.AcceptChanges();
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }


            



            #region IDisposable Members
            private bool _disposed;
            public void Dispose()
            {
                Dispose(true);

                // Use SupressFinalize in case a subclass 
                // of this type implements a finalizer.
                GC.SuppressFinalize(this);
            }
            protected virtual void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        // Clear all property values that maybe have been set
                        // when the class was instantiated
                        context.Dispose();
                    }

                    // Indicate that the instance has been disposed.
                    _disposed = true;
                }
            }
            #endregion
        }

    
}
