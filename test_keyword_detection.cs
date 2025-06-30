// Quick test to verify ContainsInvoiceKeywords logic
using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        // MANGO text sample
        string mangoText = @"High-waist straight shorts "" 3.3
ref. 570003742302 ts
: L US$
Long jumpsuit with back opening @ 18.99
ref. 570502122243
r U & US$
Mixed spike necklace 10,99
ref. 57077738990R
Subtotal USS 196.33
Shipping & Handling Free
Estimated Tax US$ 13.74
TOTAL AMOUNT US$ 210.08";

        // Same keyword list as the method
        var invoiceKeywords = new[]
        {
            "TOTAL AMOUNT", "Subtotal", "Invoice", "Order number:", "Order Number", 
            "Invoice No", "Invoice Number", "Invoice Date", "Bill To", "Ship To",
            "Item Description", "Quantity", "Unit Price", "Line Total", "Tax",
            "Shipping", "Handling", "Grand Total", "Amount Due", "Payment",
            "Order:", "Receipt", "Purchase"
        };

        var textUpper = mangoText.ToUpperInvariant();
        var foundKeywords = new List<string>();
        
        foreach (var keyword in invoiceKeywords)
        {
            if (textUpper.Contains(keyword.ToUpperInvariant()))
            {
                foundKeywords.Add(keyword);
            }
        }
        
        // Require at least 2 invoice keywords to reduce false positives
        var hasInvoiceKeywords = foundKeywords.Count >= 2;
        
        Console.WriteLine($"PDF Text Length: {mangoText.Length} characters");
        Console.WriteLine($"Keywords Found: {foundKeywords.Count} out of {invoiceKeywords.Length}");
        Console.WriteLine($"Found Keywords: [{string.Join(", ", foundKeywords)}]");
        Console.WriteLine($"Detection Threshold: Requires >= 2 keywords");
        Console.WriteLine($"Detection Result: {hasInvoiceKeywords}");
        
        if (hasInvoiceKeywords)
        {
            Console.WriteLine("✅ INVOICE_CONTENT_DETECTED: PDF contains sufficient invoice keywords");
        }
        else
        {
            Console.WriteLine("❌ INVOICE_CONTENT_NOT_DETECTED: PDF does not contain sufficient invoice keywords");
        }
    }
}