using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCR.Client.Entities;
using OCR.Client.Repositories;
using SimpleMvvmToolkit;
using WaterNut.QuerySpace.OCR;
using WaterNut.QuerySpace.OCR.ViewModels;
using OCR_BaseViewModel = WaterNut.QuerySpace.OCR.ViewModels.BaseViewModel;

namespace RegexImporter.ViewModels
{
    
    public class InvoiceExViewModel : InvoiceViewModel_AutoGen
    {
        private static readonly InvoiceExViewModel instance;

        static InvoiceExViewModel()
        {
            instance = new InvoiceExViewModel()
            {
                
            };
        }

        public static InvoiceExViewModel Instance
        {
            get { return instance; }
        }

        private InvoiceExViewModel()
        {
            RegisterToReceiveMessages<Invoice>(MessageToken.CurrentInvoiceChanged, OnCurrentOCR_InvoiceExChanged2);
            RegisterToReceiveMessages(MessageToken.InvoiceExsChanged, OnOCR_InvoiceExChanged3);
            RegisterToReceiveMessages<Invoice>(MessageToken.InvoiceExsChanged, OnOCR_InvoiceExChanged2);
            RegisterToReceiveMessages<Part>(MessageToken.CurrentPartChanged, OnOCR_PartExsChanged2);



        }

       

        private IEnumerable<Part> _parts = null;
        public IEnumerable<Part> Parts
        {
            get { return _parts; }
            set
            {
                _parts = value;
                NotifyPropertyChanged(x => this.Parts);
            }
        }


        private IEnumerable<Line> _lines = null;
        public IEnumerable<Line> Lines
        {
            get { return _lines; }
            set
            {
                _lines = value;
                NotifyPropertyChanged(x => this.Lines);
            }
        }

        private async void OnOCR_InvoiceExChanged3(object sender, NotificationEventArgs e)
        {
            await RefreshParts().ConfigureAwait(false);
        }

        private async void OnOCR_InvoiceExChanged2(object sender, NotificationEventArgs<Invoice> e)
        {
            await RefreshParts().ConfigureAwait(false);
        }

        private async Task RefreshParts()
        {
            if (OCR_BaseViewModel.Instance.CurrentInvoice != null)
            {
                Parts =
                    await
                        PartRepository.Instance.GetPartByInvoiceId(
                                OCR_BaseViewModel.Instance.CurrentInvoice.Id.ToString())
                            .ConfigureAwait(false);
            }
        }

        private async void OnCurrentOCR_InvoiceExChanged2(object sender,
            NotificationEventArgs<Invoice> e)
        {
            if (e.Data != null)
            {
                Parts =
                    await
                        PartRepository.Instance.GetPartByInvoiceId(
                                OCR_BaseViewModel.Instance.CurrentInvoice.Id.ToString())
                            .ConfigureAwait(false);
                NameFilter = e.Data.Name;
            }
        }

        private async void OnOCR_PartExsChanged2(object sender, NotificationEventArgs<Part> e)
        {
            if (e.Data != null)
            {
                Lines =
                    await
                        LineRepository.Instance.GetLineByPartId(
                                e.Data.Id.ToString())
                            .ConfigureAwait(false);
            }
        }
    }
}
