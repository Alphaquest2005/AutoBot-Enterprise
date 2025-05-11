using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using CoreEntities.Client.Entities;
using EntryDataQS.Client.Entities;
using Omu.ValueInjecter;
using SalesDataQS.Client.Entities;
using SalesDataQS.Client.Repositories;
using SimpleMvvmToolkit;


namespace WaterNut.QuerySpace.SalesDataQS.ViewModels
{
    public partial class EX9SalesDataDetailsModel : SalesDataDetailViewModel_AutoGen  
	{
        private static readonly EX9SalesDataDetailsModel instance;
        static EX9SalesDataDetailsModel()
        {
            instance = new EX9SalesDataDetailsModel(){ViewCurrentSalesData = true};
        }

        public static EX9SalesDataDetailsModel Instance
        {
            get { return instance; }
        }
        private EX9SalesDataDetailsModel()
        {
            RegisterToReceiveMessages<AsycudaDocumentSetEx>(CoreEntities.MessageToken.CurrentAsycudaDocumentSetExChanged, OnCurrentAsycudaDocumentSetExChanged);
           
        }

        private void OnCurrentAsycudaDocumentSetExChanged(object sender, NotificationEventArgs<AsycudaDocumentSetEx> e)
        {
            if (e.Data != null)
            {
                FilterData();
            }
        }

       
        bool _zeroPriceItems = false;
        public bool ZeroPriceItems
        {
            get
            {
                return _zeroPriceItems;
            }
            set
            {
                _zeroPriceItems = value;
                FilterData();

            }
        }

        bool _viewDocData = false;
        public bool ViewDocData
        {
            get
            {
                return _viewDocData;
            }
            set
            {
                _viewDocData = value;
                FilterData();
          
            }
        }

        bool _viewAll = false;
        public new bool ViewAll
        {
            get
            {
                return _viewAll;
            }
            set
            {
                _viewAll = value;
                if (_viewAll == true)
                {
                    ViewAll();
                    ViewDocData = false;
                    ZeroPriceItems = false;
                    NullItems = false;
                }
                NotifyPropertyChanged(x => ViewAll);
            }
        }

        bool _nullItems = false;
        public bool NullItems
        {
            get
            {
                return _nullItems;
            }
            set
            {
                _nullItems = value;
                FilterData();
               
            }
        }

        public override void FilterData()
        {
            var res = GetAutoPropertyFilterString();

            if (ViewDocData == true)
            {
                if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx != null)
                {
                    
                    vloader.SetNavigationExpression("AsycudaDocumentSets",
                        $"AsycudaDocumentSetId = {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId}");
                    //res.Append(string.Format(@" && AsycudaDocumentSetId == {0}", CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId));
                }
            }

            if (ZeroPriceItems == true)
            {
                res.Append(@" && Cost == 0");
            }

            if (NullItems == true)
            {
                res.Append(@" && TariffCode == null");
            }
            
            FilterData(res);
        }

        internal async Task SaveEx9SalesDetail(SalesDataDetail salesDataDetail)
        {
           await
                 SalesDataDetailRepository.Instance.SaveSalesDataDetail(salesDataDetail).ConfigureAwait(false);
            MessageBus.Default.BeginNotify(MessageToken.SalesDataDetailsChanged, null,
                         new NotificationEventArgs(MessageToken.SalesDataDetailsChanged));


            MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}