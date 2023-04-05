using Game.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Content
{
    public class ContentConvertationAgent
    {
        private List<IContentConverter> _converters;
        private Dictionary<Type, IContentConverter> _convertersByType;

        public ContentConvertationAgent()
        {
            _converters = ReflectionUtility.InstantiateAllTypes<IContentConverter>();
            _convertersByType = new Dictionary<Type, IContentConverter>();
            for (int i = 0; i < _converters.Count; i++)
            {
                var converter = _converters[i];
                _convertersByType[converter.resultType] = converter;
            }
        }

        public bool TryGetConverter(Type settingsType, out IContentConverter converter)
        {
            return _convertersByType.TryGetValue(settingsType, out converter);
        }

        public bool TryGetConverter<TResult>(out IContentConverter converter) where TResult : ContentSettings
        {
            return TryGetConverter(typeof(TResult), out converter);
        }

        public TResult Convert<TResult>(IEntrySource source) where TResult : ContentSettings
        {
            if (TryGetConverter<TResult>(out IContentConverter converter))
            {
                return converter.Convert<TResult>(source);
            }

            Debug.LogWarning($"Could not find converter for entry of type: [{typeof(TResult).Name}]");
            return null;
        }

        public TEntry Convert<TSource, TEntry>(TSource source)
            where TSource : class, IEntrySource
            where TEntry : ContentEntry
        {
            Type converterType = typeof(ContentConverter<TSource, TEntry>);

            IContentConverter nextConverter = null;
            for (int i = 0; i < _converters.Count; i++)
            {
                nextConverter = _converters[i];
                if (nextConverter.GetType() == converterType) break;
            }

            ContentConverter<TSource, TEntry> converter = nextConverter as ContentConverter<TSource, TEntry>;
            if (converter != null)
            {
                return converter.Convert(source);
            }

            Debug.LogWarning($"Could not find entry converter of type: [{converterType}]");
            return null;
        }

        /// <summary>
        /// Mapped settings (first) to sources (second).
        /// </summary>
        public BidirectionalMap<Type, Type> GetAssociativeMap()
        {
            BidirectionalMap<Type, Type> map = new BidirectionalMap<Type, Type>();

            for (int i = 0; i < _converters.Count; i++)
            {
                var converter = _converters[i];
                map.Add(converter.resultType, converter.sourceType);
            }

            return map;
        }
    }
}