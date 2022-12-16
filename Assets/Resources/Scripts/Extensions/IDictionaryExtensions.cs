using System;
using System.Collections.Generic;

public static class IDictionaryExtensions {
    public static TValue GetOrAddNew<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : new() => GetOrAddWith(dict, key, _ => new TValue());
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value) => GetOrAddWith(dict, key, _ => value);
    public static TValue GetOrAddWith<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> valueSupplier) {
        if (!dict.TryGetValue(key, out TValue val)) {
            val = valueSupplier(key);
            dict.Add(key, val);
        }
        return val;
    }

    public static bool Remove<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, out TValue target) {
        if (dict.TryGetValue(key, out target)) {
            return dict.Remove(key);
        } else {
            return false;
        }
    }

    public static bool AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue, Func<TValue, TValue> valueMapper) {
        if (dict.TryGetValue(key, out var value)) {
            dict[key] = valueMapper(value);
            return false;
        } else {
            dict.Add(key, defaultValue);
            return true;
        }
    }

    public static TValue UpdateAndGet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue, TValue> valueMapper) {
        if (dict.TryGetValue(key, out var value)) {
            var newValue = valueMapper(value);
            dict[key] = newValue;
            return newValue;
        } else {
            return default;
        }
    }
    public static TValue GetAndUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue, TValue> valueMapper) {
        if (dict.TryGetValue(key, out var value)) {
            var newValue = valueMapper(value);
            dict[key] = newValue;
            return value;
        } else {
            return default;
        }
    }
}
