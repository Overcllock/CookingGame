namespace Game.Content.Recipes
{
    public class RecipeEntryConverter : EntryConverter<RecipeSettingsScrobject, RecipeEntry>
    {
        protected override RecipeEntry SourceToSettings(RecipeSettingsScrobject source)
        {
            return new RecipeEntry
            {
                icon = source.icon
            };
        }
    }
}