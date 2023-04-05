using System;

namespace Game.Content
{
    [Serializable]
    public class ContentManifest
    {
        public ContentManifestEntry[] files;
        public string sha;
    }

    [Serializable]
    public class ContentManifestEntry
    {
        public string file;
        public string sha;
    }
}