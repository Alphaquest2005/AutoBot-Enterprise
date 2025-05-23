using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AutoBot
{
    public partial class ShipmentUtils
    {
        public static void CreateInstructions()
        {
            var dir = new DirectoryInfo(@"D:\OneDrive\Clients\Rouge\2019\October"); // Keeping original path
            var files = dir.GetFiles().Where(x => Regex.IsMatch(x.Name, @".*(P|F)\d+.xml"));
            if (File.Exists(Path.Combine(dir.FullName, "Instructions.txt")))
            {
                File.Delete(Path.Combine(dir.FullName, "Instructions.txt"));
                File.Delete(Path.Combine(dir.FullName, "InstructionResults.txt"));
            }
            foreach (var file in files)
            {
                File.AppendAllText(Path.Combine(dir.FullName, "Instructions.txt"), $"File\t{file.FullName}\r\n");
                if (File.Exists(file.FullName.Replace("xml", "csv")) && !File.Exists(file.FullName.Replace("xml", "csv.pdf")))
                    File.Copy(file.FullName.Replace("xml", "csv"), file.FullName.Replace("xml", "csv.pdf"));
                File.AppendAllText(Path.Combine(dir.FullName, "Instructions.txt"), $"Attachment\t{file.FullName.Replace("xml", "csv.pdf")}\r\n");
            }
        }
    }
}