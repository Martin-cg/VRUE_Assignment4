using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TransformParentSync : MonoBehaviourPun, IPunObservable {
    private Transform LastParent;
    private string[] LastPath;

    protected virtual void OnRemoteScenePathChanged(string[] scenePath) {
        transform.parent = Utils.ResolveScenePath(scenePath);
        Debug.Assert((scenePath.Length == 0) == (transform.parent == null));
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsReading) {
            if (LastParent != transform.parent) {
                LastParent = transform.parent;
                LastPath = gameObject.GetScenePath(null, false).ToArray();
            }
            stream.SendNext(LastPath);
        } else {
            var newPath = stream.ReceiveNext<string[]>();
            OnRemoteScenePathChanged(newPath);
            LastPath = newPath;
            LastParent = transform.parent;
        }
    }
}
