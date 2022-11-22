using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
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
        var path = GetGameObjectPath(parent.gameObject, photonView.gameObject, false, false);
        return new object[] { photonView.ViewID, path };
    }

    private static string GetGameObjectPath(GameObject target, GameObject parent = null, bool includeTarget=true, bool includeParent=true) {
        if (ReferenceEquals(target, parent)) {
            return "";
        }

        var path = new List<string>();

        if (includeTarget) {
            path.Add(target.name);
        }

        var current = target.transform;
        while (current.parent != null && ReferenceEquals(current, parent)) {
            path.Add(current.name);
            current = current.transform.parent;
        }

        if (parent && includeParent) {
            path.Add(parent.name);
        }

        return string.Join("/", Enumerable.Reverse(path));
    }
}
