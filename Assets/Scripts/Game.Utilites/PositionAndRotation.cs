using System;
using UnityEngine;

[Serializable]
public struct PositionAndRotation
{
    public Vector3 position;
    public Quaternion rotation;

    public PositionAndRotation(Vector3 position)
    {
        this.position = position;
        rotation = Quaternion.identity;
    }

    public PositionAndRotation(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }

    public static PositionAndRotation FromTransform(Transform transform)
    {
        return new PositionAndRotation(transform.position, transform.rotation);
    }

    public void ApplyTo(Transform transform)
    {
        transform.SetPositionAndRotation(position, rotation);
    }
}