using System;
using System.Collections.Generic;

namespace Game.Content
{
    public struct ContentData
    {
        public Dictionary<string, ContentMap> contentMaps;
        public Dictionary<Type, ContentModuleSettings> settings;
    }

    public interface IContentConfigurer
    {
        void ConfigureData(Action<ContentData> callback);
    }
}