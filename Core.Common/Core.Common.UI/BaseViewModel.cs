using System;
using SimpleMvvmToolkit;

namespace Core.Common.UI
{
    public class BaseViewModel : ViewModelBase<BaseViewModel>
    {
        public static SliderPanel Slider { get; set; }

        public static bool IsMyComputer => "JOSEPH-PC|AUTOBROKER-PC".Contains(Environment.MachineName);
    }
}