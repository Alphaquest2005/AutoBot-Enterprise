
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

            Console.WriteLine($"Recognizing text on page of file test.pdf");
            Console.WriteLine();

           
            var recognizedText = new PdfOcr(Log.Logger).Ocr("test.pdf", PageSegMode.SingleColumn); // Pass logger

            Console.WriteLine($"Recognized text on page");
            Console.WriteLine($"=====");
            Console.WriteLine(recognizedText);
            Console.WriteLine($"=====");

            
            Log.CloseAndFlush(); // Ensure logs are written

            Console.ReadKey();
        }

    }
}
