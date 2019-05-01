using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AdjustmentQS.Client.Entities;
using Core.Common.UI.DataVirtualization;
using WaterNut.QuerySpace.AdjustmentQS.ViewModels;

//using AdjustmentQS.Client.Entities;
//using WaterNut.QuerySpace.AdjustmentQS.ViewModels;

namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for PreviousItems.xaml
	/// </summary>
    public partial class AdjustmentOverDetailsView : UserControl
	{
        public AdjustmentOverDetailsView()
        {
            this.InitializeComponent();
            im = TryFindResource("AdjustmentDetailsModelDataSource") as AdjustmentOverDetailsModel;
            // Insert code required on object creation below this point.
        }

        private AdjustmentOverDetailsModel im;

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

        private void ViewSuggestions(object sender, MouseButtonEventArgs e)
        {
            im.ViewSuggestions(((sender as FrameworkElement).DataContext as VirtualListItem<AdjustmentOver>).Data);
        }

        private async void MatchItem(object sender, MouseButtonEventArgs e)
        {
            await im.MatchToCurrentItem(((sender as FrameworkElement).DataContext as VirtualListItem<AdjustmentOver>).Data).ConfigureAwait(false);
        }

        private async void RemoveMatch(object sender, MouseButtonEventArgs e)
        {
            await im.RemoveOsMatch(((sender as FrameworkElement).DataContext as VirtualListItem<AdjustmentOver>).Data).ConfigureAwait(false);
        }

        private async void RemoveOSDetail(object sender, MouseButtonEventArgs e)
        {
           await im.RemoveEntryDataDetail(((sender as FrameworkElement).DataContext as VirtualListItem<AdjustmentOver>).Data).ConfigureAwait(false);
        }

        private void ViewErrorChk_Copy2_Checked(object sender, RoutedEventArgs e)
        {

        }

	    private async void CreateOPS(object sender, MouseButtonEventArgs e)
	    {
	        await im.CreateOPS().ConfigureAwait(false);
	    }
	}
}