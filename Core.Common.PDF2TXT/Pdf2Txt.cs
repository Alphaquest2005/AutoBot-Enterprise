using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;
using Serilog;
using System.Diagnostics;

namespace Core.Common.PDF2TXT
{
    public class Pdf2Txt
    {
         private static readonly Pdf2Txt instance;
         static Pdf2Txt()
        {


            instance = new Pdf2Txt();

        }

         public static Pdf2Txt Instance
        {
            get { return instance; }
        }

        public string ExtractTextFromPdf(string path, ILogger logger)
        {
            var stopwatch = Stopwatch.StartNew();
            logger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]", nameof(ExtractTextFromPdf), "Process PDF file for text extraction", $"FilePath: {path}");

            PDDocument doc = null;
            try
            {
                logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}", nameof(ExtractTextFromPdf), "Loading", "Attempting to load PDF document.", $"FilePath: {path}", "");
                doc = PDDocument.load(path);
                logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}", nameof(ExtractTextFromPdf), "Loading", "PDF document loaded successfully.", $"FilePath: {path}", "");

                logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}", nameof(ExtractTextFromPdf), "Extraction", "Attempting to create PDFTextStripper.", "", "");
                var stripper = new PDFTextStripper();
                logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}", nameof(ExtractTextFromPdf), "Extraction", "PDFTextStripper created.", "", "");

                logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}", nameof(ExtractTextFromPdf), "Extraction", "Attempting to extract text.", "", "");
                var extractedText = stripper.getText(doc);
                logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}", nameof(ExtractTextFromPdf), "Extraction", "Text extracted successfully.", $"ExtractedTextLength: {extractedText.Length}", "");

                stopwatch.Stop();
                logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.", nameof(ExtractTextFromPdf), "PDF processed and text extracted", $"ExtractedTextLength: {extractedText.Length}", stopwatch.ElapsedMilliseconds);
                return extractedText;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}", nameof(ExtractTextFromPdf), "Process PDF file for text extraction", stopwatch.ElapsedMilliseconds, ex.Message);
                throw;
            }
            finally
            {
                if (doc != null)
                {
                    doc.close();
                }
            }

        }
    }
}
