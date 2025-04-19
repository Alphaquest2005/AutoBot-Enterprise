using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using WaterNut.QuerySpace;
using WaterNut.QuerySpace.AllocationQS.ViewModels;
using BaseViewModel = WaterNut.QuerySpace.CoreEntities.ViewModels.BaseViewModel;
using Path = System.Windows.Shapes.Path;

namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for SalesReport.xaml
	/// </summary>
	public partial class SalesReport : UserControl
	{
		public SalesReport()
		{
			InitializeComponent();
		    im =(SalesReportModel) FindResource("SalesReportModelDataSource");
		    // Insert code required on object creation below this point.
		}

        private void GridData_LoadingRow(object sender, DataGridRowEventArgs e)
        {

        }

	    private SalesReportModel im;
        private async void PrintReport(object sender, MouseButtonEventArgs e)
        {
            if (BaseViewModel.Instance.CurrentAsycudaDocument == null)
            {
                MessageBox.Show("Please select a document.");
                return;
            }
            var res = MessageBox.Show("Do you want to Export Sales?", "Export Sales", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {

                var od = new SaveFileDialog
                {
                    FileName = BaseViewModel.Instance.CurrentAsycudaDocument.ReferenceNumber + ".csv"
                };
                var result = od.ShowDialog();
                if (result == true)
                {
                    string first = null;
                    foreach (var name in od.FileNames)
                    {
                        first = name;
                        break;
                    }
                    if (first != null)
                    {
                       await im.Send2Excel(first,GridData).ConfigureAwait(false);
                    }

                }
                MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            }
            
        }

        private async void ExportDocSetReport(object sender, MouseButtonEventArgs e)
        {
            if (BaseViewModel.Instance.CurrentAsycudaDocumentSetEx == null)
            {
                MessageBox.Show("Please Select DoumentSet");
                return;
            }
            var res = MessageBox.Show("Do you want to Export ALL Documents in this Document Set?", "Export Document Set", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {

                var od = new SaveFileDialog
                {
                    FileName = BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.Declarant_Reference_Number + ".csv"
                };
                var result = od.ShowDialog();
                if (result == true)
                {
                    string first = null;
                    foreach (var name in od.FileNames)
                    {
                        first = name;
                        break;
                    }
                    if (first != null)
                    {
                        var directoryInfo = new DirectoryInfo(first).Parent;
                        if (directoryInfo != null)
                        {
                            await im.ExportDocSetSalesReport(BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId, directoryInfo.FullName).ConfigureAwait(false);
                        }
                    }
                    
                }
                MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            }

           
        }
	}
}