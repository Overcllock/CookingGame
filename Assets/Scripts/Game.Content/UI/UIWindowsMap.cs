using System;
using System.Collections.Generic;

namespace Game.Content.UI
{
    public class UIWindowsMap : ContentMap
    {
        public EntryMap<UIWindowEntry> settings = new EntryMap<UIWindowEntry>();
        
        protected override Dictionary<Type, EntryMap> GetMaps()
        {
            return new Dictionary<Type, EntryMap>
            {
                [typeof(UIWindowEntry)] = settings
            };
        }
    }
}