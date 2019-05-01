using CoreEntities.Client.Entities;
using CoreEntities.Client.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleMvvmToolkit;

namespace WaterNut.QuerySpace.CoreEntities.ViewModels
{
    public class MainViewModel : ViewModelBase<MainViewModel>
    {
        public MainViewModel()
        {
            try
            {
                using (ApplicationSettingsRepository ctx = new ApplicationSettingsRepository())
                {
                    BaseViewModel.Instance.CurrentApplicationSettings = ctx.ApplicationSettings().Result.FirstOrDefault(x => x.Description == "WaterNut");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

