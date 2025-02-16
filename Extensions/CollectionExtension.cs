using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxNestGroup.Extensions
{
    public static class CollectionExtension
    {
        public static void AddRange<T>(this ICollection<T> src_, IEnumerable<T> list_)
        {
            list_.ToList().ForEach(item_=> src_.Add(item_));
        }
        public static ICollection<T> Sort<T>(this ICollection<T> src_)
        {
            var list = src_.ToList();
            list.Sort();

            src_.Clear();
            src_.AddRange(list);

            return src_;
        }
    }
}
