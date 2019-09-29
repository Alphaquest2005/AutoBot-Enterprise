
using System;


namespace pdf_ocr
{
    class Program
    {

        private static void Main(string[] args)
        {
            

            Console.WriteLine($"Recognizing text on page of file test.pdf");
            Console.WriteLine();

           
            var recognizedText = PdfOcr.Ocr("test.pdf");

            Console.WriteLine($"Recognized text on page");
            Console.WriteLine($"==========================");
            Console.WriteLine(recognizedText);
            Console.WriteLine($"==========================");

            

            Console.ReadKey();
        }

    }
}
