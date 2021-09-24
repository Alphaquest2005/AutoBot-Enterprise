using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            RegisterToReceiveMessages<Match>(WaterNut.QuerySpace.OCR.MessageToken.CurrentMatch, OnCurrentMatch);
            RegisterToReceiveMessages<Group>(WaterNut.QuerySpace.OCR.MessageToken.CurrentGroup, OnCurrentGroup);
            RegisterToReceiveMessages<Capture>(WaterNut.QuerySpace.OCR.MessageToken.CurrentCapture, OnCurrentCapture);

            RegisterToReceiveMessages<string>(WaterNut.QuerySpace.OCR.MessageToken.PDFText, onPDFTextChanged);

            //RegisterToReceiveMessages<AsycudaDocumentSet_Attachments>(MessageToken.CurrentAsycudaDocumentSet_AttachmentsChanged, OnCurrentAsycudaDocumentSet_AttachmentsChanged);
            //RegisterToReceiveMessages<Part>(MessageToken.CurrentPartChanged, OnOCR_PartExsChanged2);
            // RegisterToReceiveMessages<AsycudaDocumentSetEx>(MessageToken.CurrentAsycudaDocumentSetExChanged, OnOCR_CurrentLineChanged);



        }

        private void onPDFTextChanged(object sender, NotificationEventArgs<string> e)
        {
            PDFText = e.Data;
        }

        private void OnCurrentCapture(object sender, NotificationEventArgs<Capture> e)
        {
            CurrentTextSelection = new TextSelection()
            {
                Index = e.Data.Index,
                Length = e.Data.Length
            };

        }

        private void OnCurrentGroup(object sender, NotificationEventArgs<Group> e)
        {
            CurrentTextSelection = new TextSelection()
            {
                Index = e.Data.Index,
                Length = e.Data.Length
            };

        }

        private TextSelection _currentTextSelection = null;
        public TextSelection CurrentTextSelection
        {
            get { return _currentTextSelection; }
            set
            {
                _currentTextSelection = value;
                NotifyPropertyChanged(x => this.CurrentTextSelection);
            }
        }


        private void OnCurrentMatch(object sender, NotificationEventArgs<Match> e)
        {
            CurrentTextSelection = new TextSelection()
            {
                Index = e.Data.Index,
                Length = e.Data.Length
            };

        }


        private void OnCurrentAttachmentChanged(object sender, NotificationEventArgs<AsycudaDocumentSet_Attachments> e)
        {
            if (e.Data != null)
            {
                
                CurrentImportError = new ImportErrorsRepository().GetImportErrors(e.Data.Id.ToString(), new List<string>(){ "OCR_FailedLines" }).Result;
                BaseViewModel.Instance.CurrentImportError = CurrentImportError;
                PDFText = null;
                if (CurrentImportError == null) return;
                var currentLine = new LinesRepository().GetLines(
                    CurrentImportError?.OCR_FailedLines.FirstOrDefault()?.LineId.ToString(), new List<string>() {"Parts"}).Result;
                OCR_BaseViewModel.Instance.CurrentParts = currentLine.Parts;
                OCR_BaseViewModel.Instance.CurrentInvoices = new InvoicesRepository().GetInvoices(
                    OCR_BaseViewModel.Instance.CurrentParts.TemplateId.ToString(), new List<string>() { "Parts" }).Result;
                OCR_BaseViewModel.Instance.CurrentLines = currentLine;
                PDFText = CurrentImportError.PdfText;
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


        private string _pdfText = "No Text Found";
        public string PDFText
        {
            get { return _pdfText; }
            set
            {
                _pdfText = value;
               NotifyPropertyChanged(x => this.PDFText);

            }
        }



        public Task Initialization { get; }
    }

    public class TextSelection
    {
        public int Index { get; set; }
        public int Length { get; set; }
    }
}
