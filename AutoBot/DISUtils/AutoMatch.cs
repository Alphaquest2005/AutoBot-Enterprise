using WaterNut.DataSpace; // Assuming BaseDataModel is here
// Need using statement for AdjustmentService if it's in a different namespace

namespace AutoBot
{
    public partial class DISUtils
    {
        public static void AutoMatch()
        {
           // new AdjustmentService().AutoMatch(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, null).Wait(); // CS0246 - Assuming AdjustmentService exists
        }
    }
}