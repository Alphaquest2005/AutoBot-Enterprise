using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using Core.Common.UI.DataVirtualization;

using SalesDataQS.Client.Entities;
using WaterNut.QuerySpace.SalesDataQS.ViewModels;



namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for Ex9SalesDataDetails.xaml
	/// </summary>
	public partial class Ex9SalesDataDetails : UserControl
	{
		public Ex9SalesDataDetails()
		{
            InitializeComponent();
            im = (EX9SalesDataDetailsModel)FindResource("Ex9SalesDataDetailsModelDataSource");
			// Insert code required on object creation below this point.
		}
        EX9SalesDataDetailsModel im;
        private void ItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MultiSelectChk.IsChecked == false)
            {
                for (var i = 0; i < ItemLst.SelectedItems.Count - 1; i++)
                {
                    ItemLst.SelectedItems.RemoveAt(i);
                }
                im.SelectedSalesDataDetails = new ObservableCollection<SalesDataDetail>(ItemLst.SelectedItems.OfType<VirtualListItem<SalesDataDetail>>()
                                                                .Select(x => x.Data).ToList());
            }
            if (e.AddedItems.Count > 0)
            ((ListBox)sender).ScrollIntoView(e.AddedItems[0]);

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

       

        private async void Send2Excel(object sender, MouseButtonEventArgs e)
        {
            await im.Send2Excel().ConfigureAwait(false);
        }

        private async void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            await im.SaveEx9SalesDetail((((FrameworkElement)sender).DataContext as VirtualListItem<SalesDataDetail>).Data).ConfigureAwait(false);
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is TextBox textBox)
                    textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }    
        }
	
	}
}