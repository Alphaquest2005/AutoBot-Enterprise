using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

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
                    if (Text[i - 1] != ' ' && !char.IsUpper(Text[i - 1]) ||
                        preserveAcronyms && char.IsUpper(Text[i - 1]) &&
                        i < Text.Length - 1 && !char.IsUpper(Text[i + 1]))
                        newText.Append(' ');
                newText.Append(Text[i]);
            }

            return newText.ToString();
        }

        public static string NameOfCallingClass()
        {
            string fullName;
            Type declaringType;
            int skipFrames = 2;
            do
            {
                MethodBase method = new StackFrame(skipFrames, false).GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null)
                {
                    return method.Name;
                }
                skipFrames++;
                fullName = declaringType.Name;
            }
            while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            return fullName;
        }
    }
}