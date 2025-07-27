using Core.Common.Extensions;
using Core.Common; // Added for BetterExpando
using System.Collections.Generic; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added
using OCR.Business.Entities; // Added for Part, Line

namespace WaterNut.DataSpace
{
    public partial class Template
    {
        // Logger instance is defined in the main Template.cs partial class file.
        // Assuming 'table' is a static or instance member accessible here, e.g.:
        // private static Dictionary<string, List<BetterExpando>> table = new Dictionary<string, List<BetterExpando>>();

        private static BetterExpando CreateOrGetDitm(Part part, Line line, int i, BetterExpando itm,
            ref IDictionary<string, object> ditm, List<IDictionary<string, object>> lst, ILogger logger)
        {
            // Add logging context if possible (e.g., part ID, line ID)
            int? partId = part?.OCR_Part?.Id;
            int? lineId = line?.OCR_Lines?.Id;
            logger.Verbose("Entering CreateOrGetDitm for PartId: {PartId}, LineId: {LineId}, Index: {Index}", partId,
                lineId, i);

            // Null checks for critical inputs
            if (part?.OCR_Part == null)
            {
                logger.Warning(
                    "CreateOrGetDitm called with null Part or OCR_Part. Returning original 'itm'. PartId: {PartId}, LineId: {LineId}",
                    partId, lineId);
                return itm; // Cannot proceed without part info
            }

            // Check line and fields needed later
            if (line?.OCR_Lines?.Fields == null)
            {
                logger.Warning(
                    "CreateOrGetDitm called with null Line, OCR_Lines, or Fields. Returning original 'itm'. PartId: {PartId}, LineId: {LineId}",
                    partId, lineId);
                return itm; // Cannot proceed without line/field info
            }
            // lst is only used if not a column line, check later if needed


            try
            {
                // Check if part is recurring and not composite
                bool isRecurringNonComposite = part.OCR_Part.RecuringPart != null &&
                                               part.OCR_Part.RecuringPart.IsComposite == false;
                logger.Verbose("PartId: {PartId} - IsRecurringNonComposite: {IsRecurringNonComposite}", partId,
                    isRecurringNonComposite);

                if (isRecurringNonComposite)
                {
                    bool isColumn = line.OCR_Lines?.IsColumn ?? false; // Safe access
                    logger.Verbose("LineId: {LineId} - IsColumn: {IsColumn}", lineId, isColumn);

                    if (isColumn)
                    {
                        // Logic involving the 'table' variable
                        var firstField = line.OCR_Lines.Fields.FirstOrDefault(); // Already checked Fields is not null
                        if (firstField == null)
                        {
                            logger.Warning(
                                "LineId: {LineId} IsColumn is true, but has no fields. Cannot determine EntityType. Returning original 'itm'.",
                                lineId);
                            return itm;
                        }

                        string entityType = firstField.EntityType;
                        logger.Verbose("LineId: {LineId} - First Field EntityType: {EntityType}", lineId, entityType);

                        // Check if entityType exists in table and table itself exists
                        if (table == null)
                        {
                            logger.Error(
                                "Static 'table' dictionary is null. Cannot create/get item for column line. PartId: {PartId}, LineId: {LineId}",
                                partId, lineId);
                            return itm; // Cannot proceed
                        }

                        if (!table.ContainsKey(entityType))
                        {
                            logger.Warning(
                                "'table' dictionary does not contain key: {EntityType}. Initializing empty list for this type.",
                                entityType);
                            table[entityType] = new List<BetterExpando>(); // Initialize if missing
                        }

                        var entityList = table[entityType];
                        logger.Verbose("Accessing table['{EntityType}']. Current Count: {Count}", entityType,
                            entityList.Count); // List guaranteed non-null now

                        // Corrected index check: index 'i' should be less than Count
                        if (i < 0 || i >= entityList.Count)
                        {
                            logger.Debug(
                                "Index {Index} is out of bounds for table['{EntityType}'] (Count: {Count}). Creating new BetterExpando.",
                                i, entityType, entityList.Count);
                            itm = new BetterExpando();
                            // Add the new item to the table's list
                            entityList.Add(itm);
                            logger.Verbose("Added new BetterExpando to table['{EntityType}']. New Count: {Count}",
                                entityType, entityList.Count);
                        }
                        else
                        {
                            logger.Debug(
                                "Index {Index} is within bounds for table['{EntityType}']. Retrieving existing BetterExpando.",
                                i, entityType);
                            // Retrieve existing item
                            itm = entityList[i];
                            if (itm == null)
                            {
                                // This case might indicate data corruption or unexpected nulls in the list
                                logger.Warning(
                                    "Retrieved null item from table['{EntityType}'][{Index}]. Creating new BetterExpando instead.",
                                    entityType, i);
                                itm = new BetterExpando();
                                entityList[i] = itm; // Replace null in list if desired
                            }
                        }
                    }
                    else // Not a column line
                    {
                        logger.Debug(
                            "LineId: {LineId} is not a column line. Using 'lst' (List<IDictionary<string, object>>).",
                            lineId);
                        // Check if lst is valid before using ElementAtOrDefault
                        if (lst == null)
                        {
                            logger.Warning(
                                "'lst' is null for non-column LineId: {LineId}. Creating new BetterExpando.", lineId);
                            itm = new BetterExpando();
                        }
                        else
                        {
                            // Use ElementAtOrDefault for safety
                            var existingDict = (i >= 0) ? lst.ElementAtOrDefault(i) : null;
                            if (existingDict != null)
                            {
                                logger.Debug(
                                    "Found existing dictionary at index {Index} in 'lst'. Attempting cast to BetterExpando.",
                                    i);
                                // Revert to original cast logic, assuming items in lst are castable.
                                try
                                {
                                    // The ?? new BetterExpando() handles the case where ElementAtOrDefault returns null *or* the cast results in null (though direct cast usually throws InvalidCastException)
                                    itm = (BetterExpando)existingDict ?? new BetterExpando();
                                    if (itm == null)
                                    {
                                        // This condition is less likely if cast succeeds or ElementAtOrDefault was non-null
                                        logger.Warning(
                                            "Cast resulted in null BetterExpando at index {Index}. Creating new.", i);
                                        itm = new BetterExpando();
                                    }
                                }
                                catch (InvalidCastException castEx)
                                {
                                    logger.Error(castEx,
                                        "InvalidCastException: Cannot cast item from 'lst' at index {Index} to BetterExpando. Actual type might be {ActualType}. Creating new BetterExpando.",
                                        i, existingDict.GetType().FullName);
                                    itm = new BetterExpando();
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex,
                                        "Error casting item from 'lst' at index {Index} to BetterExpando. Creating new BetterExpando.",
                                        i);
                                    itm = new BetterExpando();
                                }
                            }
                            else
                            {
                                logger.Debug(
                                    "No existing dictionary found at index {Index} in 'lst' (or index invalid). Creating new BetterExpando.",
                                    i);
                                itm = new BetterExpando();
                                // Decide if the new item should be added to lst. Original code doesn't.
                                // If it should be added, need careful index handling:
                                // if (i >= 0 && i == lst.Count) { lst.Add(itm); } else { /* Handle insert or error */ }
                            }
                        }
                    }

                    // Update the ref parameter 'ditm' safely
                    logger.Verbose(
                        "Updating 'ditm' reference to the retrieved/created BetterExpando (as IDictionary).");
                    // Ensure itm is not null before casting
                    ditm = itm != null ? ((IDictionary<string, object>)itm) : new Dictionary<string, object>();
                    if (itm == null)
                    {
                        logger.Warning(
                            "Resulting 'itm' was null before casting to 'ditm'. 'ditm' set to new empty dictionary.");
                    }

                }
                else // Not recurring or is composite
                {
                    logger.Verbose(
                        "PartId: {PartId} is not recurring or is composite. No item creation/retrieval needed. 'itm' and 'ditm' remain unchanged from input.",
                        partId);
                    // In this case, 'itm' remains its original value passed in, and 'ditm' also remains unchanged.
                }

                logger.Verbose("Exiting CreateOrGetDitm for PartId: {PartId}, LineId: {LineId}. Returning 'itm'.",
                    partId, lineId);
                return itm; // Return the created or retrieved BetterExpando (or the original one)
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error during CreateOrGetDitm for PartId: {PartId}, LineId: {LineId}, Index: {Index}",
                    partId, lineId, i);
                // Decide on return value on error - original itm? null?
                // Reset ditm to avoid using potentially corrupted state from within try block
                ditm = null;
                return itm; // Return original itm as a fallback
            }
        }
    }
}