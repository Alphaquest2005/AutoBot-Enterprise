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
	public partial class EntryDataContainersView : UserControl
	{
        public EntryDataContainersView()
		{
			InitializeComponent();
            im = (EntryDataContainersModel)FindResource("EntryDataContainersModelDataSource");
			// Insert code required on object creation below this point.
		}
        EntryDataContainersModel im;





        private async void SaveItm(object sender, MouseButtonEventArgs e)
        {
            await im.SaveContainer(BaseViewModel.Instance.CurrentContainerEx).ConfigureAwait(false);
           
            
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
                im.SelectedContainerExes = new ObservableCollection<ContainerEx>(ItemLst.SelectedItems.OfType<VirtualListItem<ContainerEx>>()
                                                                .Select(x => x.Data));
            }
            else
            {
                if (selectall == true) return;
                foreach (var itm in e.AddedItems.OfType<VirtualListItem<ContainerEx>>())
                {
                    var d = itm.Data;
                    if (d == null) continue;
                    var rd =
                        im.SelectedContainerExes.FirstOrDefault(
                            x => x != null && x.Container_Id == d.Container_Id);
                    if (rd == null)
                    {
                        im.SelectedContainerExes.Add(d);
                    }
                }

                foreach (var itm in e.RemovedItems.OfType<VirtualListItem<ContainerEx>>())
                {
                    var d = itm.Data;
                    if (d == null) continue;
                    var rd =
                        im.SelectedContainerExes.FirstOrDefault(
                            x => x != null && x.Container_Id == d.Container_Id);
                    if (rd != null)
                    {
                        im.SelectedContainerExes.Remove(rd);
                    }
                }
            }
            MessageBus.Default.BeginNotify(MessageToken.SelectedContainerExesChanged, null, new NotificationEventArgs(MessageToken.SelectedContainerExesChanged));
            if (e.AddedItems.Count > 0)
                ((ListBox)sender).ScrollIntoView(e.AddedItems[0]);

        }
        private void DeselectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectall = true;
            ItemLst.SelectedItems.Clear();
            im.SelectedContainerExes.Clear();
            MessageBus.Default.BeginNotify(MessageToken.SelectedContainerExesChanged, null, new NotificationEventArgs(MessageToken.SelectedContainerExesChanged));
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

	    private async void AssignInvoice(object sender, MouseButtonEventArgs e)
	    {
	        await im.AssignInvoies().ConfigureAwait(false);
	    }

	    private void ViewAll(object sender, MouseButtonEventArgs e)
	    {
	        im.ViewAll();
	    }

	    private async void NewContainer(object sender, MouseButtonEventArgs e)
	    {
	        await im.NewContainer(BaseViewModel.Instance.CurrentContainerEx).ConfigureAwait(false);
	    }
	}
}