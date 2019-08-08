using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Utils
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string FormatedSpace(this string val, int fixedLen)

        {
            if (val == null) val = "";
            int len = 0;
            string retVal = string.Empty;
            try

            {
                len = val.Length;
                retVal = val;
                for (int i = 0; i < fixedLen - len - 1; i++)
                {
                
                    retVal = retVal + " ";
                }

            }
            catch (Exception)

            {
                throw;

            }
            return retVal;

        }
    }
}
