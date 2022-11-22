using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class LaneManager : MonoBehaviour, IPunObservable {
    public Lane[] Lanes;
    private Dictionary<int, int> LaneAssignment = new();

    private void OnValidate() {
        if (Lanes != null) {
            for (int i = 0; i < Lanes.Length; i++) {
                if (Lanes[i] != null) {
                    Lanes[i].Manager = this;
                    Lanes[i].LaneNumber = i;
                }
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsReading) {
            LaneAssignment = (Dictionary<int, int>) stream.ReceiveNext();
        } else {
            stream.SendNext(LaneAssignment);
        }
    }

    public Lane AssignLane(Player player) {
        var players = PhotonNetwork.PlayerList;
        var activeActorNumbers = new HashSet<int>();
        foreach (var p in players) {
            activeActorNumbers.Add(p.ActorNumber);
        }

        for (var i = 0; i < Lanes.Length; i++) {
            if (LaneAssignment.TryGetValue(i, out var num)) {
                if (!activeActorNumbers.Contains(num)) {
                    LaneAssignment.Remove(i);
                } else {
                    continue;
                }
            }

            Debug.Log($"[LaneManager] Assigned lane {i} to actor {player.ActorNumber}");
            LaneAssignment.Add(i, player.ActorNumber);
            return Lanes[i];
        }

        return null;
    }

    public void UnassignLane(Player player) {
        if (LaneAssignment.ContainsKey(player.ActorNumber)) {
            Debug.Log($"[LaneManager] Unassigned lane {LaneAssignment[player.ActorNumber]} from actor {player.ActorNumber}");
            LaneAssignment.Remove(player.ActorNumber);
        }
    }

    public void Setup() {
        var seed = Utils.RandomSeed();

        foreach (var lane in Lanes) {
            lane.GetComponentInChildren<ObstacleGenerator>()?.Randomize(seed);
        }
    }
}
