using Core.Common.UI.DataVirtualization;
using CounterPointQS.Client.Entities;
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
using WaterNut.QuerySpace.CounterPointQS.ViewModels;

namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for CPPurchaseOrders.xaml
	/// </summary>
	public partial class CPPurchaseOrders : UserControl
	{
		public CPPurchaseOrders()
		{
            try
            {
                InitializeComponent();
                im = (CPPurchaseOrdersModel)FindResource("CPPurchaseOrdersModelDataSource");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message + Ex.StackTrace);
            }
			// Insert code required on object creation below this point.
		}
        CPPurchaseOrdersModel im; 
        private async void DownloadTxt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            await im.DownloadCPO((CPOGrd.SelectedItem as VirtualListItem<CounterPointPOs>).Data).ConfigureAwait(false);
            e.Handled = true;
        }

        private async void CheckBox_Checked(object sender, MouseButtonEventArgs e)
        {
           
          
        }

        private void ViewAll(object sender, MouseButtonEventArgs e)
        {
            im.ViewAll();
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

	    private async void CheckBox_MouseDown(object sender, MouseButtonEventArgs e)
	    {
            await im.DownloadCPO((((FrameworkElement)sender).DataContext as VirtualListItem<CounterPointPOs>).Data).ConfigureAwait(false);
            e.Handled = true;
	    }
	}
}