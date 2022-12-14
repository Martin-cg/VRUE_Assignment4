using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;

public class ConnectButton : PredefinedActionButton, IConnectionCallbacks {
    public NetworkManager NetworkManager;
    public TMP_Text ButtonText;

    protected override void Reset() {
        base.Reset();

        FindButtonText(true);
    }

    protected override void OnValidate() {
        base.OnValidate();

        FindButtonText(true);
    }

    protected override void Awake() {
        base.Awake();

        FindButtonText(false);
    }

    public virtual void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);
        UpdateButtonText();
    }

    public virtual void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void FindButtonText(bool ignoreCurrent) {
        if (ignoreCurrent || ButtonText == null) {
            ButtonText = Button.GetComponentInChildren<TMP_Text>();
        }
    }

    protected override void OnButtonClicked() {
        if (PhotonNetwork.IsConnected) {
            NetworkManager.Disconnect();
            UpdateButtonText(false);
        } else {
            NetworkManager.Connect();
            UpdateButtonText(true);
        }
    }

    private void UpdateButtonText(bool? connected = null) {
        if (ButtonText) {
            bool state = connected ?? PhotonNetwork.IsConnected;
            if (state) {
                ButtonText.text = "Disconnect";
            } else {
                ButtonText.text = "Connect";
            }
        }
    }

    public void OnConnected() {
        UpdateButtonText();
    }

    public void OnConnectedToMaster() {
    }

    public void OnDisconnected(DisconnectCause cause) {
        UpdateButtonText();
    }

    public void OnRegionListReceived(RegionHandler regionHandler) {
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data) {
    }

    public void OnCustomAuthenticationFailed(string debugMessage) {
    }
}
