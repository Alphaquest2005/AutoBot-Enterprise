using System;
using System.IO;
using System.Linq;

namespace AutoBot
{
    public partial class C71Utils
    {
        public static bool ImportC71Complete(string directoryName, out int lcont)
        {
            lcont = 0;

            var desFolder = directoryName + "\\";
            var overviewFile = Path.Combine(desFolder, "C71OverView-PDF.txt");
            if (File.Exists(overviewFile))
            {
                if (File.GetLastWriteTime(overviewFile) <= DateTime.Now.AddMinutes(-10)) return false;
                var lines = File.ReadAllText(Path.Combine(directoryName, "C71OverView-PDF.txt"))
                    .Split(new[] { $"\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                {
                    return false;
                }

                if (lines.FirstOrDefault() == "No Data" && File.GetLastWriteTime(overviewFile) <= DateTime.Now.AddHours(-0.25)) return false;
                if (lines.FirstOrDefault() == "No Data") return true;

                var existingfiles = 0;

                foreach (var line in lines)
                {
                    lcont += 1;

                    var p = line.Split('\t');
                    if (p.Length < 3) continue;
                    if (File.Exists(Path.Combine(desFolder, $"{p[1]}-C71.xml")))
                    {
                        existingfiles += 1;
                        continue;
                    }
                    return false;
                }

                return existingfiles != 0;
            }
            else
            {
                return false;
            }
        }
    }
}