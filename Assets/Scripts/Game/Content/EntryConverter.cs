namespace Game.Content
{
    public abstract class EntryConverter<TSource, TEntry> : ContentConverter<TSource, TEntry>
        where TSource : class, IEntrySource
        where TEntry : ContentEntry
    {
        public override TEntry Convert(TSource source)
        {
            TEntry entry = SourceToSettings(source);
            entry.id = source.id;

            return entry;
        }
    }
}