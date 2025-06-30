using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace AutoBotUtilities.Tests.Utils
{
    /// <summary>
    /// Claude Code SDK client for high-quality JSON extraction
    /// Uses Claude 4 (Sonnet) via subscription - no Opus models
    /// </summary>
    public class ClaudeCodeApiClient
    {
        private readonly ILogger _logger;

        public ClaudeCodeApiClient(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            // Claude Code SDK uses subscription authentication - no API key needed
            _logger.Information("🔧 **CLAUDE_CODE_SDK_INITIALIZED**: Using Claude 4 (Sonnet) with Max subscription");
        }

        /// <summary>
        /// Call Claude Code SDK subprocess for structured JSON extraction using subscription
        /// </summary>
        public async Task<string> GetJsonExtractionAsync(string prompt)
        {
            _logger.Information("🎯 **CLAUDE_CODE_SDK_REQUEST**: Sending JSON extraction request via Claude Code subprocess");

            try
            {
                // Create a temporary file for the prompt to avoid command line length limits
                var tempPromptFile = Path.GetTempFileName();
                File.WriteAllText(tempPromptFile, prompt);

                _logger.Information("📊 **CLAUDE_SDK_DETAILS**: Using Claude 4 (Sonnet) model, PromptLength={Length}", prompt.Length);

                // Execute Claude Code as subprocess with JSON output format
                // Force Claude 4 (Sonnet) model - never use Opus
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "claude",
                    Arguments = $"--model sonnet --output-format json --file \"{tempPromptFile}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = new Process { StartInfo = processStartInfo })
                {
                    process.Start();
                    
                    var output = await process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
                    var error = await process.StandardError.ReadToEndAsync().ConfigureAwait(false);
                    
                    process.WaitForExit();

                    // Clean up temp file
                    try { File.Delete(tempPromptFile); } catch { }

                    _logger.Information("🔍 **CLAUDE_SDK_RESPONSE**: ExitCode={ExitCode}, OutputLength={OutputLength}", 
                        process.ExitCode, output?.Length ?? 0);

                    if (process.ExitCode != 0)
                    {
                        _logger.Error("❌ **CLAUDE_SDK_ERROR**: Exit code {ExitCode}, Error: {Error}", 
                            process.ExitCode, error);
                        throw new InvalidOperationException($"Claude Code subprocess failed: {error}");
                    }

                    if (string.IsNullOrEmpty(output))
                    {
                        _logger.Error("❌ **CLAUDE_SDK_EMPTY**: No output from Claude Code subprocess");
                        throw new InvalidOperationException("Claude Code returned empty output");
                    }

                    // Parse JSON response from Claude Code SDK
                    dynamic jsonResponse = JsonConvert.DeserializeObject(output);
                    var extractedContent = jsonResponse?.content?.ToString();

                    if (string.IsNullOrEmpty(extractedContent))
                    {
                        _logger.Error("❌ **CLAUDE_SDK_NO_CONTENT**: No content in Claude Code response");
                        throw new InvalidOperationException("Claude Code returned no content");
                    }

                    _logger.Information("✅ **CLAUDE_SDK_SUCCESS**: Received Claude 4 (Sonnet) response via Max subscription, content length: {Length}", 
                        extractedContent.Length);

                    return extractedContent;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **CLAUDE_SDK_EXCEPTION**: Claude Code SDK call failed");
                throw;
            }
        }

        public void Dispose()
        {
            // No resources to dispose for subprocess-based approach
        }
    }

    #region Response Models

    public class ClaudeResponse
    {
        [JsonProperty("content")]
        public List<ClaudeContent> Content { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("usage")]
        public ClaudeUsage Usage { get; set; }
    }

    public class ClaudeContent
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class ClaudeUsage
    {
        [JsonProperty("input_tokens")]
        public int InputTokens { get; set; }

        [JsonProperty("output_tokens")]
        public int OutputTokens { get; set; }
    }

    #endregion
}