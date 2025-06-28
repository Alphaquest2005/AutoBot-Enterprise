using System;
using System.Data.Entity;
using System.Linq;
using NUnit.Framework;
using OCR.Business.Entities;
using Serilog;

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class DatabaseVerificationTest
    {
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _logger = Log.Logger;
        }

        [Test]
        public void VerifyMultiFieldLineCreation()
        {
            using (var context = new OCRContext())
            {
                // Check for recently created multi-field lines
                var recentLines = context.Lines
                    .Include(l => l.RegularExpressions)
                    .Where(l => l.Name.StartsWith("AutoOmission_"))
                    .OrderByDescending(l => l.Id)
                    .Take(10)
                    .ToList();

                _logger.Information("🔍 **DATABASE_VERIFICATION**: Found {Count} AutoOmission lines", recentLines.Count);

                foreach (var line in recentLines)
                {
                    _logger.Information("   - Line ID: {Id}, Name: {Name}, PartId: {PartId}", 
                        line.Id, line.Name, line.PartId);
                        
                    // Check associated regex
                    if (line.RegularExpressions != null)
                    {
                        _logger.Information("     - Regex: {Pattern}", line.RegularExpressions.RegEx.Substring(0, Math.Min(100, line.RegularExpressions.RegEx.Length)));
                    }
                    
                    // Check associated fields
                    var fields = context.Fields.Where(f => f.LineId == line.Id).ToList();
                    _logger.Information("     - Field Count: {Count}", fields.Count);
                    foreach (var field in fields)
                    {
                        _logger.Information("       - Field: {FieldName} -> {DatabaseField} ({EntityType})", 
                            field.Key, field.Field, field.EntityType);
                    }
                }

                Assert.That(recentLines.Count, Is.GreaterThan(0), "Should have at least one AutoOmission line");
            }
        }

        [Test]
        public void VerifyFieldFormatCorrections()
        {
            using (var context = new OCRContext())
            {
                // Check for recently created field format corrections
                var recentCorrections = context.OCR_FieldFormatRegEx
                    .Include(ff => ff.RegEx)
                    .Include(ff => ff.ReplacementRegEx)
                    .OrderByDescending(ff => ff.Id)
                    .Take(10)
                    .ToList();

                _logger.Information("🔍 **DATABASE_VERIFICATION**: Found {Count} field format corrections", recentCorrections.Count);

                foreach (var correction in recentCorrections)
                {
                    _logger.Information("   - Correction ID: {Id}, FieldId: {FieldId}", 
                        correction.Id, correction.FieldId);
                        
                    // Get field info
                    var field = context.Fields.FirstOrDefault(f => f.Id == correction.FieldId);
                    if (field != null)
                    {
                        _logger.Information("     - Field: {FieldName} -> {DatabaseField}", 
                            field.Key, field.Field);
                    }
                    
                    // Get pattern and replacement regexes
                    if (correction.RegEx != null && correction.ReplacementRegEx != null)
                    {
                        _logger.Information("     - Pattern: '{Pattern}' -> '{Replacement}'", 
                            correction.RegEx.RegEx, correction.ReplacementRegEx.RegEx);
                    }
                }

                _logger.Information("✅ **DATABASE_VERIFICATION**: Field format corrections check complete");
            }
        }

        [Test]
        public void VerifyLearningRecords()
        {
            using (var context = new OCRContext())
            {
                // Check for recently created learning records
                var recentLearning = context.OCRCorrectionLearning
                    .Where(l => l.CorrectionType == "multi_field_omission" || l.CorrectionType == "format_correction")
                    .OrderByDescending(l => l.Id)
                    .Take(10)
                    .ToList();

                _logger.Information("🔍 **DATABASE_VERIFICATION**: Found {Count} multi-field learning records", recentLearning.Count);

                foreach (var learning in recentLearning)
                {
                    _logger.Information("   - Learning ID: {Id}, Field: {FieldName}, Type: {CorrectionType}, Success: {Success}", 
                        learning.Id, learning.FieldName, learning.CorrectionType, learning.Success);
                        
                    _logger.Information("     - Original: '{Original}' -> Correct: '{Correct}'", 
                        learning.OriginalError, learning.CorrectValue);
                        
                    if (!string.IsNullOrEmpty(learning.DeepSeekReasoning))
                    {
                        _logger.Information("     - Reasoning: {Reasoning}", 
                            learning.DeepSeekReasoning.Substring(0, Math.Min(100, learning.DeepSeekReasoning.Length)));
                    }
                }

                _logger.Information("✅ **DATABASE_VERIFICATION**: Learning records check complete");
            }
        }
    }
}