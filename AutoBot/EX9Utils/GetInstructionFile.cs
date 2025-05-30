using System.IO;
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class EX9Utils
    {
        // Expression-bodied member
        private static string GetInstructionFile(string docReference) => Path.Combine(BaseDataModel.GetDocSetDirectoryName(docReference), "Instructions.txt"); // Assuming GetDocSetDirectoryName exists
    }
}