using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WaterNut.QuerySpace;
using Core.Common.UI;

namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for MainView.xaml
	/// </summary>
	public partial class MainView : UserControl
	{
		public MainView()
		{
            try
            {

                InitializeComponent();
                BaseViewModel.Slider = slider;
                //im = (MainViewModel)this.FindResource("MainViewModelDataSource");
                // Insert code required on object creation below this point.
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message + Ex.StackTrace);
            }

		}

		//MainViewModel im;
		private void BringIntoView(object sender, MouseEventArgs e)
		{

			BringIntoView(sender);
		}

		private static void BringIntoView(object sender)
		{
			if (typeof(Expander).IsInstanceOfType(sender))
			{
				BaseViewModel.Slider.BringIntoView(((FrameworkElement)sender) as Expander);
			}
			else
			{
               // var p = ((FrameworkElement)sender).Parent as Expander;
               //// p.IsExpanded = true;
               // p.UpdateLayout();
               // BaseViewModel.Slider.BringIntoView(p);
			}
		}

		private void BackBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (IsMouseOver == true)
			{
				BaseViewModel.Slider.MoveToPreviousCtl();
			}
		}

		private void BringIntoViewClick(object sender, MouseButtonEventArgs e)
		{
			BringIntoView(((FrameworkElement)sender).Parent);
		}

		
	}
}