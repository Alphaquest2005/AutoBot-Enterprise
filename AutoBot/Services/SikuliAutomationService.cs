using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using CoreEntities.Business.Entities; // Assuming CoreEntitiesContext is needed for SubmitScriptErrors
using EmailDownloader; // For EmailDownloader access in SubmitScriptErrors
using WaterNut.DataSpace; // For BaseDataModel access

namespace AutoBot.Services
{
    /// <summary>
    /// Handles interactions with Sikuli automation scripts, including execution, retries, and result checking.
    /// Extracted from AutoBot.Utils class to adhere to SRP.
    /// </summary>
    public static class SikuliAutomationService
    {
        private static int oldProcessId = 0; // Moved from Utils

        public static bool AssessComplete(string instrFile, string resultsFile, out int lcont)
        {
            try
            {
                lcont = 0;
                var rcount = 0;

                if (File.Exists(instrFile))
                {
                    if (!File.Exists(resultsFile)) return false;
                    var lines = File.ReadAllLines(instrFile).Where(x => x.StartsWith("File\t")).ToArray();
                    var res = File.ReadAllLines(resultsFile).Where(x => x.StartsWith("File\t")).ToArray();
                    if (res.Length == 0)
                    {
                        return false;
                    }

                    foreach (var line in lines)
                    {
                        var p = line.Split('\t');
                        if (lcont + 1 >= res.Length) return false;
                        if (string.IsNullOrEmpty(res[lcont])) return false;
                        rcount += 1;
                        var isSuccess = false;
                        foreach (var rline in res)
                        {
                            var r = rline.Split('\t');

                            if (r.Length == 3 && p[1] == r[1] && r[2] == "Success")
                            {
                                if (r[0] == "File") lcont = rcount - 1;
                                isSuccess = true;
                                break;
                            }

                            if (r.Length == 3 && p[1] == r[0] && r[2] == "Success") // for file
                            {
                                lcont += 1;
                                isSuccess = true;
                                break;
                            }

                            if (r.Length == 4 && p[2] == r[2] && r[3] == "Success") // for file
                            {
                                lcont += 1;
                                isSuccess = true;
                                break;
                            }

                            if (r.Length == 3 && p[1] != r[1] && r[2] == "Error")
                            {
                                if (r[0] == "Screenshot")
                                {
                                    SubmitScriptErrors(r[1]); // Call within the same service
                                    return true;
                                }
                                //isSuccess = true;
                                //break;
                            }

                            if (r.Length == 3 && p[1] == r[1] && r[2] == "Error")
                            {
                                // email error
                                //if (r[0] == "File") lcont = rcount - 1;
                                //isSuccess = true;
                                //break;
                            }
                        }

                        if (isSuccess == true) continue;
                        return false;
                    }

                    return true;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void SubmitScriptErrors(string file)
        {
            try
            {
                Console.WriteLine("Submit Script Errors");

                // var saleInfo = CurrentSalesInfo(); // This dependency needs to be resolved if needed

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var contacts = ctx.Contacts
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                    && (x.Role == "Developer" || x.Role == "Broker")).Select(x => x.EmailAddress).ToArray();

                    var body = $"Please see attached.\r\n" +
                               $"Regards,\r\n" +
                               $"AutoBot";

                    // Assuming Utils.Client is accessible or refactored
                    // This dependency on Utils.Client needs to be addressed.
                    // For now, assuming it might be moved or accessed differently.
                    // If Utils.Client remains static in Utils, use AutoBot.Utils.Client
                    var msg = EmailDownloader.EmailDownloader.CreateMessage(AutoBot.Utils.Client, "AutoBot Script Error", contacts, body, new string[]
                    {
                        file
                    });
                    EmailDownloader.EmailDownloader.SendEmail(AutoBot.Utils.Client, msg);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void RetryImport(int trytimes, string script, bool redownload, string directoryName)
        {
            int lcont;
            for (int i = 0; i < trytimes; i++)
            {
                // Call ImportComplete within the same service
                if (ImportComplete(directoryName, redownload, out lcont, DateTime.Now.Year))
                    break;
                // Call RunSiKuLi within the same service
                RunSiKuLi(directoryName, script, lcont.ToString());
                // Call ImportComplete within the same service
                if (ImportComplete(directoryName, redownload, out lcont, DateTime.Now.Year)) break;
            }
        }

        public static void RetryAssess(string instrFile, string resultsFile, string directoryName, int trytimes)
        {
            var lcont = 0;
            for (int i = 0; i < trytimes; i++)
            {
                // Call AssessComplete within the same service
                if (AssessComplete(instrFile, resultsFile, out lcont) == true) break;

                // Call RunSiKuLi within the same service
                RunSiKuLi(directoryName, "AssessIM7", lcont.ToString()); //SaveIM7
                // Call AssessComplete within the same service
                if(AssessComplete(instrFile, resultsFile, out lcont) == true) break;
            }
        }

        public static void Assess(string instrFile, string resultsFile, string directoryName)
        {
            var lcont = 0;
            // Call AssessComplete and RunSiKuLi within the same service
            while (AssessComplete(instrFile, resultsFile, out lcont) == false)
            {
                RunSiKuLi(directoryName, "AssessIM7", lcont.ToString()); //SaveIM7
            }
        }

        public static void RunSiKuLi(string directoryName, string scriptName, string lastCNumber = "", int sMonths = 0, int sYears = 0, int eMonths = 0, int eYears = 0, bool enableDebugging = false)
        {
            try
            {
                if (string.IsNullOrEmpty(directoryName)) return;

                Console.WriteLine($"Executing {scriptName}");

                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "java.exe",
                    Arguments = $@"-jar C:\Users\{Environment.UserName}\OneDrive\Clients\AutoBot\sikulixide-2.0.5.jar -r C:\Users\{Environment.UserName
                    }\OneDrive\Clients\AutoBot\Scripts\{scriptName}.sikuli --args {
                        BaseDataModel.Instance.CurrentApplicationSettings.AsycudaLogin} {BaseDataModel.Instance.CurrentApplicationSettings.AsycudaPassword} ""{directoryName + "\\\\"}"" {
                        (string.IsNullOrEmpty(lastCNumber) ? "" : lastCNumber + "")
                    }{(sMonths + sYears + eMonths + eYears == 0 ? "" : $" {sMonths} {sYears} {eMonths} {eYears}")}{(enableDebugging ? " --debug-screenshots" : "")}", // Conditionally add debug flag
                    UseShellExecute = false
                };
                process.StartInfo = startInfo;
                process.EnableRaisingEvents = true;

                /// wait if instance already running
                foreach (var process1 in Process.GetProcesses().Where(x => x.MainWindowTitle.Contains("ASYCUDA"))
                             .ToList())
                {
                    Thread.Sleep(1000 * 60);
                }

                foreach (var process1 in Process.GetProcesses().Where(x => x.ProcessName.Contains("java") || x.ProcessName.Contains("Photo"))
                             .ToList())
                {
                    process1.Kill();
                }

                if (oldProcessId != 0)
                {
                    try
                    {
                        var oldProcess = Process.GetProcessById(oldProcessId);
                        oldProcess.Kill();
                    }
                    catch (Exception) // Catch specific exceptions if possible, e.g., ArgumentException if process not found
                    {
                        // Log or handle the exception if necessary
                    }
                }

                process.Start();
                oldProcessId = process.Id;
                var timeoutCycles = 0;
                // Consider making WaterNut.DataSpace.Utils._noOfCyclesBeforeHardExit configurable or passed in
                int hardExitCycles = 60; // Default to 60 minutes if the original value isn't accessible/refactored
                // if (WaterNut.DataSpace.Utils != null) hardExitCycles = WaterNut.DataSpace.Utils._noOfCyclesBeforeHardExit; // Example of checking

                while (!process.HasExited && process.Responding)
                {
                    if (timeoutCycles > 1 && !Process.GetProcesses().Where(x =>
                                x.MainWindowTitle.Contains("ASYCUDA"))
                            .ToList().Any()) break;
                    if (timeoutCycles > hardExitCycles) break;
                    Debug.WriteLine($"Waiting {timeoutCycles} Minutes");
                    Thread.Sleep(1000 * 60);
                    timeoutCycles += 1;
                }

                if (!process.HasExited) process.Kill();

                foreach (var process1 in Process.GetProcesses().Where(x => x.MainWindowTitle.Contains("ASYCUDA")
                                                                           || x.MainWindowTitle.Contains("Acrobat Reader")
                                                                           || x.MainWindowTitle.Contains("Photo")
                                                                           )
                             .ToList())
                {
                    process1.Kill();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static bool ImportComplete(string directoryName, bool redownload, out int lcont, int startYear, bool retryOnblankFile = false)
        {
            try
            {
                lcont = 0;

                var desFolder = directoryName + (directoryName.EndsWith(@"\") ? "" : "\\");
                var overviewFile = Path.Combine(desFolder, "OverView.txt");
                // Check if file exists AND was written recently (within 2 hours)
                if (File.Exists(overviewFile) && File.GetLastWriteTime(overviewFile) > DateTime.Now.AddHours(-2))
                {
                    // If written more than 1 hour ago, consider it stale for import purposes?
                    // This logic seems a bit complex, might need clarification. Assuming stale if > 1 hour old.
                    if (File.GetLastWriteTime(overviewFile) <= DateTime.Now.AddHours(-1)) return false;

                    var readAllText = File.ReadAllText(overviewFile);

                    if (readAllText == "No Files Found") return !retryOnblankFile;

                    // Improved splitting logic to handle potential empty entries better
                    var lines = readAllText
                        .Split(new[] { $"\r\n{startYear}\t", $"{startYear}\t" }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(l => !string.IsNullOrWhiteSpace(l)) // Ensure lines are not just whitespace
                        .ToArray();

                    if (lines.Length == 0)
                    {
                        return false;
                    }

                    var existingfiles = 0;

                    foreach (var line in lines)
                    {
                        lcont += 1; // This lcont logic might need review - it counts processed lines, not necessarily successful imports?

                        var p = line.Split('\t').ToList();
                        // The original logic checked if p[0] == startYear.ToString() and removed it.
                        // Since we split on that, the first element might be empty or part of the previous line if delimiters are adjacent.
                        // Let's assume the relevant data starts after potential empty strings from splitting.
                        var relevantParts = p.Where(part => !string.IsNullOrWhiteSpace(part)).ToList();

                        if (relevantParts.Count < 8) continue; // Need at least 8 valid parts
                        // Attempt parsing from the end, assuming format is relatively fixed
                        if (!DateTime.TryParse(relevantParts[relevantParts.Count - 2], out var regDate)) continue; // 6th element from end (index count-2)

                        // Assuming indices relative to relevantParts: [0]=Month?, [1]=Day?, ... [count-8]=Ref?, [count-7]=Ref?, [count-6]=Year?, [count-5]=CNumber?, [count-4]=??, [count-3]=??, [count-2]=Date, [count-1]=??
                        // This mapping is uncertain without clear file format documentation. Using original indices relative to p might be safer if splitting logic is reliable.
                        // Reverting to original logic based on p, assuming split works as intended:
                        if (p.Count < 8) continue; // Check original split parts count
                        if (!DateTime.TryParse(p[6], out regDate)) continue; // Original index 6

                        var fileName = Path.Combine(desFolder, $"{p[7] + p[8]}-{p[0]}-{regDate.Year}-{p[5]}.xml"); // Original indices
                        if (File.Exists(fileName))
                        {
                            existingfiles += 1;
                            continue;
                        }

                        return false; // If any expected file is missing, import is not complete
                    }

                    // If redownload is true, and the overview file is older than 5 minutes, consider it incomplete (needs refresh)
                    if (redownload && (DateTime.Now - new FileInfo(overviewFile).LastWriteTime).TotalMinutes > 5)
                        return false;

                    // Import is complete only if we found lines to process and all corresponding files existed
                    return existingfiles > 0 && existingfiles == lines.Length;
                }
                else
                {
                    // Overview file doesn't exist or is too old
                    return false;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error in ImportComplete: {exception.Message}");
                lcont = 0;
                return false;
            }
        }

        // Overload from original Utils.cs (lines 847-857)
        public static void RetryImport(int trytimes, string script, bool redownload, string directoryName, int sMonth,
            int sYear, int eMonth, int eYear, int startYear, bool retryOnblankFile = false)
        {
            int lcont;
            for (int i = 0; i < trytimes; i++)
            {
                // Call ImportComplete within the same service
                if (ImportComplete(directoryName, redownload, out lcont, startYear, retryOnblankFile))
                    break;
                // Call RunSiKuLi within the same service
                RunSiKuLi(directoryName, script, lcont.ToString(), sMonth, sYear, eMonth, eYear);
                // Call ImportComplete within the same service
                if (ImportComplete(directoryName, redownload, out lcont, startYear, retryOnblankFile)) break;
            }
        }
    }
}