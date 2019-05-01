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
	/// Interaction logic for TariffCodes.xaml
	/// </summary>
	public partial class TariffCodes : UserControl
	{
		public TariffCodes()
		{
			InitializeComponent();
            im = (TariffCodesModel)FindResource("TariffCodesModelDataSource");
			// Insert code required on object creation below this point.
		}
        TariffCodesModel im;
        private void ViewAll(object sender, MouseButtonEventArgs e)
        {
           im.ViewAll();
        }

        private async void AssignTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            await im.AssignTariffCodeToItm().ConfigureAwait(false);
        }

        private void ParentTariffCodeTxt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        	
        }

        private void ParentTarifCodeTxt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            BaseViewModel.Instance.CurrentTariffCategory = BaseViewModel.Instance.CurrentTariffCodes.TariffCategory;            
        }

        private void ItemsTxt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
           // BaseViewModel.OnStaticPropertyChanged("TariffCodeInventoryItemsLink");
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

	    private async void ValidateTariffCodes(object sender, MouseButtonEventArgs e)
	    {
	        await im.ValidateExistingTariffCodes().ConfigureAwait(false);
	    }
	}
}