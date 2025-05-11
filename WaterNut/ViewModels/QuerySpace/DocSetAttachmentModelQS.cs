using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Core.Common.UI;
using CoreEntities.Client.Entities;
using CoreEntities.Client.Repositories;
using Microsoft.Win32;
using SimpleMvvmToolkit;

namespace WaterNut.QuerySpace.CoreEntities.ViewModels
{
    public partial class DocSetAttachmentViewModel : AsycudaDocumentSetAttachmentsViewModel_AutoGen
    {
        private static readonly DocSetAttachmentViewModel instance;
        static DocSetAttachmentViewModel()
        {
            instance = new DocSetAttachmentViewModel();

        }

        public static DocSetAttachmentViewModel Instance
        {
            get { return instance; }
        }

        private DocSetAttachmentViewModel()
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
                AsycudaDocumentSetAttachments.Refresh();
            }
        }



        private new void OnCurrentAsycudaDocumentSetExChanged(object sender, NotificationEventArgs<AsycudaDocumentSetEx> e)
        {

            if (e.Data != null)
            {
                vloader.FilterExpression = $"AsycudaDocumentSetId == {e.Data.AsycudaDocumentSetId}";
                //vloader.SetNavigationExpression("AsycudaDocumentSets",
                //    $"AsycudaDocumentSetId == {e.Data.AsycudaDocumentSetId}");
                AsycudaDocumentSetAttachments.Refresh();
            }
        }




        internal async Task RemoveEntryData(global::EntryDataQS.Client.Entities.EntryDataEx entryDataEx)
        {
           await AsycudaDocumentSetAttachmentsRepository.Instance.DeleteAsycudaDocumentSetAttachments(entryDataEx.EntryData_Id.ToString()).ConfigureAwait(false);
            
            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentsChanged, null,
                       new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentsChanged));

            MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged, null,
                            new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged));

            MessageBus.Default.BeginNotify(CounterPointQS.MessageToken.CounterPointPOsChanged, null,
                           new NotificationEventArgs(CounterPointQS.MessageToken.CounterPointPOsChanged));

            MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentSetAttachmentsChanged, null,
                            new NotificationEventArgs(MessageToken.AsycudaDocumentSetAttachmentsChanged));
        }

        internal Task AddDocToEntry(System.Collections.Generic.List<global::EntryDataQS.Client.Entities.EntryDataEx> lst, bool perInvoice,bool combineEntryDataInSameFile)
        {
            return Task.CompletedTask;
            //StatusModel.Timer($"Creating Entries");
            //var docSet = CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx;
            //if ( docSet == null)
            //{
            //    MessageBox.Show("Please select a Document Set.");
            //    return;
            //}
            //await EntryDataExRepository.Instance.AddDocToEntry(lst.Select(x => x.InvoiceNo),docSet.AsycudaDocumentSetId, perInvoice, combineEntryDataInSameFile, false).ConfigureAwait(false);
            ////----- think this causing the docset to overwrite with empty one and not necessary
            ////await AsycudaDocumentSetExRepository.Instance.SaveAsycudaDocumentSetEx(docSet).ConfigureAwait(false);
            //MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentsChanged, null,
            //  new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentsChanged));
            //MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged, null,
            //    new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged));
            //StatusModel.StopStatusUpdate();
            //MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

            MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentSetAttachmentsChanged, null,
                         new NotificationEventArgs(MessageToken.AsycudaDocumentSetAttachmentsChanged));
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
                await AsycudaDocumentSetAttachmentsRepository.Instance.RemoveSelectedAsycudaDocumentSetAttachments(SelectedAsycudaDocumentSetAttachments.Select(x => x.Id.ToString())).ConfigureAwait(false);

                MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentsChanged, null,
                    new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentsChanged));

                MessageBus.Default.BeginNotify(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged, null,
                    new NotificationEventArgs(CoreEntities.MessageToken.AsycudaDocumentSetExsChanged));

                MessageBus.Default.BeginNotify(CounterPointQS.MessageToken.CounterPointPOsChanged, null,
                    new NotificationEventArgs(CounterPointQS.MessageToken.CounterPointPOsChanged));

                MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentSetAttachmentsChanged, null,
                    new NotificationEventArgs(MessageToken.AsycudaDocumentSetAttachmentsChanged));

                MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentSetAttachmentsFilterExpressionChanged, null,
                   new NotificationEventArgs(MessageToken.AsycudaDocumentSetAttachmentsFilterExpressionChanged));
                StatusModel.StopStatusUpdate();
                MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                //EntryDataEx.Refresh();
            }
        }


	}
}