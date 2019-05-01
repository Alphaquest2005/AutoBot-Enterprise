using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.ComponentModel;
using System.Linq;
using System.Data.Entity;
using System.Collections.ObjectModel;
using PreviousDocumentQS.Client.Entities;
using PreviousDocumentQS.Client.Repositories;
using AllocationQS.Client.Entities;
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
	        GetPreviousDocumentItems(e.Data.PreviousItem_Id);
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


        internal async Task RemoveItem(int p)
        {
            throw new NotImplementedException();
        }

        internal async Task RemoveSelectedItems(List<PreviousDocumentItem> list)
        {
            throw new NotImplementedException();
        }

        internal async Task SavePreviousDocumentItem(PreviousDocumentItem previousDocumentItem)
        {
            throw new NotImplementedException();
        }
    }
}