using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SynchronizedRoomObject : MonoBehaviourPunCallbacks {
    private readonly List<ISynchronizedRoomProperty> Properties = new();
    private readonly Hashtable CurrentPropertyTableCache = new();
    private readonly Hashtable PreviousPropertyTableCache = new();
    private string ScenePath;

    protected virtual void Start() {
        ScenePath = gameObject.GetScenePathString();
    }

    protected SynchronizedRoomProperty<T> RegisterProperty<T>(string name, T startingValue, string key=null) {
        var property = new SynchronizedRoomProperty<T>(name, key ?? GetKeyForName(name), startingValue, this);
        Properties.Add(property);
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(property.Key, out var currentValue)) {
            SetRoomProperty(property.Key, currentValue);
        }
        return property;
    }

    private void SetRoomProperty(string key, object value) {
        CurrentPropertyTableCache[key] = value;
        if (PhotonNetwork.CurrentRoom != null) {
            Debug.LogWarning($"SetCustomProperties(Key={key}, Old={PreviousPropertyTableCache[key]}, New={CurrentPropertyTableCache[key]})");
            PhotonNetwork.CurrentRoom.SetCustomProperties(CurrentPropertyTableCache, PreviousPropertyTableCache);
        }
        PreviousPropertyTableCache[key] = value;
    }

    private string GetKeyForName(string name) {
        return $"{ScenePath}/{name}";
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        foreach (var property in Properties) {
            if (propertiesThatChanged.TryGetValue(property.Key, out var newValue)) {
                property.SetValue(newValue, true, false);
            }
        }
    }

    public interface ISynchronizedRoomProperty {
        string Name { get; }
        string Key { get; }
        object Value { get; set; }
        void SetValue(object newValue, bool notifyLocal, bool notifyRemote);
    }

    public class SynchronizedRoomProperty<T> : ISynchronizedRoomProperty {
        public string Name { get; private set; }
        public string Key { get; private set; }
        private T _Value;
        public T Value {
            get => _Value;
            set => SetValue(value, true, true);
        }
        object ISynchronizedRoomProperty.Value { get => Value; set => Value = (T) value; }

        public void SetValue(T newValue, bool notifyLocal, bool notifyRemote) {
            if (Equals(_Value, newValue)) {
                return;
            }
            _Value = newValue;
            if (notifyLocal) {
                ValueChanged?.Invoke(this, _Value);
            }
            if (notifyRemote) {
                Object.SetRoomProperty(Key, _Value);
            }
        }
        void ISynchronizedRoomProperty.SetValue(object newValue, bool notifyLocal, bool notifyRemote) => SetValue((T)newValue, notifyLocal, notifyRemote);

        public event EventHandler<T> ValueChanged;
        public SynchronizedRoomObject Object;

        internal SynchronizedRoomProperty(string name, string key, T startingValue, SynchronizedRoomObject @object) {
            Name = name;
            Key = key;
            Value = startingValue;
            Object = @object;
        }
    }
}