namespace WaterNut.DataSpace;

public partial class Part
{
    public List<Line> AllLines
    {
        get
        {
            int? partId = this.OCR_Part?.Id;
            string propertyName = nameof(AllLines);
            _logger.Verbose("Entering {PropertyName} getter for PartId: {PartId}", propertyName, partId);
            List<Line> finalAllLines = new List<Line>(); // Initialize

            try
            {
                // --- Get Direct Lines ---
                _logger.Verbose("{PropertyName}: Getting direct lines for PartId: {PartId}...", propertyName, partId);
                var directLines = this.Lines ?? new List<Line>();
                _logger.Verbose("{PropertyName}: Found {Count} direct lines for PartId: {PartId}.", propertyName,
                    directLines.Count, partId);

                // --- Get Child Lines ---
                _logger.Verbose("{PropertyName}: Getting child lines for PartId: {PartId}...", propertyName, partId);
                var childLines = this.ChildParts?
                    .Where(cp => cp != null) // Safe check
                    .SelectMany(x => x.AllLines ?? Enumerable.Empty<Line>()) // Access property recursively, handle null
                    .ToList() ?? new List<Line>();
                _logger.Verbose("{PropertyName}: Found {Count} lines from children for PartId: {PartId}.", propertyName,
                    childLines.Count, partId);


                // --- Combine and Deduplicate ---
                _logger.Verbose(
                    "{PropertyName}: Combining direct ({DirectCount}) and child ({ChildCount}) lines for PartId: {PartId}...",
                    propertyName, directLines.Count, childLines.Count, partId);
                finalAllLines = directLines.Union(childLines)
                    .DistinctBy(x => x?.OCR_Lines?.Id) // Safe DistinctBy using OCR_Lines ID
                    .Where(l => l != null) // Filter nulls post-distinct
                    .ToList();
                _logger.Information(
                    "{PropertyName}: Found {Count} total distinct lines (direct + child) for PartId: {PartId}",
                    propertyName, finalAllLines.Count, partId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "{PropertyName}: Error during evaluation for PartId: {PartId}. Returning empty list.",
                    propertyName, partId);
                finalAllLines = new List<Line>(); // Ensure empty list on error
            }

            _logger.Verbose("Exiting {PropertyName} getter for PartId: {PartId}", propertyName, partId);
            return finalAllLines;
        }
    }
}