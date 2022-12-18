using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SynchronizedRoomObject : MonoBehaviourPunCallbacks {
    protected bool ShareAcrossBehavioursOnSameObject = false;

    private const string KeySeparator = "/";
    private static readonly HashSet<string> RegisteredKeys = new();
    private readonly List<ISynchronizedRoomProperty> Properties = new();
    private readonly Hashtable CurrentPropertyTableCache = new();
    private readonly Hashtable PreviousPropertyTableCache = new();
    private string ScenePath;
    private string BehaviourName;
    private bool Started = false;

    protected virtual void Awake() {
        InOnlineRoomOnly.AddTo(this);
    }

    protected virtual void Start() {
        var parentView = photonView ? photonView.gameObject : null;
        ScenePath = gameObject.GetScenePathString(parentView, true, true, KeySeparator);
        BehaviourName = GetType().FullName;

        Started = true;
        foreach (var property in Properties) {
            HandleInitialPropertyValue(property);
        }
    }

    protected SynchronizedRoomProperty<T> RegisterProperty<T>(string name, T startingValue, string key=null) {
        key ??= GetKeyForName(name);
        if (Application.isEditor) {
            var added = RegisteredKeys.Add(key);
            if (!added) {
                Debug.LogError($"[RoomProperties] Detected duplicate room property key '{key}'");
            }
        }
        var property = new SynchronizedRoomProperty<T>(name, key, startingValue, this);
        Properties.Add(property);
        if (Started) {
            HandleInitialPropertyValue(property);
        }
        return property;
    }

    private void HandleInitialPropertyValue(ISynchronizedRoomProperty property) {
        Debug.Assert(Started, this);
        Debug.Assert(PhotonNetwork.CurrentRoom != null, this);

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(property.Key, out var currentValue)) {
            property.Value = currentValue;
        } else {
            // Should not be required as every client should start with the same inital value.
            // SetRoomProperty(property.Key, currentValue);
        }
    }

    private void SetRoomProperty(string key, object value) {
        CurrentPropertyTableCache[key] = value;
        if (PhotonNetwork.CurrentRoom != null) {
            var oldValue = PreviousPropertyTableCache[key];
            Debug.Log($"[RoomProperties] Set(Key={key}, Old={(oldValue == null ? "null" : oldValue.ToString())}, New={(value == null ? "null" : value.ToString())})");
            PhotonNetwork.CurrentRoom.SetCustomProperties(CurrentPropertyTableCache, PreviousPropertyTableCache);
        }
        PreviousPropertyTableCache[key] = value;
    }

    private string GetKeyForName(string name) {
        string key = "";

        if (photonView) {
            key += photonView.ViewID + KeySeparator + KeySeparator;
        }

        key += ScenePath + KeySeparator + name;

        if (!ShareAcrossBehavioursOnSameObject) {
            key += KeySeparator + BehaviourName;
        }

        return key;
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
        public event EventHandler<T> ValueChanged;
        public SynchronizedRoomObject Object { get; private set; }
        public IEqualityComparer<T> Comparer { get; private set; }
        public T _Value;
        public T Value { get => _Value; set => SetValue(value); }
        object ISynchronizedRoomProperty.Value { get => Value; set => Value = (T) value; }

        public void SetValue(T newValue, bool notifyLocal=false, bool notifyRemote=true) {
            if (Comparer.Equals(_Value, newValue)) {
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

        internal SynchronizedRoomProperty(string name, string key, T startingValue, SynchronizedRoomObject @object, IEqualityComparer<T> comparer=null) {
            Name = name;
            Key = key;
            _Value = startingValue;
            Object = @object;
            Comparer = comparer ?? StructuralEqualityComparer<T>.Default;
        }
    }
}