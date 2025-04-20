namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public static partial class InvoiceProcessingUtils
    {
        public static string CreateEmail(string file, EmailDownloader.Client client, string error, List<Line> failedlst,
            FileInfo fileInfo, string txtFile)
        {
            var body = $"Hey,\r\n\r\n {error}-'{fileInfo.Name}'.\r\n\r\n\r\n" +
                       $"{(failedlst.Any() ? failedlst.FirstOrDefault()?.OCR_Lines.Parts.Invoices.Name + "\r\n\r\n\r\n" : "")}" +
                       $"{failedlst.Select(x => $"Line:{x.OCR_Lines.Name} - RegId: {x.OCR_Lines.RegularExpressions.Id} - Regex: {x.OCR_Lines.RegularExpressions.RegEx} - Fields: {x.FailedFields.SelectMany(z => z.ToList()).SelectMany(z => z.Value.ToList()).Select(z => $"{z.Key.fields.Key} - '{z.Key.fields.Field}'").DefaultIfEmpty(string.Empty).Aggregate((o, c) => o + "\r\n\r\n" + c)}").DefaultIfEmpty(string.Empty).Aggregate((o, c) => o + "\r\n" + c)}\r\n\r\n" +
                       "Thanks\r\n" +
                       "Thanks\r\n" +
                       $"AutoBot\r\n" +
                       $"\r\n" +
                       $"\r\n" +
                       CommandsTxt
                ;
            EmailDownloader.EmailDownloader.SendEmail(client, null, "Invoice Template Not found!",
                EmailDownloader.EmailDownloader.GetContacts("Developer"), body, new[] { file, txtFile });
            return body;
        }
    }
}