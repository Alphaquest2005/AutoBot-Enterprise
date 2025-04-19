using InventoryQS.Client.Entities;
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
	/// Interaction logic for TariffSuppUnit.xaml
	/// </summary>
	public partial class TariffSuppUnit : UserControl
	{
		public TariffSuppUnit()
		{
			InitializeComponent();
            im = (TariffSuppUnitModel)FindResource("TariffSuppUnitModelDataSource");
			// Insert code required on object creation below this point.
		}
        TariffSuppUnitModel im;
        private void TextBlock_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            im.ViewAll();
        }

        private async void DataGrid_RowEditEnding_1(object sender, DataGridRowEditEndingEventArgs e)
        {

            //if (((TariffSupUnitLkps)e.Row.Item).TariffCategoryCode != null)
            //{

            //   await im.SaveTariffSupUnitLkps((sender as FrameworkElement).DataContext as TariffSupUnitLkps).ConfigureAwait(false);
            //}
        }

        private void DataGrid_InitializingNewItem_1(object sender, InitializingNewItemEventArgs e)
        {
            //if (BaseViewModel.Instance.CurrentTariffCategory != null)
            //{
            //    ((TariffSupUnitLkps)(e.NewItem)).TariffCategoryCode = BaseViewModel.Instance.CurrentTariffCategory.TariffCategoryCode;
            //}
            //else
            //{
            //    MessageBox.Show("Please select a tariff Category Code");
                
            //}
        }

        private void SuppGrd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            ((ListBox)sender).ScrollIntoView(e.AddedItems[0]);
        }
	}
}