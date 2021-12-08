using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Core.Common.UI.DataVirtualization;
using CoreEntities.Business.Entities;
using OCR.Client.Entities;
using Omu.ValueInjecter;
using SimpleMvvmToolkit;
using WaterNut.QuerySpace.CoreEntities;
using ApplicationSettings = CoreEntities.Client.Entities.ApplicationSettings;

namespace RegexImporter
{
    public class BaseViewModel : ViewModelBase<Core.Common.UI.BaseViewModel>
    {
        private List<ApplicationSettings> _applicationSettings;

        private ApplicationSettings _currentApplicationSettings;
        private ImportErrors _currentImportError;


        private VirtualListItem<ApplicationSettings> _vcurrentApplicationSettings;

        static BaseViewModel()
        {
            Instance = new BaseViewModel();
            Initialization = InitializationAsync();
        }

        private BaseViewModel()
        {
            if (CurrentApplicationSettings == null && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    ApplicationSettings = ctx.ApplicationSettings
                        .Where(x => x.IsActive)
                        .ToList()
                        .Select(x => new ApplicationSettings().InjectFrom(x))
                        .Cast<ApplicationSettings>().ToList();
                    CurrentApplicationSettings = ApplicationSettings.FirstOrDefault();
                }

                if (CurrentApplicationSettings == null) MessageBox.Show("No Default Application Settings Defined");
            }

            RegisterToReceiveMessages<ApplicationSettings>(MessageToken.CurrentApplicationSettingsChanged,
                OnCurrentApplicationSettingsChanged1);
        }

        public static BaseViewModel Instance { get; }

        public static Task Initialization { get; }

        public List<ApplicationSettings> ApplicationSettings
        {
            get => _applicationSettings;
            set
            {
                _applicationSettings = value;
                NotifyPropertyChanged(x => ApplicationSettings);
            }
        }

        public ApplicationSettings CurrentApplicationSettings
        {
            get => _currentApplicationSettings;
            set
            {
                if (_currentApplicationSettings != value)
                {
                    _currentApplicationSettings = value;
                    BeginSendMessage(MessageToken.CurrentApplicationSettingsChanged,
                        new NotificationEventArgs<ApplicationSettings>(MessageToken.CurrentApplicationSettingsChanged,
                            _currentApplicationSettings));
                    NotifyPropertyChanged(x => CurrentApplicationSettings);
                    // all current navigation properties = null
                }
            }
        }

        public ImportErrors CurrentImportError
        {
            get => _currentImportError;
            set
            {
                _currentImportError = value;
                BeginSendMessage(MessageToken.CurrentApplicationSettingsChanged,
                    new NotificationEventArgs<ImportErrors>(
                        WaterNut.QuerySpace.OCR.MessageToken.CurrentImportErrorsChanged, _currentImportError));
                NotifyPropertyChanged(x => CurrentImportError);
            }
        }

        public VirtualListItem<ApplicationSettings> VCurrentApplicationSettings
        {
            get => _vcurrentApplicationSettings;
            set
            {
                if (_vcurrentApplicationSettings != value)
                {
                    _vcurrentApplicationSettings = value;
                    if (_vcurrentApplicationSettings != null) CurrentApplicationSettings = value.Data;
                    NotifyPropertyChanged(x => VCurrentApplicationSettings);
                }
            }
        }

        private static async Task InitializationAsync()
        {
        }

        private void OnCurrentApplicationSettingsChanged1(object sender, NotificationEventArgs<ApplicationSettings> e)
        {
            //SystemRepository.Instance.SetCurrentApplicationSettings(e.Data.ApplicationSettingsId);

            //if(WaterNut.QuerySpace.CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId != e.Data.ApplicationSettingsId)
            //    WaterNut.QuerySpace.CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings = e.Data;

            //AutoBot.Utils.SetCurrentApplicationSettings(e.Data.ApplicationSettingsId);
            CurrentApplicationSettings = e.Data;
        }


        internal void OnCurrentApplicationSettingsChanged(object sender, NotificationEventArgs<ApplicationSettings> e)
        {
            //CurrentApplicationSettings = e.Data;
            NotifyPropertyChanged(m => CurrentApplicationSettings);
        }
    }
}