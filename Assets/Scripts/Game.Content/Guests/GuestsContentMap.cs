using System;
using System.Collections.Generic;

namespace Game.Content.Guests
{
    public class GuestsContentMap : ContentMap
    {
        public EntryMap<GuestEntry> guests = new EntryMap<GuestEntry>();

        protected override Dictionary<Type, EntryMap> GetMaps()
        {
            return new Dictionary<Type, EntryMap>
            {
                [typeof(GuestEntry)] = guests
            };
        }
    }
}