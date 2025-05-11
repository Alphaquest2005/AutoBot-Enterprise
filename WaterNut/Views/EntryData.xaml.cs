using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Core.Common.UI;
using Core.Common.UI.DataVirtualization;
using EntryDataQS.Client.Entities;
using SimpleMvvmToolkit;
using WaterNut.QuerySpace.AdjustmentQS.ViewModels;
using WaterNut.QuerySpace.AllocationQS.ViewModels;
using WaterNut.QuerySpace.EntryDataQS;
using EntryDataModel = WaterNut.QuerySpace.EntryDataQS.ViewModels.EntryDataModel;

namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for PurchaseOrders.xaml
	/// </summary>
	public partial class EntryData : UserControl
	{
		public EntryData()
		{
			InitializeComponent();
            im = (EntryDataModel)FindResource("EntryDataModelDataSource");
			// Insert code required on object creation below this point.
		}
        EntryDataModel im;
        
        private async void RemovePOTxt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            await im.RemoveEntryData((((FrameworkElement)sender).DataContext as VirtualListItem<EntryDataEx>).Data).ConfigureAwait(false);
        }

 
        private async void AddItemtoAdocTxt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var lst = new List<EntryDataEx>();
            foreach (var itm in ItemLst.SelectedItems.OfType<VirtualListItem<EntryDataEx>>())
            {
                if (itm.Data == null)
                {
                    itm.Load();
                }
                lst.Add(itm.Data);
            }
            var res = MessageBox.Show("Do You Want Invoice per Entry?", "Add Invoice To Document",
                MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                var res1 = MessageBox.Show("Do You Want Combine EntryData in Same File?", "Add Invoice To Document",
                    MessageBoxButton.YesNo);
                if (res1 == MessageBoxResult.Yes)
                {

                    await im.AddDocToEntry(lst, true, true).ConfigureAwait(false);
                }
                else
                {
                    await im.AddDocToEntry(lst, true, false).ConfigureAwait(false);
                }
            }
            else
            {
                await im.AddDocToEntry(lst, false, false).ConfigureAwait(false);
            }
            

        }

       
        private async void ImportSales(object sender, MouseButtonEventArgs e)
        {
            await im.SaveCSV("Sales").ConfigureAwait(false);
        }       

        private async void ImportPO(object sender, MouseButtonEventArgs e)
        {
            await im.SaveCSV("PO").ConfigureAwait(false);
        }

	    private async void ImportInv(object sender, MouseButtonEventArgs e)
	    {
	        await im.SaveCSV("INV").ConfigureAwait(false);
        }

        private async void ImportOPS(object sender, MouseButtonEventArgs e)
        {
            await im.SaveCSV("OPS").ConfigureAwait(false);
        }


        private async void RemoveSelected(object sender, MouseButtonEventArgs e)
        {
            
            await
                im.RemoveSelectedEntryData(im.SelectedEntryDataEx.ToList())
                    .ConfigureAwait(false);

          
        }

        private async void Send2Excel(object sender, MouseButtonEventArgs e)
        {
            await im.Send2Excel().ConfigureAwait(false);
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is TextBox textBox)
                    textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
        //TODO: Convert this to commands to make it automatic
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
                im.SelectedEntryDataEx = new ObservableCollection<EntryDataEx>(ItemLst.SelectedItems.OfType<VirtualListItem<EntryDataEx>>()
                                                                .Select(x => x.Data));
            }
            else
            {
                if (selectall == true) return;
                foreach (var itm in e.AddedItems.OfType<VirtualListItem<EntryDataEx>>())
                {
                    var d = itm.Data;
                    if (d == null) continue;
                    var rd =
                        im.SelectedEntryDataEx.FirstOrDefault(
                            x => x != null && x.InvoiceNo == d.InvoiceNo);
                    if (rd == null)
                    {
                        im.SelectedEntryDataEx.Add(d);
                    }
                }

                foreach (var itm in e.RemovedItems.OfType<VirtualListItem<EntryDataEx>>())
                {
                    var d = itm.Data;
                    if (d == null) continue;
                    var rd =
                        im.SelectedEntryDataEx.FirstOrDefault(
                            x => x != null && x.InvoiceNo == d.InvoiceNo);
                    if (rd != null)
                    {
                        im.SelectedEntryDataEx.Remove(rd);
                    }
                }
            }
            MessageBus.Default.BeginNotify(MessageToken.SelectedEntryDataExChanged, null, new NotificationEventArgs(MessageToken.SelectedEntryDataExChanged));
            if (e.AddedItems.Count > 0)
                ((ListBox)sender).ScrollIntoView(e.AddedItems[0]);

        }
        private void DeselectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectall = true;
            ItemLst.SelectedItems.Clear();
            im.SelectedEntryDataEx.Clear();
            MessageBus.Default.BeginNotify(MessageToken.SelectedEntryDataExChanged, null, new NotificationEventArgs(MessageToken.SelectedEntryDataExChanged));
            selectall = false;
        }

	    private async void SelectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	    {
            StatusModel.Timer("Selecting EntryData...");
	        MultiSelectChk.IsChecked = true;
            await im.SelectAll().ConfigureAwait(false);
	        Task t = Task.Run(() =>
	        {
	            this.Dispatcher.BeginInvoke((Action) (() =>
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


	    private async void ImportADJ(object sender, MouseButtonEventArgs e)
	    {
	        await AdjustmentExModel.Instance.Import("ADJ").ConfigureAwait(false);
        }

	    private async void ImportDIS(object sender, MouseButtonEventArgs e)
	    {
	        await AdjustmentExModel.Instance.Import("DIS").ConfigureAwait(false);
	    }


        private async void Import(object sender, MouseButtonEventArgs e)
        {
            await AdjustmentExModel.Instance.Import("Unknown").ConfigureAwait(false);
        }
    }
}