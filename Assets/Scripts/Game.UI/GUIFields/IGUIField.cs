using System;

namespace Game.Development
{
    public interface IGUIField
    {
        Type type { get; }

        void Draw();

        T GetValue<T>();
        object GetValue();
    }
}