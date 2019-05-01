

using System;
using System.ServiceModel;
using System.Threading;
using Core.Common.Client.Repositories;
using CoreEntities.Client.Services;


using System.Threading.Tasks;


namespace CoreEntities.Client.Repositories 
{
   
    public partial class SystemRepository :BaseRepository<SystemRepository>
    {
        
       private static readonly SystemRepository instance;
       static SystemRepository()
        {
            instance = new SystemRepository();
        }

       public static SystemRepository Instance
        {
            get { return instance; }
        }
        public bool ValidateInstallation()
        {
            WaitForServiceToStart();
            using (var ctx = new SystemClient())
            {
                return ctx.ValidateInstallation();
            }
        }

        private static bool loaded = false;

        private void WaitForServiceToStart()
        {
            if (loaded) return;
            var _proxy = new ApplicationSettingsClient();
            while (!_proxy.State.Equals(CommunicationState.Opened))
            {
                if (!_proxy.State.Equals(CommunicationState.Opening))
                {
                    try
                    {
                        _proxy.Open();
                    }
                    catch (EndpointNotFoundException enfe)
                    {
                        _proxy.Abort();
                        _proxy = new ApplicationSettingsClient();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                Thread.Sleep(1000);
            }
            loaded = true;
        }

    }
}

