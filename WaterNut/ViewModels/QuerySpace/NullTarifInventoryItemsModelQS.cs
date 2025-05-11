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
using Core.Common.UI;
using Core.Common.UI.DataVirtualization;
using CoreEntities.Client.Entities;
using EntryDataQS.Client.Entities;
using InventoryQS.Client.Entities;
using InventoryQS.Client.Repositories;
using SimpleMvvmToolkit;
using InventoryItemsEx = InventoryQS.Client.Entities.InventoryItemsEx;


namespace WaterNut.QuerySpace.InventoryQS.ViewModels

{
    public partial class InventoryItemsModel : InventoryItemsExViewModel_AutoGen 
    {
        private static readonly InventoryItemsModel instance;
        static InventoryItemsModel()
        {
            instance = new InventoryItemsModel()
            {
                EndEntryTimeStampFilter = DateTime.MinValue,
                StartEntryTimeStampFilter = DateTime.MinValue,
                EntryTimeStampFilter = DateTime.MinValue
            };
        }

        public static InventoryItemsModel Instance
        {
            get { return instance; }
        }
        private InventoryItemsModel()
		{
            RegisterToReceiveMessages<AsycudaDocument>(CoreEntities.MessageToken.CurrentAsycudaDocumentChanged, OnCurrentAsycudaDocumentChanged);
            RegisterToReceiveMessages<AsycudaDocumentSetEx>(CoreEntities.MessageToken.CurrentAsycudaDocumentSetExChanged, OnCurrentAsycudaDocumentSetChanged);
            RegisterToReceiveMessages<EntryDataEx>(EntryDataQS.MessageToken.CurrentEntryDataExChanged, OnCurrentEntryDataExChanged);
		    RegisterToReceiveMessages<ApplicationSettings>(CoreEntities.MessageToken.CurrentApplicationSettingsChanged, OnCurrentApplicationSettingsChanged);
            // RegisterToReceiveMessages<TariffCategory>(MessageToken.CurrentTariffCategoryChanged, OnCurrentTariffCategoryChanged);
        }

        private void OnCurrentApplicationSettingsChanged(object sender, NotificationEventArgs<ApplicationSettings> e)
        {
            FilterData();
        }

        private void OnCurrentAsycudaDocumentChanged(object sender, NotificationEventArgs<AsycudaDocument> e)
        {
            if (ViewDocSetItems == true) FilterData();
        }

        private void OnCurrentTariffCategoryChanged(object sender, NotificationEventArgs<TariffCategory> e)
        {
            if (e.Data != null)
            {
                vloader.FilterExpression = $@"TariffCodes.TariffCategoryCode == ""{e.Data.TariffCategoryCode}""";
                InventoryItemsEx.Refresh();
                NotifyPropertyChanged(x => InventoryItemsEx);
            }
        }

        private void OnCurrentEntryDataExChanged(object sender, NotificationEventArgs<EntryDataEx> e)
        {
            if (ViewPOItems == true)
            {
                CurrentEntryData = e.Data;
                FilterData();
            }
        }

        private EntryDataEx CurrentEntryData = null;

        private void OnCurrentAsycudaDocumentSetChanged(object sender, NotificationEventArgs<AsycudaDocumentSetEx> e)
        {
            if(ViewDocSetItems == true) FilterData();
        }

       

        bool _viewNullTariff = false;
        public bool ViewNullTariff
        {
            get
            {
                return _viewNullTariff;
            }
            set
            {
                _viewNullTariff = value;
                FilterData();
                NotifyPropertyChanged(x => this.ViewNullTariff);
            }
        }

        bool _viewUnknownTariff = false;
        public bool ViewUnknownTariff
        {
            get
            {
                return _viewUnknownTariff;
            }
            set
            {
                _viewUnknownTariff = value;
                FilterData();
                NotifyPropertyChanged(x => this.ViewUnknownTariff);
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
                NotifyPropertyChanged(x => this.ZeroPriceItems);
            }
        }



        bool _viewDocSetItems = false;
        public bool ViewDocSetItems
        {
            get
            {
                return _viewDocSetItems;
            }
            set
            {
                _viewDocSetItems = value;
                FilterData();
                NotifyPropertyChanged(x => this.ViewDocSetItems);
            }
        } 


        bool _viewPOItems = false;
        public bool ViewPOItems
        {
            get
            {
                return _viewPOItems;
            }
            set
            {
                _viewPOItems = value;
                FilterData();
                NotifyPropertyChanged(x => this.ViewPOItems);
            }
        }

        bool _viewSalesItems = false;
        public bool ViewSalesItems
        {
            get
            {
                return _viewSalesItems;
            }
            set
            {
                _viewSalesItems = value;
                FilterData();
                NotifyPropertyChanged(x => this.ViewSalesItems);
            }
        }

        public override void FilterData()
        {
            var res = GetAutoPropertyFilterString();
            res.Append($" && ApplicationSettingsId == {CoreEntities.ViewModels.BaseViewModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}");
            vloader.ClearNavigationExpression();

            var navExp = new StringBuilder();

            if (ViewNullTariff == true)
            {
                res.Append(" && TariffCode == null");
            }

            if (ViewUnknownTariff == true)
            {
                res.Append(" && TariffCode != null && TariffCodes == null");
            }

            if (ViewPOItems == true)
            {
                if (CurrentEntryData != null)
                {
                    navExp.Append($"&& EntryDataId == \"{CurrentEntryData.InvoiceNo}\"");
                   
                }
            }
            if (ViewDocSetItems == true)
            {
                if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocument != null)
                {
                    navExp.Append(
                        $"&& CNumber == \"{CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocument.CNumber}\"");
                }

                if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx != null)
                {
                   navExp.Append(
                       $"&& AsycudaDocumentSetId == \"{CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId}\"");
                }
            }

            if (navExp.Length> 0)
            {
                vloader.SetNavigationExpression("EntryDataDetailsEx", navExp.Remove(0,3).ToString());
            }

           FilterData(res);



        }




        internal async Task AssignTariffToItms(List<int> list)
        {
            StatusModel.Timer("Assign Tariff to Items");
            await InventoryItemsExRepository.Instance.AssignTariffToItms(list,
                InventoryQS.ViewModels.BaseViewModel.Instance.CurrentTariffCodes.TariffCode).ConfigureAwait(false);
            StatusModel.StopStatusUpdate();
            MessageBus.Default.BeginNotify(QuerySpace.InventoryQS.MessageToken.InventoryItemsExFilterExpressionChanged, null, new NotificationEventArgs(QuerySpace.InventoryQS.MessageToken.InventoryItemsExFilterExpressionChanged));

        }



        internal async Task SaveInventoryItem(InventoryItemsEx inv)
        {
            if (inv == null) return;
            StatusModel.Timer("Saving Inventory Item");
            if (inv.TariffCode == "") inv.TariffCode = null;
            await InventoryItemsExRepository.Instance.SaveInventoryItemsEx(inv).ConfigureAwait(false);
            StatusModel.StopStatusUpdate();
            MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}