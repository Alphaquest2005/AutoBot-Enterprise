using Ghostscript.NET.Rasterizer;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Ghostscript.NET;
using iText.Kernel.Pdf;
using Tesseract;
using Serilog;

/**
 * Recognizing text on a PDF page using Tesseract and Ghostscript
 * 
 * For more information visit: https://github.com/OmarMuscatello/pdf-ocr
 */

namespace pdf_ocr
{
    public class PdfOcr
    {
        private readonly ILogger _logger;
        private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        //private static readonly string TempDir = Path.Combine(BaseDirectory, "Temp");

        public PdfOcr(ILogger logger)
        {
            _logger = logger;
        }

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
            _logger.Information("METHOD_ENTRY: Ocr. Intention: Perform OCR on a PDF file. InitialState: {InitialState}", new { InputFile = inputPdfFile, PageSegMode = pagemode });
            try
            {
                var TempDir = Path.Combine(BaseDirectory, $"{Guid.NewGuid()}");
                _logger.Debug("INTERNAL_STEP (Ocr - TempDirCreation): Creating temporary directory: {TempDirectory}", TempDir);
                if(Directory.Exists(TempDir)) Directory.Delete(TempDir, true);
                Directory.CreateDirectory(TempDir);
                _logger.Debug("INTERNAL_STEP (Ocr - TempDirCreation): Temporary directory created.");
                
                var file = new FileInfo(inputPdfFile);
                var processFile = Path.Combine(TempDir, file.Name);
                _logger.Debug("INTERNAL_STEP (Ocr - FileCopy): Copying input file to temporary directory: {SourceFile} -> {DestinationFile}", inputPdfFile, processFile);
                File.Copy(inputPdfFile, processFile);
                _logger.Debug("INTERNAL_STEP (Ocr - FileCopy): File copied.");
                var newFile = new FileInfo(Path.Combine(TempDir,file.Name + ".txt"));
                if (newFile.Exists && newFile.LastWriteTime > file.LastAccessTime.AddMinutes(5))
                {
                    _logger.Information("INTERNAL_STEP (Ocr - ExistingTextFile): Found existing text file, returning its content: {TextFile}", newFile.FullName);
                    var existingText = File.ReadAllText(processFile + ".txt");
                    _logger.Information("METHOD_EXIT_SUCCESS: Ocr. IntentionAchieved: Returned existing text file content. FinalState: {FinalState}. Total execution time: {Elapsed:0} ms.", new { TextLength = existingText.Length }, 0); // Placeholder for elapsed time
                    return existingText;
                }
                _logger.Debug("INTERNAL_STEP (Ocr - GetImageFromPdf): Calling GetImageFromPdf.");
                GetImageFromPdf(processFile, TempDir);
                _logger.Debug("INTERNAL_STEP (Ocr - GetImageFromPdf): GetImageFromPdf completed.");
                //Recognizing text from the generated image
                _logger.Debug("INTERNAL_STEP (Ocr - GetTextFromImage): Calling GetTextFromImage.");
                var recognizedText = GetTextFromImage(pagemode, TempDir, processFile, true, _logger);
                _logger.Debug("INTERNAL_STEP (Ocr - GetTextFromImage): GetTextFromImage completed.");

                _logger.Information("METHOD_EXIT_SUCCESS: Ocr. IntentionAchieved: OCR completed successfully. FinalState: {FinalState}. Total execution time: {Elapsed:0} ms.", new { RecognizedTextLength = recognizedText.Length }, 0); // Placeholder for elapsed time
                return recognizedText;
            }
            catch (Exception e)
            {
                _logger.Error(e, "METHOD_EXIT_FAILURE: Ocr. IntentionFailed: OCR process failed for file {InputFile}.", inputPdfFile);
                Console.WriteLine(e);
                throw;
            }
        }

        public static string GetTextFromImage(PageSegMode pagemode, string TempDir, string processFile, bool deleteFolder, ILogger logger)
        {
            logger.Information("METHOD_ENTRY: GetTextFromImage (with processFile). Intention: Get text from images and save to file. InitialState: {InitialState}", new { TempDirectory = TempDir, ProcessFile = processFile, DeleteFolder = deleteFolder });
            var recognizedText = GetTextFromImage(pagemode, TempDir, logger); // Pass logger here
            logger.Debug("INTERNAL_STEP (GetTextFromImage - WriteText): Writing recognized text to file: {ProcessFile}.txt", processFile);
            File.WriteAllText(processFile + ".txt", recognizedText);
            logger.Debug("INTERNAL_STEP (GetTextFromImage - WriteText): Text written.");
            if(deleteFolder)
            {
                logger.Debug("INTERNAL_STEP (GetTextFromImage - DeleteFolder): Deleting temporary directory: {TempDirectory}", TempDir);
                Directory.Delete(TempDir, true);
                logger.Debug("INTERNAL_STEP (GetTextFromImage - DeleteFolder): Temporary directory deleted.");
            }
            logger.Information("METHOD_EXIT_SUCCESS: GetTextFromImage (with processFile). IntentionAchieved: Text extracted and saved. FinalState: {FinalState}. Total execution time: {Elapsed:0} ms.", new { RecognizedTextLength = recognizedText.Length }, 0); // Placeholder for elapsed time
            return recognizedText;
        }

        /// <summary>
        /// Get text from the specified image file.
        /// </summary>
        /// <param name="pagemode"></param>
        /// <param name="TempDir"></param>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        private static string GetTextFromImage(PageSegMode pagemode, string TempDir, ILogger logger)
        {
            logger.Information("METHOD_ENTRY: GetTextFromImage (OCR). Intention: Perform OCR on images in a directory. InitialState: {InitialState}", new { TempDirectory = TempDir, PageSegMode = pagemode });
            var result = new StringBuilder();

            var files = new DirectoryInfo(TempDir).GetFiles().Where(x => x.Extension == ".png").OrderBy(x => x.LastWriteTime).ToList();
            logger.Debug("INTERNAL_STEP (GetTextFromImage - FileScan): Found {FileCount} PNG files in temporary directory.", files.Count);
            foreach (var file in files)
            {
                logger.Debug("INTERNAL_STEP (GetTextFromImage - ProcessFile): Processing image file: {FileName}", file.FullName);
                try
                {
                    logger.Debug("INTERNAL_STEP (GetTextFromImage - LoadImage): Calling Pix.LoadFromFile for {FileName}.", file.FullName);
                    using (var img = Pix.LoadFromFile(file.FullName))
                    {
                        logger.Debug("INTERNAL_STEP (GetTextFromImage - LoadImage): Pix.LoadFromFile completed.");
                        logger.Debug("INTERNAL_STEP (GetTextFromImage - InitTesseract): Initializing TesseractEngine.");
                        using (var tesseractEngine = new TesseractEngine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata"), ocrLanguage, EngineMode.Default))
                        {
                            logger.Debug("INTERNAL_STEP (GetTextFromImage - InitTesseract): TesseractEngine initialized.");
                            tesseractEngine.DefaultPageSegMode = pagemode;
                            logger.Debug("INTERNAL_STEP (GetTextFromImage - ProcessImage): Calling tesseractEngine.Process(img).");
                            using (var page = tesseractEngine.Process(img))
                            {
                                logger.Debug("INTERNAL_STEP (GetTextFromImage - ProcessImage): tesseractEngine.Process(img) completed.");
                                var text = page.GetText();
                                logger.Verbose("INTERNAL_STEP (GetTextFromImage - ExtractText): Extracted text (first 100 chars): {ExtractedText}", text.Length > 100 ? text.Substring(0, 100) + "..." : text);
                                result.AppendLine(text);
                                logger.Debug("INTERNAL_STEP (GetTextFromImage - ExtractText): Appended text to result.");
                            }
                            logger.Debug("INTERNAL_STEP (GetTextFromImage - ProcessImage): Disposed Tesseract page.");
                        }
                        logger.Debug("INTERNAL_STEP (GetTextFromImage - InitTesseract): Disposed TesseractEngine.");
                    }
                    logger.Debug("INTERNAL_STEP (GetTextFromImage - LoadImage): Disposed Pix image.");
                }
                catch (Exception fileEx)
                {
                    logger.Error(fileEx, "INTERNAL_STEP_FAILURE (GetTextFromImage - ProcessFile): Failed to process image file {FileName}.", file.FullName);
                    // Continue processing other files
                }
            }

            logger.Information("METHOD_EXIT_SUCCESS: GetTextFromImage (OCR). IntentionAchieved: OCR completed for all images. FinalState: {FinalState}. Total execution time: {Elapsed:0} ms.", new { TotalTextLength = result.Length }, 0); // Placeholder for elapsed time
            return result.ToString();
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
            _logger.Information("METHOD_ENTRY: GetImageFromPdf. Intention: Convert PDF pages to images. InitialState: {InitialState}", new { PdfPath = pdfPath, TempDirectory = TempDir });

            try
            {
                int pageCount;
                _logger.Debug("INTERNAL_STEP (GetImageFromPdf - GetPageCount): Getting page count from PDF: {PdfPath}", pdfPath);
                using (var reader = new PdfReader(pdfPath))
                {
                   var doc = new PdfDocument(reader);

                   pageCount = doc.GetNumberOfPages();
                }
                _logger.Debug("INTERNAL_STEP (GetImageFromPdf - GetPageCount): Page count obtained: {PageCount}", pageCount);

                //var ghostscriptRasterizer = new GhostscriptRasterizer();
                //ghostscriptRasterizer.Open(pdfPath);

                _logger.Debug("INTERNAL_STEP (GetImageFromPdf - ProcessPages): Starting conversion for {PageCount} pages.", pageCount);
                for (int i = 1; i <= pageCount; i++)
                {
                    _logger.Debug("INTERNAL_STEP (GetImageFromPdf - ProcessPage): Processing page {PageNumber}.", i);
                    var outputFilePath = GetTempFile(TempDir, ".png");
                    _logger.Debug("INTERNAL_STEP (GetImageFromPdf - ProcessPage): Generated temporary image path: {OutputFilePath}", outputFilePath);
                    //using (var img = ghostscriptRasterizer.GetPage(pdfToImageDPI, pdfToImageDPI, i))
                    //{
                    //    img.Save(outputFilePath, System.Drawing.Imaging.ImageFormat.Png);
                    //    img.Dispose();
                    //}

                    _logger.Debug("INTERNAL_STEP (GetImageFromPdf - ProcessPage): Calling PdfToPngWithGhostscriptPngDevice for page {PageNumber}.", i);
                    PdfToPngWithGhostscriptPngDevice(pdfPath, i, pdfToImageDPI, pdfToImageDPI, outputFilePath);
                    _logger.Debug("INTERNAL_STEP (GetImageFromPdf - ProcessPage): PdfToPngWithGhostscriptPngDevice completed for page {PageNumber}.", i);


                }
                _logger.Debug("INTERNAL_STEP (GetImageFromPdf - ProcessPages): Finished processing pages.");

                //ghostscriptRasterizer.Close();
                _logger.Information("METHOD_EXIT_SUCCESS: GetImageFromPdf. IntentionAchieved: PDF pages converted to images. FinalState: {FinalState}. Total execution time: {Elapsed:0} ms.", new { ConvertedPageCount = pageCount }, 0); // Placeholder for elapsed time
            }
            catch (Exception e)
            {
                _logger.Error(e, "METHOD_EXIT_FAILURE: GetImageFromPdf. IntentionFailed: Failed to convert PDF pages to images for file {PdfPath}.", pdfPath);
                Console.WriteLine(e);
                throw;
            }

        }


        private void PdfToPngWithGhostscriptPngDevice(string srcFile, int pageNo, int dpiX, int dpiY, string tgtFile)
        {
            _logger.Information("METHOD_ENTRY: PdfToPngWithGhostscriptPngDevice. Intention: Convert a single PDF page to PNG using Ghostscript. InitialState: {InitialState}", new { SourceFile = srcFile, PageNumber = pageNo, DpiX = dpiX, DpiY = dpiY, TargetFile = tgtFile });
            try
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
                _logger.Debug("INTERNAL_STEP (PdfToPngWithGhostscriptPngDevice - Process): Calling dev.Process().");
                dev.Process();
                _logger.Debug("INTERNAL_STEP (PdfToPngWithGhostscriptPngDevice - Process): dev.Process() completed.");
                _logger.Information("METHOD_EXIT_SUCCESS: PdfToPngWithGhostscriptPngDevice. IntentionAchieved: PDF page converted to PNG. FinalState: {FinalState}. Total execution time: {Elapsed:0} ms.", new { OutputFile = tgtFile }, 0); // Placeholder for elapsed time
            }
            catch (Exception e)
            {
                _logger.Error(e, "METHOD_EXIT_FAILURE: PdfToPngWithGhostscriptPngDevice. IntentionFailed: Failed to convert PDF page {PageNumber} to PNG for file {SourceFile}.", pageNo, srcFile);
                throw;
            }
        }

        /// <summary>
        /// Returns the path of a new file in the <see cref="TempDir"/> directory. The file is not created.
        /// </summary>
        /// <param name="TempDir"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        private string GetTempFile(string TempDir, string extension = ".tmp")
        {
            _logger.Information("METHOD_ENTRY: GetTempFile. Intention: Generate a temporary file path. InitialState: {InitialState}", new { TempDirectory = TempDir, Extension = extension });
            try
            {
                var tempFilePath = Path.Combine(TempDir, Guid.NewGuid().ToString() + extension);
                _logger.Debug("INTERNAL_STEP (GetTempFile - GeneratePath): Generated temporary file path: {TempFilePath}", tempFilePath);
                _logger.Information("METHOD_EXIT_SUCCESS: GetTempFile. IntentionAchieved: Temporary file path generated. FinalState: {FinalState}. Total execution time: {Elapsed:0} ms.", new { TempFilePath = tempFilePath }, 0); // Placeholder for elapsed time
                return tempFilePath;
            }
            catch (Exception e)
            {
                _logger.Error(e, "METHOD_EXIT_FAILURE: GetTempFile. IntentionFailed: Failed to generate temporary file path in directory {TempDirectory}.", TempDir);
                throw;
            }
        }
    }
}
