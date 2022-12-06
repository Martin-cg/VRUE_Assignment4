using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks {
    private void Start() {
        PhotonPeer.RegisterType(typeof(Character.InstantiationData), 0, Character.InstantiationData.Serialize, Character.InstantiationData.Deserialize);
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.ConnectUsingSettings()) {
            Debug.LogError("Failed to connect using settings.");
        }
    }

    public override void OnConnectedToMaster() {
        base.OnConnectedToMaster();

        if (!PhotonNetwork.JoinRandomOrCreateRoom()) {
            Debug.LogError("[Network] Failed to create or join default room.");
        }
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();

        CharacterManager.Instance.SpawnLocalCharacter();
    }

    public override void OnLeftRoom() {
        base.OnLeftRoom();

        if (PhotonNetwork.LocalPlayer.ActorNumber != -1) {
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
        base.OnPlayerLeftRoom(otherPlayer);

        if (otherPlayer.ActorNumber == -1) {
            return;
        }

        if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.DestroyPlayerObjects(otherPlayer);
        }
    }
}
