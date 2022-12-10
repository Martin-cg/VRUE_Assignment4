using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utils {
    public static int RandomSeed() => Random.Range(int.MinValue, int.MaxValue);

    public static GameObject FindObjectByScenePath(string[] scenePath) {
        if (scenePath == null || scenePath.Length == 0) {
            return null;
        }

        var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        var current = rootObjects.First(obj => obj.name == scenePath[0]).transform;
        foreach (var name in scenePath.Skip(1)) {
            current = current.Find(name);
        }
        return current.gameObject;
    }
}
