// File: OCRCorrectionService/TemplateEngine/HandlebarsTemplate.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
// using Handlebars; // Temporarily disable for basic template implementation
using Serilog;

namespace WaterNut.DataSpace.TemplateEngine
{
    /// <summary>
    /// Individual Handlebars template implementation with compilation, rendering, and validation.
    /// </summary>
    public class HandlebarsTemplate : ITemplate
    {
        private readonly ILogger _logger;
        private readonly string _rawTemplate;

        public string Name { get; }
        public string Path { get; }
        public DateTime LastModified { get; }
        public List<string> RequiredVariables { get; }
        public TemplateMetadata Metadata { get; }

        public HandlebarsTemplate(
            string name, 
            string path, 
            string content, 
            TemplateMetadata metadata, 
            ILogger logger)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Path = path ?? throw new ArgumentNullException(nameof(path));
            _rawTemplate = content ?? throw new ArgumentNullException(nameof(content));
            Metadata = metadata ?? new TemplateMetadata();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            LastModified = File.GetLastWriteTimeUtc(path);
            RequiredVariables = ExtractRequiredVariables(_rawTemplate);

            _logger.Verbose("🔧 **BASIC_TEMPLATE_READY**: Basic template '{TemplateName}' ready for rendering", Name);
        }

        public async Task<string> RenderAsync(TemplateContext context)
        {
            _logger.Verbose("🎨 **TEMPLATE_RENDER_START**: Rendering template '{TemplateName}' with {VariableCount} variables", 
                Name, context?.Variables?.Count ?? 0);

            try
            {
                context = context ?? new TemplateContext();
                
                // Validate context has required variables
                var validationResult = await ValidateContextAsync(context);
                if (!validationResult.IsValid && context.Options.EnableStrictMode)
                {
                    var errorMessage = $"Template '{Name}' validation failed: {string.Join("; ", validationResult.Errors.Select(e => e.Message))}";
                    throw new TemplateValidationException(errorMessage);
                }

                // Prepare data for Handlebars
                var handlebarsData = PrepareHandlebarsData(context);
                
                // Apply timeout if specified
                string result;
                if (context.Options.RenderTimeout > TimeSpan.Zero)
                {
                    var renderTask = Task.Run(() => _compiledTemplate(handlebarsData));
                    
                    if (await Task.WhenAny(renderTask, Task.Delay(context.Options.RenderTimeout)) == renderTask)
                    {
                        result = await renderTask;
                    }
                    else
                    {
                        throw new TimeoutException($"Template '{Name}' rendering exceeded timeout of {context.Options.RenderTimeout}");
                    }
                }
                else
                {
                    result = _compiledTemplate(handlebarsData);
                }

                // Post-process result if needed
                if (context.Options.ValidateOutput)
                {
                    ValidateRenderedOutput(result);
                }

                _logger.Verbose("✅ **TEMPLATE_RENDER_SUCCESS**: Template '{TemplateName}' rendered {Length} characters", 
                    Name, result?.Length ?? 0);

                return result;
            }
            catch (TemplateValidationException)
            {
                throw; // Re-throw validation exceptions as-is
            }
            catch (TimeoutException)
            {
                throw; // Re-throw timeout exceptions as-is
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **TEMPLATE_RENDER_ERROR**: Failed to render template '{TemplateName}'", Name);
                throw new TemplateRenderException($"Failed to render template '{Name}': {ex.Message}", ex);
            }
        }

        public async Task<TemplateValidationResult> ValidateAsync(TemplateContext context = null)
        {
            _logger.Verbose("🔍 **TEMPLATE_VALIDATION_START**: Validating template '{TemplateName}'", Name);

            var result = new TemplateValidationResult();
            context = context ?? new TemplateContext();

            try
            {
                // Validate template structure
                ValidateTemplateStructure(result);

                // Validate required variables
                var contextValidation = await ValidateContextAsync(context);
                result.Errors.AddRange(contextValidation.Errors);
                result.Warnings.AddRange(contextValidation.Warnings);
                result.MissingVariables.AddRange(contextValidation.MissingVariables);

                // Validate metadata
                ValidateMetadata(result);

                // Test rendering with minimal context to check for runtime issues
                if (result.Errors.Count == 0)
                {
                    try
                    {
                        var testContext = CreateMinimalTestContext();
                        var testResult = await RenderAsync(testContext);
                        
                        if (string.IsNullOrEmpty(testResult))
                        {
                            result.Warnings.Add("Template renders to empty string with test context");
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add(new TemplateValidationError
                        {
                            ErrorCode = "RENDER_TEST_FAILED",
                            Message = $"Template failed test rendering: {ex.Message}",
                            Severity = "Error"
                        });
                    }
                }

                result.IsValid = result.Errors.Count == 0;

                _logger.Verbose("🔍 **TEMPLATE_VALIDATION_COMPLETE**: Template '{TemplateName}' validation {Status}", 
                    Name, result.IsValid ? "PASSED" : "FAILED");

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **TEMPLATE_VALIDATION_ERROR**: Exception during template validation for '{TemplateName}'", Name);
                
                result.Errors.Add(new TemplateValidationError
                {
                    ErrorCode = "VALIDATION_EXCEPTION",
                    Message = $"Validation exception: {ex.Message}",
                    Severity = "Error"
                });
                
                result.IsValid = false;
                return result;
            }
        }

        #region Private Helper Methods

        private List<string> ExtractRequiredVariables(string templateContent)
        {
            var variables = new HashSet<string>();
            
            // Extract Handlebars variables: {{variable}}, {{#each variable}}, etc.
            var patterns = new[]
            {
                @"\{\{\s*([a-zA-Z_][a-zA-Z0-9_\.]*)\s*\}\}", // Simple variables
                @"\{\{\s*#(?:each|if|unless|with)\s+([a-zA-Z_][a-zA-Z0-9_\.]*)", // Block helpers
                @"\{\{\s*([a-zA-Z_][a-zA-Z0-9_\.]*)\s+", // Helper calls with variables
            };

            foreach (var pattern in patterns)
            {
                var matches = Regex.Matches(templateContent, pattern, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    var variable = match.Groups[1].Value.Split('.')[0]; // Get root variable name
                    if (!string.IsNullOrEmpty(variable) && !IsHandlebarsKeyword(variable))
                    {
                        variables.Add(variable);
                    }
                }
            }

            return variables.ToList();
        }

        private bool IsHandlebarsKeyword(string word)
        {
            var keywords = new HashSet<string> { "this", "root", "parent", "key", "index", "first", "last" };
            return keywords.Contains(word.ToLower());
        }

        private async Task<TemplateValidationResult> ValidateContextAsync(TemplateContext context)
        {
            var result = new TemplateValidationResult();

            // Check for missing required variables
            foreach (var requiredVar in RequiredVariables)
            {
                if (!context.Variables.ContainsKey(requiredVar))
                {
                    result.MissingVariables.Add(requiredVar);
                    result.Errors.Add(new TemplateValidationError
                    {
                        ErrorCode = "MISSING_VARIABLE",
                        Message = $"Required variable '{requiredVar}' not found in context",
                        Severity = "Error"
                    });
                }
            }

            // Check for OCR-specific variables that are commonly needed
            var ocrVariables = new[] { "invoice", "fileText", "metadata" };
            var missingOcrVars = ocrVariables.Where(v => !context.Variables.ContainsKey(v)).ToList();
            
            if (missingOcrVars.Any())
            {
                result.Warnings.Add($"Common OCR variables missing: {string.Join(", ", missingOcrVars)}");
            }

            result.IsValid = result.Errors.Count == 0;
            return await Task.FromResult(result);
        }

        private void ValidateTemplateStructure(TemplateValidationResult result)
        {
            // Check for balanced Handlebars blocks
            var openBlocks = Regex.Matches(_templateContent, @"\{\{\s*#\w+").Count;
            var closeBlocks = Regex.Matches(_templateContent, @"\{\{\s*/\w+").Count;
            
            if (openBlocks != closeBlocks)
            {
                result.Errors.Add(new TemplateValidationError
                {
                    ErrorCode = "UNBALANCED_BLOCKS",
                    Message = $"Unbalanced Handlebars blocks: {openBlocks} opening, {closeBlocks} closing",
                    Severity = "Error"
                });
            }

            // Check for potential regex escaping issues
            if (_templateContent.Contains(@"\\\\\\") && !_templateContent.Contains("escapeFor"))
            {
                result.Warnings.Add("Template contains multiple backslashes without escaping helpers - consider using escapeForJson, escapeForDocumentation, or escapeForValidation");
            }

            // Check for very long lines that might indicate formatting issues
            var lines = _templateContent.Split('\n');
            var longLines = lines.Where((line, index) => line.Length > 1000).ToList();
            
            if (longLines.Any())
            {
                result.Warnings.Add($"Template contains {longLines.Count} very long lines (>1000 chars) - consider breaking them up for readability");
            }
        }

        private void ValidateMetadata(TemplateValidationResult result)
        {
            if (Metadata == null)
            {
                result.Warnings.Add("Template has no metadata");
                return;
            }

            if (string.IsNullOrEmpty(Metadata.Description))
            {
                result.Warnings.Add("Template metadata missing description");
            }

            if (string.IsNullOrEmpty(Metadata.Version))
            {
                result.Warnings.Add("Template metadata missing version");
            }
        }

        private object PrepareHandlebarsData(TemplateContext context)
        {
            var data = new Dictionary<string, object>(context.Variables);
            
            // Add template metadata as special variables
            data["__template"] = new
            {
                name = Name,
                version = Metadata?.Version ?? "1.0.0",
                lastModified = LastModified
            };

            // Add current timestamp
            data["__timestamp"] = DateTime.UtcNow;
            
            // Add escaping configuration if available
            if (context.Options.EscapingConfig?.Any() == true)
            {
                data["__escaping"] = context.Options.EscapingConfig;
            }

            return data;
        }

        private TemplateContext CreateMinimalTestContext()
        {
            var context = new TemplateContext
            {
                Options = new TemplateRenderOptions
                {
                    EnableStrictMode = false,
                    ValidateOutput = false,
                    RenderTimeout = TimeSpan.FromSeconds(5)
                }
            };

            // Provide minimal test data for common variables
            foreach (var variable in RequiredVariables)
            {
                switch (variable.ToLower())
                {
                    case "invoice":
                        context.Variables[variable] = new { InvoiceNo = "TEST001", InvoiceTotal = 100.00 };
                        break;
                    case "filetext":
                        context.Variables[variable] = "Test OCR content";
                        break;
                    case "metadata":
                        context.Variables[variable] = new Dictionary<string, object> { { "testField", "testValue" } };
                        break;
                    case "errors":
                        context.Variables[variable] = new List<object>();
                        break;
                    default:
                        context.Variables[variable] = $"test_{variable}";
                        break;
                }
            }

            return context;
        }

        private void ValidateRenderedOutput(string output)
        {
            if (string.IsNullOrWhiteSpace(output))
            {
                throw new TemplateValidationException($"Template '{Name}' rendered empty or whitespace-only output");
            }

            // Check for common rendering errors
            if (output.Contains("[HELPER_ERROR:"))
            {
                var errorMatch = Regex.Match(output, @"\[HELPER_ERROR:\s*([^\]]+)\]");
                if (errorMatch.Success)
                {
                    throw new TemplateValidationException($"Template '{Name}' contains helper error: {errorMatch.Groups[1].Value}");
                }
            }

            // Check for unresolved variables (if using default missing formatter)
            if (output.Contains(" is undefined"))
            {
                var undefinedVars = Regex.Matches(output, @"(\w+) is undefined")
                    .Cast<Match>()
                    .Select(m => m.Groups[1].Value)
                    .Distinct()
                    .ToList();
                
                if (undefinedVars.Any())
                {
                    _logger.Warning("⚠️ **UNDEFINED_VARIABLES**: Template '{TemplateName}' has undefined variables: {Variables}", 
                        Name, string.Join(", ", undefinedVars));
                }
            }
        }

        #endregion
    }

    #region Custom Exceptions

    /// <summary>
    /// Exception thrown when template compilation fails.
    /// </summary>
    public class TemplateCompilationException : Exception
    {
        public TemplateCompilationException(string message) : base(message) { }
        public TemplateCompilationException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Exception thrown when template rendering fails.
    /// </summary>
    public class TemplateRenderException : Exception
    {
        public TemplateRenderException(string message) : base(message) { }
        public TemplateRenderException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Exception thrown when template validation fails.
    /// </summary>
    public class TemplateValidationException : Exception
    {
        public TemplateValidationException(string message) : base(message) { }
        public TemplateValidationException(string message, Exception innerException) : base(message, innerException) { }
    }

    #endregion
}