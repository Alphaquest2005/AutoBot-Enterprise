using Core.Common.Extensions;

namespace WaterNut.DataSpace;

public partial class Invoice
{
    private static BetterExpando CreateOrGetDitm(Part part, Line line, int i, BetterExpando itm,
        ref IDictionary<string, object> ditm, List<IDictionary<string, object>> lst)
    {
        if (part.OCR_Part.RecuringPart != null && part.OCR_Part.RecuringPart.IsComposite == false)
        {
            if (line.OCR_Lines?.IsColumn == true)
            {
                if (i > table[line.OCR_Lines.Fields.First().EntityType]
                        .Count - 1)
                {
                    itm = new BetterExpando();
                    if (line.OCR_Lines.IsColumn == true)
                    {
                        table[line.OCR_Lines.Fields.First().EntityType].Add(itm);
                    }
                }
                else
                {
                    itm = table[line.OCR_Lines.Fields.First().EntityType][i];
                }
            }
            else
            {
                itm = (BetterExpando)lst.ElementAtOrDefault(i) ?? new BetterExpando();
            }


            ditm = ((IDictionary<string, object>)itm);
        }

        return itm;
    }
}