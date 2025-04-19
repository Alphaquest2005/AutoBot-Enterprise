
namespace WaterNut.DataSpace
{
    public class SubItemsDS 
    {
        private static readonly SubItemsDS instance;
        static SubItemsDS()
        {
            instance = new SubItemsDS();
        }

        public static SubItemsDS Instance
        {
            get { return instance; }
        }
        
    }
}
