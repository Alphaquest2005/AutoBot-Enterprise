using System.Collections.Generic;

namespace WaterNut.DataSpace;

internal class LicenceDocItem
{
    public int Item_Id { get; set; }
    public List<string> Details { get; set; }
    public string TariffCode { get; set; }
    public double? ItemQuantity { get; set; }
    public int? AsycudaDocumentId { get; set; }
}