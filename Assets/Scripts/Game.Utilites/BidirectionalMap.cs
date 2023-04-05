using System;
using System.Collections.Generic;

namespace Game.Utilities
{
    /// <summary>
    /// Bidirectional collection where you can lookup
    /// both ways (with either "keys" or "values").
    /// All added entries must be unique in order to map properly.
    /// <br/>
    ///
    /// Note that indexer calls will be ambiguous if first and second
    /// arguments are of the same type.
    /// </summary>
    public class BidirectionalMap<TFirst, TSecond>
    {
        private readonly IDictionary<TFirst, TSecond> firstToSecond_;
        private readonly IDictionary<TSecond, TFirst> secondToFirst_;

        public int Count { get { return firstToSecond_.Count; } }

        public TSecond this[TFirst first]
        {
            get { return firstToSecond_[first]; }
            set
            {
                firstToSecond_[first] = value;
                secondToFirst_[value] = first;
            }
        }

        public TFirst this[TSecond second]
        {
            get { return secondToFirst_[second]; }
            set
            {
                secondToFirst_[second] = value;
                firstToSecond_[value] = second;
            }
        }

        public BidirectionalMap()
        {
            firstToSecond_ = new Dictionary<TFirst, TSecond>();
            secondToFirst_ = new Dictionary<TSecond, TFirst>();
        }

        public BidirectionalMap(int count)
        {
            firstToSecond_ = new Dictionary<TFirst, TSecond>(count);
            secondToFirst_ = new Dictionary<TSecond, TFirst>(count);
        }

        public void Add(TFirst first, TSecond second)
        {
            if (firstToSecond_.ContainsKey(first) ||
                secondToFirst_.ContainsKey(second))
            {
                throw new ArgumentException("Detected duplicate entries for biderectional map.");
            }

            firstToSecond_.Add(first, second);
            secondToFirst_.Add(second, first);
        }

        public TSecond GetByFirst(TFirst first)
        {
            return firstToSecond_[first];
        }

        public TFirst GetBySecond(TSecond second)
        {
            return secondToFirst_[second];
        }

        public bool TryGetByFirst(TFirst first, out TSecond second)
        {
            return firstToSecond_.TryGetValue(first, out second);
        }

        public bool TryGetBySecond(TSecond second, out TFirst first)
        {
            return secondToFirst_.TryGetValue(second, out first);
        }
    }
}