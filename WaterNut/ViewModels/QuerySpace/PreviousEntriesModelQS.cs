using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using AllocationQS.Client.Entities;
using PreviousDocumentQS.Client.Entities;
using PreviousDocumentQS.Client.Repositories;
using SimpleMvvmToolkit;

namespace WaterNut.QuerySpace.PreviousDocumentQS.ViewModels
{
    public partial class PreviousDocumentItemsModel : PreviousDocumentItemViewModel_AutoGen
	{
         private static readonly PreviousDocumentItemsModel instance;
         static PreviousDocumentItemsModel()
        {
            instance = new PreviousDocumentItemsModel()
            {
                EntryTimeStampFilter= DateTime.MinValue,
                StartEntryTimeStampFilter = DateTime.MinValue,
                EndEntryTimeStampFilter= DateTime.MinValue,
                ViewCurrentPreviousDocument = true
                
            };
        }

        public static PreviousDocumentItemsModel Instance
        {
            get { return instance; }
        }
        private PreviousDocumentItemsModel()
		{
            RegisterToReceiveMessages<AsycudaSalesAndAdjustmentAllocationsEx>(AllocationQS.MessageToken.CurrentAsycudaSalesAndAdjustmentAllocationsExChanged, OnCurrentAsycudaSalesAllocationsExChanged);
		}

	    private void OnCurrentAsycudaSalesAllocationsExChanged(object sender, NotificationEventArgs<AsycudaSalesAndAdjustmentAllocationsEx> e)
	    {
	        GetPreviousDocumentItems(e.Data.PreviousItem_Id.GetValueOrDefault());
        }

	    private void OnCurrentAsycudaSalesAllocationsExChanged(object sender, NotificationEventArgs<AsycudaSalesAllocationsEx> e)
        {
            GetPreviousDocumentItems(e.Data.PreviousItem_Id);
        }

	    private void GetPreviousDocumentItems(int previousItem_Id)
	    {
	        vloader.FilterExpression = $"Item_Id = {previousItem_Id}";
	        PreviousDocumentItems.Refresh();
	    }

	    bool _manualMode = false;
        public bool ManualMode
        {
            get
            {
                return _manualMode;
            }
            set
            {
                _manualMode = value;
            }
        }


        internal Task RemoveItem(int p)
        {
            throw new NotImplementedException();
        }

        internal Task RemoveSelectedItems(List<PreviousDocumentItem> list)
        {
            throw new NotImplementedException();
        }

        internal Task SavePreviousDocumentItem(PreviousDocumentItem previousDocumentItem)
        {
            throw new NotImplementedException();
        }
    }
}