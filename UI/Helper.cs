using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Aligner.UI
{
    static class Helper
    {
        public static void Map<T>(this IEnumerable<T> e, Action<T> a)
        {
            foreach (var item in e) a(item);
        }
    }
}
