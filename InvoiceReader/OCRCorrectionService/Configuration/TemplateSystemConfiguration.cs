// File: OCRCorrectionService/Configuration/TemplateSystemConfiguration.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog;

namespace WaterNut.DataSpace.Configuration
{
    /// <summary>
    /// Comprehensive configuration management for the file-based template system.
    /// Handles runtime settings, template paths, Meta AI integration, and deployment configurations.
    /// CRITICAL: All templates must be within InvoiceReader/OCRCorrectionService/ directory structure.
    /// </summary>
    public class TemplateSystemConfiguration
    {
        private readonly ILogger _logger;
        private readonly string _configurationPath;
        private TemplateSystemSettings _settings;
        private readonly JsonSerializerOptions _jsonOptions;

        public TemplateSystemConfiguration(ILogger logger, string configurationPath = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configurationPath = configurationPath ?? GetDefaultConfigurationPath();
            
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter() }
            };

            LoadConfiguration();
            
            _logger.Information("⚙️ **TEMPLATE_SYSTEM_CONFIG_INITIALIZED**: Configuration loaded from '{ConfigPath}'", _configurationPath);
        }

        #region Public Properties

        public TemplateSystemSettings Settings => _settings;

        public string TemplateBasePath => _settings.TemplatePaths.BasePath;
        public bool IsMetaAIEnabled => _settings.MetaAI.Enabled && !string.IsNullOrEmpty(_settings.MetaAI.ApiKey);
        public bool IsHotReloadEnabled => _settings.TemplateEngine.EnableHotReload;
        public bool IsAutoImplementationEnabled => _settings.AutoImplementation.Enabled;

        #endregion

        #region Configuration Management

        /// <summary>
        /// Loads configuration from file system with validation and defaults.
        /// </summary>
        public void LoadConfiguration()
        {
            try
            {
                if (File.Exists(_configurationPath))
                {
                    var configJson = File.ReadAllText(_configurationPath);
                    _settings = JsonSerializer.Deserialize<TemplateSystemSettings>(configJson, _jsonOptions);
                    _logger.Information("✅ **CONFIG_LOADED**: Configuration loaded from file");
                }
                else
                {
                    _logger.Information("📁 **CONFIG_DEFAULT**: Configuration file not found, using defaults");
                    _settings = CreateDefaultConfiguration();
                    SaveConfiguration(); // Save default configuration for future use
                }

                ValidateConfiguration();
                NormalizeConfiguration();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **CONFIG_LOAD_ERROR**: Failed to load configuration, using defaults");
                _settings = CreateDefaultConfiguration();
            }
        }

        /// <summary>
        /// Saves current configuration to file system.
        /// </summary>
        public void SaveConfiguration()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_configurationPath));
                var configJson = JsonSerializer.Serialize(_settings, _jsonOptions);
                File.WriteAllText(_configurationPath, configJson);
                
                _logger.Information("💾 **CONFIG_SAVED**: Configuration saved to '{ConfigPath}'", _configurationPath);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **CONFIG_SAVE_ERROR**: Failed to save configuration");
            }
        }

        /// <summary>
        /// Updates configuration with new settings and saves automatically.
        /// </summary>
        public void UpdateConfiguration(Action<TemplateSystemSettings> updateAction)
        {
            if (updateAction == null) throw new ArgumentNullException(nameof(updateAction));

            try
            {
                updateAction(_settings);
                ValidateConfiguration();
                NormalizeConfiguration();
                SaveConfiguration();
                
                _logger.Information("🔄 **CONFIG_UPDATED**: Configuration updated and saved");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **CONFIG_UPDATE_ERROR**: Failed to update configuration");
                throw;
            }
        }

        #endregion

        #region Template Path Management

        /// <summary>
        /// Gets the full path for a template category.
        /// CRITICAL: All paths are within OCRCorrectionService directory structure.
        /// </summary>
        public string GetTemplateCategoryPath(string category)
        {
            var categoryPath = Path.Combine(_settings.TemplatePaths.BasePath, category);
            EnsureDirectoryExists(categoryPath);
            return categoryPath;
        }

        /// <summary>
        /// Gets the full path for a specific template file.
        /// CRITICAL: Validates path is within allowed OCRCorrectionService structure.
        /// </summary>
        public string GetTemplateFilePath(string category, string templateName)
        {
            var categoryPath = GetTemplateCategoryPath(category);
            var templatePath = Path.Combine(categoryPath, $"{templateName}.hbs");
            
            // SECURITY: Validate path is within allowed directory structure
            ValidateTemplatePath(templatePath);
            
            return templatePath;
        }

        /// <summary>
        /// Creates the complete directory structure for templates.
        /// CRITICAL: Only creates directories within OCRCorrectionService.
        /// </summary>
        public void InitializeTemplateDirectoryStructure()
        {
            _logger.Information("🏗️ **TEMPLATE_STRUCTURE_INIT**: Creating template directory structure");

            try
            {
                var directories = new[]
                {
                    "OCR/HeaderDetection",
                    "OCR/ProductDetection", 
                    "OCR/DirectCorrection",
                    "OCR/RegexCreation",
                    "OCR/Shared",
                    "MetaAI/Recommendations",
                    "MetaAI/Optimizations",
                    "System/Backups",
                    "System/Audit",
                    "System/Validation"
                };

                foreach (var directory in directories)
                {
                    var fullPath = Path.Combine(_settings.TemplatePaths.BasePath, directory);
                    EnsureDirectoryExists(fullPath);
                }

                _logger.Information("✅ **TEMPLATE_STRUCTURE_COMPLETE**: Template directory structure created");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **TEMPLATE_STRUCTURE_ERROR**: Failed to create template directory structure");
                throw;
            }
        }

        #endregion

        #region Environment-Specific Configuration

        /// <summary>
        /// Gets configuration for the current environment (Development, Testing, Production).
        /// </summary>
        public EnvironmentConfiguration GetEnvironmentConfiguration()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            
            switch (environmentName.ToLower())
            {
                case "development":
                    return _settings.Environments.Development;
                case "testing":
                    return _settings.Environments.Testing;
                case "production":
                    return _settings.Environments.Production;
                default:
                    _logger.Warning("⚠️ **UNKNOWN_ENVIRONMENT**: Unknown environment '{Environment}', using Development settings", environmentName);
                    return _settings.Environments.Development;
            }
        }

        /// <summary>
        /// Applies environment-specific overrides to current configuration.
        /// </summary>
        public void ApplyEnvironmentConfiguration()
        {
            var envConfig = GetEnvironmentConfiguration();
            
            _logger.Information("🌍 **ENVIRONMENT_CONFIG**: Applying {Environment} environment configuration", envConfig.Name);

            // Override settings based on environment
            if (envConfig.OverrideSettings != null)
            {
                if (envConfig.OverrideSettings.ContainsKey("EnableMetaAI"))
                {
                    _settings.MetaAI.Enabled = Convert.ToBoolean(envConfig.OverrideSettings["EnableMetaAI"]);
                }

                if (envConfig.OverrideSettings.ContainsKey("EnableHotReload"))
                {
                    _settings.TemplateEngine.EnableHotReload = Convert.ToBoolean(envConfig.OverrideSettings["EnableHotReload"]);
                }

                if (envConfig.OverrideSettings.ContainsKey("EnableAutoImplementation"))
                {
                    _settings.AutoImplementation.Enabled = Convert.ToBoolean(envConfig.OverrideSettings["EnableAutoImplementation"]);
                }

                if (envConfig.OverrideSettings.ContainsKey("LogLevel"))
                {
                    _settings.Logging.MinimumLevel = envConfig.OverrideSettings["LogLevel"].ToString();
                }
            }
        }

        #endregion

        #region Private Helper Methods

        private static string GetDefaultConfigurationPath()
        {
            // CRITICAL: Configuration file must be within OCRCorrectionService directory
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDirectory, "OCRCorrectionService", "Configuration", "template-system.json");
        }

        private TemplateSystemSettings CreateDefaultConfiguration()
        {
            // CRITICAL: Base path must be within OCRCorrectionService directory structure
            var ocrServicePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OCRCorrectionService");
            
            return new TemplateSystemSettings
            {
                TemplatePaths = new TemplatePathConfiguration
                {
                    BasePath = Path.Combine(ocrServicePath, "Templates"),
                    BackupPath = Path.Combine(ocrServicePath, "Templates", "System", "Backups"),
                    AuditPath = Path.Combine(ocrServicePath, "Templates", "System", "Audit")
                },
                TemplateEngine = new TemplateEngineConfiguration
                {
                    EnableHotReload = true,
                    FileWatcherDelayMs = 100,
                    ValidationTimeout = TimeSpan.FromSeconds(30),
                    RenderTimeout = TimeSpan.FromMinutes(2)
                },
                MetaAI = new MetaAIConfiguration
                {
                    Enabled = false, // Disabled by default for security
                    ApiEndpoint = "https://api.meta.ai/v1/",
                    TimeoutMinutes = 5,
                    MaxRetryAttempts = 3,
                    EnableOptimization = false
                },
                AutoImplementation = new AutoImplementationConfiguration
                {
                    Enabled = false, // Disabled by default for safety
                    RequireBackup = true,
                    EnableRollback = true,
                    EnableValidation = true,
                    RollbackOnValidationFailure = true
                },
                Environments = new EnvironmentConfigurations
                {
                    Development = new EnvironmentConfiguration
                    {
                        Name = "Development",
                        OverrideSettings = new Dictionary<string, object>
                        {
                            ["EnableMetaAI"] = false,
                            ["EnableHotReload"] = true,
                            ["EnableAutoImplementation"] = false,
                            ["LogLevel"] = "Verbose"
                        }
                    },
                    Testing = new EnvironmentConfiguration
                    {
                        Name = "Testing",
                        OverrideSettings = new Dictionary<string, object>
                        {
                            ["EnableMetaAI"] = false,
                            ["EnableHotReload"] = true,
                            ["EnableAutoImplementation"] = true,
                            ["LogLevel"] = "Information"
                        }
                    },
                    Production = new EnvironmentConfiguration
                    {
                        Name = "Production",
                        OverrideSettings = new Dictionary<string, object>
                        {
                            ["EnableMetaAI"] = true,
                            ["EnableHotReload"] = false,
                            ["EnableAutoImplementation"] = true,
                            ["LogLevel"] = "Warning"
                        }
                    }
                },
                Logging = new LoggingConfiguration
                {
                    MinimumLevel = "Information",
                    EnableFileLogging = true,
                    LogFilePattern = "Logs/TemplateSystem-{Date}.log",
                    RetainedFileCount = 30
                }
            };
        }

        private void ValidateConfiguration()
        {
            var errors = new List<string>();

            // Validate template paths are within OCRCorrectionService
            if (!IsPathWithinOCRCorrectionService(_settings.TemplatePaths.BasePath))
            {
                errors.Add("Template base path must be within OCRCorrectionService directory structure");
            }

            if (!IsPathWithinOCRCorrectionService(_settings.TemplatePaths.BackupPath))
            {
                errors.Add("Backup path must be within OCRCorrectionService directory structure");
            }

            // Validate Meta AI configuration
            if (_settings.MetaAI.Enabled && string.IsNullOrEmpty(_settings.MetaAI.ApiKey))
            {
                _logger.Warning("⚠️ **CONFIG_WARNING**: Meta AI enabled but no API key provided");
            }

            // Validate timeouts
            if (_settings.TemplateEngine.RenderTimeout < TimeSpan.FromSeconds(10))
            {
                errors.Add("Render timeout must be at least 10 seconds");
            }

            if (errors.Any())
            {
                var errorMessage = string.Join("; ", errors);
                _logger.Error("❌ **CONFIG_VALIDATION_FAILED**: {Errors}", errorMessage);
                throw new InvalidOperationException($"Configuration validation failed: {errorMessage}");
            }
        }

        private void NormalizeConfiguration()
        {
            // Normalize paths to use consistent separators
            _settings.TemplatePaths.BasePath = Path.GetFullPath(_settings.TemplatePaths.BasePath);
            _settings.TemplatePaths.BackupPath = Path.GetFullPath(_settings.TemplatePaths.BackupPath);
            _settings.TemplatePaths.AuditPath = Path.GetFullPath(_settings.TemplatePaths.AuditPath);

            // Ensure Meta AI endpoint ends with slash
            if (!string.IsNullOrEmpty(_settings.MetaAI.ApiEndpoint) && !_settings.MetaAI.ApiEndpoint.EndsWith("/"))
            {
                _settings.MetaAI.ApiEndpoint += "/";
            }
        }

        private void ValidateTemplatePath(string templatePath)
        {
            if (!IsPathWithinOCRCorrectionService(templatePath))
            {
                throw new UnauthorizedAccessException($"Template path '{templatePath}' is not within allowed OCRCorrectionService directory structure");
            }
        }

        private bool IsPathWithinOCRCorrectionService(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;

            try
            {
                var fullPath = Path.GetFullPath(path);
                var ocrServicePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OCRCorrectionService"));
                
                return fullPath.StartsWith(ocrServicePath, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private void EnsureDirectoryExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                _logger.Verbose("📁 **DIRECTORY_CREATED**: {DirectoryPath}", directoryPath);
            }
        }

        #endregion
    }

    #region Configuration Data Models

    /// <summary>
    /// Root configuration settings for the template system.
    /// </summary>
    public class TemplateSystemSettings
    {
        public TemplatePathConfiguration TemplatePaths { get; set; } = new TemplatePathConfiguration();
        public TemplateEngineConfiguration TemplateEngine { get; set; } = new TemplateEngineConfiguration();
        public MetaAIConfiguration MetaAI { get; set; } = new MetaAIConfiguration();
        public AutoImplementationConfiguration AutoImplementation { get; set; } = new AutoImplementationConfiguration();
        public EnvironmentConfigurations Environments { get; set; } = new EnvironmentConfigurations();
        public LoggingConfiguration Logging { get; set; } = new LoggingConfiguration();
    }

    /// <summary>
    /// Template path configuration - CRITICAL: All paths must be within OCRCorrectionService.
    /// </summary>
    public class TemplatePathConfiguration
    {
        public string BasePath { get; set; } = "OCRCorrectionService/Templates";
        public string BackupPath { get; set; } = "OCRCorrectionService/Templates/System/Backups";
        public string AuditPath { get; set; } = "OCRCorrectionService/Templates/System/Audit";
        public string SharedResourcesPath { get; set; } = "OCRCorrectionService/Templates/OCR/Shared";
        public string MetaAIPath { get; set; } = "OCRCorrectionService/Templates/MetaAI";
    }

    /// <summary>
    /// Template engine configuration.
    /// </summary>
    public class TemplateEngineConfiguration
    {
        public bool EnableHotReload { get; set; } = true;
        public int FileWatcherDelayMs { get; set; } = 100;
        public TimeSpan ValidationTimeout { get; set; } = TimeSpan.FromSeconds(30);
        public TimeSpan RenderTimeout { get; set; } = TimeSpan.FromMinutes(2);
        public bool EnableStrictValidation { get; set; } = true;
        public bool CacheCompiledTemplates { get; set; } = true;
    }

    /// <summary>
    /// Meta AI service configuration.
    /// </summary>
    public class MetaAIConfiguration
    {
        public bool Enabled { get; set; } = false;
        public string ApiEndpoint { get; set; } = "https://api.meta.ai/v1/";
        public string ApiKey { get; set; }
        public int TimeoutMinutes { get; set; } = 5;
        public int MaxRetryAttempts { get; set; } = 3;
        public bool EnableOptimization { get; set; } = false;
        public bool EnableContinuousOptimization { get; set; } = false;
        public TimeSpan OptimizationInterval { get; set; } = TimeSpan.FromDays(1);
    }

    /// <summary>
    /// Auto-implementation configuration.
    /// </summary>
    public class AutoImplementationConfiguration
    {
        public bool Enabled { get; set; } = false;
        public bool RequireBackup { get; set; } = true;
        public bool EnableRollback { get; set; } = true;
        public bool EnableValidation { get; set; } = true;
        public bool RollbackOnValidationFailure { get; set; } = true;
        public List<string> RestrictedOperations { get; set; } = new List<string>();
        public TimeSpan ValidationTimeout { get; set; } = TimeSpan.FromMinutes(5);
    }

    /// <summary>
    /// Environment-specific configurations.
    /// </summary>
    public class EnvironmentConfigurations
    {
        public EnvironmentConfiguration Development { get; set; } = new EnvironmentConfiguration();
        public EnvironmentConfiguration Testing { get; set; } = new EnvironmentConfiguration();
        public EnvironmentConfiguration Production { get; set; } = new EnvironmentConfiguration();
    }

    /// <summary>
    /// Individual environment configuration.
    /// </summary>
    public class EnvironmentConfiguration
    {
        public string Name { get; set; }
        public Dictionary<string, object> OverrideSettings { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Logging configuration.
    /// </summary>
    public class LoggingConfiguration
    {
        public string MinimumLevel { get; set; } = "Information";
        public bool EnableFileLogging { get; set; } = true;
        public string LogFilePattern { get; set; } = "Logs/TemplateSystem-{Date}.log";
        public int RetainedFileCount { get; set; } = 30;
        public bool EnableConsoleLogging { get; set; } = true;
    }

    #endregion
}