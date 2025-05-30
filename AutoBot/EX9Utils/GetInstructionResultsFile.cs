using System.IO;
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace AutoBot
{
    public partial class EX9Utils
    {
        // Expression-bodied member
        private static string GetInstructionResultsFile(string docReference) => Path.Combine(BaseDataModel.GetDocSetDirectoryName(docReference), "InstructionResults.txt"); // Assuming GetDocSetDirectoryName exists
    }
}