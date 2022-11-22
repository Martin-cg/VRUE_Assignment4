using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
[RequireComponent(typeof(PhotonView))]
public class ButtonController : MonoBehaviourPun, IPunObservable {
    public UnityEvent<bool> StateChanged = new();
    private bool isPressed = false;
    public bool IsPressed {
        get => isPressed;
        set {
            var changed = isPressed != value;
            isPressed = value;
            if (changed) {
                if (!PhotonNetwork.IsMasterClient) {
                    photonView.RPC(nameof(SetState), RpcTarget.Others, isPressed);
                }
                StateChanged?.Invoke(isPressed);
            }
        }
    }
    public bool IsSwitch = true;
    private float LastPressed;

    public void OnPressed() {
        if (Time.time - LastPressed <= 0.2) {
            return;
        }
        LastPressed = Time.time;

        if (IsSwitch) {
            IsPressed = !IsPressed;
        } else {
            IsPressed = true;
        }
    }

    public void OnReleased() {
        if (IsSwitch) {
        } else {
            IsPressed = false;
        }
    }

    [PunRPC]
    public void SetState(bool state) {
        IsPressed = state;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsReading) {
            IsPressed = ((int) stream.ReceiveNext()) == 1;
        } else {
            stream.SendNext(IsPressed ? 1 : 0);
        }
    }
}
