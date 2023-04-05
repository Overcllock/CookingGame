using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace UnityEditor
{
    public static class EditorAssetsCache
    {
        private static Dictionary<Type, Object[]> _cachedAssets;

        public static T GetCachedAsset<T>() where T : Object
        {
            var objs = GetCachedAssets(typeof(T));
            var instance = objs.First();

            return instance as T;
        }

        public static Object[] GetCachedAssets(Type type)
        {
            if (_cachedAssets == null)
            {
                _cachedAssets = new Dictionary<Type, Object[]>();
            }

            if (!_cachedAssets.TryGetValue(type, out var assets))
            {
                assets = AssetDatabaseUtility.GetAssets(type);
                _cachedAssets[type] = assets;
            }

            return assets;
        }

        public static void RefreshCache()
        {
            if (_cachedAssets.IsNullOrEmpty()) return;

            foreach (var kvp in _cachedAssets)
            {
                var type = kvp.Key;
                _cachedAssets[type] = AssetDatabaseUtility.GetAssets(type);
            }
        }

        public static void ResetCache()
        {
            _cachedAssets.Clear();
        }
    }
}