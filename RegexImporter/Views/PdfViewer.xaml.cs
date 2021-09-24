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
    /// Interaction logic for PdfViewer.xaml
    /// </summary>
    public partial class PdfViewer : UserControl
	{
		public PdfViewer()
		{
			InitializeComponent();
			im = (PDFViewerViewModel)FindResource("FileDetailsViewModelDataSource");
            // Insert code required on object creation below this point.

            pdfWebViewer.Navigate(new Uri("about:blank"));
            im.PropertyChanged += ImOnPropertyChanged;

        }

        private void ImOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentAttachment")
            {
                pdfWebViewer.Navigate(im.CurrentAttachment.Attachments.FilePath);
            }
        }

        PDFViewerViewModel im;
		


		
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

        private void ExtractTxt(object sender, MouseButtonEventArgs e)
        {
            im.ExtractTxt();
        }
    }
}