using System.IO;

namespace AutoBot
{
    public partial class C71Utils
    {
        public static bool AssessC71Complete(string instrFile, string resultsFile, out int lcont)
        {
            lcont = 0;

            if (File.Exists(instrFile))
            {
                if (!File.Exists(resultsFile)) return false;
                var lines = File.ReadAllLines(instrFile);
                var res = File.ReadAllLines(resultsFile);
                if (res.Length == 0)
                {
                    return false;
                }

                foreach (var line in lines)
                {
                    var p = line.Split('\t');
                    if (lcont >= res.Length) return false;
                    if (string.IsNullOrEmpty(res[lcont])) return false;
                    var r = res[lcont].Split('\t');
                    lcont += 1;
                    // Potential IndexOutOfRangeException if p or r have less than 2 or 5 elements respectively
                    if (p.Length > 1 && r.Length > 4 && p[1] == r[1] && r[4] == "Success")
                    {
                        continue;
                    }
                    return false;
                }

                return true;
            }
            else
            {
                return true;
            }
        }
    }
}