namespace WaterNut.DataSpace;

public partial class Part
{
    public List<Line> FailedLines
    {
        get
        {
            int? partId = this.OCR_Part?.Id;
            string propertyName = nameof(FailedLines);
            _logger.Verbose("Entering {PropertyName} getter for PartId: {PartId}", propertyName, partId);
            List<Line> finalFailedList = new List<Line>(); // Initialize to empty list

            try
            {
                // --- Get Direct Failed Lines ---
                _logger.Verbose("{PropertyName}: Getting direct failed lines for PartId: {PartId}...", propertyName,
                    partId);
                var directFailed = this.Lines?
                    .Where(x => x?.OCR_Lines?.Fields != null && // Safe checks
                                x.OCR_Lines.Fields.Any(z => z != null && z.IsRequired && z.FieldValue?.Value == null) &&
                                (x.Values == null || !x.Values.Any())) // Check Values null/empty
                    .ToList() ?? new List<Line>();
                _logger.Verbose("{PropertyName}: Found {Count} direct failed lines for PartId: {PartId}.", propertyName,
                    directFailed.Count, partId);


                // --- Get Child Failed Lines ---
                _logger.Verbose("{PropertyName}: Getting child failed lines for PartId: {PartId}...", propertyName,
                    partId);
                var childFailed = this.ChildParts?
                    .Where(cp => cp != null) // Safe check
                    .SelectMany(x =>
                        x.FailedLines ?? Enumerable.Empty<Line>()) // Access property recursively, handle null
                    .ToList() ?? new List<Line>();
                _logger.Verbose("{PropertyName}: Found {Count} failed lines from children for PartId: {PartId}.",
                    propertyName, childFailed.Count, partId);


                // --- Combine and Deduplicate ---
                _logger.Verbose(
                    "{PropertyName}: Combining direct ({DirectCount}) and child ({ChildCount}) failed lines for PartId: {PartId}...",
                    propertyName, directFailed.Count, childFailed.Count, partId);
                // Union implicitly handles duplicates if Line implements Equals/GetHashCode correctly, otherwise use DistinctBy
                finalFailedList = directFailed.Union(childFailed)
                    // Optional: Add DistinctBy if Union isn't sufficient
                    // .DistinctBy(l => l?.OCR_Lines?.Id)
                    // .Where(l => l != null)
                    .ToList();
                _logger.Information("{PropertyName}: Found {Count} total unique failed lines for PartId: {PartId}",
                    propertyName, finalFailedList.Count, partId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "{PropertyName}: Error during evaluation for PartId: {PartId}. Returning empty list.",
                    propertyName, partId);
                finalFailedList = new List<Line>(); // Ensure empty list on error
            }

            _logger.Verbose("Exiting {PropertyName} getter for PartId: {PartId}", propertyName, partId);
            return finalFailedList;
        }
    }
}