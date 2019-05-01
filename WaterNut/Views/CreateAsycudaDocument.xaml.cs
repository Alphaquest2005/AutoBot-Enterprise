using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CoreEntities.Client.Entities;
using WaterNut.QuerySpace.CoreEntities.ViewModels;

namespace WaterNut.Views
{
	/// <summary>
	/// Interaction logic for CreateAsycudaDocument.xaml
	/// </summary>
	public partial class CreateAsycudaDocument : UserControl
	{
		public CreateAsycudaDocument()
		{
			InitializeComponent();
            im = (CreateAsycudaDocumentModel)FindResource("CreateAsycudaDocumentModelDataSource");
			// Insert code required on object creation below this point.
		}
        CreateAsycudaDocumentModel im;
        private async void NewBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            await im.NewDocument().ConfigureAwait(false);
        }

        private async void SaveBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            await im.SaveDocumentSetEx().ConfigureAwait(false);

        }

        private async void DeleteBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            await im.DeleteDocument().ConfigureAwait(false);
        }

        private async void CreateSet(object sender, MouseButtonEventArgs e)
        {
            await im.NewDocumentSet(BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ConfigureAwait(false);
        }

        private async void AssessAll(object sender, MouseButtonEventArgs e)
        {
            await im.AssessAllinSet().ConfigureAwait(false);
        }

	    
	}
}