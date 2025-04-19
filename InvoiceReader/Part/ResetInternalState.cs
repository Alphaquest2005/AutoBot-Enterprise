namespace WaterNut.DataSpace;

public partial class Part
{
    private void ResetInternalState()
    {
        _startlines.Clear();
        _endlines.Clear();
        _lines.Clear();
        _instanceLinesTxt.Clear();
        lastLineRead = 0;
        _currentInstanceStartLineNumber = -1;
    }
}