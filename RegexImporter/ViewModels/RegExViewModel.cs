using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Common;
using Core.Common.Extensions;
using OCR.Client.Entities;
using OCR.Client.Repositories;
using SimpleMvvmToolkit;
using WaterNut.QuerySpace.OCR;
using WaterNut.QuerySpace.OCR.ViewModels;

using OCR_BaseViewModel = WaterNut.QuerySpace.OCR.ViewModels.BaseViewModel;

using RegularExpressions = OCR.Client.Entities.RegularExpressions;

namespace RegexImporter.ViewModels
{

    public class RegexViewModel : ViewModelBase<RegexViewModel>, IAsyncInitialization
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
            RegisterToReceiveMessages<Invoices>(MessageToken.CurrentInvoicesChanged, OnCurrentOCR_InvoiceExChanged2);
            RegisterToReceiveMessages<Parts>(MessageToken.CurrentPartsChanged, OnOCR_PartExsChanged2);
            RegisterToReceiveMessages<Lines>(MessageToken.CurrentLinesChanged, OnOCR_CurrentLineChanged);
            RegisterToReceiveMessages<Fields>(MessageToken.CurrentFieldsChanged, OnOCR_CurrentFieldsChanged);



        }

        private void OnOCR_CurrentFieldsChanged(object sender, NotificationEventArgs<Fields> e)
        {
            RegexObjects = new List<RegularExpressions>();
            if (e.Data != null)
            {
                dynamic part = new BetterExpando();
                part.Source = e.Data;
                part.Save = (Action<Fields>)(x =>
                {
                    part.Source = FieldsRepository.Instance.UpdateFields(e.Data).Result;
                    UpdateRegexObjects();
                    CurrentRegexObject = part;

                });

                CurrentRegexObject = part;
                var sres = e.Data.FormatRegEx.Select(x => x.RegEx).ToList();
                sres.ForEach(x => x.Name = "Format");
                RegexObjects.AddRange(sres);
                var eres = e.Data.FormatRegEx.Select(x => x.ReplacementRegEx).ToList();
                eres.ForEach(x => x.Name = "Replacement");
                RegexObjects.AddRange(eres);
                CurrentRegex = RegexObjects.FirstOrDefault();

            }
        }

        private void OnOCR_CurrentLineChanged(object sender, NotificationEventArgs<Lines> e)
        {
            if (e.Data != null)
            {
                dynamic line = new BetterExpando();
                line.Source = e.Data;
                line.Save = (Action<Lines>)(x =>
                {
                    line.Source = LinesRepository.Instance.UpdateLines(e.Data).Result;
                    UpdateRegexObjects();
                    CurrentRegexObject = line;
                   
                });

                CurrentRegexObject = line;

                var list = e.Data.RegularExpressions == null ? new List<RegularExpressions>() : new List<RegularExpressions>() { e.Data.RegularExpressions };
                list.ForEach(x => x.Name = "Line");
                RegexObjects = list;
                CurrentRegex = RegexObjects.FirstOrDefault();

            }

            
        }


        private dynamic _currentRegexObject = null;
        public dynamic CurrentRegexObject
        {
            get { return _currentRegexObject; }
            set
            {
                _currentRegexObject = value;
                NotifyPropertyChanged(x => this.CurrentRegexObject);
            }
        }

        private List<RegularExpressions> _regexObjects = null;
        public List<RegularExpressions> RegexObjects
        {
            get { return _regexObjects; }
            set
            {
                _regexObjects = value;
                NotifyPropertyChanged(x => this.RegexObjects);
            }
        }


        private RegularExpressions _currentRegex = null;
        public RegularExpressions CurrentRegex
        {
            get { return _currentRegex; }
            set
            {
                _currentRegex = value;
                NotifyPropertyChanged(x => this.CurrentRegex);
            }
        }




        private Match _currentMatch = null;
        
        

        public Match CurrentMatch
        {
            get { return _currentMatch; }
            set
            {
                _currentMatch = value;
                BeginSendMessage(MessageToken.CurrentMatch,
                    new NotificationEventArgs<Match>(MessageToken.CurrentMatch, _currentMatch));
                NotifyPropertyChanged(x => this.CurrentMatch);
            }
        }

        private Group _currentGroup;
        public Group CurrentGroup
        {
            get => _currentGroup;
            set
            {
                _currentGroup = value;
                BeginSendMessage(MessageToken.CurrentGroup,
                    new NotificationEventArgs<Group>(MessageToken.CurrentGroup, _currentGroup));
                NotifyPropertyChanged(x => this.CurrentGroup);
            }
        }

        private Capture _currentCapture;
        public Capture CurrentCapture
        {
            get => _currentCapture;
            set
            {
                _currentCapture = value;
                BeginSendMessage(MessageToken.CurrentCapture,
                    new NotificationEventArgs<Capture>(MessageToken.CurrentCapture, _currentCapture));
                NotifyPropertyChanged(x => this.CurrentCapture);

            }
        }

        private async void OnCurrentOCR_InvoiceExChanged2(object sender,
            NotificationEventArgs<Invoices> e)
        {
            RegexObjects = new List<RegularExpressions>();
            if (e.Data != null)
            {
                dynamic part = new BetterExpando();
                part.Source = e.Data;
                part.Save = (Action<Invoices>)(x =>
                {
                    part.Source = InvoicesRepository.Instance.UpdateInvoices(e.Data).Result;
                    UpdateRegexObjects();
                    CurrentRegexObject = part;

                });

                CurrentRegexObject = part;
                var sres = e.Data.RegEx.Select(x => x.RegEx).ToList();
                sres.ForEach(x => x.Name = "CutOut");
                RegexObjects.AddRange(sres);
                var eres = e.Data.InvoiceIdentificatonRegEx.Select(x => x.OCR_RegularExpressions).ToList();
                eres.ForEach(x => x.Name = "Identification ");
                RegexObjects.AddRange(eres);
                CurrentRegex = RegexObjects.FirstOrDefault();

            }


        }

        private async void OnOCR_PartExsChanged2(object sender, NotificationEventArgs<Parts> e)
        {
            RegexObjects = new List<RegularExpressions>();
            if (e.Data != null)
            {
                dynamic part = new BetterExpando();
                part.Source = e.Data;
                part.Save = (Action<Lines>)(x =>
                {
                    part.Source = PartsRepository.Instance.UpdateParts(e.Data).Result;
                    UpdateRegexObjects();
                    CurrentRegexObject = part;

                });

                CurrentRegexObject = part;
                var sres = e.Data.Start.Select(x => x.RegularExpressions).ToList();
                sres.ForEach(x => x.Name = "Start");
                RegexObjects.AddRange(sres);
                var eres = e.Data.End.Select(x => x.RegularExpressions).ToList();
                eres.ForEach(x => x.Name = "End");
                RegexObjects.AddRange(eres);
                CurrentRegex = RegexObjects.FirstOrDefault();

            }


        }

        private void UpdateRegexObjects()
        {
            var uRegObj = new List<RegularExpressions>();
            foreach (var reg in RegexObjects)
            {
                uRegObj.Add(RegularExpressionsRepository.Instance.UpdateRegularExpressions(reg).Result);
            }

            RegexObjects = uRegObj;
            CurrentRegex = RegexObjects.FirstOrDefault();
        }


        public Task Initialization { get; }
        public List<Match> Matches { get; set; } = new List<Match>();

     
    }
}
