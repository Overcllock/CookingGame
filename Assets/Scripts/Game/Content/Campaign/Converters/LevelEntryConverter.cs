namespace Game.Content.Campaign
{
    public class LevelEntryConverter : EntryConverter<LevelSettingsScrobject, LevelEntry>
    {
        protected override LevelEntry SourceToSettings(LevelSettingsScrobject source)
        {
            return new LevelEntry
            {
                time = source.time,
                ordersCount = source.ordersCount,
                ordersCapacity = source.ordersCapacity,
                minOrderTime = source.minOrderTime,
                maxOrderTime = source.maxOrderTime,
                orders = source.orders,
                availableGuests = source.availableGuests,
                recipesSources = source.recipesSources,
                randomizeGuests = source.randomizeGuests
            };
        }
    }
}