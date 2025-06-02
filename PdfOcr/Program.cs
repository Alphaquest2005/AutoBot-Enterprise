
using System;
using Tesseract;
using Serilog; // Added

namespace pdf_ocr
{
    class Program
    {

        private static void Main(string[] args)
        {
            // Configure a basic Serilog logger for this utility
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Recognizing text on page of file test.pdf");


            var recognizedText = new PdfOcr(Log.Logger).Ocr("test.pdf", PageSegMode.SingleColumn); // Pass logger

            Log.Information("Recognized text on page");
            Log.Information("=====");
            Log.Information("{RecognizedText}", recognizedText);
            Log.Information("=====");


            Log.CloseAndFlush(); // Ensure logs are written

            Console.ReadKey();
        }

    }
}
