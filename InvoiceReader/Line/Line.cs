﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MoreLinq;
using OCR.Business.Entities;

namespace WaterNut.DataSpace
{
    public partial class Line
    {
        public Lines OCR_Lines { get; }

        public Line(Lines lines)
        {
            OCR_Lines = lines;
        }

        

        // Modified to accept instance from the calling Part

        public Dictionary<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>> Values { get; } = new Dictionary<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>>();
        //public bool MultiLine => OCR_Lines.MultiLine;

        public List<Dictionary<string, List<KeyValuePair<(Fields fields, int instance), string>>>> FailedFields => this.Values
            .Where(x => x.Value.Any(z => z.Key.fields.IsRequired && string.IsNullOrEmpty(z.Value.ToString())))
            .SelectMany(x => x.Value.ToList())
            .DistinctBy(x => x.Key.fields.Id)
            .GroupBy(x => $"{x.Key.fields.Field}-{x.Key.fields.Key}")
            .DistinctBy(x => x.Key)
            .Select(x => x.ToDictionary(k => x.Key, v => x.ToList()))
            .ToList();

    }
}