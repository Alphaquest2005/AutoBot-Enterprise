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
    }
}