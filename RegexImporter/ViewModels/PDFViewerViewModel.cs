using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common;
using CoreEntities.Client.Entities;
using OCR.Client.Entities;
using OCR.Client.Repositories;
using SimpleMvvmToolkit;
using WaterNut.DataSpace;
using WaterNut.QuerySpace.CoreEntities;
using WaterNut.QuerySpace.CoreEntities.ViewModels;
using WaterNut.QuerySpace.OCR.ViewModels;
using AsycudaDocumentSetEx = CoreEntities.Client.Entities.AsycudaDocumentSetEx;
using OCR_BaseViewModel = WaterNut.QuerySpace.OCR.ViewModels.BaseViewModel;

namespace RegexImporter.ViewModels
{
    
    public class PDFViewerViewModel : ViewModelBase<PDFViewerViewModel> //AsycudaDocumentSet_AttachmentsViewModel_AutoGen
    {
        private static readonly PDFViewerViewModel instance;

        static PDFViewerViewModel()
        {
            instance = new PDFViewerViewModel()
            {
                
            };
        }

        public static PDFViewerViewModel Instance
        {
            get { return instance; }
        }

        private PDFViewerViewModel()
        {
            RegisterToReceiveMessages<AsycudaDocumentSet_Attachments>(MessageToken.CurrentAsycudaDocumentSet_AttachmentsChanged, OnCurrentAttachmentChanged);
//RegisterToReceiveMessages<AsycudaDocumentSet_Attachments>(MessageToken.CurrentAsycudaDocumentSet_AttachmentsChanged, OnCurrentAsycudaDocumentSet_AttachmentsChanged);
            //RegisterToReceiveMessages<Part>(MessageToken.CurrentPartChanged, OnOCR_PartExsChanged2);
           // RegisterToReceiveMessages<AsycudaDocumentSetEx>(MessageToken.CurrentAsycudaDocumentSetExChanged, OnOCR_CurrentLineChanged);



        }

      

        private void OnCurrentAttachmentChanged(object sender, NotificationEventArgs<AsycudaDocumentSet_Attachments> e)
        {
            if (e.Data != null)
            {
                CurrentAttachment = e.Data;
            }
        }

    

        private AsycudaDocumentSet_Attachments _currentAttachment = null;
        public AsycudaDocumentSet_Attachments CurrentAttachment
        {
            get { return _currentAttachment; }
            set
            {
                _currentAttachment = value;
                NotifyPropertyChanged(x => this.CurrentAttachment);
            }
        }



        public Task Initialization { get; }

        public void ExtractTxt()
        {
           PDFText = InvoiceReader.GetPdftxt(CurrentAttachment.Attachments.FilePath).ToString();
        }

        private string _pdfText = null;
        public string PDFText
        {
            get { return _pdfText; }
            set
            {
                _pdfText = value;
                BeginSendMessage(WaterNut.QuerySpace.OCR.MessageToken.PDFText,
                    new NotificationEventArgs<string>(WaterNut.QuerySpace.OCR.MessageToken.PDFText, _pdfText));
                NotifyPropertyChanged(x => this.PDFText);

            }
        }
    }
}
