using System;

namespace Game.Content
{
    public class ContentInstantiator : IContentInstantiator
    {
        public ContentMap Instantiate(Type type)
        {
            var instance = Activator.CreateInstance(type);
            try
            {
                return (ContentMap)instance;
            }
            catch (Exception e)
            {
                throw new Exception($"Could not convert type into content map: [{type.Name}]", e);
            }
        }
    }
}