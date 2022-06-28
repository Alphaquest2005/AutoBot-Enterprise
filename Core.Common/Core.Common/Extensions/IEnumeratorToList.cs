using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Extensions
{
    public static class IEnumeratorToList
    {
        public static List<T> ToList<T>(this System.Collections.IEnumerator e)
        {
            var list = new List<T>();
            while (e.MoveNext())
            {
                list.Add((T)e.Current);
            }
            return list;
        }
    }
}
