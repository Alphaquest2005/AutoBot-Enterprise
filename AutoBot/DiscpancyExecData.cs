using System;

namespace AutoBotUtilities
{
    internal class DiscpancyExecData
    {
        public string InvoiceNo { get; internal set; }
        public DateTime InvoiceDate { get; internal set; }
        public string ItemNumber { get; internal set; }
        public double? ReceivedQty { get; internal set; }
        public double? InvoiceQty { get; internal set; }
        public string CNumber { get; internal set; }
        public string Status { get; internal set; }
        public string xCNumber { get; internal set; }
        public int? xLineNumber { get; internal set; }
        public string xRegistrationDate { get; internal set; }
        public string EmailId { get; set; }
        public int ApplicationSettingsId { get; set; }
        public int AsycudaDocumentSetId { get; set; }
    }
}