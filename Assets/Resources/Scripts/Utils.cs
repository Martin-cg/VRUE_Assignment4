using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utils {
    public static int RandomSeed() => Random.Range(int.MinValue, int.MaxValue);

    public static Transform ResolveScenePath(IEnumerable<string> scenePath, Transform root = null) {
        if (scenePath == null) {
            return null;
        }

        Transform current = root;
        if (root == null) {
            var rootName = scenePath.FirstOrDefault();
            if (rootName == null) {
                return null;
            }

            var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            current = rootObjects.First(obj => obj.name == rootName).transform;
        }

        foreach (var name in scenePath) {
            current = current.Find(name);
            if (current == null) {
                return null;
            }
        }

        return current;
    }
    public static GameObject ResolveScenePathToObject(IEnumerable<string> scenePath, GameObject root=null) {
        var rootTransform = root ? root.transform : null;
        var resolved = ResolveScenePath(scenePath, rootTransform);
        return resolved ? resolved.gameObject : null;
    }
}
