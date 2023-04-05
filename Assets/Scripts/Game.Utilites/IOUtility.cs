using System.IO;
using UnityEngine;

public static class IOUtility
{
    public static string GetRelativePath(string absoulteFilePath)
    {
        absoulteFilePath = Path.GetFullPath(absoulteFilePath);
        string absoulteAssetsDirectory = Path.GetFullPath(Application.dataPath);
        string relativeFilePath = absoulteFilePath.Replace(absoulteAssetsDirectory, "Assets");
        return relativeFilePath;
    }

    public static string GetAbsolutePath(string relativeAssetPath)
    {
        relativeAssetPath = relativeAssetPath.Replace("Assets/", "");
        return $"{Application.dataPath}/{relativeAssetPath}";
    }
}