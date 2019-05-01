using Core.Common.UI.DataVirtualization;
using CounterPointQS.Client.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WaterNut.QuerySpace.CounterPointQS.ViewModels;

namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for CPSales.xaml
	/// </summary>
	public partial class CPSales : UserControl
	{
		public CPSales()
		{
			InitializeComponent();
			
			// Insert code required on object creation below this point.
            im = (CPSalesModel)FindResource("CPSalesModelDataSource");
		}

        private CPSalesModel im;
        private async void DownloadTxt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            await im.DownloadCPSales((SalesGrd.SelectedItem as VirtualListItem<CounterPointSales>).Data).ConfigureAwait(false);
            e.Handled = true;
        }

       
        private async void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            await im.DownloadCPSales((((FrameworkElement)sender).DataContext as VirtualListItem<CounterPointSales>).Data).ConfigureAwait(false);
            e.Handled = true;
        }


        private async void ImportSalesDateRange(object sender, MouseButtonEventArgs e)
        {
            await im.DownloadCPSalesDateRange().ConfigureAwait(false);
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = sender as TextBox;
                if (textBox != null)
                    textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
	}
}