using Photon.Pun;
using TMPro;
using UnityEngine;

public class TextSync : MonoBehaviour, IPunObservable {
    public TMP_Text Text;

    void Start() {
        Text = GetComponent<TMP_Text>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsReading) {
            Text.text = (string)stream.ReceiveNext();
            Text.fontSize = (float)stream.ReceiveNext();
        } else {
            stream.SendNext(Text.text);
            stream.SendNext(Text.fontSize);
        }
    }
}
