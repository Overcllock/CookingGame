using UnityEngine;

public static class GameObjectExtensions
{
    /// <summary>
    /// Merge activeSelf self and activeInHierarchy
    /// </summary>
    public static bool IsActive(this GameObject obj)
    {
        return obj.activeSelf && obj.activeInHierarchy;
    }

    public static void SetLayerRecursively(this GameObject obj, int layer)
    {
        obj.layer = layer;

        var childCount = obj.transform.childCount;
        for (var i = 0; i < childCount; i++) SetLayerRecursively(obj.transform.GetChild(i).gameObject, layer);
    }

    public static Transform ExtractTransform(this Object obj)
    {
        switch (obj)
        {
            case GameObject gameObject: return gameObject.transform;
            case Component component: return component.transform;

            default: return null;
        }
    }

    public static bool TryExtractTransform(this Object obj, out Transform transform)
    {
        transform = obj.ExtractTransform();
        return transform != null;
    }
}