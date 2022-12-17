using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UnityExtensions {
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

    public static IEnumerable<Transform> GetChildren(this Transform parent) => parent.Cast<Transform>();
    public static IEnumerable<GameObject> GetChildren(this GameObject parent) => parent.transform.GetChildren().Select(transform => transform.gameObject);
    public static IEnumerable<Transform> GetAllChildren(this Transform parent) {
        foreach (Transform child in parent) {
            yield return child;

            foreach (Transform transform in child.GetAllChildren()) {
                yield return transform;
            }
        }
    }
    public static IEnumerable<GameObject> GetAllChildren(this GameObject parent) => parent.transform.GetAllChildren().Select(transform => transform.gameObject);

    public static List<string> GetScenePath(this GameObject target, GameObject parent = null, bool includeTarget = true, bool includeParent = true) =>
        GetScenePath(target.transform, parent == null ? null : parent.transform, includeTarget, includeParent);

    public static string GetScenePathString(this GameObject target, GameObject parent = null, bool includeTarget = true, bool includeParent = true, string delimiter = "/") =>
        GetScenePathString(target.transform, parent == null ? null : parent.transform, includeTarget, includeParent, delimiter);

    public static List<string> GetScenePath(this Transform target, Transform parent = null, bool includeTarget = true, bool includeParent = true) {
        if (ReferenceEquals(target, parent)) {
            return new List<string>();
        }

        var path = new List<string>();

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

    public static string GetScenePathString(this Transform target, Transform parent = null, bool includeTarget = true, bool includeParent = true, string delimiter = "/") =>
        string.Join(delimiter, GetScenePath(target, parent, includeTarget, includeParent));

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
    public static bool IsParentOf(this GameObject target, GameObject child) => IsChildOf(target.transform, child.transform);
    public static bool IsParentOf(this Transform target, Transform child) => IsChildOf(child, target);

    public static Pose GetPose(this Transform transform) => new(transform.position, transform.rotation);
    public static Pose GetLocalPose(this Transform transform) => new(transform.localPosition, transform.localRotation);
    public static void SetPose(this Transform transform, Pose pose) {
        transform.SetPositionAndRotation(pose.position, pose.rotation);
    }
    public static void SetLocalPose(this Transform transform, Pose pose) {
        transform.localPosition = pose.position;
        transform.localRotation = pose.rotation;
    }

    public static void SetLayerRecursively(this GameObject target, int layerMask) {
        target.layer = layerMask;
        foreach (var child in target.GetAllChildren()) {
            child.layer = layerMask;
        }
    }
}
