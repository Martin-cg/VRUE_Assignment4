using UnityEngine;

public static class ResourcePathUtils {
    public static string GetPrefabPath(string name) => $"Prefabs/{name}";
    public static string GetPrefabPath(GameObject prefab) => GetPrefabPath(prefab.name);

    public static string GetMaterialPath(string name) => $"Materials/{name}";
    public static string GetMaterialPath(Material material) => GetMaterialPath(material.name);
}
