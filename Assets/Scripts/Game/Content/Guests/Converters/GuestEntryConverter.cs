namespace Game.Content.Guests
{
    public class GuestEntryConverter : EntryConverter<GuestSettingsScrobject, GuestEntry>
    {
        protected override GuestEntry SourceToSettings(GuestSettingsScrobject source)
        {
            return new GuestEntry
            {
                icon = source.icon,
                minRecipesCount = source.minRecipesCount,
                maxRecipesCount = source.maxRecipesCount
            };
        }
    }
}