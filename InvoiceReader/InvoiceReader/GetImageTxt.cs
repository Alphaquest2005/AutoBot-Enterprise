using pdf_ocr;
using Tesseract;

namespace WaterNut.DataSpace;

public partial class InvoiceReader
{
    public static string GetImageTxt(string directoryName)
    {
        var str = PdfOcr.GetTextFromImage(PageSegMode.SingleColumn, directoryName,
            Path.Combine(directoryName, "AllImagetxt"), false);


        return str;
    }
}