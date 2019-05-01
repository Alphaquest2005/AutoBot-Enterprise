using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;
using System.Data.Entity;
using WaterNut.QuerySpace;
using WaterNut.QuerySpace.PreviousDocumentQS.ViewModels;

namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for PreviousDocuments.xaml
	/// </summary>
	public partial class PreviousDocuments : UserControl
	{
		public PreviousDocuments()
		{
			InitializeComponent();
            im = (PreviousDocumentModel)FindResource("PreviousDocumentsModelDataSource");
			// Insert code required on object creation below this point.
		}
        PreviousDocumentModel im;
        private void ItemLstGrd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            ((ListBox)sender).ScrollIntoView(e.AddedItems[0]);
        }

        private void ViewAll(object sender, MouseButtonEventArgs e)
        {
            im.ViewAll();
        }

        private async void Send2Excel(object sender, MouseButtonEventArgs e)
        {
            await im.Send2Excel().ConfigureAwait(false);
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
            
                var textBox = sender as DatePicker;
                if (textBox != null)
                    textBox.GetBindingExpression(DatePicker.SelectedDateProperty).UpdateSource();
           
	    }
	}
}