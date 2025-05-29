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
        private readonly ILogger _logger;
        private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(5);

        private readonly List<InvoiceLine> _startlines = new List<InvoiceLine>();
        private readonly List<InvoiceLine> _endlines = new List<InvoiceLine>();
        private readonly List<InvoiceLine> _lines = new List<InvoiceLine>();
        private readonly StringBuilder _instanceLinesTxt = new StringBuilder();
        private List<IDictionary<string, object>> _values = new List<IDictionary<string, object>>();

        public Parts OCR_Part { get; }

        public Part(Parts part, ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

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
                var childParts = part.ChildParts?
                    .Where(pp => pp?.ChildPart != null)
                    .Select(x =>
                    {
                        _logger.Verbose("{MethodName}: Creating child Part for OCR_Part Id: {ChildPartId} (Parent: {ParentPartId})", methodName, x.ChildPart.Id, partId);
                        return new Part(x.ChildPart, _logger);
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
                        return new Line(x, _logger);
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

        /// <summary>
        /// Clears only the mutable state that gets populated during the Read process,
        /// without affecting configuration state like StartCount, EndCount, Lines, etc.
        /// </summary>
        public void ClearPartForReimport()
        {
            int? partId = this.OCR_Part?.Id;
            string methodName = nameof(ClearPartForReimport);

            _logger.Debug("Entering {MethodName} for PartId: {PartId}", methodName, partId);

            try
            {
                // Clear accumulated lines from processing (not configuration)
                _logger.Verbose("{MethodName}: Clearing _startlines (Count: {Count})", methodName, _startlines?.Count ?? 0);
                _startlines?.Clear();

                _logger.Verbose("{MethodName}: Clearing _endlines (Count: {Count})", methodName, _endlines?.Count ?? 0);
                _endlines?.Clear();

                _logger.Verbose("{MethodName}: Clearing _lines (Count: {Count})", methodName, _lines?.Count ?? 0);
                _lines?.Clear();

                // Clear text buffer
                _logger.Verbose("{MethodName}: Clearing _instanceLinesTxt (Length: {Length})", methodName, _instanceLinesTxt?.Length ?? 0);
                _instanceLinesTxt?.Clear();

                // Clear extracted values
                _logger.Verbose("{MethodName}: Clearing _values (Count: {Count})", methodName, _values?.Count ?? 0);
                _values?.Clear();

                // Reset processing state (but keep configuration state)
                _logger.Verbose("{MethodName}: Resetting lastLineRead to 0 (was {PreviousValue})", methodName, lastLineRead);
                lastLineRead = 0;

                _logger.Verbose("{MethodName}: Resetting _currentInstanceStartLineNumber to -1 (was {PreviousValue})",
                    methodName, _currentInstanceStartLineNumber);
                _currentInstanceStartLineNumber = -1;

                // Reset processing counters but keep them at reasonable values for re-import
                _logger.Verbose("{MethodName}: Resetting _instance to 1 (was {PreviousValue})", methodName, _instance);
                _instance = 1;

                _logger.Verbose("{MethodName}: Resetting _lastProcessedParentInstance to 0 (was {PreviousValue})",
                    methodName, _lastProcessedParentInstance);
                _lastProcessedParentInstance = 0;

                // Clear EverStarted flag
                _logger.Verbose("{MethodName}: Resetting EverStarted to null (was {PreviousValue})", methodName, EverStarted);
                EverStarted = null;

                // Recursively clear child parts
                if (ChildParts != null && ChildParts.Any())
                {
                    _logger.Debug("{MethodName}: Recursively clearing {Count} child parts for PartId: {PartId}",
                        methodName, ChildParts.Count, partId);
                    ChildParts.ForEach(child =>
                    {
                        if (child != null)
                        {
                            child.ClearPartForReimport();
                        }
                        else
                        {
                            _logger.Warning("{MethodName}: Skipping null child part for Parent PartId: {ParentPartId}",
                                methodName, partId);
                        }
                    });
                }

                _logger.Debug("Finished {MethodName} for PartId: {PartId}", methodName, partId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during {MethodName} for PartId: {PartId}", methodName, partId);
                throw;
            }
        }
    }
}