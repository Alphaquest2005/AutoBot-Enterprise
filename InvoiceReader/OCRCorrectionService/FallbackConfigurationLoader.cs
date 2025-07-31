// File: OCRCorrectionService/FallbackConfigurationLoader.cs
// Fallback Configuration Loader - Loads fallback settings from Config/fallback-config.json or creates defaults
using System;
using System.IO;
using Serilog;
using Newtonsoft.Json;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Loads fallback configuration from appsettings.json or provides sensible defaults
    /// Part of build-validated fallback configuration system to prevent debugging disasters
    /// </summary>
    public static class FallbackConfigurationLoader
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(FallbackConfigurationLoader));
        private static FallbackConfiguration _cachedConfiguration = null;
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Loads fallback configuration from appsettings.json with intelligent defaults
        /// Caches configuration for performance while allowing reload capability
        /// </summary>
        /// <param name="forceReload">Force reload from configuration source</param>
        /// <returns>FallbackConfiguration instance</returns>
        public static FallbackConfiguration LoadConfiguration(bool forceReload = false)
        {
            lock (_lockObject)
            {
                if (_cachedConfiguration != null && !forceReload)
                {
                    _logger.Debug("🔄 **FALLBACK_CONFIG_CACHED**: Using cached fallback configuration");
                    return _cachedConfiguration;
                }

                _logger.Information("📖 **FALLBACK_CONFIG_LOADING**: Loading fallback configuration from appsettings.json");

                try
                {
                    // Try to load from appsettings.json
                    var configuration = LoadFromAppSettings();
                    
                    if (configuration != null)
                    {
                        _cachedConfiguration = configuration;
                        _logger.Information("✅ **FALLBACK_CONFIG_LOADED**: Successfully loaded fallback configuration from appsettings.json");
                        LogConfigurationState(configuration, "LOADED_FROM_CONFIG");
                        return configuration;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "⚠️ **FALLBACK_CONFIG_ERROR**: Failed to load from appsettings.json, using defaults");
                }

                // Fallback to default configuration
                _cachedConfiguration = FallbackConfiguration.CreateDefault();
                _logger.Information("🎯 **FALLBACK_CONFIG_DEFAULT**: Using default fallback configuration (database-driven mode)");
                LogConfigurationState(_cachedConfiguration, "DEFAULT_CONFIGURATION");
                
                return _cachedConfiguration;
            }
        }

        /// <summary>
        /// Loads configuration from Config/fallback-config.json file
        /// Returns null if file doesn't exist or has invalid format
        /// </summary>
        private static FallbackConfiguration LoadFromAppSettings()
        {
            try
            {
                // Follow AITemplateService pattern: read from Config directory
                var configPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Config", "fallback-config.json");
                
                if (!File.Exists(configPath))
                {
                    _logger.Debug("📋 **FALLBACK_CONFIG_MISSING**: No fallback-config.json found at {ConfigPath}", configPath);
                    return null;
                }

                // Read and parse JSON configuration file
                var fallbackConfigJson = File.ReadAllText(configPath);
                
                if (string.IsNullOrEmpty(fallbackConfigJson))
                {
                    _logger.Warning("⚠️ **FALLBACK_CONFIG_EMPTY**: fallback-config.json file is empty");
                    return null;
                }

                var configuration = JsonConvert.DeserializeObject<FallbackConfiguration>(fallbackConfigJson);
                
                if (configuration == null)
                {
                    _logger.Warning("⚠️ **FALLBACK_CONFIG_PARSE_FAILED**: Failed to parse FallbackConfiguration JSON from {ConfigPath}", configPath);
                    return null;
                }

                _logger.Debug("🔍 **FALLBACK_CONFIG_PARSED**: Successfully parsed FallbackConfiguration from {ConfigPath}", configPath);
                return configuration;
            }
            catch (JsonException jsonEx)
            {
                _logger.Error(jsonEx, "🚨 **FALLBACK_CONFIG_JSON_ERROR**: Invalid JSON format in fallback-config.json");
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **FALLBACK_CONFIG_LOAD_ERROR**: Unexpected error loading fallback-config.json");
                return null;
            }
        }

        /// <summary>
        /// Logs the current configuration state for diagnostic purposes
        /// Helps LLMs understand current fallback behavior without assumptions
        /// </summary>
        /// <param name="configuration">Configuration to log</param>
        /// <param name="source">Source description for context</param>
        private static void LogConfigurationState(FallbackConfiguration configuration, string source)
        {
            if (configuration == null)
            {
                _logger.Warning("⚠️ **FALLBACK_CONFIG_NULL**: Configuration is null for source={Source}", source);
                return;
            }

            _logger.Information("📊 **FALLBACK_CONFIG_STATE** ({Source}): " +
                "EnableLogicFallbacks={EnableLogicFallbacks}, " +
                "EnableGeminiFallback={EnableGeminiFallback}, " +
                "EnableTemplateFallback={EnableTemplateFallback}, " +
                "EnableDocumentTypeAssumption={EnableDocumentTypeAssumption}",
                source,
                configuration.EnableLogicFallbacks,
                configuration.EnableGeminiFallback,
                configuration.EnableTemplateFallback,
                configuration.EnableDocumentTypeAssumption);

            // Log behavioral implications for LLM diagnosis
            if (!configuration.EnableLogicFallbacks)
            {
                _logger.Information("🎯 **BEHAVIORAL_IMPLICATION**: Logic fallbacks DISABLED - System will fail-fast on missing corrections/templates/mappings");
            }
            else
            {
                _logger.Information("⚠️ **BEHAVIORAL_IMPLICATION**: Logic fallbacks ENABLED - System will return empty results and continue (may mask problems)");
            }

            if (!configuration.EnableDocumentTypeAssumption)
            {
                _logger.Information("🎯 **BEHAVIORAL_IMPLICATION**: DocumentType assumptions DISABLED - System will fail when DocumentType cannot be determined");
            }
            else
            {
                _logger.Information("⚠️ **BEHAVIORAL_IMPLICATION**: DocumentType assumptions ENABLED - System will assume 'Invoice' when DocumentType unknown");
            }
        }

        /// <summary>
        /// Clears cached configuration - useful for testing or configuration reloading
        /// </summary>
        public static void ClearCache()
        {
            lock (_lockObject)
            {
                _cachedConfiguration = null;
                _logger.Information("🔄 **FALLBACK_CONFIG_CACHE_CLEARED**: Configuration cache cleared, next load will read from source");
            }
        }

        /// <summary>
        /// Creates configuration for testing scenarios with specific settings
        /// Useful for unit tests that need to verify specific fallback behaviors
        /// </summary>
        /// <param name="enableLogicFallbacks">Enable logic fallbacks</param>
        /// <param name="enableGeminiFallback">Enable Gemini LLM fallback</param>
        /// <param name="enableTemplateFallback">Enable template fallback</param>
        /// <param name="enableDocumentTypeAssumption">Enable DocumentType assumption</param>
        /// <returns>FallbackConfiguration with specified settings</returns>
        public static FallbackConfiguration CreateTestConfiguration(
            bool enableLogicFallbacks = false,
            bool enableGeminiFallback = true,
            bool enableTemplateFallback = false,
            bool enableDocumentTypeAssumption = false)
        {
            var testConfig = new FallbackConfiguration
            {
                EnableLogicFallbacks = enableLogicFallbacks,
                EnableGeminiFallback = enableGeminiFallback,
                EnableTemplateFallback = enableTemplateFallback,
                EnableDocumentTypeAssumption = enableDocumentTypeAssumption
            };

            _logger.Information("🧪 **FALLBACK_CONFIG_TEST**: Created test configuration");
            LogConfigurationState(testConfig, "TEST_CONFIGURATION");
            
            return testConfig;
        }
    }
}