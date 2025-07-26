// File: OCRCorrectionService/TemplateEngine/ITemplateEngine.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WaterNut.DataSpace.TemplateEngine
{
    /// <summary>
    /// Core template engine interface for file-based prompt template system.
    /// Provides hot-reload, variable substitution, and validation capabilities.
    /// </summary>
    public interface ITemplateEngine
    {
        /// <summary>
        /// Loads and compiles a template from file system.
        /// </summary>
        Task<ITemplate> LoadTemplateAsync(string templatePath, string templateName);

        /// <summary>
        /// Renders a template with provided data context.
        /// </summary>
        Task<string> RenderAsync(string templateName, TemplateContext context);

        /// <summary>
        /// Validates template syntax and required variables.
        /// </summary>
        Task<TemplateValidationResult> ValidateTemplateAsync(string templatePath);

        /// <summary>
        /// Reloads templates from file system (hot-reload).
        /// </summary>
        Task ReloadTemplatesAsync();

        /// <summary>
        /// Registers template helpers for complex operations (escaping, etc.).
        /// </summary>
        void RegisterHelper(string name, Func<TemplateContext, object[], object> helper);

        /// <summary>
        /// Gets list of available templates.
        /// </summary>
        Task<List<TemplateInfo>> GetAvailableTemplatesAsync();

        /// <summary>
        /// Creates backup of current template before modification.
        /// </summary>
        Task<string> BackupTemplateAsync(string templateName);

        /// <summary>
        /// Restores template from backup.
        /// </summary>
        Task RestoreTemplateAsync(string templateName, string backupId);

        /// <summary>
        /// Event fired when templates are reloaded.
        /// </summary>
        event EventHandler<TemplateReloadedEventArgs> TemplateReloaded;
    }

    /// <summary>
    /// Individual template interface with metadata and rendering capabilities.
    /// </summary>
    public interface ITemplate
    {
        string Name { get; }
        string Path { get; }
        DateTime LastModified { get; }
        List<string> RequiredVariables { get; }
        TemplateMetadata Metadata { get; }
        
        Task<string> RenderAsync(TemplateContext context);
        Task<TemplateValidationResult> ValidateAsync(TemplateContext context);
    }

    /// <summary>
    /// Template rendering context with data and configuration.
    /// </summary>
    public class TemplateContext
    {
        public Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> Helpers { get; set; } = new Dictionary<string, object>();
        public TemplateRenderOptions Options { get; set; } = new TemplateRenderOptions();

        // Convenience methods for common OCR data
        public void SetInvoiceData(object invoice) => Variables["invoice"] = invoice;
        public void SetFileText(string fileText) => Variables["fileText"] = fileText;
        public void SetMetadata(Dictionary<string, object> metadata) => Variables["metadata"] = metadata;
        public void SetErrorContext(List<object> errors) => Variables["errors"] = errors;
        
        public T GetVariable<T>(string name, T defaultValue = default(T))
        {
            if (Variables.TryGetValue(name, out var value) && value is T typedValue)
                return typedValue;
            return defaultValue;
        }
    }

    /// <summary>
    /// Template rendering options and configurations.
    /// </summary>
    public class TemplateRenderOptions
    {
        public bool EnableStrictMode { get; set; } = true;
        public bool ValidateOutput { get; set; } = true;
        public TimeSpan RenderTimeout { get; set; } = TimeSpan.FromMinutes(2);
        public Dictionary<string, object> EscapingConfig { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Template metadata from template files.
    /// </summary>
    public class TemplateMetadata
    {
        public string Description { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public Dictionary<string, object> CustomProperties { get; set; } = new Dictionary<string, object>();
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
    }

    /// <summary>
    /// Template validation result with detailed error information.
    /// </summary>
    public class TemplateValidationResult
    {
        public bool IsValid { get; set; }
        public List<TemplateValidationError> Errors { get; set; } = new List<TemplateValidationError>();
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> MissingVariables { get; set; } = new List<string>();
        public string Summary => IsValid ? "✅ Template validation passed" : $"❌ Template validation failed: {Errors.Count} errors, {Warnings.Count} warnings";
    }

    /// <summary>
    /// Individual template validation error.
    /// </summary>
    public class TemplateValidationError
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public int LineNumber { get; set; }
        public int ColumnNumber { get; set; }
        public string Severity { get; set; }
        public string Context { get; set; }
    }

    /// <summary>
    /// Template information for registry and management.
    /// </summary>
    public class TemplateInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Category { get; set; }
        public TemplateMetadata Metadata { get; set; }
        public DateTime LastLoaded { get; set; }
        public bool IsActive { get; set; }
        public string BackupPath { get; set; }
    }

    /// <summary>
    /// Event args for template reload notifications.
    /// </summary>
    public class TemplateReloadedEventArgs : EventArgs
    {
        public List<string> ReloadedTemplates { get; set; } = new List<string>();
        public List<TemplateValidationError> Errors { get; set; } = new List<TemplateValidationError>();
        public DateTime ReloadTimestamp { get; set; } = DateTime.UtcNow;
        public string ReloadReason { get; set; }
    }
}