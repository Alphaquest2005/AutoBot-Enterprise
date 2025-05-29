using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Enhanced error detection with gift card pattern recognition
    /// </summary>
    public static class GiftCardDetector
    {
        private static readonly Regex[] GiftCardPatterns = new[]
        {
            new Regex(@"gift\s*card[:\s]*-?\$?([0-9]+\.?[0-9]*)", RegexOptions.IgnoreCase),
            new Regex(@"store\s*credit[:\s]*-?\$?([0-9]+\.?[0-9]*)", RegexOptions.IgnoreCase),
            new Regex(@"promo\s*code[:\s]*-?\$?([0-9]+\.?[0-9]*)", RegexOptions.IgnoreCase),
            new Regex(@"discount[:\s]*-?\$?([0-9]+\.?[0-9]*)", RegexOptions.IgnoreCase),
            new Regex(@"coupon[:\s]*-?\$?([0-9]+\.?[0-9]*)", RegexOptions.IgnoreCase),
            new Regex(@"-\$([0-9]+\.?[0-9]*)\s*(gift|credit|discount|promo)", RegexOptions.IgnoreCase)
        };

        public static List<(string Type, double Amount)> DetectDiscounts(string text)
        {
            var discounts = new List<(string Type, double Amount)>();

            foreach (var pattern in GiftCardPatterns)
            {
                var matches = pattern.Matches(text);
                foreach (Match match in matches)
                {
                    if (match.Groups.Count >= 2 && double.TryParse(match.Groups[1].Value, out var amount))
                    {
                        var type = DetermineDiscountType(match.Value);
                        discounts.Add((type, Math.Abs(amount))); // Always positive for TotalDeduction
                    }
                }
            }

            return discounts;
        }

        private static string DetermineDiscountType(string text)
        {
            var lowerText = text.ToLower();
            if (lowerText.Contains("gift")) return "Gift Card";
            if (lowerText.Contains("credit")) return "Store Credit";
            if (lowerText.Contains("promo")) return "Promo Code";
            if (lowerText.Contains("coupon")) return "Coupon";
            return "Discount";
        }
    }
}
