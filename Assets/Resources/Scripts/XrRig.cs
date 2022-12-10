using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

[DisallowMultipleComponent]
public class XRRig : MonoBehaviour {
    public TrackedPoseDriver Head;
    public ActionBasedController RightHand;
    public ActionBasedController LeftHand;
}
