using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreEntities.Client.Entities;
using CoreEntities.Client.Repositories;
using SimpleMvvmToolkit;
using WaterNut.QuerySpace.CoreEntities;
using WaterNut.QuerySpace.CoreEntities.ViewModels;
using OCR_BaseViewModel = WaterNut.QuerySpace.OCR.ViewModels.BaseViewModel;

namespace RegexImporter.ViewModels
{
    public class DocumentFilesViewModel : AsycudaDocumentSetExViewModel_AutoGen
    {
        private IEnumerable<AsycudaDocumentSet_Attachments> _attachments;
        private DateTime _endFileDateFilter = DateTime.Today;
        private DateTime _startFileDateFilter = DateTime.Today;

        static DocumentFilesViewModel()
        {
            Instance = new DocumentFilesViewModel
            {
                ViewCurrentApplicationSettings = true
            };
        }

        private DocumentFilesViewModel()
        {
            RegisterToReceiveMessages<AsycudaDocumentSetEx>(MessageToken.CurrentAsycudaDocumentSetExChanged,
                OnCurrentAsycudaDocumentSetExChanged2);
            RegisterToReceiveMessages(MessageToken.AsycudaDocumentSetExsChanged, OnAsycudaDocumentSetExsChanged3);
            RegisterToReceiveMessages<AsycudaDocumentSetEx>(MessageToken.AsycudaDocumentSetExsChanged,
                OnAsycudaDocumentSetExsChanged2);
        }

        public static new DocumentFilesViewModel Instance { get; }

        public DateTime StartFileDateFilter
        {
            get => _startFileDateFilter;
            set
            {
                _startFileDateFilter = value;
                GetAttachments(WaterNut.QuerySpace.CoreEntities.ViewModels.BaseViewModel.Instance
                    .CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId);
            }
        }

        public DateTime EndFileDateFilter
        {
            get => _endFileDateFilter;
            set
            {
                _endFileDateFilter = value;
                GetAttachments(WaterNut.QuerySpace.CoreEntities.ViewModels.BaseViewModel.Instance
                    .CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId);
            }
        }

        public IEnumerable<AsycudaDocumentSet_Attachments> Attachments
        {
            get => _attachments;
            set
            {
                _attachments = value;
                NotifyPropertyChanged(x => Attachments);
            }
        }


        private async void OnAsycudaDocumentSetExsChanged3(object sender, NotificationEventArgs e)
        {
            await RefreshAsycudaDocuments().ConfigureAwait(false);
        }

        private async void OnAsycudaDocumentSetExsChanged2(object sender, NotificationEventArgs<AsycudaDocumentSetEx> e)
        {
            await RefreshAsycudaDocuments().ConfigureAwait(false);
        }

        private async Task RefreshAsycudaDocuments()
        {
            if (WaterNut.QuerySpace.CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx != null)
                Attachments =
                    await
                        AsycudaDocumentSet_AttachmentsRepository.Instance
                            .GetAsycudaDocumentSet_AttachmentsByAsycudaDocumentSetId(
                                WaterNut.QuerySpace.CoreEntities.ViewModels.BaseViewModel.Instance
                                    .CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId.ToString())
                            .ConfigureAwait(false);
        }

        private void OnCurrentAsycudaDocumentSetExChanged2(object sender,
            NotificationEventArgs<AsycudaDocumentSetEx> e)
        {
            if (e.Data != null)
                GetAttachments(e.Data.AsycudaDocumentSetId);
            //Attachments =
            //     await
            //         AsycudaDocumentSet_AttachmentsRepository.Instance.GetAsycudaDocumentSet_AttachmentsByExpressionNav(
            //             $"AsycudaDocumentSetId == {e.Data.AsycudaDocumentSetId}", new Dictionary<string, string>(){{"Attachments","FilePath.Contains(\".pdf\")"}}, new List<string>(){"Attachments"}).ConfigureAwait(false);
        }

        private void GetAttachments(int asycudaDocumentSetId)
        {
            Attachments =
                AsycudaDocumentSet_AttachmentsRepository.Instance.GetAsycudaDocumentSet_AttachmentsByExpressionNav(
                    $"AsycudaDocumentSetId == {asycudaDocumentSetId} && FileDate >= \"{StartFileDateFilter.ToShortDateString()}\" && FileDate <= \"{EndFileDateFilter.ToShortDateString()}\"",
                    new Dictionary<string, string> {{"Attachments", "FilePath.Contains(\".pdf\")"}},
                    new List<string> {"Attachments"}).Result;
        }
    }
}