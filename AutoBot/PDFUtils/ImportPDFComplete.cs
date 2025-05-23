using System;
using System.IO;
using System.Linq;

namespace AutoBot
{
    public partial class PDFUtils
    {
        private static bool ImportPDFComplete(string directoryName, out int lcont)
        {
            lcont = 0;

            var desFolder = directoryName + "\\";
            if (File.Exists(Path.Combine(desFolder, "OverView-PDF.txt")))
            {
                var lines = File.ReadAllText(Path.Combine(directoryName, "OverView-PDF.txt"))
                    .Split(new[] { $"\r\n{DateTime.Now.Year}\t" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                {

                    return false;
                }

                var existingfiles = 0;

                foreach (var line in lines)
                {
                    lcont += 1;

                    var p = line.StartsWith($"{DateTime.Now.Year}\t")
                        ? line.Replace($"{DateTime.Now.Year}\t", "").Split('\t')
                        : line.Split('\t');
                    if (p.Length < 8) continue;
                    if (File.Exists(Path.Combine(desFolder, $"{p[7] + p[8]}-{p[0]}-{p[5]}.pdf"))
                        && File.Exists(Path.Combine(desFolder, $"{p[7] + p[8]}-{p[0]}-{p[5]}-Assessment.pdf")))
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