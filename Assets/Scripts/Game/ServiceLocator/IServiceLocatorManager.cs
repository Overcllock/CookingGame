using System;

namespace Game.Infrastructure
{
    public interface IServiceLocatorManager : IDisposable
    {
        void Add<T>(T value);
        T Get<T>();
        bool TryGet<T>(out T value);
    }
}
