using Photon.Pun;
using UnityEngine;

public class PhotonInstantiateParenter : MonoBehaviourPun, IPunInstantiateMagicCallback {
    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        var instantiationData = info.photonView.InstantiationData;
        var parent = PhotonView.Find((int)instantiationData[0]).transform;
        var path = (string)instantiationData[1];
        foreach (var name in path.Split("/")) {
            parent = parent.Find(name);
        }
        transform.SetParent(parent, false);
    }

    public static object[] GetInstantiateParameters(Transform parent) {
        var photonView = PhotonView.Get(parent);
        var path = parent.gameObject.GetScenePathString(photonView.gameObject, false, false, "/");
        return new object[] { photonView.ViewID, path };
    }
}
