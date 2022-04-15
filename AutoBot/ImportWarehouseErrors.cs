using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CoreEntities.Business.Entities;
using WaterNut.DataSpace;

namespace AutoBot
{
    public class ImportWarehouseErrorsUtils
    {
        public static void ImportWarehouseErrors()
        {
            try
            {
                var warehouseRegex = new List<(string Name, string RegEx, Action<Match> Action)>()
                {
                    ("Cancelled Previous Document",
                        @"Previous declaration\s(?<Year>\d{4})\s(?<Office>\w+)\sC(?<pCNumber>\d+)\shas been cancelled. Please, update previous document linked with item\s(?<LineNumber>)",
                        (mat) => SaveCancelledErrors(mat))
                    ,

                    ("Not Enough Remaining Balance",
                        @"not [enough balance remaining in decltaration]+ \""(?<Year>\d{4}) (?<Office>\w{5})[\sC]+(?<pCNumber>\d+)[\s-]+(?<pLineNumber>\d+)[%,\""”]+(?<Error>.+?)($|\r)",
                        (mat) => SaveBalanceErrors(mat)),

                    ("Not Enough Remaining Weight",
                        @"([On previous document]{10,}[\s\""“]+(?<Year>\d{4}) (?<Office>\w{5})[\. C]+(?<pCNumber>\d+)[\s-]+(?<pLineNumber>\d+)[%,\""”]*(?<Error>.+?)($|\r))",
                        (mat) => SaveWeightErrors(mat))
                    ,

                    ("Changed Tariff Code",
                        @"([Goods (Tarifficountry) in previous declaration]{10,}[\s\""“]+(?<Year>\d{4}) (?<Office>\w{5})[\. C]+(?<pCNumber>\d+)[\s-]+(?<pLineNumber>\d+)[%,\""”]*(?<Error>.+?)($|\r))" ,
                        (mat) => SaveTariffCode(mat))
                };



                Console.WriteLine("Importing Warehouse errors");
                var directoryName = BaseDataModel.CurrentSalesInfo().Item4;
                var attachments = Directory.GetFiles(directoryName, "*.png").ToArray();
                if (!attachments.Any()) return;


                var imageTxt = InvoiceReader.GetImageTxt(directoryName);
                var errorLst = new List<string>();

                foreach (var reg in warehouseRegex)
                {
                    var errors = Regex.Matches(imageTxt, reg.RegEx,
                        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
                    foreach (Match error in errors)
                    {
                        reg.Action.Invoke(error);
                        errorLst.Add(error.Value);
                    }
                }

                if (errorLst.Any())
                {
                    var body = "Ex-Warehouse Errors Found: \r\n" +
                               "Errors are as follows: \r\n" +
                               $"\t {errorLst.Aggregate((old, current) => old + "\r\n\t" + current)}\r\n" +
                               $"\r\nPlease Check the spreadsheet or inform Joseph Bartholomew if this is an Error.\r\n" +
                               $"Regards,\r\n" +
                               $"AutoBot";
                    var contacts = new CoreEntitiesContext().Contacts.Where(x => x.Role == "Broker" && x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Select(x => x.EmailAddress).ToArray();
                    EmailDownloader.EmailDownloader.SendEmail(Utils.Client, directoryName, $"Exwarehouse Errors Found for: {BaseDataModel.CurrentSalesInfo().Item3.Declarant_Reference_Number}", contacts, body, attachments);
                    foreach (var att in attachments)
                    {
                        File.Delete(att);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static void SaveWeightErrors(Match mat)
        {
            MarkXWarehouseError(mat);
        }

        private static void MarkXWarehouseError(Match mat)
        {
            try
            {
                new CoreEntitiesContext().Database.ExecuteSqlCommand($@"UPDATE xcuda_Item
                                                SET         xWarehouseError = N'{mat.Groups["Error"]}'
                                                FROM    AsycudaDocument INNER JOIN
                                                                 xcuda_Item ON AsycudaDocument.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id
                                                WHERE (AsycudaDocument.CNumber = N'{mat.Groups["pCNumber"]}') and year(AsycudaDocument.RegistrationDate) = {mat.Groups["Year"]} and AsycudaDocument.Customs_clearance_office_code = '{mat.Groups["Office"]}' AND (xcuda_Item.LineNumber = {mat.Groups["pLineNumber"]})");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void SaveTariffCode(Match mat)
        {
            MarkXWarehouseError(mat);
        }

        private static void SaveBalanceErrors(Match mat)
        {
            MarkXWarehouseError(mat);
        }

        private static void SaveCancelledErrors(Match mat)
        {
            try
            {
                new CoreEntitiesContext().Database.ExecuteSqlCommand($@"UPDATE       xcuda_ASYCUDA_ExtendedProperties
                                                SET                Cancelled = 1
                                                FROM            AsycudaDocumentBasicInfo INNER JOIN
                                                                         xcuda_ASYCUDA_ExtendedProperties ON AsycudaDocumentBasicInfo.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
                                                WHERE        (AsycudaDocumentBasicInfo.CNumber = {mat.Groups["pCNumber"]} and AsycudaDocumentBasicInfo.ApplicationSettingsId = {BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId})");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}