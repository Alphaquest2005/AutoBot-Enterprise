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
        //private static readonly string TempDir = Path.Combine(BaseDirectory, "Temp");

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

        public string Ocr(string inputPdfFile, PageSegMode pagemode)
        {
            try
            {
                var TempDir = Path.Combine(BaseDirectory, $"{Guid.NewGuid()}");
                if(Directory.Exists(TempDir)) Directory.Delete(TempDir, true);
                Directory.CreateDirectory(TempDir);
                
                var file = new FileInfo(inputPdfFile);
                var processFile = Path.Combine(TempDir, file.Name);
                File.Copy(inputPdfFile, processFile);
                var newFile = new FileInfo(Path.Combine(TempDir,file.Name + ".txt"));
                if (newFile.Exists && newFile.LastWriteTime > file.LastAccessTime.AddMinutes(5))
                {
                    return File.ReadAllText(processFile + ".txt");
                }
                GetImageFromPdf(processFile, TempDir);
                //Recognizing text from the generated image
                return GetTextFromImage(pagemode, TempDir, processFile, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static string GetTextFromImage(PageSegMode pagemode, string TempDir, string processFile, bool deleteFolder)
        {
            var recognizedText = GetTextFromImage(pagemode, TempDir);
            File.WriteAllText(processFile + ".txt", recognizedText);
            if(deleteFolder) Directory.Delete(TempDir, true);
            return recognizedText;
        }

        /// <summary>
        /// Get an image from a PDF page.
        /// </summary>
        /// <param name="pdfPath"></param>
        /// <param name="TempDir"></param>
        /// <param name="pageNumber"></param>
        /// <returns>The path of the generated image.</returns>
        private void GetImageFromPdf(string pdfPath, string TempDir)
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
                    var outputFilePath = GetTempFile(TempDir, ".png");
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


        private void PdfToPngWithGhostscriptPngDevice(string srcFile, int pageNo, int dpiX, int dpiY, string tgtFile)
        {
            GhostscriptPngDevice dev = new GhostscriptPngDevice(GhostscriptPngDeviceType.PngGray)
 {
     GraphicsAlphaBits = GhostscriptImageDeviceAlphaBits.V_4,
     TextAlphaBits = GhostscriptImageDeviceAlphaBits.V_4,
     ResolutionXY = new GhostscriptImageDeviceResolution(dpiX, dpiY)
 };
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
        /// <param name="pagemode"></param>
        /// <param name="TempDir"></param>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        private static string GetTextFromImage(PageSegMode pagemode, string TempDir)
        {
            var result = new StringBuilder();

            var files = new DirectoryInfo(TempDir).GetFiles().Where(x => x.Extension == ".png").OrderBy(x => x.LastWriteTime).ToList();
            foreach (var file in files)
            {
                using (var img = Pix.LoadFromFile(file.FullName))
                {
                    using (var tesseractEngine = new TesseractEngine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata"), ocrLanguage, EngineMode.Default))
                    {
                        tesseractEngine.DefaultPageSegMode = pagemode;
                        
                        using (var page = tesseractEngine.Process(img))
                        {
                            result.AppendLine(page.GetText());
                        }
                        
                        //tesseractEngine.DefaultPageSegMode = PageSegMode.SparseText;

                        //using (var page = tesseractEngine.Process(img))
                        //{
                        //    result.AppendLine(page.GetText());
                        //}
                    }
                    
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Returns the path of a new file in the <see cref="TempDir"/> directory. The file is not created.
        /// </summary>
        /// <param name="TempDir"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        private string GetTempFile(string TempDir, string extension = ".tmp")
        {
            return Path.Combine(TempDir, Guid.NewGuid().ToString() + extension);
        }
    }
}
