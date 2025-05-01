using System;
using System.Net.Http;

namespace WaterNut.Business.Services.Utils.LlmApi;

public class RateLimitException : HttpRequestException
{
    public int StatusCode { get; }
    public RateLimitException(int statusCode, string message) : base($"Rate limit exceeded (Status {statusCode}): {message}") { StatusCode = statusCode; }
    public RateLimitException(int statusCode, string message, Exception inner) : base($"Rate limit exceeded (Status {statusCode}): {message}", inner) { StatusCode = statusCode; }
    // Add other constructors if needed
}