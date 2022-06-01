namespace WaterNut.Business.Services.Importers
{
    public class RawEntryData
    {
        public string ItemNumber { get; }
        public string ItemDescription { get; set; }
        public string TariffCode { get; }
        public int InventoryItemId { get; set; }
        public string ItemAlias { get; }
        public string SupplierItemNumber { get; }
        public string SupplierItemDescription { get; }
        public string SourceRow { get; }
        public string EntryDataId { get; }
    }
}