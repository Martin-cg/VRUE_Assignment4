using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks {
    public GameObject CharacterPrefab;
    public Transform Spawn;
    private GameObject LocalOfflineCharacter;

    private void Start() {
        PhotonPeer.RegisterType(typeof(Character.InstantiationData), 0, Character.InstantiationData.Serialize, Character.InstantiationData.Deserialize);
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.ConnectUsingSettings()) {
            Debug.LogError("Failed to connect using settings.");
        }

        // Create a new Character instance so that the user can see his hands and stuff until we connect.
        LocalOfflineCharacter = Instantiate(CharacterPrefab, Spawn.position, Spawn.rotation);
        LocalOfflineCharacter.name = "Local Offline Player";
        var character = LocalOfflineCharacter.AddComponent<LocalCharacter>();
        character.Rig = GameObject.FindGameObjectWithTag(Tags.XROrigin).GetComponent<XRRig>();
    }

    public override void OnConnectedToMaster() {
        base.OnConnectedToMaster();

        if (!PhotonNetwork.JoinRandomOrCreateRoom()) {
            Debug.LogError("[Network] Failed to create or join default room.");
        }
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();

        DestroyImmediate(LocalOfflineCharacter);
        var characterObject = PhotonNetwork.Instantiate($"Prefabs/{CharacterPrefab.name}", Spawn.position, Spawn.rotation, default, new object[] {
            new Character.InstantiationData() { ActorNumber = PhotonNetwork.LocalPlayer.ActorNumber }
        });
        characterObject.name = "Local Player";
        var character = characterObject.AddComponent<LocalCharacter>();
        character.Rig = GameObject.FindGameObjectWithTag(Tags.XROrigin).GetComponent<XRRig>();
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
