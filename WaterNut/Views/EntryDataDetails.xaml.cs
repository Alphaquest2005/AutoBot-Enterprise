using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;
using Core.Common.UI.DataVirtualization;
using EntryDataQS.Client.Repositories;
using Omu.ValueInjecter;
using SimpleMvvmToolkit;
using WaterNut.QuerySpace;
using EntryDataQS.Client.Entities;
using WaterNut.QuerySpace.EntryDataQS;
using WaterNut.QuerySpace.EntryDataQS.ViewModels;


namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for PurchaseOrderDetails.xaml
	/// </summary>
	public partial class EntryDataDetailsView : UserControl
	{
        public EntryDataDetailsView()
		{
			InitializeComponent();
            im = (EntryDataDetailsModel)FindResource("EntryDataDetailsModelDataSource");
			// Insert code required on object creation below this point.
		}
        EntryDataDetailsModel im;



        private async void AddItemtoAdocTxt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            await im.AddToEntry(ItemLst.SelectedItems.OfType<VirtualListItem<EntryDataDetailsEx>>()
                                                                 .Select(x => x.Data).ToList()).ConfigureAwait(false);
        }



        private async void SaveItm(object sender, MouseButtonEventArgs e)
        {
            await im.SaveEntryDataDetailsEx(BaseViewModel.Instance.CurrentEntryDataDetailsEx).ConfigureAwait(false);
           
            
        }

        private async void Send2Excel(object sender, MouseButtonEventArgs e)
        {
            await im.Send2Excel().ConfigureAwait(false);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
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
                im.SelectedEntryDataDetailsExes = new ObservableCollection<EntryDataDetailsEx>(ItemLst.SelectedItems.OfType<VirtualListItem<EntryDataDetailsEx>>()
                                                                .Select(x => x.Data));
            }
            else
            {
                if (selectall == true) return;
                foreach (var itm in e.AddedItems.OfType<VirtualListItem<EntryDataDetailsEx>>())
                {
                    var d = itm.Data;
                    if (d == null) continue;
                    var rd =
                        im.SelectedEntryDataDetailsExes.FirstOrDefault(
                            x => x != null && x.EntryDataDetailsId == d.EntryDataDetailsId);
                    if (rd == null)
                    {
                        im.SelectedEntryDataDetailsExes.Add(d);
                    }
                }

                foreach (var itm in e.RemovedItems.OfType<VirtualListItem<EntryDataDetailsEx>>())
                {
                    var d = itm.Data;
                    if (d == null) continue;
                    var rd =
                        im.SelectedEntryDataDetailsExes.FirstOrDefault(
                            x => x != null && x.EntryDataDetailsId == d.EntryDataDetailsId);
                    if (rd != null)
                    {
                        im.SelectedEntryDataDetailsExes.Remove(rd);
                    }
                }
            }
            MessageBus.Default.BeginNotify(MessageToken.SelectedEntryDataDetailsExesChanged, null, new NotificationEventArgs(MessageToken.SelectedEntryDataDetailsExesChanged));
            if (e.AddedItems.Count > 0)
                ((ListBox)sender).ScrollIntoView(e.AddedItems[0]);

        }
        private void DeselectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectall = true;
            ItemLst.SelectedItems.Clear();
            im.SelectedEntryDataDetailsExes.Clear();
            MessageBus.Default.BeginNotify(MessageToken.SelectedEntryDataDetailsExesChanged, null, new NotificationEventArgs(MessageToken.SelectedEntryDataDetailsExesChanged));
            selectall = false;
        }

        private async void SelectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectall = true;
            MultiSelectChk.IsChecked = true;
            await im.SelectAll().ConfigureAwait(false);
            for (var index = 0; index < ItemLst.Items.Count; index++)
            {
                var item = ItemLst.Items[index];
                ItemLst.SelectedItems.Add(item);
                if (index == ItemLst.Items.Count - 1) selectall = false;
            }
        }
        #endregion

	    private async void NewItm(object sender, MouseButtonEventArgs e)
	    {
	        await im.NewEntryDataDetailEx().ConfigureAwait(false);
	    }
	}
}