using System;
using System.Collections.Generic;

namespace Advent
{
    public class FuncComparer<T> : Comparer<T>
    {
        private readonly Func<T, T, int> m_func;
        public FuncComparer(Func<T, T, int> func)
        {
            m_func = func;
        }
        public override int Compare(T x, T y) => m_func(x, y);
    }
}
