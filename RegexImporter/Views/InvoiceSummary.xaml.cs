using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Core.Common.UI.DataVirtualization;
using CoreEntities.Client.Entities;
using OCR.Client.Entities;
using RegexImporter.ViewModels;
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
			im = (InvoiceExViewModel)FindResource("IDataSource");
			// Insert code required on object creation below this point.

		}
        InvoiceExViewModel im;
		


		private void Expander_Expanded(object sender, RoutedEventArgs e)
		{
			QuerySpace.OCR.ViewModels.BaseViewModel.Instance.CurrentInvoice = (Invoice)((sender as FrameworkElement).DataContext as VirtualListItem<Invoice>).Data;

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


		private void Expander_Collapsed(object sender, RoutedEventArgs e)
		{
			QuerySpace.OCR.ViewModels.BaseViewModel.Instance.CurrentInvoice = null;
			QuerySpace.OCR.ViewModels.BaseViewModel.Instance.CurrentParts = null;
		   
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