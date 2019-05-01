using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Utils
{
    public class FunctionLibary
    {
        public string AddSpacesToSentence(string Text, bool preserveAcronyms)
        {
            if (string.IsNullOrWhiteSpace(Text))
                return string.Empty;
            var newText = new StringBuilder(Text.Length * 2);
            newText.Append(Text[0]);
            for (var i = 1; i < Text.Length; i++)
            {
                if (char.IsUpper(Text[i]))
                    if ((Text[i - 1] != ' ' && !char.IsUpper(Text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(Text[i - 1]) &&
                         i < Text.Length - 1 && !char.IsUpper(Text[i + 1])))
                        newText.Append(' ');
                newText.Append(Text[i]);
            }
            return newText.ToString();
        }
    }
}
