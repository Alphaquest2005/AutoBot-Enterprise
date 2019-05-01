
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core;

using System.Data.SqlClient;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Transactions;
using DocumentItemDS.Business.Entities;
using Core.Common.Business.Services;
using TrackableEntities.EF6;
using System.Linq;

namespace DocumentItemDS.Business.Services
{

    public partial class xcuda_ItemService
    {
        public async Task<bool> Updatexcuda_Items(List<xcuda_Item> entities)
        {
            var exceptions = new ConcurrentQueue<Exception>();
            Parallel.ForEach(entities, itm =>
            {
                try
                {
                    UpdateEntity(itm.ChangeTracker.GetChanges().FirstOrDefault());
                }
                catch (Exception ex)
                {
                    exceptions.Enqueue(ex);
                }

            });
            if (exceptions.Count > 0) throw new AggregateException(exceptions);
            return true;
        }

        private void UpdateEntity(xcuda_Item entity)
        {


            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew,
                    new TransactionOptions() {IsolationLevel = IsolationLevel.ReadUncommitted}))
            {
                using (var dbContext = new DocumentItemDSContext())
                {
                    try
                    {
                        dbContext.ApplyChanges(entity);
                        dbContext.SaveChanges();
                        //entity.AcceptChanges();
                        scope.Complete();
                    }
                    catch (DbUpdateConcurrencyException dce)
                    {
                        // Get failed entry
                        foreach (var itm in dce.Entries)
                        {
                            itm.OriginalValues.SetValues(itm.GetDatabaseValues());
                        }
                    }
                    catch (OptimisticConcurrencyException oce)
                    {
                        var context = ((IObjectContextAdapter) dbContext).ObjectContext;

                        foreach (var entry in oce.StateEntries)
                        {
                            context.Refresh(System.Data.Entity.Core.Objects.RefreshMode.StoreWins, entry.Entity);
                        }
                    }
                    catch (DbUpdateException e)
                    {
                        // Debugger.Break();
                        SqlException s = e.InnerException.InnerException as SqlException;
                        if (s != null && s.Number == 2627)
                        {
                        }
                        else
                        {
                            Debugger.Break();
                            throw;
                        }
                    }
                    catch (Exception updateEx)
                    {
                        System.Diagnostics.Debugger.Break();
                        //throw new FaultException(updateEx.Message);
                        var fault = new ValidationFault
                        {
                            Result = false,
                            Message = updateEx.Message,
                            Description = updateEx.StackTrace
                        };
                        throw new FaultException<ValidationFault>(fault);
                    }
                }

            }

        }
    }
}



