﻿using System.Collections.Generic;
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
    public static float NextFloat(this System.Random random, float min, float max) {
        return (float)random.NextDouble() * (max - min) + min;
    }
}
