using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using Core.Common.UI.DataVirtualization;
using InventoryQS.Client.Entities;
using SimpleMvvmToolkit;
using WaterNut.QuerySpace.InventoryQS;
using WaterNut.QuerySpace.InventoryQS.ViewModels;

namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for NullTarifInventoryItems.xaml
	/// </summary>
	public partial class NullTarifInventoryItems : UserControl
	{
		public NullTarifInventoryItems()
		{
			InitializeComponent();
            im = (InventoryItemsModel)FindResource("NullTarifInventoryItemsModelDataSource");
			// Insert code required on object creation below this point.
		}
        InventoryItemsModel im;

        private async void AssignTariffTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(BaseViewModel.Instance.CurrentTariffCodes == null)
            {
                MessageBox.Show("Please Select Tariff before continuing.");
                return;
            }
            var lst = ItemLst.SelectedItems.OfType<VirtualListItem<InventoryItemsEx>>();
            foreach (var result in lst.Where(x => x.Data == null))
            {
                result.Load();
            }
            await im.AssignTariffToItms(lst.Select(x => x.Data.InventoryItemId).ToList()).ConfigureAwait(false);
        }

        private async void Send2Excel(object sender, MouseButtonEventArgs e)
        {
            await im.Send2Excel().ConfigureAwait(false);
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

        private async void SaveOnEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = sender as TextBox;
                if (textBox != null)
                {
                    textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                    await
                        im.SaveInventoryItem(
                            ((sender as FrameworkElement).DataContext as VirtualListItem<InventoryItemsEx>).Data)
                            .ConfigureAwait(false);
                }
            }
        }

        #region "ListBox Selection"
        bool selectall = false;
        private void ItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MultiSelectChk.IsChecked == false)
            {
                for (var i = 0; i < ItemLst.SelectedItems.Count - 1; i++)
                {
                    ItemLst.SelectedItems.RemoveAt(i);
                }
                im.SelectedInventoryItemsEx = new ObservableCollection<InventoryItemsEx>(ItemLst.SelectedItems.OfType<VirtualListItem<InventoryItemsEx>>()
                                                                .Select(x => x.Data));
            }
            else
            {
                if (selectall == true) return;
                foreach (var itm in e.AddedItems.OfType<VirtualListItem<InventoryItemsEx>>())
                {
                    var d = itm.Data;
                    if (d == null) continue;
                    var rd =
                        im.SelectedInventoryItemsEx.FirstOrDefault(
                            x => x != null && x.ItemNumber == d.ItemNumber);
                    if (rd == null)
                    {
                        im.SelectedInventoryItemsEx.Add(d);
                    }
                }

                foreach (var itm in e.RemovedItems.OfType<VirtualListItem<InventoryItemsEx>>())
                {
                    var d = itm.Data;
                    if (d == null) continue;
                    var rd =
                        im.SelectedInventoryItemsEx.FirstOrDefault(
                            x => x != null && x.ItemNumber == d.ItemNumber);
                    if (rd != null)
                    {
                        im.SelectedInventoryItemsEx.Remove(rd);
                    }
                }
            }
            MessageBus.Default.BeginNotify(MessageToken.SelectedInventoryItemsExChanged, null, new NotificationEventArgs(MessageToken.SelectedInventoryItemsExChanged));
            if (e.AddedItems.Count > 0)
                ((ListBox)sender).ScrollIntoView(e.AddedItems[0]);

        }
        private void DeselectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectall = true;
            ItemLst.SelectedItems.Clear();
            im.SelectedInventoryItemsEx.Clear();
            MessageBus.Default.BeginNotify(MessageToken.SelectedInventoryItemsExChanged, null, new NotificationEventArgs(MessageToken.SelectedInventoryItemsExChanged));
            selectall = false;
        }

        private async void SelectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectall = true;
            MultiSelectChk.IsChecked = true;
            await im.SelectAll().ConfigureAwait(true);
            for (var index = 0; index < ItemLst.Items.Count; index++)
            {
                var item = ItemLst.Items[index];
                ItemLst.SelectedItems.Add(item);
                if (index == ItemLst.Items.Count - 1) selectall = false;
            }
        }
        #endregion 

        private void ViewAll(object sender, MouseButtonEventArgs e)
        {
            im.ViewAll();
        }


	}
}