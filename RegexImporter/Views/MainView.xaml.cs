using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RegexImporter.Views
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
				Core.Common.UI.BaseViewModel.Slider = slider;
				//im = (MainViewModel)this.FindResource("MainViewModelDataSource");
				// Insert code required on object creation below this point.
				CompanyLst.Visibility = Core.Common.UI.BaseViewModel.IsMyComputer ? Visibility.Visible : Visibility.Collapsed;
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
				Core.Common.UI.BaseViewModel.Slider.BringIntoView(((FrameworkElement)sender) as Expander);
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
				Core.Common.UI.BaseViewModel.Slider.MoveToPreviousCtl();
			}
		}

		private void BringIntoViewClick(object sender, MouseButtonEventArgs e)
		{
			BringIntoView(((FrameworkElement)sender).Parent);
		}

		
	}
}