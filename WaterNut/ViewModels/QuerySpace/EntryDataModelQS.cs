using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Core.Common.UI;
using CoreEntities.Client.Entities;
using CoreEntities.Client.Repositories;
using EntryDataQS.Client.Repositories;
using Microsoft.Win32;
using SimpleMvvmToolkit;

namespace WaterNut.QuerySpace.EntryDataQS.ViewModels
{
    public partial class EntryDataModel : EntryDataExViewModel_AutoGen
	{
        private static readonly EntryDataModel instance;
        static EntryDataModel()
        {
            instance = new EntryDataModel();

        }

        public static EntryDataModel Instance
        {
            get { return instance; }
        }

        private EntryDataModel()
        {
            RegisterToReceiveMessages<AsycudaDocumentSetEx>(CoreEntities.MessageToken.CurrentAsycudaDocumentSetExChanged, OnCurrentAsycudaDocumentSetExChanged);
            RegisterToReceiveMessages<AsycudaDocument>(CoreEntities.MessageToken.CurrentAsycudaDocumentChanged, OnCurrentAsycudaDocumentChanged);

        }

        private void OnCurrentAsycudaDocumentChanged(object sender, NotificationEventArgs<AsycudaDocument> e)
        {
            if (e.Data != null)
            {
                vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}";
                // vloader.NavExpression = string.Format("AsycudaDocumentSetId = {0}", e.Data.AsycudaDocumentSetId);
                vloader.SetNavigationExpression("AsycudaDocuments", $"AsycudaDocumentId == {e.Data.ASYCUDA_Id}");
                EntryDataEx.Refresh();
            }
        }



        private new void OnCurrentAsycudaDocumentSetExChanged(object sender, NotificationEventArgs<AsycudaDocumentSetEx> e)
        {

            if (e.Data != null)
            {
                vloader.FilterExpression = $"ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}" +
                                           $" && AsycudaDocumentSetId == {e.Data.AsycudaDocumentSetId}";
                //vloader.SetNavigationExpression("AsycudaDocumentSets",
                //    $"AsycudaDocumentSetId == {e.Data.AsycudaDocumentSetId}");
                EntryDataEx.Refresh();
            }
        }




        internal async Task RemoveEntryData(global::EntryDataQS.Client.Entities.EntryDataEx entryDataEx)
        {
           await EntryDataRepository.Instance.DeleteEntryData(entryDataEx.EntityId).ConfigureAwait(false);
            
            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentsChanged, null,
                       new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentsChanged));

            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged, null,
                            new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged));

            MessageBus.Default.BeginNotify(CounterPointQS.MessageToken.CounterPointPOsChanged, null,
                           new NotificationEventArgs(CounterPointQS.MessageToken.CounterPointPOsChanged));

            MessageBus.Default.BeginNotify(MessageToken.EntryDataExChanged, null,
                            new NotificationEventArgs(MessageToken.EntryDataExChanged));
        }

        internal async Task AddDocToEntry(System.Collections.Generic.List<global::EntryDataQS.Client.Entities.EntryDataEx> lst, bool perInvoice,bool combineEntryDataInSameFile)
        {
            StatusModel.Timer($"Creating Entries");
            var docSet = CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx;
            if ( docSet == null)
            {
                MessageBox.Show("Please select a Document Set.");
                return;
            }
            await EntryDataExRepository.Instance.AddDocToEntry(lst.Select(x => x.InvoiceNo),docSet.AsycudaDocumentSetId, perInvoice, combineEntryDataInSameFile).ConfigureAwait(false);
            //----- think this causing the docset to overwrite with empty one and not necessary
            //await AsycudaDocumentSetExRepository.Instance.SaveAsycudaDocumentSetEx(docSet).ConfigureAwait(false);
            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentsChanged, null,
              new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentsChanged));
            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged, null,
                new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged));
            StatusModel.StopStatusUpdate();
            MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        public async Task SaveCSV(string fileType)
        {
            StatusModel.Timer($"Importing {fileType}");
            var docSet = CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx;
            if (docSet == null)
            {
                MessageBox.Show("Please select a Document Set.");
                return;
            }
            await QuerySpace.SaveCSV.Instance.SaveCSVFile(fileType,
                CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId).ConfigureAwait(false);

            MessageBus.Default.BeginNotify(MessageToken.EntryDataExChanged, null,
                         new NotificationEventArgs(MessageToken.EntryDataExChanged));
            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentsChanged, null,
                new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentsChanged));
            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged, null,
                new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged));

            StatusModel.StopStatusUpdate();
            MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            
        }

        internal async Task RemoveSelectedEntryData(
            System.Collections.Generic.List<global::EntryDataQS.Client.Entities.EntryDataEx> list)
        {

            var res = MessageBox.Show("Are you sure you want to delete all Selected Items?", "Delete selected Items",
                MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                StatusModel.Timer($"Deleting Data");
                await EntryDataRepository.Instance.RemoveSelectedEntryData(SelectedEntryDataEx.Select(x => x.InvoiceNo)).ConfigureAwait(false);

                MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentsChanged, null,
                    new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentsChanged));

                MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged, null,
                    new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged));

                MessageBus.Default.BeginNotify(CounterPointQS.MessageToken.CounterPointPOsChanged, null,
                    new NotificationEventArgs(CounterPointQS.MessageToken.CounterPointPOsChanged));

                MessageBus.Default.BeginNotify(MessageToken.EntryDataExChanged, null,
                    new NotificationEventArgs(MessageToken.EntryDataExChanged));

                MessageBus.Default.BeginNotify(MessageToken.EntryDataExFilterExpressionChanged, null,
                   new NotificationEventArgs(MessageToken.EntryDataExFilterExpressionChanged));
                StatusModel.StopStatusUpdate();
                MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                //EntryDataEx.Refresh();
            }
        }
	}
}