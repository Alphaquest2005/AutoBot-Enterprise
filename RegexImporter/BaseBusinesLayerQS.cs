using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Core.Common.UI.DataVirtualization;
using Omu.ValueInjecter;
using CoreEntities.Client.Entities;
using CoreEntities.Client.Repositories;
using SimpleMvvmToolkit;
using WaterNut.QuerySpace.CoreEntities;

namespace RegexImporter
{
    public partial class BaseViewModel: ViewModelBase<Core.Common.UI.BaseViewModel>
    {
        private List<ApplicationSettings> _applicationSettings;

        private BaseViewModel()
        {
        
            if (CurrentApplicationSettings == null && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                using (var ctx = new CoreEntities.Business.Entities.CoreEntitiesContext())
                {


                    ApplicationSettings = ctx.ApplicationSettings
                            .Where(x => x.IsActive)
                            .ToList()
                            .Select(x => new ApplicationSettings().InjectFrom(x))
                            .Cast<ApplicationSettings>().ToList();
                    CurrentApplicationSettings = ApplicationSettings.FirstOrDefault();
                }

                if (CurrentApplicationSettings == null)
                {
                    MessageBox.Show("No Default Application Settings Defined");
                }
            }
            RegisterToReceiveMessages<ApplicationSettings>(MessageToken.CurrentApplicationSettingsChanged, OnCurrentApplicationSettingsChanged1);
        }


        private static readonly BaseViewModel _instance;
        static BaseViewModel()
        {
            _instance = new BaseViewModel();
            Initialization = InitializationAsync();
        }

        public static BaseViewModel Instance
        {
            get { return _instance; }
        }

        public static Task Initialization { get; private set; }
        private static async Task InitializationAsync()
        {
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


        internal void OnCurrentApplicationSettingsChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<ApplicationSettings> e)
        {
            //CurrentApplicationSettings = e.Data;
            NotifyPropertyChanged(m => this.CurrentApplicationSettings);
        }

        private ApplicationSettings _currentApplicationSettings;
        public ApplicationSettings CurrentApplicationSettings
        {
            get
            {
                return _currentApplicationSettings;
            }
            set
            {
                if (_currentApplicationSettings != value)
                {
                    _currentApplicationSettings = value;
                    BeginSendMessage(MessageToken.CurrentApplicationSettingsChanged,
                                                     new NotificationEventArgs<ApplicationSettings>(MessageToken.CurrentApplicationSettingsChanged, _currentApplicationSettings));
                    NotifyPropertyChanged(x => this.CurrentApplicationSettings);
                    // all current navigation properties = null

                }
            }
        }

        VirtualListItem<ApplicationSettings> _vcurrentApplicationSettings;
        public VirtualListItem<ApplicationSettings> VCurrentApplicationSettings
        {
            get
            {
                return _vcurrentApplicationSettings;
            }
            set
            {
                if (_vcurrentApplicationSettings != value)
                {
                    _vcurrentApplicationSettings = value;
                    if (_vcurrentApplicationSettings != null) CurrentApplicationSettings = value.Data;
                    NotifyPropertyChanged(x => this.VCurrentApplicationSettings);
                }
            }
        }


    }
}
