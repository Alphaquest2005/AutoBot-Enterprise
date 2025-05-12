namespace EmailDownloader;

using System.IO;
using System.Text.RegularExpressions;

using Core.Common.Utils;

public static partial class EmailDownloader
{
    private static string CleanFileName(string originalFileName)
    {
        if (string.IsNullOrWhiteSpace(originalFileName))
        {
            return "untitled"; // Or throw an ArgumentNullException
        }

        // 1. Remove invalid path characters defined by the OS
        string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
        string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
        string sanitizedFileName = Regex.Replace(originalFileName, invalidRegStr, "_");

        // 2. Handle the extension separately and more safely
        string nameWithoutExtension;
        string extension;
        int lastDotIndex = sanitizedFileName.LastIndexOf('.');

        if (lastDotIndex > 0 && lastDotIndex < sanitizedFileName.Length - 1) // Ensure dot is not first or last char
        {
            nameWithoutExtension = sanitizedFileName.Substring(0, lastDotIndex);
            extension = sanitizedFileName.Substring(lastDotIndex); // Includes the dot
        }
        else
        {
            nameWithoutExtension = sanitizedFileName; // No valid extension found
            extension = string.Empty;
        }

        // 3. Apply your custom special character replacement (if StringExtensions.ReplaceSpecialChar is still used)
        // Ensure StringExtensions.ReplaceSpecialChar does not remove periods if you want to keep them.
        // The regex in the example StringExtensions.ReplaceSpecialChar keeps hyphens and underscores.
        // If you want to replace other special characters in the name part:
        nameWithoutExtension = StringExtensions.ReplaceSpecialChar(nameWithoutExtension, "-"); // Replaces other special chars with hyphen

        // 4. Trim and ensure not empty
        string finalName = (nameWithoutExtension.Trim() + extension.Trim()).Trim();

        if (string.IsNullOrWhiteSpace(finalName))
        {
            return "untitled" + extension.Trim(); // Keep extension if original was just dots or invalid chars
        }

        // Optional: Limit file name length (OS dependent, usually around 255-260 characters for the full path)
        // int maxComponentLength = 200; // Example, leave room for path
        // if (finalName.Length > maxComponentLength)
        // {
        //     string extForLength = Path.GetExtension(finalName); // Re-get extension after all ops
        //     string nameOnlyForLength = Path.GetFileNameWithoutExtension(finalName);
        //     if (nameOnlyForLength.Length > maxComponentLength - (extForLength?.Length ?? 0))
        //     {
        //        nameOnlyForLength = nameOnlyForLength.Substring(0, maxComponentLength - (extForLength?.Length ?? 0) - 3) + "...";
        //     }
        //     finalName = nameOnlyForLength + extForLength;
        // }


        return finalName;
    }

    // If you need the StringExtensions.ReplaceSpecialChar and it's not in a shared library,
    // you'd include its definition here or ensure Core.Common.Utils is accessible.
    // For example:
    // public static class StringExtensionsInternal // To avoid conflict if Core.Common.Utils is also referenced
    // {
    //     public static string ReplaceSpecialChar(string str, string replacement = "")
    //     {
    //         if (string.IsNullOrEmpty(str)) return string.Empty;
    //         // Keeps alphanumeric, whitespace, hyphen, underscore. Replaces others.
    //         return Regex.Replace(str, @"[^a-zA-Z0-9\s\-_]", replacement);
    //     }
    // }
    // And then call it as: nameWithoutExtension = StringExtensionsInternal.ReplaceSpecialChar(nameWithoutExtension, "-");
}