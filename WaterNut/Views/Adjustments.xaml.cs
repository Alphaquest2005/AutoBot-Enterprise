namespace WaterNut.Views;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

using AdjustmentQS.Client.Entities;

using Core.Common.UI;
using Core.Common.UI.DataVirtualization;

using Serilog;

using SimpleMvvmToolkit;

using WaterNut.QuerySpace.AdjustmentQS;
using WaterNut.QuerySpace.AdjustmentQS.ViewModels;

/// <summary>
///     Interaction logic for PreviousItems.xaml
/// </summary>
public partial class AdjustmentsView : UserControl
{
    // void BaseViewModel_staticPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    // {
    // if (e.PropertyName == "CurrentOversShort")
    // {
    // if (im.CurrentOversShort != null)
    // {
    // ItemLst.UpdateLayout();
    // ItemLst.ScrollIntoView(ItemLst.Items[ItemLst.SelectedIndex]);
    // }

    // //        var listBoxItem = (ListBoxItem)ItemLst
    ////.ItemContainerGenerator
    ////.ContainerFromItem(ItemLst.SelectedItem);

    ////        listBoxItem.Focus();

    // }
    // }
    private readonly AdjustmentExModel im;

    private bool selectall;

    public AdjustmentsView()
    {
        this.InitializeComponent();
        this.im = this.TryFindResource("AdjustmentsModelDataSource") as AdjustmentExModel;
    }

    private void AddToEntry(object sender, MouseButtonEventArgs e)
    {
        // await im.CreateOSEntries().ConfigureAwait(false);
    }

    private void AutoMatch(object sender, MouseButtonEventArgs e)
    {
        // await im.AutoMatch().ConfigureAwait(false);
    }

    private void CreateEntries(object sender, MouseButtonEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void DeselectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        this.selectall = true;
        this.ItemLst.SelectedItems.Clear();
        this.im.SelectedAdjustmentExes.Clear();
        MessageBus.Default.BeginNotify(
            MessageToken.SelectedAdjustmentExesChanged,
            null,
            new NotificationEventArgs(MessageToken.SelectedAdjustmentExesChanged));
        this.selectall = false;
    }

    private async void ImportAdjustments(object sender, MouseButtonEventArgs e)
    {
        await this.im.Import("ADJ", Log.Logger).ConfigureAwait(false);
    }

    private void ItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (this.MultiSelectChk.IsChecked == false)
        {
            for (var i = 0; i < this.ItemLst.SelectedItems.Count - 1; i++) this.ItemLst.SelectedItems.RemoveAt(i);

            this.im.SelectedAdjustmentExes = new ObservableCollection<AdjustmentEx>(
                this.ItemLst.SelectedItems.OfType<VirtualListItem<AdjustmentEx>>().Select((VirtualListItem<AdjustmentEx> x) => x.Data));
        }
        else
        {
            if (this.selectall) return;
            foreach (var itm in e.AddedItems.OfType<VirtualListItem<AdjustmentEx>>())
            {
                var d = itm.Data;
                if (d == null) continue;
                var rd = this.im.SelectedAdjustmentExes.FirstOrDefault((AdjustmentEx x) => x != null && x.EntityId == d.EntityId);
                if (rd == null) this.im.SelectedAdjustmentExes.Add(d);
            }

            foreach (var itm in e.RemovedItems.OfType<VirtualListItem<AdjustmentEx>>())
            {
                var d = itm.Data;
                if (d == null) continue;
                var rd = this.im.SelectedAdjustmentExes.FirstOrDefault((AdjustmentEx x) => x != null && x.EntityId == d.EntityId);
                if (rd != null) this.im.SelectedAdjustmentExes.Remove(rd);
            }
        }

        MessageBus.Default.BeginNotify(
            MessageToken.SelectedAdjustmentExesChanged,
            null,
            new NotificationEventArgs(MessageToken.SelectedAdjustmentExesChanged));
        if (e.AddedItems.Count > 0)
            ((ListBox)sender).ScrollIntoView(e.AddedItems[0]);
    }

    private void MatchEntries(object sender, MouseButtonEventArgs e)
    {
        // await im.MatchEntries().ConfigureAwait(false);
    }

    private async void RemoveSelected(object sender, MouseButtonEventArgs e)
    {
        await this.im.RemoveSelectedAdjustment(this.im.SelectedAdjustmentExes.ToList()).ConfigureAwait(false);
    }

    private void SaveCNumber(object sender, MouseButtonEventArgs e)
    {
        // await im.SaveCNumber(CNumberTxt.Text).ConfigureAwait(false);
    }

    private void SaveReferenceNumber(object sender, MouseButtonEventArgs e)
    {
        // await im.SaveReferenceNumber(RefNumTxt.Text).ConfigureAwait(false);
    }

    private async void SelectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        StatusModel.Timer("Selecting EntryData...");
        this.MultiSelectChk.IsChecked = true;
        await this.im.SelectAll().ConfigureAwait(false);
        var t = Task.Run(
            () =>
                {
                    this.Dispatcher.BeginInvoke(
                        (Action)(() =>
                                        {
                                            this.selectall = true;
                                            for (var index = 0; index < this.ItemLst.Items.Count; index++)
                                            {
                                                var item = this.ItemLst.Items[index];
                                                this.ItemLst.SelectedItems.Add(item);
                                                if (index == this.ItemLst.Items.Count - 1) this.selectall = false;
                                            }
                                        }));
                });
        await t.ConfigureAwait(false);
        StatusModel.StopStatusUpdate();
    }

    private void Send2Excel(object sender, MouseButtonEventArgs e)
    {
    }

    private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
    }

    private void ViewAll(object sender, MouseButtonEventArgs e)
    {
        AdjustmentExModel.Instance.ViewAll();
    }

    // TODO: Convert this to commands to make it automatic
}