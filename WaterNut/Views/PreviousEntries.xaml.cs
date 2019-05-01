using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;
using System.Data.Entity;
using System.Collections.ObjectModel;
using Core.Common.UI.DataVirtualization;
using WaterNut.QuerySpace;
using PreviousDocumentQS.Client.Entities;
using EntryDataQS.Client.Entities;
using WaterNut.QuerySpace.PreviousDocumentQS.ViewModels;


namespace WaterNut.Views
{
    /// <summary>
    /// Interaction logic for PreviousEntries.xaml
    /// </summary>
    public partial class PreviousEntries : UserControl
    {
        public PreviousEntries()
        {
            InitializeComponent();
            im = (PreviousDocumentItemsModel)FindResource("PreviousEntriesModelDataSource");
            // Insert code required on object creation below this point.
        }
        PreviousDocumentItemsModel im;
        private async void RemoveTxt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            await im.RemoveItem(((sender as FrameworkElement).DataContext as PreviousDocumentItem).Item_Id).ConfigureAwait(false);
        }

        private async void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           await im.RemoveSelectedItems(ItemLst.SelectedItems.OfType<VirtualListItem<PreviousDocumentItem>>()
                                                                .Select(x => x.Data).ToList()).ConfigureAwait(false);

        }
        private void DeselectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ItemLst.SelectedItems.Clear();
        }

        private void SelectTxt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MultiSelectChk.IsChecked = true;
            foreach (var item in ItemLst.Items)
            {
                ItemLst.SelectedItems.Add(item);
            }

        }

        private void ItemLst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
         
            if (MultiSelectChk.IsChecked == false || ManualModeChk.IsChecked == true)
            {
                for (var i = 0; i < ItemLst.SelectedItems.Count - 1; i++)
                {
                    ItemLst.SelectedItems.RemoveAt(i);
                }
                if (ManualModeChk.IsChecked != true)
                    im.SelectedPreviousDocumentItems = new ObservableCollection<PreviousDocumentItem>(ItemLst.SelectedItems.OfType<VirtualListItem<PreviousDocumentItem>>().Select(x => x.Data as PreviousDocumentItem));
            }
            if (e.AddedItems.Count > 0)
                ((ListBox)sender).ScrollIntoView(e.AddedItems[0]);
        }

        private void ViewAll(object sender, MouseButtonEventArgs e)
        {
            im.ViewAll();

        }


        Point startPoint;
        bool dragdropstart = false;
        private void ItemLst_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (im.ManualMode == false) return;
            
            startPoint = e.GetPosition(null);
            dragdropstart = true;
          
        }

        private void ItemLst_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (im.ManualMode == false) return;
            if (dragdropstart != true) return;
            var mousePos = e.GetPosition(null);
            var diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                // Get the dragged ListViewItem
                var listView = sender as ListBox;
                var listViewItem =
                    FindAnchestor<ListBoxItem>((DependencyObject)e.OriginalSource);

                // Find the data behind the ListViewItem
                if (listViewItem != null)
                {
                    try
                    {

                        var previousEntry = ((VirtualListItem<PreviousDocumentItem>)listView.ItemContainerGenerator.ItemFromContainer(listViewItem)).Data;

                        // Initialize the drag & drop operation
                        var dragData = new DataObject("xcuda_Item_Format", previousEntry);
                        DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
                    }
                    catch (Exception Ex)
                    {
                    }
                }
                dragdropstart = false;
            }
        }

        private static T FindAnchestor<T>(DependencyObject current)
                                                                   where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        private async void Send2Excel(object sender, MouseButtonEventArgs e)
        {
            await im.Send2Excel().ConfigureAwait(false);
        }

        private async void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            await im.SavePreviousDocumentItem(((FrameworkElement)sender).DataContext as PreviousDocumentItem).ConfigureAwait(false);   
        }

        private async void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            await im.SavePreviousDocumentItem(((FrameworkElement)sender).DataContext as PreviousDocumentItem).ConfigureAwait(false);   
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

      

    }


}

