using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class OwnershipTest : MonoBehaviourPun {
    private static Dictionary<int, Color> Colors = new();

    void Update() {
        this.GetComponent<Renderer>().material.color = Colors.GetOrAddWith(photonView.OwnerActorNr, k => {
            var random = new System.Random(k);
            return new Color(random.NextFloat(), random.NextFloat(), random.NextFloat());
        });
    }
}
