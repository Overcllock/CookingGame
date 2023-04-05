using System;
using System.Collections.Generic;
using Game.Utilites;
using UnityEngine;

namespace Game.Infrastructure
{
    public class SimpleServiceLocatorManager : IServiceLocatorManager
    {
        private Dictionary<Type, object> _referenceCache;

        public SimpleServiceLocatorManager()
        {
            _referenceCache = new Dictionary<Type, object>();
        }

        public void Add<T>(T value)
        {
            var type = typeof(T);
            
            if (TryGet<T>(out var val))
            {
                Debug.LogWarning($"Trying to add existing service type {type}");
                return;
            }
            
            _referenceCache.Add(type, value);
        }

        public T Get<T>()
        {
            Type type = typeof(T);

            if (_referenceCache.TryGetValue(type, out var obj))
            {
                return (T)obj;
            }

            Error.Assert(true, $"Cannot resolve type: {type.Name}");
            return default;
        }

        public bool TryGet<T>(out T value)
        {
            try
            {
                value = Get<T>();
                return value != null;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public void Dispose()
        {
            foreach (var obj in _referenceCache.Values)
            {
                if (obj is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            
            _referenceCache.Clear();
            _referenceCache = null;
        }
    }
}