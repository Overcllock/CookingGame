using System;
using System.Collections.Generic;

namespace Game.Utilities
{
    public class CompareSorter<T> : IComparer<T>
    {
        private Func<T, T, int>[] _comparisons;

        public CompareSorter(params Func<T, T, int>[] comparisons)
        {
            _comparisons = comparisons;
        }

        public int Compare(T x, T y)
        {
            var sort = 0;

            for (int i = 0; i < _comparisons.Length; i++)
            {
                var comparison = _comparisons[i];
                sort = comparison(x, y);

                if (sort != 0)
                    return sort;
            }

            return sort;
        }
    }
}