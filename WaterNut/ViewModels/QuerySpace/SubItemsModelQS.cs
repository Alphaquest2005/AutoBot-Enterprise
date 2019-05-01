using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SimpleMvvmToolkit;

namespace WaterNut.QuerySpace.CoreEntities.ViewModels
{
    public partial class SubItemsModelQS : SubItemsViewModel_AutoGen
    {
        private static readonly SubItemsModelQS instance;
        static SubItemsModelQS()
         {
             instance = new SubItemsModelQS() {ViewCurrentAsycudaDocumentItem = true};
         }

         public static SubItemsModelQS Instance
        {
            get { return instance; }
        }

         private SubItemsModelQS()
        {
        }

         internal async Task SaveCSV(string fileType)
         {

             await QuerySpace.SaveCSV.Instance.SaveCSVFile(fileType, BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId).ConfigureAwait(false);

             MessageBus.Default.BeginNotify(MessageToken.SubItemsChanged, null,
                           new NotificationEventArgs(MessageToken.SubItemsChanged));
             MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
         }
    }
}
