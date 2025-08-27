//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using OCR.Client.Entities;
//using OCR.Client.Repositories;
//using SimpleMvvmToolkit;
//using WaterNut.QuerySpace.OCR;
//using WaterNut.QuerySpace.OCR.ViewModels;
//using OCR_BaseViewModel = WaterNut.QuerySpace.OCR.ViewModels.BaseViewModel;

//namespace RegexImporter.ViewModels
//{
//    public class InvoiceExViewModel : InvoicesViewModel_AutoGen
//    {
//        public new readonly InvoicesVirturalListLoader vloader = new InvoicesVirturalListLoader
//            {IncludesLst = new List<string> {"RegEx.RegEx", "TemplateIdentificatonRegEx.OCR_RegularExpressions"}};


//        private IEnumerable<Lines> _lines;


//        private IEnumerable<Parts> _parts;

//        static InvoiceExViewModel()
//        {
//            Instance = new InvoiceExViewModel();
//        }

//        private InvoiceExViewModel()
//        {
//            RegisterToReceiveMessages<Invoices>(MessageToken.CurrentInvoicesChanged, OnCurrentOCR_InvoiceExChanged2);
//            RegisterToReceiveMessages(MessageToken.InvoicesChanged, OnOCR_InvoiceExChanged3);
//            RegisterToReceiveMessages<Invoices>(MessageToken.InvoicesChanged, OnOCR_InvoiceExChanged2);
//            RegisterToReceiveMessages<Parts>(MessageToken.CurrentPartsChanged, OnOCR_PartExsChanged2);
//        }

//        public static new InvoiceExViewModel Instance { get; }

//        public IEnumerable<Parts> Parts
//        {
//            get => _parts;
//            set
//            {
//                _parts = value;
//                NotifyPropertyChanged(x => Parts);
//            }
//        }

//        public IEnumerable<Lines> Lines
//        {
//            get => _lines;
//            set
//            {
//                _lines = value;
//                NotifyPropertyChanged(x => Lines);
//            }
//        }

//        private async void OnOCR_InvoiceExChanged3(object sender, NotificationEventArgs e)
//        {
//            await RefreshParts().ConfigureAwait(false);
//        }

//        private async void OnOCR_InvoiceExChanged2(object sender, NotificationEventArgs<Invoices> e)
//        {
//            await RefreshParts().ConfigureAwait(false);
//        }

//        private async Task RefreshParts()
//        {
//            if (OCR_BaseViewModel.Instance.CurrentInvoices != null)
//                Parts =
//                    await
//                        PartsRepository.Instance.GetPartsByTemplateId(
//                                OCR_BaseViewModel.Instance.CurrentInvoices.Id.ToString(),
//                                new List<string> {"PartTypes", "Start.RegularExpressions", "End.RegularExpressions"})
//                            .ConfigureAwait(false);
//        }

//        private async void OnCurrentOCR_InvoiceExChanged2(object sender,
//            NotificationEventArgs<Invoices> e)
//        {
//            if (e.Data != null)
//            {
//                Parts =
//                    await
//                        PartsRepository.Instance.GetPartsByExpression(
//                                $"TemplateId == {OCR_BaseViewModel.Instance.CurrentInvoices.Id}",
//                                new List<string> {"PartTypes", "Start.RegularExpressions", "End.RegularExpressions"}
//                            )
//                            .ConfigureAwait(false);
//                NameFilter = e.Data.Name;
//            }
//        }

//        private async void OnOCR_PartExsChanged2(object sender, NotificationEventArgs<Parts> e)
//        {
//            if (e.Data != null)
//                Lines =
//                    await
//                        LinesRepository.Instance.GetLinesByPartId(
//                                e.Data.Id.ToString(),
//                                new List<string>
//                                {
//                                    "RegularExpressions", "Fields.FormatRegEx.RegEx",
//                                    "Fields.FormatRegEx.ReplacementRegEx"
//                                })
//                            .ConfigureAwait(false);
//        }


//        public void AutoDetect()
//        {
//            var invoices = InvoicesRepository.Instance.GetInvoicesByExpression("All",
//                new List<string> {"RegEx.RegEx", "TemplateIdentificatonRegEx.OCR_RegularExpressions"}).Result.ToList();

//            var selected = new List<Invoices>();
//            var pdfText = TXTViewerViewModel.Instance.PDFText;
//            var parts = PartsRepository.Instance.GetPartsByExpression("All",
//                    new List<string> {"Start.RegularExpressions", "End.RegularExpressions"}
//                )
//                .Result.ToList();
//            foreach (var inv in invoices)

//                if (inv.TemplateIdentificatonRegEx.Any())
//                {
//                    if (inv.TemplateIdentificatonRegEx.Any(x => x.OCR_RegularExpressions.Regex.Match(pdfText).Success))
//                        selected.Add(inv);
//                }
//                else
//                {
//                    if (parts.All(x =>
//                        x.TemplateId == inv.Id && x.Start.Any(z => z.RegularExpressions.Regex.IsMatch(pdfText))))
//                        selected.Add(inv);
//                }

//            var res = new StringBuilder();
//            res.Append(
//                $" && \"{selected.Select(x => x.Id.ToString()).DefaultIfEmpty("").Aggregate((o, n) => $"{o}, {n}")}\".Contains(Id)");
//            FilterData(res);
//        }
//    }
//}