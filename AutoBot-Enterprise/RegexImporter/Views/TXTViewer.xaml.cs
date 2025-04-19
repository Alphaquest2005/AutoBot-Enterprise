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
            im.PropertyChanged += onPropertyChanged;


        }

        private void onPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(im.CurrentTextSelection))
            {
                if (im.CurrentTextSelection == null) return;
                txtViewer.Focus();
                txtViewer.Select(im.CurrentTextSelection.Index, im.CurrentTextSelection.Length);
                txtViewer.ScrollToLine(txtViewer.GetLineIndexFromCharacterIndex(txtViewer.SelectionStart));
            }
        }


        TXTViewerViewModel im;
		


		
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
                if (sender is TextBox textBox)
					textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
			}
		}

		private void DatePicker_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
		{
            if (sender is DatePicker datePicker)
					datePicker.GetBindingExpression(DatePicker.SelectedDateProperty).UpdateSource();
		  
		}

	}
}