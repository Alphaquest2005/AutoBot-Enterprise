

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using CoreEntities.Client.Repositories;


using System.ComponentModel;
using CoreEntities.Client.Entities;
using SimpleMvvmToolkit;


namespace WaterNut.QuerySpace.CoreEntities.ViewModels
{
    public partial class BaseViewModel 
    {
        private List<ApplicationSettings> _applicationSettings;

        private BaseViewModel()
        {
        
            if (CurrentApplicationSettings == null && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                using (var ctx = new ApplicationSettingsRepository())
                {
                    ApplicationSettings = ctx.ApplicationSettings().Result.Where(x => x.IsActive).ToList();
                    CurrentApplicationSettings = ApplicationSettings.FirstOrDefault();
                }

                if (CurrentApplicationSettings == null)
                {
                    MessageBox.Show("No Default Application Settings Defined");
                }
            }
            RegisterToReceiveMessages<ApplicationSettings>(MessageToken.CurrentApplicationSettingsChanged, OnCurrentApplicationSettingsChanged1);
        }

        private void OnCurrentApplicationSettingsChanged1(object sender, NotificationEventArgs<ApplicationSettings> e)
        {
            SystemRepository.Instance.SetCurrentApplicationSettings(e.Data.ApplicationSettingsId);
            AutoBot.Utils.SetCurrentApplicationSettings(e.Data.ApplicationSettingsId);
        }

        public List<ApplicationSettings> ApplicationSettings
        {
            get => _applicationSettings;
            set
            {
                _applicationSettings = value;
                NotifyPropertyChanged(x => this.ApplicationSettings);
            }
        }
   

    
    }
}
