using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
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
            im.PropertyChanged += onpropchanged;
            // Insert code required on object creation below this point.

        }

        private void onpropchanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(im.CurrentRegex))
            {
                ProcessRegex();
            }
        }

        private void ProcessRegex()
        {
            ResultsTree.Items.Clear();
            im.Matches.Clear();
            if (im.CurrentRegex == null) return;
            var pdfText = WaterNut.QuerySpace.OCR.ViewModels.BaseViewModel.Instance.CurrentImportErrors?.PdfText?? TXTViewerViewModel.Instance.PDFText ?? "No Text Found";
            
            for (Match m =
                    im.CurrentRegex.Regex.Match(pdfText);
                m.Success;
                m = m.NextMatch())
            {
                im.Matches.Add(m);
                if (m.Value.Length > 0)
                {
                    var tItm = new TreeViewItem() {Header = "[" + m.Value + "]"};
                    ResultsTree.Items.Add(tItm);
                    int ThisNode = ResultsTree.Items.Count - 1;
                    ((TreeViewItem) ResultsTree.Items[ThisNode]).Tag = m;
                    if (m.Groups.Count > 1)
                    {
                        for (int i = 1; i < m.Groups.Count; i++)
                        {
                            ((TreeViewItem) ResultsTree.Items[ThisNode]).Items.Add(new TreeViewItem()
                                {Header = im.CurrentRegex.Regex.GroupNameFromNumber(i) + ": [" + m.Groups[i].Value + "]"});
                            ((TreeViewItem) ((TreeViewItem) ResultsTree.Items[ThisNode]).Items[i - 1]).Tag = m.Groups[i];
                            //This bit of code puts in another level of nodes showing the captures for each group
                            int Number = m.Groups[i].Captures.Count;
                            if (Number > 1)
                                for (int j = 0; j < Number; j++)
                                {
                                    ((TreeViewItem) ((TreeViewItem) ResultsTree.Items[ThisNode]).Items[i - 1]).Items.Add(
                                        m.Groups[i].Captures[j].Value);
                                    ((TreeViewItem) ((TreeViewItem) ((TreeViewItem) ResultsTree.Items[ThisNode]).Items[i - 1])
                                        .Items[j]).Tag = m.Groups[i].Captures[j];
                                }
                        }
                    }
                }
            }
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

       

        private void ResultsTree_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null) return;
           // var mat = im.Matches.FirstOrDefault(x => ((TreeViewItem)e.NewValue).Header.ToString().Contains(x.Value));
            if (((TreeViewItem)ResultsTree.SelectedItem).Tag is Match mat)
                im.CurrentMatch = mat;
            else if(((TreeViewItem)ResultsTree.SelectedItem).Tag is Group grp)
            {
                im.CurrentGroup = grp;
            }
            else if (((TreeViewItem)ResultsTree.SelectedItem).Tag is Capture capture)
            {
                im.CurrentCapture = capture;
            }

        }

        private void Save(object sender, MouseButtonEventArgs e)
        {
            im.CurrentRegexObject.Save(im.CurrentRegexObject.Source);
        }

        private void FindMatches(object sender, MouseButtonEventArgs e)
        {
            ProcessRegex();
        }

        private void ItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new System.NotImplementedException();
        }
    }
}