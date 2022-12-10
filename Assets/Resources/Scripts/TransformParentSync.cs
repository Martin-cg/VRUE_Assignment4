using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TransformParentSync : SynchronizedRoomObject {
    private SynchronizedRoomProperty<string[]> ScenePath;

    protected virtual void Awake() {
        var key = $"{nameof(TransformParentSync)}-{photonView.ViewID}";
        ScenePath = RegisterProperty<string[]>("ScenePath", null, key);
        ScenePath.ValueChanged += (s, e) => OnScenePathPropertyChanged(e);
    }

    protected override void Start() {
        base.Start();

        UpdateScenePathProperty();
    }

    protected virtual void OnScenePathPropertyChanged(string[] scenePath) {
        Debug.LogWarning(string.Join("/", scenePath));
        transform.parent = Utils.FindObjectByScenePath(scenePath)?.transform;
    }

    protected virtual void OnTransformParentChanged() {
        UpdateScenePathProperty();
    }

    private void UpdateScenePathProperty() {
        if (!PhotonNetwork.InRoom || !photonView.IsMine) {
            return;
        }

        var path = gameObject.GetScenePathString(null, false);
        Debug.LogError(path);
        ScenePath.SetValue(gameObject.GetScenePath(null, false).ToArray(), notifyLocal: false, notifyRemote: true);
    }
}
