using UnityEngine;

public class ChoppingBoard : RigidbodyContainer {
    protected override Pose CaptureAttachPose(ContainedObject obj) {
        var pose = base.CaptureAttachPose(obj);

        var direction = transform.position - obj.GameObject.transform.position;
        if (obj.Rigidbody.SweepTest(direction, out var hit)) {
            pose.position = transform.InverseTransformPoint(hit.point);
        }

        return pose;
    }
}
