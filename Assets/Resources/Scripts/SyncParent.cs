using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class SyncParent : MonoBehaviourPunCallbacks {
    private readonly Hashtable PropertyTableCacheOld = new();
    private readonly Hashtable PropertyTableCacheCurrent = new();
    private string ScenePath;
    private string PropertyKey;

    protected virtual void Awake() {
        PropertyKey = $"{nameof(SyncParent)}-{photonView.ViewID}";
    }

    protected virtual void Start() {
        UpdateRoomProperty();
    }

    protected virtual void OnTransformParentChanged() {
        UpdateRoomProperty();
    }

    private void UpdateRoomProperty() {
        if (!PhotonNetwork.InRoom || !photonView.IsMine) {
            return;
        }

        ScenePath = gameObject.GetScenePathString();
        PropertyTableCacheCurrent[PropertyKey] = ScenePath;
        if (PhotonNetwork.CurrentRoom.SetCustomProperties(PropertyTableCacheCurrent, PropertyTableCacheOld)) {
            PropertyTableCacheOld[PropertyKey] = ScenePath;
        } else {
            Debug.LogError("Failed to update custom properties", this);
        }
    }

}
