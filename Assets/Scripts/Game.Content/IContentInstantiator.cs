using System;

namespace Game.Content
{
    public interface IContentInstantiator
    {
        ContentMap Instantiate(Type type);
    }
}