﻿
using System;
using Tesseract;


namespace pdf_ocr
{
    class Program
    {

        private static void Main(string[] args)
        {
            

            Console.WriteLine($"Recognizing text on page of file test.pdf");
            Console.WriteLine();

           
            var recognizedText = new PdfOcr().Ocr("test.pdf", PageSegMode.SingleColumn);

            Console.WriteLine($"Recognized text on page");
            Console.WriteLine($"=====");
            Console.WriteLine(recognizedText);
            Console.WriteLine($"=====");

            

            Console.ReadKey();
        }

    }
}
