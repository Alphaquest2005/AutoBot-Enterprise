using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common;
using CoreEntities.Client.Entities;
using CoreEntities.Client.Repositories;
using OCR.Client.Entities;
using OCR.Client.Repositories;
using SimpleMvvmToolkit;
using WaterNut.QuerySpace.CoreEntities.ViewModels;
using WaterNut.QuerySpace.OCR;
using WaterNut.QuerySpace.OCR.ViewModels;
using MessageToken = WaterNut.QuerySpace.CoreEntities.MessageToken;
using OCR_BaseViewModel = WaterNut.QuerySpace.OCR.ViewModels.BaseViewModel;

namespace RegexImporter.ViewModels
{
    
    public class DocumentFilesViewModel :  AsycudaDocumentSetExViewModel_AutoGen
    {
        private static readonly DocumentFilesViewModel instance;

        static DocumentFilesViewModel()
        {
            instance = new DocumentFilesViewModel()
            {
                ViewCurrentApplicationSettings = true,
                
            };
        }

        public static DocumentFilesViewModel Instance
        {
            get { return instance; }
        }

        private DocumentFilesViewModel()
        {
            RegisterToReceiveMessages<AsycudaDocumentSetEx>(WaterNut.QuerySpace.CoreEntities.MessageToken.CurrentAsycudaDocumentSetExChanged, OnCurrentAsycudaDocumentSetExChanged2);
            RegisterToReceiveMessages(WaterNut.QuerySpace.CoreEntities.MessageToken.AsycudaDocumentSetExsChanged, OnAsycudaDocumentSetExsChanged3);
            RegisterToReceiveMessages<AsycudaDocumentSetEx>(MessageToken.AsycudaDocumentSetExsChanged, OnAsycudaDocumentSetExsChanged2);
            



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
            {
                Attachments =
                    await
                        AsycudaDocumentSet_AttachmentsRepository.Instance.GetAsycudaDocumentSet_AttachmentsByAsycudaDocumentSetId(
                                WaterNut.QuerySpace.CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId.ToString())
                            .ConfigureAwait(false);
            }
        }

        private async void OnCurrentAsycudaDocumentSetExChanged2(object sender, NotificationEventArgs<AsycudaDocumentSetEx> e)
        {
            if (e.Data != null)
            {
                GetAttachments(e.Data.AsycudaDocumentSetId);
               //Attachments =
               //     await
               //         AsycudaDocumentSet_AttachmentsRepository.Instance.GetAsycudaDocumentSet_AttachmentsByExpressionNav(
               //             $"AsycudaDocumentSetId == {e.Data.AsycudaDocumentSetId}", new Dictionary<string, string>(){{"Attachments","FilePath.Contains(\".pdf\")"}}, new List<string>(){"Attachments"}).ConfigureAwait(false);
            }
        }

        public DateTime StartFileDateFilter
        {
            get => _startFileDateFilter;
            set
            {
                _startFileDateFilter = value;
                GetAttachments(WaterNut.QuerySpace.CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId);
            }
        }

        public DateTime EndFileDateFilter
        {
            get => _endFileDateFilter;
            set
            {
                _endFileDateFilter = value;
                GetAttachments(WaterNut.QuerySpace.CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId);

            }
        }

        private void GetAttachments(int asycudaDocumentSetId)
        {
            Attachments =
                AsycudaDocumentSet_AttachmentsRepository.Instance.GetAsycudaDocumentSet_AttachmentsByExpressionNav(
                    $"AsycudaDocumentSetId == {asycudaDocumentSetId} && FileDate >= \"{StartFileDateFilter.ToShortDateString()}\" && FileDate <= \"{EndFileDateFilter.ToShortDateString()}\"",
                    new Dictionary<string, string>() {{"Attachments", "FilePath.Contains(\".pdf\")"}},
                    new List<string>() {"Attachments"}).Result;
        }


        private IEnumerable<AsycudaDocumentSet_Attachments> _attachments = null;
        private DateTime _endFileDateFilter = DateTime.Today;
        private DateTime _startFileDateFilter = DateTime.Today;

        public IEnumerable<AsycudaDocumentSet_Attachments> Attachments
        {
            get { return _attachments; }
            set
            {
                _attachments = value;
                NotifyPropertyChanged(x => this.Attachments);
            }
        }
    }
}
