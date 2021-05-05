using System;
using System.Text.RegularExpressions;

namespace Core.Common.Utils
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string Right(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(value.Length - maxLength);
        }

        public static string FormatedSpace(this string val, int fixedLen)

        {
            if (val == null) val = "";
            var len = 0;
            var retVal = string.Empty;
            try

            {
                len = val.Length;
                retVal = val;
                for (var i = 0; i < fixedLen - len - 1; i++) retVal = retVal + " ";
            }
            catch (Exception)

            {
                throw;
            }

            return retVal;
        }

        public static string ReplaceSpecialChar(this string msgSubject, string rstring)
        {
            return Regex.Replace(msgSubject, @"[^0-9a-zA-Z\s]+", rstring);
        }

        public static string UpdateToCurrentUser(string dataFolder)
        {
            return dataFolder.Replace("josep", Environment.UserName);
        }
    }
}