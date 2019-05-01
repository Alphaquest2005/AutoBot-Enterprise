using DocumentItemDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public class DocumentItemModelDS
    {
        public static int AddFreeText(int itmcnt, xcuda_Item itm, string entryDataId)
        {
            if (BaseDataModel.Instance.CurrentApplicationSettings.GroupEX9 != true)
            {
                if (itmcnt < 5)
                {
                    if (itm.Free_text_1 == null) itm.Free_text_1 = ""; //"Inv.#"
                    itm.Free_text_1 = itm.Free_text_1 + "|" +
                                      string.Format("{0}", entryDataId);
                    //CleanText(allo.EntryDataDetails.EntryDataId));
                }
                else
                {
                    if (itm.Free_text_2 == null) itm.Free_text_2 = ""; //"Inv.#"
                    itm.Free_text_2 = itm.Free_text_2 + "|" +
                                      string.Format("{0}", entryDataId);
                    // CleanText(allo.EntryDataDetails.EntryDataId)); 
                }

                itmcnt += 1;
            }
            if (itm.Free_text_1 != null && itm.Free_text_1.Length > 1)
            {
                if (itm.Free_text_1.Length < 31)
                {
                    itm.Free_text_1 = itm.Free_text_1.Substring(1);
                }
                else
                {
                    itm.Free_text_1 = itm.Free_text_1.Substring(1, 30);
                }
            }


            if (itm.Free_text_2 != null && itm.Free_text_2.Length > 1)
            {
                if (itm.Free_text_2.Length < 21)
                {
                    itm.Free_text_2 = itm.Free_text_2.Substring(1);
                }
                else
                {
                    itm.Free_text_2 = itm.Free_text_2.Substring(1, 20);
                }
            }
            return itmcnt;
        }
    }
}