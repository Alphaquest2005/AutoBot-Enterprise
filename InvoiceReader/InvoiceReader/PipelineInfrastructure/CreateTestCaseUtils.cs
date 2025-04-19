using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Core.Common;
using Core.Common.Extensions;
using Core.Common.Utils;
using DocumentDS.Business.Entities;
using EmailDownloader;
using CoreEntities.Business.Entities;
using OCR.Business.Entities;
using pdf_ocr;
using Tesseract;
using TrackableEntities;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using WaterNut.Business.Services.Utils;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using Attachments = CoreEntities.Business.Entities.Attachments;
using FileTypes = CoreEntities.Business.Entities.FileTypes;
using Invoices = OCR.Business.Entities.Invoices;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public static partial class InvoiceProcessingUtils
    {
        public static void CreateTestCase(string file, List<Line> failedlst, string txtFile, string body)
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
}