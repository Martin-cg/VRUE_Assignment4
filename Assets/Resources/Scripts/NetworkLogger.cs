using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class NetworkLogger : MonoBehaviourPunCallbacks {
    public override void OnConnected() {
        Debug.Log("[Network] Connected");
    }

    public override void OnConnectedToMaster() {
        Debug.Log("[Network] Connected to master");
    }

    public override void OnCustomAuthenticationFailed(string debugMessage) {
        Debug.LogError($"[Network] Custom authentication failed: {debugMessage}");
    }

    public override void OnDisconnected(DisconnectCause cause) {
        Debug.Log($"[Network] Disconnected (cause={cause})");
    }

    public override void OnRegionListReceived(RegionHandler regionHandler) {
        Debug.Log("[Network] Region List Received");
    }

    public override void OnJoinedLobby() {
        Debug.Log("[Network] Joined Lobby");
    }

    public override void OnLeftLobby() {
        Debug.Log("[Network] Left Lobby");
    }

    public override void OnJoinedRoom() {
        Debug.Log("[Network] Joined Room");
    }

    public override void OnLeftRoom() {
        Debug.Log("[Network] Left Room");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        Debug.Log("[Network] Room List updated");
    }

    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics) {
        Debug.Log("[Network] Lobby statistics updated");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
        Debug.Log($"[Network] Player {newPlayer.NickName} entered Room");
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
        Debug.Log($"[Network] Player {otherPlayer.NickName} left Room");
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) {
        Debug.Log("[Network] Room properties updated");
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps) {
        Debug.Log($"[Network] Player {targetPlayer.NickName} properties updated");
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient) {
        Debug.Log("[Network] Master client switched");
    }
}
