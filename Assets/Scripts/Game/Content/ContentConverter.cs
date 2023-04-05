using System;
using UnityEngine;

namespace Game.Content
{
    public interface IContentConverter
    {
        public Type resultType { get; }
        public Type sourceType { get; }

        public ContentSettings Convert(IContentSource source);
        public T Convert<T>(IContentSource source) where T : ContentSettings;
    }

    public abstract class ContentConverter<TSource, TResult> : IContentConverter
        where TSource : class, IContentSource
        where TResult : ContentSettings
    {
        public Type resultType { get { return typeof(TResult); } }
        public Type sourceType { get { return typeof(TSource); } }

        public virtual TResult Convert(TSource source)
        {
            var entry = SourceToSettings(source);
            return entry;
        }

        public T Convert<T>(IContentSource source) where T : ContentSettings
        {
            TResult entry = Convert((TSource)source);
            T result = entry as T;

            if (result == null)
            {
                Debug.LogWarning(
                    $"Could not convert config of type: [{source.GetType().Name}] " +
                    $"into settings of type: [{typeof(T).Name}]");
            }

            return result;
        }

        public ContentSettings Convert(IContentSource source)
        {
            TSource cast = source as TSource;

            if (cast == null)
            {
                Debug.LogWarning(
                    $"Trying to convert invalid type of config source: " +
                    $"[{source.GetType().Name}] into entry of type: [{resultType.Name}]");
                return null;
            }

            return Convert(cast);
        }

        protected abstract TResult SourceToSettings(TSource source);
    }
}