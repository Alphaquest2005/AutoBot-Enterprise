using Ghostscript.NET.Rasterizer;
using System;
using System.IO;
using System.Text;
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
                Directory.CreateDirectory(TempDir);
                GetImageFromPdf(inputPdfFile);
                //Recognizing text from the generated image
                var recognizedText = GetTextFromImage();
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
                var ghostscriptRasterizer = new GhostscriptRasterizer();
                ghostscriptRasterizer.Open(pdfPath);

                for (int i = 1; i <= ghostscriptRasterizer.PageCount; i++)
                {
                    var outputFilePath = GetTempFile(".png");
                    using (var img = ghostscriptRasterizer.GetPage(pdfToImageDPI, pdfToImageDPI, i))
                    {
                        img.Save(outputFilePath, System.Drawing.Imaging.ImageFormat.Png);
                        img.Dispose();
                    }
                }

                ghostscriptRasterizer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        /// <summary>
        /// Get text from the specified image file.
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        private static string GetTextFromImage()
        {
            var result = new StringBuilder();
            foreach (var file in Directory.GetFiles(TempDir))
            {


                using (var img = Pix.LoadFromFile(file))
                {
                    using (var tesseractEngine = new TesseractEngine("tessdata", ocrLanguage, EngineMode.Default))
                    {
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
