using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Core.Common.UI.DataVirtualization;
using OCR.Client.Entities;
using RegexImporter.ViewModels;
using WaterNut.QuerySpace.CoreEntities;

namespace RegexImporter.Views
{
    /// <summary>
    /// Interaction logic for TxtViewer.xaml
    /// </summary>
    public partial class TxtViewer : UserControl
	{
		public TxtViewer()
		{
			InitializeComponent();
			im = (TXTViewerViewModel)FindResource("FileDetailsViewModelDataSource");
            // Insert code required on object creation below this point.


        }

     

        TXTViewerViewModel im;
		


		
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