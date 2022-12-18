using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TransformParentSync : SynchronizedRoomObject {
    private SynchronizedRoomProperty<string[]> ScenePath;
    private Transform LastParent;

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
        Debug.LogWarning("OnScenePathPropertyChanged(): " + name + " " + string.Join(" /", scenePath));
        transform.parent = Utils.ResolveScenePath(scenePath);
        LastParent = transform.parent;
    }


    protected virtual void Update() {
        // OnTransformParentChanged() is not triggered consitently, so we do this instead.
        if (photonView.IsMine && LastParent != transform.parent) {
            LastParent = transform.parent;
            UpdateScenePathProperty();
        }
    }
    /*
    protected virtual void OnTransformParentChanged() {
        UpdateScenePathProperty();
    }
    */

    private void UpdateScenePathProperty() {
        if (!PhotonNetwork.InRoom || !photonView.IsMine) {
            return;
        }

        var newPath = gameObject.GetScenePath(null, false).ToArray();
        Debug.LogWarning("UpdateScenePathProperty(): " + name + " " + string.Join(" /", newPath));
        ScenePath.SetValue(newPath, notifyLocal: false, notifyRemote: true);
    }
}
