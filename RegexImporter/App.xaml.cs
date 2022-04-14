using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Core.Common;
using CoreEntities.Client.Repositories;
using WaterNut.Client.Bootstrapper;

namespace RegexImporter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
            : base()
        {
            try
            {
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

                
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
                            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
