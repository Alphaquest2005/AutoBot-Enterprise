using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace WaterNut.Client.Services
{
    public class ClientService<T> : ClientBase<T>, IDisposable where T: class
    {
       public ClientService()
           : base(
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
        
        void IDisposable.Dispose()
        {
            base.Close();
        }
    }
}
