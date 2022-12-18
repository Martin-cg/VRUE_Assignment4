using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utils {
    public static int RandomSeed() => Random.Range(int.MinValue, int.MaxValue);

    public static GameObject ResolveScenePath(IEnumerable<string> scenePath, GameObject root=null) {
        if (scenePath == null) {
            return null;
        }

        Transform current;
        if (root == null) {
            var rootName = scenePath.FirstOrDefault();
            if (rootName == null) {
                return null;
            }

            var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            current = rootObjects.First(obj => obj.name == rootName).transform;
        } else {
            current = root.transform;
        }
        foreach (var name in scenePath) {
            current = current.Find(name);
            if (current == null) {
                return null;
            }
        }

        return current.gameObject;
    }
}
