using Game.Infrastructure;
using UnityEngine;

public class ServiceLocator : StaticWrapper<IServiceLocatorManager>
{
    public static void Add<T>(T service)
    {
        if (InitializationCheck())
        {
            _instance.Add(service);
        }
    }
    
    public static void Get<T>(out T service)
    {
        if (!TryGet(out service))
        {
            Debug.LogWarning($"Could not find service of type: {typeof(T)}");
        }
    }

    public static T Get<T>()
    {
        if (InitializationCheck())
        {
            return _instance.Get<T>();
        }

        return default;
    }

    public static bool TryGet<T>(out T service)
    {
        if (InitializationCheck())
        {
            return _instance.TryGet(out service);
        }

        service = default;
        return false;
    }

    public static void Dispose()
    {
        if (InitializationCheck())
        {
            _instance.Dispose();
        }
    }
}