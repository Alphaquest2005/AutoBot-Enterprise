using Ghostscript.NET.Rasterizer;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

                _logger.Debug("INTERNAL_STEP (GetImageFromPdf - ProcessPages): Starting resilient conversion for {PageCount} pages.", pageCount);
                
                // **RESILIENT_PROCESSING**: Track success/failure for each page
                int successfulPages = 0;
                int failedPages = 0;
                
                for (int i = 1; i <= pageCount; i++)
                {
                    _logger.Debug("INTERNAL_STEP (GetImageFromPdf - ProcessPage): Processing page {PageNumber}.", i);
                    var outputFilePath = GetTempFile(TempDir, ".png");
                    _logger.Debug("INTERNAL_STEP (GetImageFromPdf - ProcessPage): Generated temporary image path: {OutputFilePath}", outputFilePath);
                    
                    try
                    {
                        _logger.Debug("INTERNAL_STEP (GetImageFromPdf - ProcessPage): Calling PdfToPngWithGhostscriptPngDevice for page {PageNumber}.", i);
                        PdfToPngWithGhostscriptPngDevice(pdfPath, i, pdfToImageDPI, pdfToImageDPI, outputFilePath);
                        _logger.Debug("✅ **PAGE_SUCCESS**: PdfToPngWithGhostscriptPngDevice completed for page {PageNumber}.", i);
                        successfulPages++;
                    }
                    catch (TimeoutException timeoutEx)
                    {
                        _logger.Warning(timeoutEx, "⏰ **PAGE_TIMEOUT**: Page {PageNumber} conversion timed out, continuing with next page", i);
                        failedPages++;
                        // Continue processing other pages
                    }
                    catch (ThreadAbortException threadAbortEx)
                    {
                        _logger.Warning(threadAbortEx, "🚨 **PAGE_THREADABORT**: ThreadAbort on page {PageNumber}, attempting recovery", i);
                        Thread.ResetAbort(); // Reset abort for current page
                        failedPages++;
                        // Continue processing other pages
                    }
                    catch (Exception pageEx)
                    {
                        _logger.Warning(pageEx, "❌ **PAGE_FAILURE**: Failed to convert page {PageNumber}, continuing with next page. Error: {ErrorMessage}", i, pageEx.Message);
                        failedPages++;
                        // Continue processing other pages rather than failing completely
                    }
                }
                
                _logger.Information("🔍 **PROCESSING_SUMMARY**: Finished processing {TotalPages} pages. Success: {SuccessfulPages}, Failed: {FailedPages}", 
                    pageCount, successfulPages, failedPages);
                
                // **GRACEFUL_DEGRADATION**: Allow partial success rather than complete failure
                if (successfulPages == 0)
                {
                    throw new InvalidOperationException($"All {pageCount} pages failed to convert. No images could be generated from PDF.");
                }
                else if (failedPages > 0)
                {
                    _logger.Warning("⚠️ **PARTIAL_SUCCESS**: {FailedPages}/{TotalPages} pages failed, but continuing with {SuccessfulPages} successful pages", 
                        failedPages, pageCount, successfulPages);
                }

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
            _logger.Information("METHOD_ENTRY: PdfToPngWithGhostscriptPngDevice. Intention: Convert a single PDF page to PNG using Ghostscript with timeout protection. InitialState: {InitialState}", new { SourceFile = srcFile, PageNumber = pageNo, DpiX = dpiX, DpiY = dpiY, TargetFile = tgtFile });
            
            // **GHOSTSCRIPT_TIMEOUT_FIX**: Set reasonable timeout for Ghostscript operations (30 seconds)
            const int timeoutMs = 30000;
            var startTime = DateTime.UtcNow;
            
            try
            {
                GhostscriptPngDevice dev = null;
                try
                {
                    dev = new GhostscriptPngDevice(GhostscriptPngDeviceType.PngGray)
                    {
                        GraphicsAlphaBits = GhostscriptImageDeviceAlphaBits.V_4,
                        TextAlphaBits = GhostscriptImageDeviceAlphaBits.V_4,
                        ResolutionXY = new GhostscriptImageDeviceResolution(dpiX, dpiY)
                    };
                    
                    dev.InputFiles.Add(srcFile);
                    dev.Pdf.FirstPage = pageNo;
                    dev.Pdf.LastPage = pageNo;
                    dev.CustomSwitches.Add("-dDOINTERPOLATE");
                    dev.CustomSwitches.Add("-dBATCH");
                    dev.CustomSwitches.Add("-dNOPAUSE");
                    dev.CustomSwitches.Add("-dSAFER");
                    dev.OutputPath = tgtFile;
                    
                    _logger.Debug("INTERNAL_STEP (PdfToPngWithGhostscriptPngDevice - Process): Calling dev.Process() with {TimeoutMs}ms timeout.", timeoutMs);
                    
                    // **TIMEOUT_WRAPPER**: Execute Ghostscript with timeout protection
                    using (var cancellationTokenSource = new CancellationTokenSource(timeoutMs))
                    {
                        var task = Task.Run(() =>
                        {
                            try
                            {
                                // **THREADABORT_PROTECTION**: Wrap in try-catch to handle ThreadAbortException
                                dev.Process();
                                _logger.Debug("✅ **GHOSTSCRIPT_SUCCESS**: dev.Process() completed successfully");
                                return true;
                            }
                            catch (ThreadAbortException threadAbortEx)
                            {
                                _logger.Warning(threadAbortEx, "🚨 **GHOSTSCRIPT_THREADABORT**: ThreadAbortException during Ghostscript processing");
                                Thread.ResetAbort(); // Reset the abort to prevent re-throw
                                return false;
                            }
                            catch (Exception ghostscriptEx)
                            {
                                _logger.Error(ghostscriptEx, "❌ **GHOSTSCRIPT_PROCESS_ERROR**: Exception during dev.Process()");
                                return false;
                            }
                        }, cancellationTokenSource.Token);
                        
                        // Wait for completion or timeout with ThreadAbortException protection
                        bool completed = false;
                        try
                        {
                            completed = task.Wait(timeoutMs);
                        }
                        catch (System.Threading.ThreadAbortException threadAbortEx)
                        {
                            _logger.Error(threadAbortEx, "🚨 **THREADABORT_DURING_WAIT**: ThreadAbortException caught during task.Wait() in PdfToPngWithGhostscriptPngDevice");
                            
                            // **CRITICAL**: Reset thread abort to prevent automatic re-throw
                            System.Threading.Thread.ResetAbort();
                            _logger.Information("✅ **THREADABORT_RESET**: Thread abort reset successfully in PdfOcr");
                            
                            // Treat as timeout and continue with graceful degradation
                            completed = false;
                            _logger.Warning("🔄 **RECOVERY_STRATEGY**: Treating ThreadAbortException as timeout - continuing with fallback PNG creation");
                        }
                        var elapsedMs = (DateTime.UtcNow - startTime).TotalMilliseconds;
                        
                        if (!completed)
                        {
                            _logger.Warning("⏰ **GHOSTSCRIPT_TIMEOUT**: Ghostscript operation timed out after {TimeoutMs}ms for page {PageNumber}", timeoutMs, pageNo);
                            cancellationTokenSource.Cancel();
                            
                            // **GRACEFUL_DEGRADATION**: Create empty PNG file as fallback
                            try
                            {
                                File.WriteAllBytes(tgtFile, new byte[0]); // Create empty file
                                _logger.Information("🔄 **FALLBACK_CREATED**: Created empty PNG file as timeout fallback");
                            }
                            catch (Exception fallbackEx)
                            {
                                _logger.Error(fallbackEx, "❌ **FALLBACK_FAILED**: Could not create fallback PNG file");
                            }
                            
                            throw new TimeoutException($"Ghostscript operation timed out after {timeoutMs}ms for page {pageNo}");
                        }
                        
                        var success = task.Result;
                        if (!success)
                        {
                            throw new InvalidOperationException($"Ghostscript processing failed for page {pageNo}");
                        }
                        
                        _logger.Information("METHOD_EXIT_SUCCESS: PdfToPngWithGhostscriptPngDevice. IntentionAchieved: PDF page converted to PNG with timeout protection. FinalState: {FinalState}. Total execution time: {ElapsedMs:0}ms.", new { OutputFile = tgtFile, Success = true }, elapsedMs);
                    }
                }
                finally
                {
                    // **RESOURCE_CLEANUP**: GhostscriptPngDevice doesn't implement IDisposable, no cleanup needed
                    if (dev != null)
                    {
                        _logger.Debug("✅ **RESOURCE_CLEANUP**: GhostscriptPngDevice processed, no disposal needed");
                        dev = null; // Clear reference
                    }
                }
            }
            catch (TimeoutException timeoutEx)
            {
                var elapsedMs = (DateTime.UtcNow - startTime).TotalMilliseconds;
                _logger.Error(timeoutEx, "METHOD_EXIT_FAILURE: PdfToPngWithGhostscriptPngDevice. IntentionFailed: Ghostscript timeout after {ElapsedMs:0}ms for page {PageNumber} of file {SourceFile}.", elapsedMs, pageNo, srcFile);
                throw;
            }
            catch (Exception e)
            {
                var elapsedMs = (DateTime.UtcNow - startTime).TotalMilliseconds;
                _logger.Error(e, "METHOD_EXIT_FAILURE: PdfToPngWithGhostscriptPngDevice. IntentionFailed: Failed to convert PDF page {PageNumber} to PNG for file {SourceFile} after {ElapsedMs:0}ms.", pageNo, srcFile, elapsedMs);
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
