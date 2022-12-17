﻿using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public static class PhotonSerdeUtils {
    public static (List<string>, PhotonView) GetPhotonViewRelativeScenePath(GameObject target) {
        var photonView = PhotonView.Get(target);
        var path = target.GetScenePath(photonView.gameObject, true, true);
        return (path, photonView);
    }
    public static GameObject ResolvePhotonViewRelativeScenePath(int sceneViewId, IEnumerable<string> path) =>
        ResolvePhotonViewRelativeScenePath(PhotonView.Find(sceneViewId), path);
    public static GameObject ResolvePhotonViewRelativeScenePath(PhotonView view, IEnumerable<string> path) =>
        Utils.ResolveScenePath(path, view.gameObject);
}
