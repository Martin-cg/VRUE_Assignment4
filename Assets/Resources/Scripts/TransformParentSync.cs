using Photon.Pun;
using System.Linq;
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
        transform.parent = Utils.FindObjectByScenePath(scenePath).transform;
    }

    protected virtual void OnTransformParentChanged() {
        UpdateScenePathProperty();
    }

    private void UpdateScenePathProperty() {
        if (!PhotonNetwork.InRoom || !photonView.IsMine) {
            return;
        }

        ScenePath.Value = gameObject.GetScenePath(null, false).ToArray();
    }
}
