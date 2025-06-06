using System;
using System.Collections.Generic;
using System.Data.Entity; // For AsNoTracking
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Business.Services; // Assuming ValidationFault is here
using CoreEntities.Business.Entities; // Assuming Actions, CoreEntitiesContext are here
using TrackableEntities.Common; // For StartTracking extension method

namespace CoreEntities.Business.Services
{
    public partial class ActionsService // Assuming IActionsService, IDisposable are handled in other partials
    {
        public async Task<IEnumerable<Actions>> GetActions(List<string> includesLst = null, bool tracking = true)
        {
            try
            {
                //using (var scope = new TransactionScope(TransactionScopeOption.Required,
                                       //new TransactionOptions() {IsolationLevel = IsolationLevel.ReadUncommitted}))
                   // {
                      using ( var dbContext = new CoreEntitiesContext(){StartTracking = StartTracking})
                      {
                        // This calls AddIncludes, which needs to be in its own partial class
                        var set = AddIncludes(includesLst, dbContext);
                        IEnumerable<Actions> entities = set.AsNoTracking().ToList();
                               //scope.Complete();
                                if(tracking) entities.AsParallel(new ParallelLinqOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }).ForAll(x => x.StartTracking());
                                return entities;
                       }
                    //}
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