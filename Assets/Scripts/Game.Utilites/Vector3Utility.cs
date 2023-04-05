using UnityEngine;

public static class Vector3Utility
{
    public static bool TryParse(string source, out Vector3 vector, char separator = ';')
    {
        vector = default;
        if (string.IsNullOrEmpty(source)) return false;

        if (source.StartsWith("(") && source.EndsWith(")"))
        {
            source = source.Substring(1, source.Length - 2);
        }

        string[] values = source.Split(separator);
        if (values.Length == 3)
        {
            vector = new Vector3(
            float.Parse(values[0]),
            float.Parse(values[1]),
            float.Parse(values[2]));

            return true;
        }

        return false;
    }

    public static Vector3 Parse(string v3source, char separator = ';')
    {
        TryParse(v3source, out var vector, separator);
        return vector;
    }

    public static Vector3 RandomPointInBounds(Vector3 center, Bounds bounds, bool ignoreVerticality = false)
    {
        Vector3 point = RandomPointInBounds(bounds, ignoreVerticality);
        return center + point;
    }

    public static Vector3 RandomPointInBounds(Bounds bounds, bool ignoreVerticality = false)
    {
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = ignoreVerticality ? bounds.min.y : Random.Range(bounds.min.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(x, y, z);
    }

    public static Vector3 RandomPointOnPlainCircle(Vector3 center, float radius)
    {
        Vector3 pos = Random.insideUnitCircle * radius;
        return new Vector3(pos.x, 0, pos.y) + center;
    }

    public static Vector3 RandomPointOnPlainCircleEdge(Vector3 center, float radius)
    {
        float angle = Random.Range(0, 2f * Mathf.PI);
        return center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
    }

    public static Vector3[] DistributePointsOnCircleEdge(Vector3 center, int pointsAmount, float radius)
    {
        Vector3[] points = new Vector3[pointsAmount];

        for (int i = 0; i < pointsAmount; i++)
        {
            var radians = 2 * Mathf.PI / pointsAmount * i;

            var vertical = Mathf.Sin(radians);
            var horizontal = Mathf.Cos(radians);

            points[i] = center + (new Vector3(horizontal, 0, vertical) * radius);
        }

        return points;
    }

    public static Vector3 ShiftX(this Vector3 v3, float x)
    {
        return new Vector3(x, v3.y, v3.z);
    }

    public static Vector3 ShiftY(this Vector3 v3, float y)
    {
        return new Vector3(v3.x, y, v3.z);
    }

    public static Vector3 ShiftZ(this Vector3 v3, float z)
    {
        return new Vector3(v3.x, v3.y, z);
    }
}