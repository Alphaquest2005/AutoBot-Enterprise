using AdjustmentQS.Client.Entities;

namespace WaterNut.QuerySpace.AdjustmentQS.ViewModels
{
    public class ShortAllocationsModel : ShortAllocationViewModel_AutoGen
    {
        private static readonly ShortAllocationsModel instance;
        static ShortAllocationsModel()
        {
            instance = new ShortAllocationsModel() { ViewCurrentAdjustmentShort = true };
        }



        public static ShortAllocationsModel Instance
        {
            get { return instance; }
        }

        internal override void OnCurrentAdjustmentShortChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs<AdjustmentShort> e)
        {
            if (ViewCurrentAdjustmentShort == false) return;
            if (e.Data == null || e.Data.EntryDataDetailsId == null)
            {
                vloader.FilterExpression = "None";
            }
            else
            {
                vloader.FilterExpression = $"EntryDataDetailsId == {e.Data.EntryDataDetailsId.ToString()} " +
                                           $"&& AsycudaDocumentSetId == \"{CoreEntities.ViewModels.BaseViewModel.Instance?.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId}\"";
            }

            ShortAllocations.Refresh();
            NotifyPropertyChanged(x => this.ShortAllocations);
            // SendMessage(MessageToken.ShortAllocationsChanged, new NotificationEventArgs(MessageToken.ShortAllocationsChanged));
        }
    }
}