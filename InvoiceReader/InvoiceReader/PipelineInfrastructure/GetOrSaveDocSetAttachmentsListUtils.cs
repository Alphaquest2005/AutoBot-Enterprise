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
        private static List<AsycudaDocumentSet_Attachments> GetOrSaveDocSetAttachmentsList(
            List<AsycudaDocumentSet> asycudaDocumentSets, string file, string emailId,
            int fileTypeId, FileInfo fileInfo) =>
            asycudaDocumentSets.SelectMany(docSet => GetDocSetAttachements(file, emailId, fileTypeId, fileInfo, docSet))
                .ToList();
    }
}