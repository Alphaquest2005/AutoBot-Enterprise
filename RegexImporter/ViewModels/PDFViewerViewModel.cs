using System.Threading.Tasks;
using CoreEntities.Client.Entities;
using SimpleMvvmToolkit;
using WaterNut.DataSpace;
using WaterNut.QuerySpace.CoreEntities;
using OCR_BaseViewModel = WaterNut.QuerySpace.OCR.ViewModels.BaseViewModel;

namespace RegexImporter.ViewModels
{
    public class
        PDFViewerViewModel : ViewModelBase<PDFViewerViewModel> //AsycudaDocumentSet_AttachmentsViewModel_AutoGen
    {
        private AsycudaDocumentSet_Attachments _currentAttachment;

        private string _pdfText;

        static PDFViewerViewModel()
        {
            Instance = new PDFViewerViewModel();
        }

        private PDFViewerViewModel()
        {
            RegisterToReceiveMessages<AsycudaDocumentSet_Attachments>(
                MessageToken.CurrentAsycudaDocumentSet_AttachmentsChanged, OnCurrentAttachmentChanged);
//RegisterToReceiveMessages<AsycudaDocumentSet_Attachments>(MessageToken.CurrentAsycudaDocumentSet_AttachmentsChanged, OnCurrentAsycudaDocumentSet_AttachmentsChanged);
            //RegisterToReceiveMessages<Part>(MessageToken.CurrentPartChanged, OnOCR_PartExsChanged2);
            // RegisterToReceiveMessages<AsycudaDocumentSetEx>(MessageToken.CurrentAsycudaDocumentSetExChanged, OnOCR_CurrentLineChanged);
        }

        public static PDFViewerViewModel Instance { get; }

        public AsycudaDocumentSet_Attachments CurrentAttachment
        {
            get => _currentAttachment;
            set
            {
                _currentAttachment = value;
                NotifyPropertyChanged(x => CurrentAttachment);
            }
        }


        public Task Initialization { get; }

        public string PDFText
        {
            get => _pdfText;
            set
            {
                _pdfText = value;
                BeginSendMessage(WaterNut.QuerySpace.OCR.MessageToken.PDFText,
                    new NotificationEventArgs<string>(WaterNut.QuerySpace.OCR.MessageToken.PDFText, _pdfText));
                NotifyPropertyChanged(x => PDFText);
            }
        }


        private void OnCurrentAttachmentChanged(object sender, NotificationEventArgs<AsycudaDocumentSet_Attachments> e)
        {
            if (e.Data != null) CurrentAttachment = e.Data;
        }

        public void ExtractTxt()
        {
            //PDFText = InvoiceReader.GetPdftxt(CurrentAttachment.Attachments.FilePath).ToString();
        }
    }
}