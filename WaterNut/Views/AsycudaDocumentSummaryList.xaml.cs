using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Core.Common.UI.DataVirtualization;
using CoreEntities.Client.Entities;
using WaterNut.QuerySpace.AllocationQS.ViewModels;
using WaterNut.QuerySpace.CoreEntities.ViewModels;



namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for AsycudaDocumentSummaryList.xaml
	/// </summary>
	public partial class AsycudaDocumentSummaryList : UserControl
	{
		public AsycudaDocumentSummaryList()
		{
			InitializeComponent();
			im = (AsycudaDocumentSetsModel)FindResource("AsycudaDocumentSetsModelDataSource");
			// Insert code required on object creation below this point.
		}
		AsycudaDocumentSetsModel im;
		
		private async void ImportBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			
			await im.ImportDocuments(im.ImportOnlyRegisteredDocuments, im.ImportTariffCodes, im.NoMessages, im.OverwriteExisting, im.LinkPi).ConfigureAwait(false);
		}

		private async void ExportBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			await im.ExportDocuments().ConfigureAwait(false);
		}

		private void Expander_Expanded(object sender, RoutedEventArgs e)
		{
			QuerySpace.CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx = (AsycudaDocumentSetEx)((sender as FrameworkElement).DataContext as VirtualListItem<AsycudaDocumentSetEx>).Data;

		}

		private async void DeleteAll(object sender, MouseButtonEventArgs e)
		{
			var frameworkElement = sender as FrameworkElement;
			if (frameworkElement == null) return;
			var asycudaDocumentSet = frameworkElement.DataContext as VirtualListItem<AsycudaDocumentSetEx>;
			if (asycudaDocumentSet != null)
				await im.DeleteDocuments(asycudaDocumentSet.Data.AsycudaDocumentSetId).ConfigureAwait(false);
			// BaseViewModel.Instance.CurrentAsycudaDocumentSet.Clear();
		}

		private void DocSetGrd_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0)
				((ListBox)sender).ScrollIntoView(e.AddedItems[0]);
		}

		private void DocGrd_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0)
				((ListBox)sender).ScrollIntoView(e.AddedItems[0]);
		}

		private void ViewAll(object sender, MouseButtonEventArgs e)
		{
			im.ViewAll();
		}

		private async void ExportAll(object sender, MouseButtonEventArgs e)
		{
			var frameworkElement = sender as FrameworkElement;
			if (frameworkElement == null) return;
			var asycudaDocumentSet = (frameworkElement.DataContext) as VirtualListItem<AsycudaDocumentSetEx>;
			if (asycudaDocumentSet != null)
				await im.ExportDocSet(asycudaDocumentSet.Data as AsycudaDocumentSetEx).ConfigureAwait(false);
		    
            //  BaseViewModel.Instance.CurrentAsycudaDocumentSet.ExportDocSet();
        }

		private void Expander_Collapsed(object sender, RoutedEventArgs e)
		{
			QuerySpace.CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx = null;
			QuerySpace.CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocument = null;
		   
		}

		private async void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			await im.SaveDocument((sender as FrameworkElement).DataContext as AsycudaDocument).ConfigureAwait(false);
		}

		private async void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
			await im.SaveDocument((sender as FrameworkElement).DataContext as AsycudaDocument).ConfigureAwait(false);
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

		private void DatePicker_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
		   
				var datePicker = sender as DatePicker;
				if (datePicker != null)
					datePicker.GetBindingExpression(DatePicker.SelectedDateProperty).UpdateSource();
		  
		}

	}
}