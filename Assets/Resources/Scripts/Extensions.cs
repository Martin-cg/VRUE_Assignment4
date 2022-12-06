using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extensions {
    public static IEnumerable<GameObject> FindChildrenWithTag(this GameObject parent, string tag) {
        return parent.GetAllChildren().Where(component => component.CompareTag(tag));
    }

    public static IEnumerable<Transform> GetAllChildren(this Transform parent) {
        foreach (Transform child in parent) {
            yield return child;

            foreach (Transform transform in child.GetAllChildren()) {
                yield return transform;
            }
        }
    }

    public static IEnumerable<GameObject> GetAllChildren(this GameObject parent) {
        return parent.transform.GetAllChildren().Select(e => e.gameObject);
    }

    public static IEnumerable<string> GetScenePath(this GameObject target, GameObject parent = null, bool includeTarget = true, bool includeParent = true) {
        if (ReferenceEquals(target, parent)) {
            return Enumerable.Empty<string>();
        }

        var path = new List<string>();

        if (includeTarget) {
            path.Add(target.name);
        }

        var current = target.transform;
        while (current.parent != null && ReferenceEquals(current, parent)) {
            path.Add(current.name);
            current = current.transform.parent;
        }

        if (parent && includeParent) {
            path.Add(parent.name);
        }

        return Enumerable.Reverse(path);
    }

    public static string GetScenePathString(this GameObject target, GameObject parent = null, bool includeTarget = true, bool includeParent = true, string delimiter = "/") {
        return string.Join(delimiter, GetScenePath(target, parent, includeTarget, includeParent));
    }

    public static Vector3 WithX(this Vector3 vector, float x) {
        return new Vector3(x, vector.y, vector.z);
    }
    public static Vector3 WithY(this Vector3 vector, float y) {
        return new Vector3(vector.x, y, vector.z);
    }
    public static Vector3 WithZ(this Vector3 vector, float z) {
        return new Vector3(vector.x, vector.y, z);
    }

    public static double NextDouble(this System.Random random, double min, double max) {
        return random.NextDouble() * (max - min) + min;
    }
    public static float NextFloat(this System.Random random) {
        return (float)random.NextDouble();
    }
    public static float NextFloat(this System.Random random, float min, float max) {
        return (float)random.NextDouble(min, max);
    }

    public static TValue GetOrAddNew<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : new() => GetOrAddWith(dict, key, _ => new TValue());
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value) => GetOrAddWith(dict, key, _ => value);
    public static TValue GetOrAddWith<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> valueSupplier) {
        if (!dict.TryGetValue(key, out TValue val)) {
            val = valueSupplier(key);
            dict.Add(key, val);
        }
        return val;
    }
}
