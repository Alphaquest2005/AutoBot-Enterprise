using System;
using System.IO;
using System.Linq;

namespace AutoBot
{
    public partial class LICUtils
    {
        public static bool ImportLICComplete(string directoryName, out int lcont)
        {
            lcont = 0;

            var overviewFile = Path.Combine(directoryName + "\\", "LICOverView-PDF.txt");
            if (File.Exists(overviewFile))
            {
                if (File.GetLastWriteTime(overviewFile) <= DateTime.Now.AddHours(-1)) return false;
                var lines = File.ReadAllText(Path.Combine(directoryName, "LICOverView-PDF.txt"))
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
                    if (p.Length < 5) continue;
                    // Potential IndexOutOfRangeException if p has less than 4 elements
                    if (string.IsNullOrEmpty(p[3])
                        && (DateTime.Now - File.GetLastWriteTime(Path.Combine(directoryName, "LICOverView-PDF.txt"))).TotalMinutes > 60) return false;
                    // Potential IndexOutOfRangeException if p has less than 4 elements
                    if (File.Exists(Path.Combine(directoryName, $"{p[3]}-LIC.xml")))
                    {
                        existingfiles += 1;
                        continue;
                    }
                    return false;
                }

                if (lines.Length == lcont && existingfiles == 0)
                    return true;
                else
                    return existingfiles != 0;
            }
            else
            {
                return false;
            }
        }
    }
}