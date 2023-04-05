using System;
using System.Collections.Generic;

namespace Game.Utilities
{
    public abstract class BasePredicated<TPredicate> : IDisposable where TPredicate : Delegate
    {
        protected List<TPredicate> _predicates;

        public virtual void AddPredicate(params TPredicate[] predicates)
        {
            if (_predicates == null)
            {
                _predicates = new List<TPredicate>(predicates.Length);
            }

            for (int i = 0; i < predicates.Length; i++)
            {
                _predicates.Add(predicates[i]);
            }
        }
        public void Dispose()
        {
            _predicates = null;
        }
    }

    public abstract class Predicated : BasePredicated<Func<bool>>
    {
        protected bool CheckPredicates()
        {
            if (_predicates == null) return true;

            for (int i = 0; i < _predicates.Count; i++)
            {
                if (!_predicates[i].Invoke())
                {
                    return false;
                }
            }

            return true;
        }
    }

    public abstract class Predicated<T> : BasePredicated<Func<T, bool>>
    {
        protected bool CheckPredicates(T obj)
        {
            if (_predicates == null) return true;

            for (int i = 0; i < _predicates.Count; i++)
            {
                if (!_predicates[i].Invoke(obj))
                {
                    return false;
                }
            }

            return true;
        }
    }
}