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

    protected void SetProperty<T>(string key, T value, bool triggerLocalCallback=false) {
        var property = GetProperty(key);
        property.PropertyTableCache[key] = value;
        PhotonNetwork.CurrentRoom.SetCustomProperties(property.PropertyTableCache);
        if (triggerLocalCallback) {
            property.PropertyChange.Invoke(value);
        }
    }
    protected void RegisterProperty<T>(string key, Action<T> propertyChangeCallback) {
        var realKey = $"{ScenePath}/{key}";
        CustomProperties.Add(realKey, new CustomProperty() {
            Name = key,
            PropertyChange = v => propertyChangeCallback((T)v),
            PropertyTableCache = new Hashtable()
        });
    }
    private CustomProperty GetProperty(string key) {
        var realKey = $"{ScenePath}/{key}";
        if (CustomProperties.TryGetValue(realKey, out var property)) { return property; }
        throw new Exception("Custom property not defined.");
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) {
        foreach (var (name, property) in CustomProperties) {
            if (propertiesThatChanged.TryGetValue(name, out var newValue)) {
                property.PropertyChange.Invoke(newValue);
            }
        }
    }

    private struct CustomProperty {
        public string Name;
        public Action<object> PropertyChange;
        public Hashtable PropertyTableCache;
    }
}
