using Serilog; // Added
using System; // Added
using System.Linq; // Added for Any()
using Core.Common.Extensions; // Added for ForEach
using System.Collections.Generic; // Added for List<>

namespace WaterNut.DataSpace
{

    public partial class Part
    {
        // Assuming _logger exists from another partial part
        // private static readonly ILogger _logger = Log.ForContext<Part>();

        public void Reset()
        {
            int? partId = this.OCR_Part?.Id; // Safe access for logging
            _logger.Information("Entering Part.Reset for PartId: {PartId}", partId);

            try
            {
                ResetInternalStateWithLogging(partId);
                ResetInstanceFieldsWithLogging(partId);
                ResetChildPartsWithLogging(partId);

                _logger.Information("Finished Part.Reset for PartId: {PartId}", partId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during Part.Reset for PartId: {PartId}", partId);
                // Decide if exception should be propagated
            }
        }

        private void ResetInternalStateWithLogging(int? partId)
        {
            _logger.Debug("PartId: {PartId} - Calling ResetInternalState.", partId);
            ResetInternalState(); // Assuming this handles its own logging
        }

        private void ResetInstanceFieldsWithLogging(int? partId)
        {
            _logger.Debug("PartId: {PartId} - Resetting _instance to 1 (was {PreviousValue}).", partId, _instance);
            _instance = 1;

            _logger.Debug("PartId: {PartId} - Resetting _lastProcessedParentInstance to 0 (was {PreviousValue}).",
                partId, _lastProcessedParentInstance);
            _lastProcessedParentInstance = 0;
        }

        private void ResetChildPartsWithLogging(int? partId)
        {
            if (ChildParts != null && ChildParts.Any())
            {
                _logger.Debug("PartId: {PartId} - Recursively calling Reset() on {Count} child parts.", partId,
                    ChildParts.Count);
                ChildParts.ForEach(child =>
                {
                    if (child != null)
                    {
                        child.Reset(); // Child Reset handles its own logging
                    }
                    else
                    {
                        _logger.Warning(
                            "Skipping Reset() call on null child part for Parent PartId: {ParentPartId}", partId);
                    }
                });
            }
            else
            {
                _logger.Debug("PartId: {PartId} - No child parts to reset.", partId);
            }
        }

        // Assuming ResetInternalState exists in another partial class part
        // private void ResetInternalState() { ... }
    }
}