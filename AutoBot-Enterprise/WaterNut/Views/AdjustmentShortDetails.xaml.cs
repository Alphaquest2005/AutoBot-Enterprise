using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AdjustmentQS.Client.Entities;
using Core.Common.UI.DataVirtualization;
using WaterNut.QuerySpace.AdjustmentQS.ViewModels;
using WaterNut.QuerySpace.AllocationQS.ViewModels;

//using AdjustmentQS.Client.Entities;
//using WaterNut.QuerySpace.AdjustmentQS.ViewModels;

namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for PreviousItems.xaml
	/// </summary>
    public partial class AdjustmentShortDetailsView : UserControl
	{
        public AdjustmentShortDetailsView()
        {
            this.InitializeComponent();
            im = TryFindResource("AdjustmentDetailsModelDataSource") as AdjustmentShortDetailsModel;
            // Insert code required on object creation below this point.
        }

        private AdjustmentShortDetailsModel im;

        private async void Send2Excel(object sender, MouseButtonEventArgs e)
        {
           await im.Send2Excel().ConfigureAwait(false);
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }

        private void ViewSuggestions(object sender, MouseButtonEventArgs e)
        {
            im.ViewSuggestions(((sender as FrameworkElement).DataContext as VirtualListItem<AdjustmentShort>).Data);
        }

        private async void MatchItem(object sender, MouseButtonEventArgs e)
        {
            await im.MatchToCurrentItem(((sender as FrameworkElement).DataContext as VirtualListItem<AdjustmentShort>).Data).ConfigureAwait(false);
        }

        private async void RemoveMatch(object sender, MouseButtonEventArgs e)
        {
            await im.RemoveOsMatch(((sender as FrameworkElement).DataContext as VirtualListItem<AdjustmentShort>).Data).ConfigureAwait(false);
        }

        private async void RemoveOSDetail(object sender, MouseButtonEventArgs e)
        {
           await im.RemoveEntryDataDetail(((sender as FrameworkElement).DataContext as VirtualListItem<AdjustmentShort>).Data).ConfigureAwait(false);
        }


	    private async void AutoMatch(object sender, MouseButtonEventArgs e)
	    {
	        await im.AutoMatch().ConfigureAwait(false);
        }

	    private async void CreateIM9(object sender, MouseButtonEventArgs e)
	    {
	        await im.CreateIM9().ConfigureAwait(false);
	    }

	    private async void AllocateAdjustments(object sender, MouseButtonEventArgs e)
	    {
	        await AllocationsModel.Instance.AllocateSales(true, false).ConfigureAwait(false);
        }

	    private async void CreateIM4(object sender, MouseButtonEventArgs e)
	    {
	        await im.CreateIM4().ConfigureAwait(false);
        }
	}
}