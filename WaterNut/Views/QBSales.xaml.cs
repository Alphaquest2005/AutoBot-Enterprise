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
using WaterNut.QuerySpace.QuickBooksQS.ViewModels;


namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for QBSales.xaml
	/// </summary>
	public partial class QBSales : UserControl
	{
		public QBSales()
		{
			InitializeComponent();
			
			// Insert code required on object creation below this point.
            im = (QBSalesModel)FindResource("QBSalesModelDataSource");

		}

	    private QBSalesModel im;
        private async void ImportSalesDateRange(object sender, MouseButtonEventArgs e)
        {
           await im.DownloadQBData().ConfigureAwait(false);
        }
	}
}