namespace WaterNut.DataSpace;

public partial class Part
{
    public void Reset()
    {
        ResetInternalState();
        _instance = 1;
        _lastProcessedParentInstance = 0;

        Console.WriteLine($"[OCR DEBUG] Part.Reset: FULL Resetting Part ID {OCR_Part.Id}");
        ChildParts.ForEach(child => child.Reset());
    }
}