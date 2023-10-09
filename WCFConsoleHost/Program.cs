using System.Diagnostics;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Core.Common;
using Core.Common.Contracts;
using HRManager.Business.Bootstrapper;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;

using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Security.Permissions;

namespace AutoWaterNutServer
{
    class Program
    {
        
        [Import]
        public static IEnumerable Services { get; set; }
        
        static void Main(string[] args)
        {
            try
            {
                Z.EntityFramework.Extensions.LicenseManager.AddLicense("7242;101-Joseph-Bartholomew", "09748212-b8b0-15fc-e099-d4e670e6d713");

                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

                FileIOPermission f2 = new FileIOPermission(FileIOPermissionAccess.Read, Environment.CurrentDirectory + "\\AutoBot-EnterpriseDB.mdf");
                f2.AddPathList(FileIOPermissionAccess.Write | FileIOPermissionAccess.Read, Environment.CurrentDirectory + "\\AutoBot-EnterpriseDB.mdf");
                FileIOPermission f3 = new FileIOPermission(FileIOPermissionAccess.Read, Environment.CurrentDirectory + "\\AutoBot-EnterpriseDB_log.ldf");
                f3.AddPathList(FileIOPermissionAccess.Write | FileIOPermissionAccess.Read, Environment.CurrentDirectory + "\\AutoBot-EnterpriseDB_log.ldf");

                BusinessObjectBase.Container = MEFLoader.Init();

            Services = BusinessObjectBase.Container.GetExportedValues<IBusinessService>().ToList();
            //container.Compose(this);


            Parallel.ForEach(Services.OfType<IBusinessService>(), service =>
                {
                    var s = new ServiceHost(service.GetType());
                    StartService(s);
                    s.Faulted += s_Faulted;
                }
                );
           
            

            Console.WriteLine("");
            Console.WriteLine("Press [Enter] to exit.");
            Console.ReadLine();
            Console.WriteLine("");

            //StopService(BatchServiceHost, "BatchService");
            }
            catch (Exception ex)
            {
                Debugger.Break();
                Console.WriteLine(ex.Message);
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine(ex.StackTrace);
            }
        }
        
        static void s_Faulted(object sender, EventArgs e)
        {
            Debugger.Break();
            StopService(sender as ServiceHost);
            StartService(sender as ServiceHost);

        }

        static void StartService(ServiceHost host)
        {
            try
            {
                // configure endpoint
                var _netTcpBinding = ConfigNetTcpBinding();

                host.AddServiceEndpoint(host.Description.ServiceType.FullName.Replace(".Business.Services.", ".Business.Services.I"), _netTcpBinding, "net.tcp://localhost:8734/" + host.Description.ServiceType.FullName);
                host.Faulted += host_Faulted;
                host.UnknownMessageReceived +=host_UnknownMessageReceived;
                host.Closed += host_Closed;
                host.Open();
                Console.WriteLine("Service '{0}' started.", host.Description.ServiceType.FullName);

                foreach (var endpoint in host.Description.Endpoints)
                {
                    Console.WriteLine(string.Format("Listening on endpoint:"));
                    Console.WriteLine($"Address: {endpoint.Address.Uri.ToString()}");
                    Console.WriteLine($"Binding: {endpoint.Binding.Name}");
                    Console.WriteLine($"Contract: {endpoint.Contract.ConfigurationName}");
                }

                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw;
            }
        }

        private static NetTcpBinding ConfigNetTcpBinding()
        {
          
            var _netTcpBinding = new NetTcpBinding(SecurityMode.None, false)
            {
                MaxConnections = 1000,
                TransactionFlow = false,
                PortSharingEnabled = false,
                ListenBacklog = 1000,
                MaxReceivedMessageSize = Int32.MaxValue,
                ReliableSession = new OptionalReliableSession(new ReliableSessionBindingElement(false)){Enabled = false},
                CloseTimeout = new TimeSpan(0, 60, 0),
                SendTimeout = new TimeSpan(0, 60, 0),
                OpenTimeout = new TimeSpan(0, 60, 0),
                ReceiveTimeout = new TimeSpan(0, 60, 0)
            };

            return _netTcpBinding;

        }

        static void host_Closed(object sender, EventArgs e)
        {
            Debugger.Break();
        }

        private static void host_UnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
        {
            Debugger.Break();
        }

        static void host_Faulted(object sender, EventArgs e)
        {
            Debugger.Break();
            StopService(sender as ServiceHost);
            StartService(sender as ServiceHost);
        }

        static void StopService(ServiceHost host)
        {
            host.Close();
            Console.WriteLine("Service '{0}' stopped.", host.Description.ServiceType.FullName);
        }
    }
}
