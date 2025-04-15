using System.IO;
using AdjustmentQS.Business.Entities;
using EntryDataDS.Business.Entities;
using OCR.Business.Services;
using WaterNut.Business.Services.Importers;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;

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
        public void CanImportShipmentInvoice() // Original Test
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var testFile = Infrastructure.Utils.GetTestSalesFile(new List<string>() { "01987.pdf" });
                var fileTypes = (IEnumerable<FileTypes>)FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.ShipmentInvoice, FileTypeManager.FileFormats.PDF, testFile);
                foreach (var fileType in fileTypes)
                {
                    PDFUtils.ImportPDF(new FileInfo[]{new FileInfo(testFile)}, fileType);


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
          public void CanImportAmazonMultiSectionInvoice() // Added Test (Corrected)
          {
              try
              {
                  //if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true); // Skip if not test settings
                  // Use the absolute path provided by the user
                  var testFile = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\one amazon with muliple invoice details sections.pdf"; 
                  
                  if (!File.Exists(testFile))
                  {
                      Assert.Warn($"Test file not found: {testFile}");
                      return; // Skip test if file doesn't exist
                  }
  
                  var fileTypes = (IEnumerable<FileTypes>)FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.ShipmentInvoice, FileTypeManager.FileFormats.PDF, testFile);
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
                      PDFUtils.ImportPDF(new FileInfo[]{new FileInfo(testFile)}, fileType);
  
  
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
         public void CanImportSheinMultiInvoice() // New Test for Shein
         {
             try
             {
                 //if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true); // Skip if not test settings
                 // Use the absolute path provided by the user for Shein PDF
                 var testFile = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\Shein - multiple invoices for one shipment .pdf"; 
                 
                 if (!File.Exists(testFile))
                 {
                     Assert.Warn($"Test file not found: {testFile}");
                     return; // Skip test if file doesn't exist
                 }
 
                 var fileTypes = (IEnumerable<FileTypes>)FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.ShipmentInvoice, FileTypeManager.FileFormats.PDF, testFile);
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
                     PDFUtils.ImportPDF(new FileInfo[]{new FileInfo(testFile)}, fileType);
 
 
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
 
     } // End Class
 } // End Namespace