namespace Game.Content
{
    [System.Serializable]
    public abstract class ContentEntry : ContentSettings
    {
        public string id;

        public override string ToString()
        {
            return $"Type: [ {GetType().Name} ] Id: [ {id} ]";
        }
    }
}