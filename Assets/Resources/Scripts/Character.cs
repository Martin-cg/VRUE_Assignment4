using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using System.Runtime.InteropServices;

public class Character : MonoBehaviour, IPunInstantiateMagicCallback {
    public static Character Local => ForPlayer(PhotonNetwork.LocalPlayer);
    public static Character ForPlayer(Player player) => player.TagObject as Character;
    public static IEnumerable<Character> All => PhotonNetwork.PlayerList.Select(ForPlayer);
    public static IEnumerable<Character> Others => PhotonNetwork.PlayerListOthers.Select(ForPlayer);

    public Player Player { get; private set; }
    public bool IsLocal => Player.IsLocal;

    public GameObject Root;
    public GameObject Head;
    public GameObject LeftHand;
    public GameObject RightHand;

    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        var instantiationData = (InstantiationData) info.photonView.InstantiationData[0];
        Player = PhotonNetwork.LocalPlayer.Get(instantiationData.ActorNumber);
        Player.TagObject = this;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InstantiationData  {
        public int ActorNumber;

        public static short Serialize(StreamBuffer outStream, object customObject) {
            return (short) Serde.Serialize(outStream, (InstantiationData) customObject);
        }

        public static object Deserialize(StreamBuffer inStream, short length) {
            Debug.Assert(length == Marshal.SizeOf(typeof(InstantiationData)));
            return Serde.Deserialize<InstantiationData>(inStream);
        }
    }
}
