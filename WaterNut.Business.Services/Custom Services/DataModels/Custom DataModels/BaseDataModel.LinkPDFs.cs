using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using TrackableEntities;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    public static void LinkPDFs(List<int> entries, string docCode = "NA")
    {
        Console.WriteLine("Link PDF Files");
        var directoryName = StringExtensions.UpdateToCurrentUser(
            Path.Combine(Instance.CurrentApplicationSettings.DataFolder,
                "Imports"));

        using (var ctx = new CoreEntitiesContext())
        {
            foreach (var entryId in entries)
            {
                var doc = ctx.AsycudaDocuments.FirstOrDefault(x => x.ASYCUDA_Id == entryId);
                if (doc == null) continue;
                var fileInfos = new DirectoryInfo(directoryName).GetFiles($"*-{doc.CNumber}.pdf").ToList();
                fileInfos.AddRange(new DirectoryInfo(directoryName).GetFiles($"*-{doc.CNumber}-*.pdf").ToList());
                var csvFiles = fileInfos
                    .Where(x => Regex.IsMatch(x.FullName,
                        @".*(?<=\\)([A-Z,0-9]{3}\-[A-Z]{5}\-)(?<pCNumber>\d+).*.pdf",
                        RegexOptions.IgnoreCase)).ToArray();
                foreach (var file in csvFiles)
                {
                    var dfile = ctx.Attachments.Include(x => x.AsycudaDocument_Attachments)
                        .FirstOrDefault(x => x.FilePath == file.FullName);

                    if (dfile != null &&
                        dfile.AsycudaDocument_Attachments.Any(x => x.AsycudaDocumentId == doc.ASYCUDA_Id)) continue;
                    var mat = Regex.Match(file.FullName,
                        @".*(?<=\\)([A-Z,0-9]{3}\-[A-Z]{5}\-)(?<pCNumber>\d+).*.pdf",
                        RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                    if (!mat.Success) continue;

                    var attachment = new Attachments(true)
                    {
                        FilePath = file.FullName,
                        DocumentCode = docCode,
                        Reference = file.Name.Replace(file.Extension, ""),
                        TrackingState = TrackingState.Added
                    };
                    ctx.AsycudaDocument_Attachments.Add(
                        new CoreEntities.Business.Entities.AsycudaDocument_Attachments(true)
                        {
                            AsycudaDocumentId = entryId,
                            Attachments = attachment,

                            TrackingState = TrackingState.Added
                        });

                    ctx.AsycudaDocumentSet_Attachments.Add(
                        new CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments(true)
                        {
                            AsycudaDocumentSetId = doc.AsycudaDocumentSetId.GetValueOrDefault(),
                            Attachments = attachment,

                            TrackingState = TrackingState.Added
                        });

                    ctx.SaveChanges();
                }
            }
        }
    }
}