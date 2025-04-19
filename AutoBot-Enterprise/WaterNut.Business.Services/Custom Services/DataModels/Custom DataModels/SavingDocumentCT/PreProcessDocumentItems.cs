using WaterNut.Business.Entities;

namespace WaterNut.DataSpace
{
    public class PreProcessDocumentItems
    {
        public static void Execute(DocumentCT cdoc)
        {
            foreach (var item in cdoc.DocumentItems)
            {
                item.ASYCUDA_Id = cdoc.Document.ASYCUDA_Id;
                item.LineNumber = cdoc.DocumentItems.IndexOf(item) + 1;
                if (item.xcuda_PreviousItem != null)
                {
                    item.xcuda_PreviousItem.ASYCUDA_Id = cdoc.Document.ASYCUDA_Id;
                    item.xcuda_PreviousItem.Current_item_number = item.LineNumber;
                }
            }
        }
    }
}