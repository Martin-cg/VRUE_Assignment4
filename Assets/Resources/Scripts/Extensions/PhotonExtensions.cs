using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public static class PhotonExtensions {
    public static T ReceiveNext<T>(this PhotonStream stream) => (T)stream.ReceiveNext();
}
