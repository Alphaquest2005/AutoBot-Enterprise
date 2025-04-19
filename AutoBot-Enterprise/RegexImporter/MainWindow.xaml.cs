using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Core.Common.UI;

namespace RegexImporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //App.splash.Close(TimeSpan.FromSeconds(1));
            Core.Common.UI.BaseViewModel.Slider.MoveTo("InvoiceSummaryEXP");
        }

        bool collapseHome = false;
        private void Expander_Expanded_1(object sender, RoutedEventArgs e)
        {
            FrameworkElement p = FooterBar;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(p); i++)
            {
                var child = VisualTreeHelper.GetChild(p, i);
                if (typeof(Expander).IsInstanceOfType(child) && child != sender)
                {
                    if (child == homeExpand && collapseHome == false)
                    {
                        collapseHome = true;
                    }

                    (child as Expander).IsExpanded = false;

                }

            }
            if (((Expander)sender).Name == "homeExpand")
            {
                collapseHome = false;
            }
            else
            {
                collapseHome = true;
                homeExpand.IsExpanded = false;
            }


        }
        private void HomeExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            if (collapseHome == false)
            {
                ((Expander)sender).UpdateLayout();
                ((Expander)sender).IsExpanded = true;
                // collapseHome = true;
            }

        }

        private void HomeBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Core.Common.UI.BaseViewModel.Slider.MoveTo("InvoiceSummaryExP");
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();

        }

        private void SwitchWindowState(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                return;
            }
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;

            }


        }
     

        private void MinimizeWindow(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseWindow(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
        
    }
}
