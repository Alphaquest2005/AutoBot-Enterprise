using Serilog; // Added
using System; // Added

namespace WaterNut.DataSpace
{
    public partial class Part
    {
        // Assuming _logger exists from another partial part
        // private static readonly ILogger _logger = Log.ForContext<Part>();

        private void ResetInternalState()
        {
            int? partId = this.OCR_Part?.Id; // For logging context
            _logger.Debug("Entering ResetInternalState for PartId: {PartId}", partId);

            try
            {
                _logger.Verbose("Clearing _startlines (Count: {Count})", _startlines?.Count ?? 0);
                _startlines?.Clear(); // Use null-conditional operator for safety

                _logger.Verbose("Clearing _endlines (Count: {Count})", _endlines?.Count ?? 0);
                _endlines?.Clear(); // Use null-conditional operator

                _logger.Verbose("Clearing _lines (Count: {Count})", _lines?.Count ?? 0);
                _lines?.Clear(); // Use null-conditional operator

                _logger.Verbose("Clearing _instanceLinesTxt (Length: {Length})", _instanceLinesTxt?.Length ?? 0);
                _instanceLinesTxt?.Clear(); // Use null-conditional operator

                _logger.Verbose("Resetting lastLineRead to 0 (was {PreviousValue})", lastLineRead);
                lastLineRead = 0;

                _logger.Verbose("Resetting _currentInstanceStartLineNumber to -1 (was {PreviousValue})",
                    _currentInstanceStartLineNumber);
                _currentInstanceStartLineNumber = -1;

                // Note: _instance and _lastProcessedParentInstance are reset in the public Reset() method, not here.

                _logger.Debug("Finished ResetInternalState for PartId: {PartId}", partId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during ResetInternalState for PartId: {PartId}", partId);
                // Decide if exception should be propagated
            }
        }
    }
}