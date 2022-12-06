using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using AllocationQS.Client.Entities;
using Core.Common.UI.DataVirtualization;
using PreviousDocumentQS.Client.Entities;
using WaterNut.QuerySpace.AllocationQS.ViewModels;
using AsycudaSalesAllocationsEx = AllocationQS.Client.Entities.AsycudaSalesAllocationsEx;
using BaseViewModel = WaterNut.QuerySpace.CoreEntities.ViewModels.BaseViewModel;

namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for Allocations.xaml
	/// </summary>
	public partial class Allocations : UserControl
	{
		public Allocations()
		{
			InitializeComponent();
            im = (AllocationsModel)FindResource("AllocationsModelDataSource");
			// Insert code required on object creation below this point.
		}



        AllocationsModel im;
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            ((ListBox)sender).ScrollIntoView(e.AddedItems[0]);
            im.SelectedAsycudaSalesAndAdjustmentAllocationsExes = new ObservableCollection<AsycudaSalesAndAdjustmentAllocationsEx>(ItemLst.SelectedItems.OfType<AsycudaSalesAndAdjustmentAllocationsEx>());
        }

        private void ViewAll(object sender, MouseButtonEventArgs e)
        {
           // im.ItemDescriptionFilter = "please change filter to view data";
            im.ViewAll();
        }

        private async void Export(object sender, MouseButtonEventArgs e)
        {
            if (BaseViewModel.Instance.CurrentAsycudaDocumentSetEx == null)
            {
                MessageBox.Show("Please Select a Asycuda Document Set before proceeding");
                return;
            }
            await im.CreateEx9().ConfigureAwait(false);
         
        }

        private async void CreateOPS(object sender, MouseButtonEventArgs e)
        {
            await im.CreateOPS().ConfigureAwait(false);
        }

        private async void CreateErrorOPS(object sender, MouseButtonEventArgs e)
        {
            await im.CreateErrorOPS().ConfigureAwait(false);
        }

        private async void GoToEntryTxt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            await AllocationsModel.Instance.GoToxBondEntry(((VirtualListItem<AsycudaSalesAndAdjustmentAllocationsEx>)((FrameworkElement)sender).DataContext).Data.xBond_Item_Id.ToString()).ConfigureAwait(false);
        }

        private void ItemLst_Drop(object sender, DragEventArgs e)
        {
            
        }

        private void ItemLst_DragEnter(object sender, DragEventArgs e)
        {
           
        }

        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("xcuda_Item_Format"))
            {
                var previousEntry = e.Data.GetData("xcuda_Item_Format") as PreviousDocumentItem;
                var callo = ((VirtualListItem<AsycudaSalesAndAdjustmentAllocationsEx>)((FrameworkElement)sender).DataContext).Data;
                await im.ManuallyAllocate(callo,previousEntry).ConfigureAwait(false);

            }
        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
 if (!e.Data.GetDataPresent("xcuda_Item_Format") ||  sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private async void CreateIncompOPS(object sender, MouseButtonEventArgs e)
        {
            await im.CreateIncompOPS().ConfigureAwait(false);
        }

        private async void Send2Excel(object sender, MouseButtonEventArgs e)
        {
            await AllocationsModel.Instance.Send2Excel().ConfigureAwait(false);
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }

        private async void RebuildSales(object sender, MouseButtonEventArgs e)
        {
               await im.ReBuildSalesReports().ConfigureAwait(false);
        }

        private void MultiSelectChk_Copy8_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}