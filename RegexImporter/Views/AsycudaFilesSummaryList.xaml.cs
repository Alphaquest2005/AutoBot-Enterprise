﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Core.Common.UI.DataVirtualization;
using CoreEntities.Client.Entities;
using RegexImporter.ViewModels;
using WaterNut.QuerySpace.CoreEntities.ViewModels;

namespace RegexImporter.Views
{
	/// <summary>
	/// Interaction logic for AsycudaFilesSummaryList.xaml
	/// </summary>
	public partial class AsycudaFilesSummaryList : UserControl
	{
		public AsycudaFilesSummaryList()
		{
			InitializeComponent();
			im = (DocumentFilesViewModel)FindResource("DocumentFilesViewModelDataSource");
			// Insert code required on object creation below this point.

		}
        DocumentFilesViewModel im;
		
		private async void ImportBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			
			//await im.ImportDocuments(im.ImportOnlyRegisteredDocuments, im.ImportTariffCodes, im.NoMessages, im.OverwriteExisting, im.LinkPi).ConfigureAwait(false);
		}

		private async void ExportBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			//await im.ExportDocuments().ConfigureAwait(false);
		}

		private void Expander_Expanded(object sender, RoutedEventArgs e)
		{
			WaterNut.QuerySpace.CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx = (AsycudaDocumentSetEx)((sender as FrameworkElement).DataContext as VirtualListItem<AsycudaDocumentSetEx>).Data;

		}

		private async void DeleteAll(object sender, MouseButtonEventArgs e)
		{
            if (!(sender is FrameworkElement frameworkElement)) return;
			var asycudaDocumentSet = frameworkElement.DataContext as VirtualListItem<AsycudaDocumentSetEx>;
			//if (asycudaDocumentSet != null)
				//await im.DeleteDocuments(asycudaDocumentSet.Data.AsycudaDocumentSetId).ConfigureAwait(false);
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
			//im.ViewAll();
		}

	

		private void Expander_Collapsed(object sender, RoutedEventArgs e)
		{
			WaterNut.QuerySpace.CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx = null;
			WaterNut.QuerySpace.CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocument = null;
		   
		}

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

	    private async void AttachDocuments(object sender, MouseButtonEventArgs e)
	    {

	      //  await im.AttachDocuments().ConfigureAwait(false);

	    }
	}
}