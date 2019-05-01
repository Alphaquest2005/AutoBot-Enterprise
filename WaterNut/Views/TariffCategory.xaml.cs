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
using WaterNut.QuerySpace.InventoryQS.ViewModels;

namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for TariffCategory.xaml
	/// </summary>
	public partial class TariffCategory : UserControl
	{
		public TariffCategory()
		{
			InitializeComponent();
            im = (TariffCategoryModel)FindResource("TariffCategoryModelDataSource");
			// Insert code required on object creation below this point.
		}
        TariffCategoryModel im;
        private void ViewAll(object sender, MouseButtonEventArgs e)
        {
            //im.TariffCategoryCodeFilter = im.CurrentTariffCategory.TariffCategoryCode;
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
	}
}