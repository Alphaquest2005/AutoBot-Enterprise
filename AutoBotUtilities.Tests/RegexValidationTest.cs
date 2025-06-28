using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class RegexValidationTest
    {
        [Test]
        public void ValidateObjectOrientedMultiFieldRegex()
        {
            // LineText from the diagnostic result
            string lineText = @"3 of: MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial $39.99
Lighting Shelves with Remote Control (2 Tier, 16 inch)";

            // Current suggested regex (single-line)
            string currentRegex = @"(?<Quantity>\d+)\s*of:\s*(?<ItemDescription>.+?)\$(?<UnitPrice>\d+\.\d{2})";
            
            // Enhanced multi-line regex
            string multilineRegex = @"(?<Quantity>\d+)\s*of:\s*(?<ItemDescription>.+?)\s*\$(?<UnitPrice>\d+\.\d{2})";

            Console.WriteLine("=== Testing Current Regex (Single-Line) ===");
            var currentMatch = Regex.Match(lineText, currentRegex);
            Console.WriteLine($"Match Success: {currentMatch.Success}");
            if (currentMatch.Success)
            {
                Console.WriteLine($"Quantity: '{currentMatch.Groups["Quantity"].Value}'");
                Console.WriteLine($"ItemDescription: '{currentMatch.Groups["ItemDescription"].Value}'");
                Console.WriteLine($"UnitPrice: '{currentMatch.Groups["UnitPrice"].Value}'");
            }

            Console.WriteLine("\n=== Testing Enhanced Multi-Line Regex ===");
            var multilineMatch = Regex.Match(lineText, multilineRegex, RegexOptions.Singleline);
            Console.WriteLine($"Match Success: {multilineMatch.Success}");
            if (multilineMatch.Success)
            {
                Console.WriteLine($"Quantity: '{multilineMatch.Groups["Quantity"].Value}'");
                Console.WriteLine($"ItemDescription: '{multilineMatch.Groups["ItemDescription"].Value}'");
                Console.WriteLine($"UnitPrice: '{multilineMatch.Groups["UnitPrice"].Value}'");
            }

            Console.WriteLine("\n=== Expected Values ===");
            Console.WriteLine("Quantity: '3'");
            Console.WriteLine("ItemDescription: 'MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Tier, 16 inch)'");
            Console.WriteLine("UnitPrice: '39.99'");

            // Assertion
            Assert.That(multilineMatch.Success, Is.True, "Multi-line regex should match the complete object");
            Assert.That(multilineMatch.Groups["Quantity"].Value, Is.EqualTo("3"));
            Assert.That(multilineMatch.Groups["UnitPrice"].Value, Is.EqualTo("39.99"));
        }
    }
}