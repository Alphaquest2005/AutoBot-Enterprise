using System;
using System.IO;

namespace AutoBot
{
    public partial class LICUtils
    {
        public static bool AssessLICComplete(string instrFile, string resultsFile, out int lcont)
        {
            lcont = 0;
            try
            {
                if (File.Exists(instrFile))
                {
                    if (!File.Exists(resultsFile)) return false;
                    var instructions = File.ReadAllLines(instrFile);
                    var res = File.ReadAllLines(resultsFile);
                    if (res.Length == 0)
                    {
                        return false;
                    }

                    foreach (var inline in instructions)
                    {
                        var p = inline.Split('\t');
                        // --- disable because it when it finished it cause it to repeat... better to check thru all to decide if to repeat or not
                        if (lcont >= res.Length)
                        {
                            // This logic seems complex and potentially error-prone, might need review
                            if ((res.Length / 2) == (instructions.Length / 3) || ((res.Length - 1) / 2) == (instructions.Length / 3) || (res.Length == 2 && instructions.Length == 3) || (res.Length == 3 && instructions.Length == 6)) return true; else return false;
                        }
                        if (string.IsNullOrEmpty(res[lcont])) return false;
                        var isSuccess = false;
                        foreach (var rline in res)
                        {
                            var r = rline.Split('\t');

                            // Potential IndexOutOfRangeException
                            if (r.Length == 5 && p.Length > 1 && p[1] == r[1] && r[4] == "Success") //for attachment
                            {
                                lcont += 1;
                                isSuccess = true;
                                break;
                            }

                            // Potential IndexOutOfRangeException
                            if (r.Length == 3 && p.Length > 1 && p[1] == r[1] && r[2] == "Success") // for file
                            {
                                lcont += 1;
                                isSuccess = true;
                                break;
                            }
                        }

                        if (isSuccess == true) continue;
                        return false;
                    }

                    return true;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}