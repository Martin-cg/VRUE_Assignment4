using UnityEngine;
using Photon.Realtime;
using System.Collections.Generic;
using Photon.Pun;
using UnityEditor;

public class ExitButton : PredefinedActionButton, IConnectionCallbacks {
    public bool DisconnectBeforeQuit = true;
    public NetworkManager NetworkManager;
    private bool QuitOnNextDisconnect = false;

    public virtual void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public virtual void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    protected override void OnButtonClicked() {
        if (DisconnectBeforeQuit && PhotonNetwork.IsConnected) {
            QuitOnNextDisconnect = true;
            NetworkManager.Disconnect();
        } else {
            Application.Quit();
        }
    }

    public void OnConnected() {
    }

    public void OnConnectedToMaster() {
    }

    public void OnCustomAuthenticationFailed(string debugMessage) {
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data) {
    }

    public void OnDisconnected(DisconnectCause cause) {
        if (QuitOnNextDisconnect) {
            QuitOnNextDisconnect = false;
            if (Application.isPlaying) {
                Application.Quit();
            } else {
                EditorApplication.isPlaying = false;
            }
        }
    }

    public void OnRegionListReceived(RegionHandler regionHandler) {
    }
}
