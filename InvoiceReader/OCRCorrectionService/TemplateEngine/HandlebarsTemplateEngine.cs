// File: OCRCorrectionService/TemplateEngine/HandlebarsTemplateEngine.cs
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
// using Handlebars; // Temporarily disable for basic template implementation
using Serilog;

namespace WaterNut.DataSpace.TemplateEngine
{
    /// <summary>
    /// Handlebars.NET-based template engine implementation with hot-reload and validation.
    /// Provides comprehensive template management for OCR prompt generation.
    /// </summary>
    public class HandlebarsTemplateEngine : ITemplateEngine
    {
        private readonly ILogger _logger;
        private readonly string _templateBasePath;
        private readonly ConcurrentDictionary<string, ITemplate> _templateCache;
        private readonly ConcurrentDictionary<string, FileSystemWatcher> _fileWatchers;
        // Basic template engine - no Handlebars dependency
        private readonly TemplateEngineConfig _config;

        public event EventHandler<TemplateReloadedEventArgs> TemplateReloaded;

        public HandlebarsTemplateEngine(ILogger logger, string templateBasePath, TemplateEngineConfig config = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _templateBasePath = templateBasePath ?? throw new ArgumentNullException(nameof(templateBasePath));
            _config = config ?? new TemplateEngineConfig();
            
            _templateCache = new ConcurrentDictionary<string, ITemplate>();
            _fileWatchers = new ConcurrentDictionary<string, FileSystemWatcher>();
            
            // Basic template engine initialization - no Handlebars needed
            
            _logger.Information("🏗️ **TEMPLATE_ENGINE_INITIALIZED**: Base path='{BasePath}', Hot reload={HotReload}", 
                _templateBasePath, _config.EnableHotReload);
                
            RegisterBuiltInHelpers();
            
            if (_config.EnableHotReload)
            {
                SetupFileWatchers();
            }
        }

        #region Core Template Operations

        public async Task<ITemplate> LoadTemplateAsync(string templatePath, string templateName)
        {
            _logger.Information("📄 **TEMPLATE_LOAD_START**: Loading template '{TemplateName}' from '{TemplatePath}'", templateName, templatePath);
            
            try
            {
                var fullPath = Path.Combine(_templateBasePath, templatePath);
                if (!File.Exists(fullPath))
                {
                    throw new FileNotFoundException($"Template file not found: {fullPath}");
                }

                var content = await File.ReadAllTextAsync(fullPath);
                var metadata = await LoadTemplateMetadataAsync(fullPath);
                var template = new HandlebarsTemplate(templateName, fullPath, content, metadata, _handlebars, _logger);
                
                // Validate template during load
                var validation = await template.ValidateAsync(new TemplateContext());
                if (!validation.IsValid)
                {
                    _logger.Warning("⚠️ **TEMPLATE_VALIDATION_WARNINGS**: Template '{TemplateName}' has validation issues: {Issues}", 
                        templateName, string.Join("; ", validation.Errors.Select(e => e.Message)));
                }

                _templateCache.AddOrUpdate(templateName, template, (key, oldValue) => template);
                
                _logger.Information("✅ **TEMPLATE_LOADED**: Template '{TemplateName}' loaded successfully", templateName);
                return template;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **TEMPLATE_LOAD_ERROR**: Failed to load template '{TemplateName}' from '{TemplatePath}'", templateName, templatePath);
                throw;
            }
        }

        public async Task<string> RenderAsync(string templateName, TemplateContext context)
        {
            _logger.Verbose("🎨 **TEMPLATE_RENDER_START**: Rendering template '{TemplateName}'", templateName);
            
            if (!_templateCache.TryGetValue(templateName, out var template))
            {
                throw new InvalidOperationException($"Template '{templateName}' not found. Available templates: {string.Join(", ", _templateCache.Keys)}");
            }

            try
            {
                var result = await template.RenderAsync(context);
                
                _logger.Verbose("✅ **TEMPLATE_RENDER_SUCCESS**: Template '{TemplateName}' rendered {Length} characters", 
                    templateName, result?.Length ?? 0);
                    
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **TEMPLATE_RENDER_ERROR**: Failed to render template '{TemplateName}'", templateName);
                throw;
            }
        }

        public async Task<TemplateValidationResult> ValidateTemplateAsync(string templatePath)
        {
            _logger.Information("🔍 **TEMPLATE_VALIDATION_START**: Validating template at '{TemplatePath}'", templatePath);
            
            var result = new TemplateValidationResult();
            
            try
            {
                var fullPath = Path.Combine(_templateBasePath, templatePath);
                if (!File.Exists(fullPath))
                {
                    result.Errors.Add(new TemplateValidationError
                    {
                        ErrorCode = "FILE_NOT_FOUND",
                        Message = $"Template file not found: {fullPath}",
                        Severity = "Error"
                    });
                    return result;
                }

                var content = await File.ReadAllTextAsync(fullPath);
                
                // Validate Handlebars syntax
                try
                {
                    var compiledTemplate = _handlebars.Compile(content);
                    _logger.Verbose("✅ **HANDLEBARS_SYNTAX_VALID**: Template syntax is valid");
                }
                catch (HandlebarsException hbEx)
                {
                    result.Errors.Add(new TemplateValidationError
                    {
                        ErrorCode = "HANDLEBARS_SYNTAX_ERROR",
                        Message = $"Handlebars syntax error: {hbEx.Message}",
                        Severity = "Error"
                    });
                }

                // Validate required variables
                var requiredVars = ExtractRequiredVariables(content);
                var missingVars = ValidateRequiredVariables(requiredVars);
                result.MissingVariables.AddRange(missingVars);

                // Validate template structure
                ValidateTemplateStructure(content, result);

                result.IsValid = !result.Errors.Any();
                
                _logger.Information("🔍 **TEMPLATE_VALIDATION_COMPLETE**: Template validation {Status}. Errors: {ErrorCount}, Warnings: {WarningCount}", 
                    result.IsValid ? "PASSED" : "FAILED", result.Errors.Count, result.Warnings.Count);
                    
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **TEMPLATE_VALIDATION_ERROR**: Failed to validate template '{TemplatePath}'", templatePath);
                result.Errors.Add(new TemplateValidationError
                {
                    ErrorCode = "VALIDATION_EXCEPTION",
                    Message = $"Validation exception: {ex.Message}",
                    Severity = "Error"
                });
                return result;
            }
        }

        #endregion

        #region Hot Reload and File Watching

        public async Task ReloadTemplatesAsync()
        {
            _logger.Information("🔄 **TEMPLATE_RELOAD_START**: Reloading all templates");
            
            var reloadedTemplates = new List<string>();
            var errors = new List<TemplateValidationError>();
            
            try
            {
                // Get all template files
                var templateFiles = Directory.GetFiles(_templateBasePath, "*.hbs", SearchOption.AllDirectories);
                
                foreach (var templateFile in templateFiles)
                {
                    try
                    {
                        var relativePath = Path.GetRelativePath(_templateBasePath, templateFile);
                        var templateName = Path.GetFileNameWithoutExtension(templateFile);
                        
                        await LoadTemplateAsync(relativePath, templateName);
                        reloadedTemplates.Add(templateName);
                        
                        _logger.Verbose("✅ **TEMPLATE_RELOADED**: {TemplateName}", templateName);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new TemplateValidationError
                        {
                            ErrorCode = "RELOAD_ERROR",
                            Message = $"Failed to reload {templateFile}: {ex.Message}",
                            Severity = "Error"
                        });
                        _logger.Error(ex, "❌ **TEMPLATE_RELOAD_ERROR**: Failed to reload template '{TemplateFile}'", templateFile);
                    }
                }

                var eventArgs = new TemplateReloadedEventArgs
                {
                    ReloadedTemplates = reloadedTemplates,
                    Errors = errors,
                    ReloadReason = "Manual reload request"
                };

                TemplateReloaded?.Invoke(this, eventArgs);
                
                _logger.Information("🔄 **TEMPLATE_RELOAD_COMPLETE**: Reloaded {Count} templates, {ErrorCount} errors", 
                    reloadedTemplates.Count, errors.Count);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **TEMPLATE_RELOAD_CRITICAL_ERROR**: Critical error during template reload");
                throw;
            }
        }

        private void SetupFileWatchers()
        {
            _logger.Information("👁️ **FILE_WATCHER_SETUP**: Setting up file watchers for hot reload");
            
            try
            {
                var templateDirectories = Directory.GetDirectories(_templateBasePath, "*", SearchOption.AllDirectories)
                    .Concat(new[] { _templateBasePath })
                    .Distinct();

                foreach (var directory in templateDirectories)
                {
                    var watcher = new FileSystemWatcher(directory, "*.hbs")
                    {
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime,
                        EnableRaisingEvents = true
                    };

                    watcher.Changed += OnTemplateFileChanged;
                    watcher.Created += OnTemplateFileChanged;
                    watcher.Deleted += OnTemplateFileChanged;
                    watcher.Renamed += OnTemplateFileRenamed;

                    _fileWatchers.TryAdd(directory, watcher);
                    _logger.Verbose("👁️ **FILE_WATCHER_ADDED**: Watching directory '{Directory}'", directory);
                }
                
                _logger.Information("✅ **FILE_WATCHER_SETUP_COMPLETE**: Watching {Count} directories for changes", _fileWatchers.Count);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **FILE_WATCHER_SETUP_ERROR**: Failed to setup file watchers");
            }
        }

        private async void OnTemplateFileChanged(object sender, FileSystemEventArgs e)
        {
            _logger.Information("🔄 **FILE_CHANGED**: Template file '{FilePath}' was {ChangeType}", e.FullPath, e.ChangeType);
            
            try
            {
                // Add small delay to avoid multiple rapid events
                await Task.Delay(100);
                
                var relativePath = Path.GetRelativePath(_templateBasePath, e.FullPath);
                var templateName = Path.GetFileNameWithoutExtension(e.FullPath);
                
                if (e.ChangeType == WatcherChangeTypes.Deleted)
                {
                    _templateCache.TryRemove(templateName, out _);
                    _logger.Information("🗑️ **TEMPLATE_REMOVED**: Template '{TemplateName}' removed from cache", templateName);
                }
                else
                {
                    await LoadTemplateAsync(relativePath, templateName);
                    _logger.Information("🔄 **TEMPLATE_AUTO_RELOADED**: Template '{TemplateName}' automatically reloaded", templateName);
                }

                var eventArgs = new TemplateReloadedEventArgs
                {
                    ReloadedTemplates = new List<string> { templateName },
                    ReloadReason = $"File {e.ChangeType.ToString().ToLower()}: {e.Name}"
                };

                TemplateReloaded?.Invoke(this, eventArgs);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **AUTO_RELOAD_ERROR**: Failed to auto-reload template '{FilePath}'", e.FullPath);
            }
        }

        private void OnTemplateFileRenamed(object sender, RenamedEventArgs e)
        {
            _logger.Information("📝 **FILE_RENAMED**: Template file renamed from '{OldPath}' to '{NewPath}'", e.OldFullPath, e.FullPath);
            
            // Remove old template from cache
            var oldTemplateName = Path.GetFileNameWithoutExtension(e.OldFullPath);
            _templateCache.TryRemove(oldTemplateName, out _);
            
            // Trigger reload for new template
            OnTemplateFileChanged(sender, new FileSystemEventArgs(WatcherChangeTypes.Created, Path.GetDirectoryName(e.FullPath), e.Name));
        }

        #endregion

        #region Template Management

        public async Task<List<TemplateInfo>> GetAvailableTemplatesAsync()
        {
            _logger.Information("📋 **TEMPLATE_LIST_REQUEST**: Getting available templates");
            
            var templates = new List<TemplateInfo>();
            
            try
            {
                var templateFiles = Directory.GetFiles(_templateBasePath, "*.hbs", SearchOption.AllDirectories);
                
                foreach (var templateFile in templateFiles)
                {
                    var relativePath = Path.GetRelativePath(_templateBasePath, templateFile);
                    var templateName = Path.GetFileNameWithoutExtension(templateFile);
                    var category = Path.GetDirectoryName(relativePath)?.Replace(Path.DirectorySeparatorChar, '/') ?? "Root";
                    
                    var metadata = await LoadTemplateMetadataAsync(templateFile);
                    var fileInfo = new FileInfo(templateFile);
                    
                    templates.Add(new TemplateInfo
                    {
                        Name = templateName,
                        Path = relativePath,
                        Category = category,
                        Metadata = metadata,
                        LastLoaded = _templateCache.ContainsKey(templateName) ? DateTime.UtcNow : DateTime.MinValue,
                        IsActive = _templateCache.ContainsKey(templateName)
                    });
                }
                
                _logger.Information("📋 **TEMPLATE_LIST_COMPLETE**: Found {Count} available templates", templates.Count);
                return templates;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **TEMPLATE_LIST_ERROR**: Failed to get available templates");
                throw;
            }
        }

        public async Task<string> BackupTemplateAsync(string templateName)
        {
            _logger.Information("💾 **TEMPLATE_BACKUP_START**: Creating backup for template '{TemplateName}'", templateName);
            
            try
            {
                if (!_templateCache.TryGetValue(templateName, out var template))
                {
                    throw new InvalidOperationException($"Template '{templateName}' not found in cache");
                }

                var backupDir = Path.Combine(_templateBasePath, "System", "backup");
                Directory.CreateDirectory(backupDir);
                
                var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
                var backupId = $"{templateName}_{timestamp}";
                var backupPath = Path.Combine(backupDir, $"{backupId}.hbs");
                
                var originalContent = await File.ReadAllTextAsync(template.Path);
                await File.WriteAllTextAsync(backupPath, originalContent);
                
                _logger.Information("✅ **TEMPLATE_BACKUP_COMPLETE**: Backup created with ID '{BackupId}' at '{BackupPath}'", backupId, backupPath);
                return backupId;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **TEMPLATE_BACKUP_ERROR**: Failed to backup template '{TemplateName}'", templateName);
                throw;
            }
        }

        public async Task RestoreTemplateAsync(string templateName, string backupId)
        {
            _logger.Information("🔄 **TEMPLATE_RESTORE_START**: Restoring template '{TemplateName}' from backup '{BackupId}'", templateName, backupId);
            
            try
            {
                var backupPath = Path.Combine(_templateBasePath, "System", "backup", $"{backupId}.hbs");
                if (!File.Exists(backupPath))
                {
                    throw new FileNotFoundException($"Backup file not found: {backupPath}");
                }

                if (!_templateCache.TryGetValue(templateName, out var template))
                {
                    throw new InvalidOperationException($"Template '{templateName}' not found in cache");
                }

                var backupContent = await File.ReadAllTextAsync(backupPath);
                await File.WriteAllTextAsync(template.Path, backupContent);
                
                // Reload template from restored file
                var relativePath = Path.GetRelativePath(_templateBasePath, template.Path);
                await LoadTemplateAsync(relativePath, templateName);
                
                _logger.Information("✅ **TEMPLATE_RESTORE_COMPLETE**: Template '{TemplateName}' restored from backup '{BackupId}'", templateName, backupId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **TEMPLATE_RESTORE_ERROR**: Failed to restore template '{TemplateName}' from backup '{BackupId}'", templateName, backupId);
                throw;
            }
        }

        #endregion

        #region Helper Registration and Built-ins

        public void RegisterHelper(string name, Func<TemplateContext, object[], object> helper)
        {
            _logger.Information("🔧 **HELPER_REGISTRATION**: Registering template helper '{HelperName}'", name);
            
            _handlebars.RegisterHelper(name, (writer, context, parameters) =>
            {
                try
                {
                    var templateContext = context.Value as TemplateContext ?? new TemplateContext();
                    var result = helper(templateContext, parameters);
                    writer.WriteSafeString(result?.ToString() ?? "");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "❌ **HELPER_ERROR**: Error in template helper '{HelperName}'", name);
                    writer.WriteSafeString($"[HELPER_ERROR: {ex.Message}]");
                }
            });
        }

        private void RegisterBuiltInHelpers()
        {
            _logger.Information("🔧 **BUILTIN_HELPERS_REGISTRATION**: Registering built-in template helpers");
            
            // Regex escaping helpers for OCR template compatibility
            _handlebars.RegisterHelper("escapeForJson", (writer, context, parameters) =>
            {
                var input = parameters.FirstOrDefault()?.ToString() ?? "";
                var escaped = input.Replace(@"\", @"\\");
                writer.WriteSafeString(escaped);
            });

            _handlebars.RegisterHelper("escapeForDocumentation", (writer, context, parameters) =>
            {
                var input = parameters.FirstOrDefault()?.ToString() ?? "";
                var escaped = input.Replace(@"\", @"\\\\");
                writer.WriteSafeString(escaped);
            });

            _handlebars.RegisterHelper("escapeForValidation", (writer, context, parameters) =>
            {
                var input = parameters.FirstOrDefault()?.ToString() ?? "";
                var escaped = input.Replace(@"\", @"\\\");
                writer.WriteSafeString(escaped);
            });

            // JSON serialization helper
            _handlebars.RegisterHelper("toJson", (writer, context, parameters) =>
            {
                var obj = parameters.FirstOrDefault();
                if (obj != null)
                {
                    var options = new JsonSerializerOptions 
                    { 
                        WriteIndented = true,
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                    };
                    var json = JsonSerializer.Serialize(obj, options);
                    writer.WriteSafeString(json);
                }
            });

            // Truncation helper for log-safe output
            _handlebars.RegisterHelper("truncate", (writer, context, parameters) =>
            {
                var input = parameters.FirstOrDefault()?.ToString() ?? "";
                var length = 100;
                
                if (parameters.Length > 1 && int.TryParse(parameters[1].ToString(), out var customLength))
                {
                    length = customLength;
                }
                
                var truncated = input.Length > length ? input.Substring(0, length) + "..." : input;
                writer.WriteSafeString(truncated);
            });

            // Current timestamp helper
            _handlebars.RegisterHelper("now", (writer, context, parameters) =>
            {
                var format = parameters.FirstOrDefault()?.ToString() ?? "yyyy-MM-dd HH:mm:ss";
                writer.WriteSafeString(DateTime.UtcNow.ToString(format));
            });

            _logger.Information("✅ **BUILTIN_HELPERS_COMPLETE**: Built-in template helpers registered successfully");
        }

        #endregion

        #region Private Helper Methods

        private async Task<TemplateMetadata> LoadTemplateMetadataAsync(string templatePath)
        {
            var metadataPath = Path.ChangeExtension(templatePath, ".metadata.json");
            
            if (File.Exists(metadataPath))
            {
                try
                {
                    var metadataJson = await File.ReadAllTextAsync(metadataPath);
                    return JsonSerializer.Deserialize<TemplateMetadata>(metadataJson) ?? new TemplateMetadata();
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "⚠️ **METADATA_LOAD_WARNING**: Failed to load metadata for template '{TemplatePath}'", templatePath);
                }
            }

            var fileInfo = new FileInfo(templatePath);
            return new TemplateMetadata
            {
                CreatedDate = fileInfo.CreationTimeUtc,
                LastModified = fileInfo.LastWriteTimeUtc
            };
        }

        private List<string> ExtractRequiredVariables(string templateContent)
        {
            var variables = new List<string>();
            var variablePattern = @"\{\{(?:\s*(?!#|/|\^|>|!))([^}]+)\}\}";
            var matches = Regex.Matches(templateContent, variablePattern);
            
            foreach (Match match in matches)
            {
                var variable = match.Groups[1].Value.Trim();
                if (!variables.Contains(variable))
                {
                    variables.Add(variable);
                }
            }
            
            return variables;
        }

        private List<string> ValidateRequiredVariables(List<string> requiredVars)
        {
            // This can be enhanced to check against a schema or known variable list
            return new List<string>();
        }

        private void ValidateTemplateStructure(string content, TemplateValidationResult result)
        {
            // Check for common OCR template requirements
            if (!content.Contains("{{"))
            {
                result.Warnings.Add("Template appears to contain no Handlebars variables");
            }

            // Check for potential issues with regex patterns
            if (content.Contains(@"\\\\\\") && !content.Contains("escapeFor"))
            {
                result.Warnings.Add("Template contains multiple backslashes but no escaping helpers - consider using escapeForJson, escapeForDocumentation, or escapeForValidation helpers");
            }
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            _logger.Information("🧹 **TEMPLATE_ENGINE_DISPOSE**: Disposing template engine and file watchers");
            
            foreach (var watcher in _fileWatchers.Values)
            {
                watcher?.Dispose();
            }
            _fileWatchers.Clear();
            _templateCache.Clear();
            
            _logger.Information("✅ **TEMPLATE_ENGINE_DISPOSED**: Template engine disposed successfully");
        }

        #endregion
    }

    /// <summary>
    /// Configuration for the Handlebars template engine.
    /// </summary>
    public class TemplateEngineConfig
    {
        public bool EnableHotReload { get; set; } = true;
        public TimeSpan FileWatcherDelay { get; set; } = TimeSpan.FromMilliseconds(100);
        public bool ThrowOnUnresolvedBindingExpression { get; set; } = false;
        public string MissingFormatter { get; set; } = "{0} is undefined";
    }
}