using UnityEditor;
using UnityEngine;

public class MovingObstacle : RandomizableObject {
    public GameObject Wall;
    public float CycleTime;
    public float Speed => (MaxWallPosition - MinWallPosition)/CycleTime;
    private float WallWidth => Wall.transform.localScale.x;
    private float WallPosition {
        get => Wall.transform.localPosition.x;
        set => Wall.transform.localPosition = Wall.transform.localPosition.WithX(value);
    }
    private float MinWallPosition => (-1 + WallWidth) / 2;
    private float MaxWallPosition => (1 - WallWidth) / 2;
    private float WallMoveDirection;

    public override void Randomize(System.Random random) {
        WallPosition = random.NextFloat(MinWallPosition, MaxWallPosition);
    }

    public void FixedUpdate() {
        var delta = (Speed * Time.fixedDeltaTime) * Mathf.Sign(WallMoveDirection);
        var newWallPosition = WallPosition + delta;
        if (newWallPosition > MaxWallPosition) {
            var overshoot = newWallPosition - MaxWallPosition;
            newWallPosition = MaxWallPosition - overshoot;
            WallMoveDirection = -1;
        } else if (newWallPosition < MinWallPosition) {
            var overshoot = MinWallPosition - newWallPosition;
            newWallPosition = MinWallPosition + overshoot;
            WallMoveDirection = +1;
        }
        WallPosition = newWallPosition;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MovingObstacle))]
public class MovingObstacleEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        var target = this.target as MovingObstacle;

        if (GUILayout.Button("Randomize")) {
            target.Randomize();
        }
    }
}
#endif
