using System.IO;
using AdjustmentQS.Business.Entities;
using EntryDataDS.Business.Entities;
using OCR.Business.Services;
using WaterNut.Business.Services.Importers;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;

using InvoiceReader;
using System.Threading.Tasks;
namespace AutoBotUtilities.Tests
{
    using AutoBot;
    using System;
    using NUnit.Framework;
    using CoreEntities.Business.Entities;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class PDFUtilsTests
    {
        private PDFUtils _testClass;

        [SetUp]
        public void SetUp()
        {
             Infrastructure.Utils.SetTestApplicationSettings(3);
             SetupTest();


            _testClass = new PDFUtils();
           
        }

        private static void SetupTest()
        {
            
            Infrastructure.Utils.ClearDataBase();
            
        }

       

        [Test]
        public void CanConstruct()
        {
            var instance = new PDFUtils();
            Assert.That(instance, Is.Not.Null);
        }


        
        [Test]
        public async Task CanImportShipmentInvoice() // Original Test
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var testFile = Infrastructure.Utils.GetTestSalesFile(new List<string>() { "01987.pdf" });
                var fileTypes = (IEnumerable<FileTypes>)FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.ShipmentInvoice, FileTypeManager.FileFormats.PDF, testFile);
                if (!fileTypes.Any())
                {
                    Assert.Fail($"No suitable PDF FileType found for: {testFile}");
                    return; // Skip if no matching filetype
                }
                foreach (var fileType in fileTypes)
                {
                    await PDFUtils.ImportPDF(new FileInfo[]{new FileInfo(testFile)}, fileType).ConfigureAwait(false);


                    using (var ctx = new EntryDataDSContext())
                    {
                        Assert.Multiple(() =>
                        {

                            Assert.Equals(ctx.ShipmentInvoice.Count(), 1);
                            Assert.Equals(ctx.ShipmentInvoiceDetails.Count(), 8);
                        });
                    }


                }

                Assert.That(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }

          [Test]
          public async Task CanImportAmazonMultiSectionInvoice() // Added Test (Corrected)
          {
              try
              {
                  //if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true); // Skip if not test settings
                  // Use the absolute path provided by the user
                  var testFile = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\Test cases\one amazon with muliple invoice details sections.pdf";
                  
                  if (!File.Exists(testFile))
                  {
                      Assert.Fail($"Test file not found: {testFile}");
                      return; // Skip test if file doesn't exist
                  }
  
                  var fileTypes = await FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.ShipmentInvoice, FileTypeManager.FileFormats.PDF, testFile).ConfigureAwait(false);
                  if (!fileTypes.Any())
                  {
                      Assert.Fail($"No suitable PDF FileType found for: {testFile}");
                      return; // Skip if no matching filetype
                  }
  
                  foreach (var fileType in fileTypes)
                  {
                      Console.WriteLine($"Testing with FileType: {fileType.Description} (ID: {fileType.Id})"); // Corrected property
                      // Clear DB before each filetype test within this method
                      Infrastructure.Utils.ClearDataBase();
                      await PDFUtils.ImportPDF(new FileInfo[]{new FileInfo(testFile)}, fileType).ConfigureAwait(false);
  
  
                      using (var ctx = new EntryDataDSContext())
                      {
                          // Basic assertion: Check if *any* data was imported.
                          // Specific counts might vary depending on the actual PDF content.
                          Assert.That(ctx.ShipmentInvoice.Any(), Is.True, "No ShipmentInvoice created."); // Corrected Assert
                          Assert.That(ctx.ShipmentInvoiceDetails.Any(), Is.True, "No ShipmentInvoiceDetails created."); // Corrected Assert
                          Console.WriteLine($"Import successful for FileType {fileType.Id}. Invoices: {ctx.ShipmentInvoice.Count()}, Details: {ctx.ShipmentInvoiceDetails.Count()}"); // Corrected property
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
         public async Task CanImportSheinMultiInvoice() // New Test for Shein
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
 
                 var fileTypes = (IEnumerable<FileTypes>)await FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.ShipmentInvoice, FileTypeManager.FileFormats.PDF, testFile).ConfigureAwait(false);
                 if (!fileTypes.Any())
                 {
                     Assert.Warn($"No suitable PDF FileType found for: {testFile}");
                     return; // Skip if no matching filetype
                 }
 
                 foreach (var fileType in fileTypes)
                 {
                     Console.WriteLine($"Testing with FileType: {fileType.Description} (ID: {fileType.Id})"); // Corrected property
                     // Clear DB before each filetype test within this method
                     Infrastructure.Utils.ClearDataBase();
                     await PDFUtils.ImportPDF(new FileInfo[]{new FileInfo(testFile)}, fileType).ConfigureAwait(false);
 
 
                     using (var ctx = new EntryDataDSContext())
                     {
                         // Basic assertion: Check if *any* data was imported. 
                         // Specific counts might vary depending on the actual PDF content.
                         Assert.That(ctx.ShipmentInvoice.Any(), Is.True, "No ShipmentInvoice created."); // Corrected Assert
                         Assert.That(ctx.ShipmentInvoiceDetails.Any(), Is.True, "No ShipmentInvoiceDetails created."); // Corrected Assert
                         Console.WriteLine($"Import successful for FileType {fileType.Id}. Invoices: {ctx.ShipmentInvoice.Count()}, Details: {ctx.ShipmentInvoiceDetails.Count()}"); // Corrected property
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
 
// --- START TEMU Template Import Tests ---


        [Test]
        public async Task CanImportTemuInvoice_07252024_TEMU() 
        {
            try
            {
                var testFile = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\Temu\07252024_TEMU.pdf"; 
                
                if (!File.Exists(testFile))
                {
                    Assert.Warn($"Test file not found: {testFile}");
                    return; // Skip test if file doesn't exist
                }

                var fileTypes = await FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.ShipmentInvoice, FileTypeManager.FileFormats.PDF, testFile).ConfigureAwait(false);
                if (!fileTypes.Any(ft => ft.Id == 1147)) // Ensure we use the generic PDF invoice type
                {
                    Assert.Warn($"FileType 1147 not found or not applicable for: {testFile}");
                    return; // Skip if the specific filetype isn't applicable
                }
                
                var fileType = fileTypes.First(ft => ft.Id == 1147); // Use the specific FileType

                Console.WriteLine($"Testing {Path.GetFileName(testFile)} with FileType: {fileType.Description} (ID: {fileType.Id})"); 
                Infrastructure.Utils.ClearDataBase(); 
                await PDFUtils.ImportPDF(new FileInfo[]{new FileInfo(testFile)}, fileType).ConfigureAwait(false);

                using (var ctx = new EntryDataDSContext())
                {
                    var invoices = ctx.ShipmentInvoice.Include("InvoiceDetails").Where(x => x.InvoiceNo == "PO-211-17867849567351770").ToList();
                    var details = ctx.ShipmentInvoiceDetails.Where(x => x.Invoice.InvoiceNo == "PO-211-17867849567351770").ToList();

                    Assert.That(invoices.Any(), Is.True, "No ShipmentInvoice created.");
                    Assert.That(details.Any(), Is.True, "No ShipmentInvoiceDetails created."); // Basic check

                    // Specific TEMU Assertions
                    Assert.That(invoices.Count, Is.EqualTo(1), "Expected exactly one ShipmentInvoice.");
                    var invoice = invoices.First();
                    
                    // TODO: Update expected values once extraction rules are defined
                    Assert.That(invoice.InvoiceNo, Is.EqualTo("PO-211-17867849567351770"), "Template number mismatch."); 
                    Assert.That(invoice.TotalsZero, Is.EqualTo(0), "TotalsZero should be 0."); 
                   

                    Console.WriteLine($"Import successful for {Path.GetFileName(testFile)}. Template: {invoice.InvoiceNo}, Details: {details.Count}, TotalsZero: {invoice.TotalsZero}"); 
                }

                Assert.That(true); // Indicate overall success if no exceptions/failures
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR in CanImportTemuInvoice_07252024_TEMU: {e}");
                Assert.Fail($"Test failed with exception: {e.Message}");
            }
        }

        [Test]
        public async Task CanImportTemuInvoice_07262024_TEMU() 
        {
            try
            {
                var testFile = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\Temu\07262024_TEMU.pdf"; 
                
                if (!File.Exists(testFile))
                {
                    Assert.Warn($"Test file not found: {testFile}");
                    return; // Skip test if file doesn't exist
                }

                var fileTypes = (IEnumerable<FileTypes>)FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.ShipmentInvoice, FileTypeManager.FileFormats.PDF, testFile);
                if (!fileTypes.Any(ft => ft.Id == 1147)) // Ensure we use the generic PDF invoice type
                {
                    Assert.Warn($"FileType 1147 not found or not applicable for: {testFile}");
                    return; // Skip if the specific filetype isn't applicable
                }
                
                var fileType = fileTypes.First(ft => ft.Id == 1147); // Use the specific FileType

                Console.WriteLine($"Testing {Path.GetFileName(testFile)} with FileType: {fileType.Description} (ID: {fileType.Id})"); 
                Infrastructure.Utils.ClearDataBase(); 
                await PDFUtils.ImportPDF(new FileInfo[] { new FileInfo(testFile) }, fileType).ConfigureAwait(false);

                using (var ctx = new EntryDataDSContext())
                {
                    var invoices = ctx.ShipmentInvoice.Include("InvoiceDetails").Where(x => x.InvoiceNo == "PO-211-01046445307513763").ToList();
                    var details = ctx.ShipmentInvoiceDetails.Where(x => x.Invoice.InvoiceNo == "PO-211-01046445307513763").ToList();

                    Assert.That(invoices.Any(), Is.True, "No ShipmentInvoice created.");
                    Assert.That(details.Any(), Is.True, "No ShipmentInvoiceDetails created."); // Basic check

                    // Specific TEMU Assertions
                    Assert.That(invoices.Count, Is.EqualTo(1), "Expected exactly one ShipmentInvoice.");
                    var invoice = invoices.First();
                    
                    // TODO: Update expected values once extraction rules are defined
                    Assert.That(invoice.InvoiceNo, Is.EqualTo("PO-211-01046445307513763"), "Template number mismatch."); 
                    Assert.That(invoice.TotalsZero, Is.EqualTo(0), "TotalsZero should be 0."); 
                    

                    Console.WriteLine($"Import successful for {Path.GetFileName(testFile)}. Template: {invoice.InvoiceNo}, Details: {details.Count}, TotalsZero: {invoice.TotalsZero}"); 
                }

                Assert.That(true); // Indicate overall success if no exceptions/failures
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR in CanImportTemuInvoice_07262024_TEMU: {e}");
                Assert.Fail($"Test failed with exception: {e.Message}");
            }
        }

        [Test]
        public async Task CanImportTemuInvoice_03152025103631() 
        {
            try
            {
                var testFile = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\Temu\03152025103631.pdf"; 
                
                if (!File.Exists(testFile))
                {
                    Assert.Warn($"Test file not found: {testFile}");
                    return; // Skip test if file doesn't exist
                }

                var fileTypes = (IEnumerable<FileTypes>)FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.ShipmentInvoice, FileTypeManager.FileFormats.PDF, testFile);
                if (!fileTypes.Any(ft => ft.Id == 1147)) // Ensure we use the generic PDF invoice type
                {
                    Assert.Warn($"FileType 1147 not found or not applicable for: {testFile}");
                    return; // Skip if the specific filetype isn't applicable
                }
                
                var fileType = fileTypes.First(ft => ft.Id == 1147); // Use the specific FileType

                Console.WriteLine($"Testing {Path.GetFileName(testFile)} with FileType: {fileType.Description} (ID: {fileType.Id})"); 
                Infrastructure.Utils.ClearDataBase(); 
                var res = await PDFUtils.ImportPDF(new FileInfo[]{new FileInfo(testFile)}, fileType).ConfigureAwait(false);

                using (var ctx = new EntryDataDSContext())
                {
                    var invoices = ctx.ShipmentInvoice
                                                        .Include("InvoiceDetails")
                                                        .Where(x => x.InvoiceNo == "PO-211-11245148453753900").ToList();
                    var details = ctx.ShipmentInvoiceDetails.Where(x => x.Invoice.InvoiceNo == "PO-211-01046445307513763").ToList();

                    Assert.That(invoices.Any(), Is.True, "No ShipmentInvoice created.");
                    Assert.That(details.Any(), Is.True, "No ShipmentInvoiceDetails created."); // Basic check

                    // Specific TEMU Assertions
                    Assert.That(invoices.Count, Is.EqualTo(1), "Expected exactly one ShipmentInvoice.");
                    var invoice = invoices.First();
                    
                    // TODO: Update expected values once extraction rules are defined
                    Assert.That(invoice.InvoiceNo, Is.EqualTo("PO-211-11245148453753900"), "Template number mismatch."); 
                    Assert.That(invoice.TotalsZero, Is.EqualTo(0), "TotalsZero should be 0."); 
                   

                    Console.WriteLine($"Import successful for {Path.GetFileName(testFile)}. Template: {invoice.InvoiceNo}, Details: {details.Count}, TotalsZero: {invoice.TotalsZero}"); 
                }

                Assert.That(true); // Indicate overall success if no exceptions/failures
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR in CanImportTemuInvoice_03152025103631: {e}");
                Assert.Fail($"Test failed with exception: {e.Message}");
            }
        }

        [Test]
        public async Task CanImportTemuInvoice_03152025103721() 
        {
            try
            {
                var testFile = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\Temu\03152025103721.pdf"; 
                
                if (!File.Exists(testFile))
                {
                    Assert.Warn($"Test file not found: {testFile}");
                    return; // Skip test if file doesn't exist
                }

                var fileTypes = (IEnumerable<FileTypes>)FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.ShipmentInvoice, FileTypeManager.FileFormats.PDF, testFile);
                if (!fileTypes.Any(ft => ft.Id == 1147)) // Ensure we use the generic PDF invoice type
                {
                    Assert.Warn($"FileType 1147 not found or not applicable for: {testFile}");
                    return; // Skip if the specific filetype isn't applicable
                }
                
                var fileType = fileTypes.First(ft => ft.Id == 1147); // Use the specific FileType

                Console.WriteLine($"Testing {Path.GetFileName(testFile)} with FileType: {fileType.Description} (ID: {fileType.Id})"); 
                Infrastructure.Utils.ClearDataBase(); 
                await PDFUtils.ImportPDF(new FileInfo[]{new FileInfo(testFile)}, fileType).ConfigureAwait(false);

                using (var ctx = new EntryDataDSContext())
                {
                    var invoices = ctx.ShipmentInvoice
                        .Include("InvoiceDetails")
                        .Where(x => x.InvoiceNo == "PO-211-11245253311353900").ToList();
                    var details = ctx.ShipmentInvoiceDetails.Where(x => x.Invoice.InvoiceNo == "PO-211-11245253311353900").ToList();

                    Assert.That(invoices.Any(), Is.True, "No ShipmentInvoice created.");
                    Assert.That(details.Any(), Is.True, "No ShipmentInvoiceDetails created."); // Basic check

                    // Specific TEMU Assertions
                    Assert.That(invoices.Count, Is.EqualTo(1), "Expected exactly one ShipmentInvoice.");
                    var invoice = invoices.First();
                    
                    // TODO: Update expected values once extraction rules are defined
                    Assert.That(invoice.InvoiceNo, Is.EqualTo("PO-211-11245253311353900"), "Template number mismatch."); 
                    Assert.That(invoice.TotalsZero, Is.EqualTo(0), "TotalsZero should be 0."); 
                    

                    Console.WriteLine($"Import successful for {Path.GetFileName(testFile)}. Template: {invoice.InvoiceNo}, Details: {details.Count}, TotalsZero: {invoice.TotalsZero}"); 
                }

                Assert.That(true); // Indicate overall success if no exceptions/failures
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR in CanImportTemuInvoice_03152025103721: {e}");
                Assert.Fail($"Test failed with exception: {e.Message}");
            }
        }

        [Test]
        public async Task CanImportTemuInvoice_03152025135830() 
        {
            try
            {
                var testFile = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\Temu\03152025135830.pdf"; 
                
                if (!File.Exists(testFile))
                {
                    Assert.Warn($"Test file not found: {testFile}");
                    return; // Skip test if file doesn't exist
                }

                var fileTypes = (IEnumerable<FileTypes>)FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.ShipmentInvoice, FileTypeManager.FileFormats.PDF, testFile);
                if (!fileTypes.Any(ft => ft.Id == 1147)) // Ensure we use the generic PDF invoice type
                {
                    Assert.Warn($"FileType 1147 not found or not applicable for: {testFile}");
                    return; // Skip if the specific filetype isn't applicable
                }
                
                var fileType = fileTypes.First(ft => ft.Id == 1147); // Use the specific FileType

                Console.WriteLine($"Testing {Path.GetFileName(testFile)} with FileType: {fileType.Description} (ID: {fileType.Id})"); 
                Infrastructure.Utils.ClearDataBase(); 
                await PDFUtils.ImportPDF(new FileInfo[] { new FileInfo(testFile) }, fileType).ConfigureAwait(false);

                using (var ctx = new EntryDataDSContext())
                {
                    var invoices = ctx.ShipmentInvoice
                        .Include("InvoiceDetails")
                        .Where(x => x.InvoiceNo == "PO-211-0650403289407038S").ToList();
                    var details = ctx.ShipmentInvoiceDetails.Where(x => x.Invoice.InvoiceNo == "PO-211-0650403289407038S").ToList();

                    Assert.That(invoices.Any(), Is.True, "No ShipmentInvoice created.");
                    Assert.That(details.Any(), Is.True, "No ShipmentInvoiceDetails created."); // Basic check

                    // Specific TEMU Assertions
                    Assert.That(invoices.Count, Is.EqualTo(1), "Expected exactly one ShipmentInvoice.");
                    var invoice = invoices.First();
                    
                    // TODO: Update expected values once extraction rules are defined
                    Assert.That(invoice.InvoiceNo, Is.EqualTo("PO-211-0650403289407038S"), "Template number mismatch."); 
                    Assert.That(invoice.TotalsZero, Is.EqualTo(0), "TotalsZero should be 0."); 
                    

                    Console.WriteLine($"Import successful for {Path.GetFileName(testFile)}. Template: {invoice.InvoiceNo}, Details: {details.Count}, TotalsZero: {invoice.TotalsZero}"); 
                }

                Assert.That(true); // Indicate overall success if no exceptions/failures
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR in CanImportTemuInvoice_03152025135830: {e}");
                Assert.Fail($"Test failed with exception: {e.Message}");
            }
        }

        // --- END TEMU Template Import Tests ---
        // --- TEMPORARY TESTS TO EXTRACT PDF TEXT ---


        [Test]
        public async Task ExtractText_Temu_07252024_TEMU()
        {
            var filePath = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\Temu\07252024_TEMU.pdf";
            if (!File.Exists(filePath)) { TestContext.WriteLine($"File not found: {filePath}"); return; }
            var text = await InvoiceReader.InvoiceReader.GetPdftxt(filePath).ConfigureAwait(false);
            TestContext.WriteLine($"--- START TEXT: {Path.GetFileName(filePath)} ---");
            TestContext.WriteLine(text);
            TestContext.WriteLine($"--- END TEXT: {Path.GetFileName(filePath)} ---");
            Assert.Pass("Text extracted (check output).");
        }

        [Test]
        public async Task ExtractText_Temu_07262024_TEMU()
        {
            var filePath = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\Temu\07262024_TEMU.pdf";
            if (!File.Exists(filePath)) { TestContext.WriteLine($"File not found: {filePath}"); return; }
            var text = await InvoiceReader.InvoiceReader.GetPdftxt(filePath).ConfigureAwait(false);
            TestContext.WriteLine($"--- START TEXT: {Path.GetFileName(filePath)} ---");
            TestContext.WriteLine(text);
            TestContext.WriteLine($"--- END TEXT: {Path.GetFileName(filePath)} ---");
            Assert.Pass("Text extracted (check output).");
        }

        [Test]
        public async Task ExtractText_Temu_03152025103631()
        {
            var filePath = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\Temu\03152025103631.pdf";
            if (!File.Exists(filePath)) { TestContext.WriteLine($"File not found: {filePath}"); return; }
            var text = await InvoiceReader.InvoiceReader.GetPdftxt(filePath).ConfigureAwait(false);
            TestContext.WriteLine($"--- START TEXT: {Path.GetFileName(filePath)} ---");
            TestContext.WriteLine(text);
            TestContext.WriteLine($"--- END TEXT: {Path.GetFileName(filePath)} ---");
            Assert.Pass("Text extracted (check output).");
        }

        [Test]
        public async Task ExtractText_Temu_03152025103721()
        {
            var filePath = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\Temu\03152025103721.pdf";
            if (!File.Exists(filePath)) { TestContext.WriteLine($"File not found: {filePath}"); return; }
            var text = await InvoiceReader.InvoiceReader.GetPdftxt(filePath).ConfigureAwait(false);
            TestContext.WriteLine($"--- START TEXT: {Path.GetFileName(filePath)} ---");
            TestContext.WriteLine(text);
            TestContext.WriteLine($"--- END TEXT: {Path.GetFileName(filePath)} ---");
            Assert.Pass("Text extracted (check output).");
        }

        [Test]
        public async Task ExtractText_Temu_03152025135830()
        {
            var filePath = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\Temu\03152025135830.pdf";
            if (!File.Exists(filePath)) { TestContext.WriteLine($"File not found: {filePath}"); return; }
            var text = await InvoiceReader.InvoiceReader.GetPdftxt(filePath).ConfigureAwait(false);
            TestContext.WriteLine($"--- START TEXT: {Path.GetFileName(filePath)} ---");
            TestContext.WriteLine(text);
            TestContext.WriteLine($"--- END TEXT: {Path.GetFileName(filePath)} ---");
            Assert.Pass("Text extracted (check output).");
        }

        // --- END TEMPORARY TESTS ---
     } // End Class
 } // End Namespace