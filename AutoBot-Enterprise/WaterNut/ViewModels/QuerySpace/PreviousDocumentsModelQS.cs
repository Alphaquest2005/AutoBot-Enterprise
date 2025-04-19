using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks; // Added for Task
using System.Collections.ObjectModel;
using CoreEntities.Client.Entities;
using PreviousDocumentQS.Client.Repositories;
using PreviousDocumentQS.Client.Entities;
using SimpleMvvmToolkit;
using WaterNut.QuerySpace.PreviousDocumentQS.ViewModels;
using AllocationQS.Client.Entities;


namespace WaterNut.QuerySpace.PreviousDocumentQS.ViewModels
{
    public partial class PreviousDocumentModel : PreviousDocumentViewModel_AutoGen 
	{
        private static readonly PreviousDocumentModel instance;
        static PreviousDocumentModel()
        {
            instance = new PreviousDocumentModel()
            {
                EffectiveRegistrationDateFilter = DateTime.MinValue,
                EndEffectiveRegistrationDateFilter = DateTime.MinValue,
                RegistrationDateFilter = DateTime.MinValue,
                StartEffectiveRegistrationDateFilter = DateTime.MinValue
                
                
            };
        }

        public static PreviousDocumentModel Instance
        {
            get { return instance; }
        }
        private PreviousDocumentModel()
		{
			RegisterToReceiveMessages<AsycudaDocumentSetEx>(CoreEntities.MessageToken.CurrentAsycudaDocumentSetExChanged, OnCurrentAsycudaDocumentSetChanged);
           
            RegisterToReceiveMessages<PreviousDocumentItem>(MessageToken.CurrentPreviousDocumentItemChanged, OnCurrentPreviousDocumentItemChanged);
  }

        // Change signature to async void for event handler
        private async void OnCurrentPreviousDocumentItemChanged(object sender, NotificationEventArgs<PreviousDocumentItem> e)
        {
            if (e.Data != null && ManualMode == false)
            {
                if (e.Data == null || e.Data.ASYCUDA_Id == null)
                {
                    vloader.FilterExpression = null;
                }
                else
                {

                    vloader.FilterExpression = $"ASYCUDA_Id == {e.Data.ASYCUDA_Id}";
                }

                PreviousDocuments.Refresh();
                NotifyPropertyChanged(x => this.PreviousDocuments);

                //BaseViewModel.Instance.CurrentPreviousDocument = await PreviousDocumentQS.ViewModels.BaseViewModel.Instance. e.Data.PreviousDocument;
            }
        }

       
        private new void OnCurrentAsycudaDocumentSetChanged(object sender, NotificationEventArgs<AsycudaDocumentSetEx> e)
        {
            if (e.Data != null && ManualMode == false )
            {
                //base.get filterstring
            }
        }

        public bool ManualMode { get; set; }
    }
}