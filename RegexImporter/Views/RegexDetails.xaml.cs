using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Core.Common.UI.DataVirtualization;
using OCR.Client.Entities;
using RegexImporter.ViewModels;

namespace RegexImporter.Views
{
    /// <summary>
    /// Interaction logic for RegexDetails.xaml
    /// </summary>
    public partial class RegexDetails : UserControl
	{
		public RegexDetails()
		{
			InitializeComponent();
			im = (RegexViewModel)FindResource("RegexViewModelDataSource");
			// Insert code required on object creation below this point.

		}
        RegexViewModel im;
		


		
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