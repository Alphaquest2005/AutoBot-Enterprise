using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OCR.Client.Entities
{
    public partial class RegularExpressions
    {

        public string Name { get; set; }
        public Regex Regex => new Regex(RegEx,
            (MultiLine == true ? RegexOptions.Multiline : RegexOptions.Singleline) |
            RegexOptions.IgnoreCase);
    }
}
