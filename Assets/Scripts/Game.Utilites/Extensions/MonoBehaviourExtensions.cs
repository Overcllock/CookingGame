using System;
using System.Collections.Generic;
using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static T GetRequiredComponent<T>(this GameObject gameObject)
    {
        var component = gameObject.GetComponent<T>();
        Debug.Assert(component != null, $"Missing component {typeof(T).Name}.", gameObject);
        return component;
    }

    public static T GetRequiredComponent<T>(this MonoBehaviour monoBehaviour)
    {
        var component = monoBehaviour.GetComponent<T>();
        Debug.Assert(component != null, $"Missing component {typeof(T).Name}.", monoBehaviour);
        return component;
    }

    public static List<T> GetComponentsInChildren<T>(this GameObject gameObject, Func<T, bool> predicate, bool includeInactive = false) where T : Component
    {
        var allComps = gameObject.GetComponentsInChildren<T>(includeInactive);
        var required = new List<T>();

        for (int i = 0; i < allComps.Length; i++)
        {
            T nextComponent = allComps[i];
            if (predicate.Invoke(nextComponent))
            {
                required.Add(nextComponent);
            }
        }

        return required;
    }

    public static bool HasComponent<T>(this GameObject gameObject) where T : Component
    {
        var component = gameObject.GetComponent<T>();
        return component != null;
    }
    public static bool HasComponentInChildren<T>(this GameObject gameObject) where T : Component
    {
        var component = gameObject.GetComponentInChildren<T>();
        return component != null;
    }

    public static bool HasComponent<T>(this Component source) where T : Component
    {
        var component = source.GetComponent<T>();
        return component != null;
    }

    public static T AddComponent<T>(this Component source) where T : Component
    {
        return source.gameObject.AddComponent<T>();
    }

    public static T GetOrAddComponent<T>(this GameObject source) where T : Component
    {
        var component = source.GetComponent<T>();

        return component == null ?
            source.AddComponent<T>() :
            component;
    }

    public static T GetOrAddComponent<T>(this Component source) where T : Component
    {
        var component = source.GetComponent<T>();

        return component == null ?
            source.AddComponent<T>() :
            component;
    }

    public static void EnsureComponentExists<T>(this GameObject source) where T : Component
    {
        source.GetOrAddComponent<T>();
    }

    public static void EnsureComponentExists<T>(this Component source) where T : Component
    {
        source.GetOrAddComponent<T>();
    }

    public static void SetActive<T>(this T component, bool active) where T : Component
    {
        if (component == null || component.gameObject == null) return;
        component.gameObject.SetActive(active);
    }
    
    public static void SetActive(this GameObject[] gameObjects, bool active)
    {
        if (gameObjects.IsNullOrEmpty())
            return;
        
        for (int i = 0; i < gameObjects.Length; i++)
        {
            var gameObject = gameObjects[i];
            if (gameObject == null || gameObject.gameObject == null) return;
            gameObject.gameObject.SetActive(active);
        }
    }
    
    public static void SetActive<T>(this T[] components, bool active) where T : Component
    {
        if (components.IsNullOrEmpty())
            return;
        
        for (int i = 0; i < components.Length; i++)
        {
            var component = components[i];
            if (component == null || component.gameObject == null) return;
            component.gameObject.SetActive(active);
        }
    }

    public static void SetActiveSafe(this GameObject gameObject, bool active)
    {
        if (gameObject != null)
        {
            gameObject.SetActive(active);
        }
    }
}