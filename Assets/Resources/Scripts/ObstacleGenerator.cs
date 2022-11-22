using Photon.Pun;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ObstacleGenerator : RandomizableObject {
    private string[] ObstaclePrefabNames;
    public GameObject[] ObstaclePrefabs;
    public float[] ObstacleProbabilities;

    public Vector3 LaneStart;
    public Vector3 LaneEnd;
    public float LaneWidth;
    public float ObstacleSpacing;

    public void Start() {
        if (ObstaclePrefabs.Length != ObstacleProbabilities.Length) {
            Debug.LogError("Obstacle data has incompatible lengths.", this);
        }
        ObstaclePrefabNames = ObstaclePrefabs.Select(prefab => $"Prefabs/{prefab.name}").ToArray();
    }

    public void Clear() {
        if (Application.isPlaying) {
            foreach (Transform child in transform) {
                if (PhotonNetwork.IsConnected) {
                    PhotonNetwork.Destroy(child.gameObject);
                } else {
                    child.gameObject.SetActive(false);
                    GameObject.Destroy(child.gameObject);
                }
            }
        } else {
            while (transform.childCount > 0) {
                GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
    }

    private (GameObject, string) GetRandomObstacle(System.Random random) {
        var obstacleProbabilityIndex = random.NextFloat(0f, 1f);
        for (var i = 0; i < ObstaclePrefabs.Length; i++) {
            obstacleProbabilityIndex -= ObstacleProbabilities[i];
            if (obstacleProbabilityIndex < 0f) {
                return (ObstaclePrefabs[i], ObstaclePrefabNames[i]);
            }
        }
        throw new System.Exception("Illegal state reached");
    }

    public override void Randomize(System.Random random) {
        Clear();

        var lane = LaneEnd - LaneStart;
        var direction = lane.normalized;
        var length = lane.magnitude;
        var rotation = Quaternion.LookRotation(direction, transform.up);

        var distance = 0f;
        while (true) {
            var (obstaclePrefab, obstacleName) = GetRandomObstacle(random);
            var obstacleWidth = obstaclePrefab.transform.localScale.x;
            var obstacleHeight = obstaclePrefab.transform.localScale.y;
            var obstacleLength = obstaclePrefab.transform.localScale.z;

            distance += obstacleLength / 2;
            if (distance + obstacleLength / 2 > length) {
                break;
            }

            var currentPosition = LaneStart + direction * distance + transform.up * obstacleHeight / 2;
            var obstacle = PhotonNetwork.InstantiateRoomObject(obstacleName, currentPosition, rotation, 0, PhotonInstantiateParenter.GetInstantiateParameters(transform));
            // obstacle.transform.SetParent(transform, false);
            obstacle.transform.localPosition = currentPosition;
            obstacle.transform.localRotation = rotation;

            foreach (var o in obstacle.GetComponentsInChildren<RandomizableObject>()) {
                o.Randomize(random);
            }

            distance += obstacleLength / 2 + ObstacleSpacing;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position + LaneStart, 0.5f);
        Gizmos.DrawLine(transform.position + LaneStart, transform.position + LaneEnd);
        Gizmos.DrawWireSphere(transform.position + LaneEnd, 0.5f);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ObstacleGenerator))]
public class ObstacleGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        var target = this.target as ObstacleGenerator;

        if (GUILayout.Button("Clear")) {
            target.Clear();
        }
        if (GUILayout.Button("Generate")) {
            target.Randomize();
        }
    }
}
#endif
