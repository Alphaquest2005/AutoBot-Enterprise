using System;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using AllocationQS.Business.Services;
using Core.Common.Business.Services;
using Core.Common.Contracts;
using CoreEntities.Business.Entities;
using WaterNut.DataSpace;


namespace CoreEntities.Business.Services
{
    [Export(typeof(IAsycudaSalesAllocationsExService))]
    [Export(typeof(IBusinessService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession,
                     ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class SystemService : ISystemService, IDisposable
    {


        public bool ValidateInstallation()
        {
            try
            {
                
                RunCmd("sqllocaldb stop mssqllocaldb");
                RunCmd("sqllocaldb delete mssqllocaldb");
                RunCmd("sqllocaldb start \"MSSQLLocalDB\"");
                return WaterNut.DataSpace.BaseDataModel.Instance.ValidateInstallation();
            }
            catch (Exception e)
            {
                var ex = e;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }

                if (ex.Message.Contains("Login failed for user"))
                {
                    RunCmd("sqllocaldb stop mssqllocaldb");
                    RunCmd("sqllocaldb delete mssqllocaldb");
                    RunCmd("sqllocaldb start \"MSSQLLocalDB\"");
                   
                }
                var fault = new ValidationFault
                {
                    
                    Result = false,
                    Message = ex.Message,
                    Description = ex.StackTrace
                };
                throw new FaultException<ValidationFault>(fault, ex.Message);
            }

        }

        

        public void SetCurrentApplicationSettings(int applicationSettingId)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                BaseDataModel.Instance.CurrentApplicationSettings =
                    ctx.ApplicationSettings
                        .Include(x => x.Declarants)
                        .Where(x => x.IsActive)
                        .First(x => x.ApplicationSettingsId == applicationSettingId);
            }
        }

        private static void RunCmd(string cmd)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
 {
     WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
     FileName = "cmd.exe",
     Arguments = "/C " + cmd
 };
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        #region IDisposable Members

        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        #endregion

    }
}

