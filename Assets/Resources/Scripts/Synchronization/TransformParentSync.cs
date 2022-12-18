using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TransformParentSync : SynchronizedRoomObject {
    private SynchronizedRoomProperty<string[]> ScenePath;

    protected override void Awake() {
        base.Awake();

        var key = $"{nameof(TransformParentSync)}-{photonView.ViewID}";
        ScenePath = RegisterProperty<string[]>("ScenePath", null, key);
        ScenePath.ValueChanged += (s, e) => OnScenePathPropertyChanged(e);
    }

    protected override void Start() {
        base.Start();

        UpdateScenePathProperty();
    }

    protected virtual void OnScenePathPropertyChanged(string[] scenePath) {
        transform.parent = Utils.ResolveScenePath(scenePath);
        Debug.Assert((scenePath.Length == 0) == (transform.parent == null));
    }

    protected virtual void OnTransformParentChanged() {
        UpdateScenePathProperty();
    }

    private void UpdateScenePathProperty() {
        if (!PhotonNetwork.InRoom || !photonView.IsMine) {
            return;
        }

        var newPath = gameObject.GetScenePath(null, false).ToArray();
        Debug.LogWarning(string.Join("/", newPath));
        ScenePath.SetValue(newPath, notifyLocal: true, notifyRemote: true);
    }
}
