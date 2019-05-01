using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Client.Services
{
    public class ClientService<T> : ClientBase<T>, IDisposable where T: class
    {
       public ClientService()
           : base( //new NetTcpBinding(SecurityMode., true) 
            //                        { MaxReceivedMessageSize = Int32.MaxValue,
            //                        TransactionFlow = true,
            //                        //MaxConnections = 48,
                                    
            //                        ReceiveTimeout = new TimeSpan(0,25,0),
            //                          OpenTimeout = new TimeSpan(0,25, 0),
            //                          CloseTimeout = new TimeSpan(0,25, 0),
            //                          SendTimeout = new TimeSpan(0, 25, 0),
            //                        ReliableSession = new OptionalReliableSession(new ReliableSessionBindingElement(true))
            //                        },
           new NetTcpBinding(SecurityMode.None, true)
           {
               MaxReceivedMessageSize = Int32.MaxValue,
               TransactionFlow = false,
               MaxConnections = 1000,
               ListenBacklog = 1000,
               PortSharingEnabled = false,               
               ReliableSession =  new OptionalReliableSession(new ReliableSessionBindingElement(false)){Enabled = false},                                    
                                    ReceiveTimeout = new TimeSpan(0,60,0),
                                      OpenTimeout = new TimeSpan(0,60, 0),
                                      CloseTimeout = new TimeSpan(0,60, 0),
                                      SendTimeout = new TimeSpan(0, 60, 0),
                                    
                                    },
                  new EndpointAddress("net.tcp://localhost:8733/" + typeof(T).FullName.Replace(".Client.Contracts.I", ".Business.Services."))
                  )
        {
            
        }

       protected virtual void Dispose()
       {
           Dispose(true);
       }

       /// <summary>
       /// Dispose worker method. Handles graceful shutdown of the
       /// client even if it is an faulted state.
       /// </summary>
       /// <param name="disposing">Are we disposing (alternative
       /// is to be finalizing)</param>
       protected virtual void Dispose(bool disposing)
       {
           if (disposing)
           {
               try
               {
                   if (State != CommunicationState.Faulted)
                   {
                       Close();
                   }
               }
               finally
               {
                   if (State != CommunicationState.Closed)
                   {
                       Abort();
                   }
                   GC.SuppressFinalize(this);
               }
           }
       }

    /// <summary>
    /// Finalizer.
    /// </summary>
       ~ClientService()
    {
        Dispose(false);
    }
    }
}
