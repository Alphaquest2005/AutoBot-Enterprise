using System;

namespace WaterNut.Business.Services.Utils.LlmApi;

public class LlmApiException : Exception // Renamed generic exception
{
    public LlmApiException(string message) : base(message) { }
    public LlmApiException(string message, Exception inner) : base(message, inner) { }
}