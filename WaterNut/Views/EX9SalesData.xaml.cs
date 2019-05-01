using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using Core.Common.UI;
using Core.Common.UI.DataVirtualization;
using EntryDataQS.Client.Entities;

using SalesDataQS.Client.Entities;
using SimpleMvvmToolkit;
using WaterNut.QuerySpace.AllocationQS.ViewModels;
using WaterNut.QuerySpace.SalesDataQS;
using Ex9SalesDataModel = WaterNut.QuerySpace.SalesDataQS.ViewModels.Ex9SalesDataModel;

namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for Ex9SalesData.xaml
	/// </summary>
	public partial class Ex9SalesData : UserControl
	{
		public Ex9SalesData()
		{
			InitializeComponent();
            im = (Ex9SalesDataModel)FindResource("Ex9SalesDataModelDataSource");
			// Insert code required on object creation below this point.
		}
        Ex9SalesDataModel im;
        //private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (MultiSelectChk.IsChecked == false)
        //    {
        //        for (int i = 0; i < ItemLst.SelectedItems.Count - 1; i++)
        //        {
        //            ItemLst.SelectedItems.RemoveAt(i);
        //        }

        //    }
        //    if (e.AddedItems.Count > 0)
        //    ((ListBox)sender).ScrollIntoView(e.AddedItems[0]);
        //}

        private async void RemovePOTxt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
           await im.RemoveSalesData(((FrameworkElement)sender).DataContext as SalesData).ConfigureAwait(false);
        }

        //private void SelectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    MultiSelectChk.IsChecked = true;
        //    foreach (var item in ItemLst.Items)
        //    {
        //        ItemLst.SelectedItems.Add(item);
        //    }
        //    im.SelectedSalesDatas = new ObservableCollection<SalesData>(ItemLst.SelectedItems.OfType<SalesData>().ToList());
        //}

        //private void DeselectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    ItemLst.SelectedItems.Clear();
        //    im.SelectedSalesDatas.Clear();
        //}




        private async void ImportSales(object sender, MouseButtonEventArgs e)
        {
            await im.SaveCSV("Sales").ConfigureAwait(false);
        }

     
   

        private async void RemoveSelected(object sender, MouseButtonEventArgs e)
        {
           await im.RemoveSelectedSalesData(ItemLst.SelectedItems.OfType<VirtualListItem<SalesData>>()
                                                                .Select(x => x.Data).ToList()).ConfigureAwait(false);
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
                im.SelectedSalesDatas = new ObservableCollection<SalesData>(ItemLst.SelectedItems.OfType<VirtualListItem<SalesData>>()
                                                                .Select(x => x.Data));
            }
            else
            {
                if (selectall == true) return;
                foreach (var itm in e.AddedItems.OfType<VirtualListItem<SalesData>>())
                {
                    var d = itm.Data;
                    if (d == null) continue;
                    var rd =
                        im.SelectedSalesDatas.FirstOrDefault(
                            x => x != null && x.EntryDataId == d.EntryDataId);
                    if (rd == null)
                    {
                        im.SelectedSalesDatas.Add(d);
                    }
                }

                foreach (var itm in e.RemovedItems.OfType<VirtualListItem<EntryDataEx>>())
                {
                    var d = itm.Data;
                    if (d == null) continue;
                    var rd =
                        im.SelectedSalesDatas.FirstOrDefault(
                            x => x != null && x.EntryDataId == d.InvoiceNo);
                    if (rd != null)
                    {
                        im.SelectedSalesDatas.Remove(rd);
                    }
                }
            }
            MessageBus.Default.BeginNotify(MessageToken.SelectedSalesDatasChanged, null, new NotificationEventArgs(MessageToken.SelectedSalesDatasChanged));
            if (e.AddedItems.Count > 0)
                ((ListBox)sender).ScrollIntoView(e.AddedItems[0]);

        }
        private void DeselectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectall = true;
            ItemLst.SelectedItems.Clear();
            im.SelectedSalesDatas.Clear();
            MessageBus.Default.BeginNotify(MessageToken.SelectedSalesDatasChanged, null, new NotificationEventArgs(MessageToken.SelectedSalesDatasChanged));
            selectall = false;
        }

        private async void SelectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StatusModel.Timer("Selecting SalesData...");
            MultiSelectChk.IsChecked = true;
            await im.SelectAll().ConfigureAwait(false);
            Task t = Task.Run(() =>
            {
                this.Dispatcher.BeginInvoke((Action)(() =>
                {
                    selectall = true;
                    for (var index = 0; index < ItemLst.Items.Count; index++)
                    {
                        var item = ItemLst.Items[index];
                        ItemLst.SelectedItems.Add(item);
                        if (index == ItemLst.Items.Count - 1) selectall = false;
                    }
                }));
            });
            await t.ConfigureAwait(false);
            StatusModel.StopStatusUpdate();
        }
        #endregion 

        private void ViewAll(object sender, MouseButtonEventArgs e)
        {
            im.ViewAll();
        }

	}
}