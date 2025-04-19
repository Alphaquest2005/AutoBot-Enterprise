using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace WaterNut
{
    public class SplashScreen2 : SplashScreen
{
    private DispatcherTimer dt;
    
    public SplashScreen2(string resourceName)
        : base(resourceName)
    { }
    
    /// <summary>
    /// Shows the splash screen
    /// </summary>
    /// <param name="minDur">The minimum duration this splash should show for</param>
    /// <param name="topmost">True if the splash screen should appear 
    /// top most in windows. Recommended to be true, to ensure it does not appear 
   /// behind the loaded main window</param>
    public void Show(TimeSpan minDur, bool topmost = false)
    {
        if (dt == null)//prevent calling twice
        {
            base.Show(false, topmost);
            //dt = new DispatcherTimer(minDur, DispatcherPriority.Loaded, 
            //            CloseAfterDelay, Dispatcher.CurrentDispatcher);
        }
    }
    
    private void CloseAfterDelay(object sender, EventArgs e)
    {
        dt.Stop();
        Close(TimeSpan.FromMilliseconds(300));
    }
}
}
