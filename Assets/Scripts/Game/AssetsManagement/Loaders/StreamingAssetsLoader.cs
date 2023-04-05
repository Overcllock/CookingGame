namespace Game.AssetsManagement
{
    public class StreamingAssetsLoader : AssetSourceLoader
    {
        public override AssetSource source { get { return AssetSource.StreamingAssets; } }

        public override void Load<T>(AssetContainer<T> container, bool async = false)
        {
            throw new System.NotImplementedException("StreamingAssetsLoader: Yet to be implemented");
        }
    }
}