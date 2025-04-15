using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using CoreEntities.Business.Entities;
using EntryDataDS.Business.Entities; // Needed for EntryDataDSContext
using WaterNut.Business.Services.Utils; // For PDFUtils
using WaterNut.DataSpace; // For FileTypes, FileTypeManager
using AutoBot; // For PDFUtils if namespace is AutoBot

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class PDFImportTests
    {
        [SetUp]
        public void SetUp()
        {
             // Ensure test settings are applied and DB is cleared before each test in this fixture
             Infrastructure.Utils.SetTestApplicationSettings(3); 
             Infrastructure.Utils.ClearDataBase();
        }

        [Test]
        public async Task CanImportAmazonMultiSectionInvoice() 
        {
            try
            {
                //if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true); // Skip if not test settings
                // Use the absolute path provided by the user
                var testFile = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\Test cases\one amazon with muliple invoice details sections.pdf"; 
                
                if (!File.Exists(testFile))
                {
                    Assert.Warn($"Test file not found: {testFile}");
                    return; // Skip test if file doesn't exist
                }

                var fileTypes = (IEnumerable<CoreEntities.Business.Entities.FileTypes>)FileTypeManager
                    .GetImportableFileType(FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.PDF,
                        testFile)
                    .Where(x => x.Description == "Unknown")
                    .ToList();// Filter specifically for "Unknown" type; // Fully qualified
                if (!fileTypes.Any())
                {
                    Assert.Warn($"No suitable PDF FileType found for: {testFile}");
                    return; // Skip if no matching filetype
                }

                foreach (var fileType in fileTypes)
                {
                    Console.WriteLine($"Testing with FileType: {fileType.Description} (ID: {fileType.Id})"); 
                    // DB is cleared in SetUp
                    await PDFUtils.ImportPDF(new FileInfo[]{new FileInfo(testFile)}, fileType).ConfigureAwait(false);


                    using (var ctx = new EntryDataDSContext())
                    {
                        // Basic assertion: Check if *any* data was imported. 
                        Assert.That(ctx.ShipmentInvoice.Any(x => x.InvoiceNo == "114-7827932-2029910"), Is.True, "No ShipmentInvoice created."); 
                        Assert.That(ctx.ShipmentInvoiceDetails.Count(x => x.Invoice.InvoiceNo == "114-7827932-2029910") > 2, Is.True, "No ShipmentInvoiceDetails created."); 
                        Console.WriteLine($"Import successful for FileType {fileType.Id}. Invoices: {ctx.ShipmentInvoice.Count()}, Details: {ctx.ShipmentInvoiceDetails.Count()}"); 
                    }
                }

                Assert.That(true);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR in CanImportAmazonMultiSectionInvoice: {e}");
                Assert.Fail($"Test failed with exception: {e.Message}");
            }
        }

        [Test]
        public async Task CanImportSheinMultiInvoice() 
        {
            try
            {
                //if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true); // Skip if not test settings
                // Use the absolute path provided by the user for Shein PDF
                var testFile = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\Test cases\Shein - multiple invoices for one shipment .pdf"; 
                
                if (!File.Exists(testFile))
                {
                    Assert.Warn($"Test file not found: {testFile}");
                    return; // Skip test if file doesn't exist
                }

                var fileTypes = (IEnumerable<CoreEntities.Business.Entities.FileTypes>)FileTypeManager
                    .GetImportableFileType(FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.PDF,
                        testFile)
                    .Where(x => x.Description == "Unknown")
                    .ToList();// Filter specifically for "Unknown" type; // Fully qualified
                if (!fileTypes.Any())
                {
                    Assert.Warn($"No suitable PDF FileType found for: {testFile}");
                    return; // Skip if no matching filetype
                }

                foreach (var fileType in fileTypes)
                {
                    Console.WriteLine($"Testing with FileType: {fileType.Description} (ID: {fileType.Id})"); 
                    // DB is cleared in SetUp
                   await PDFUtils.ImportPDF(new FileInfo[]{new FileInfo(testFile)}, fileType).ConfigureAwait(false);


                    using (var ctx = new EntryDataDSContext())
                    {
                        // Basic assertion: Check if *any* data was imported. 
                        Assert.That(ctx.ShipmentInvoice.Any(), Is.True, "No ShipmentInvoice created."); 
                        Assert.That(ctx.ShipmentInvoiceDetails.Any(), Is.True, "No ShipmentInvoiceDetails created."); 
                        Console.WriteLine($"Import successful for FileType {fileType.Id}. Invoices: {ctx.ShipmentInvoice.Count()}, Details: {ctx.ShipmentInvoiceDetails.Count()}"); 
                    }
                }

                Assert.That(true);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR in CanImportSheinMultiInvoice: {e}");
                Assert.Fail($"Test failed with exception: {e.Message}");
            }
        }

    } // End Class
} // End Namespace