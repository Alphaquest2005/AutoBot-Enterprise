using CoreEntities.Business.Entities;
using OCR.Business.Entities;
using TrackableEntities;
using System.Linq; // Added
using System.Collections.Generic; // Added
using Serilog; // Added
using System;
using MoreLinq; // Added

namespace WaterNut.DataSpace.PipelineInfrastructure
{

    public static partial class InvoiceProcessingUtils
    {
        // Assuming _utilsLogger is defined in another partial class part
        // private static readonly ILogger _utilsLogger = Log.ForContext(typeof(InvoiceProcessingUtils));

        private static OCR_FailedLines CreateFailedLines(AsycudaDocumentSet_Attachments att, Line x)
        {
            // Add null checks for inputs
            if (att == null)
            {
                _utilsLogger.Error("CreateFailedLines called with null AsycudaDocumentSet_Attachments.");
                return null; // Or throw
            }

            if (x == null || x.OCR_Lines == null)
            {
                // Use att.Id if available, otherwise indicate null attachment
                _utilsLogger.Error(
                    "CreateFailedLines called with null Line or null Line.OCR_Lines for AttachmentId: {AttachmentId}.",
                    att?.Id ?? -1);
                return null; // Or throw
            }

            int lineId = x.OCR_Lines.Id; // Cache for logging
            _utilsLogger.Debug("Creating FailedLines object for AttachmentId: {AttachmentId}, LineId: {LineId}", att.Id,
                lineId);

            OCR_FailedLines failedLine = null;
            try
            {
                _utilsLogger.Verbose("Processing FailedFields for LineId: {LineId}", lineId);
                // Corrected LINQ based on the actual type: List<Dictionary<string, List<KeyValuePair<(Fields fields, int instance), string>>>>
                var failedFieldsList = new List<OCR_FailedFields>();
                if (x.FailedFields != null)
                {
                    var distinctFields = x.FailedFields
                        .Where(dict => dict != null) // Filter out null dictionaries in the list
                        .SelectMany(dict =>
                            dict.Values) // Get all List<KVP<(Fields, int), string>> from dictionary values
                        .SelectMany(list =>
                            list ?? Enumerable
                                .Empty<
                                    KeyValuePair<(Fields fields, int instance),
                                        string>>()) // Flatten the lists of KVPs, handling null lists
                        .Where(kvp => kvp.Key.fields != null) // Ensure the 'fields' part of the tuple Key is not null
                        .Select(kvp => kvp.Key.fields) // Select the Fields object
                        .DistinctBy(f => f.Id) // Get distinct Fields by Id
                        .ToList(); // Materialize

                    _utilsLogger.Verbose("Found {DistinctFieldCount} distinct failed fields for LineId: {LineId}",
                        distinctFields.Count, lineId);

                    failedFieldsList = distinctFields
                        .Select(f => new OCR_FailedFields(true)
                        {
                            TrackingState = TrackingState.Added,
                            FieldId = f.Id,
                            // FailedLineId will be set by EF relationship if configured, or needs manual assignment if required before SaveChanges elsewhere
                        })
                        .ToList();
                    _utilsLogger.Verbose(
                        "Created {FailedFieldObjectCount} OCR_FailedFields objects for LineId: {LineId}",
                        failedFieldsList.Count, lineId);
                }
                else
                {
                    _utilsLogger.Debug("Line.FailedFields is null for LineId: {LineId}", lineId);
                }


                failedLine = new OCR_FailedLines(true)
                {
                    TrackingState = TrackingState.Added,
                    DocSetAttachmentId = att.Id,
                    LineId = lineId, // Use cached ID
                    Resolved = false,
                    OCR_FailedFields = failedFieldsList // Use the processed list
                };
                _utilsLogger.Debug(
                    "Successfully created OCR_FailedLines object for AttachmentId: {AttachmentId}, LineId: {LineId} with {FailedFieldCount} associated fields.",
                    att.Id, lineId, failedFieldsList.Count);
            }
            catch (Exception ex)
            {
                _utilsLogger.Error(ex,
                    "Error creating OCR_FailedLines for AttachmentId: {AttachmentId}, LineId: {LineId}", att.Id,
                    lineId);
                // Depending on requirements, return null or rethrow
                return null;
            }

            return failedLine;
        }
    }
}