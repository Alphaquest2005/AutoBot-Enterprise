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
using Serilog; // Added for Serilog configuration

namespace AutoWaterNutServer
{
    class Program
    {
        
        [Import]
        public static IEnumerable Services { get; set; }
        
        static void Main(string[] args)
        {
            // Configure centralized Serilog logger for WCF services
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "AutoWaterNutServer")
                .WriteTo.Console(
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .WriteTo.File("logs/AutoWaterNutServer-.txt",
                              rollingInterval: RollingInterval.Day,
                              retainedFileCountLimit: 7,
                              outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .CreateLogger();

            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();

            try
            {
                Log.Information("WCF Server Starting...");
                Log.Information("Centralized logger configured for all WCF services");

                Z.EntityFramework.Extensions.LicenseManager.AddLicense("7242;101-JosephBartholomew", "2080412a-8e17-8a71-cb4a-8e12f684d4da");

                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

                FileIOPermission f2 = new FileIOPermission(FileIOPermissionAccess.Read, Environment.CurrentDirectory + "\\AutoBot-EnterpriseDB.mdf");
                f2.AddPathList(FileIOPermissionAccess.Write | FileIOPermissionAccess.Read, Environment.CurrentDirectory + "\\AutoBot-EnterpriseDB.mdf");
                FileIOPermission f3 = new FileIOPermission(FileIOPermissionAccess.Read, Environment.CurrentDirectory + "\\AutoBot-EnterpriseDB_log.ldf");
                f3.AddPathList(FileIOPermissionAccess.Write | FileIOPermissionAccess.Read, Environment.CurrentDirectory + "\\AutoBot-EnterpriseDB_log.ldf");

                BusinessObjectBase.Container = MEFLoader.Init();

            Services = BusinessObjectBase.Container.GetExportedValues<IBusinessService>().ToList();
            //container.Compose(this);


            Log.Information("Starting {ServiceCount} WCF services...", Services.OfType<IBusinessService>().Count());
            
            Parallel.ForEach(Services.OfType<IBusinessService>(), service =>
                {
                    var s = new ServiceHost(service.GetType());
                    StartService(s);
                    s.Faulted += s_Faulted;
                }
                );
           
            

            Log.Information("Services started. Press [Enter] to exit.");
            Console.ReadLine();
            Log.Information("User pressed [Enter] to exit.");

            //StopService(BatchServiceHost, "BatchService");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly.");
                Debugger.Break();
                Log.Error("Exception Message: {ExceptionMessage}", ex.Message);
                Log.Error("Exception StackTrace: {StackTrace}", ex.StackTrace);
            }
            finally
            {
                Log.Information("Application Shutting Down...");
                Log.CloseAndFlush();
            }
        }
        
        static void s_Faulted(object sender, EventArgs e)
        {
            var serviceHost = sender as ServiceHost;
            Log.Error("Service '{ServiceType}' faulted. Attempting restart...", serviceHost?.Description?.ServiceType?.FullName ?? "Unknown");
            Debugger.Break();
            StopService(serviceHost);
            StartService(serviceHost);
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
                Log.Information("Service '{ServiceType}' started.", host.Description.ServiceType.FullName);

                foreach (var endpoint in host.Description.Endpoints)
                {
                    Log.Information("Listening on endpoint:");
                    Log.Information("Address: {Address}", endpoint.Address.Uri.ToString());
                    Log.Information("Binding: {Binding}", endpoint.Binding.Name);
                    Log.Information("Contract: {Contract}", endpoint.Contract.ConfigurationName);
                }
            }
            catch (Exception)
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
                CloseTimeout = new TimeSpan(0, 180, 0),
                SendTimeout = new TimeSpan(0, 180, 0),
                OpenTimeout = new TimeSpan(0, 180, 0),
                ReceiveTimeout = new TimeSpan(0, 180, 0)
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
            var serviceHost = sender as ServiceHost;
            Log.Error("Host '{ServiceType}' faulted. Attempting restart...", serviceHost?.Description?.ServiceType?.FullName ?? "Unknown");
            Debugger.Break();
            StopService(serviceHost);
            StartService(serviceHost);
        }

        static void StopService(ServiceHost host)
        {
            host.Close();
            Log.Information("Service '{ServiceType}' stopped.", host.Description.ServiceType.FullName);
        }
    }
}
