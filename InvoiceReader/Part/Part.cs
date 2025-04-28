﻿using System.Text;
using OCR.Business.Entities;
using System.Collections.Generic; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added
using Core.Common; // Added for BetterExpando if needed elsewhere
using MoreLinq; // Added for DistinctBy

namespace WaterNut.DataSpace
{
    public partial class Part
    {
        private static readonly ILogger _logger = Log.ForContext<Part>();
        private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(5);

        private readonly List<InvoiceLine> _startlines = new List<InvoiceLine>();
        private readonly List<InvoiceLine> _endlines = new List<InvoiceLine>();
        private readonly List<InvoiceLine> _lines = new List<InvoiceLine>();
        private readonly StringBuilder _instanceLinesTxt = new StringBuilder();
        private List<IDictionary<string, object>> _values = new List<IDictionary<string, object>>();

        public Parts OCR_Part { get; }

        public Part(Parts part)
        {
            string methodName = nameof(Part) + " Constructor";
            int? partId = part?.Id;

            LogEnteringConstructor(methodName, partId);

            ValidateInput(part, methodName);

            OCR_Part = part;
            LogAssignedOCRPart(methodName, partId);

            StartCount = part.Start?.Count() ?? 0;
            EndCount = part.End?.Count() ?? 0;
            LogCounts(methodName, partId);

            ChildParts = InitializeChildParts(part, methodName, partId);

            ParentPart = part.ChildParts.FirstOrDefault()?.ParentPart; // Assuming ChildPart here refers to the parent Parts entity and GetExistingPart is a non-recursive lookup method

            Lines = InitializeLines(part, methodName, partId);

            lastLineRead = 0;
            LogInitialState(methodName, partId);

            _logger.Information("Exiting {MethodName} successfully for PartId: {PartId}", methodName, partId);
        }

        public Parts ParentPart { get; set; }


        public List<Part> ChildParts { get; }
        public List<Line> Lines { get; }
        private int EndCount { get; }
        private int StartCount { get; }

        public bool WasStarted => _startlines?.Any() ?? false;

        private int lastLineRead = 0;
        private int _instance = 1;
        private int _lastProcessedParentInstance = 0;
        private int _currentInstanceStartLineNumber = -1;

        public List<IDictionary<string, object>> Values => _values;
        public bool? EverStarted { get; set; } = null;
        public int? Instance  => _instance;

        private void LogEnteringConstructor(string methodName, int? partId)
        {
            _logger.Verbose("Entering {MethodName} for OCR_Part Id: {PartId}", methodName, partId);
        }

        private void ValidateInput(Parts part, string methodName)
        {
            if (part == null)
            {
                _logger.Error("{MethodName}: Called with null OCR_Part object. Cannot initialize.", methodName);
                _logger.Verbose("Exiting {MethodName} due to null input.", methodName);
                throw new ArgumentNullException(nameof(part), "OCR_Part object cannot be null.");
            }
        }

        private void LogAssignedOCRPart(string methodName, int? partId)
        {
            _logger.Verbose("{MethodName}: Assigned OCR_Part (Id: {PartId}) to Part property.", methodName, partId);
        }

        private void LogCounts(string methodName, int? partId)
        {
            _logger.Verbose("{MethodName}: PartId: {PartId} - StartCondition Count: {StartCount}, EndCondition Count: {EndCount}", methodName, partId, StartCount, EndCount);
        }

        private List<Part> InitializeChildParts(Parts part, string methodName, int? partId)
        {
            _logger.Verbose("{MethodName}: Initializing ChildParts for PartId: {PartId}...", methodName, partId);
            try
            {
                var childParts = part.ParentParts?
                    .Where(pp => pp?.ChildPart != null)
                    .Select(x =>
                    {
                        _logger.Verbose("{MethodName}: Creating child Part for OCR_Part Id: {ChildPartId} (Parent: {ParentPartId})", methodName, x.ChildPart.Id, partId);
                        return new Part(x.ChildPart);
                    })
                    .ToList() ?? new List<Part>();

                _logger.Information("{MethodName}: Initialized PartId: {PartId} with {ChildCount} ChildParts.", methodName, partId, childParts.Count);
                return childParts;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "{MethodName}: Error initializing ChildParts for PartId: {PartId}", methodName, partId);
                _logger.Warning("{MethodName}: ChildParts collection set to empty list due to initialization error for PartId: {PartId}.", methodName, partId);
                return new List<Part>();
            }
        }

        private List<Line> InitializeLines(Parts part, string methodName, int? partId)
        {
            _logger.Verbose("{MethodName}: Initializing Lines for PartId: {PartId}...", methodName, partId);
            try
            {
                var lines = part.Lines?
                    .Where(x => x != null && (x.IsActive ?? true))
                    .Select(x =>
                    {
                        _logger.Verbose("{MethodName}: Creating Line object for OCR_Lines Id: {LineId} (Parent: {ParentPartId})", methodName, x.Id, partId);
                        return new Line(x);
                    })
                    .ToList() ?? new List<Line>();

                _logger.Information("{MethodName}: Initialized PartId: {PartId} with {LineCount} active Lines.", methodName, partId, lines.Count);
                return lines;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "{MethodName}: Error initializing Lines for PartId: {PartId}", methodName, partId);
                _logger.Warning("{MethodName}: Lines collection set to empty list due to initialization error for PartId: {PartId}.", methodName, partId);
                return new List<Line>();
            }
        }

        private void LogInitialState(string methodName, int? partId)
        {
            _logger.Verbose("{MethodName}: Initialized lastLineRead to 0 for PartId: {PartId}.", methodName, partId);
        }
    }
}