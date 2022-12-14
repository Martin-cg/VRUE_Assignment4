using System;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions {
    public static Vector3 WithX(this Vector3 vector, float x) {
        return new Vector3(x, vector.y, vector.z);
    }
    public static Vector3 WithY(this Vector3 vector, float y) {
        return new Vector3(vector.x, y, vector.z);
    }
    public static Vector3 WithZ(this Vector3 vector, float z) {
        return new Vector3(vector.x, vector.y, z);
    }

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
