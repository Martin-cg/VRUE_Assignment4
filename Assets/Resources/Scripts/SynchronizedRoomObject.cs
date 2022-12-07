using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Collections.Generic;

public class SynchronizedRoomObject : MonoBehaviourPunCallbacks {
    private readonly Dictionary<string, CustomProperty> CustomProperties = new();
    private string ScenePath;

    protected virtual void Start() {
        ScenePath = gameObject.GetScenePathString();
    }

    protected void SetProperty<T>(string name, T value) {
        var property = GetProperty(name);
        if (property.Value == (object) value) {
            return;
        }
        property.Value = value;
        property.PropertyTableCache[property.Key] = value;
        if (PhotonNetwork.CurrentRoom != null) {
            PhotonNetwork.CurrentRoom.SetCustomProperties(property.PropertyTableCache);
        }
    }
    protected void RegisterProperty<T>(string name, T value, Action<T> propertyChangeCallback) {
        var key = GetKeyForName(name);
        CustomProperties.Add(key, new CustomProperty() {
            Name = name,
            Key = key,
            Value = value,
            ChangeCallback = v => propertyChangeCallback((T)v),
            PropertyTableCache = new Hashtable()
        });
    }
    private CustomProperty GetProperty(string name) {
        var key = GetKeyForName(name);
        if (CustomProperties.TryGetValue(key, out var property)) { return property; }
        throw new Exception("Custom property not defined.");
    }
    private string GetKeyForName(string name) {
        return $"{ScenePath}/{name}";
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        foreach (var (name, property) in CustomProperties) {
            if (propertiesThatChanged.TryGetValue(name, out var newValue)) {
                if (newValue != property.Value) {
                    property.ChangeCallback.Invoke(newValue);
                }
            }
        }
    }

    private class CustomProperty {
        public string Name;
        public string Key;
        public object Value;
        public Action<object> ChangeCallback;
        public Hashtable PropertyTableCache;
    }
}
