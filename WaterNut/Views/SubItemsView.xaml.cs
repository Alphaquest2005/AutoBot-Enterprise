using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Core.Common.UI;
using Core.Common.UI.DataVirtualization;
using CoreEntities.Client.Entities;
using SimpleMvvmToolkit;
using WaterNut.QuerySpace.CoreEntities;
using WaterNut.QuerySpace.CoreEntities.ViewModels;

namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for SubItems.xaml
	/// </summary>
	public partial class SubItemsView : UserControl
	{
		public SubItemsView()
		{
			InitializeComponent();
		    im = (SubItemsModelQS)FindResource("SubItemsModelDataSource");
		    // Insert code required on object creation below this point.
		}

	    private SubItemsModelQS im;
        private void Send2Excel(object sender, MouseButtonEventArgs e)
        {

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

        private async void ImportSI(object sender, MouseButtonEventArgs e)
        {
            await im.SaveCSV("SI").ConfigureAwait(false);
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
                im.SelectedSubItems = new ObservableCollection<SubItems>(ItemLst.SelectedItems.OfType<VirtualListItem<SubItems>>()
                                                                .Select(x => x.Data));
            }
            else
            {
                if (selectall == true) return;
                foreach (var itm in e.AddedItems.OfType<VirtualListItem<SubItems>>())
                {
                    var d = itm.Data;
                    if (d == null) continue;
                    var rd =
                        im.SelectedSubItems.FirstOrDefault(
                            x => x != null && x.SubItem_Id == d.SubItem_Id);
                    if (rd == null)
                    {
                        im.SelectedSubItems.Add(d);
                    }
                }

                foreach (var itm in e.RemovedItems.OfType<VirtualListItem<SubItems>>())
                {
                    var d = itm.Data;
                    if (d == null) continue;
                    var rd =
                        im.SelectedSubItems.FirstOrDefault(
                            x => x != null && x.SubItem_Id == d.SubItem_Id);
                    if (rd != null)
                    {
                        im.SelectedSubItems.Remove(rd);
                    }
                }
            }
            MessageBus.Default.BeginNotify(MessageToken.SelectedSubItemsChanged, null, new NotificationEventArgs(MessageToken.SelectedSubItemsChanged));
            if (e.AddedItems.Count > 0)
                ((ListBox)sender).ScrollIntoView(e.AddedItems[0]);

        }
        private void DeselectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectall = true;
            ItemLst.SelectedItems.Clear();
            im.SelectedSubItems.Clear();
            MessageBus.Default.BeginNotify(MessageToken.SelectedSubItemsChanged, null, new NotificationEventArgs(MessageToken.SelectedSubItemsChanged));
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