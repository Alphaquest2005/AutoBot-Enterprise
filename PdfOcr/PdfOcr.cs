using Ghostscript.NET.Rasterizer;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Ghostscript.NET;
using iTextSharp.text.pdf;
using Tesseract;

/**
 * Recognizing text on a PDF page using Tesseract and Ghostscript
 * 
 * For more information visit: https://github.com/OmarMuscatello/pdf-ocr
 */

namespace pdf_ocr
{
    public class PdfOcr
    {
        private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string TempDir = Path.Combine(BaseDirectory, "Temp");

        #region Settings

        /// <summary>
        /// The input PDF file
        /// </summary>
       // private static string inputPdfFile = Path.Combine(BaseDirectory, "test.pdf");

        /// <summary>
        /// The page from which recognize text
        /// </summary>
   

        /// <summary>
        /// Language of text recognition. Depends on the files you have in the tessadata directory. See installation instruction on GitHub repository.
        /// </summary>
        private const string ocrLanguage = "eng";

        /// <summary>
        /// Pixel density used to convert the PDF file to image
        /// </summary>
        private const int pdfToImageDPI = 300;

        #endregion

        public static string Ocr(string inputPdfFile)
        {
            try
            {
                if(Directory.Exists(TempDir)) Directory.Delete(TempDir, true);
                Directory.CreateDirectory(TempDir);
                var file = new FileInfo(inputPdfFile);
                var newFile = new FileInfo(inputPdfFile + ".txt");
                if (newFile.Exists && newFile.LastWriteTime > file.LastAccessTime.AddMinutes(5))
                {
                    return File.ReadAllText(inputPdfFile + ".txt");
                }
                GetImageFromPdf(inputPdfFile);
                //Recognizing text from the generated image
                var recognizedText = GetTextFromImage();
                File.WriteAllText(inputPdfFile+ ".txt", recognizedText);
                Directory.Delete(TempDir, true);
                return recognizedText;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Get an image from a PDF page.
        /// </summary>
        /// <param name="pdfPath"></param>
        /// <param name="pageNumber"></param>
        /// <returns>The path of the generated image.</returns>
        private static void GetImageFromPdf(string pdfPath)
        {

            try
            {
                int pageCount;
                using (var reader = new PdfReader(pdfPath))
                {
                    //  as a matter of fact we need iTextSharp PdfReader (and all of iTextSharp) only to get the page count of PDF document;
                    //  unfortunately GhostScript itself doesn't know how to do it
                    pageCount = reader.NumberOfPages;
                }

                //var ghostscriptRasterizer = new GhostscriptRasterizer();
                //ghostscriptRasterizer.Open(pdfPath);

                for (int i = 1; i <= pageCount; i++)
                {
                    var outputFilePath = GetTempFile(".png");
                    //using (var img = ghostscriptRasterizer.GetPage(pdfToImageDPI, pdfToImageDPI, i))
                    //{
                    //    img.Save(outputFilePath, System.Drawing.Imaging.ImageFormat.Png);
                    //    img.Dispose();
                    //}

                    PdfToPngWithGhostscriptPngDevice(pdfPath, i, pdfToImageDPI, pdfToImageDPI, outputFilePath);


                }

                //ghostscriptRasterizer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }


        private static void PdfToPngWithGhostscriptPngDevice(string srcFile, int pageNo, int dpiX, int dpiY, string tgtFile)
        {
            GhostscriptPngDevice dev = new GhostscriptPngDevice(GhostscriptPngDeviceType.PngGray);
            dev.GraphicsAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
            dev.TextAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
            dev.ResolutionXY = new GhostscriptImageDeviceResolution(dpiX, dpiY);
            dev.InputFiles.Add(srcFile);
            dev.Pdf.FirstPage = pageNo;
            dev.Pdf.LastPage = pageNo;
            dev.CustomSwitches.Add("-dDOINTERPOLATE");
            dev.OutputPath = tgtFile;
            dev.Process();
        }
        /// <summary>
        /// Get text from the specified image file.
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        private static string GetTextFromImage()
        {
            var result = new StringBuilder();
            foreach (var file in new DirectoryInfo(TempDir).GetFiles().OrderBy(x => x.LastWriteTime))
            {


                using (var img = Pix.LoadFromFile(file.FullName))
                {
                    using (var tesseractEngine = new TesseractEngine(Path.Combine(Environment.CurrentDirectory, "tessdata"), ocrLanguage, EngineMode.Default))
                    {
                        tesseractEngine.DefaultPageSegMode = PageSegMode.SingleColumn;
                        
                        using (var page = tesseractEngine.Process(img))
                        {
                            result.AppendLine(page.GetText());
                        }
                        result.AppendLine("------------------------------------------Sparse Text-------------------------");
                        tesseractEngine.DefaultPageSegMode = PageSegMode.SparseText;

                        using (var page = tesseractEngine.Process(img))
                        {
                            result.AppendLine(page.GetText());
                        }
                    }
                    
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Returns the path of a new file in the <see cref="TempDir"/> directory. The file is not created.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private static string GetTempFile(string extension = ".tmp")
        {
            return Path.Combine(TempDir, Guid.NewGuid().ToString() + extension);
        }
    }
}
