using System.Collections.Generic;

namespace Game.Content
{
    [System.Serializable]
    public class ContentModuleContainer
    {
        public string moduleName;
        public List<ContentEntry> entries;
        public ContentModuleSettings settings;

        public override string ToString()
        {
            var entryInfo = StringUtility.GetCompositeString(entries);
            return $"Module name: [ {moduleName} ] " +
                   $"\n\nEntries: {entryInfo}";
        }
    }
}