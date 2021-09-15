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
using WaterNut.QuerySpace.CoreEntities;
using WaterNut.QuerySpace.CoreEntities.ViewModels;
using WaterNut.QuerySpace.OCR.ViewModels;
using AsycudaDocumentSetEx = CoreEntities.Client.Entities.AsycudaDocumentSetEx;
using OCR_BaseViewModel = WaterNut.QuerySpace.OCR.ViewModels.BaseViewModel;

namespace RegexImporter.ViewModels
{
    
    public class TXTViewerViewModel : ViewModelBase<TXTViewerViewModel> //AsycudaDocumentSet_AttachmentsViewModel_AutoGen
    {
        private static readonly TXTViewerViewModel instance;

        static TXTViewerViewModel()
        {
            instance = new TXTViewerViewModel()
            {
                
            };
        }

        public static TXTViewerViewModel Instance
        {
            get { return instance; }
        }

        private TXTViewerViewModel()
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
                
                CurrentImportError = new ImportErrorsRepository().GetImportErrors(e.Data.Id.ToString(), new List<string>(){ "OCR_FailedLines" }).Result;
                OCR_BaseViewModel.Instance.CurrentImportErrors = CurrentImportError;
                if (CurrentImportError == null) return;
                var currentLine = new LineRepository().GetLine(
                    CurrentImportError?.OCR_FailedLines.FirstOrDefault()?.LineId.ToString(), new List<string>() {"Part"}).Result;
                OCR_BaseViewModel.Instance.CurrentPart = currentLine.Part;
                OCR_BaseViewModel.Instance.CurrentInvoice = new InvoiceRepository().GetInvoice(
                    OCR_BaseViewModel.Instance.CurrentPart.InvoiceId.ToString(), new List<string>() { "Parts" }).Result;
                OCR_BaseViewModel.Instance.CurrentLine = currentLine;
            }
        }

    

        private ImportErrors _currentImportError = null;
        public ImportErrors CurrentImportError
        {
            get { return _currentImportError; }
            set
            {
                _currentImportError = value;
                NotifyPropertyChanged(x => this.CurrentImportError);
            }
        }



        public Task Initialization { get; }
    }
}
