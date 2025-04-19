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
        public const string CommandsTxt = "Commands:\r\n" +
                                          "UpdateRegex: RegId: 000, Regex: 'xyz', IsMultiline: True\r\n" +
                                          "AddFieldRegEx: RegId: 000,  Field: Name, Regex: '', ReplaceRegex: ''\r\n" +
                                          "RequestInvoice: Name:'xyz'\r\n" +
                                          "AddInvoice: Name:'', IDRegex:''\r\n" +
                                          "AddPart: Invoice:'', Name: '', StartRegex: '', ParentPart:'', IsRecurring: True, IsComposite: False, IsMultiLine: True \r\n" +
                                          "AddLine: Invoice:'',  Part: '', Name: '', Regex: ''\r\n" +
                                          "UpdateLine: Invoice:'',  Part: '', Name: '', Regex: ''\r\n" +
                                          "AddFieldFormatRegex: RegexId: 000, Keyword:'', Regex:'', ReplaceRegex:'', ReplacementRegexIsMultiLine: True, RegexIsMultiLine: True\r\n";
    }


}