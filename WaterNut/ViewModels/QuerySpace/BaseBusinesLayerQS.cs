

using System.Collections.Generic;
using System.Linq;
using System.Windows;

using CoreEntities.Client.Repositories;


using System.ComponentModel;
using CoreEntities.Client.Entities;


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
                    ApplicationSettings = ctx.ApplicationSettings().Result.ToList();
                    CurrentApplicationSettings = ctx.ApplicationSettings().Result.FirstOrDefault();
                }

                if (CurrentApplicationSettings == null)
                {
                    MessageBox.Show("No Default Application Settings Defined");
                }
            }
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
