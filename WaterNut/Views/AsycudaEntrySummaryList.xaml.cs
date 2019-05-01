using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using Core.Common.UI;
using Core.Common.UI.DataVirtualization;
using CoreEntities.Client.Entities;
using SimpleMvvmToolkit;
using WaterNut.QuerySpace.CoreEntities;
using WaterNut.QuerySpace.CoreEntities.ViewModels;


namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for AsycudaEntrySummaryList.xaml
	/// </summary>
	public partial class AsycudaEntrySummaryList : UserControl
	{
		public AsycudaEntrySummaryList()
		{
			InitializeComponent();
            im = (AsycudaDocumentItemsModel)FindResource("AsycudaDocumentItemsModelDataSource");
			// Insert code required on object creation below this point.
		}
        AsycudaDocumentItemsModel im;

        private void ItemLst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MultiSelectChk.IsChecked == false)
            {
                for (var i = 0; i < ItemLst.SelectedItems.Count - 1; i++)
                {
                    ItemLst.SelectedItems.RemoveAt(i);
                }
                im.SelectedAsycudaDocumentItems = new ObservableCollection<AsycudaDocumentItem>(ItemLst.SelectedItems.OfType<VirtualListItem<AsycudaDocumentItem>>()
                                                                .Select(x => x.Data).ToList());
            }
            if (e.AddedItems.Count > 0)
                ((ListBox)sender).ScrollIntoView(e.AddedItems[0]);
        }

        //private async void RemoveTxt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    await im.RemoveItem().ConfigureAwait(false);
        //}

        private async void RemoveTxt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            foreach (var d in ItemLst.SelectedItems.OfType<VirtualListItem<AsycudaDocumentItem>>().Where(x => x.Data == null))
            {
                d.Load();
            }
           await im.RemoveSelectedItems(ItemLst.SelectedItems.OfType<VirtualListItem<AsycudaDocumentItem>>()
                                                                .Select(x => x.Data).ToList()).ConfigureAwait(false);
            
        }
        //private void DeselectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    ItemLst.SelectedItems.Clear();
        //}

        //private void SelectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    MultiSelectChk.IsChecked = true;
        //    foreach (var item in ItemLst.Items)
        //    {
        //        ItemLst.SelectedItems.Add(item);
        //    }
           
        //}

        private void ViewAll(object sender, MouseButtonEventArgs e)
        {
            im.ViewAll();
        }

        private async void Send2Excel(object sender, MouseButtonEventArgs e)
        {
            await AsycudaDocumentItemsModel.Instance.Send2Excel().ConfigureAwait(false);
        }

        private async void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var c = ((FrameworkElement) sender).DataContext;
            if (c != null)
            {
                var v = c as VirtualListItem<AsycudaDocumentItem>;
                if (v != null)
                {
                    var vi = v.Data;
                    if(vi != null)
                    await
                        im.SaveDocumentItem(vi).ConfigureAwait(false);
                }
            }
        }

        private async void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var c = ((FrameworkElement)sender).DataContext;
            if (c != null)
            await im.SaveDocumentItem((((FrameworkElement)sender).DataContext as VirtualListItem<AsycudaDocumentItem>).Data).ConfigureAwait(false);
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
                im.SelectedAsycudaDocumentItems = new ObservableCollection<AsycudaDocumentItem>(ItemLst.SelectedItems.OfType<VirtualListItem<AsycudaDocumentItem>>()
                                                                .Select(x => x.Data));
            }
            else
            {
                if (selectall == true) return;
                foreach (var itm in e.AddedItems.OfType<VirtualListItem<AsycudaDocumentItem>>())
                {
                    var d = itm.Data;
                    if (d == null) continue;
                    var rd =
                        im.SelectedAsycudaDocumentItems.FirstOrDefault(
                            x => x != null && x.Item_Id == d.Item_Id);
                    if (rd == null)
                    {
                        im.SelectedAsycudaDocumentItems.Add(d);
                    }
                }

                foreach (var itm in e.RemovedItems.OfType<VirtualListItem<AsycudaDocumentItem>>())
                {
                    var d = itm.Data;
                    if (d == null) continue;
                    var rd =
                        im.SelectedAsycudaDocumentItems.FirstOrDefault(
                            x => x != null && x.Item_Id == d.Item_Id);
                    if (rd != null)
                    {
                        im.SelectedAsycudaDocumentItems.Remove(rd);
                    }
                }
            }
            MessageBus.Default.BeginNotify(MessageToken.SelectedAsycudaDocumentItemsChanged, null, new NotificationEventArgs(MessageToken.SelectedAsycudaDocumentItemsChanged));
            if (e.AddedItems.Count > 0)
                ((ListBox)sender).ScrollIntoView(e.AddedItems[0]);

        }
        private void DeselectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectall = true;
            ItemLst.SelectedItems.Clear();
            im.SelectedAsycudaDocumentItems.Clear();
            MessageBus.Default.BeginNotify(MessageToken.SelectedAsycudaDocumentItemsChanged, null, new NotificationEventArgs(MessageToken.SelectedAsycudaDocumentItemsChanged));
            selectall = false;
        }

        private async void SelectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StatusModel.Timer("Selecting EntryData...");
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
	}
}