using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common;
using OCR.Client.Entities;
using OCR.Client.Repositories;
using SimpleMvvmToolkit;
using WaterNut.QuerySpace.OCR;
using WaterNut.QuerySpace.OCR.ViewModels;
using OCR_BaseViewModel = WaterNut.QuerySpace.OCR.ViewModels.BaseViewModel;

namespace RegexImporter.ViewModels
{
    
    public class RegexViewModel :  ViewModelBase<RegexViewModel>, IAsyncInitialization
    {
        private static readonly RegexViewModel instance;

        static RegexViewModel()
        {
            instance = new RegexViewModel()
            {
                
            };
        }

        public static RegexViewModel Instance
        {
            get { return instance; }
        }

        private RegexViewModel()
        {
            RegisterToReceiveMessages<Invoice>(MessageToken.CurrentInvoiceChanged, OnCurrentOCR_InvoiceExChanged2);
            RegisterToReceiveMessages<Part>(MessageToken.CurrentPartChanged, OnOCR_PartExsChanged2);
            RegisterToReceiveMessages<Line>(MessageToken.CurrentLineChanged, OnOCR_CurrentLineChanged);



        }

        private void OnOCR_CurrentLineChanged(object sender, NotificationEventArgs<Line> e)
        {
            if (e.Data != null)
            {
                CurrentObject = e.Data;
            }
        }


        private dynamic _currentObject = null;
        public dynamic CurrentObject
        {
            get { return _currentObject; }
            set
            {
                _currentObject = value;
                NotifyPropertyChanged(x => this.CurrentObject);
            }
        }




        private async void OnCurrentOCR_InvoiceExChanged2(object sender,
            NotificationEventArgs<Invoice> e)
        {
            if (e.Data != null)
            {
                CurrentObject = e.Data;
            }
        }

        private async void OnOCR_PartExsChanged2(object sender, NotificationEventArgs<Part> e)
        {
            if (e.Data != null)
            {
                CurrentObject = e.Data;
            }
        }

        public Task Initialization { get; }
    }
}
