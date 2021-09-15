using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Core.Common.UI.DataVirtualization;
using OCR.Client.Entities;
using RegexImporter.ViewModels;

namespace RegexImporter.Views
{
	/// <summary>
	/// Interaction logic for InvoiceSummary.xaml
	/// </summary>
	public partial class InvoiceSummary : UserControl
	{
		public InvoiceSummary()
		{
			InitializeComponent();
			im = (InvoiceExViewModel)FindResource("InvoiceModelDataSource");
			// Insert code required on object creation below this point.

		}
        InvoiceExViewModel im;
		


		private void Invoice_Expanded(object sender, RoutedEventArgs e)
		{
			WaterNut.QuerySpace.OCR.ViewModels.BaseViewModel.Instance.CurrentInvoice = (Invoice)((sender as FrameworkElement).DataContext as VirtualListItem<Invoice>).Data;

		}

        private void Part_Expanded(object sender, RoutedEventArgs e)
        {
            WaterNut.QuerySpace.OCR.ViewModels.BaseViewModel.Instance.CurrentPart = (Part)((sender as FrameworkElement).DataContext );

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


		private void Invoice_Collapsed(object sender, RoutedEventArgs e)
		{
			WaterNut.QuerySpace.OCR.ViewModels.BaseViewModel.Instance.CurrentInvoice = null;
			WaterNut.QuerySpace.OCR.ViewModels.BaseViewModel.Instance.CurrentParts = null;
		   
		}

        private void Part_Collapsed(object sender, RoutedEventArgs e)
        {
            WaterNut.QuerySpace.OCR.ViewModels.BaseViewModel.Instance.CurrentPart = null;
            WaterNut.QuerySpace.OCR.ViewModels.BaseViewModel.Instance.CurrentLine = null;

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