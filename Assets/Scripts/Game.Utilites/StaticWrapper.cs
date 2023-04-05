using System;

/// <summary>
/// Use it to implement static wrapper for features.
/// Check each method invocation with InitializationCheck to lot invalid use.
/// Should only be one wrapper per type of T.
/// </summary>
public abstract class StaticWrapper<T> where T : class
{
    protected static T _instance;

    public static bool initialized { get { return _instance != null; } }

    protected StaticWrapper()
    {
    }

    public static void Initialize(T instance)
    {
        if (_instance != null &&
           _instance is IDisposable disposable)
        {
            disposable.Dispose();
        }

        _instance = instance;
    }

    protected static bool InitializationCheck()
    {
        if (!initialized)
        {
            UnityEngine.Debug.LogWarning($"Trying to access <color=white>[ {typeof(T).Name} ]</color> functions while its not initialized!");
        }

        return initialized;
    }
}