using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Core.Common.UI;
using Core.Common.UI.DataVirtualization;
using AdjustmentQS.Client.Entities;
using SimpleMvvmToolkit;
using WaterNut.QuerySpace.AdjustmentQS;
using WaterNut.QuerySpace.AdjustmentQS.ViewModels;


namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for PreviousItems.xaml
	/// </summary>
	public partial class AdjustmentsView : UserControl
	{
        public AdjustmentsView()
		{
			this.InitializeComponent();
            im = TryFindResource("AdjustmentsModelDataSource") as AdjustmentExModel;
            
		}

        //void BaseViewModel_staticPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "CurrentOversShort")
        //    {
        //        if (im.CurrentOversShort != null)
        //        {
        //            ItemLst.UpdateLayout();
        //            ItemLst.ScrollIntoView(ItemLst.Items[ItemLst.SelectedIndex]);
        //        }
                
        //        //        var listBoxItem = (ListBoxItem)ItemLst
        ////.ItemContainerGenerator
        ////.ContainerFromItem(ItemLst.SelectedItem);

        ////        listBoxItem.Focus();

        //    }
        //}

	    private AdjustmentExModel im;
        private void ViewAll(object sender, MouseButtonEventArgs e)
        {
            AdjustmentExModel.Instance.ViewAll();
        }

        private void Send2Excel(object sender, MouseButtonEventArgs e)
        {

        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }

        private async void ImportAdjustments(object sender, MouseButtonEventArgs e)
        {
            
           await im.Import("Adjustments").ConfigureAwait(false);
        }

        private async void MatchEntries(object sender, MouseButtonEventArgs e)
        {
          //  await im.MatchEntries().ConfigureAwait(false);
        }

        private async void SaveReferenceNumber(object sender, MouseButtonEventArgs e)
        {
//await im.SaveReferenceNumber(RefNumTxt.Text).ConfigureAwait(false);
        }

        private async void RemoveSelected(object sender, MouseButtonEventArgs e)
        {
            await im.RemoveSelectedAdjustment(im.SelectedAdjustmentExes.ToList()).ConfigureAwait(false);
        }


        private async void AutoMatch(object sender, MouseButtonEventArgs e)
        {
           // await im.AutoMatch().ConfigureAwait(false);
        }

        private async void SaveCNumber(object sender, MouseButtonEventArgs e)
        {
          //  await im.SaveCNumber(CNumberTxt.Text).ConfigureAwait(false);
        }

        private async void AddToEntry(object sender, MouseButtonEventArgs e)
        {
          //  await im.CreateOSEntries().ConfigureAwait(false);
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
                im.SelectedAdjustmentExes = new ObservableCollection<AdjustmentEx>(ItemLst.SelectedItems.OfType<VirtualListItem<AdjustmentEx>>()
                                                                .Select(x => x.Data));
            }
            else
            {
                if (selectall == true) return;
                foreach (var itm in e.AddedItems.OfType<VirtualListItem<AdjustmentEx>>())
                {
                    var d = itm.Data;
                    if (d == null) continue;
                    var rd =
                        im.SelectedAdjustmentExes.FirstOrDefault(
                            x => x != null && x.EntityId == d.EntityId);
                    if (rd == null)
                    {
                        im.SelectedAdjustmentExes.Add(d);
                    }
                }

                foreach (var itm in e.RemovedItems.OfType<VirtualListItem<AdjustmentEx>>())
                {
                    var d = itm.Data;
                    if (d == null) continue;
                    var rd =
                        im.SelectedAdjustmentExes.FirstOrDefault(
                            x => x != null && x.EntityId == d.EntityId);
                    if (rd != null)
                    {
                        im.SelectedAdjustmentExes.Remove(rd);
                    }
                }
            }
            MessageBus.Default.BeginNotify(MessageToken.SelectedAdjustmentExesChanged, null, new NotificationEventArgs(MessageToken.SelectedAdjustmentExesChanged));
            if (e.AddedItems.Count > 0)
                ((ListBox)sender).ScrollIntoView(e.AddedItems[0]);

        }
        private void DeselectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectall = true;
            ItemLst.SelectedItems.Clear();
            im.SelectedAdjustmentExes.Clear();
            MessageBus.Default.BeginNotify(MessageToken.SelectedAdjustmentExesChanged, null, new NotificationEventArgs(MessageToken.SelectedAdjustmentExesChanged));
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


	    private void CreateEntries(object sender, MouseButtonEventArgs e)
	    {
	        throw new NotImplementedException();
	    }
	}
}