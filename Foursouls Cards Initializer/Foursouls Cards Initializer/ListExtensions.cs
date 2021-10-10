using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foursouls_Cards_Initializer
{
    public static class ListExtensions
    {
        public static List<R> Maped<T, R>(this List<T> list, Func<T, R> mapper)
        {
            if (list == null)
            {
                return new List<R>();
            }

            return new List<R>(list.Select(mapper));
        }

        public static List<T> Sorted<T>(this List<T> list, Comparison<T> comparison)
        {
            if (list == null)
            {
                return new List<T>();
            }

            list.Sort(comparison);
            return list;
        }
    }
}
