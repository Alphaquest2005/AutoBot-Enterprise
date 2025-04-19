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
using WaterNut.QuerySpace.CoreEntities.ViewModels;

namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for LicenceSummary.xaml
	/// </summary>
	public partial class LicenceSummary : UserControl
	{
		public LicenceSummary()
		{
			InitializeComponent();
		    im =(LicenceSummaryModel) FindResource("LicenceSummaryModelDataSource");

		    // Insert code required on object creation below this point.
		}

	    private LicenceSummaryModel im;
        private void Send2Excel(object sender, MouseButtonEventArgs e)
        {
            im.Send2Excel();
        }

        private async void RefreshData(object sender, MouseButtonEventArgs e)
        {
            await im.RefreshData().ConfigureAwait(false);
        }
	}
}