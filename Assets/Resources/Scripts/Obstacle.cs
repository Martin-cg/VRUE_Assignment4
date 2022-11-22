using UnityEditor;
using UnityEngine;

public class Obstacle : RandomizableObject {
    public GameObject LeftWall;
    public GameObject RightWall;
    public float OpeningWidth;
    public float MinWallWidth;

    public override void Randomize(System.Random random) {
        var width = 1f;
        var opening = random.NextFloat(OpeningWidth / 2, width - OpeningWidth / 2);
        var leftWidth = opening - OpeningWidth / 2f;
        if (leftWidth < MinWallWidth) {
            LeftWall.SetActive(false);
        }
        var rightWidth = width - opening - OpeningWidth / 2f;
        if (rightWidth < MinWallWidth) {
            RightWall.SetActive(false);
        }

        LeftWall.transform.localScale = new Vector3(leftWidth, 1, 1);
        LeftWall.transform.localPosition = new Vector3((-width + leftWidth) / 2, LeftWall.transform.localPosition.y, LeftWall.transform.localPosition.z);
        RightWall.transform.localScale = new Vector3(rightWidth, 1, 1);
        RightWall.transform.localPosition = new Vector3((width - rightWidth) / 2, RightWall.transform.localPosition.y, RightWall.transform.localPosition.z);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Obstacle))]
public class ObstacleEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        var target = this.target as Obstacle;

        if (GUILayout.Button("Randomize")) {
            target.Randomize();
        }
    }
}
#endif
