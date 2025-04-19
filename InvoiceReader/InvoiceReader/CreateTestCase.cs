using Core.Common;
using Core.Common.Extensions;
using Core.Common.Utils;

namespace WaterNut.DataSpace;

public partial class InvoiceReader
{
    private static void CreateTestCase(string file, List<Line> failedlst, string txtFile, string body)
    {
        dynamic testCaseData = new BetterExpando();
        testCaseData.DateTime = DateTime.Now;
        testCaseData.Id = failedlst.FirstOrDefault()?.OCR_Lines.Parts.Invoices.Id;
        testCaseData.Supplier = failedlst.FirstOrDefault()?.OCR_Lines.Parts.Invoices.Name;
        testCaseData.PdfFile = file;
        testCaseData.TxtFile = txtFile;
        testCaseData.Message = body;
        //write to info
        UnitTestLogger.Log(
            new List<String>
                { FunctionLibary.NameOfCallingClass(), failedlst.FirstOrDefault()?.OCR_Lines.Parts.Invoices.Name, },
            BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, testCaseData);
    }
}