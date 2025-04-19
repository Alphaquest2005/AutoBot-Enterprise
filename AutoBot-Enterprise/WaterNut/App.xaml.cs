using System.Threading.Tasks;
using System.Windows.Threading;
using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security.Permissions;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using Core.Common.UI;
using CoreEntities.Client.Repositories;
using CoreEntities.Client.Services;
using WaterNut.Client.Bootstrapper;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace WaterNut
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
       
        public static SplashScreen2 splash = new SplashScreen2(@"coconut-water-splash.jpg");

        public App()
            : base()
        {

            try
            {
                Z.EntityFramework.Extensions.LicenseManager.AddLicense("7242;101-JosephBartholomew", "2080412a-8e17-8a71-cb4a-8e12f684d4da");
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
                
                splash.Show(TimeSpan.FromSeconds(0));
                Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                Current.Exit += Current_Exit;
                Dispatcher.UnhandledException += OnDispatcherUnhandledException;



                if (Core.Common.Utils.ProcessExtentions.IsProcessOpen("AutoWaterNutServer") == null)
                {
                    Process p = new Process();
                    p.StartInfo = new ProcessStartInfo("AutoWaterNutServer.exe")
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };
                    p.Start();
                }
                // LoginRoutine();

                try
                {
                    ClientObjectBase.Container = MEFLoader.Init(new List<ComposablePartCatalog>()
                    {
                        new AssemblyCatalog(Assembly.GetExecutingAssembly())
                    });



                    if (!SystemRepository.Instance.ValidateInstallation())
                    {
                        MessageBox.Show("Invalid Installation", "Asycuda Toolkit", MessageBoxButton.OK,
                            MessageBoxImage.Exclamation);
                        Current.Shutdown();
                    }


                    AsycudaDocumentSetExRepository.Instance.BaseDataModelInitialize().Wait();

                }
                catch (Exception e)
                {
                    var lastexception = false;
                    var errorMessage = "Loading components";
                    Exception exp = e;
                    while (lastexception == false)
                    {
                        if (exp.InnerException == null)
                        {
                            lastexception = true;
                            errorMessage +=
                                $"An unhandled Exception occurred!: {exp.Message} "; //---- {1}, exp.StackTrace
                            ////////MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            //e. = true;
                        }

                        errorMessage += $"An unhandled Exception occurred!: {exp.Message}"; //---- {1}
                        exp = exp.InnerException;

                    }
                }


            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Invalid Configuration")) throw;
                MessageBox.Show(ex.Message + "|" + ex.StackTrace);
            }

        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            var p = Core.Common.Utils.ProcessExtentions.IsProcessOpen("AutoWaterNutServer");
            if (p != null)
            {
                p.Kill();
            }
        }

        void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var lastexception = false;
            var errorMessage = "Dispatcher";
            Exception exp = e.Exception; 
            while (lastexception == false)
            {
                if (exp.InnerException == null)
                {
                    lastexception = true;
                     errorMessage += $"An unhandled Exception occurred!: {exp.Message} ";//---- {1}, exp.StackTrace
                    MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    e.Handled = true;
                }
                errorMessage += $"An unhandled Exception occurred!: {exp.Message}"; //---- {1}
                exp = exp.InnerException;

            }
        }

    
    }
}
