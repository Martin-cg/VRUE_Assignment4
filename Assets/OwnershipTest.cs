using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class OwnershipTest : MonoBehaviourPun {
    private static Dictionary<int, Color> Colors = new();

    private void Start() {
        Colors.Add(0, Color.red);
        Colors.Add(1, Color.blue);
        Colors.Add(2, Color.green);
        Colors.Add(3, Color.yellow);
    }

    private void Update() {
        this.GetComponent<Renderer>().material.color = Colors.GetOrAddWith(photonView.OwnerActorNr, k => {
            var random = new System.Random(k);
            return new Color(random.NextFloat(), random.NextFloat(), random.NextFloat());
        });
    }
}
