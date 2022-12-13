using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extensions {
    public static bool HasComponent<T>(this GameObject obj) where T : Component => obj.GetComponent<T>() != null;
    public static bool HasComponent<T>(this Component component) where T : Component => component.GetComponent<T>() != null;

    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component {
        var instance = obj.GetComponent<T>();
        if (!instance) {
            instance = obj.AddComponent<T>();
        }
        return instance;
    }

    public static IEnumerable<GameObject> FindChildrenWithTag(this GameObject parent, string tag) {
        return parent.GetAllChildren().Where(component => component.CompareTag(tag));
    }

    public static IEnumerable<Transform> GetChildren(this Transform parent) {
        return parent.Cast<Transform>();
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

    public static List<string> GetScenePath(this GameObject target, GameObject parent = null, bool includeTarget = true, bool includeParent = true) =>
        GetScenePath(target.transform, parent == null ? null : parent.transform, includeTarget, includeParent);

    public static string GetScenePathString(this GameObject target, GameObject parent = null, bool includeTarget = true, bool includeParent = true, string delimiter = "/") =>
        GetScenePathString(target.transform, parent == null ? null : parent.transform, includeTarget, includeParent, delimiter);

    public static List<string> GetScenePath(this Transform target, Transform parent = null, bool includeTarget = true, bool includeParent = true) {
        var path = new List<string>();

        if (ReferenceEquals(target, parent)) {
            return path;
        }

        if (includeTarget) {
            path.Add(target.name);
        }

        var current = target;
        while (current.parent != null && !ReferenceEquals(current, parent)) {
            current = current.transform.parent;
            path.Add(current.name);
        }

        if (parent && includeParent) {
            path.Add(parent.name);
        }

        path.Reverse();

        return path;
    }

    public static string GetScenePathString(this Transform target, Transform parent = null, bool includeTarget = true, bool includeParent = true, string delimiter = "/") {
        return string.Join(delimiter, GetScenePath(target, parent, includeTarget, includeParent));
    }

    public static bool IsChildOf(this GameObject target, GameObject parent) => IsChildOf(target.transform, parent.transform);
    public static bool IsChildOf(this Transform target, Transform parent) {
        var current = target;

        while (current.parent != null) {
            current = current.parent;
            if (current == parent) {
                return true;
            }
        }

        return false;
    }
    public static bool IsParentOf(this GameObject target, GameObject child) => IsParentOf(target.transform, child.transform);
    public static bool IsParentOf(this Transform target, Transform child) => IsChildOf(child, target);

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

    public static T ReceiveNext<T>(this PhotonStream stream) => (T)stream.ReceiveNext();

    public static Vector3 Average(this IEnumerable<Vector3> source) {
        if (source == null) {
            throw new ArgumentNullException(nameof(source));
        }

        var sum = Vector3.zero;
        var count = 0L;
        checked {
            foreach (var item in source) {
                sum += item;
                count++;
            }
        }
        if (count > 0) {
            return sum / count;
        }
        throw new InvalidOperationException("source sequence is empty");
    }
    public static Vector2 Average(this IEnumerable<Vector2> source) {
        if (source == null) {
            throw new ArgumentNullException(nameof(source));
        }

        var sum = Vector2.zero;
        var count = 0L;
        checked {
            foreach (var item in source) {
                sum += item;
                count++;
            }
        }
        if (count > 0) {
            return sum / count;
        }
        throw new InvalidOperationException("source sequence is empty");
    }
}
