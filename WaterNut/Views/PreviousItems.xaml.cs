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
using WaterNut.QuerySpace.PreviousDocumentQS.ViewModels;

namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for PreviousItems.xaml
	/// </summary>
	public partial class PreviousItems : UserControl
	{
		public PreviousItems()
		{
			InitializeComponent();
			
			// Insert code required on object creation below this point.
		}

        private void ViewAll(object sender, MouseButtonEventArgs e)
        {

        }

        private async void Send2Excel(object sender, MouseButtonEventArgs e)
        {
            await PreviousItemsViewModel.Instance.Send2Excel().ConfigureAwait(false);
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
	}
}