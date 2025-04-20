namespace WaterNut.DataSpace;

public partial class Part
{
    private bool AllChildPartsSucceded()
    {
        int? partId = this.OCR_Part?.Id;
        string methodName = nameof(AllChildPartsSucceded);
        _logger.Verbose("Entering {MethodName} for PartId: {PartId}", methodName, partId);

        if (this.ChildParts == null)
        {
            _logger.Warning(
                "{MethodName}: ChildParts collection is null for PartId: {PartId}. Assuming success (returning true).",
                methodName, partId);
            _logger.Verbose("Exiting {MethodName} for PartId: {PartId} (null ChildParts).", methodName, partId);
            return true; // No children means success in this context
        }

        bool allSucceeded = true; // Assume success initially
        int childIndex = 0;
        foreach (var childPart in this.ChildParts)
        {
            childIndex++;
            if (childPart == null)
            {
                _logger.Warning("{MethodName}: Found null child part at index {Index} for PartId: {PartId}. Skipping.",
                    methodName, childIndex - 1, partId);
                continue; // Skip null children
            }

            int? childPartId = childPart.OCR_Part?.Id;
            // Access Success property of child, which triggers its evaluation and logging
            bool childSuccess = childPart.Success;
            _logger.Verbose(
                "{MethodName}: PartId: {PartId} - Child Part {Index}/{Total} (Id: {ChildPartId}) Success: {ChildSuccess}",
                methodName, partId, childIndex, this.ChildParts.Count, childPartId, childSuccess);

            if (!childSuccess)
            {
                allSucceeded = false;
                // Optional: break here if we only care if *any* child failed
                // break;
            }
        }

        _logger.Verbose(
            "{MethodName}: Final evaluation result for PartId: {PartId}: {Result} (Checked {Count} non-null children)",
            methodName, partId, allSucceeded, this.ChildParts.Count(cp => cp != null));
        _logger.Verbose("Exiting {MethodName} for PartId: {PartId}", methodName, partId);
        return allSucceeded;
    }
}