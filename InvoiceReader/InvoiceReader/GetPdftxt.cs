//using System.Text;
//using pdf_ocr;
//using Tesseract;

//namespace WaterNut.DataSpace;

//public partial class InvoiceReader
//{
//    public static async Task<StringBuilder> GetPdftxt(string file)
//    {
//        StringBuilder pdftxt = new StringBuilder();


//        //pdftxt = parseUsingPDFBox(file);
//        var ripTask = Task.Run(() =>
//        {
//            var txt = "------------------------------------------Ripped Text-------------------------\r\n";
//            txt += PdfPigText(file); //TODO: need to implement the layout logic
//            return txt;
//        });


//        var singleColumnTask = Task.Run(() =>
//        {
//            var txt =
//                "------------------------------------------Single Column-------------------------\r\n";
//            txt += new PdfOcr().Ocr(file, PageSegMode.SingleColumn);
//            return txt;
//        });

//        var sparseTextTask = Task.Run(() =>
//        {
//            var txt = "------------------------------------------SparseText-------------------------\r\n";
//            txt += new PdfOcr().Ocr(file, PageSegMode.SparseText);
//            return txt;
//        });


//        await Task.WhenAll(ripTask, singleColumnTask, sparseTextTask).ConfigureAwait(false); //RawLineTextTask, OsdOnlyTextTask, SingleWordTextTask, sparsOsdTextTask

//        pdftxt.AppendLine(singleColumnTask.Result);
//        pdftxt.AppendLine(sparseTextTask.Result);
//        pdftxt.AppendLine(ripTask.Result);
//        return pdftxt;
//    }
//}