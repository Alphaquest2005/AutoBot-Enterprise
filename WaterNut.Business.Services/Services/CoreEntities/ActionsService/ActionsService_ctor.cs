using System;
using System.ServiceModel;
using Core.Common.Business.Services; // Assuming ValidationFault is here

namespace CoreEntities.Business.Services
{
    public partial class ActionsService // Assuming IActionsService, IDisposable are handled in other partials
    {
        public ActionsService()
        {
            try
            {
                // dbContext = new CoreEntitiesContext(){StartTracking = StartTracking};
                StartTracking = false;
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
                    throw new FaultException<ValidationFault>(fault, new FaultReason(fault.Message));
            }
        }
    }
}