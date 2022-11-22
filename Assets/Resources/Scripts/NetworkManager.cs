using Photon.Pun;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks {
    public GameObject PlayerPrefab;
    public LaneManager LaneManager;

    private void Start() {
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

        var playerObject = PhotonNetwork.Instantiate($"Prefabs/{PlayerPrefab.name}", default, default);
        playerObject.name = "Local Player";
        var character = playerObject.GetComponent<Character>();
        character.ActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        character.XRRig = GameObject.FindGameObjectWithTag(Tags.XROrigin).GetComponent<XRRig>();
        PhotonNetwork.LocalPlayer.TagObject = character;

        if (PhotonNetwork.IsMasterClient) {
            LaneManager.Setup();
            var lane = LaneManager.AssignLane(PhotonNetwork.LocalPlayer);
            character.SpawnAtLane(lane);
        }
    }

    public override void OnLeftRoom() {
        base.OnLeftRoom();

        if (PhotonNetwork.LocalPlayer.ActorNumber != -1) {
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
        base.OnPlayerEnteredRoom(newPlayer);

        if (PhotonNetwork.IsMasterClient) {
            var lane = LaneManager.AssignLane(newPlayer);
            photonView.RPC(nameof(SetSpawn), newPlayer, lane.LaneNumber);
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
        base.OnPlayerLeftRoom(otherPlayer);

        if (otherPlayer.ActorNumber == -1) {
            return;
        }

        if (PhotonNetwork.IsMasterClient) {
            LaneManager.UnassignLane(otherPlayer);
            PhotonNetwork.DestroyPlayerObjects(otherPlayer);
        }
    }

    [PunRPC] public void SetSpawn(int laneNumber) {
        var character = PhotonNetwork.LocalPlayer.TagObject as Character;
        var lane = LaneManager.Lanes[laneNumber];
        character.SpawnAtLane(lane);
    }
}
