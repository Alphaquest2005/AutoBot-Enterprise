using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoreEntities.Client.Entities;
using OCR.Client.Entities;
using OCR.Client.Repositories;
using SimpleMvvmToolkit;
using WaterNut.QuerySpace.CoreEntities;
using OCR_BaseViewModel = WaterNut.QuerySpace.OCR.ViewModels.BaseViewModel;

namespace RegexImporter.ViewModels
{
    public class
        TXTViewerViewModel : ViewModelBase<TXTViewerViewModel> //AsycudaDocumentSet_AttachmentsViewModel_AutoGen
    {
        private ImportErrors _currentImportError;

        private TextSelection _currentTextSelection;


        private string _pdfText = "No Text Found";

        static TXTViewerViewModel()
        {
            Instance = new TXTViewerViewModel();
        }

        private TXTViewerViewModel()
        {
            RegisterToReceiveMessages<AsycudaDocumentSet_Attachments>(
                MessageToken.CurrentAsycudaDocumentSet_AttachmentsChanged, OnCurrentAttachmentChanged);
            RegisterToReceiveMessages<Match>(WaterNut.QuerySpace.OCR.MessageToken.CurrentMatch, OnCurrentMatch);
            RegisterToReceiveMessages<Group>(WaterNut.QuerySpace.OCR.MessageToken.CurrentGroup, OnCurrentGroup);
            RegisterToReceiveMessages<Capture>(WaterNut.QuerySpace.OCR.MessageToken.CurrentCapture, OnCurrentCapture);

            RegisterToReceiveMessages<string>(WaterNut.QuerySpace.OCR.MessageToken.PDFText, onPDFTextChanged);

            //RegisterToReceiveMessages<AsycudaDocumentSet_Attachments>(MessageToken.CurrentAsycudaDocumentSet_AttachmentsChanged, OnCurrentAsycudaDocumentSet_AttachmentsChanged);
            //RegisterToReceiveMessages<Part>(MessageToken.CurrentPartChanged, OnOCR_PartExsChanged2);
            // RegisterToReceiveMessages<AsycudaDocumentSetEx>(MessageToken.CurrentAsycudaDocumentSetExChanged, OnOCR_CurrentLineChanged);
        }

        public static TXTViewerViewModel Instance { get; }

        public TextSelection CurrentTextSelection
        {
            get => _currentTextSelection;
            set
            {
                _currentTextSelection = value;
                NotifyPropertyChanged(x => CurrentTextSelection);
            }
        }

        public ImportErrors CurrentImportError
        {
            get => _currentImportError;
            set
            {
                _currentImportError = value;
                NotifyPropertyChanged(x => CurrentImportError);
            }
        }

        public string PDFText
        {
            get => _pdfText;
            set
            {
                _pdfText = value;
                NotifyPropertyChanged(x => PDFText);
            }
        }


        public Task Initialization { get; }

        private void onPDFTextChanged(object sender, NotificationEventArgs<string> e)
        {
            PDFText = e.Data;
        }

        private void OnCurrentCapture(object sender, NotificationEventArgs<Capture> e)
        {
            CurrentTextSelection = new TextSelection
            {
                Index = e.Data.Index,
                Length = e.Data.Length
            };
        }

        private void OnCurrentGroup(object sender, NotificationEventArgs<Group> e)
        {
            CurrentTextSelection = new TextSelection
            {
                Index = e.Data.Index,
                Length = e.Data.Length
            };
        }


        private void OnCurrentMatch(object sender, NotificationEventArgs<Match> e)
        {
            CurrentTextSelection = new TextSelection
            {
                Index = e.Data.Index,
                Length = e.Data.Length
            };
        }


        private void OnCurrentAttachmentChanged(object sender, NotificationEventArgs<AsycudaDocumentSet_Attachments> e)
        {
            if (e.Data != null)
            {
                CurrentImportError = new ImportErrorsRepository()
                    .GetImportErrors(e.Data.Id.ToString(), new List<string> {"OCR_FailedLines"}).Result;
                BaseViewModel.Instance.CurrentImportError = CurrentImportError;
                PDFText = null;
                if (CurrentImportError == null) return;
                var currentLine = new LinesRepository().GetLines(
                        CurrentImportError?.OCR_FailedLines.FirstOrDefault()?.LineId.ToString(),
                        new List<string> {"Parts"})
                    .Result;
                OCR_BaseViewModel.Instance.CurrentParts = currentLine.Parts;
                OCR_BaseViewModel.Instance.CurrentInvoices = new InvoicesRepository().GetInvoices(
                    OCR_BaseViewModel.Instance.CurrentParts.TemplateId.ToString(), new List<string> {"Parts"}).Result;
                OCR_BaseViewModel.Instance.CurrentLines = currentLine;
                PDFText = CurrentImportError.PdfText;
            }
        }
    }

    public class TextSelection
    {
        public int Index { get; set; }
        public int Length { get; set; }
    }
}