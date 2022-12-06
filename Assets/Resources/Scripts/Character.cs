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
    public static IEnumerable<Character> Players => All.Where(c => c.Role == CharacterRole.Player);
    public static IEnumerable<Character> Spectators => All.Where(c => c.Role == CharacterRole.Spectator);

    public Player Player { get; private set; }
    public bool IsLocal => Player.IsLocal;
    public CharacterRole? Role => Player.CustomProperties.TryGetValue(nameof(Role), out var value) ? (CharacterRole) value : null;

    public GameObject Root;
    public GameObject Head;
    public GameObject LeftHand;
    public GameObject RightHand;

    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        var instantiationData = (InstantiationData) info.photonView.InstantiationData[0];
        Player = PhotonNetwork.LocalPlayer.Get(instantiationData.ActorNumber);
        Player.TagObject = this;
        SetRole(CharacterRole.Undefined);
    }

    public void SetRole(CharacterRole newRole) {
        Player.SetCustomProperties(new Hashtable() { [nameof(Role)] = newRole });
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

public enum CharacterRole {
    Player,
    Spectator,
    Undefined
}
