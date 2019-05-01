using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.UI;
using Core.Common.UI.DataVirtualization;
using CoreEntities.Client.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SimpleMvvmToolkit;
using WaterNut.QuerySpace.AllocationQS.ViewModels;
using WaterNut.QuerySpace.CoreEntities;
using WaterNut.QuerySpace.CoreEntities.ViewModels;


namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for AsycudaDocuments.xaml
	/// </summary>
	public partial class AsycudaDocuments : UserControl
	{
		public AsycudaDocuments()
		{
			InitializeComponent();
            im = (AsycudaDocumentsModel)FindResource("AsycudaDocumentsModelDataSource");
            // Insert code required on object creation below this point.
        }
        AsycudaDocumentsModel im;

        private void ViewAll(object sender, MouseButtonEventArgs e)
        {
            im.ViewAll();
        }

        private async void Send2Excel(object sender, MouseButtonEventArgs e)
        {
            await AsycudaDocumentsModel.Instance.Send2Excel().ConfigureAwait(false);
        }

        private async void ExportDocument(object sender, MouseButtonEventArgs e)
        {
            await AsycudaDocumentSetsModel.Instance.ExportDocuments().ConfigureAwait(false);
            
        }

	    private async void ImportDocument(object sender, MouseButtonEventArgs e)
        {
            await im.ImportDocuments().ConfigureAwait(false);
        }

        private async void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            await SaveDocument(sender).ConfigureAwait(false);
        }

        private async void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            await SaveDocument(sender).ConfigureAwait(false);
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

        private async void DatePicker_LostFocus(object sender, RoutedEventArgs e)
        {
            await SaveDocument(sender).ConfigureAwait(false);
        }

	    private async Task SaveDocument(object sender)
	    {
	        var c = ((FrameworkElement) sender).DataContext;
	        if (c != null)
	            await im.SaveDocument((((FrameworkElement) sender).DataContext as VirtualListItem<AsycudaDocument>).Data).ConfigureAwait(false);
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
                im.SelectedAsycudaDocuments = new ObservableCollection<AsycudaDocument>(ItemLst.SelectedItems.OfType<VirtualListItem<AsycudaDocument>>()
                                                                .Select(x => x.Data));
            }
            else
            {
                if (selectall == true) return;
                foreach (var itm in e.AddedItems.OfType<VirtualListItem<AsycudaDocument>>())
                {
                    var d = itm.Data;
                    if (d == null) continue;
                    var rd =
                        im.SelectedAsycudaDocuments.FirstOrDefault(
                            x => x != null && x.ASYCUDA_Id == d.ASYCUDA_Id);
                    if (rd == null)
                    {
                        im.SelectedAsycudaDocuments.Add(d);
                    }
                }

                foreach (var itm in e.RemovedItems.OfType<VirtualListItem<AsycudaDocument>>())
                {
                    var d = itm.Data;
                    if (d == null) continue;
                    var rd =
                        im.SelectedAsycudaDocuments.FirstOrDefault(
                            x => x != null && x.ASYCUDA_Id == d.ASYCUDA_Id);
                    if (rd != null)
                    {
                        im.SelectedAsycudaDocuments.Remove(rd);
                    }
                }
            }
            MessageBus.Default.BeginNotify(MessageToken.SelectedAsycudaDocumentsChanged, null, new NotificationEventArgs(MessageToken.SelectedAsycudaDocumentsChanged));
            if (e.AddedItems.Count > 0)
                ((ListBox)sender).ScrollIntoView(e.AddedItems[0]);

        }
        private void DeselectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectall = true;
            ItemLst.SelectedItems.Clear();
            im.SelectedAsycudaDocuments.Clear();
            MessageBus.Default.BeginNotify(MessageToken.SelectedAsycudaDocumentsChanged, null, new NotificationEventArgs(MessageToken.SelectedAsycudaDocumentsChanged));
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