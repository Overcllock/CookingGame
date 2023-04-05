using System;
using System.Collections.Generic;

namespace Game.Content.Campaign
{
    public class CampaignContentMap : ContentMap
    {
        public EntryMap<LevelEntry> levels = new EntryMap<LevelEntry>();

        protected override Dictionary<Type, EntryMap> GetMaps()
        {
            return new Dictionary<Type, EntryMap>
            {
                [typeof(LevelEntry)] = levels
            };
        }
    }
}