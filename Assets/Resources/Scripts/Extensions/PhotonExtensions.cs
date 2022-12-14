using Photon.Pun;

public static class PhotonExtensions {
    public static T ReceiveNext<T>(this PhotonStream stream) => (T)stream.ReceiveNext();
}
