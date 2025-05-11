using System;
using System.Threading.Tasks;
using SimpleMvvmToolkit;

namespace Core.Common.UI
{
    public class BaseViewModel : ViewModelBase<BaseViewModel>
    {
        public static SliderPanel Slider { get; set; }

        public static bool IsMyComputer => "JOSEPH-PC|AUTOBROKER-PC|JOEXPS".Contains(Environment.MachineName);

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
        private static Task InitializationAsync()
        {
            return Task.CompletedTask;
        }

    }
}