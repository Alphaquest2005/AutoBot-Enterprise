using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using InventoryQS.Client.Entities;
using InventoryQS.Client.Repositories;
using SimpleMvvmToolkit;


namespace WaterNut.QuerySpace.InventoryQS.ViewModels
{
    public partial class TariffCodesModel : TariffCodesViewModel_AutoGen
	{
         private static readonly TariffCodesModel instance;
         static TariffCodesModel()
        {
            instance = new TariffCodesModel {ViewCurrentTariffCategory = true};
        }

         public static TariffCodesModel Instance
        {
            get { return instance; }
        }
        private TariffCodesModel()
		{
           RegisterToReceiveMessages<InventoryItemsEx>(MessageToken.CurrentInventoryItemsExChanged, OnCurrentInventoryItemsExChanged2);
        }

        private void OnCurrentInventoryItemsExChanged2(object sender, NotificationEventArgs<InventoryItemsEx> e)
        {
             if (e.Data != null && e.Data.TariffCode != null)
                {
                    TariffCodeFilter = e.Data.TariffCode;
                 
                }              
                
        }

        public async Task ValidateExistingTariffCodes()
        {
            if (CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx == null)
            {
                MessageBox.Show("Please select Document Set.");
            }

            await
                InventoryItemsExRepository.Instance.ValidateExistingTariffCodes(
                    CoreEntities.ViewModels.BaseViewModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId)
                    .ConfigureAwait(false);

            MessageBox.Show("Complete","Asycuda Toolkit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }






        internal Task AssignTariffCodeToItm()
        {
            if (BaseViewModel.Instance.CurrentInventoryItemsEx == null)
            {
                MessageBox.Show("Please select Inventory Item");
                return Task.CompletedTask;
            }
            if (BaseViewModel.Instance.CurrentTariffCodes == null)
            {
                MessageBox.Show("Please select TariffCode");
                return Task.CompletedTask;
            }
            MessageBus.Default.BeginNotify(MessageToken.CurrentInventoryItemsExChanged, this, new NotificationEventArgs(MessageToken.CurrentInventoryItemsExChanged));
            return Task.CompletedTask;
        }
    }
}