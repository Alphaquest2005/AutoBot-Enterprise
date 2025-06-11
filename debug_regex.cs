using System;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        // Test the pattern that DeepSeek returned
        string deepSeekPattern = @"(?<TotalInsurance>-\$\d+\.\d{2})";
        string testText = "-$6.99";
        
        Console.WriteLine($"Pattern: {deepSeekPattern}");
        Console.WriteLine($"Test text: {testText}");
        
        try
        {
            var regex = new Regex(deepSeekPattern, RegexOptions.IgnoreCase);
            var match = regex.Match(testText);
            
            Console.WriteLine($"Match success: {match.Success}");
            if (match.Success)
            {
                Console.WriteLine($"Full match: '{match.Value}'");
                Console.WriteLine($"TotalInsurance group: '{match.Groups["TotalInsurance"].Value}'");
                Console.WriteLine($"Group success: {match.Groups["TotalInsurance"].Success}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Regex error: {ex.Message}");
        }
        
        // Test with single backslashes
        string correctedPattern = @"(?<TotalInsurance>-\$\d+\.\d{2})";
        Console.WriteLine($"\nCorrected pattern: {correctedPattern}");
        
        try
        {
            var regex2 = new Regex(correctedPattern, RegexOptions.IgnoreCase);
            var match2 = regex2.Match(testText);
            
            Console.WriteLine($"Match success: {match2.Success}");
            if (match2.Success)
            {
                Console.WriteLine($"Full match: '{match2.Value}'");
                Console.WriteLine($"TotalInsurance group: '{match2.Groups["TotalInsurance"].Value}'");
                Console.WriteLine($"Group success: {match2.Groups["TotalInsurance"].Success}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Regex error: {ex.Message}");
        }
    }
}