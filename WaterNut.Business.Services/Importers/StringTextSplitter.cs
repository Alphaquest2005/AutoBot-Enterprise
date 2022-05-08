using System;
using System.Collections.Generic;
using System.Linq;

namespace WaterNut.Business.Services.Importers
{
    public class StringTextSplitter
    {
        private string[] _separator;

        public StringTextSplitter(IEnumerable<string> separator)
        {
            _separator = separator.ToArray();
        }

        public IEnumerable<string> Execute(string fixedFileTxt) => fixedFileTxt.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
    }
}